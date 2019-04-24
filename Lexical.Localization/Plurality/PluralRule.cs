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
        /// Wrap <paramref name="rule"/> into evaluatable.
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
        /// Wrap <paramref name="expression"/> into evaluatable.
        /// </summary>
        /// <param name="name">(optional)</param>
        /// <param name="expression"></param>
        public PluralRuleEvaluatable(string name, IPluralRuleExpression expression) : this(name, expression.Rule, expression.Samples) { }

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
    public class PluralExpressionCase : PluralRuleEvaluatable, IPluralCase
    {
        /// <summary>
        /// Is this case optional
        /// </summary>
        public bool Optional { get; internal set; }

        /// <summary>
        /// Adapt expression into <see cref="IPluralCase"/> with name.
        /// </summary>
        /// <param name="name">(optional) name</param>
        /// <param name="optional">Is case optional for translator to supply</param>
        /// <param name="rule">rule expression</param>
        /// <param name="samples">(optional) samples</param>
        public PluralExpressionCase(string name, bool optional, IExpression rule, ISamplesExpression[] samples = default) : base(name, rule, samples)
        {
            Optional = optional;
        }

        /// <summary>
        /// Adapt expression into <see cref="IPluralCase"/> with name.
        /// </summary>
        /// <param name="name">(optional) name</param>
        /// <param name="expression">expression</param>
        public PluralExpressionCase(string name, bool optional, IPluralRuleExpression expression) : this(name, optional, expression.Rule, expression.Samples)
        {
        }
    }

    /// <summary>
    /// Abstract plural Case, without expression.
    /// </summary>
    public abstract class PluralCase : IPluralCase, IPluralRuleEvaluatable
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Is this case optional
        /// </summary>
        public bool Optional { get; internal set; }

        /// <summary>
        /// Evaluate number whether it matches the case.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public abstract bool Evaluate(IPluralNumber number);

        /// <summary>
        /// Adapt expression into <see cref="IPluralCase"/> with name.
        /// </summary>
        /// <param name="name">(optional) name</param>
        /// <param name="optional">Is case optional for translator to supply</param>
        public PluralCase(string name, bool optional) 
        {
            this.Name = name;
            this.Optional = optional;
        }

        /// <summary>
        /// Zero case that matches when number is 0.
        /// </summary>
        public class Zero : PluralCase
        {
            private static readonly Zero __zero = new Zero("zero");
            private static readonly Zero __Zero = new Zero("Zero");

            /// <summary>
            /// Zero case "zero"
            /// </summary>
            public static Zero _zero => __zero;

            /// <summary>
            /// Zero case "Zero"
            /// </summary>
            public static Zero _Zero => __Zero;

            /// <summary>
            /// Create zero case
            /// </summary>
            /// <param name="name"></param>
            public Zero(string name) : base(name, true) { }

            /// <summary>
            /// Compare to zero.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
            {
                if (number == null) return false;
                return PluralNumberComparer.Instance.Equals(number, DecimalNumber._0);
            }
        }

        /// <summary>
        /// Null case that matches when number is null.
        /// </summary>
        public class Null : PluralCase
        {
            private static readonly Null __null = new Null("null");
            private static readonly Null __Null = new Null("Null");

            /// <summary>
            /// Null case "null"
            /// </summary>
            public static Null _null => __null;

            /// <summary>
            /// Null case "Null"
            /// </summary>
            public static Null _Null => __Null;

            /// <summary>
            /// Create case.
            /// </summary>
            /// <param name="name"></param>
            public Null(string name) : base(name, true) { }

            /// <summary>
            /// Compare to zero.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => number == null || PluralNumberComparer.Instance.Equals(number, DecimalNumber.Empty);
        }

        /// <summary>
        /// True case that matches always.
        /// </summary>
        public class True : PluralCase
        {
            private static readonly True __other = new True("other");
            private static readonly True __Other = new True("Other");

            /// <summary>
            /// True case "true"
            /// </summary>
            public static True other => __other;

            /// <summary>
            /// True case "True"
            /// </summary>
            public static True Other => __Other;

            /// <summary>
            /// Create case.
            /// </summary>
            /// <param name="name"></param>
            public True(string name) : base(name, true) { }

            /// <summary>
            /// Compare to zero.
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            public override bool Evaluate(IPluralNumber number)
                => true;
        }
    }


}
