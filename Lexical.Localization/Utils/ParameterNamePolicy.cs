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
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    public static class ParameterNamePolicyExtensions
    {
        /// <summary>
        /// Parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns>result key, or null if it contained no parameters.</returns>
        /// <exception cref="System.FormatException">The parameter is not of the correct format.</exception>
        public static IAssetKey Parse(this ParameterNamePolicy policy, string keyString)
        {
            IAssetKey result = null;
            MatchCollection matches = ParameterNamePolicy.ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) throw new FormatException(keyString);
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) throw new FormatException(keyString);
                string key = policy.UnescapeLiteral(k_key.Value);
                string value = policy.UnescapeLiteral(k_value.Value);
                if (key == "Root") continue;
                result = result == null ? Key.Create(key, value) : result.AppendParameter(key, value);
            }
            return result;
        }

        /// <summary>
        /// Try parse string into IAssetKey.
        /// </summary>
        /// <param name="keyString"></param>
        /// <param name="resultKey">result key, or null if it contained no parameters.</param>
        /// <returns>true if parse was successful</returns>
        public static bool TryParse(this ParameterNamePolicy policy, string keyString, out IAssetKey resultKey)
        {
            IAssetKey result = null;
            MatchCollection matches = ParameterNamePolicy.ParsePattern.Matches(keyString);
            foreach (Match m in matches)
            {
                if (!m.Success) { resultKey = null; return false; }
                Group k_key = m.Groups["key"], k_value = m.Groups["value"];
                if (!k_key.Success || !k_value.Success) { resultKey = null; return false; }
                string key = policy.UnescapeLiteral(k_key.Value);
                string value = policy.UnescapeLiteral(k_value.Value);
                if (key == "Root") continue;
                result = result == null ? Key.Create(key, value) : result.AppendParameter(key, value);
            }
            resultKey = result;
            return true;
        }

    }

}
