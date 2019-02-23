// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Context free format of asset key
    /// </summary>
    public partial class Key
    {
        public class NamePolicy : IAssetKeyNameProvider
        {
            static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
            static NamePolicy instance = new NamePolicy("\\\n\t\r\0\a\b\f:");
            static NamePolicy json = new NamePolicy("\\\n\t\r\0\a\b\f:\"");
            static NamePolicy ini = new NamePolicy("\\\n\t\r\0\a\b\f:=[]");
            static NamePolicy xml = new NamePolicy("\\\n\t\r\0\a\b\f:\"'&<>");

            /// <summary>
            /// Generic string serializer where colons can be used in the key and value literals.
            /// </summary>
            public static NamePolicy Instance => instance;

            /// <summary>
            /// String serializer for json literals
            /// </summary>
            public static NamePolicy Json => json;

            /// <summary>
            /// String serializer for ini literals, where it's possible to use any char
            /// </summary>
            public static NamePolicy Ini => ini;

            /// <summary>
            /// String serializer for xml literals, where ambersands aren't needed 
            /// </summary>
            public static NamePolicy Xml => xml;

            Regex ParsePattern =
                new Regex(@"(?<key>([^:\\]|\\.)*)\:(?<value>([^:\\]|\\.)*)(\:|$)", opts);

            Regex LiteralEscape;
            Regex LiteralUnescape = new Regex(@"\\.", opts);
            MatchEvaluator escapeChar, unescapeChar;

            /// <summary>
            /// Create new string serializer
            /// </summary>
            /// <param name="escapeCharacters">list of characters that are to be escaped</param>
            public NamePolicy(string escapeCharacters)
            {
                LiteralEscape = new Regex("[" + Regex.Escape(escapeCharacters) + "]", opts);
                escapeChar = EscapeChar;
                unescapeChar = UnescapeChar;
            }

            /// <summary>
            /// Build path string from key.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="parametrizer">(optional) how to extract parameters from key. If not set uses the default implementation <see cref="AssetKeyParametrizer"/></param>
            /// <returns>full name string</returns>
            string IAssetKeyNameProvider.BuildName(object key, IAssetKeyParametrizer parametrizer)
                => PrintAssetKey(key, parametrizer);

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
            /// <param name="parametrizer"></param>
            /// <returns></returns>
            public string PrintAssetKey(object key, IAssetKeyParametrizer parametrizer = default)
            {
                StringBuilder sb = new StringBuilder();
                if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
                foreach (var parameter in parametrizer.GetAllParameters(key))
                {
                    if (parameter.Key == "root") continue;
                    if (sb.Length > 0) sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Key));
                    sb.Append(':');
                    sb.Append(EscapeLiteral(parameter.Value));
                }
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
            /// <param name="keyParameters"></param>
            /// <returns></returns>
            public string PrintKey(IEnumerable<KeyValuePair<string, string>> keyParameters)
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
            /// <param name="parametrizer"></param>
            /// <returns>key</returns>
            /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
            public IAssetKey ParseAssetKey(string keyString, IAssetKey rootKey, IAssetKeyParametrizer parametrizer = default)
            {
                object result = rootKey;
                if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
                MatchCollection matches = ParsePattern.Matches(keyString);
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new FormatException(keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                    string key = UnescapeLiteral(k_key.Value);
                    string value = UnescapeLiteral(k_value.Value);
                    if (key == "root") continue;
                    result = parametrizer.CreatePart(result, key, value);
                }
                return (IAssetKey)result;
            }

            /// <summary>
            /// Try parse string into IAssetKey.
            /// </summary>
            /// <param name="keyString"></param>
            /// <param name="rootKey">root key to span values from</param>
            /// <returns>key or null</returns>
            public IAssetKey TryParseAssetKey(string keyString, IAssetKey rootKey, IAssetKeyParametrizer parametrizer = default)
            {
                object result = rootKey;
                if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
                MatchCollection matches = ParsePattern.Matches(keyString);
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new FormatException(keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                    string key = UnescapeLiteral(k_key.Value);
                    string value = UnescapeLiteral(k_value.Value);
                    if (key == "root") continue;
                    result = parametrizer.TryCreatePart(result, key, value);
                    if (result == null) return null;
                }
                return result as IAssetKey;
            }

            /// <summary>
            /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
            /// </summary>
            /// <param name="keyString"></param>
            /// <returns>key or null if string was empty</returns>
            /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
            public Key ParseKey(string keyString)
            {
                MatchCollection matches = ParsePattern.Matches(keyString);
                Key result = null;
                foreach (Match m in matches)
                {
                    if (!m.Success) throw new FormatException(keyString);
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                    string name = UnescapeLiteral(k_key.Value);
                    string value = UnescapeLiteral(k_value.Value);
                    result = Key.Parametrizer.Default.CreatePart(result, name, value) as Key;
                }
                return result;
            }

            /// <summary>
            /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
            /// </summary>
            /// <param name="keyString"></param>
            /// <param name="result">output of result, or null if string was empty</param>
            /// <returns>true if successful</returns>
            public bool TryParseKey(string keyString, out Key result)
            {
                MatchCollection matches = ParsePattern.Matches(keyString);
                result = null;
                foreach (Match m in matches)
                {
                    if (!m.Success) { result = null; return false; }
                    Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                    if (!k_key.Success || !k_value.Success) { result = null; return false; }
                    string name = UnescapeLiteral(k_key.Value);
                    string value = UnescapeLiteral(k_value.Value);
                    result = Key.Parametrizer.Default.CreatePart(result, name, value) as Key;
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

}
