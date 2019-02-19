// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Context free format of asset key
    /// </summary>
    public class AssetKeyStringSerializer
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        static AssetKeyStringSerializer generic = new AssetKeyStringSerializer("\\\n\t\r\0\a\b\f:");
        static AssetKeyStringSerializer json = new AssetKeyStringSerializer("\\\n\t\r\0\a\b\f:\"");
        static AssetKeyStringSerializer ini = new AssetKeyStringSerializer("\\\n\t\r\0\a\b\f:=[]");
        static AssetKeyStringSerializer xml = new AssetKeyStringSerializer("\\\n\t\r\0\a\b\f:\"'&<>");

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public static AssetKeyStringSerializer Generic => generic;

        /// <summary>
        /// String serializer for json literals
        /// </summary>
        public static AssetKeyStringSerializer Json => json;

        /// <summary>
        /// String serializer for ini literals, where it's possible to use any char
        /// </summary>
        public static AssetKeyStringSerializer Ini => ini;

        /// <summary>
        /// String serializer for xml literals, where ambersands aren't needed 
        /// </summary>
        public static AssetKeyStringSerializer Xml => xml;

        Regex ParsePattern =
            new Regex(@"(?<key>([^:\\]|\\.)*)\:(?<value>([^:\\]|\\.)*)(\:|$)", opts);

        Regex LiteralEscape;
        Regex LiteralUnescape = new Regex(@"\\.", opts);
        MatchEvaluator escapeChar, unescapeChar;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        public AssetKeyStringSerializer(string escapeCharacters)
        {
            LiteralEscape = new Regex("["+Regex.Escape(escapeCharacters)+"]", opts);
            escapeChar = EscapeChar;
            unescapeChar = UnescapeChar;
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
        public string PrintString(IEnumerable<KeyValuePair<string, string>> keyParameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var parameter in keyParameters)
            {
                if (sb.Length > 0) sb.Append(':');
                sb.Append(EscapeLiteral(parameter.Key));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.Value));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parse string into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public KeyValuePair<string, string>[] ParseString(string keyString)
        {
            MatchCollection matches = ParsePattern.Matches(keyString);
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[matches.Count];
            int ix = 0;
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result[ix++] = new KeyValuePair<string, string>(key, value);
            }
            return result;
        }


        /// <summary>
        /// Parse string into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="result">output of result</param>
        /// <returns>true if successful</returns>
        public bool TryParseString(string keyString, out KeyValuePair<string, string>[] result)
        {
            MatchCollection matches = ParsePattern.Matches(keyString);
            result = new KeyValuePair<string, string>[matches.Count];
            int ix = 0;
            foreach (Match m in matches)
            {
                if (!m.Success) { result = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { result = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result[ix++] = new KeyValuePair<string, string>(key, value);
            }
            return true;
        }


        /// <summary>
        /// Escape literal and output it to string builder.
        /// </summary>
        public String EscapeLiteral(String input) => LiteralEscape.Replace(input, escapeChar);
        public String UnescapeLiteral(String input) => LiteralUnescape.Replace(input, unescapeChar);

        static String EscapeChar(Match m) => @"\" + m.Value;
        static String UnescapeChar(Match m)
        {
            string capture = m.Value;
            char _ch = capture[1];
            switch (_ch)
            {
                case '0': return "\0";
                case 'a': return "\a";
                case 'b': return "\\";
                case 't': return "\t";
                case 'f': return "\f";
                case 'n': return "\n";
                case 'r': return "\r";
                case 'u':
                    char ch = (char)Hex.ToUInt(capture, 2);
                    return new string(ch, 1);
                case 'x':
                    char c = (char)Hex.ToUInt(capture, 2);
                    return new string(c, 1);
                default:
                    return new string(_ch, 1);
            }
        }

    }

}
