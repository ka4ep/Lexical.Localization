// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           15.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Makes equals comparison for expressions that are used with string format placeholders.
    /// </summary>
    public class StringFormatExpressionEquals
    {
        /// <summary>
        /// Hash in expression
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public bool Equals(IExpression e1, IExpression e2)
        {
            if (e1 == e2) return true;
            if (e1 == null && e2 == null) return true;
            if (e1 is IUnaryOpExpression uop1 && e2 is IUnaryOpExpression uop2) return uop1.Op == uop2.Op && Equals(uop1.Element, uop2.Element);
            if (e1 is IBinaryOpExpression bop1 && e2 is IBinaryOpExpression bop2) return bop1.Op == bop2.Op && Equals(bop1.Left, bop2.Left) && Equals(bop1.Right, bop2.Right);
            if (e1 is ITrinaryOpExpression top1 && e2 is ITrinaryOpExpression top2) return top1.Op == top2.Op && Equals(top1.A, top2.A) && Equals(top1.B, top2.B) && Equals(top1.C, top2.C);
            if (e1 is IArgumentIndexExpression arg1 && e2 is IArgumentIndexExpression arg2) return arg1.Index == arg2.Index;
            if (e1 is ICallExpression c1 && e2 is ICallExpression c2)
            {
                if (c1.Name != c2.Name) return false;
                if (c1.Args == null && c2.Args == null) return true;
                if (c1.Args == null || c2.Args == null) return false;
                if (c1.Args.Length != c2.Args.Length) return false;
                for (int i = 0; i < c1.Args.Length; i++)
                    if (!Equals(c1.Args[i], c2.Args[i])) return false;
                return true;
            }
            if (e1 is IConstantExpression x1 && e2 is IConstantExpression x2)
            {
                if (x1 == null && x2 == null) return true;
                if (x1 == null || x2 == null) return true;
                return x1.Equals(x2);
            }
            if (e1 is IParenthesisExpression p1 && e2 is IParenthesisExpression p2) return Equals(p1.Element, p2.Element);
            return false;
        }
    }

}
