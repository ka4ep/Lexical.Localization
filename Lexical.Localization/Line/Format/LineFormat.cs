// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Parameter policy that uses the following format "Key:Value:...". 
    /// Parses to <see cref="ILineParameter"/> parts.
    /// </summary>
    public class LineFormat : LineParameterFilterComposition, ILinePrinter, ILineParser, ILineAppendParser
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

        /// <summary>
        /// Format that prints and parses parameters except "Value" parameter.
        /// 
        /// Uses <see cref="ParameterInfos.Default"/> to instantiate known keys and hints as they are.
        /// Unknown parameters are instantated as <see cref="ILineParameter"/> and left for the caller to interpret.
        /// </summary>
        static LineFormat parameters = new LineFormat("\\:", false, "\\:", false, ParameterInfos.Default).Rule("Value", -1, "").SetReadonly() as LineFormat;

        /// <summary>
        /// Format that prints and parses all parameters.
        /// 
        /// Uses <see cref="ParameterInfos.Default"/> to instantiate known keys and hints as they are.
        /// Unknown parameters are instantated as <see cref="ILineParameter"/> and left for the caller to interpret.
        /// </summary>
        static LineFormat parametersInclValue = new LineFormat("\\:", false, "\\:", false, ParameterInfos.Default).SetReadonly() as LineFormat;

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
        protected IReadOnlyDictionary<string, IParameterInfo> parameterInfos;


        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacters">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharacters">Unescape tnab0f</param>
        /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key. <see cref="ParameterInfos.Default"/> for default infos.</param>
        public LineFormat(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacters, bool unescapeControlCharacters, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            // Regex.Escape doen't work for brackets []
            //string escapeCharactersEscaped = Regex.Escape(escapeCharacters);
            string escapeCharactersEscaped = escapeCharacters.Select(c => c == ']' ? "\\]" : Regex.Escape(""+c)).Aggregate((a,b)=>a+b);
            if (escapeControlCharacters)
            {
                LiteralEscape = new Regex("[" + escapeCharactersEscaped + "]|[\\x00-\\x1f]", opts);
            } else
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
            StructList12<ILineParameter> list = new StructList12<ILineParameter>();
            key.GetParameterParts<StructList12<ILineParameter>>(ref list);
            for (int i=0; i<list.Count; i++)
            {
                if (i>0) sb.Append(':');
                var parameter = list[i];
                sb.Append(EscapeLiteral(parameter.ParameterName));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.ParameterValue));
            }
            return sb;
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
            foreach (var parameter in keyParameters)
            {
                if (sb.Length > 0) sb.Append(':');
                sb.Append(EscapeLiteral(parameter.Key));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.Value));
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
            foreach (Match m in matches)
            {
                if (!m.Success) throw new LineException(null, str);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                prevPart = appender.Create<ILineParameter, string, string>(prevPart, key, value);
            }
            return prevPart;
        }

        /// <summary>
        /// Parse to parameter arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public IEnumerable<ILineArguments> Parse(string str)
        {
            MatchCollection matches = ParsePattern.Matches(str);
            int count = matches.Count;
            ILineArguments[] result = new ILineArguments[count];
            for (int i=0; i<count; i++)
            {
                Match m = matches[i];
                if (!m.Success) throw new LineException(null, str);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result[i] = new ParameterArgument(key, value);
            }
            return result;
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
            foreach (Match m in matches)
            {
                if (!m.Success) { result = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { result = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                ILineParameter parameter;
                if (appender.TryCreate<ILineParameter, string, string>(prevPart, key, value, out parameter)) prevPart = parameter; else { result = null; return false; }
            }
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
            MatchCollection matches = ParsePattern.Matches(str);
            int count = matches.Count;
            ILineArguments[] result = new ILineArguments[count];
            for (int i = 0; i < count; i++)
            {
                Match m = matches[i];
                if (!m.Success) { args = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { args = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result[i] = new ParameterArgument(key, value);
            }
            args = result;
            return true;
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
            foreach (Match m in matches)
            {
                if (!m.Success) throw new LineException(null, keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new LineException(null, keyString);
                string name = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                yield return new KeyValuePair<string, string>(name, value);
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
            foreach (Match m in matches)
            {
                if (!m.Success) { result = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) return false;
                string name = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result.Add(new KeyValuePair<string, string>(name, value));
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

}
