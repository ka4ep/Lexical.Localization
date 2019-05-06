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
    /// Parameter policy that uses string format of "Key:Value:..."
    /// </summary>
    public class ParameterPolicy : ParameterParser, IParameterPrinter, IParameterParser
    {
        static ParameterPolicy instance = new ParameterPolicy("\\:", false, "\\:", false);

        /// <summary>
        /// Generic string serializer where colons can be used in the key and value literals.
        /// </summary>
        public new static ParameterPolicy Instance => instance;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        /// <param name="escapeControlCharacters">Escape characters 0x00 - 0x1f</param>
        /// <param name="unescapeCharacteres">list of characters that are to be unescaped</param>
        /// <param name="unescapeControlCharactrs">Unescape tnab0f</param>
        public ParameterPolicy(string escapeCharacters, bool escapeControlCharacters, string unescapeCharacteres, bool unescapeControlCharactrs) : base(escapeCharacters, escapeControlCharacters, unescapeCharacteres, unescapeControlCharactrs)
        {
        }

        /// <summary>
        /// Parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="rootKey">(optional) root key to span values from</param>
        /// <returns>result key, or null if it contained no parameters and <paramref name="rootKey"/> was null.</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public override ILine Parse(string keyString, ILine rootKey)
        {
            ILine result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result = result == null ? Key.Create(key, value) : result.Parameter(key, value);
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
        public override bool TryParse(string keyString, out ILine resultKey, ILine rootKey)
        {
            ILine result = rootKey;
            MatchCollection matches = ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) { resultKey = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { resultKey = null; return false; }
                string key = UnescapeLiteral(k_key.Value);
                string value = UnescapeLiteral(k_value.Value);
                result = result == null ? Key.Create(key, value) : result.Parameter(key, value);
            }
            resultKey = result;
            return resultKey != null;
        }


        /// <summary>
        /// Build path string from key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>full name string</returns>
        public string Print(ILine key)
            => PrintKey(key as ILine);

    }
}
