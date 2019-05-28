// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
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
    /// Parses to <see cref="ILineParameter"/> parts.
    /// </summary>
    public class LineFormat : LineParameterQualifierComposition, ILinePrinter, ILineParser, ILineAppendParser, ILineFormatParameterInfos
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

        /// <summary>
        /// Format that prints and parses parameters except "Value" parameter.
        /// 
        /// Uses <see cref="ParameterInfos.Default"/> to instantiate known keys and hints as they are.
        /// Unknown parameters are instantated as <see cref="ILineParameter"/> and left for the caller to interpret.
        /// </summary>
        static LineFormat parameters = new LineFormat("\\:", false, "\\:", false, Utils.ParameterInfos.Default).Rule("Value", -1, "").SetReadonly() as LineFormat;

        /// <summary>
        /// Format that prints and parses all parameters.
        /// 
        /// Uses <see cref="ParameterInfos.Default"/> to instantiate known keys and hints as they are.
        /// Unknown parameters are instantated as <see cref="ILineParameter"/> and left for the caller to interpret.
        /// </summary>
        static LineFormat parametersInclValue = new LineFormat("\\:", false, "\\:", false, Utils.ParameterInfos.Default).SetReadonly() as LineFormat;

        /// <summary>
        /// Format that prints and parses parameters 
        /// </summary>
        public static LineFormat Parameters => parameters;

        /// <summary>
        /// Format that prints and parses all parameters.
        /// 
        /// Uses <see cref="ParameterInfos.Default"/> to instantiate known keys and hints as they are.
        /// Unknown parameters are instantated as <see cref="ILineParameter"/> and left for the caller to interpret.
        /// </summary>
        public static LineFormat ParametersInclValue => parametersInclValue;

        /// <summary>
        /// Parameter infos.
        /// </summary>
        public IParameterInfos ParameterInfos { get => parameterInfos; set => new InvalidOperationException("read-only"); }

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
        /// (optional) Parameter infos for determining if parameter is key.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacters">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharacters">Unescape tnab0f</param>
        /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key. <see cref="ParameterInfos.Default"/> for default infos.</param>
        public LineFormat(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacters, bool unescapeControlCharacters, IParameterInfos parameterInfos = null)
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

            this.parameterInfos = parameterInfos;
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
            if (HasParameterRules)
            {
                StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
                key.GetParameterPartsWithOccurance<StructList12<(ILineParameter, int)>>(ref list);
                for (int i = list.Count-1; i >= 0; i--)
                {
                    if (i < list.Count-1) sb.Append(':');
                    (ILineParameter parameter, int occ) = list[i];
                    if (!Qualify(parameter, occ)) continue;
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
            if (HasParameterRules)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (var parameter in keyParameters)
                {
                    int occ = AddOccurance(ref list, parameter.Key);
                    if (!Qualify(new ParameterArgument(parameter.Key, parameter.Value), occ)) continue;
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
            if (appender == null) appender = prevPart.GetAppender();
            MatchCollection matches = ParsePattern.Matches(str);
            if (HasParameterRules)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    int occ = AddOccurance(ref list, parameterName);
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    if (!Qualify(arg as ILineParameter, occ)) continue;
                    prevPart = appender.Create(prevPart, arg);
                }
            }
            else
            {
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, str);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    prevPart = appender.Create(prevPart, arg);
                }
            }
            return prevPart;
        }

        ILineArguments CreateLineArgument(string parameterName, string parameterValue)
        {
            IParameterInfo parameterInfo;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out parameterInfo))
            {
                if (parameterInfo.InterfaceType == typeof(ILineParameter)) return new ParameterArgument(parameterName, parameterValue);
                if (parameterInfo.InterfaceType == typeof(ILineHint)) return new HintArgument(parameterName, parameterValue);
                if (parameterInfo.InterfaceType == typeof(ILineCanonicalKey)) return new KeyCanonicalArgument(parameterName, parameterValue);
                if (parameterInfo.InterfaceType == typeof(ILineNonCanonicalKey)) return new KeyNonCanonicalArgument(parameterName, parameterValue);
                return (ILineArguments)typeof(Argument<>).MakeGenericType(parameterInfo.InterfaceType).GetConstructors()[0].Invoke(new object[] { parameterName, parameterValue });
            }
            return new ParameterArgument(parameterName, parameterValue);
        }

        /// <summary>
        /// Parse to parameter arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public IEnumerable<ILineArguments> ParseArgs(string str)
        {
            MatchCollection matches = ParsePattern.Matches(str);
            if (HasParameterRules)
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
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualify(arg as ILineParameter, occ)) continue;
                    result.Add(arg);
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
                    result[i] = CreateLineArgument(parameterName, parameterValue);
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
            if (appender == null && !prevPart.TryGetAppender(out appender)) { result = null; return false; }
            MatchCollection matches = ParsePattern.Matches(keyString);
            if (HasParameterRules)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) { result = null; return false; }
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualify(arg as ILineParameter, occ)) continue;
                    ILine tmp;
                    if (appender.TryCreate(prevPart, arg, out tmp)) prevPart = tmp; else { result = null; return false; }
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
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    ILine tmp;
                    if (appender.TryCreate(prevPart, arg, out tmp)) prevPart = tmp; else { result = null; return false; }
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
            if (HasParameterRules)
            {
                StructList8<ILineArguments> result = new StructList8<ILineArguments>();
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                for (int i = 0; i < matches.Count; i++)
                {
                    Match m = matches[i];
                    if (!m.Success) { args = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    ILineArguments arg = CreateLineArgument(parameterName, parameterValue);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualify(arg as ILineParameter, occ)) continue;
                    result.Add(arg);
                }
                args = result.ToArray();
                return true;
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
                    result[i] = CreateLineArgument(parameterName, parameterValue);
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
            if (HasParameterRules)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new LineException(null, keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new LineException(null, keyString);
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualify((ILineParameter)CreateLineArgument(parameterName, parameterValue), occ)) continue;
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
            if (HasParameterRules)
            {
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) return false;
                    string parameterName = UnescapeLiteral(k_key.Value);
                    string parameterValue = UnescapeLiteral(k_value.Value);
                    int occ = AddOccurance(ref list, parameterName);
                    if (!Qualify((ILineParameter)CreateLineArgument(parameterName, parameterValue), occ)) continue;
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
