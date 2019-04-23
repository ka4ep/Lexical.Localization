// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Exp
{
    /// <summary>
    /// General purpose expression class.
    /// 
    /// The actual rules of printing, parsing and evaluation depends on the context the expression is used in.
    /// </summary>
    public abstract class Expression : IExpression
    {
        /// <summary>
        /// Print to string for debugging purposes.
        /// This does not produce parseable expression. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Append(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Append expression to <paramref name="sb"/> for debugging purposes.
        /// This does not produce parseable expression. 
        /// </summary>
        /// <param name="sb"></param>
        public abstract void Append(StringBuilder sb);

        /// <summary>
        /// Append <paramref name="exp"/> to <paramref name="sb"/>.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="exp"></param>
        public void AppendExp(StringBuilder sb, IExpression exp)
        {
            if (exp is Expression _exp) _exp.Append(sb); else sb.Append(exp.ToString());
        }

    }

    /// <summary>
    /// Unary operation expression
    /// </summary>
    public class UnaryOpExpression : Expression, IUnaryOpExpression
    {
        /// <summary> </summary>
        public UnaryOp Op { get; internal set; }
        /// <summary> </summary>
        public IExpression Element { get; internal set; }
        /// <summary>
        /// Create unary operator expression
        /// </summary>
        /// <param name="op"></param>
        /// <param name="component"></param>
        public UnaryOpExpression(Exp.UnaryOp op, IExpression component)
        {
            Op = op;
            Element = component ?? throw new ArgumentNullException(nameof(component));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append(Op switch { UnaryOp.Negate=>"-", UnaryOp.Not=>"!", UnaryOp.OnesComplement=>"~", UnaryOp.Plus=>"+", _=>"??" });
            AppendExp(sb, Element);
        }
    }

    /// <summary>
    /// Binary operation expression
    /// </summary>
    public class BinaryOpExpression : Expression, IBinaryOpExpression
    {
        /// <summary> </summary>
        public BinaryOp Op { get; internal set; }
        /// <summary> </summary>
        public IExpression Left { get; internal set; }
        /// <summary> </summary>
        public IExpression Right { get; internal set; }
        /// <summary>
        /// Create expression
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public BinaryOpExpression(Exp.BinaryOp op, IExpression left, IExpression right)
        {
            Op = op;
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            AppendExp(sb, Left);
            sb.Append(' ');
            sb.Append(Op switch { 
                BinaryOp.And => "&", BinaryOp.Or => "|", BinaryOp.LogicalAnd => "&&", BinaryOp.LogicalOr => "||",
                BinaryOp.Divide => "/", BinaryOp.Equal=>"=", BinaryOp.Xor=>"^",
                BinaryOp.Add => "+",
                BinaryOp.GreaterThan=>">", BinaryOp.GreaterThanOrEqual=>">=", BinaryOp.In => "=", BinaryOp.LeftShift => "<<", BinaryOp.LessThan => "<", BinaryOp.LessThanOrEqual => "<=",
                BinaryOp.Modulo=>"%", BinaryOp.Multiply=>"*", BinaryOp.NotEqual=>"!=", BinaryOp.Power=>"^", BinaryOp.RightShift=>">>", 
                BinaryOp.Subtract=>"-",
                _ => "??" });
            sb.Append(' ');
            AppendExp(sb, Right);
        }

    }

    /// <summary>
    /// Argument name
    /// </summary>
    public class ArgumentNameExpression : Expression, IArgumentNameExpression
    {
        /// <summary> </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Create expression
        /// </summary>
        /// <param name="name"></param>
        public ArgumentNameExpression(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append(Name);
        }
    }

    /// <summary>
    /// Argument index reference expression
    /// </summary>
    public class ArgumentIndexExpression : Expression, IArgumentIndexExpression
    {
        /// <summary> </summary>
        public int Index { get; internal set; }
        /// <summary>
        /// Create expression
        /// </summary>
        /// <param name="index"></param>
        public ArgumentIndexExpression(int index)
        {
            Index = index;
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append("#");
            sb.Append(Index);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class FunctionExpression : Expression, IFunctionExpression
    {
        /// <summary> </summary>
        public String Name { get; internal set; }

        /// <summary> </summary>
        public IExpression[] Args { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public FunctionExpression(string name, IExpression[] args)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Args = args ?? throw new ArgumentNullException(nameof(args));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append(Name);
            sb.Append('(');
            for (int i=0; i<Args.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                AppendExp(sb, Args[i]);
            }
            sb.Append(')');
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConstantExpression : Expression, IConstantExpression
    {
        /// <summary> </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public ConstantExpression(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append(Value);
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
        {
            AppendExp(sb, MinValue);
            sb.Append("..");
            AppendExp(sb, MaxValue);
        }

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
        public GroupExpression(IExpression[] values)
        {
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                AppendExp(sb, Values[i]);
            }
        }
    }

    /// <summary>
    /// Plural rule
    /// </summary>
    public class PluralRuleExpression : Expression, IPluralRuleExpression
    {
        /// <summary> </summary>
        public String Name { get; internal set; }
        /// <summary> </summary>
        public IExpression Rule { get; internal set; }
        /// <summary> </summary>
        public ISamplesExpression[] Samples { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">(optional) name of the expression</param>
        /// <param name="rule"></param>
        /// <param name="samples"></param>
        public PluralRuleExpression(string name, IExpression rule, ISamplesExpression[] samples)
        {
            Name = name;
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            Samples = samples ?? throw new ArgumentNullException(nameof(samples));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            if (Name != null)
            {
                sb.Append('§');
                sb.Append(Name);
                sb.Append(' ');
            }

            AppendExp(sb, Rule);

            foreach(ISamplesExpression samples in Samples)
            {
                sb.Append(" ");
                AppendExp(sb, samples);
            }
        }
    }

    /// <summary>
    /// Plural rules
    /// </summary>
    public class PluralRulesExpression : Expression, IPluralRulesExpression
    {
        /// <summary> </summary>
        public string[] Names { get; internal set; }
        /// <summary> </summary>
        public IPluralRuleExpression[] Rules { get; internal set; }

        /// <summary>
        /// Create rules .
        /// </summary>
        /// <param name="names"></param>
        /// <param name="rules"></param>
        public PluralRulesExpression(string[] names, IPluralRuleExpression[] rules)
        {
            Names = names;
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            foreach(string name in Names)
            { 
                sb.Append('#');
                sb.Append(name);
                sb.Append(' ');
            }

            for (int i=0; i<Rules.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                AppendExp(sb, Rules[i]);
            }
        }

    }

    /// <summary>
    /// Plural ruleset
    /// </summary>
    public class PluralRuleSetExpression : Expression, IPluralRuleSetExpression
    {
        /// <summary> </summary>
        public string Name { get; internal set; }
        /// <summary> </summary>
        public IPluralRulesExpression[] RulesList { get; internal set; }
        /// <summary>
        /// Create rules.
        /// </summary>
        /// p<param name="name"></param>
        /// <param name="rulesList"></param>
        public PluralRuleSetExpression(string name, IPluralRulesExpression[] rulesList)
        {
            Name = name;
            RulesList = rulesList ?? throw new ArgumentNullException(nameof(rulesList));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            if (Name != null)
            {
                sb.Append('$');
                sb.Append(Name);
                sb.Append(' ');
            }

            for (int i = 0; i < RulesList.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                AppendExp(sb, RulesList[i]);
            }
        }

    }

    /// <summary>
    /// Plural rulesets
    /// </summary>
    public class PluralRuleSetsExpression : Expression, IPluralRuleSetsExpression
    {
        /// <summary> </summary>
        public IPluralRuleSetExpression[] RuleSets { get; internal set; }
        /// <summary>
        /// Create rules.
        /// </summary>
        /// <param name="ruleSets"></param>
        public PluralRuleSetsExpression(IPluralRuleSetExpression[] ruleSets)
        {
            RuleSets = ruleSets ?? throw new ArgumentNullException(nameof(ruleSets));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            for (int i = 0; i < RuleSets.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                AppendExp(sb, RuleSets[i]);
            }
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
            for (int i=0; i<Samples.Length; i++)
            {
                if (i>0) sb.Append(" ");
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

    /// <summary>
    /// Parenthesis expression.
    /// </summary>
    public class ParenthesisExpression : Expression, IParenthesisExpression
    {
        /// <summary> </summary>
        public IExpression Element { get; internal set; }

        /// <summary>
        /// Create parenthesis expression
        /// </summary>
        /// <param name="element"></param>
        public ParenthesisExpression(IExpression element)
        {
            this.Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <inheritdoc/>
        public override void Append(StringBuilder sb)
        {
            sb.Append('(');
            AppendExp(sb, Element);
            sb.Append(')');
        }
    }
}
