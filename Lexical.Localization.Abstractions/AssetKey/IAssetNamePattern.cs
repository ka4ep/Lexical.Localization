// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    #region IAssetNamePattern
    /// <summary>
    /// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="IAssetKey"/> instances.
    /// Is a sequence of parameter and text parts.
    /// 
    /// Parameter parts:
    ///  {Culture}           - Matches to key.Culture("en")
    ///  {Assembly}          - Matches to key.Assembly(asm).
    ///  {Resource}          - Matches to key.Resource("xx").
    ///  {Type}              - Matches to key.Type(type)
    ///  {Section}           - Matches to key.Section("xx")
    ///  {Location}          - Matches to key.Location("xx") and a physical folder, separator is '/'.
    ///  {anysection}        - Matches to assembly, type and section.
    ///  {Key}               - Matches to key key.Key("x")
    /// 
    /// Before and after the part pre- and postfix separator characters can be added:
    ///  {/Culture.}
    ///  
    /// Parts can be optional in curly braces {} and required in brackets [].
    ///  [Culture]
    /// 
    /// Part can be added multiple times, which matches when part has identifier secion multiple times. Latter part names must be suffixed with "_number".
    ///  "localization{-Key_0}{-Key_1}.ini"  - Matches to key.Key("x").Key("x");
    /// 
    /// Suffix "_n" refers to the last occurance. This is also the case without an occurance number.
    ///  "{Culture.}localization.ini"        - Matches to "fi" in: key.SetCulture("en").SetCulture("de").SetCulture("fi");
    ///  "{Location_0/}{Location_1/}{Location_2/}{Location_n/}location.ini 
    ///  
    /// Regular expressions can be written between &lt; and &gt; characters to specify match criteria. \ escapes \, *, +, ?, |, {, [, (,), &lt;, &gr; ^, $,., #, and white space.
    ///  "{Section&lt;[^:]*&gt;.}"
    /// 
    /// Regular expressions can be used for greedy match when matching against filenames and embedded resources.
    ///  "{Assembly.}{Resource&lt;.*&gt;.}{Type.}{Section.}{Key}"
    /// 
    /// Examples:
    ///   "[Assembly.]Resources.localization{-Culture}.json"
    ///   "[Assembly.]Resources.{Type.}localization[-Culture].json"
    ///   "Assets/{Type/}localization{-Culture}.ini"
    ///   "Assets/{Assembly/}{Type/}{Section.}localization{-Culture}.ini"
    ///   "{Culture.}{Type.}{Section_0.}{Section_1.}{Section_2.}[Section_n]{.Key_0}{.Key_1}{.Key_n}"
    /// 
    /// </summary>
    public interface IAssetNamePattern : IAssetKeyNamePolicy
    {
        /// <summary>
        /// Pattern in string format
        /// </summary>
        string Pattern { get; }

        /// <summary>
        /// All parts of the pattern
        /// </summary>
        IAssetNamePatternPart[] AllParts { get; }

        /// <summary>
        /// All parts that capture a part of string.
        /// </summary>
        IAssetNamePatternPart[] CaptureParts { get; }
        
        /// <summary>
        /// Maps parts by identifier.
        /// </summary>
        IReadOnlyDictionary<string, IAssetNamePatternPart> PartMap { get; }

        /// <summary>
        /// List of all parameter names
        /// </summary>
        string[] ParameterNames { get; }

        /// <summary>
        /// Maps parts by parameter identifier.
        /// </summary>
        IReadOnlyDictionary<string, IAssetNamePatternPart[]> ParameterMap { get; }

        /// <summary>
        /// Match parameters from an object.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAssetNamePatternMatch Match(IAssetKey obj);

        /// <summary>
        /// A regular expression pattern that captures same parts from a filename string.
        /// </summary>
        Regex Regex { get; }
    }
    #endregion IAssetNamePattern

    #region IAssetNamePatternPart
    /// <summary>
    /// Part of a pattern.
    /// </summary>
    public interface IAssetNamePatternPart
    {
        /// <summary>
        /// Text that represents this part in pattern.
        /// </summary>
        string PatternText { get; }

        /// <summary>
        /// Part identifier, unique in context of Pattern.CaptureParts.
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Separator
        /// </summary>
        string PrefixSeparator { get; }

        /// <summary>
        /// Separator
        /// </summary>
        string PostfixSeparator { get; }

        /// <summary>
        /// Parameter identifier.
        /// </summary>
        string ParameterName { get; }
        /// <summary>
        /// If set, then is non-matchable Text part.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Is part mandatory
        /// </summary>
        bool Required { get; }

        /// <summary>
        /// Index in <see cref="IAssetNamePattern.AllParts"/>.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Index in <see cref="IAssetNamePatternPart.CaptureParts"/>.
        /// </summary>
        int CaptureIndex { get; }

        /// <summary>
        /// The order of occurance to capture against.
        /// 
        /// As special case Int32.MaxValue means the last occurance "{.Section}"
        /// 
        /// For example "{.Section_0}" captures first occurance, and the part's OccuranceIndex = 0.
        ///             "{.Section}" captures the last occurance overriding possible ordered occurance if there is only one match.
        /// </summary>
        int OccuranceIndex { get; }

        /// <summary>
        /// Pattern of this part.
        /// </summary>
        Regex Regex { get; }

        /// <summary>
        /// Tests if text is match.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool IsMatch(string text);
    }
    #endregion IAssetNamePatternPart

    #region IAssetNamePatternMatch
    /// <summary>
    /// Match result.
    /// </summary>
    public interface IAssetNamePatternMatch : IReadOnlyDictionary<string, string>
    {
        /// <summary>
        /// Associated patern.
        /// </summary>
        IAssetNamePattern Pattern { get; }

        /// <summary>
        /// Resolved part values.
        /// </summary>
        string[] PartValues { get; }

        /// <summary>
        /// Part values by <see cref="CaptureParts" />
        /// </summary>
        /// <param name="ix"></param>
        /// <returns></returns>
        string this[int ix] { get; }

        /// <summary>
        /// Get part value by part identifier.
        /// </summary>
        /// <param name="identifier">identifier, e.g. "Culture", "Type"</param>
        /// <returns>value or null</returns>
        new string this[string identifier] { get; }

        /// <summary>
        /// Where all required parts found.
        /// </summary>
        bool Success { get; }
    }
    #endregion IAssetNamePatternMatch

    public static partial class AssetNamePatternExtensions
    {
        /// <summary>
        /// Match parameters of an object and convert into a string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="key"></param>
        /// <returns>match as string or null</returns>
        public static string MatchToString(this IAssetNamePattern pattern, IAssetKey key)
        {
            IAssetNamePatternMatch match = pattern.Match(key);
            return pattern.BuildName(match.PartValues);
        }

        /// <summary>
        /// Builds name from captured part values.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="partValues"></param>
        /// <returns>Built name, or null if one of the required parts were not found.</returns>
        public static string BuildName(this IAssetNamePattern pattern, IEnumerable<string> partValues)
        {
            // Count chars
            int length = 0;
            IEnumerator<string> partValuesEtor = partValues.GetEnumerator();
            foreach (IAssetNamePatternPart part in pattern.AllParts)
            {
                if (part.Text != null) length += part.Text.Length;
                else
                {
                    if (!partValuesEtor.MoveNext()) return null;
                    string value = partValuesEtor.Current;
                    if (value != null) length += part.PrefixSeparator.Length + part.PostfixSeparator.Length + value.Length;
                }
            }

            // Put together string
            // .. without StringBuilder, there needs to be efficiencies since this may be called frequently.
            int ix = 0;
            char[] chars = new char[length];
            partValuesEtor.Reset();
            foreach (IAssetNamePatternPart part in pattern.AllParts)
            {
                if (part.Text != null) { part.Text.CopyTo(0, chars, ix, part.Text.Length); ix += part.Text.Length; }
                else
                {
                    if (!partValuesEtor.MoveNext()) return null;
                    string value = partValuesEtor.Current;
                    if (value == null) { if (part.Required) return null; else continue; }
                    part.PrefixSeparator.CopyTo(0, chars, ix, part.PrefixSeparator.Length); ix += part.PrefixSeparator.Length;
                    value.CopyTo(0, chars, ix, value.Length); ix += value.Length;
                    part.PostfixSeparator.CopyTo(0, chars, ix, part.PostfixSeparator.Length); ix += part.PostfixSeparator.Length;
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// Build name from captured part values.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>Built name, or null if one of the required parts were not found.</returns>
        public static string BuildName(this IAssetNamePattern pattern, IReadOnlyDictionary<string, string> parts)
        {
            // Count chars
            int length = 0;
            foreach (IAssetNamePatternPart part in pattern.AllParts)
            {
                if (part.Text != null) length += part.Text.Length;
                else
                {
                    string value = null;
                    if (parts.TryGetValue(part.Identifier, out value) && !string.IsNullOrEmpty(value))
                        length += part.PrefixSeparator.Length + part.PostfixSeparator.Length + value.Length;
                    else if (part.Required) return null;
                }
            }

            // Put together string
            // .. without StringBuilder, there needs to be efficiencies since this may be called frequently.
            int ix = 0;
            char[] chars = new char[length];
            foreach (IAssetNamePatternPart part in pattern.AllParts)
            {
                if (part.Text != null) { part.Text.CopyTo(0, chars, ix, part.Text.Length); ix += part.Text.Length; }
                else
                {
                    string value = null;
                    if (parts.TryGetValue(part.Identifier, out value))
                    {
                        if (string.IsNullOrEmpty(value)) { if (part.Required) return null; else continue; }
                        part.PrefixSeparator.CopyTo(0, chars, ix, part.PrefixSeparator.Length); ix += part.PrefixSeparator.Length;
                        value.CopyTo(0, chars, ix, value.Length); ix += value.Length;
                        part.PostfixSeparator.CopyTo(0, chars, ix, part.PostfixSeparator.Length); ix += part.PostfixSeparator.Length;
                    }
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// Flags for <see cref="BuildRegexString(IAssetNamePattern, IReadOnlyDictionary{string, string}, BuildRegexFlags)"/>.
        /// </summary>
        [Flags]
        public enum BuildRegexFlags : Int32
        {
            /// <summary>
            /// Escape the nonparts
            /// </summary>
            Escape_NonPart = 1,

            /// <summary>
            /// Escape the prefix parts.
            /// </summary>
            Escape_Prefix = 2,

            /// <summary>
            /// Escape postfix parts.
            /// </summary>
            Escape_Postfix = 4,

            /// <summary>
            /// Escape identifier part
            /// </summary>
            Escape_Identifier = 8,

            /// <summary>
            /// Enable all flags
            /// </summary>
            All = -1
        };

        internal static string Escape(string txt, BuildRegexFlags flags, BuildRegexFlags requiredFlag)
            => (flags & requiredFlag) != 0 ? Regex.Escape(txt) : txt;

        /// <summary>
        /// Build regex pattern that captures same parts from filename string.
        /// 
        /// For example "localization{-Culture}.ini" translates to regex "localization(-(?&lt;Culture&gt;.*))?.ini".
        /// This can be matched against filename "localization-en.ini" with group m.Group["Culture"].Value == "en".
        /// </summary>
        /// <paramref name="pattern"/>
        /// <paramref name="filledParameters"/>>
        /// <returns>regex</returns>
        public static string BuildRegexString(this IAssetNamePattern pattern, IReadOnlyDictionary<string, string> filledParameters, BuildRegexFlags flags = BuildRegexFlags.All)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('^');
            for (int i = 0; i < pattern.AllParts.Length; i++)
            {
                IAssetNamePatternPart part = pattern.AllParts[i];

                // Add regular part
                if (part.Text != null) { sb.Append(Escape(part.Text, flags, BuildRegexFlags.Escape_NonPart)); continue; }

                string prefilled_value;
                if (filledParameters != null && filledParameters.TryGetValue(part.Identifier, out prefilled_value) && prefilled_value != null)
                {
                    if (part.PrefixSeparator != null) sb.Append( Escape(part.PrefixSeparator, flags, BuildRegexFlags.Escape_Prefix) );
                    sb.Append( Escape(prefilled_value, flags, BuildRegexFlags.Escape_Identifier) );
                    if (part.PostfixSeparator != null) sb.Append( Escape(part.PostfixSeparator, flags, BuildRegexFlags.Escape_Postfix) );
                }
                else
                {
                    sb.Append("(");
                    if (part.PrefixSeparator != null) sb.Append(Escape(part.PrefixSeparator, flags, BuildRegexFlags.Escape_Prefix));
                    sb.Append("(?<");
                    sb.Append(Escape(part.Identifier, flags, BuildRegexFlags.Escape_Identifier));
                    sb.Append('>');
                    // Append type specific pattern
                    Regex part_pattern = part.Regex;
                    string pattern_text = part_pattern.ToString();
                    int start = 0, end = pattern_text.Length;
                    if (pattern_text.Length > 0 && pattern_text[start] == '^') start++;
                    if (pattern_text.Length > 0 && pattern_text[end - 1] == '$') end--;
                    if (start < end) sb.Append(pattern_text.Substring(start, end - start));
                    sb.Append(')');
                    if (part.PostfixSeparator != null) sb.Append(Escape(part.PostfixSeparator, flags, BuildRegexFlags.Escape_Postfix));
                    sb.Append(")");
                    if (!part.Required) sb.Append('?');
                }
            }
            sb.Append('$');

            return sb.ToString();
        }

        /// <summary>
        /// Construct regular expression with half filled parameters
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="filledParameters"></param>
        /// <returns></returns>
        public static Regex BuildRegex(this IAssetNamePattern pattern, IReadOnlyDictionary<string, string> filledParameters)
        {
            string pattern_text = pattern.BuildRegexString(filledParameters);
            RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            if (filledParameters == null || filledParameters.Count == 0) options |= RegexOptions.Compiled;
            return new Regex(pattern_text, options);
        }

        /// <summary>
        /// Test if <paramref name="parameters"/> fill all required parameters of <paramref name="pattern"/>.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="parameters"></param>
        /// <param name="allParts">if true tests if all capture parts match, if false, if non-optional parts match</param>
        /// <returns></returns>
        public static bool TestSuccess(this IAssetNamePattern pattern, IReadOnlyDictionary<string, string> parameters, bool allParts)
        {
            if (parameters == null) return false;
            foreach (var part in pattern.CaptureParts)
            {
                if (!allParts && !part.Required) continue;
                string value;
                if (!parameters.TryGetValue(part.Identifier, out value)) return false;
                if (value == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Default patterns to use for each parameter.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, Regex> default_parameter_patterns;
        static readonly Regex default_reluctant_pattern = new Regex(".*?", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        public static Regex GetDefaultPattern(string parameter)
        {
            if (parameter == null || parameter == "") return default_reluctant_pattern;
            Regex pattern;
            if (default_parameter_patterns.TryGetValue(parameter, out pattern)) return pattern;
            return default_reluctant_pattern;
        }

        static AssetNamePatternExtensions()
        {
            var map = new Dictionary<string, Regex>();
            default_parameter_patterns = map;
            map["Culture"] = new Regex(@"^([a-z]{2,5})(-([A-Za-z]{2,7}))?$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            map["Type"] = new Regex("[^:0-9][^:]*", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }

        /// <summary>
        /// Match against string or filename.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="text"></param>
        /// <param name="filledParameters"></param>
        /// <returns>match object</returns>
        public static IAssetNamePatternMatch Match(this IAssetNamePattern pattern, string text, IReadOnlyDictionary<string, string> filledParameters = null)
        {
            bool hasFilledParameters = false;
            if (filledParameters != null && filledParameters.Count > 0) foreach (var kp in filledParameters) if (kp.Value != null) { hasFilledParameters = true; break; }
            Regex regex = hasFilledParameters ? pattern.Regex : pattern.BuildRegex(filledParameters);
            System.Text.RegularExpressions.Match match = regex.Match(text);
            NamePatternMatch _match = new NamePatternMatch(pattern);
            if (match.Success)
            {
                foreach (IAssetNamePatternPart part in pattern.CaptureParts)
                {
                    string prefilled_value;
                    if (hasFilledParameters && filledParameters.TryGetValue(part.Identifier, out prefilled_value))
                    {
                        _match.PartValues[part.CaptureIndex] = prefilled_value;
                    }
                    else
                    {
                        Group g = match.Groups[part.Identifier];
                        if (g.Success) _match.PartValues[part.CaptureIndex] = g.Value;
                    }
                }
            }
            _match._fixPartsWithOccurancesAndLastOccurance();
            return _match;
        }


        /// <summary>
        /// Add values from dictionary.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="values"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static IAssetNamePatternMatch Add(this IAssetNamePatternMatch match, IReadOnlyDictionary<string, string> values, bool overwrite = true)
        {
            foreach (var kp in values)
            {
                if (kp.Value == null) continue;
                IAssetNamePatternPart part;
                if (!match.Pattern.PartMap.TryGetValue(kp.Key, out part)) continue;
                if (!overwrite && match.PartValues[part.CaptureIndex] != null) continue;
                match.PartValues[part.CaptureIndex] = kp.Value;
            }
            return match;
        }

        /// <summary>
        /// Create pattern match from regular expression
        /// </summary>
        /// <param name="_match"></param>
        /// <param name="match"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static IAssetNamePatternMatch Add(this IAssetNamePatternMatch _match, Match match, bool overwrite = true)
        {
            foreach (var part in _match.Pattern.CaptureParts)
            {
                Group g = match.Groups[part.Identifier];
                if (!g.Success) continue;
                if (!overwrite && _match.PartValues[part.CaptureIndex] != null) continue;
                _match.PartValues[part.CaptureIndex] = g.Value;
            }
            return _match;
        }
    }

}
