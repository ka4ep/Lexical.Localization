// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
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
        public PluralRuleExpression(IPluralRuleInfosExpression infos, IExpression rule, ISamplesExpression[] samples)
        {
            Infos = infos;
            Rule = rule;
            Samples = samples;
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            bool hasPrev = false;
            if (Infos != null)
            {
                AppendExp(sb, Infos);
                hasPrev = true;
            }

            if (Rule != null)
            {
                if (hasPrev) { sb.Append(' '); hasPrev = false; }
                AppendExp(sb, Rule);
            }

            foreach (ISamplesExpression samples in Samples)
            {
                sb.Append(" ");
                AppendExp(sb, samples);
            }
        }
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
        public PluralRuleInfosExpression(IPluralRuleInfoExpression[] infos)
        {
            Infos = infos;
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            if (Infos != null)
            {
                sb.Append('[');
                for (int i = 0; i < Infos.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    AppendExp(sb, Infos[i]);
                }
                sb.Append(']');
            }
        }
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
        {
            sb.Append(Name);
            sb.Append('=');
            sb.Append(Value);
        }
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
        public SamplesExpression(string name, IExpression[] samples)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Samples = samples ?? throw new ArgumentNullException(nameof(samples));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            if (Name != null)
            {
                sb.Append('@');
                sb.Append(Name);
                sb.Append(' ');
            }
            for (int i = 0; i < Samples.Length; i++)
            {
                if (i > 0) sb.Append(" ");
                AppendExp(sb, Samples[i]);
            }
        }

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

}
