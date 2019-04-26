// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// See subinterfaces:
    /// <list type="bullet">
    /// <item><see cref="IPluralRulesEnumerable"/>Lists rules</item>
    /// <item><see cref="IPluralRulesEvaluatable"/>Evaluates number</item>
    /// <item><see cref="IPluralRulesQueryable"/>Queryable rules</item>
    /// </list>
    /// 
    /// For examples, there can be rules for
    /// 
    /// category=cardinal
    ///      case="":         "{cardinal:0} car(s)"
    ///      case="one":      "a car"                       when {0}=1
    ///      case="other":    "{0} cars"                    when {0}=5
    ///      
    /// category=ordinal
    ///      case="":         "Ta {ordinal:0}:a svängen till höger"
    ///      case="one":      "Ta {0}:a svängen till höger" when {0}=1
    ///      case="other":    "Ta {1}:e svängen till höger" when {0}=3
    /// 
    /// category=optional
    ///      case="":         "{optional:0} cars"
    ///      case="null":     "No cars"                      when {0}=null
    /// 
    /// </summary>
    public interface IPluralRules
    {
    }

    /// <summary>
    /// Collection of plural rules.
    /// </summary>
    public interface IPluralRulesEnumerable : IPluralRules, IEnumerable<IPluralRule>
    {
    }

    /// <summary>
    /// Queryable rules.
    /// </summary>
    public interface IPluralRulesQueryable : IPluralRules
    {
        /// <summary>
        /// Query a collection of rules.
        /// </summary>
        /// <param name="filterCriteria">filter criteria</param>
        /// <returns>enumerable of rules, or null if could not run query with the <paramref name="filterCriteria"/></returns>
        IPluralRulesEnumerable Query(PluralRuleInfo filterCriteria);
    }

    /// <summary>
    /// Evaluates a <see cref="IPluralNumber"/> to the collection of rules.
    /// </summary>
    public interface IPluralRulesEvaluatable : IPluralRules
    {
        /// <summary>
        /// Evaluates number against subset of rules.
        /// 
        /// First results are optional, last one is mandatory.
        /// </summary>
        /// <param name="subset">RuleSet, Culture and Category must have non-null value. "" is valid. Case must be null and optional must be -1.</param>
        /// <param name="number">(optional) numeric and text representation of numberic value</param>
        /// <returns>Matching cases, first ones are optional, the last one is always mandatory (and only mandatory). Or null if evaluate failed.</returns>
        IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number);
    }

    /// <summary>
    /// Plurality extension methods.
    /// </summary>
    public static partial class PluralityExtensions
    {
        static string[] empty = new string[0];

        /// <summary>
        /// Get all rulesets in a collection of rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static IEnumerable<string> RuleSets(this IPluralRules rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (rules is IEnumerable<IPluralRule> enumr) return enumr.Select(r => r.Info.RuleSet).Where(rs => rs != null).Distinct();
            return empty;
        }

        /// <summary>
        /// Get all categories in a collection of rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static IEnumerable<string> Categories(this IPluralRules rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (rules is IEnumerable<IPluralRule> enumr) return enumr.Select(r => r.Info.Category).Where(c => c != null).Distinct();
            return empty;
        }

        /// <summary>
        /// Get all cultures in a collection of rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static IEnumerable<string> Cultures(this IPluralRules rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (rules is IEnumerable<IPluralRule> enumr) return enumr.Select(r => r.Info.Culture).Where(c => c != null).Distinct();
            return empty;
        }

        /// <summary>
        /// Get all cases in a collection of rules
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static IEnumerable<string> Case(this IPluralRules rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (rules is IEnumerable<IPluralRule> enumr) return enumr.Select(r => r.Info.Case).Where(c => c != null).Distinct();
            return empty;
        }

    }
}
