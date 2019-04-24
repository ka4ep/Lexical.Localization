// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Abstract plural rules
    /// </summary>
    public abstract class PluralRules : IPluralRules, IPluralRulesCaseMap
    {
        /// <summary>
        /// List of cases in order of: 1. optional, 2. required.
        /// </summary>
        public readonly IPluralRule[] Rules;

        /// <summary>
        /// Get names of values
        /// </summary>
        public IEnumerable<string> Keys { get; internal set; }

        /// <summary>
        /// Get values.
        /// </summary>
        public IEnumerable<IPluralRule> Values => Rules;

        /// <summary>
        /// Number of cases
        /// </summary>
        public int Count => Rules.Length;

        /// <summary>
        /// Lookup map
        /// </summary>
        Dictionary<string, IPluralRule> lookup;

        /// <summary>
        /// Get case by name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public IPluralRule this[string key] 
            => lookup[key];

        /// <summary>
        /// Create rules from a list of cases.
        /// </summary>
        /// <param name="rules"></param>
        public PluralRules(IPluralRule[] rules) : this((IEnumerable<IPluralRule>)rules)
        {
        }

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="rules"></param>
        public PluralRules(IEnumerable<IPluralRule> rules)
        {
            this.Keys = rules.Select(c => c.Name).ToArray();
            this.Rules = rules.ToArray();
            this.lookup = new Dictionary<string, IPluralRule>(this.Rules.Length);
            foreach (var c in rules) this.lookup[c.Name] = c;
        }

        /// <summary>
        /// Tests whether a key exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
            => lookup.ContainsKey(key);

        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out IPluralRule value)
            => lookup.TryGetValue(key, out value);

        /// <summary>
        /// Enumerate cases
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, IPluralRule>> GetEnumerator()
        {
            foreach (var @case in Values)
                yield return new KeyValuePair<string, IPluralRule>(@case.Name, @case);
        }

        /// <summary>
        /// Enumerate cases
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var @case in Values)
                yield return new KeyValuePair<string, IPluralRule>(@case.Name, @case);
        }
    }

    /// <summary>
    /// Category that composes cases.
    /// </summary>
    public class PluralCategory : PluralRules, IPluralCategory
    {
        /// <summary>
        /// (Optional) Category name, e.g. "plural" "ordinal", "optional"
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Plural cases
        /// </summary>
        public IPluralCase[] Cases { get; internal set; }

        /// <summary>
        /// Create category with cases.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cases"></param>
        public PluralCategory(string name, params IPluralCase[] cases) : this(name, (IEnumerable<IPluralCase>)cases) { }

        /// <summary>
        /// Create category with cases.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cases"></param>
        public PluralCategory(string name, IEnumerable<IPluralCase> cases) : base(cases)
        {
            this.Name = name;
            StructList12<IPluralCase> list = new StructList12<IPluralCase>();
            foreach(var rule in Rules)
            {
                if (rule is IPluralCase c) list.Add(c);
            }
            this.Cases = list.ToArray();
        }
    }

    /// <summary>
    /// This class evaluates an array of <see cref="IPluralRuleEvaluatable"/> as a whole, and returns
    /// all the cases - optional and required - that match the requested <see cref="IPluralNumber" />.
    /// </summary>
    public class PluralRulesEvaluatable : PluralRules, IPluralRulesEvaluatable
    {
        /// <summary>
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
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If one of the cases doesn't implement <see cref="IPluralRuleEvaluatable"></see></exception>
        static IEnumerable<IPluralRule> Reorder(IEnumerable<IPluralRule> rules)
        {
            // Add optional cases
            foreach (IPluralRule rule in rules)
            {
                if (rule is IPluralCase c && c.Optional)
                    yield return rule;
            }

            // Add required cases
            foreach (IPluralRule rule in rules)
            {
                if (rule is IPluralCase c && c.Optional) continue;
                yield return rule;
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
        public PluralRulesEvaluatable(IEnumerable<IPluralRule> evaluatableCases) : base(Reorder(evaluatableCases))
        {
            StructList12<IPluralRuleEvaluatable> evaluatables = new StructList12<IPluralRuleEvaluatable>();
            int firstNonOptionalCase = -1;
            for (int i=0; i<Rules.Length; i++)
            {
                IPluralRule rule = Rules[i];
                if (rule is IPluralRuleEvaluatable ce) evaluatables.Add(ce);
                bool isOptional = rule is IPluralCase c && c.Optional;
                if (!isOptional && firstNonOptionalCase < 0) firstNonOptionalCase = i;
            }
            this.EvaluatableCases = evaluatables.ToArray();
            this.OptionalCaseCount = firstNonOptionalCase;
            if (OptionalCaseCount > 10) throw new ArgumentException($"Maximum number of optional cases is 10, got {OptionalCaseCount}");
            OptionalCasePerumutationCount = (1 << OptionalCaseCount)-1;

            // Add non-optional
            StructList12<IPluralRule> list = new StructList12<IPluralRule>();
            StructList12<Line> lines = new StructList12<Line>();
            for (int l=firstNonOptionalCase; l<EvaluatableCases.Length; l++)
            {
                IPluralRule c = Rules[l];
                IPluralRuleEvaluatable ce = EvaluatableCases[l];
                Line line = new Line { Evaluatable = ce };
                line.OptionalCasePermutations = (IPluralRule[][]) Array.CreateInstance(typeof(IPluralRule[]), OptionalCasePerumutationCount);
                for (int i = 0; i < OptionalCasePerumutationCount; i++)
                {
                    list.Clear();
                    for (int j = 0; j < OptionalCaseCount; j++)
                        if ((i & (1 << j)) != 0) list.Add(list[j]);
                    list.Add(c);
                    line.OptionalCasePermutations[i] = list.ToArray();
                }
                lines.Add(line);
            }
            this.lines = lines.ToArray();
        }

        /// <summary>
        /// Evaluate cases
        /// </summary>
        /// <param name="number"></param>
        /// <returns>matching cases. First ones are optional, last one is non-optional. Or null if none matched.</returns>
        public IPluralRule[] Evaluate(IPluralNumber number)
        {
            // Evaluate each optional cases
            int optionalCaseBits = 0;
            for (int i=0; i<OptionalCaseCount; i++)
                if (EvaluatableCases[i].Evaluate(number)) optionalCaseBits |= 1 << i;

            // Evaluate required cases
            for (int i = 0; i < lines.Length; i++)
                // Evaluate required case
                if (lines[i].Evaluatable.Evaluate(number))
                    // Return precalculated array
                    return lines[i].OptionalCasePermutations[optionalCaseBits];

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
            /// List of case-result arrays for the result of <see cref="IPluralRulesEvaluatable.Evaluate(IPluralNumber)"/>.
            /// One result array for every permutation of optional cases.
            /// 
            /// The last element of the array is the required case.
            /// </summary>
            public IPluralRule[][] OptionalCasePermutations;
        }

        /// <summary>
        /// Category with name and cases
        /// </summary>
        public class Category : PluralRulesEvaluatable, IPluralCategory
        {
            /// <summary>
            /// Category name 
            /// </summary>
            public string Name { get; internal set; }

            /// <summary>
            /// Categories.
            /// </summary>
            public IPluralCase[] Cases { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="evaluatableCases">cases that implement <see cref="IPluralRuleEvaluatable"/></param>
            public Category(string name, params IPluralCase[] evaluatableCases) : base(evaluatableCases)
            {
                Name = name;
                Cases = evaluatableCases;
            }
        }
    }
}
