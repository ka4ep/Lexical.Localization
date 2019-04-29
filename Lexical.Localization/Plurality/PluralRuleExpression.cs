// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Plural rule
    /// </summary>
    public class PluralRuleExpression : Expression, IPluralRuleExpression
    {
        /// <summary>
        /// (Optional) Rule infos.
        /// 
        /// E.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
        /// </summary>
        public IPluralRuleInfosExpression Infos { get; }
        /// <summary> </summary>
        public IExpression Rule { get; internal set; }
        /// <summary> </summary>
        public ISamplesExpression[] Samples { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infos">(optional) name of the expression</param>
        /// <param name="rule"></param>
        /// <param name="samples"></param>
        public PluralRuleExpression(IPluralRuleInfosExpression infos, IExpression rule, params ISamplesExpression[] samples)
        {
            Infos = infos;
            Rule = rule;
            Samples = samples;
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);
    }

    /// <summary>
    /// Plural rule infos.
    /// 
    /// e.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
    /// </summary>
    public class PluralRuleInfosExpression : Expression, IPluralRuleInfosExpression
    {
        /// <summary>
        /// Array of infos.
        /// </summary>
        public IPluralRuleInfoExpression[] Infos { get; internal set; }

        /// <summary>
        /// Create infos
        /// </summary>
        /// <param name="infos"></param>
        public PluralRuleInfosExpression(params IPluralRuleInfoExpression[] infos)
        {
            Infos = infos;
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);

    }

    /// <summary>
    /// Single plural rule info expression
    /// </summary>
    public class PluralRuleInfoExpression : Expression, IPluralRuleInfoExpression
    {
        /// <summary>
        /// Info name. Valid values are: "RuleSet", "Category", "Culture", "Case".
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// Info value. 
        /// </summary>
        public String Value { get; internal set; }

        /// <summary>
        /// Create rule info
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public PluralRuleInfoExpression(string name, string value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);

    }

    /// <summary>
    /// List of plural rule samples.
    /// </summary>
    public class SamplesExpression : Expression, ISamplesExpression
    {
        /// <summary> </summary>
        public string Name { get; internal set; }
        /// <summary> </summary>
        public IExpression[] Samples { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="samples"></param>
        public static SamplesExpression Create(string name, params Object[] samples)
            => new SamplesExpression(name, samples.Select(s => new ConstantExpression(s)).ToArray());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="samples"></param>
        public SamplesExpression(string name, params IExpression[] samples)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Samples = samples ?? throw new ArgumentNullException(nameof(samples));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);


    }

    /// <summary>
    /// Marks sample sequence as infinite.
    /// 
    /// "…" or "..."
    /// </summary>
    public class InfiniteExpression : Expression, IInfiniteExpression
    {

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append('…');
        }

    }


    /// <summary>
    /// Range expression.
    /// </summary>
    public class RangeExpression : Expression, IRangeExpression
    {
        /// <summary> </summary>
        public IExpression MinValue { get; internal set; }
        /// <summary> </summary>
        public IExpression MaxValue { get; internal set; }

        /// <summary>
        /// Create range expression
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public RangeExpression(IExpression minValue, IExpression maxValue)
        {
            MinValue = minValue ?? throw new ArgumentNullException(nameof(minValue));
            MaxValue = maxValue ?? throw new ArgumentNullException(nameof(maxValue));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);


    }

    /// <summary>
    /// Group expression
    /// </summary>
    public class GroupExpression : Expression, IGroupExpression
    {
        /// <summary> </summary>
        public IExpression[] Values { get; internal set; }
        /// <summary>
        /// Create group
        /// </summary>
        /// <param name="values"></param>
        public GroupExpression(params IExpression[] values)
        {
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
            => new PluralRuleExpressionStringPrinter(sb).Append(this);

    }


}
