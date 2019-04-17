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
    /// <item><see cref="IPluralRuleSetMap"/>Map of rule-sets</item>
    /// <item><see cref="IPluralRuleSetInfo"/>Info about rule-set</item>
    /// </list>
    /// </summary>
    public interface IPluralRuleSet
    {
    }

    /// <summary>
    /// Map that contains plurality rules.
    /// </summary>
    public interface IPluralRuleSetMap : IPluralRuleSet, IReadOnlyDictionary<String, IPluralRules>
    {
    }

    /// <summary>
    /// Rule-set info
    /// </summary>
    public interface IPluralRuleSetInfo : IPluralRuleSet
    {
        /// <summary>
        /// Rule set name
        /// </summary>
        String Name { get; }
    }

    /// <summary>
    /// Key where an explicit <see cref="IPluralRuleSet"/> can be assigned.
    /// </summary>
    public interface IPluralRuleSetAssignableKey : ILocalizationKey
    {
        /// <summary>
        /// Assign rule-sets.
        /// </summary>
        /// <param name="ruleSets"></param>
        /// <returns></returns>
        IPluralRuleSetAssignedKey PluralityRuleSet(IPluralRuleSets ruleSets);
    }

    /// <summary>
    /// A key where an explicit <see cref="IPluralRuleSet"/> is assigned.
    /// </summary>
    public interface IPluralRuleSetAssignedKey : ILocalizationKey
    {
        /// <summary>
        /// Get (possibly) assigned rule-sets.
        /// </summary>
        IPluralRuleSet PluralityRuleSet { get; }
    }

    /// <summary>
    /// Key where an name of <see cref="IPluralRuleSet"/> can be assigned. 
    /// 
    /// The <see cref="IPluralRuleSet"/> is resolved from <see cref="IPluralRuleSets"/> by the assigned name.
    /// </summary>
    public interface IPluralRuleSetNameAssignableKey : ILocalizationKey
    {
        /// <summary>
        /// Assign rule-sets.
        /// </summary>
        /// <param name="ruleSets"></param>
        /// <returns></returns>
        IPluralRuleSetNameAssignedKey PluralityRuleSet(String ruleSets);
    }

    /// <summary>
    /// A key where an explicit <see cref="IPluralRuleSet"/> is assigned.
    /// 
    /// The parameter name of the key is "PluralityRuleSet", and example values are "CLDRv35", "CLDR" for the latest version.
    /// </summary>
    public interface IPluralRuleSetNameAssignedKey : ILocalizationKey, IAssetKeyParameterAssigned
    {
        /// <summary>
        /// Get (possibly) assigned rule-sets.
        /// </summary>
        String PluralityRuleSet { get; }
    }

    /// <summary>
    /// Plurality extension methods.
    /// </summary>
    public static partial class PluralityExtensions
    {

    }

}
