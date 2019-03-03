﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="IAssetKey"/> instances.
    /// Is a sequence of parameter and text parts.
    /// 
    /// Parameter parts:
    ///  {Culture}           - Matches to key.SetCulture("en")
    ///  {Assembly}          - Matches to key.AssemblySection(asm).
    ///  {Resource}          - Matches to key.ResourceSection("xx").
    ///  {Type}              - Matches to key.TypeSection(type)
    ///  {Section}           - Matches to key.Section("xx")
    ///  {Location}          - Matches to key.LocationSection("xx") and a physical folder, separator is '/'.
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
    public class AssetNamePattern : IAssetNamePattern, IAssetKeyNameParser
    {
        static Regex regex = new Regex(
            @"(?<text>[^\[\{\}\]]+)|" +
            @"(?<o_patterntext>\{(?<o_prefix>[^\}a-zA-Z]*)(?<o_identifier>(?<o_keyreader_identifier>[a-zA-Z]+)(_(?<o_occurance_index>[0-9]+|n))?)(\<(?<o_pattern>([^\\\>\\<]|\\.)*)\>)?(?<o_postfix>[^\}a-zA-Z]*)\})|" +
            @"(?<r_patterntext>\[(?<r_prefix>[^\]a-zA-Z]*)(?<r_identifier>(?<r_keyreader_identifier>[a-zA-Z]+)(_(?<r_occurance_index>[0-9]+|n))?)(\<(?<r_pattern>([^\\\>\\<]|\\.)*)\>)?(?<r_postfix>[^\]a-zA-Z]*)\])",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public string Pattern { get; internal set; }
        public override string ToString() => Pattern;

        /// <summary>
        /// All parts of the pattern
        /// </summary>
        readonly Part[] allParts;
        public IAssetNamePatternPart[] AllParts => allParts;

        /// <summary>
        /// All parts that capture a part of string.
        /// </summary>
        readonly Part[] captureParts;
        public IAssetNamePatternPart[] CaptureParts => captureParts;

        /// <summary>
        /// Maps parts by part identifier.
        /// </summary>
        public IReadOnlyDictionary<string, IAssetNamePatternPart> PartMap { get; internal set; }

        /// <summary>
        /// Maps parts by parameter identifier.
        /// </summary>
        public IReadOnlyDictionary<string, IAssetNamePatternPart[]> ParameterMap { get; internal set; }

        /// <summary>
        /// List of all parameter names
        /// </summary>
        public string[] ParameterNames { get; internal set; }

        protected Regex cachedRegex;

        /// <summary>
        /// A regular expression pattern that captures same parts from filename strings.
        /// </summary>
        public Regex Regex => cachedRegex ?? (cachedRegex = this.BuildRegex(null));

        /// <summary>
        /// Create pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
        public AssetNamePattern(string pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            System.Text.RegularExpressions.MatchCollection matches = regex.Matches(pattern);
            if (matches.Count == 0) throw new ArgumentException($"Failed to parse filename pattern \"{pattern}\"");

            List<Part> list = new List<Part>(matches.Count);
            int ix = 0, matchIx = 0;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (!match.Success) throw new ArgumentException($"Failed to parse filename pattern \"{pattern}\"");
                Group g_text = match.Groups["text"];
                Group g_o_identifier = match.Groups["o_identifier"];
                Group g_r_identifier = match.Groups["r_identifier"];
                if (g_text.Success)
                {
                    Part part = new Part { Text = g_text.Value, Index = ix++, CaptureIndex = -1 };
                    list.Add(part);
                }
                else if (g_o_identifier.Success)
                {
                    Group g_occurance = match.Groups["o_occurance_index"];
                    Group g_pattern = match.Groups["o_pattern"];
                    string part_pattern = g_pattern.Success ? g_pattern.Value : null;
                    Part part = new Part
                    {
                        PatternText = match.Groups["o_patterntext"].Value,
                        PrefixSeparator = match.Groups["o_prefix"].Value,
                        PostfixSeparator = match.Groups["o_postfix"].Value,
                        Identifier = g_o_identifier.Value,
                        ParameterName = match.Groups["o_keyreader_identifier"].Value,
                        Required = false,
                        Index = ix++,
                        CaptureIndex = matchIx++,
                        regex = part_pattern == null ? null : new Regex(part_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
                        OccuranceIndex = g_occurance.Success ? g_occurance.Value == "n" ? Int32.MaxValue : int.Parse(g_occurance.Value) : Int32.MaxValue
                    };
                    list.Add(part);
                }
                else if (g_r_identifier.Success)
                {
                    Group g_occurance = match.Groups["r_occurance_index"];
                    Group g_pattern = match.Groups["r_pattern"];
                    string part_pattern = g_pattern.Success ? g_pattern.Value : null;
                    Part part = new Part
                    {
                        PatternText = match.Groups["r_patterntext"].Value,
                        PrefixSeparator = match.Groups["r_prefix"].Value,
                        PostfixSeparator = match.Groups["r_postfix"].Value,
                        Identifier = g_r_identifier.Value,
                        ParameterName = match.Groups["r_keyreader_identifier"].Value,
                        Required = true,
                        Index = ix++,
                        CaptureIndex = matchIx++,
                        regex = part_pattern == null ? null : new Regex(part_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
                        OccuranceIndex = g_occurance.Success ? g_occurance.Value == "n" ? Int32.MaxValue : int.Parse(g_occurance.Value) : Int32.MaxValue
                    };
                    list.Add(part);
                }
            }
            allParts = list.ToArray();
            captureParts = list.Where(part => part.Identifier != null).ToArray();
            PartMap = CaptureParts.ToDictionary(p => p.Identifier);
            _keyvisitor = KeyVisitor;
            ParameterNames = captureParts.Select(part => part.ParameterName).Distinct().ToArray();
            ParameterMap = ParameterNames.ToDictionary(s => s, parameterName => CaptureParts.Where(part => part.ParameterName == parameterName).OrderBy(p => p.OccuranceIndex).ToArray());
        }

        public class Part : IAssetNamePatternPart
        {
            /// <summary>
            /// Text that represents this part in pattern.
            /// </summary>
            public string PatternText { get; internal set; }

            /// <summary>
            /// Separator
            /// </summary>
            public string PrefixSeparator { get; set; }

            /// <summary>
            /// Separator
            /// </summary>
            public string PostfixSeparator { get; set; }

            /// <summary>
            /// Part identifier, unique identifier.
            /// </summary>
            public string Identifier { get; set; }

            /// <summary>
            /// Parameter identifier.
            /// </summary>
            public string ParameterName { get; set; }

            /// <summary>
            /// Text part, no matching needed.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Required part.
            /// </summary>
            public bool Required { get; set; }

            /// <summary>
            /// Index in <see cref="AllParts"/>.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Index in <see cref="CaptureParts"/>.
            /// </summary>
            public int CaptureIndex { get; set; }

            /// <summary>
            /// Match occurance index.
            /// </summary>
            public int OccuranceIndex { get; set; }

            /// <summary>
            /// Overriding pattern that matches this part.
            /// </summary>
            internal Regex regex;

            /// <summary>
            /// Pattern of this part.
            /// </summary>
            public Regex Regex => regex ?? AssetNamePatternExtensions.GetDefaultPattern(ParameterName);

            /// <summary>
            /// Tests if text is match.
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public bool IsMatch(string text) => Regex == AssetNamePatternExtensions.GetDefaultPattern(null) ? true : Regex.IsMatch(text);

            /// <summary>
            /// Print part
            /// </summary>
            /// <returns></returns>
            public override string ToString() => PatternText;
        }

        public IAssetNamePatternMatch Match(IAssetKey key)
        {
            NamePatternMatch match = new NamePatternMatch(this);
            key.VisitParameters(_keyvisitor, ref match);
            match._fixPartsWithOccurancesAndLastOccurance();
            return match;
        }

        KeyParameterVisitor<NamePatternMatch> _keyvisitor;
        void KeyVisitor(string parameterName, string parameterValue, ref NamePatternMatch match)
        {
            // Search parts
            IAssetNamePatternPart[] parts;
            if (ParameterMap.TryGetValue(parameterName, out parts))
            {
                // Iterate each part
                foreach (IAssetNamePatternPart part in parts)
                {
                    // Test if part is already filled
                    if (match[part.CaptureIndex] != null) continue;

                    // Read value
                    if (parameterValue == null) continue;
                    if (!part.IsMatch(parameterValue)) continue;

                    // Set value
                    match.PartValues[part.CaptureIndex] = parameterValue;
                    return;
                }
            }

            // "anysection"
            if ((parameterName == "Section" || parameterName == "Location" || parameterName == "Type" || parameterName == "Resource" || parameterName == "Assembly") && ParameterMap.TryGetValue("anysection", out parts))
            {
                // Iterate each part
                foreach (IAssetNamePatternPart part in parts)
                {
                    // Test if part is already filled
                    if (match[part.CaptureIndex] != null) continue;

                    // Read value
                    if (parameterValue == null) continue;
                    if (!part.IsMatch(parameterValue)) continue;

                    // Set value
                    match.PartValues[part.CaptureIndex] = parameterValue;
                    return;
                }
            }
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str">key as string</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="FormatException">If parse failed</exception>
        /// <exception cref="ArgumentException">If <paramref name="policy"/> doesn't implement <see cref="IAssetKeyNameParser"/>.</exception>
        public IAssetKey Parse(string str, IAssetKey rootKey = default)
        {
            IAssetNamePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) throw new FormatException($"Key \"{str}\" did not match the pattern \"{Pattern}\"");

            IAssetKey result = rootKey;
            foreach (var kp in match)
            {
                if (kp.Key == null || kp.Value == null) continue;
                result = result == null ? Key.Create(kp.Key, kp.Value) : result.AppendParameter(kp.Key, kp.Value);
            }
            return result;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">key result or null if contained no content</param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        public bool TryParse(string str, out IAssetKey key, IAssetKey rootKey = default)
        {
            IAssetNamePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) { key = null; return false; }
            IAssetKey result = rootKey;
            foreach (var kp in match)
            {
                if (kp.Key == null || kp.Value == null) continue;
                result = result == null ? Key.Create(kp.Key, kp.Value) : result.AppendParameter(kp.Key, kp.Value);
            }
            key = result;
            return true;
        }

    }

    public static partial class AssetNamePatternExtensions_
    {
        /// <summary>
        /// Convert <paramref name="match"/> into an asset key that contains the captured parameters.
        /// </summary>
        /// <param name="match"></param>
        /// <returns>key or null if <paramref name="match"/> contained no values</returns>
        public static IAssetKey ToKey(this IAssetNamePatternMatch match)
        {
            Key result = null;
            foreach(IAssetNamePatternPart part in match.Pattern.CaptureParts)
            {
                string value = match[part.CaptureIndex];
                if (value == null) continue;
                result = Key.Create(result, part.ParameterName, value);
            }
            return result;
        }

        /// <summary>
        /// Convert <paramref name="match"/> into an enumeration of parameter names and values.
        /// </summary>
        /// <param name="match">(optional)</param>
        /// <returns>parameter names and values</returns>
        public static IEnumerable<KeyValuePair<string, string>> ToParameters(this IAssetNamePatternMatch match)
        {
            if (match == null) yield break;
            foreach (IAssetNamePatternPart part in match.Pattern.CaptureParts)
            {
                string value = match[part.CaptureIndex];
                if (value == null) continue;
                yield return new KeyValuePair<string, string>(part.ParameterName, value);
            }
        }

        /// <summary>
        /// Convert "match" parameters into an array of "non-match" parameters.
        /// 
        /// "match" parameter is a parameter in the format of <see cref="IAssetNamePattern"/>, where match contains
        /// capture index "_#". For example "section_0", "section_1". The capture index is removed from the result of 
        /// this function. Keys are orderd by this index.
        /// 
        /// "non-match" parameter does not have "anysection" nor capture index "_#".
        /// 
        /// "anysection" is converted to "Section".
        /// 
        /// Parameters are returned in the following order:
        ///  "Root", "Culture", "Assembly", "Location", "Resource", "Type", "Section", other parts here in alphabetical order, "Key", "N", "N1", "N2", "N3", "N4", "N5", "N6", ....
        ///  
        /// This is workaround as the order information is lost in the dictionary format.
        /// </summary>
        /// <param name="matchParameters">(optional) </param>
        /// <returns>converted parameters</returns>
        public static IEnumerable<KeyValuePair<string, string>> ConvertMatchParametersToNonMatchParameters(IReadOnlyDictionary<string, string> matchParameters)
        {
            if (matchParameters == null) return new KeyValuePair<string, string>[0];
            if (matchParameters is IAssetNamePatternMatch match) return ToParameters(match);

            // (parameter name, parameter value, sorting value)
            List<(string, string, int)> list = new List<(string, string, int)>();

            foreach(var matchParameter in matchParameters)
            {
                // Parse
                if (String.IsNullOrEmpty(matchParameter.Key)) continue;
                Match m = occuranceIndexParser.Match(matchParameter.Key);
                if (!m.Success) continue;
                Group g_name = m.Groups["name"], g_index = m.Groups["index"];

                // Sorting index
                int ix = 0;

                // Get name
                string name = g_name.Value;
                if (name == "anysection") name = "Section";
                if (!ConvertPriority.TryGetValue(name, out ix)) ix = 0;

                // Occurance index "_#"
                if (g_index.Success) ix += Int32.Parse(g_index.Value);

                list.Add((name, matchParameter.Value, ix));
            }

            // Sort 
            var result = list.OrderBy(l => l.Item3).Select(l => new KeyValuePair<string, string>(l.Item1, l.Item2)).ToArray();
            // return result as array
            return result;
        }
        static Regex occuranceIndexParser = new Regex("(?<name>.*)(_(?<index>\\d+))?$", RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture);

        /// <summary>
        /// Sorting priority for <see cref="ConvertMatchParametersToNonMatchParameters(IReadOnlyDictionary{string, string})"/>
        /// </summary>
        public static Dictionary<string, int> ConvertPriority = new Dictionary<string, int> {
            { "Root", -8000 }, { "Culture", -7000 }, { "Assembly", -6000 }, { "Location", -5000 }, { "Resource", -4000 }, { "Type", -3000 }, { "Section", -2000 }, { "Key", 1000 },
            { "N", 10000 }, { "N1", 11000 }, { "N2", 12000 }, { "N3", 13000 }, { "N4", 14000 }, { "N5", 15000 }, { "N6", 16000 }, { "N7", 17000 }, { "N8", 18000 }, { "N9", 19000 }, { "N10", 20000 },
        };

    }

}
