// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           15.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    public partial struct StringFormatEvaluationContext
    {
        /// <summary>
        /// Evaluate expression into object.
        /// 
        /// The following types are supported by Ops:
        /// <list type="bullet">
        ///     <item>string</item>
        ///     <item>long</item>
        ///     <item>double</item>
        ///     <item>object (as string or understod by function such as Format)</item>
        ///     <item>null</item>
        /// </list>
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If invalid argument</exception>
        public object Evaluate(IExpression exp)
            => exp switch
            {
                IConstantExpression ce => ce.Value,
                IParenthesisExpression pe => Evaluate(pe.Element),
                IUnaryOpExpression uop =>
                    uop.Op switch
                    {
                        UnaryOp.Negate => null,
                        UnaryOp.Not => null,
                        UnaryOp.OnesComplement => null,
                        UnaryOp.Plus => Evaluate(uop.Element),
                        _ => throw new ArgumentException($"Unsupported {nameof(UnaryOp)} {uop.Op}")
                    },
                IBinaryOpExpression bop =>
                    bop.Op switch
                    {
                        BinaryOp.Add => (object)null,
                        BinaryOp.And => null,
                        BinaryOp.Coalesce => null,
                        BinaryOp.Divide => null,
                        BinaryOp.Equal => EvaluateEquals(Evaluate(bop.Left), Evaluate(bop.Right)),
                        BinaryOp.GreaterThan => null,
                        BinaryOp.GreaterThanOrEqual => null,
                        BinaryOp.In => null,
                        BinaryOp.LeftShift => null,
                        BinaryOp.LessThan => null,
                        BinaryOp.LessThanOrEqual => null,
                        BinaryOp.LogicalAnd => null,
                        BinaryOp.LogicalOr => null,
                        BinaryOp.Modulo => null,
                        BinaryOp.Multiply => null,
                        BinaryOp.NotEqual => EvaluateNotEquals(Evaluate(bop.Left), Evaluate(bop.Right)),
                        BinaryOp.Or => null,
                        BinaryOp.Power => null,
                        BinaryOp.RightShift => null,
                        BinaryOp.Subtract => null,
                        BinaryOp.Xor => null,
                        _ => throw new ArgumentException($"Unsupported {nameof(BinaryOp)} {bop.Op}")
                    },
                ITrinaryOpExpression top =>
                    top.Op switch
                    {
                        TrinaryOp.Condition => (object)null,
                        _ => throw new ArgumentException($"Unsupported {nameof(TrinaryOp)} {top.Op}")
                    },
                IArgumentIndexExpression arg => Args == null || arg.Index < 0 || arg.Index >= Args.Length ? null : Args[arg.Index],
                IArgumentNameExpression argName => GetArgByName(argName.Name),
                ICallExpression call => null,
                _ => throw new ArgumentException($"Unsupported exeption {exp}")
            };

        object GetArgByName(string argName)
        {
            if (ArgsByName == null || argName == null) return null;
            object result;
            if (ArgsByName.TryGetValue(argName, out result)) return result;
            return null;
        }

        bool EvaluateEquals(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            return left.Equals(right);
        }
        bool EvaluateNotEquals(object left, object right)
        {
            if (left == null && right == null) return false;
            if (left == null || right == null) return true;
            return !left.Equals(right);
        }

    }

}
