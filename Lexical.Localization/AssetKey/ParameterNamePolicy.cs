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
    public class ParameterNamePolicy : IAssetKeyNameProvider
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        static ParameterNamePolicy instance = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:");
        static ParameterNamePolicy json = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:\"");
        static ParameterNamePolicy ini = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:=[]");
        static ParameterNamePolicy xml = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:\"'&<>");

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public static ParameterNamePolicy Instance => instance;

        /// <summary>
        /// String serializer for json literals
        /// </summary>
        public static ParameterNamePolicy Json => json;

        /// <summary>
        /// String serializer for ini literals, where it's possible to use any char
        /// </summary>
        public static ParameterNamePolicy Ini => ini;

        /// <summary>
        /// String serializer for xml literals, where ambersands aren't needed 
        /// </summary>
        public static ParameterNamePolicy Xml => xml;

        Regex ParsePattern =
            new Regex(@"(?<key>([^:\\]|\\.)*)\:(?<value>([^:\\]|\\.)*)(\:|$)", opts);

        Regex LiteralEscape;
        Regex LiteralUnescape = new Regex(@"\\.", opts);
        MatchEvaluator escapeChar, unescapeChar;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        public ParameterNamePolicy(string escapeCharacters)
        {
            LiteralEscape = new Regex("[" + Regex.Escape(escapeCharacters) + "]", opts);
            escapeChar = EscapeChar;
            unescapeChar = UnescapeChar;
            _parameterVisitor = parameterVisitor;
        }

        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>full name string</returns>
        string IAssetKeyNameProvider.BuildName(IAssetKey key)
            => PrintKey(key as IAssetKey);

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
        public string PrintKey(IAssetKey key)
        {
            StringBuilder sb = new StringBuilder();
            key.VisitFromRoot(_parameterVisitor, ref sb);
            return sb.ToString();
        }
        AssetKeyVisitor<StringBuilder> _parameterVisitor;
        void parameterVisitor(IAssetKey key, ref StringBuilder sb)
        {
            if (key is IAssetKeyParametrized parameter && parameter.ParameterName != "root")
            {
                if (sb.Length > 0) sb.Append(':');
                sb.Append(EscapeLiteral(parameter.ParameterName));
                sb.Append(':');
                sb.Append(EscapeLiteral(parameter.Name));
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
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public IAssetKey ParseKey(string keyString, IAssetKey rootKey)
        {
            IAssetKey result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                if (key == "root") continue;
                result = result.AppendParameter(key, value);
            }
            return result;
        }

        /// <summary>
        /// Try parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="rootKey">root key to span values from</param>
        /// <returns>key or null</returns>
        public IAssetKey TryParseKey(string keyString, IAssetKey rootKey)
        {
            IAssetKey result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                if (key == "root") continue;
                result = result.AppendParameter(key, value);
            }
            return result;
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
