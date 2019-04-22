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
    /// Expression
    /// </summary>
    public class Expression : IExpression
    {
    }

    /// <summary>
    /// Unary operation expression
    /// </summary>
    public class UnaryOpExpression : Expression, IUnaryOpExpression
    {
        /// <summary> </summary>
        public Exp.UnaryOp Op { get; internal set; }
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
    }

    /// <summary>
    /// Binary operation expression
    /// </summary>
    public class BinaryOpExpression : Expression, IBinaryOpExpression
    {
        /// <summary> </summary>
        public Exp.BinaryOp Op { get; internal set; }
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
    }

    /// <summary>
    /// 
    /// </summary>
    public class FunctionExpression : Expression, IFunctionExpression
    {
        /// <summary> </summary>
        public IExpression[] Args { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public FunctionExpression(IExpression[] args)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));
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
    }

    /// <summary>
    /// Marks sample sequence as infinite.
    /// 
    /// "…" or "..."
    /// </summary>
    public class InfiniteExpression : Expression, IInfiniteExpression
    {
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

    }
}
