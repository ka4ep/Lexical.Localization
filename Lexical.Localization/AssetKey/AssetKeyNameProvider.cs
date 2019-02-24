// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           31.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// A name policy implementation that builds names of <see cref="IAssetKey"/>. 
    /// 
    /// Non-canonical parts are appended in the beginning of the string. Order of non-canonical parameters uses string ordinal comparer, making "culture" parameter typically first.
    /// Example: key.Section("x").Key("y").SetCulture("fi") builds into string "fi:x:y".
    /// 
    /// Canonical parts are concatenated in canonical order from root to tail.
    /// Example: key.Key("x").Section("y").Key("z") builds into string "x:y:z".
    /// 
    /// This implementation uses <see cref="IAssetKeyParametrizer"/> to extract parameters from key parts.
    /// 
    /// If parameter value is "", then it is considered as non-existing and will not be appended.
    /// </summary>
    public class AssetKeyNameProvider : IAssetKeyNameDescription, IAssetKeyNameProvider, ICloneable
    {
        private static readonly AssetKeyNameProvider colon_colon_colon = new AssetKeyNameProvider().SetDefault(true, ":", "");
        private static readonly AssetKeyNameProvider colon_colon_dot = new AssetKeyNameProvider().SetDefault(true, ":", "").SetParameter("key", true, ".", "");
        private static readonly AssetKeyNameProvider none_colon_colon = new AssetKeyNameProvider().SetDefault(true, ":", "").SetNonCanonicalDefault(false);
        private static readonly AssetKeyNameProvider dot_dot_dot = new AssetKeyNameProvider().SetDefault(true, ".", "");
        private static readonly AssetKeyNameProvider colon_dot_dot = new AssetKeyNameProvider().SetNonCanonicalDefault(true, "", ":").SetCanonicalDefault(true, ".", "");
        private static readonly AssetKeyNameProvider none_dot_dot = new AssetKeyNameProvider().SetNonCanonicalDefault(false).SetCanonicalDefault(true, ".", "");
        private static readonly AssetKeyNameProvider _default = new AssetKeyNameProvider().SetParameter("culture", true, "", ":").SetNonCanonicalDefault(false).SetDefault(true, ":", "");

        /// <summary>
        /// Default name policy for language strings matching. Suitable for language strings policy of asset loader, but not suitable for filename matching.
        /// 
        /// Appends "culture:" non-canonical parameter, but not other non-canonicals.
        /// 
        /// The section and key separators are ":".
        /// 
        /// Example "en:ConsoleApp1:MyController:Success".
        /// </summary>
        public static AssetKeyNameProvider Default => _default;

        /// Name policy where every separator is ":".
        /// 
        /// Example "en:ConsoleApp1:MyController:Success".
        /// </summary>
        public static AssetKeyNameProvider Colon_Colon_Colon => colon_colon_colon;

        /// Name policy where every canonical separator is ":", and non-canonical (e.g. culture) is not appeded.
        /// 
        /// Example "ConsoleApp1:MyController:Success".
        /// </summary>
        public static AssetKeyNameProvider None_Colon_Colon => none_colon_colon;

        /// Name policy where every separator is ":", except for "key" that has "." separtor.
        /// 
        /// Example "en:ConsoleApp1:MyController.Success".
        /// </summary>
        public static AssetKeyNameProvider Colon_Colon_Dot => colon_colon_dot;

        /// <summary>
        /// Name policy where every separator is ".".
        /// 
        /// Example "en.ConsoleApp1.MyController.Success"
        /// </summary>
        public static AssetKeyNameProvider Dot_Dot_Dot => dot_dot_dot;

        /// <summary>
        /// Name policy where non-canonical parts are not written out, and canonical parts have "." as separator.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static AssetKeyNameProvider None_Dot_Dot => none_dot_dot;

        /// <summary>
        /// Name policy where non-canonical parts have ":", and canonical parts "." as separator.
        /// 
        /// Example "en:ConsoleApp1.MyController.Success".
        /// </summary>
        public static AssetKeyNameProvider Colon_Dot_Dot => colon_dot_dot;

        Dictionary<string, AssetKeyParameterDescription> parameters = new Dictionary<string, AssetKeyParameterDescription>();
        
        /// <summary>
        /// Get indexed parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAssetKeyParameterDescription this[string name] { get { AssetKeyParameterDescription result = null; parameters.TryGetValue(name, out result); return result; } }

        /// <summary>
        /// Get all parametres
        /// </summary>
        public IEnumerable<IAssetKeyParameterDescription> Parameters => parameters.Values;

        /// <summary>
        /// Construct a new name provider with no rules. 
        /// Use chain methods to add some rules.
        /// </summary>
        public AssetKeyNameProvider()
        {
        }

        /// <summary>
        /// Set rule to not include a specific type of parameter, for example "culture".
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns>this</returns>
        public AssetKeyNameProvider DontInclude(string parameterName)
        {
            parameters[parameterName] = new AssetKeyParameterDescription(parameterName, null, null, false);
            return this;
        }

        /// <summary>
        /// Set rule for a parameter type.
        /// </summary>
        /// <param name="parameterName">parameter name, for example "culture", "section", "key", etc</param>
        /// <param name="isIncluded">true, parameter is included in name. False, parameter is not to be included. </param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public AssetKeyNameProvider SetParameter(string parameterName, bool isIncluded, string prefixSeparator = "", string postfixSeparator = "")
        {
            parameters[parameterName] = new AssetKeyParameterDescription(parameterName, prefixSeparator, postfixSeparator, isIncluded);
            return this;
        }

        /// <summary>
        /// Set default rule for canonical key parts. Applied if there is no parameter specific explicit rule.
        /// </summary>
        /// <param name="isIncluded">are canonical parts to be included</param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public AssetKeyNameProvider SetCanonicalDefault(bool isIncluded, string prefixSeparator = "", string postfixSeparator = "")
        {
            parameters["canonical"] = new AssetKeyParameterDescription("canonical", prefixSeparator, postfixSeparator, isIncluded);
            return this;
        }

        /// <summary>
        /// Set default rule for non-canonical parts. Applied if there is no parameter specific explicit rule.
        /// </summary>
        /// <param name="isIncluded"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public AssetKeyNameProvider SetNonCanonicalDefault(bool isIncluded, string prefixSeparator = "", string postfixSeparator = "")
        {
            parameters["noncanonical"] = new AssetKeyParameterDescription("noncanonical", prefixSeparator, postfixSeparator, isIncluded);
            return this;
        }

        /// <summary>
        /// Set default rule. Applied if there are no other applying rules.
        /// </summary>
        /// <param name="isIncluded"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public AssetKeyNameProvider SetDefault(bool isIncluded, string prefixSeparator = "", string postfixSeparator = "")
        {
            parameters[""] = new AssetKeyParameterDescription("", prefixSeparator, postfixSeparator, isIncluded);
            return this;
        }

        /// <summary>
        /// Build key into a string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string BuildName(IAssetKey key)
        {
            // Build name without string builder, without reallocating buffers, and (most of the time) without any other heap objects.
            // A bit of performance makes it looks messy. Tradeoffs.

            // Calculate length
            int length = 0;

            string pendingSeparator = null;
            LazyList<string> noncanonicalParameters = new LazyList<string>();
            // Calculate canonicals
            for (IAssetKey part = key; part != null; part = part.GetPreviousKey())
            {
                if (part is IAssetKeyCanonicallyCompared == false) continue;
                if (part is IAssetKeyParametrized parametrized)
                {
                    // Read parameter name and value
                    string parameterName = parametrized.ParameterName, parameterValue = part.Name;
                    if (string.IsNullOrEmpty(parameterValue)) continue;

                    // Read description
                    AssetKeyParameterDescription desc = null;
                    if (parameters.TryGetValue(parameterName, out desc) && !desc.IsIncluded) continue;

                    // Try default descriptions
                    if (desc == null) parameters.TryGetValue("canonical", out desc);
                    if (desc == null) parameters.TryGetValue("", out desc);

                    // No description
                    if (desc == null) continue;

                    // This parameter is disabled
                    if (!desc.IsIncluded) continue;

                    // Add to length
                    if (!string.IsNullOrEmpty(pendingSeparator) && length > 0) length += pendingSeparator.Length;
                    else if (!string.IsNullOrEmpty(desc.PostfixSeparator)) length += desc.PostfixSeparator.Length;
                    length += parameterValue.Length;
                    pendingSeparator = desc.PrefixSeparator;
                }
            }

            // Append non-canonical
            noncanonicalParameters.Clear();
            for (IAssetKey part = key; part != null; part = part.GetPreviousKey())
            {
                if (part is IAssetKeyNonCanonicallyCompared == false) continue;
                if (part is IAssetKeyParametrized parametrized)
                {
                    // Read parameter name and value
                    string parameterName = parametrized.ParameterName, parameterValue = part.Name;
                    if (string.IsNullOrEmpty(parameterValue)) continue;

                    // parameter by this name has already been added
                    if (noncanonicalParameters.Contains(parameterName)) continue;

                    // Is this parameter type included
                    AssetKeyParameterDescription desc = null;
                    if (parameters.TryGetValue(parameterName, out desc) && !desc.IsIncluded) continue;

                    // Try default descriptions
                    if (desc == null) parameters.TryGetValue("noncanonical", out desc);
                    if (desc == null) parameters.TryGetValue("", out desc);

                    // No configuration
                    if (desc == null) continue;

                    // This parameter is disabled
                    if (!desc.IsIncluded) continue;

                    // Add to length
                    if (!string.IsNullOrEmpty(desc.PostfixSeparator) && length > 0) length += desc.PostfixSeparator.Length;
                    else if (!string.IsNullOrEmpty(pendingSeparator) && length > 0) length += pendingSeparator.Length;
                    length += parameterValue.Length;
                    pendingSeparator = desc.PrefixSeparator;
                    noncanonicalParameters.Add(parameterName);
                }
            }

            // Build Name
            char[] dst = new char[length];
            int ix = length;
            pendingSeparator = null;
            // Append canonical
            for (IAssetKey part = key; part != null; part = part.GetPreviousKey())
            {
                if (part is IAssetKeyCanonicallyCompared == false) continue;
                if (part is IAssetKeyParametrized parametrized)
                {
                    // Read parameter name and value
                    string parameterName = parametrized.ParameterName, parameterValue = part.Name;
                    if (string.IsNullOrEmpty(parameterValue)) continue;

                    // Read description
                    AssetKeyParameterDescription desc = null;
                    if (parameters.TryGetValue(parameterName, out desc) && !desc.IsIncluded) continue;

                    // Try default descriptions
                    if (desc == null) parameters.TryGetValue("canonical", out desc);
                    if (desc == null) parameters.TryGetValue("", out desc);

                    // No description
                    if (desc == null) continue;

                    // This parameter is disabled
                    if (!desc.IsIncluded) continue;

                    // Add to length
                    if (!string.IsNullOrEmpty(pendingSeparator) && ix<length) { ix -= pendingSeparator.Length; pendingSeparator.CopyTo(0, dst, ix, pendingSeparator.Length); }
                    else if (!string.IsNullOrEmpty(desc.PostfixSeparator)) { ix -= desc.PostfixSeparator.Length; desc.PostfixSeparator.CopyTo(0, dst, ix, desc.PostfixSeparator.Length); }
                    ix -= parameterValue.Length; parameterValue.CopyTo(0, dst, ix, parameterValue.Length);
                    pendingSeparator = desc.PrefixSeparator;
                }
            }

            // Append non-canonical
            noncanonicalParameters.Clear();
            for (IAssetKey part = key; part != null; part = part.GetPreviousKey())
            {
                if (part is IAssetKeyNonCanonicallyCompared == false) continue;
                if (part is IAssetKeyParametrized parametrized)
                {
                    // Read parameter name and value
                    string parameterName = parametrized.ParameterName, parameterValue = part.Name;
                    if (string.IsNullOrEmpty(parameterValue)) continue;

                    // parameter by this name has already been added
                    if (noncanonicalParameters.Contains(parameterName)) continue;

                    // Is this parameter type included
                    AssetKeyParameterDescription desc = null;
                    if (parameters.TryGetValue(parameterName, out desc) && !desc.IsIncluded) continue;

                    // Try default descriptions
                    if (desc == null) parameters.TryGetValue("noncanonical", out desc);
                    if (desc == null) parameters.TryGetValue("", out desc);

                    // No configuration
                    if (desc == null) continue;

                    // This parameter is disabled
                    if (!desc.IsIncluded) continue;

                    // Add to length
                    if (!string.IsNullOrEmpty(desc.PostfixSeparator) && ix<length) { ix -= desc.PostfixSeparator.Length; desc.PostfixSeparator.CopyTo(0, dst, ix, desc.PostfixSeparator.Length); }
                    else if (!string.IsNullOrEmpty(pendingSeparator)) { ix -= pendingSeparator.Length; pendingSeparator.CopyTo(0, dst, ix, pendingSeparator.Length); }
                    ix -= parameterValue.Length; parameterValue.CopyTo(0, dst, ix, parameterValue.Length);
                    pendingSeparator = desc.PrefixSeparator;
                    noncanonicalParameters.Add(parameterName);
                }
            }

            // Assert everything went ok
            if (ix != 0) throw new AssetKeyException(key as IAssetKey, $"{nameof(AssetKeyNameProvider)}.BuildName failed to build name, ix != 0");

            return new string(dst);
        }

        public object Clone()
        {
            AssetKeyNameProvider result = new AssetKeyNameProvider();
            foreach (var kp in parameters)
                result.parameters[kp.Key] = kp.Value.Clone() as AssetKeyParameterDescription;
            return result;
        }
    }

    public class AssetKeyParameterDescription : IAssetKeyParameterDescription, ICloneable
    {        
        public string ParameterName { get; internal set; }
        public string PrefixSeparator { get; internal set; }
        public string PostfixSeparator { get; internal set; }
        public bool IsIncluded { get; internal set; }

        public AssetKeyParameterDescription(string parameterName, string prefixSeparator, string postfixSeparator, bool isIncluded)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            PrefixSeparator = prefixSeparator ?? "";
            PostfixSeparator = postfixSeparator ?? "";
            IsIncluded = isIncluded;
        }

        public object Clone()
            => new AssetKeyParameterDescription(ParameterName, PrefixSeparator, PostfixSeparator, IsIncluded);
    }
}
