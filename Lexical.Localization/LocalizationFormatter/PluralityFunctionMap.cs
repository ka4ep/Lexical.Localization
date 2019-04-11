// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           6.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Map that contains plurality rules.
    /// 
    /// The key is ISO 639-1 (two character) or ISO 639-2 (three character) language code.
    /// </summary>
    public class PluralityRuleMap : Dictionary<CultureInfo, IPluralityRules>, IPluralityRuleMap, ICloneable, IFormatProvider
    {
        /// <summary>
        /// </summary>
        public PluralityRuleMap() : base()
        {
        }

        /// <summary>
        /// Create new clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            PluralityRuleMap result = new PluralityRuleMap();
            foreach (var line in this)
                result[line.Key] = line.Value;
            return result;
        }

        /// <summary>
        /// Get provider for <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
            => formatType == typeof(IPluralityRuleMap) ? this : null;
    }
}
