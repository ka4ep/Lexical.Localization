// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           6.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Map that contains plurality rules.
    /// 
    /// The key is ISO 639-1 (two character) or ISO 639-2 (three character) language code.
    /// </summary>
    public class PluralizationRulesSetMap : Dictionary<string, IPluralityRuleSet>, IPluralizationRuleSetMap
    {
        /// <summary>
        /// </summary>
        public PluralizationRulesSetMap() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public PluralizationRulesSetMap(IDictionary<string, IPluralityRuleSet> dictionary) : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public PluralizationRulesSetMap(IEqualityComparer<string> comparer) : base() { }
    }

    /// <summary>
    /// Extensions for <see cref="IPluralizationRuleSetMap"/> classes.
    /// </summary>
    public static class PluralityMapExtensions_
    {
        /// <summary>
        /// Add <paramref name="value"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDictionary<string, IPluralityRuleSet> Add(this IDictionary<string, IPluralityRuleSet> map, string code, IPluralityRuleSet value)
        {
            map[code] = value;
            return map;
        }

        /// <summary>
        /// Add <paramref name="values"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IDictionary<string, IPluralityRuleSet> AddRange(this IDictionary<string, IPluralityRuleSet> map, IEnumerable<KeyValuePair<string, IPluralityRuleSet>> values)
        {
            foreach (var line in values)
                map[line.Key] = line.Value;
            return map;
        }

        /// <summary>
        /// Create clone 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static PluralizationRulesSetMap Clone(this IReadOnlyDictionary<string, IPluralityRuleSet> map)
            => new PluralizationRulesSetMap().AddRange(map) as PluralizationRulesSetMap;
    }
}
