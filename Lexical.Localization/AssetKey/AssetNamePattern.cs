// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Pattern that can be matched against localization keys and asset filenames.
    /// 
    /// Pattern is an enumeration of parts:
    ///  {culture}           - Matches to key.SetCulture("en")
    ///  {assembly}          - Matches to key.AssemblySection(asm)
    ///  {resource}          - Matches to key.ResourceSection("xx")
    ///  {type}              - Matches to key.TypeSection(type)
    ///  {section}           - Matches to key.Section("xx")
    ///  {anysection}        - Matches to assembly, type and section.
    ///  {key}               - Matches to regular key key.Key("x") and key["x"]
    ///  
    /// Parts can be optional in curly braces {} and required in brackets [].
    ///  [culture]
    /// 
    /// Part can be added multiple times, which matches when part has identifier secion multiple times. Latter part names must be suffixed with "_number".
    ///  "localization{-key_0}{-key_1}.ini"  - Matches to key.Key("x").Key("x");
    ///  
    /// Without a number, matches to last occurance of identifier
    ///  "{culture.}localization.ini"        - Matches to "fi" in: key.SetCulture("en").SetCulture("de").SetCulture("fi");
    /// 
    /// Before and after the pattern identifier pre- and postfix separator characters can be added:
    ///  {/culture.}
    ///  
    ///  
    /// If part identifier has ! at the end of name, then the part is matched with greedy quantified when matched against filenames.
    ///  {anysection!.}
    ///  
    /// There is no escaping for {, [, ] and } characters.
    /// 
    /// Examples:
    ///   "Resources.localization{-culture}.json"
    ///   "Resources.localization[-culture].json"
    ///   "Assets/{type/}localization{-culture}.ini"
    ///   "Assets/{assembly/}{type/}{section.}localization{-culture}.ini"
    ///   "{culture.}{type.}{section_0.}{section_1.}{section_2.}{section_3.}{.key_0}{.key_1}{key}"
    /// 
    /// </summary>
    public class AssetNamePattern : IAssetNamePattern
    {
        static Regex regex = new Regex(
            @"(?<text>[^\[\{\}\]]+)|"+
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
            foreach(System.Text.RegularExpressions.Match match in matches)
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
                    Part part = new Part {
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
                    Part part = new Part {
                        PatternText = match.Groups["r_patterntext"].Value,
                        PrefixSeparator = match.Groups["r_prefix"].Value,
                        PostfixSeparator = match.Groups["r_postfix"].Value,
                        Identifier = g_r_identifier.Value,
                        ParameterName = match.Groups["r_keyreader_identifier"].Value,
                        Required = true,
                        Index = ix++,
                        CaptureIndex = matchIx++,
                        regex = part_pattern == null ? null : new Regex(part_pattern, RegexOptions.Compiled|RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
                        OccuranceIndex = g_occurance.Success ? g_occurance.Value == "n" ? Int32.MaxValue : int.Parse(g_occurance.Value) : Int32.MaxValue
                    };
                    list.Add(part);
                }
            }
            allParts = list.ToArray();
            captureParts = list.Where(part=>part.Identifier!=null).ToArray();
            PartMap = CaptureParts.ToDictionary(p => p.Identifier);
            _keyvisitor = KeyVisitor;
            ParameterNames = captureParts.Select(part => part.ParameterName).Distinct().ToArray();
            ParameterMap = ParameterNames.ToDictionary(s=>s, parameterName => CaptureParts.Where(part => part.ParameterName == parameterName).OrderBy(p=>p.OccuranceIndex).ToArray());
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
            /// Identifier in <see cref="IAssetKeyParametrizer"/>, non-unique identifier.
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

        class _NamePatternMatch : NamePatternMatch
        {
            public readonly IAssetKeyParametrizer parameterReader;
            public _NamePatternMatch(IAssetNamePattern pattern, IAssetKeyParametrizer parameterReader) : base(pattern)
            {
                this.parameterReader = parameterReader;
            }
        }

        public IAssetNamePatternMatch Match(Object key, IAssetKeyParametrizer parameterReader)
        {
            _NamePatternMatch match = new _NamePatternMatch(this, parameterReader);
            parameterReader.VisitParts(key, _keyvisitor, ref match);
            match._fixPartsWithOccurancesAndLastOccurance();
            return match;
        }

        ParameterPartVisitor<_NamePatternMatch> _keyvisitor;
        void KeyVisitor(object key_part, ref _NamePatternMatch match)
        {
            string[] key_part_parameters = match.parameterReader.GetPartParameters(key_part);

            // No parameters
            if (key_part_parameters == null || key_part_parameters.Length == 0) return;

            // Iterate each parameter (typically 0 or 1).
            foreach(string parameterName in key_part_parameters)
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
                        string value = match.parameterReader.GetPartValue(key_part, part.ParameterName);
                        if (value == null) continue;
                        if (!part.IsMatch(value)) continue;

                        // Set value
                        match.PartValues[part.CaptureIndex] = value;
                        return;
                    }
                }

                // "anysection"
                if ((parameterName == "section" || parameterName == "location" || parameterName == "type" || parameterName == "resource" || parameterName == "assembly") && ParameterMap.TryGetValue("anysection", out parts))
                {
                    // Iterate each part
                    foreach (IAssetNamePatternPart part in parts)
                    {
                        // Test if part is already filled
                        if (match[part.CaptureIndex] != null) continue;

                        // Read value
                        string value = match.parameterReader.GetPartValue(key_part, part.ParameterName);
                        if (value == null) continue;
                        if (!part.IsMatch(value)) continue;

                        // Set value
                        match.PartValues[part.CaptureIndex] = value;
                        return;
                    }
                }

            }
        }
    }

}
