// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Simple immutable collection of plural rules.
    /// 
    /// Implements queryable, but does not cache indices.
    /// </summary>
    public class PluralRules : IPluralRules, IPluralRulesEnumerable, IPluralRulesQueryable
    {
        /// <summary>
        /// Array of rules
        /// </summary>
        public readonly IPluralRule[] Rules;

        /// <summary>
        /// Create rules
        /// </summary>
        /// <param name="rules"></param>
        public PluralRules(params IPluralRule[] rules)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <summary>
        /// Create rules.
        /// </summary>
        /// <param name="rules"></param>
        public PluralRules(IEnumerable<IPluralRule> rules)
        {
            Rules = (rules ?? throw new ArgumentNullException(nameof(rules))).ToArray();
        }

        /// <summary>
        /// Enumerate rules
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IPluralRule> GetEnumerator()
            => ((IEnumerable<IPluralRule>)Rules).GetEnumerator();

        /// <summary>
        /// Filter rules by <paramref name="filterCriteria"/>.
        /// </summary>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        public IPluralRulesEnumerable Query(PluralRuleInfo filterCriteria)
            => new PluralRules(Rules.Where(r => filterCriteria.FilterMatch(r.Info)));

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Rules).GetEnumerator();
    }

    /// <summary>
    /// Mutable <see cref="IPluralRule"/> collection, that caches query results, and flushes caches if content is modified.
    /// </summary>
    public class PluralRulesCollection : IPluralRules
    {

    }

    /// <summary>
    /// Indexed rules and evaluator for a specific (RuleSet,Culture,Category). 
    /// 
    /// This class evaluates an array of <see cref="IPluralRuleEvaluatable"/> as a whole, and returns
    /// all the cases - optional and required - that match the requested <see cref="IPluralNumber" />.
    /// </summary>
    public class PluralRulesEvaluatable : PluralRules, IPluralRulesEvaluatable
    {        /// <summary>
             /// List of evaluatable cases in order of: 1. optional, 2. required.
             /// </summary>
        public readonly IPluralRuleEvaluatable[] EvaluatableCases;

        /// <summary>
        /// Number of cases that are optional.
        /// </summary>
        public readonly int OptionalCaseCount;

        /// <summary>
        /// Number of permutations of optional cases: 2 ^ OptionalCaseCount - 1
        /// </summary>
        public readonly int OptionalCasePerumutationCount;

        /// <summary>
        /// List of cases organized so that each required case forms a <see cref="Line"/>.
        /// 
        /// And each line has a preconfigured result array for each permutation of optional cases.
        /// </summary>
        Line[] lines;

        /// <summary>
        /// Reorders so that optional cases are first then non-optional.
        /// Also filters out non-<see cref="IPluralRulesEvaluatable"/> rules.
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If one of the cases doesn't implement <see cref="IPluralRuleEvaluatable"></see></exception>
        static IEnumerable<IPluralRule> ReorderAndFilter(IEnumerable<IPluralRule> rules)
        {
            // Add optional cases
            foreach (IPluralRule rule in rules)
            {
                if (rule.Info.Optional == 1 && rule is IPluralRulesEvaluatable)
                    yield return rule;
            }

            // Add required cases
            foreach (IPluralRule rule in rules)
            {
                if (rule.Info.Optional == 0 && rule is IPluralRulesEvaluatable) yield return rule;
            }
        }

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="evaluatableRule">cases that implement <see cref="IPluralRuleEvaluatable"></see></param>
        public PluralRulesEvaluatable(params IPluralRule[] evaluatableRule) : this((IEnumerable<IPluralRule>)evaluatableRule) { }

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="evaluatableCases">cases that implement <see cref="IPluralRuleEvaluatable"></see></param>
        public PluralRulesEvaluatable(IEnumerable<IPluralRule> evaluatableCases) : base(ReorderAndFilter(evaluatableCases))
        {
            StructList12<IPluralRuleEvaluatable> evaluatables = new StructList12<IPluralRuleEvaluatable>();
            int firstNonOptionalCase = -1;
            for (int i = 0; i < Rules.Length; i++)
            {
                IPluralRule rule = Rules[i];
                if (rule is IPluralRuleEvaluatable ce) evaluatables.Add(ce);
                bool isOptional = rule.Info.Optional == 1;
                if (!isOptional && firstNonOptionalCase < 0) firstNonOptionalCase = i;
            }
            this.EvaluatableCases = evaluatables.ToArray();
            this.OptionalCaseCount = firstNonOptionalCase;
            if (OptionalCaseCount > 10) throw new ArgumentException($"Maximum number of optional cases is 10, got {OptionalCaseCount}");
            OptionalCasePerumutationCount = (1 << OptionalCaseCount) - 1;

            // Add non-optional
            StructList12<IPluralRule> list = new StructList12<IPluralRule>();
            StructList12<Line> lines = new StructList12<Line>();
            for (int l = firstNonOptionalCase; l < EvaluatableCases.Length; l++)
            {
                IPluralRule c = Rules[l];
                IPluralRuleEvaluatable ce = EvaluatableCases[l];
                Line line = new Line { Evaluatable = ce };
                line.OptionalRulePermutations = (IPluralRule[][])Array.CreateInstance(typeof(IPluralRule[]), OptionalCasePerumutationCount);
                for (int i = 0; i < OptionalCasePerumutationCount; i++)
                {
                    list.Clear();
                    for (int j = 0; j < OptionalCaseCount; j++)
                        if ((i & (1 << j)) != 0) list.Add(list[j]);
                    list.Add(c);
                    line.OptionalRulePermutations[i] = list.ToArray();
                }
                lines.Add(line);
            }
            this.lines = lines.ToArray();
        }

        /// <summary>
        /// Evaluate cases
        /// </summary>
        /// <param name="subset">filter</param>
        /// <param name="number"></param>
        /// <returns>matching cases. First ones are optional, last one is non-optional. Or null if none matched.</returns>
        public IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number)
        {
            // Evaluate each optional cases
            int optionalCaseBits = 0;
            for (int i = 0; i < OptionalCaseCount; i++)
                if (EvaluatableCases[i].Evaluate(number)) optionalCaseBits |= 1 << i;

            // Evaluate required cases
            for (int i = 0; i < lines.Length; i++)
                // Evaluate required case
                if (lines[i].Evaluatable.Evaluate(number))
                    // Return precalculated array
                    return lines[i].OptionalRulePermutations[optionalCaseBits];

            // None matched
            return null;
        }

        /// <summary>
        /// List of cases organized so that each non-optional case forms a <see cref="Line"/>.
        /// 
        /// And each line has a preconfigured result array for each permutation of optional cases.
        /// </summary>
        class Line
        {
            /// <summary>
            /// Evaluatable, non-optional, rule.
            /// </summary>
            public IPluralRuleEvaluatable Evaluatable;

            /// <summary>
            /// List of case-result arrays for the result of <see cref="IPluralRulesEvaluatable.Evaluate(PluralRuleInfo, IPluralNumber)"/>.
            /// One result array for every permutation of optional cases.
            /// 
            /// The last element of the array is the required case.
            /// </summary>
            public IPluralRule[][] OptionalRulePermutations;
        }
    }

}
