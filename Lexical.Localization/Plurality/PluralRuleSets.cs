// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// List of rule-sets.
    /// </summary>
    public class PluralRuleSets : Dictionary<String, IPluralRuleSet>, IPluralRuleSets, IPluralRuleSetsMap
    {
        /// <summary>
        /// (optional) Name of the rule set, e.g. "CLDRv35"
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// All rulesets. 
        /// </summary>
        public IPluralRuleSet[] RuleSets { get; internal set; }

        /// <summary>
        /// Create ruleset.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rulesetsList"></param>
        public PluralRuleSets(string name, params IPluralRuleSet[] rulesetsList) : this(name, (IEnumerable<IPluralRuleSet>)rulesetsList) { }

        /// <summary>
        /// Create rule set.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rulesetsList"></param>
        public PluralRuleSets(string name, IEnumerable<IPluralRuleSet> rulesetsList)
        {
            Name = name;
            this.RuleSets = rulesetsList.ToArray();
            foreach (var ruleset in rulesetsList)
                this[ruleset.Name] = ruleset;
        }
    }
}
