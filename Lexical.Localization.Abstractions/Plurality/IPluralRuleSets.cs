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
    /// Plural rule sets expression that contains rules for multiple cultures.
    /// 
    /// e.g. "$CLDRv35 #en #fi #sw §one i = 1 and v = 0 @integer 1 §other @integer 0, 2~16, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …".
    /// </summary>
    public interface IPluralRuleSetsExpression : IExpression, IPluralRuleSets
    {
        /// <summary>
        /// Rules list
        /// </summary>
        IPluralRuleSetExpression[] RuleSets { get; }
    }


    /// <summary>
    /// Plurality extension methods.
    /// </summary>
    public static partial class PluralityExtensions
    {

    }

}
