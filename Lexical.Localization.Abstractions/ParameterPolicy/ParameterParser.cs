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

namespace Lexical.Localization
{
    /// <summary>
    /// Parameter policy that uses the following format "Key:Value:...".
    /// </summary>
    public class ParameterParser
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        static ParameterParser instance = new ParameterParser("\\:", false, "\\:", false);

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public static ParameterParser Instance => instance;

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
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacters">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharacters">Unescape tnab0f</param>
        public ParameterParser(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacters, bool unescapeControlCharacters)
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

            _parameterVisitor = parameterVisitor;
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
        public string PrintKey(ILinePart key)
        {
            StringBuilder sb = new StringBuilder();
            key.VisitFromRoot(_parameterVisitor, ref sb);
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
        public StringBuilder PrintKey(ILinePart key, StringBuilder sb)
        {
            key.VisitFromRoot(_parameterVisitor, ref sb);
            return sb;
        }

        LinePartVisitor<StringBuilder> _parameterVisitor;
        void parameterVisitor(ILinePart key, ref StringBuilder sb)
        {
            if (key is ILineParameter parameter)
            {
                ILineParameter prevKey = key.GetPreviousParameterPart();
                if (prevKey != null) sb.Append(':');
                sb.Append(EscapeLiteral(parameter.ParameterName));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.ParameterValue));
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
        /// Parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="rootKey">root key to span values from</param>
        /// <returns>result key, or null if it contained no parameters and <paramref name="rootKey"/> was null.</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public virtual ILinePart Parse(string keyString, ILinePart rootKey)
        {
            ILinePart result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result = result.Parameter(key, value);
            }
            return result;
        }

        /// <summary>
        /// Try parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="resultKey">result key, or null if it contained no parameters and <paramref name="rootKey"/> was null.</param>
        /// <param name="rootKey">root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        public virtual bool TryParse(string keyString, out ILinePart resultKey, ILinePart rootKey)
        {
            ILinePart result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) { resultKey = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { resultKey = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result = result.Parameter(key, value);
            }
            resultKey = result;
            return true;
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns>parameters</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public IEnumerable<KeyValuePair<string, string>> ParseParameters(string keyString)
        {
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
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

    }

}
