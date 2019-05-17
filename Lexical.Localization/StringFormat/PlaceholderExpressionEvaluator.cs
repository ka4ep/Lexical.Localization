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
    /// <summary>
    /// String evaluation context
    /// </summary>
    public struct PlaceholderExpressionEvaluator
    {
        /// <summary>
        /// 
        /// </summary>
        public FunctionEvaluationContext FunctionEvaluationCtx;

        /// <summary>
        /// (optional) Format Arguments.
        /// Used when string format uses arguments by index, e.g. "Hello, {0}"
        /// </summary>
        public object[] Args;

        /// <summary>
        /// (optional) Format arguments by name
        /// Used when string format uses arguments by name, e.g. "Hello, %s"
        /// </summary>
        public IReadOnlyDictionary<string, object> ArgsByName;

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
                IUnaryOpExpression uop => EvaluateUnary(uop),
                IBinaryOpExpression bop => EvaluateBinary(bop),
                ITrinaryOpExpression top =>
                    top.Op switch
                    {
                        TrinaryOp.Condition => EvaluateCondition(Evaluate(top.A), Evaluate(top.B), Evaluate(top.C)),
                        _ => throw new ArgumentException($"Unsupported {nameof(TrinaryOp)} {top.Op}")
                    },
                IArgumentIndexExpression arg => Args == null || arg.Index < 0 || arg.Index >= Args.Length ? null : Args[arg.Index],
                IArgumentNameExpression argName => GetArgByName(argName.Name),
                ICallExpression call => EvaluateCall(call),
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

        object EvaluateCall(ICallExpression call)
        {
            object result;
            if (call.Args.Length==0)
            {
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, out result)) return result;
                if (DefaultFunctions.Instance.TryEvaluate(call.Name, ref FunctionEvaluationCtx, out result)) return result;
            } else if (call.Args.Length == 1)
            {
                object arg0 = Evaluate(call.Args[0]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, out result)) return result;
                if (DefaultFunctions.Instance.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, out result)) return result;
            } else if (call.Args.Length == 2)
            {
                object arg0 = Evaluate(call.Args[0]), arg1 = Evaluate(call.Args[1]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, out result)) return result;
                if (DefaultFunctions.Instance.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, out result)) return result;
            }
            else if (call.Args.Length == 3)
            {
                object arg0 = Evaluate(call.Args[0]), arg1 = Evaluate(call.Args[1]), arg2 = Evaluate(call.Args[2]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, arg2, out result)) return result;
                if (DefaultFunctions.Instance.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, arg2, out result)) return result;
            } else
            {
                object[] args = new object[call.Args.Length];
                for (int i = 0; i < call.Args.Length; i++)
                    args[i] = Evaluate(call.Args[i]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, args, out result)) return result;
                if (DefaultFunctions.Instance.TryEvaluate(call.Name, ref FunctionEvaluationCtx, args, out result)) return result;
            }
            throw new InvalidOperationException();
        }
        object EvaluateUnary(IUnaryOpExpression uop)
        {
            object a = Evaluate(uop.Element);
            return uop.Op switch
            {
                UnaryOp.Negate => EvaluateNegate(a),
                UnaryOp.Not => EvaluateNot(a),
                UnaryOp.OnesComplement => EvaluateOnesComplement(a),
                UnaryOp.Plus => a,
                _ => throw new ArgumentException($"Unsupported {nameof(UnaryOp)} {uop.Op}")
            };
        }
        object EvaluateNegate(object o)
        {
            return null;
        }
        object EvaluateNot(object o)
        {
            return null;
        }
        object EvaluateOnesComplement(object o)
        {
            return null;
        }
        object EvaluateBinary(IBinaryOpExpression bop)
        {
            object a = Evaluate(bop.Left), b = Evaluate(bop.Right);
            return bop.Op switch
            {
                BinaryOp.Add => EvaluateAdd(a, b),
                BinaryOp.And => EvaluateAnd(a, b),
                BinaryOp.Coalesce => EvaluateCoalesce(a, b),
                BinaryOp.Divide => EvaluateDivide(a, b),
                BinaryOp.Equal => EvaluateEquals(a, b),
                BinaryOp.GreaterThan => EvaluateGreaterThan(a, b),
                BinaryOp.GreaterThanOrEqual => EvaluateGreaterThanOrEqual(a, b),
                BinaryOp.In => EvaluateIn(a, b),
                BinaryOp.LeftShift => EvaluateLeftShift(a, b),
                BinaryOp.LessThan => EvaluateLessThan(a, b),
                BinaryOp.LessThanOrEqual => EvaluateLessThanOrEqual(a, b),
                BinaryOp.LogicalAnd => EvaluateLogicalAnd(a, b),
                BinaryOp.LogicalOr => EvaluateLogicalOr(a , b),
                BinaryOp.Modulo => EvaluateModulo(a, b),
                BinaryOp.Multiply => EvaluateMultiply(a, b),
                BinaryOp.NotEqual => EvaluateNotEquals(a, b),
                BinaryOp.Or => EvaluateOr(a, b),
                BinaryOp.Power => EvaluatePower(a, b),
                BinaryOp.RightShift => EvaluateRightShift(a, b),
                BinaryOp.Subtract => EvaluateSubtract(a, b),
                BinaryOp.Xor => EvaluateXor(a, b),
                _ => throw new ArgumentException($"Unsupported {nameof(BinaryOp)} {bop.Op}")
              };
        }
        object EvaluateAdd(object a, object b)
        {
            return null;
        }
        object EvaluateAnd(object a, object b)
        {
            return null;
        }
        object EvaluateCoalesce(object a, object b)
        {
            return null;
        }
        object EvaluateDivide(object a, object b)
        {
            return null;
        }
        object EvaluateEqual(object a, object b)
        {
            return null;
        }
        object EvaluateGreaterThan(object a, object b)
        {
            return null;
        }
        object EvaluateGreaterThanOrEqual(object a, object b)
        {
            return null;
        }
        object EvaluateIn(object a, object b)
        {
            return null;
        }
        object EvaluateLeftShift(object a, object b)
        {
            return null;
        }
        object EvaluateLessThan(object a, object b)
        {
            return null;
        }
        object EvaluateLessThanOrEqual(object a, object b)
        {
            return null;
        }
        object EvaluateLogicalAnd(object a, object b)
        {
            return null;
        }
        object EvaluateLogicalOr(object a, object b)
        {
            return null;
        }
        object EvaluateModulo(object a, object b)
        {
            return null;
        }
        object EvaluateMultiply(object a, object b)
        {
            return null;
        }
        object EvaluateNotEqual(object a, object b)
        {
            return null;
        }
        object EvaluateOr(object a, object b)
        {
            return null;
        }
        object EvaluatePower(object a, object b)
        {
            return null;
        }
        object EvaluateRightShift(object a, object b)
        {
            return null;
        }
        object EvaluateSubtract(object a, object b)
        {
            return null;
        }
        object EvaluateXor(object a, object b)
        {
            return null;
        }
        object EvaluateCondition(object a, object b, object c)
        {
            return null;
        }

    }

}
