// --------------------------------------------------------
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
    /// A name pattern, akin to regular expression, that can be matched against filenames and <see cref="ILine"/> instances.
    /// Is a sequence of parameter and text parts.
    /// 
    /// Parameter parts:
    ///  {Culture}           - Matches to key.Culture("en")
    ///  {Assembly}          - Matches to key.Assembly(asm).
    ///  {BaseName}          - Matches to key.BaseName("xx").
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
    /// Part can be added multiple times
    ///  "{Location/}{Location/}{Location/}{Key}"  - Matches to, from 0 to 3 occurances of Location(), e.g. key.Location("dir").Location("dir1");
    /// 
    /// If parts need to be matched out of order, then occurance index can be used "_number".
    ///  "{Location_2/}{Location_1/}{Location_0/}{Key}"  - Matches to, from 0 to 3 occurances of Location, e.g. key.Location("dir").Location("dir1");
    /// 
    /// Suffix "_n" translates to five conscutive parts.
    ///  "[Location_n/]location.ini" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}"
    ///  "[Location/]{Location_n/}location.ini" translates to "[Location_0/]{Location_1/}{Location_2/}{Location_3/}{Location_4/}{Location_5/}"
    ///  
    /// Regular expressions can be written between &lt; and &gt; characters to specify match criteria. \ escapes \, *, +, ?, |, {, [, (,), &lt;, &gt; ^, $,., #, and white space.
    ///  "{Section&lt;[^:]*&gt;.}"
    /// 
    /// Regular expressions can be used for greedy match when matching against filenames and embedded resources.
    ///  "{Assembly.}{BaseName&lt;.*&gt;.}{Type.}{Section.}{Key}"
    /// 
    /// Examples:
    ///   "[Assembly.]Resources.localization{-Culture}.json"
    ///   "[Assembly.]Resources.{Type.}localization[-Culture].json"
    ///   "Assets/{Type/}localization{-Culture}.ini"
    ///   "Assets/{Assembly/}{Type/}{Section.}localization{-Culture}.ini"
    ///   "{Culture.}{Type.}{Section_0.}{Section_1.}{Section_2.}[Section_n]{.Key_0}{.Key_1}{.Key_n}"
    /// 
    /// </summary>
    public class LinePattern : ILinePattern, ILineParser, ILineAppendParser, ILinePrinter
    {
        static Regex regex = new Regex(
            @"(?<text>[^\[\{\}\]]+)|" +
            @"(?<o_patterntext>\{(?<o_prefix>[^\}a-zA-Z]*)(?<o_identifier>(?<o_parametername>[a-zA-Z]+)(_(?<o_occurance_index>[0-9]+|n))?)(\<(?<o_pattern>([^\\\>\\<]|\\.)*)\>)?(?<o_postfix>[^\}a-zA-Z]*)\})|" +
            @"(?<r_patterntext>\[(?<r_prefix>[^\]a-zA-Z]*)(?<r_identifier>(?<r_parametername>[a-zA-Z]+)(_(?<r_occurance_index>[0-9]+|n))?)(\<(?<r_pattern>([^\\\>\\<]|\\.)*)\>)?(?<r_postfix>[^\]a-zA-Z]*)\])",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        /// <summary>
        /// The number of parts "_n" occurance represents.
        /// </summary>
        static int n = 5;

        /// <summary>
        /// Try to parse <paramref name="pattern"/>.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>pattern or null if parse failed.</returns>
        public static LinePattern TryParse(string pattern)
        {
            try
            {
                return new LinePattern(pattern);
            }
            catch (Exception) { }
            {
                return null;
            }
        }

        /// <summary>
        /// Pattern as string.
        /// </summary>
        public string Pattern { get; internal set; }

        /// <summary>
        /// Pattern as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Pattern;

        /// <summary>
        /// All parts of the pattern
        /// </summary>
        readonly Part[] allParts;

        /// <summary>
        /// All parts of the pattern
        /// </summary>
        public ILinePatternPart[] AllParts => allParts;

        /// <summary>
        /// All parts that capture a part of string.
        /// </summary>
        readonly Part[] captureParts;

        /// <summary>
        /// All parts that capture a part of string.
        /// </summary>
        public ILinePatternPart[] CaptureParts => captureParts;

        /// <summary>
        /// Maps parts by part identifier.
        /// </summary>
        public IReadOnlyDictionary<string, ILinePatternPart> PartMap { get; internal set; }

        /// <summary>
        /// Maps parts by parameter identifier.
        /// </summary>
        public IReadOnlyDictionary<string, ILinePatternPart[]> ParameterMap { get; internal set; }

        /// <summary>
        /// List of all parameter names
        /// </summary>
        public string[] ParameterNames { get; internal set; }

        /// <summary>
        /// Cached regex.
        /// </summary>
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
        public LinePattern(string pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            System.Text.RegularExpressions.MatchCollection matches = regex.Matches(pattern);
            //if (matches.Count == 0) throw new ArgumentException($"Failed to parse filename pattern \"{pattern}\"");

            List<Part> list = new List<Part>(matches.Count);
            int ix = 0, matchIx = 0;
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (!match.Success) throw new ArgumentException($"Failed to parse filename pattern \"{pattern}\"");
                Group g_text = match.Groups["text"];
                Group g_o_identifier = match.Groups["o_identifier"];
                Group g_r_identifier = match.Groups["r_identifier"];

                // Matched as text
                if (g_text.Success)
                {
                    list.Add(new Part { Text = g_text.Value, Index = ix++, CaptureIndex = -1 });
                    continue;
                }

                // Assert that either optional or required groups match
                if (!g_o_identifier.Success && !g_r_identifier.Success) throw new ArgumentException($"Could not parse {nameof(LinePattern)} \"{pattern}\"");

                bool optional = g_o_identifier.Success;

                Group g_pattern = optional ? match.Groups["o_pattern"] : match.Groups["r_pattern"];
                string part_pattern = g_pattern.Success ? g_pattern.Value : null;
                string parameterName = optional ? match.Groups["o_parametername"].Value : match.Groups["r_parametername"].Value;
                string patternText = optional ? match.Groups["o_patterntext"].Value : match.Groups["r_patterntext"].Value;
                string prefix = optional ? match.Groups["o_prefix"].Value : match.Groups["r_prefix"].Value;
                string postfix = optional ? match.Groups["o_postfix"].Value : match.Groups["r_postfix"].Value;
                string identifier = optional ? g_o_identifier.Value : g_r_identifier.Value;
                Group g_occurance = optional ? match.Groups["o_occurance_index"] : match.Groups["r_occurance_index"];

                // "_n"
                bool n_occurances = g_occurance.Success && g_occurance.Value == "n";

                int occuranceIndex = g_occurance.Success && !n_occurances ? int.Parse(g_occurance.Value) : -1;

                Part part = new Part
                {
                    PatternText = patternText, regex = part_pattern == null ? null : new Regex(part_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
                    PrefixSeparator = prefix, PostfixSeparator = postfix, Identifier = occuranceIndex == 0 ? parameterName : identifier,
                    ParameterName = parameterName, Required = !optional, Index = ix++, CaptureIndex = matchIx++,                   
                    OccuranceIndex = occuranceIndex
                };
                list.Add(part);

                // For "_n", add consecutive parts 1 to n.
                if (n_occurances)
                {
                    for (int i=1; i<n; i++)
                    {
                        Part p = new Part()
                        {
                            PatternText = "", regex = null,
                            PrefixSeparator = prefix, PostfixSeparator = postfix,
                            ParameterName = parameterName, Required = false,
                            Index = ix++, CaptureIndex = matchIx++,
                            OccuranceIndex = -1
                        };
                        list.Add(p);
                    }
                }
            }

            // Update -1 occurance indices, which have not yet been assigned. Use the first free occurance
            MapList<string, int> occurances = null;
            foreach (Part part in list.Where(p=>p.CaptureIndex>=0&&p.OccuranceIndex==-1))
            {
                // Make list of indices for each parameter name
                if (occurances == null)
                {
                    occurances = new MapList<string, int>(list.Where(p => p.CaptureIndex >= 0 && p.OccuranceIndex>=0).Select(p => new KeyValuePair<string, int>(p.ParameterName, p.OccuranceIndex)));
                    foreach (var __list in occurances.Values) __list.Sort();
                }

                // Get list for "parameter name"
                int occuranceIndex = 0;
                List<int> _list = occurances.TryGetList(part.ParameterName);
                if (_list != null)
                {
                    foreach (int reservedOccuranceIndex in _list)
                        if (reservedOccuranceIndex == occuranceIndex) occuranceIndex = reservedOccuranceIndex + 1;
                } 
                
                part.OccuranceIndex = occuranceIndex;
                part.Identifier = occuranceIndex == 0 ? part.ParameterName : part.ParameterName + "_" + occuranceIndex;
                occurances.Add(part.ParameterName, occuranceIndex);                
            }

            allParts = list.ToArray();
            captureParts = list.Where(part => part.Identifier != null).ToArray();
            PartMap = CaptureParts.ToDictionary(p => p.Identifier);
            ParameterNames = captureParts.Select(part => part.ParameterName).Distinct().ToArray();
            ParameterMap = ParameterNames.ToDictionary(s => s, parameterName => CaptureParts.Where(part => part.ParameterName == parameterName).OrderBy(p => p.OccuranceIndex).ToArray());
        }

        /// <summary>
        /// Part info
        /// </summary>
        public class Part : ILinePatternPart
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
            public Regex Regex => regex ?? ILinePatternExtensions.GetDefaultPattern(ParameterName);

            /// <summary>
            /// Tests if text is match.
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public bool IsMatch(string text) => Regex == ILinePatternExtensions.GetDefaultPattern(null) ? true : Regex.IsMatch(text);

            /// <summary>
            /// Print part
            /// </summary>
            /// <returns></returns>
            public override string ToString() => PatternText;
        }

        /// <summary>
        /// Match parameters in <paramref name="line"/> with the parts in the pattern.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public ILinePatternMatch Match(ILine line)
        {
            StructList16<ILineParameter> parameters = new StructList16<ILineParameter>();
            line.GetParameterParts<StructList16<ILineParameter>>(ref parameters);
            LinePatternMatch match = new LinePatternMatch(this);
            // parameters is in reverse order, so iterate from end which is actually the root
            for(int i=parameters.Count-1; i>=0; i--)
            {
                ILineParameter parameter = parameters[i];
                MatchParameter(parameter.ParameterName, parameter.ParameterValue, match);
            }
            return match;
        }

        void MatchParameter(string parameterName, string parameterValue, LinePatternMatch match)
        {
            // Search parts
            ILinePatternPart[] parts;
            if (ParameterMap.TryGetValue(parameterName, out parts))
            {
                // Iterate each part
                foreach (ILinePatternPart part in parts)
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
                foreach (ILinePatternPart part in parts)
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
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>key result or null if contained no content</returns>
        /// <exception cref="LineException">If parse failed</exception>
        public ILine Parse(string str, ILine prevPart = default, ILineFactory appender = default)
        {
            // Get appender
            if (appender == null) appender = prevPart.GetAppender();

            // Match
            ILinePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) throw new LineException(null, $"Key \"{str}\" did not match the pattern \"{Pattern}\"");

            // Append to line
            for (int i = 0; i < match.PartValues.Length; i++)
            {
                string value = match.PartValues[i];
                if (value == null) continue;
                string key = CaptureParts[i].ParameterName;
                if (key == "anysection") key = "Section";
                prevPart = appender.Create<ILineParameter, string, string>(prevPart, key, value);
            }

            // Return line
            return prevPart;
        }

        /// <summary>
        /// Parse to parameter arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public IEnumerable<ILineArguments> Parse(string str)
        {
            // Match
            ILinePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) throw new LineException(null, $"Key \"{str}\" did not match the pattern \"{Pattern}\"");

            // Count parts
            int count = 0;
            foreach (string partValue in match.PartValues) if (partValue != null) count++;

            // Create args
            ILineArguments[] result = new ILineArguments[count];
            int ix = 0;
            for (int i=0; i<match.PartValues.Length; i++)
            {
                string value = match.PartValues[i];
                if (value == null) continue;
                string key = CaptureParts[i].ParameterName;
                if (key == "anysection") key = "Section";
                result[ix++] = new ParameterArgument(key, value);
            }

            // Return args
            return result;
        }

        /// <summary>
        /// Parse string into key.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result">key result or null if contained no content</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>true if parse was successful</returns>
        public bool TryParse(string str, out ILine result, ILine prevPart = default, ILineFactory appender = default)
        {
            // Get appender
            if (appender == null && !prevPart.TryGetAppender(out appender)) { result = null; return false; }

            // Match
            ILinePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) { result = null; return false; }

            // Append to line
            for (int i = 0; i < match.PartValues.Length; i++)
            {
                string value = match.PartValues[i];
                if (value == null) continue;
                string key = CaptureParts[i].ParameterName;
                if (key == "anysection") key = "Section";
                ILineParameter parameter;
                if (appender.TryCreate<ILineParameter, string, string>(prevPart, key, value, out parameter)) prevPart = parameter; else { result = null; return false; }
            }

            // Return line
            result = prevPart;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool TryParse(string str, out IEnumerable<ILineArguments> args)
        {
            // Match
            ILinePatternMatch match = this.Match(text: str, filledParameters: null);
            if (!match.Success) { args = null; return false; }

            // Count parts
            int count = 0;
            foreach (string partValue in match.PartValues) if (partValue != null) count++;

            // Create args
            ILineArguments[] result = new ILineArguments[count];
            int ix = 0;
            for (int i = 0; i < match.PartValues.Length; i++)
            {
                string value = match.PartValues[i];
                if (value == null) continue;
                string key = CaptureParts[i].ParameterName;
                if (key == "anysection") key = "Section";
                result[ix++] = new ParameterArgument(key, value);
            }

            // Return args
            args = result;
            return true;
        }

        /// <summary>
        /// Print line as string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Print(ILine key)
        {
            ILinePatternMatch match = Match(key);
            return ILinePatternExtensions.Print(this, match.PartValues);
        }

        /// <summary>
        /// Get hashcode for pattern.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => Pattern.GetHashCode() ^ 0x4234234;

        /// <summary>
        /// Compare object for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is LinePattern other)
                return other.Pattern == Pattern;
            return false;
        }

        class ParameterArgument : ILineArguments<ILineParameter, string, string>
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            readonly string ParameterName, ParameterValue;
            public ParameterArgument(string parameterName, string parameterValue)
            {
                ParameterName = parameterName;
                ParameterValue = parameterValue;
            }
        }
    }

    /// <summary></summary>
    public static partial class LinePatternExtensions
    {
        /// <summary>
        /// Convert <paramref name="match"/> into an asset key that contains the captured parameters.
        /// </summary>
        /// <param name="match"></param>
        /// <returns>key or null if <paramref name="match"/> contained no values</returns>
        public static ILine ToKey(this ILinePatternMatch match)
        {
            ILine result = null;
            foreach(ILinePatternPart part in match.Pattern.CaptureParts)
            {
                string value = match[part.CaptureIndex];
                if (value == null) continue;
                result = LineAppender.Default.Create<ILineParameter, string, string>(result, part.ParameterName, value);
            }
            return result;
        }

        /// <summary>
        /// Convert <paramref name="match"/> into an enumeration of parameter names and values.
        /// </summary>
        /// <param name="match">(optional)</param>
        /// <returns>parameter names and values</returns>
        public static IEnumerable<KeyValuePair<string, string>> ToParameters(this ILinePatternMatch match)
        {
            if (match == null) yield break;
            foreach (ILinePatternPart part in match.Pattern.CaptureParts)
            {
                string value = match[part.CaptureIndex];
                if (value == null) continue;
                yield return new KeyValuePair<string, string>(part.ParameterName, value);
            }
        }

        /// <summary>
        /// Convert "match" parameters into an array of "non-match" parameters.
        /// 
        /// "match" parameter is a parameter in the format of <see cref="ILinePattern"/>, where match contains
        /// capture index "_#". For example "section_0", "section_1". The capture index is removed from the result of 
        /// this function. Keys are orderd by this index.
        /// 
        /// "non-match" parameter does not have "anysection" nor capture index "_#".
        /// 
        /// "anysection" is converted to "Section".
        /// 
        /// Parameters are returned in the following order:
        ///  "Culture", "Assembly", "Location", "Resource", "Type", "Section", other parts here in alphabetical order, "Key", "N", "N1", "N2", "N3", "N4", "N5", "N6", ....
        ///  
        /// This is workaround as the order information is lost in the dictionary format.
        /// </summary>
        /// <param name="matchParameters">(optional) </param>
        /// <returns>converted parameters</returns>
        public static IEnumerable<KeyValuePair<string, string>> ConvertMatchParametersToNonMatchParameters(IReadOnlyDictionary<string, string> matchParameters)
        {
            if (matchParameters == null) return new KeyValuePair<string, string>[0];
            if (matchParameters is ILinePatternMatch match) return ToParameters(match);

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
                IParameterInfo info;

                // Get name
                string name = g_name.Value;
                if (name == "anysection") name = "Section";
                if (ParameterInfos.Default.TryGetValue(name, out info)) ix = info.Order;

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

    }

}
