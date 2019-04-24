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
    /// Ruleset
    /// </summary>
    public class PluralRuleSet : Dictionary<String, IPluralRules>, IPluralRuleSetMap
    {
        /// <summary>
        /// (optional) Name of the rule set, e.g. "CLDRv35"
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// All rules. 
        /// </summary>
        public IPluralRules[] Rules { get; internal set; }

        /// <summary>
        /// Create ruleset.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rulesList"></param>
        public PluralRuleSet(string name, params IPluralRules[] rulesList) : this(name, (IEnumerable<IPluralRules>)rulesList) { }

        /// <summary>
        /// Create rule set.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rulesList"></param>
        public PluralRuleSet(string name, IEnumerable<IPluralRules> rulesList)
        {
            Name = name;
            this.Rules = rulesList.ToArray();
            foreach (var rules in rulesList)
                if (rules is IPluralCategory category && category.Name != null)
                    this[category.Name] = category;
        }
    }
}
