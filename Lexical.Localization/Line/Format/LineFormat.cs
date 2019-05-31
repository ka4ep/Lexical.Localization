// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Parameter policy that uses the following format "Key:Value:...". 
    /// </summary>
    public class LineFormat : ILinePrinter, ILineParser, ILineAppendParser, ILineFormatFactory
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

        /// <summary>
        /// Parameter qualifier that excludes parameter "Value".
        /// </summary>
        static readonly ILineQualifier ExcludeValue = new LineQualifierRule.IsEqualTo("Value", -1, ""); // new LineParameterQualifierComposition().Rule("Value", -1, "");

        /// <summary>
        /// Format that prints and parses strings as parameter lines. Excludes "Value" parameter.
        /// </summary>
        static readonly LineFormat parameters = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.NonResolving, ExcludeValue);

        /// <summary>
        /// Format that prints and parses strings as parameter lines. 
        /// </summary>
        static readonly LineFormat parametersInclValue = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.NonResolving, null);

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters are resolved to default instance types. For example "Culture" to CultureInfo. Excludes "Value" parameter.
        /// </summary>
        static readonly LineFormat key = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.Default, ExcludeValue);

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters and values are resolved to default instance types. For example "Culture" to CultureInfo, and "Value" to <see cref="IFormatString"/>.
        /// </summary>
        static readonly LineFormat line = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.Default, null);

        /// <summary>
        /// Format that prints and parses strings as parameter lines. Excludes "Value" parameter.
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineKeyNonCanonical("Culture", "en").LineKeyNonCanonical("Key", "x")
        /// </summary>
        public static LineFormat Parameters => parameters;

        /// <summary>
        /// Format that prints and parses strings as parameter lines. 
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineKeyNonCanonical("Culture", "en").LineKeyNonCanonical("Key", "x").LineHint("Value", "z")
        /// </summary>
        public static LineFormat ParametersInclValue => parametersInclValue;

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters are resolved to default instance types. For example "Culture" to CultureInfo. Excludes "Value" parameter.
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineCulture("en").LineKeyNonCanonical("Key", "x")
        /// </summary>
        public static LineFormat Key => key;

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters and values are resolved to default instance types. For example "Culture" to CultureInfo, and "Value" to <see cref="IFormatString"/>.
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineCulture("en").LineKeyNonCanonical("Key", "x").LineValue("z")
        /// </summary>
        public static LineFormat Line => line;

        /// <summary>
        /// Pattern that parses "ParameterName:ParameterValue" texts.
        /// </summary>
        protected Regex ParsePattern = new Regex(@"(?<key>([^:\\]|\\.)*)\:(?<value>([^:\\]|\\.)*)(\:|$)", opts);

        /// <summary>
        /// Pattern that escapes string literal.
        /// </summary>
        protected Regex LiteralEscape;

        /// <summary>
        /// Pattern that unescapes string literal.
        /// </summary>
        protected Regex LiteralUnescape;

        /// <summary>
        /// Match delegate that escapes a character match.
        /// </summary>
        protected MatchEvaluator escapeChar;

        /// <summary>
        /// Match delegate that unescapes a character match.
        /// </summary>
        protected MatchEvaluator unescapeChar;

        /// <summary>
        /// Qualifier that validates parameters.
        /// </summary>
        public virtual ILineQualifier Qualifier { get => qualifier; set => new InvalidOperationException("immutable"); }

        /// <summary>
        /// Line appender that creates line parts from parsed strings.
        /// 
        /// Appender implementation may also resolve parameter into instance, for example "Culture" into <see cref="CultureInfo"/>.
        /// </summary>
        public virtual ILineFactory LineFactory { get => lineFactory; set => throw new InvalidOperationException("immutable"); }

        /// <summary>
        /// (optional) Qualifier that validates parameters.
        /// </summary>
        protected ILineQualifier qualifier;

        /// <summary>
        /// Line appender
        /// </summary>
        protected ILineFactory lineFactory;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacters">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharacters">Unescape tnab0f</param>
        /// <param name="lineAppender">line appender that can append <see cref="ILineParameter"/> and <see cref="ILineValue"/></param>
        /// <param name="qualifier">(optional) parameter qualifier</param>
        public LineFormat(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacters, bool unescapeControlCharacters, ILineFactory lineAppender, ILineQualifier qualifier)
        {
            // Regex.Escape doen't work for brackets []
            //string escapeCharactersEscaped = Regex.Escape(escapeCharacters);
            string escapeCharactersEscaped = escapeCharacters.Select(c => c == ']' ? "\\]" : Regex.Escape("" + c)).Aggregate((a, b) => a + b);
            if (escapeControlCharacters)
            {
                LiteralEscape = new Regex("[" + escapeCharactersEscaped + "]|[\\x00-\\x1f]", opts);
            }
            else
            {
                LiteralEscape = new Regex("[" + escapeCharactersEscaped + "]", opts);
            }
            escapeChar = EscapeChar;

            string unescapeCharactersEscaped = unescapeCharacters.Select(c => c == ']' ? "\\]" : Regex.Escape("" + c)).Aggregate((a, b) => a + b);
            LiteralUnescape = new Regex(
                unescapeControlCharacters ?
                "\\\\([0abtfnrv" + unescapeCharactersEscaped + "]|x[0-9a-fA-F]{2}|u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8})" :
                "\\\\[" + unescapeCharactersEscaped + "]"
                , opts);
            unescapeChar = UnescapeChar;

            this.lineFactory = lineAppender ?? throw new ArgumentNullException(nameof(lineAppender));
            this.qualifier = qualifier;
        }

        /// <summary>
        /// Convert a sequence of key,value pairs to a string.
        /// 
        /// The format is as following:
        ///   parameterKey:parameterValue:parameterKey:parameterValue:...
        /// 
        /// Escape character is backslash.
        ///  \unnnn
        ///  \xnnnn
        ///  \:
        ///  \\
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Print(ILine key)
        {
            StringBuilder sb = new StringBuilder();
            Print(key, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Convert a sequence of key,value pairs to a string.
        /// 
        /// The format is as following:
        ///   parameterKey:parameterValue:parameterKey:parameterValue:...
        /// 
        /// Escape character is backslash.
        ///  \unnnn
        ///  \xnnnn
        ///  \:
        ///  \\
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sb"></param>
        /// <returns><paramref name="sb"/></returns>
        public StringBuilder Print(ILine key, StringBuilder sb)
        {
            if (Qualifier != null)
            {
                StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
                key.GetParameterPartsWithOccurance<StructList12<(ILineParameter, int)>>(ref list);
                for (int i = list.Count-1; i >= 0; i--)
                {
                    if (i < list.Count-1) sb.Append(':');
                    (ILineParameter parameter, int occ) = list[i];
                    if (!Qualifier.Qualify(parameter, occ)) continue;
                    sb.Append(EscapeLiteral(parameter.ParameterName));
                    sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.ParameterValue));
                }
                return sb;
            }
            else
            {
                StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                key.GetParameterParts<StructList12<ILineParameter>>(ref list);
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (i < list.Count - 1) sb.Append(':');
                    var parameter = list[i];
                    sb.Append(EscapeLiteral(parameter.ParameterName));
                    sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.ParameterValue));
                }
                return sb;
            }
        }

        /// <summary>
        /// Convert a sequence of key,value pairs to a string.
        /// 
        /// The format is as following:
        ///   parameterKey:parameterValue:parameterKey:parameterValue:...
        /// 
        /// Escape character is backslash.
        ///  \unnnn
        ///  \xnnnn
        ///  \:
        ///  \\
        /// </summary>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public string PrintParameters(IEnumerable<KeyValuePair<string, string>> keyParameters)
        {
            StringBuilder sb = new StringBuilder();
            if (Qualifier != null)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (var parameter in keyParameters)
                {
                    int occ = AddOccurance(ref list, parameter.Key);
                    if (!Qualifier.Qualify(new ParameterArgument(parameter.Key, parameter.Value), occ)) continue;
                    if (sb.Length > 0) sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Key));
                    sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Value));
                }
            }
            else
            {
                foreach (var parameter in keyParameters)
                {
                    if (sb.Length > 0) sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Key));
                    sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Value));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parse string into ILine.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>result key, or null if it contained no parameters and <paramref name="prevPart"/> was null.</returns>
        /// <exception cref="LineException">The parameter is not of the correct format.</exception>
        public virtual ILine Parse(string str, ILine prevPart = default, ILineFactory appender = default)
        {
            if (appender == null) appender = lineFactory;
            if (appender == null) appender = prevPart.GetAppender();
            MatchCollection matches = ParsePattern.Matches(str);

            // With qualifier
            if (Qualifier != null)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineParameter parameter = lineFactory.Create<ILineParameter, string, string>(prevPart, parameterName, parameterValue);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualifier.Qualify(parameter, occ)) continue;
                    prevPart = parameter;
                }
            }
            else
            // No qualifier
            {
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    prevPart = lineFactory.Create<ILineParameter, string, string>(prevPart, parameterName, parameterValue);
                }
            }
            return prevPart;
        }


        /// <summary>
        /// Parse to parameter arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public IEnumerable<ILineArguments> ParseArgs(string str)
        {
            MatchCollection matches = ParsePattern.Matches(str);
            if (Qualifier != null)
            {
                StructList8<ILineArguments> result = new StructList8<ILineArguments>();
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                for (int i = 0; i < matches.Count; i++)
                {
                    Match m = matches[i];
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineParameter parameter = lineFactory.Create<ILineParameter, string, string>(null, parameterName, parameterValue);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualifier.Qualify(parameter, occ)) continue;
                    result.Add(ToArgument(parameter));
                }
                return result.ToArray();
            }
            else
            {
                int count = matches.Count;
                ILineArguments[] result = new ILineArguments[count];
                for (int i = 0; i < count; i++)
                {
                    Match m = matches[i];
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    result[i] = ToArgument(lineFactory.Create<ILineParameter, string, string>(null, parameterName, parameterValue));
                }
                return result;
            }
        }

        /// <summary>
        /// Try parse string into ILine.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="result">result key, or null if it contained no parameters and <paramref name="prevPart"/> was null.</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>true if parse was successful</returns>
        public virtual bool TryParse(string keyString, out ILine result, ILine prevPart = default, ILineFactory appender = default)
        {
            if (appender == null) appender = lineFactory;
            if (appender == null && !prevPart.TryGetAppender(out appender)) { result = null; return false; }
            MatchCollection matches = ParsePattern.Matches(keyString);
            if (Qualifier != null)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) { result = null; return false; }
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineParameter parameter;
                    if (!LineFactory.TryCreate<ILineParameter, string, string>(prevPart, parameterName, parameterValue, out parameter)) { result = null; return false; }
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualifier.Qualify(parameter, occ)) continue;
                    prevPart = parameter;
                }
                result = prevPart;
                return true;
            }
            else
            {
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) { result = null; return false; }
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineParameter parameter;
                    if (!LineFactory.TryCreate<ILineParameter, string, string>(prevPart, parameterName, parameterValue, out parameter)) { result = null; return false; }
                    prevPart = parameter;
                }
                result = prevPart;
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool TryParseArgs(string str, out IEnumerable<ILineArguments> args)
        {
            MatchCollection matches = ParsePattern.Matches(str);
            if (Qualifier != null)
            {
                StructList8<ILineArguments> result = new StructList8<ILineArguments>();
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                ILineParameter tmp = null;
                for (int i = 0; i < matches.Count; i++)
                {
                    Match m = matches[i];
                    if (!m.Success) { args = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    if (!lineFactory.TryCreate<ILineParameter, string, string>(tmp, parameterName, parameterValue, out tmp)) { args = null; return false; }
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualifier.Qualify(tmp, occ)) continue;
                    result.Add(ToArgument(tmp));
                }
                args = result.ToArray();
                return true;
            }
            else
            {
                int count = matches.Count;
                ILineArguments[] result = new ILineArguments[count];
                ILineParameter tmp = null;
                for (int i = 0; i < count; i++)
                {
                    Match m = matches[i];
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    if (!lineFactory.TryCreate<ILineParameter, string, string>(tmp, parameterName, parameterValue, out tmp)) { args = null; return false; }
                    result[i] = ToArgument(tmp);
                }
                args = result;
                return true;
            }
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns>parameters</returns>
        /// <exception cref="LineException">The parameter is not of the correct format.</exception>
        public IEnumerable<KeyValuePair<string, string>> ParseParameters(string keyString)
        {
            MatchCollection matches = ParsePattern.Matches(keyString);
            if (Qualifier != null)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                ILineParameter tmp = null;
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, keyString);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    int occ = AddOccurance(ref list, parameterName);
                    if (lineFactory.TryCreate<ILineParameter, string, string>(tmp, parameterName, parameterValue, out tmp) && !Qualifier.Qualify(tmp, occ)) continue;
                    yield return new KeyValuePair<string, string>(parameterName, parameterValue);
                }
            }
            else
            {
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, keyString);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    yield return new KeyValuePair<string, string>(parameterName, parameterValue);
                }
            }
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="result">result to write result to</param>
        /// <returns>true if successful, if false then parse failed and <paramref name="result"/>is in undetermined state</returns>
        public bool TryParseParameters(string keyString, ICollection<KeyValuePair<string, string>> result)
        {
            if (keyString == null) return false;
            MatchCollection matches = ParsePattern.Matches(keyString);
            if (Qualifier != null)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                ILineParameter tmp = null;
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) return false;
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    int occ = AddOccurance(ref list, parameterName);
                    if (lineFactory.TryCreate<ILineParameter, string, string>(tmp, parameterName, parameterValue, out tmp) && !Qualifier.Qualify(tmp, occ)) continue;
                    result.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
                }
            }
            else
            {
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) return false;
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    result.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
                }
            }
            return true;
        }

        /// <summary>
        /// Escape literal and output it to string builder.
        /// </summary>
        public String EscapeLiteral(String input) => LiteralEscape.Replace(input, escapeChar);

        /// <summary>
        /// Unescape string literal.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public String UnescapeLiteral(String input) => LiteralUnescape.Replace(input, unescapeChar);

        static String EscapeChar(Match m)
        {
            string s = m.Value;
            char _ch = s[0];
            switch (_ch)
            {
                case '\\': return "\\\\";
                case '\0': return "\\0";
                case '\a': return "\\a";
                case '\b': return "\\b";
                case '\v': return "\\v";
                case '\t': return "\\t";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
            }
            if (_ch < 32) return "\\x" + ((uint)_ch).ToString("x2", CultureInfo.InvariantCulture);
            if (_ch >= 0xD800 && _ch <= 0xDFFF) return "\\u" + ((uint)_ch).ToString("x4", CultureInfo.InvariantCulture);
            return "\\" + _ch;
        }

        static String UnescapeChar(Match m)
        {
            string s = m.Value;
            char _ch = s[1];
            switch (_ch)
            {
                case '0': return "\0";
                case 'a': return "\a";
                case 'b': return "\b";
                case 'v': return "\v";
                case 't': return "\t";
                case 'f': return "\f";
                case 'n': return "\n";
                case 'r': return "\r";
                case '\\': return "\\";
                case 'x':
                    char ch = (char)Hex.ToUInt(s, 2);
                    return new string(ch, 1);
                case 'u':
                    char ch_ = (char)Hex.ToUInt(s, 2);
                    return new string(ch_, 1);
                case 'U':
                    uint c = Hex.ToUInt(s, 2);
                    return new string((char)(c >> 16), 1) + new string((char)(c & 0xffffU), 1);
                default:
                    return new string(_ch, 1);
            }
        }

        /// <summary>
        /// Add occurance index of a specific parameter name.
        /// </summary>
        /// <param name="paramOccurances">catalog of parameter occurances</param>
        /// <param name="parameterName"></param>
        /// <returns>occurance of <paramref name="parameterName"/></returns>
        static int AddOccurance(ref StructList8<(string, int)> paramOccurances, string parameterName)
        {
            for (int i = 0; i < paramOccurances.Count; i++)
            {
                (string name, int occ) = paramOccurances[i];
                if (name == parameterName)
                {
                    paramOccurances[i] = (name, occ + 1);
                    return occ + 1;
                }
            }
            paramOccurances.Add((parameterName, 0));
            return 0;
        }

        ILineArguments ToArgument(ILineParameter parameter)
        {
            if (parameter is ILineArguments args) return args;
            if (parameter is ILineHint hint) return new HintArgument(parameter.ParameterName, parameter.ParameterValue);
            if (parameter is ILineCanonicalKey canonicalKey) return new KeyCanonicalArgument(parameter.ParameterName, parameter.ParameterValue);
            if (parameter is ILineNonCanonicalKey nonCanonicalKey) return new KeyNonCanonicalArgument(parameter.ParameterName, parameter.ParameterValue);
            return null;
        }

        class Argument<T> : ILineArguments<T, string, string>, ILineParameter
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public Argument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }
        class ParameterArgument : ILineArguments<ILineParameter, string, string>, ILineParameter
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public ParameterArgument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }
        class HintArgument : ILineArguments<ILineHint, string, string>, ILineHint
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public HintArgument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }
        class KeyCanonicalArgument : ILineArguments<ILineCanonicalKey, string, string>, ILineCanonicalKey
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public KeyCanonicalArgument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }
        class KeyNonCanonicalArgument : ILineArguments<ILineNonCanonicalKey, string, string>, ILineNonCanonicalKey
        {
            public string Argument0 => ParameterName;
            public string Argument1 => ParameterValue;
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
            public KeyNonCanonicalArgument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }

    }

}
