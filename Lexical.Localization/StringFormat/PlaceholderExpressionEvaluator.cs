// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           15.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

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
                BinaryOp.Equal => EvaluateEqual(a, b),
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
                BinaryOp.NotEqual => EvaluateNotEqual(a, b),
                BinaryOp.Or => EvaluateOr(a, b),
                BinaryOp.Power => EvaluatePower(a, b),
                BinaryOp.RightShift => EvaluateRightShift(a, b),
                BinaryOp.Subtract => EvaluateSubtract(a, b),
                BinaryOp.Xor => EvaluateXor(a, b),
                _ => throw new ArgumentException($"Unsupported {nameof(BinaryOp)} {bop.Op}")
              };
        }
        bool isInteger(object a) => a is int || a is uint || a is long || a is ulong;
        bool isFloat(object a) => a is float || a is double;
        string toString(object a) {
            if (a == null) return "";
            if (a is string str) return str;
            CultureInfo culture = FunctionEvaluationCtx.Culture;

            if (culture != null)
            {
                if (FunctionEvaluationCtx.FormatProvider != null)
                {
                    ICustomFormatter customFormatter = FunctionEvaluationCtx.FormatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
                    if (customFormatter != null)
                    {
                        string formatted = customFormatter.Format("", a, culture);
                        if (formatted != null) return formatted;
                    }
                }
                if (a is IFormattable formattable)
                {
                    string formatted = formattable.ToString("", culture);
                    if (formatted != null) return formatted;
                }
            }

            return a.ToString();
        }
        double toFloat(object a) => a switch { float f => f, double d => d, _ => Double.Parse(toString(a)) };
        long toInteger(object a) => a switch { int i => i, long l => l, uint ui => ui, ulong ul => (long)ul, _ => long.Parse(toString(a)) };
        bool toBool(object a) => a switch { bool b => b, _ => bool.Parse(toString(a)) };
        object EvaluateAdd(object a, object b)
        {
            if (a == null) return b;
            if (b == null) return a;
            if (a is string || b is string) return toString(a) + toString(b);
            if (isFloat(a) || isFloat(b)) return toFloat(a) + toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) + toInteger(b);
            return toString(a) + toString(b);
        }
        object EvaluateAnd(object a, object b)
            => isInteger(a) && isInteger(b) ? toInteger(a) & toInteger(b) : throw new ArgumentException($"Cannot evaluate And with arguments {a} and {b}");
        object EvaluateCoalesce(object a, object b)
            => a == null ? b : a;
        object EvaluateDivide(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) / toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) / toInteger(b);
            throw new ArgumentException($"Cannot evaluate '/' with arguments {a} and {b}");
        }
        object EvaluateEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a is string || b is string) return toString(a) == toString(b);
            if (isFloat(a) || isFloat(b)) return toFloat(a) == toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) == toInteger(b);
            return toString(a) == toString(b);
        }
        object EvaluateGreaterThan(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) > toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) > toInteger(b);
            throw new ArgumentException($"Cannot evaluate '>' with arguments {a} and {b}");
        }
        object EvaluateGreaterThanOrEqual(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) >= toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) >= toInteger(b);
            throw new ArgumentException($"Cannot evaluate '>=' with arguments {a} and {b}");
        }
        object EvaluateIn(object a, object b)
        {
            if (a == null || b == null) return false;
            if (b is IEnumerable enumr)
            {
                foreach (object o in enumr)
                    if ((bool)EvaluateEqual(a, o)) return true;
                return false;
            }
            throw new ArgumentException($"Cannot evaluate 'in' with arguments {a} and {b}");
        }
        object EvaluateLeftShift(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) << (int)toInteger(b);
            throw new ArgumentException($"Cannot evaluate '<<' with arguments {a} and {b}");
        }
        object EvaluateLessThan(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) < toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) < toInteger(b);
            throw new ArgumentException($"Cannot evaluate '<' with arguments {a} and {b}");
        }
        object EvaluateLessThanOrEqual(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) <= toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) <= toInteger(b);
            throw new ArgumentException($"Cannot evaluate '<=' with arguments {a} and {b}");
        }
        object EvaluateLogicalAnd(object a, object b)
            => toBool(a) && toBool(b);
        object EvaluateLogicalOr(object a, object b)
            => toBool(a) || toBool(b);
        object EvaluateModulo(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) % toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) % toInteger(b);
            throw new ArgumentException($"Cannot evaluate '%' with arguments {a} and {b}");
        }
        object EvaluateMultiply(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) * toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) * toInteger(b);
            throw new ArgumentException($"Cannot evaluate '*' with arguments {a} and {b}");
        }
        object EvaluateNotEqual(object a, object b)
        {
            if (a == null && b == null) return false;
            if (a == null || b == null) return true;
            if (a is string || b is string) return toString(a) != toString(b);
            if (isFloat(a) || isFloat(b)) return toFloat(a) != toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) != toInteger(b);
            return toString(a) != toString(b);
        }
        object EvaluateOr(object a, object b)
            => isInteger(a) && isInteger(b) ? toInteger(a) | toInteger(b) : throw new ArgumentException($"Cannot evaluate Or with arguments {a} and {b}");
        object EvaluatePower(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return Math.Pow(toFloat(a), toFloat(b));
            if (isInteger(a) || isInteger(b)) return Math.Pow(toInteger(a), toInteger(b));
            throw new ArgumentException($"Cannot evaluate 'pow' with arguments {a} and {b}");
        }
        object EvaluateRightShift(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) >> (int)toInteger(b);
            throw new ArgumentException($"Cannot evaluate '>>' with arguments {a} and {b}");
        }
        object EvaluateSubtract(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) - toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) - toInteger(b);
            throw new ArgumentException($"Cannot evaluate '-' with arguments {a} and {b}");
        }
        object EvaluateXor(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) ^ toInteger(b);
            throw new ArgumentException($"Cannot evaluate '^' with arguments {a} and {b}");
        }
        object EvaluateCondition(object a, object b, object c)
            => toBool(a) ? b : c;
    }

}
