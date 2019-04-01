// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Context free format of asset key
    /// </summary>
    public class ParameterNamePolicy : ParameterParser, IAssetKeyNameProvider, IAssetKeyNameParser
    {
        static ParameterNamePolicy instance = new ParameterNamePolicy("\\:", false, "\\:", false);

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public new static ParameterNamePolicy Instance => instance;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacteres">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharactrs">Unescape tnab0f</param>
        public ParameterNamePolicy(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacteres, bool unescapeControlCharactrs) : base(escapeCharacters, escapeControlCharacters, unescapeCharacteres, unescapeControlCharactrs)
        {
        }

        /// <summary>
        /// Parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>result key, or null if it contained no parameters and <paramref name="rootKey"/> was null.</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public override IAssetKey Parse(string keyString, IAssetKey rootKey)
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
                if (key == "Root") continue;
                result = result == null ? Key.Create(key, value) : result.AppendParameter(key, value);
            }
            return result;
        }

        /// <summary>
        /// Try parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="resultKey">(optional) result key, or null if it contained no parameters and <paramref name="rootKey"/> was null.</param>
        /// <param name="rootKey">root key to span values from</param>
        /// <returns>true if parse was successful</returns>
        public override bool TryParse(string keyString, out IAssetKey resultKey, IAssetKey rootKey)
        {
            IAssetKey result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) { resultKey = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { resultKey = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                if (key == "Root") continue;
                result = result == null ? Key.Create(key, value) : result.AppendParameter(key, value);
            }
            resultKey = result;
            return resultKey != null;
        }


        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>full name string</returns>
        public string BuildName(IAssetKey key)
            => PrintKey(key as IAssetKey);

    }
}
