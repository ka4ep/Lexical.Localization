// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           27.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
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
        public const int FNVHashBasis = unchecked((int)0x811C9DC5);

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
            }
            return this;
        }

        /// <summary>
        /// Hash in expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public PluralRuleExpressionHashCode Hash(IExpression exp)
            => exp == null ? this : exp switch
            {
                IConstantExpression c => c==null?this:c.Value is string str ? Hash(str) : Hash(c.Value.GetHashCode()),
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
                _ => this
            };

    }

}
