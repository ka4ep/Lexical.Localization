// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           27.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Calculates <see cref="IPluralRuleExpression"/> hashcode so that strings
    /// produce consistent values across sessions.
    /// </summary>
    public struct PluralRuleExpressionHashCode
    {
        /// <summary>
        /// Start value
        /// </summary>
        public static readonly PluralRuleExpressionHashCode Initial = unchecked((int)0x811C9DC5);

        /// <summary>
        /// Prime factor
        /// </summary>
        const int FNVHashPrime = 0x1000193;

        /// <summary>
        /// Hashcode
        /// </summary>
        public int HashCode;

        /// <summary>
        /// Convert to int
        /// </summary>
        /// <param name="r"></param>
        public static implicit operator int(PluralRuleExpressionHashCode r) => r.HashCode;

        /// <summary>
        /// Convert to struct
        /// </summary>
        /// <param name="code"></param>
        public static implicit operator PluralRuleExpressionHashCode(int code) => new PluralRuleExpressionHashCode { HashCode = code };

        /// <summary>
        /// HAsh in string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(string str)
        {
            if (str != null)
            {
                foreach (char c in str)
                {
                    HashCode ^= c;
                    HashCode *= FNVHashPrime;
                }
            }
            return this;
        }

        /// <summary>
        /// Hash in char
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(char ch)
        {
            HashCode ^= ch;
            HashCode *= FNVHashPrime;
            return this;
        }

        /// <summary>
        /// Hash in integer
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(int v)
        {
            HashCode ^= v;
            HashCode *= FNVHashPrime;
            return this;
        }


        /// <summary>
        /// Hash in List
        /// </summary>
        /// <param name="enumr"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(IEnumerable enumr)
        {
            if (enumr == null) return this;
            IEnumerator etor = enumr.GetEnumerator();
            while (etor.MoveNext())
            {
                if (etor.Current is IExpression exp) this = this.Hash(exp);
                else if (etor.Current is String str) this = this.Hash(str);
                else if (etor.Current is int _int) this = this.Hash(_int);
                else this = this.Hash(etor.Current.GetHashCode());
            }
            return this;
        }

        /// <summary>
        /// Hash in expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(IExpression exp)
        {
            if (exp == null) return this;
            if (exp is IConstantExpression c) return c == null ? this : c.Value is string str ? Hash(nameof(IConstantExpression)).Hash(str) : Hash(nameof(IConstantExpression)).Hash(c.Value.GetHashCode());
            if (exp is IPluralRuleExpression pre) return Hash(nameof(IPluralRuleExpression)).Hash(pre.Infos).Hash(pre.Rule).Hash(pre.Samples);
            if (exp is IPluralRuleInfosExpression infos) return Hash(nameof(IPluralRuleInfosExpression)).Hash(infos.Infos);
            if (exp is IPluralRuleInfoExpression info) return Hash(nameof(IPluralRuleInfoExpression)).Hash(info.Name).Hash(info.Value);
            if (exp is ISamplesExpression samples) return Hash(nameof(ISamplesExpression)).Hash(samples.Name).Hash(samples.Samples);
            if (exp is IRangeExpression range) return Hash(nameof(IRangeExpression)).Hash(range.MinValue).Hash(range.MaxValue);
            if (exp is IGroupExpression group) return Hash(nameof(IGroupExpression)).Hash(group.Values);
            if (exp is IInfiniteExpression inf) return Hash('…');
            if (exp is IArgumentNameExpression arg) return Hash(nameof(IArgumentNameExpression)).Hash(arg.Name);
            if (exp is IParenthesisExpression par) return Hash(nameof(IParenthesisExpression)).Hash(par.Element);
            if (exp is IUnaryOpExpression uop) return Hash(nameof(IUnaryOpExpression)).Hash((int)uop.Op).Hash(uop.Element);
            if (exp is IBinaryOpExpression bop) return Hash(nameof(IBinaryOpExpression)).Hash((int)bop.Op).Hash(bop.Left).Hash(bop.Right);
            if (exp is ITrinaryOpExpression top) return Hash(nameof(ITrinaryOpExpression)).Hash((int)top.Op).Hash(top.A).Hash(top.B).Hash(top.C);
            return this;
            /*
            return exp switch
               {
                   IConstantExpression c => c == null ? this : c.Value is string str ? Hash(str) : Hash(c.Value.GetHashCode()),
                   IPluralRuleExpression pre => Hash(pre.Infos).Hash(pre.Rule).Hash(pre.Samples),
                   IPluralRuleInfosExpression infos => Hash(infos.Infos),
                   IPluralRuleInfoExpression info => Hash(info.Name).Hash(info.Value),
                   ISamplesExpression samples => Hash(samples.Name).Hash(samples.Samples),
                   IRangeExpression range => Hash(range.MinValue).Hash(range.MaxValue),
                   IGroupExpression group => Hash(group.Values),
                   IInfiniteExpression inf => Hash('…'),
                   IArgumentNameExpression arg => Hash(arg.Name),
                   IParenthesisExpression par => Hash(par.Element),
                   IUnaryOpExpression uop => Hash((int)uop.Op).Hash(uop.Element),
                   IBinaryOpExpression bop => Hash((int)bop.Op).Hash(bop.Left).Hash(bop.Right),
                   ITrinaryOpExpression top => Hash((int)top.Op).Hash(top.A).Hash(top.B).Hash(top.C),
                   _ => this
               };*/
        }
    }

}
