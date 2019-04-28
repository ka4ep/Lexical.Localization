// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Prints <see cref="IPluralRuleExpression"/> as string.
    /// </summary>
    public abstract class PluralRuleExpressionPrinter
    {
        /// <summary>
        /// String builder
        /// </summary>
        public readonly StringBuilder sb;

        /// <summary>
        /// Create printer
        /// </summary>
        /// <param name="sb"></param>
        public PluralRuleExpressionPrinter(StringBuilder sb = default)
        {
            this.sb = sb ?? new StringBuilder();
        }

        /// <summary>
        /// Print as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => sb.ToString();

        /// <summary>
        /// Append string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public PluralRuleExpressionPrinter Append(string str)
        {
            if (str != null) sb.Append(str);
            return this;
        }

        /// <summary>
        /// Append char
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public PluralRuleExpressionPrinter Append(char ch)
        {
            sb.Append(ch);
            return this;
        }

        /// <summary>
        /// Append List
        /// </summary>
        /// <param name="enumr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public PluralRuleExpressionPrinter Append(IEnumerable enumr, string separator)
        {
            if (enumr == null) return this;
            int c = 0;
            IEnumerator etor = enumr.GetEnumerator();
            while (etor.MoveNext())
            {
                if (c++ > 0) sb.Append(separator);
                if (etor.Current is IExpression exp) Append(exp);
                else Append(etor.Current.ToString());
            }
            return this;
        }

        /// <summary>
        /// Append expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="postSeparator"></param>
        /// <returns></returns>
        public abstract PluralRuleExpressionPrinter Append(IExpression exp, string postSeparator = null);

    }

    /// <summary>
    /// Prints <see cref="IPluralRuleExpression"/> as C# code.
    /// 
    /// <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/>
    /// </summary>
    public class PluralRuleExpressionCSharpPrinter : PluralRuleExpressionPrinter
    {
        /// <summary>
        /// Create printer
        /// </summary>
        /// <param name="sb"></param>
        public PluralRuleExpressionCSharpPrinter(StringBuilder sb = default) : base(sb) { }

        /// <summary>
        /// Append expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="postSeparator"></param>
        /// <returns></returns>
        public override PluralRuleExpressionPrinter Append(IExpression exp, string postSeparator = null)
        {
            if (exp == null) return this;

            if (exp is IBinaryOpExpression blop && (blop.Op == BinaryOp.Equal || blop.Op == BinaryOp.NotEqual))
            {
                // Handle group
                if (blop.Left is IGroupExpression gleft)
                {
                    Append('(');
                    int c = 0;
                    foreach (var _exp in gleft.Values)
                    {
                        if (c++ > 0) Append("||");
                        Append(new BinaryOpExpression(blop.Op, _exp, blop.Right));
                    }
                    Append(')');
                    return this;
                }
                if (blop.Right is IGroupExpression gright)
                {
                    Append('(');
                    int c = 0;
                    foreach (var _exp in gright.Values)
                    {
                        if (c++ > 0) Append("||");
                        Append(new BinaryOpExpression(blop.Op, blop.Left, _exp));
                    }
                    Append(')');
                    return this;
                }
            }

            var x = exp switch
            {
                IPluralRuleExpression pre => Append(pre.Rule, " "),
                IRangeExpression range => Append("(PluralNumberComparer.Instance.Compare(").Append(range.MinValue).Append(", value)<=0 && PluralNumberComparer.Instance.Compare(value, ").Append(range.MaxValue).Append(")<=0)"),
                IConstantExpression c => 
                    c.Value switch
                    {
                        long _long => Append("new DecimalNumber.Long(").Append(c.Value.ToString() + "L)"),
                        int _int => Append("new DecimalNumber.Long(").Append(c.Value.ToString() + "L)"),
                        // TODO floats, decimal, text
                        // TODO Put consts into dictionary and static readonly variables
                        _ => this // <- text
                    },
                IArgumentNameExpression arg => 
                    arg.Name switch
                    {
                        "n" => Append("value.N"),
                        "i" => Append("value.I"),
                        "e" => Append("value.E"),
                        "f" => Append("value.F"),
                        "t" => Append("value.T"),
                        "v" => Append("new DecimalNumber.Long(value.F_Digits)"),
                        "w" => Append("new DecimalNumber.Long(value.T_Digits)"),
                        _ => this,
                    },
                IParenthesisExpression par => Append('(').Append(par.Element).Append(')'),
                IUnaryOpExpression uop => Append(uop.Op switch { UnaryOp.Plus => "+", UnaryOp.Not => "!", UnaryOp.OnesComplement => "~", UnaryOp.Negate => "-", _ => "" }).Append(uop.Element),
                IBinaryOpExpression bop => 
                    bop.Op switch
                    {
                        BinaryOp.LogicalAnd => Append('(').Append(bop.Left).Append(" && ").Append(bop.Right).Append(')'),
                        BinaryOp.LogicalOr => Append('(').Append(bop.Left).Append(" || ").Append(bop.Right).Append(')'),
                        BinaryOp.And => Append('(').Append(bop.Left).Append(" && ").Append(bop.Right).Append(')'),
                        BinaryOp.Or => Append('(').Append(bop.Left).Append(" || ").Append(bop.Right).Append(')'),
                        BinaryOp.Modulo => Append(bop.Left).Append(".Modulo(").Append(bop.Right.ToString()).Append(")"),
                        _ => this
                    },
                _ => this
            };

            if (postSeparator != null) sb.Append(postSeparator);
            return this;
        }

    }

    /// <summary>
    /// Prints <see cref="IPluralRuleExpression"/> as expression string.
    /// 
    /// Uses unicode <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/> notation,
    /// with some additional features. 
    /// 
    /// The rule info is added in brackets "[RuleSet=Unicode.CLDRv35,Category=cardinal,Case=zero,Culture=fi,Optional=1]".
    /// </summary>
    public class PluralRuleExpressionStringPrinter : PluralRuleExpressionPrinter
    {
        /// <summary>
        /// Create printer
        /// </summary>
        /// <param name="sb"></param>
        public PluralRuleExpressionStringPrinter(StringBuilder sb = default) : base(sb) { }

        /// <summary>
        /// Append expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="postSeparator"></param>
        /// <returns></returns>
        public override PluralRuleExpressionPrinter Append(IExpression exp, string postSeparator = null)
        {
            if (exp == null) return this;
            var x = exp switch
            {
                IPluralRuleExpression pre => Append(pre.Infos, " ").Append(pre.Rule, " ").Append(pre.Samples, " "),
                IPluralRuleInfosExpression infos => Append('[').Append(infos.Infos, ",").Append(']'),
                IPluralRuleInfoExpression info => Append(info.Name).Append('=').Append(info.Value),
                ISamplesExpression samples => (Append("@"+samples.Name+" ")as PluralRuleExpressionStringPrinter).AppendSamples(samples.Samples, ", "),
                IRangeExpression range  => Append(range.MinValue).Append("..").Append(range.MaxValue),
                IGroupExpression group => Append(group.Values, ","),
                IInfiniteExpression inf => Append('…'),
                IConstantExpression c => Append(c.Value?.ToString()),
                IArgumentNameExpression arg => Append(arg.Name),
                IParenthesisExpression par => Append('(').Append(par.Element).Append(')'),
                IUnaryOpExpression uop => Append(uop.Op switch { UnaryOp.Plus=>"+", UnaryOp.Not=>"not ", UnaryOp.OnesComplement=>"~", UnaryOp.Negate=>"-", _=>"¤" }).Append(uop.Element),
                IBinaryOpExpression bop => 
                    Append(bop.Left).Append(bop.Op switch {
                        BinaryOp.Add=>"+",
                        BinaryOp.And=>" and ",
                        BinaryOp.Divide=> "/",
                        BinaryOp.Equal=> "=",
                        BinaryOp.GreaterThan=> ">",
                        BinaryOp.GreaterThanOrEqual=> ">=",
                        BinaryOp.LeftShift=> "<<",
                        BinaryOp.RightShift=> ">>",
                        BinaryOp.LessThan=> "<",
                        BinaryOp.LessThanOrEqual=> "<=",
                        BinaryOp.LogicalAnd=> " and ",
                        BinaryOp.LogicalOr=> " or ",
                        BinaryOp.Modulo=> " % ",
                        BinaryOp.Multiply=> "*",
                        BinaryOp.NotEqual => "!=",
                        BinaryOp.Or => " or ",
                        BinaryOp.Power => " pow ",
                        BinaryOp.Xor => "^",
                        BinaryOp.Subtract => "-",
                        BinaryOp.Coalesce => "??",
                        _ => "¤"
                    }).Append(bop.Right),
                _ => this
            };
            if (postSeparator != null) sb.Append(postSeparator);
            return this;
        }

        /// <summary>
        /// Append List
        /// </summary>
        /// <param name="enumr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public PluralRuleExpressionPrinter AppendSamples(IEnumerable enumr, string separator)
        {
            if (enumr == null) return this;
            int c = 0;
            IEnumerator etor = enumr.GetEnumerator();
            while (etor.MoveNext())
            {
                if (c++ > 0) sb.Append(separator);
                if (etor.Current is IExpression exp) AppendSample(exp);
                else Append(etor.Current.ToString());
            }
            return this;
        }

        /// <summary>
        /// Append sample
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public PluralRuleExpressionPrinter AppendSample(IExpression exp)
        {
            if (exp == null) return this;
            var x = exp switch
            {
                ISamplesExpression samples => Append("@" + samples.Name + " ").Append(samples.Samples, ", "),
                IRangeExpression range => Append(range.MinValue).Append("~").Append(range.MaxValue),
                IGroupExpression group => Append(group.Values, ", "),
                IInfiniteExpression inf => Append('…'),
                IConstantExpression c => Append(c.Value?.ToString()),
                _ => this
            };
            return this;
        }
    }

}
