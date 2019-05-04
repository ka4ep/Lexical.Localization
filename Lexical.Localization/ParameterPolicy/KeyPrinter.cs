// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           31.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// A generic configurable name policy that converts <see cref="ILinePart"/> to strings. 
    /// 
    /// Used with localizationa assets such <see cref="LocalizationAsset"/>, where keys are non-parseable strings.
    /// 
    /// Translation policy can be placed for specific parameter names, non-canonical parameters, canonical parameters, and rest. 
    /// 
    /// Parameters are be configured for visibility, separator and sorting order.
    /// 
    /// Parameters with same sort order, are printed out in order of occurance in the key, from left (root) to right (tail).
    /// For example: root.Section("1").Section("2") is printed out as "1.2".
    /// </summary>
    public class KeyPrinter : IParameterPrinter, ICloneable
    {
        /// <summary>
        /// Default name policy. Appends known parameters in <see cref="Utils.ParameterInfos"/> with ":" separator.
        /// 
        /// Example "en:ConsoleApp1:MyController:Success".
        /// </summary>
        public static IParameterPrinter Default => instance;
        private static readonly KeyPrinter instance = 
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ":"); // Add known parameters for sorting correcly

        /// <summary>
        /// Name policy where every separator is ":".
        /// 
        /// Example "en:ConsoleApp1:MyController:Success".
        /// </summary>
        public static IParameterPrinter Colon_Colon_Colon => colon_colon_colon;
        private static readonly KeyPrinter colon_colon_colon =
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ":") // Add known parameters for sorting correctly
                .DefaultRule(true, prefixSeparator: ":"); // Add policy for unknown parameters

        /// <summary>
        /// Name policy where every separator is ":", but culture is not appeded.
        /// 
        /// Example "ConsoleApp1:MyController:Success".
        /// </summary>
        public static IParameterPrinter None_Colon_Colon => none_colon_colon;
        private static readonly KeyPrinter none_colon_colon =
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ":") // Add known parameters for sorting correctly
                .Ignore("Culture") // Ignore Culture
                .DefaultRule(true, prefixSeparator: ":");

        /// <summary>
        /// Name policy where every separator is ":", except for "Key" that has "." separtor.
        /// 
        /// Example "en:ConsoleApp1:MyController.Success".
        /// </summary>
        public static IParameterPrinter Colon_Colon_Dot => colon_colon_dot;
        private static readonly KeyPrinter colon_colon_dot = 
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ":") // Add known parameters for sorting correctly
                .Separator("Key", prefixSeparator: ".")
                .DefaultRule(true, prefixSeparator: ":");

        /// <summary>
        /// Name policy where every separator is ".".
        /// 
        /// Example "en.ConsoleApp1.MyController.Success"
        /// </summary>
        public static IParameterPrinter Dot_Dot_Dot => dot_dot_dot;
        private static readonly KeyPrinter dot_dot_dot = 
            new KeyPrinter()
            .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ".") // Add known parameters for sorting correctly
            .DefaultRule(true, prefixSeparator: ".");

        /// <summary>
        /// Name policy where "Culture" is not printed, and canonical parts have "." as separator.
        /// 
        /// Example "ConsoleApp1.MyController.Success"
        /// </summary>
        public static IParameterPrinter None_Dot_Dot => none_dot_dot;
        private static readonly KeyPrinter none_dot_dot = 
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ".") // Add known parameters for sorting correctly
                .Ignore("Culture") // Ignore Culture
                .DefaultRule(true, prefixSeparator: ".");

        /// <summary>
        /// Name policy where "Culture" has ":", and other parts "." have as separator.
        /// 
        /// Example "en:ConsoleApp1.MyController.Success".
        /// </summary>
        public static IParameterPrinter Colon_Dot_Dot => colon_dot_dot;
        private static readonly KeyPrinter colon_dot_dot =
            new KeyPrinter()
                .ParameterInfo(ParameterInfos.Default.Comparables(), prefixSeparator: ".") // Add known parameters for sorting correctly
                .Separator("Culture", postfixSeparator: ":") // Print with ":"
                .DefaultRule(true, prefixSeparator: ".");

        Dictionary<string, _Rule> parameters = new Dictionary<string, _Rule>();
        
        /// <summary>
        /// Get indexed parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public _Rule this[string name] { get { _Rule result = null; parameters.TryGetValue(name, out result); return result; } }

        /// <summary>
        /// Get all parametres
        /// </summary>
        public IEnumerable<_Rule> Parameters => parameters.Values;

        /// <summary>
        /// Construct a new name provider with no rules. 
        /// Use chain methods to add some rules.
        /// </summary>
        public KeyPrinter()
        {
        }

        /// <summary>
        /// Sets rule to not to include <paramref name="parameterName"/>, for example "Root".
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns>this</returns>
        public KeyPrinter Ignore(string parameterName)
        {
            parameters[parameterName] = new _Rule(parameterName, false, null, null, 0);
            return this;
        }

        /// <summary>
        /// Set rule for a parameter with <see cref="Utils.ParameterInfo"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public KeyPrinter ParameterInfo(IParameterInfo info, string prefixSeparator = null, string postfixSeparator = null)
        {
            parameters[info.ParameterName] = new _Rule(info.ParameterName, true, prefixSeparator, postfixSeparator, info.Order);
            return this;
        }

        /// <summary>
        /// Set rule for parameters with <see cref="Utils.ParameterInfo"/>s.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public KeyPrinter ParameterInfo(IEnumerable<IParameterInfo> infos, string prefixSeparator = null, string postfixSeparator = null)
        {
            foreach (var info in infos)
            {
                parameters[info.ParameterName] = new _Rule(info.ParameterName, true, prefixSeparator, postfixSeparator, info.Order);
            }
            return this;
        }

        /// <summary>
        /// Set rule for a specific <paramref name="parameterName"/>.
        /// </summary>
        /// <param name="parameterName">parameter name, for example "Culture", "Section", "Key", etc</param>
        /// <param name="isIncluded">true, parameter is included in name. False, parameter is not to be included. </param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <param name="order"></param>
        /// <returns>this</returns>
        public KeyPrinter Rule(string parameterName, bool isIncluded, string prefixSeparator = null, string postfixSeparator = null, int order = 0)
        {
            parameters[parameterName] = new _Rule(parameterName, isIncluded, prefixSeparator, postfixSeparator, order);
            return this;
        }

        /// <summary>
        /// Changes separator of existing rule, or creates new rule and uses Order 0.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <returns>this</returns>
        public KeyPrinter Separator(string parameterName, string prefixSeparator = null, string postfixSeparator = null)
        {
            _Rule rule;
            if (parameters.TryGetValue(parameterName, out rule))
            {
                rule.PrefixSeparator = prefixSeparator;
                rule.PostfixSeparator = postfixSeparator;
            } else
            {
                parameters[parameterName] = new _Rule(parameterName, true, prefixSeparator, postfixSeparator, 0);
            }
            return this;
        }

        /// <summary>
        /// Set default rule for unmatched canonical keys. Applied if there were no policy match with specific parameter name.
        /// </summary>
        /// <param name="isIncluded">are canonical parts to be included</param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <param name="order"></param>
        /// <returns>this</returns>
        public KeyPrinter CanonicalRule(bool isIncluded, string prefixSeparator = null, string postfixSeparator = null, int order = 0)
        {
            parameters["canonical"] = new _Rule("canonical", isIncluded, prefixSeparator, postfixSeparator, order);
            return this;
        }

        /// <summary>
        /// Set default rule for unmatched non-canonical key. Applied if there were no policy match with specific parameter name.
        /// </summary>
        /// <param name="isIncluded"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <param name="order"></param>
        /// <returns>this</returns>
        public KeyPrinter NonCanonicalRule(bool isIncluded, string prefixSeparator = null, string postfixSeparator = null, int order = 0)
        {
            parameters["noncanonical"] = new _Rule("noncanonical", isIncluded, prefixSeparator, postfixSeparator, order);
            return this;
        }

        /// <summary>
        /// Set rule for unknown parameterNames. This is the last rule that is applied.
        /// </summary>
        /// <param name="isIncluded"></param>
        /// <param name="prefixSeparator"></param>
        /// <param name="postfixSeparator"></param>
        /// <param name="order"></param>
        /// <returns>this</returns>
        public KeyPrinter DefaultRule(bool isIncluded, string prefixSeparator = null, string postfixSeparator = null, int order = 0)
        {
            parameters[""] = new _Rule("", isIncluded, prefixSeparator, postfixSeparator, order);
            return this;
        }

        /// <summary>
        /// Build key into a string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Print(ILinePart key)
        {
            // List for parts
            StructList16<Part> parts = new StructList16<Part>(Part.Comparer.Default);

            // Iterate key
            int occuranceIndex = 1;
            for (ILinePart part = key; part != null; part = part.PreviousPart)
            {
                if (part is ILineParameter parametrized)
                {
                    // Read parameter name and value
                    string parameterName = parametrized.ParameterName, parameterValue = part.GetParameterValue();
                    if (string.IsNullOrEmpty(parameterName) || parameterValue == null) continue;

                    // Get description
                    _Rule desc = null;
                    if (parameters.TryGetValue(parameterName, out desc) && !desc.IsIncluded) continue;

                    // Try default descriptions
                    if (desc == null && part is ILineCanonicallyComparedKey) parameters.TryGetValue("canonical", out desc);
                    if (desc == null && part is ILineNonCanonicallyComparedKey) parameters.TryGetValue("noncanonical", out desc);
                    if (desc == null) parameters.TryGetValue("", out desc);

                    // No description
                    if (desc == null) continue;

                    // This parameter is disabled
                    if (!desc.IsIncluded) continue;

                    // Count occurance index
                    occuranceIndex--;

                    // Add to list
                    parts.Add(new Part { ParameterName = parameterName, ParameterValue = parameterValue, Policy = desc, Order = desc.Order+occuranceIndex });
                }
            }

            // Sort list
            sorter.Sort(ref parts);

            // Calculate char count
            int len = 0;
            for(int i=0; i<parts.Count; i++)
            {
                len += parts[i].ParameterValue.Length;
                // Count in separator
                if (i>0)
                {
                    string separator = parts[i - 1].Policy.PostfixSeparator;
                    if (separator != null) len += separator.Length;
                    else 
                    {
                        separator = parts[i].Policy.PrefixSeparator;
                        if (separator != null) len += separator.Length;
                    }
                }
            }
            // Put together a string
            char[] chars = new char[len];
            int ix = 0;
            for (int i = 0; i < parts.Count; i++)
            {
                string s;
                // Add separator
                if (i > 0)
                {
                    s = parts[i - 1].Policy.PostfixSeparator;
                    if (s != null)
                    {
                        s.CopyTo(0, chars, ix, s.Length);
                        ix += s.Length;
                    }
                    else
                    {
                        s = parts[i].Policy.PrefixSeparator;
                        if (s != null)
                        {
                            s.CopyTo(0, chars, ix, s.Length);
                            ix += s.Length;
                        }
                    }
                }

                // Add text
                s = parts[i].ParameterValue;
                s.CopyTo(0, chars, ix, s.Length);
                ix += s.Length;
            }

            return new string(chars);
        }

        static StructListSorter<StructList16<Part>, Part> sorter = new StructListSorter<StructList16<Part>, Part>(Part.Comparer.Default);

        /// <summary>
        /// Copy policy.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            KeyPrinter result = new KeyPrinter();
            foreach (var kp in parameters)
                result.parameters[kp.Key] = kp.Value.Clone() as _Rule;
            return result;
        }

        /// <summary>
        /// A policy on how to print out keys that use a specific parameter name.
        /// </summary>
        public class _Rule : ICloneable
        {
            /// <summary>
            /// Name of parameter this policy applies to.
            /// 
            /// As special case value: 
            ///     "noncanonical" generic policy for non-canonical keys,
            ///     "canonical" generic policy for canonical keys,
            ///     "" fallback policy for any parameter that wasn't handled.
            /// </summary>
            public string ParameterName { get; internal set; }

            /// <summary>
            /// Policy whether a key with this parameter name is to be printed or not.
            /// </summary>
            public bool IsIncluded { get; internal set; }

            /// <summary>
            /// Separator to be used after a key that is of this parameter, if key has succeeding key.
            /// </summary>
            public string PostfixSeparator { get; internal set; }

            /// <summary>
            /// Separator to be used before a key that is of this parameter, if previous key didn't specify <see cref="PostfixSeparator"/>.
            /// </summary>
            public string PrefixSeparator { get; internal set; }

            /// <summary>
            /// Sorting order for this parameters. The value should go in thousands. See <see cref="Utils.ParameterInfo"/>.
            /// </summary>
            public int Order { get; internal set; }

            /// <summary>
            /// Create new parameter policy for a specific <paramref name="parameterName"/>.
            /// </summary>
            /// <param name="parameterName">parameter name or "canonical", "noncanonical", "" for special cases</param>
            /// <param name="isIncluded"></param>
            /// <param name="prefixSeparator"></param>
            /// <param name="postfixSeparator"></param>
            /// <param name="order"></param>
            public _Rule(string parameterName, bool isIncluded, string prefixSeparator, string postfixSeparator, int order)
            {
                ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
                IsIncluded = isIncluded;
                PrefixSeparator = prefixSeparator;
                PostfixSeparator = postfixSeparator;
                Order = order;
            }

            /// <summary>
            /// Copy this policy
            /// </summary>
            /// <returns></returns>
            public object Clone()
                => new _Rule(ParameterName, IsIncluded, PrefixSeparator, PostfixSeparator, Order);

            /// <summary>
            /// Print out policy
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{nameof(_Rule)}({ParameterName}, {IsIncluded}, {PrefixSeparator}, {PostfixSeparator}, {Order})";
        }

        /// <summary>
        /// Intermediate information about a <see cref="ILinePart"/> that was matched to a <see cref="_Rule"/>.
        /// </summary>
        struct Part
        {
            /// <summary>
            /// Parameter name
            /// </summary>
            public string ParameterName;

            /// <summary>
            /// Parameter value
            /// </summary>
            public string ParameterValue;

            /// <summary>
            /// Policy
            /// </summary>
            public _Rule Policy;

            /// <summary>
            /// Sort order
            /// </summary>
            public int Order;

            /// <summary>
            /// <see cref="Part"/> comparer.
            /// </summary>
            public class Comparer : IComparer<Part>, IEqualityComparer<Part>
            {
                private static Comparer instance;
                public static Comparer Default => instance ?? (instance = new Comparer());

                public bool Equals(Part x, Part y)
                    => (x.ParameterName == y.ParameterName) && (x.ParameterValue == y.ParameterValue);
                public int GetHashCode(Part obj)
                    => (obj.ParameterName == null ? 0 : 11 * obj.ParameterName.GetHashCode()) +
                       (obj.ParameterValue == null ? 0 : 13 * obj.ParameterValue.GetHashCode());
                public int Compare(Part x, Part y)
                    => x.Order - y.Order;
            }

            public override string ToString()
                => ParameterName + ":" + ParameterValue;
        }


    }

}
