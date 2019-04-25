// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// A stack of rules. 
    /// </summary>
    public struct PluralRulesScope : IPluralRules, IPluralRulesEvaluatable
    {
        StructList4<IPluralRules> stack;

        /// <summary>
        /// Push new <paramref name="rules"/> to the scope.
        /// The last added have higher priority.
        /// </summary>
        /// <param name="rules"></param>
        public void Push(IPluralRules rules)
            => stack.Add(rules);

        /// <summary>
        /// Remove last element
        /// </summary>
        public void Pop()
            => stack.Dequeue();

        /// <summary>
        /// Remove all rules.
        /// </summary>
        public void Clear()
            => stack.Clear();

        /// <summary>
        /// Evaluate <paramref name="number"/> against all rules in the scope stating from the last added.
        /// </summary>
        /// <param name="subset">RuleSet, Culture and Category must have non-null value. "" is valid. Case must be null and optional must be -1.</param>
        /// <param name="number">(optional) numeric and text representation of numberic value</param>
        /// <returns>Matching cases, first ones are optional, the last one is always mandatory (and only mandatory). Or null if evaluate failed.</returns>
        public IPluralRule[] Evaluate(PluralRuleInfo subset, IPluralNumber number)
        {
            for (int i=stack.Count-1; i>=0; i--)
            {
                if (stack[i] is IPluralRulesEvaluatable eval)
                {
                    IPluralRule[] result = eval.Evaluate(subset, number);
                    if (result != null) return result;
                }
            }
            // Could not evaluate.
            return null;
        }

    }
}
