// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Abstract plural rules
    /// </summary>
    public class PluralRules : IPluralRules
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class PluralRulesEvaluatable : IPluralRulesEvaluatable
    {
        /// <summary>
        /// List of evaluatable cases in order of: 1. optional, 2. optional.
        /// </summary>
        public readonly IPluralRuleEvaluatable[] EvaluatableCases;

        /// <summary>
        /// Create evaluatable rules from a list of cases.
        /// 
        /// Last case can be non-evaluatable (e.g. "other"). 
        /// It will be used as fallback result, if no evaluatable cases match.
        /// </summary>
        /// <param name="evaluatableCases"></param>
        public PluralRulesEvaluatable(IPluralCase[] evaluatableCases)
        {
            StructList12<IPluralRuleEvaluatable> list = new StructList12<IPluralRuleEvaluatable>();
            // Add optional
            foreach(var c in evaluatableCases)
            {
                if (c.Optional && c is IPluralRuleEvaluatable ce) list.Add(ce);
            }
            // Add non-optional
            foreach (var c in evaluatableCases)
            {
                if (!c.Optional && c is IPluralRuleEvaluatable ce) list.Add(ce);
            }
            this.EvaluatableCases = list.ToArray();
        }

        /// <summary>
        /// Evaluate cases
        /// </summary>
        /// <param name="number"></param>
        /// <returns>matching cases. First ones are optional, last one is non-optional.</returns>
        public IPluralCase[] Evaluate(IPluralNumber number)
        {
            for (int i=0; i<EvaluatableCases.Length; i++)
            {

            }
            return null;
        }
    }

    /// <summary>
    /// Category with cases.
    /// </summary>
    public class PluralCategory : Dictionary<string, IPluralCase>, IPluralCategory, IPluralRulesCaseMap, IPluralRulesEvaluatable
    {
        /// <summary>
        /// (Optional) Category name, e.g. "plural" "ordinal", "optional"
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Possible cases.
        /// 
        /// Optional cases are first. One non-optional is last, usually "other".
        /// </summary>
        public IPluralCase[] Cases { get; internal set; }

        /// <summary>
        /// All evaluatable cases.
        /// </summary>
        IPluralRuleEvaluatable[] CasesEvaluatable;

        /// <summary>
        /// Create category with cases.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rules"></param>
        public PluralCategory(string name, IPluralCase[] rules) : base()
        {
            Name = name;
            Cases = rules ?? throw new ArgumentNullException(nameof(rules));

            foreach (var @case in rules)
            {
                this[@case.Name] = @case;
            }

            StructList8<IPluralRule> _cases = new StructList8<IPluralRule>();
            StructList8<IPluralRuleEvaluatable> _eval_cases = new StructList8<IPluralRuleEvaluatable>();

            // Add optional cases
            foreach(IPluralCase c in rules)
            {
                //if (c is IPluralCase)
            }
        }

        /// <summary>
        /// Evaluate number to case.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public IPluralCase[] Evaluate(IPluralNumber number)
        {
            throw new NotImplementedException();
        }
    }


}
