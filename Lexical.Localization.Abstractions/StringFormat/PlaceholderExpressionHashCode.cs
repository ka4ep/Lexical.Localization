// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           27.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Calculates <see cref="IExpression"/> hashcode of <see cref="IPlaceholder"/>s.
    /// </summary>
    public struct PlaceholderExpressionHashCode
    {
        /// <summary>
        /// Start value
        /// </summary>
        public static readonly PlaceholderExpressionHashCode Initial = unchecked((int)0x811C9DC5);

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
        public static implicit operator int(PlaceholderExpressionHashCode r) => r.HashCode;

        /// <summary>
        /// Convert to struct
        /// </summary>
        /// <param name="code"></param>
        public static implicit operator PlaceholderExpressionHashCode(int code) => new PlaceholderExpressionHashCode { HashCode = code };

        /// <summary>
        /// Hash in string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public PlaceholderExpressionHashCode Hash(string str)
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
        public PlaceholderExpressionHashCode Hash(char ch)
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
        public PlaceholderExpressionHashCode Hash(int v)
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
        public PlaceholderExpressionHashCode Hash(IEnumerable enumr)
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
        public PlaceholderExpressionHashCode Hash(IExpression exp)
        {
            if (exp == null) return this;
            if (exp is IUnaryOpExpression uop) return Hash(nameof(IUnaryOpExpression)).Hash((int)uop.Op).Hash(uop.Element);
            if (exp is IBinaryOpExpression bop) return Hash(nameof(IBinaryOpExpression)).Hash((int)bop.Op).Hash(bop.Left).Hash(bop.Right);
            if (exp is ITrinaryOpExpression top) return Hash(nameof(ITrinaryOpExpression)).Hash((int)top.Op).Hash(top.A).Hash(top.B).Hash(top.C);
            if (exp is IArgumentIndexExpression arg) return Hash(nameof(IArgumentIndexExpression)).Hash(arg.Index);
            if (exp is IArgumentNameExpression arg_) return Hash(nameof(IArgumentNameExpression)).Hash(arg_.Name);
            if (exp is ICallExpression c_) return Hash(nameof(ICallExpression)).Hash(c_.Name).Hash(c_.Args);
            if (exp is IConstantExpression c) return c == null ? this : c.Value is string str ? Hash(nameof(IConstantExpression)).Hash(str) : Hash(nameof(IConstantExpression)).Hash(c.Value.GetHashCode());
            if (exp is IParenthesisExpression par) return Hash(nameof(IParenthesisExpression)).Hash(par.Element);
            return this;
        }
    }

}
