// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Set of rules. 
    /// 
    /// See subinterfaces:
    /// <list type="bullet">
    /// <item><see cref="IPluralRuleSetsMap"/>Map of rule-sets</item>
    /// <item><see cref="IPluralRuleSetInfo"/>Info about rule-set</item>
    /// </list>
    /// </summary>
    public interface IPluralRuleSets
    {
    }

    /// <summary>
    /// Map that contains plurality rules.
    /// </summary>
    public interface IPluralRuleSetsMap : IPluralRuleSets, IReadOnlyDictionary<String, IPluralRuleSet>
    {
    }

    /// <summary>
    /// Key where <see cref="IPluralRuleSets"/> can be assigned.
    /// </summary>
    public interface IPluralRuleSetsAssignableKey : ILocalizationKey
    {
        /// <summary>
        /// Assign rule-sets.
        /// </summary>
        /// <param name="ruleSets"></param>
        /// <returns></returns>
        IPluralRuleSetsAssignedKey PluralityRuleSets(IPluralRuleSets ruleSets);
    }

    /// <summary>
    /// A key where <see cref="IPluralRuleSets"/> is assigned.
    /// </summary>
    public interface IPluralRuleSetsAssignedKey : ILocalizationKey
    {
        /// <summary>
        /// Get (possibly) assigned rule-sets.
        /// </summary>
        IPluralRuleSets PluralityRuleSets { get; }
    }

    /// <summary>
    /// Plurality extension methods.
    /// </summary>
    public static partial class PluralityExtensions
    {

    }

}
