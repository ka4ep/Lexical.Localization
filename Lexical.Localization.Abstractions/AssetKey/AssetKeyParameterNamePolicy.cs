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
    public class AssetKeyParameterNamePolicy : IAssetKeyNameProvider
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        static AssetKeyParameterNamePolicy instance = new AssetKeyParameterNamePolicy("\\\n\t\r\0\a\b\f:");
        static AssetKeyParameterNamePolicy json = new AssetKeyParameterNamePolicy("\\\n\t\r\0\a\b\f:\"");
        static AssetKeyParameterNamePolicy ini = new AssetKeyParameterNamePolicy("\\\n\t\r\0\a\b\f:=[]");
        static AssetKeyParameterNamePolicy xml = new AssetKeyParameterNamePolicy("\\\n\t\r\0\a\b\f:\"'&<>");

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public static AssetKeyParameterNamePolicy Instance => instance;

        /// <summary>
        /// String serializer for json literals
        /// </summary>
        public static AssetKeyParameterNamePolicy Json => json;

        /// <summary>
        /// String serializer for ini literals, where it's possible to use any char
        /// </summary>
        public static AssetKeyParameterNamePolicy Ini => ini;

        /// <summary>
        /// String serializer for xml literals, where ambersands aren't needed 
        /// </summary>
        public static AssetKeyParameterNamePolicy Xml => xml;

        Regex ParsePattern =
            new Regex(@"(?<key>([^:\\]|\\.)*)\:(?<value>([^:\\]|\\.)*)(\:|$)", opts);

        Regex LiteralEscape;
        Regex LiteralUnescape = new Regex(@"\\.", opts);
        MatchEvaluator escapeChar, unescapeChar;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        public AssetKeyParameterNamePolicy(string escapeCharacters)
        {
            LiteralEscape = new Regex("["+Regex.Escape(escapeCharacters)+"]", opts);
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
            => PrintKey(key, parametrizer);

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
        public string PrintKey(object key, IAssetKeyParametrizer parametrizer = default)
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
        public string PrintParameters(IEnumerable<KeyValuePair<string, string>> keyParameters)
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
        /// Parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>key</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public IAssetKey ParseKey(string keyString, IAssetKey rootKey)
        {
            object result = rootKey;
            IAssetKeyParametrizer parametrizer = AssetKeyParametrizer.Singleton;
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
        public IAssetKey TryParseKey(string keyString, IAssetKey rootKey, IAssetKeyParametrizer parametrizer = default)
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
        public Key ParseParameters(string keyString)
        {
            MatchCollection matches = ParsePattern.Matches(keyString);
            Key result = null;
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string ame = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result = new Key(result, ame, value);
            }
            return result;
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="result">output of result, or null if string was empty</param>
        /// <returns>true if successful</returns>
        public bool TryParseParameters(string keyString, out Key result)
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
                result = new Key(result, name, value);
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
