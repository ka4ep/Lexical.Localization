// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
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
    public class LineFormat : LineFormatBase
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

        /// <summary>
        /// Format that prints and parses strings as parameter lines. Excludes "String" parameter.
        /// </summary>
        static readonly LineFormat parameters = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.NonResolving, ExcludeValue);

        /// <summary>
        /// Format that prints and parses strings as parameter lines. 
        /// </summary>
        static readonly LineFormat parametersInclString = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.NonResolving, null);

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters are resolved to default instance types. For example "Culture" to CultureInfo. Excludes "String" parameter.
        /// </summary>
        static readonly LineFormat key = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.Default, ExcludeValue);

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters and values are resolved to default instance types. For example "Culture" to CultureInfo, and "String" to <see cref="IString"/>.
        /// </summary>
        static readonly LineFormat line = new LineFormat("\\:", false, "\\:", false, Lexical.Localization.LineAppender.Default, null);

        /// <summary>
        /// Format that prints and parses strings as parameter lines. Excludes "String" parameter.
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineKeyNonCanonical("Culture", "en").LineKeyNonCanonical("Key", "x")
        /// </summary>
        public static LineFormat Parameters => parameters;

        /// <summary>
        /// Format that prints and parses strings as parameter lines. 
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineKeyNonCanonical("Culture", "en").LineKeyNonCanonical("Key", "x").LineHint("String", "z")
        /// </summary>
        public static LineFormat ParametersInclString => parametersInclString;

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters are resolved to default instance types. For example "Culture" to CultureInfo. Excludes "String" parameter.
        /// 
        /// For example "Culture:en:Key:x:Value:z" is parsed into LineCulture("en").LineKeyNonCanonical("Key", "x")
        /// </summary>
        public static LineFormat Key => key;

        /// <summary>
        /// Format that prints and parses strings as lines. Parameters and values are resolved to default instance types. For example "Culture" to CultureInfo, and "String" to <see cref="IString"/>.
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
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacters">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharacters">Unescape tnab0f</param>
        /// <param name="lineAppender">line appender that can append <see cref="ILineParameter"/> and <see cref="ILineString"/></param>
        /// <param name="qualifier">(optional) qualifier</param>
        public LineFormat(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacters, bool unescapeControlCharacters, ILineFactory lineAppender, ILineQualifier qualifier) : base(lineAppender, qualifier)
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
        }

        /// <summary>
        /// Append a sequence of key,value pairs to a string.
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
        /// <param name="parameters"></param>
        /// <param name="sb"></param>
        /// <returns></returns>
        public override void Print(StructList12<ILineParameter> parameters, StringBuilder sb)
        {
            for (int i = parameters.Count - 1; i >= 0; i--)
            {
                if (i < parameters.Count - 1) sb.Append(':');
                ILineParameter parameter = parameters[i];
                sb.Append(EscapeLiteral(parameter.ParameterName));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.ParameterValue));
            }
        }

        /// <summary>
        /// Parse <paramref name="str"/> into parameters.
        /// </summary>
        /// <param name="str">(optional) string to parse</param>
        /// <param name="parameters"></param>
        public override void Parse(string str, ref StructList12<KeyValuePair<string, string>> parameters)
        {
            if (str == null) return;
            MatchCollection matches = ParsePattern.Matches(str);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new LineException(null, str);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new LineException(null, str);
                string parameterName = UnescapeLiteral(k_key.Value);
                string parameterValue = UnescapeLiteral(k_value.Value);
                parameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
            }
        }

        /// <summary>
        /// Try parse <paramref name="str"/> into parameters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override bool TryParse(string str, ref StructList12<KeyValuePair<string, string>> parameters)
        {
            if (str == null) return false;
            MatchCollection matches = ParsePattern.Matches(str);
            foreach (Match m in matches)
            {
                if (!m.Success) return false;
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) return false;
                string parameterName = UnescapeLiteral(k_key.Value);
                string parameterValue = UnescapeLiteral(k_value.Value);
                parameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
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
