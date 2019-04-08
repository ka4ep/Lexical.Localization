﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           6.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Map that contains plurality rules.
    /// 
    /// The key is ISO 639-1 (two character) or ISO 639-2 (three character) language code.
    /// </summary>
    public class PluralizationRulesSetMap : Dictionary<KeyValuePair<string, string>, IPluralityFunction>, IPluralityFunctionMap, ICloneable
    {
        /// <summary>
        /// </summary>
        public PluralizationRulesSetMap() : base(KeyValuePairEqualityComparer<string, string>.Default)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public IPluralityFunction TryGet(string languageCode, string functionName)
        {
            IPluralityFunction result = null;
            TryGetValue(new KeyValuePair<string, string>(languageCode, functionName), out result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public PluralizationRulesSetMap Add(string languageCode, IPluralityFunction function)
        {
            this[new KeyValuePair<string, string>(languageCode, function.Name)] = function;
            return this;
        }

        /// <summary>
        /// Enumerate content
        /// </summary>
        /// <returns>Tuples of language code and function</returns>
        IEnumerator<(string, IPluralityFunction)> IEnumerable<(string, IPluralityFunction)>.GetEnumerator()
        {
            foreach (var line in this)
                yield return (line.Key.Key, line.Value);
        }

        /// <summary>
        /// Create new clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            PluralizationRulesSetMap result = new PluralizationRulesSetMap();
            foreach (var line in this)
                result.Add(line.Key, line.Value);
            return result;
        }
    }
}
