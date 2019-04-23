// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Plurality;
using System;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Abstract plural rule with name.
    /// </summary>
    public class PluralRule : IPluralRule
    {
        /// <summary>
        /// (optional) Name of the rule.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Create abstract rule.
        /// </summary>
        /// <param name="name">(optional)</param>
        public PluralRule(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Plural rule that is evaluatable.
    /// </summary>
    public class PluralRuleEvaluatable : PluralRule, IPluralRuleEvaluatable, IPluralRuleExpression
    {
        /// <summary>
        /// Rule expression that is evaluated.
        /// </summary>
        public IExpression Rule { get; internal set; }

        /// <summary>
        /// Samples.
        /// </summary>
        public ISamplesExpression[] Samples { get; internal set; }

        /// <summary>
        /// No samples
        /// </summary>
        public static ISamplesExpression[] NO_SAMPLES = new ISamplesExpression[0];

        /// <summary>
        /// Wrap <paramref name="rule"/> into evaluatable info class.
        /// </summary>
        /// <param name="name">(optional)</param>
        /// <param name="rule"></param>
        /// <param name="samples">(optional)</param>
        public PluralRuleEvaluatable(string name, IExpression rule, ISamplesExpression[] samples = default) : base(name)
        {
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Samples = samples ?? NO_SAMPLES;
        }

        /// <summary>
        /// Evaluate number against the rule expression.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Evaluate(IPluralNumber number)
        {
            PluralRuleEvaluator evaluator = new PluralRuleEvaluator(number);
            return evaluator.EvaluateBoolean(Rule);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PluralCase : PluralRuleEvaluatable, IPluralCase
    {
        /// <summary>
        /// Is this case optional
        /// </summary>
        public bool Optional { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">(optional) name</param>
        /// <param name="optional">Is case optional for translator to supply</param>
        /// <param name="rule">rule expression</param>
        /// <param name="samples">(optional) samples</param>
        public PluralCase(string name, bool optional, IExpression rule, ISamplesExpression[] samples = default) : base(name, rule, samples)
        {
            Optional = optional;
        }
    }

    /// <summary>
    /// Instance of plural case that carries information as member of plural category.
    /// </summary>
    public class PluralCategoryPart : PluralCase, IPluralCategoryPart
    {
        /// <summary>
        /// The the category this case is part of.
        /// </summary>
        public IPluralCategory Category { get; internal set; }

        /// <summary>
        /// Index in <see cref="IPluralCategory.Cases"/>.
        /// </summary>
        public int CaseIndex { get; internal set; }

        /// <summary>
        /// Create category part.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="caseIndex"></param>
        /// <param name="name">(optional) name</param>
        /// <param name="optional">Is case optional for translator to supply</param>
        /// <param name="rule">rule expression</param>
        /// <param name="samples">(optional) samples</param>
        public PluralCategoryPart(IPluralCategory category, int caseIndex, string name, bool optional, IExpression rule, ISamplesExpression[] samples = default)  : base(name, optional, rule, samples)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            CaseIndex = caseIndex;
        }
    }

}
