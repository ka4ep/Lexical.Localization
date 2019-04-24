// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
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
    /// </list>
    /// </summary>
    public interface IPluralRuleSet
    {
        /// <summary>
        /// (optional) Name of the rule set, e.g. "CLDRv35"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// All rules. 
        /// </summary>
        IPluralRules[] Rules { get; }
    }

    /// <summary>
    /// Map that contains plurality rules.
    /// </summary>
    public interface IPluralRuleSetMap : IPluralRuleSet, IReadOnlyDictionary<String, IPluralRules>
    {
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
    /// Plural rule set expression that contains rules for multiple cultures.
    /// 
    /// e.g. "$CLDRv35 #ast ca de en et fi fy gl ia io it ji nl pt_PT sc scn sv sw ur yi §one i = 1 and v = 0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
    /// </summary>
    public interface IPluralRuleSetExpression : IExpression
    {
        /// <summary>
        /// Rules list
        /// </summary>
        IPluralRulesExpression[] RulesList { get; }
    }

    /// <summary>
    /// Plurality extension methods.
    /// </summary>
    public static partial class PluralityExtensions
    {

    }

}
