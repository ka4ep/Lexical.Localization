// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           15.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
        /// Placeholder status.
        /// </summary>
        public LineStatus Status;

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
        /// 
        /// If error occurs <see cref="Status"/> is updated with an error code.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
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
                        _ => Error(LineStatus.PlaceholderErrorTrinaryOpUnsupported)
                    },
                IArgumentIndexExpression arg => GetArgByIndex(arg.Index),
                IArgumentNameExpression argName => GetArgByName(argName.Name),
                ICallExpression call => EvaluateCall(call),
                _ => Error(LineStatus.PlaceholderErrorExpressionUnsupported)
            };

        object Error(LineStatus code)
        {
            Status.UpPlaceholder(code);
            return null;
        }

        object GetArgByIndex(int argIndex)
        {
            if (Args == null || argIndex < 0 || argIndex >= Args.Length)
            {
                Status.UpPlaceholder(LineStatus.PlaceholderWarningArgumentValuesNotSupplied);
                return null;
            }
            return Args[argIndex];
        }

        object GetArgByName(string argName)
        {
            if (argName == null) return null;
            object result;
            if (ArgsByName != null && ArgsByName.TryGetValue(argName, out result)) return result;
            Status.UpPlaceholder(LineStatus.PlaceholderWarningArgumentValuesNotSupplied);
            return null;
        }

        object EvaluateCall(ICallExpression call)
        {
            object result;
            if (call.Args.Length==0)
            {
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, out result)) return result;
                if (Functions.Default.TryEvaluate(call.Name, ref FunctionEvaluationCtx, out result)) return result;
            } else if (call.Args.Length == 1)
            {
                object arg0 = Evaluate(call.Args[0]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, out result)) return result;
                if (Functions.Default.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, out result)) return result;
            } else if (call.Args.Length == 2)
            {
                object arg0 = Evaluate(call.Args[0]), arg1 = Evaluate(call.Args[1]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, out result)) return result;
                if (Functions.Default.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, out result)) return result;
            }
            else if (call.Args.Length == 3)
            {
                object arg0 = Evaluate(call.Args[0]), arg1 = Evaluate(call.Args[1]), arg2 = Evaluate(call.Args[2]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, arg2, out result)) return result;
                if (Functions.Default.TryEvaluate(call.Name, ref FunctionEvaluationCtx, arg0, arg1, arg2, out result)) return result;
            } else
            {
                object[] args = new object[call.Args.Length];
                for (int i = 0; i < call.Args.Length; i++)
                    args[i] = Evaluate(call.Args[i]);
                if (FunctionEvaluationCtx.Functions.TryEvaluate(call.Name, ref FunctionEvaluationCtx, args, out result)) return result;
                if (Functions.Default.TryEvaluate(call.Name, ref FunctionEvaluationCtx, args, out result)) return result;
            }
            Status.UpPlaceholder(LineStatus.PlaceholderErrorCallExpressionUnknown);
            return null;
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
                _ => Error(LineStatus.PlaceholderErrorUnaryOpUnsupported)
            };
        }
        object EvaluateNegate(object o)
        {
            Error(LineStatus.PlaceholderErrorExpressionUnsupported);
            return null;
        }
        object EvaluateNot(object o)
        {
            Error(LineStatus.PlaceholderErrorExpressionUnsupported);
            return null;
        }
        object EvaluateOnesComplement(object o)
        {
            Error(LineStatus.PlaceholderErrorExpressionUnsupported);
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
                _ => Error(LineStatus.PlaceholderErrorBinaryOpUnsupported)
        };
        }
        bool isInteger(object a) => a is int || a is uint || a is long || a is ulong;
        bool isFloat(object a) => a is float || a is double;

        /// <summary>
        /// Formulate object <paramref name="a"/> when format is "".
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public string toString(object a) {
            if (a == null) return "";

            // Return string as is
            if (a is string str) return str;

            // Use custom formatter
            if (FunctionEvaluationCtx.FormatProvider != null)
            {
                ICustomFormatter customFormatter = FunctionEvaluationCtx.FormatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
                if (customFormatter != null)
                {
                    string formatted = customFormatter.Format("", a, FunctionEvaluationCtx.Culture);
                    if (formatted != null) return formatted;
                }
            }

            // Handle enumeration
            if (a is Enum @enum && FunctionEvaluationCtx.EnumResolver != null)
            {
                // Evaluate enum
                LineString enum_str = FunctionEvaluationCtx.EvaluateEnum(@enum);
                // Return value
                if (enum_str.Value != null)
                {
                    Status.Up(enum_str.Status);
                    return enum_str.Value;
                }
            }

            // Call culture specific formattable 
            if (FunctionEvaluationCtx.Culture != null && a is IFormattable formattable)
            {
                string formatted = formattable.ToString("", FunctionEvaluationCtx.Culture);
                if (formatted != null) return formatted;
            }

            // ToString()
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
            => isInteger(a) && isInteger(b) ? toInteger(a) & toInteger(b) : Error(LineStatus.PlaceholderErrorLogicalOpError);
        object EvaluateCoalesce(object a, object b)
            => a == null ? b : a;
        object EvaluateDivide(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) / toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) / toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
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
            return Error(LineStatus.PlaceholderErrorInequalityOpError);
        }
        object EvaluateGreaterThanOrEqual(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) >= toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) >= toInteger(b);
            return Error(LineStatus.PlaceholderErrorInequalityOpError);
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
            return Error(LineStatus.PlaceholderErrorExpressionEvaluation);
        }
        object EvaluateLessThan(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) < toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) < toInteger(b);
            return Error(LineStatus.PlaceholderErrorInequalityOpError);
        }
        object EvaluateLessThanOrEqual(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) <= toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) <= toInteger(b);
            return Error(LineStatus.PlaceholderErrorInequalityOpError);
        }
        object EvaluateLogicalAnd(object a, object b)
            => toBool(a) && toBool(b);
        object EvaluateLogicalOr(object a, object b)
            => toBool(a) || toBool(b);
        object EvaluateModulo(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) % toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) % toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
        }
        object EvaluateMultiply(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) * toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) * toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
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
            => isInteger(a) && isInteger(b) ? toInteger(a) | toInteger(b) : Error(LineStatus.PlaceholderErrorArithmeticOpError);
        object EvaluatePower(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return Math.Pow(toFloat(a), toFloat(b));
            if (isInteger(a) || isInteger(b)) return Math.Pow(toInteger(a), toInteger(b));
            return Error(LineStatus.PlaceholderErrorFloatingOpError);
        }
        object EvaluateLeftShift(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) << (int)toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
        }
        object EvaluateRightShift(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) >> (int)toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
        }
        object EvaluateSubtract(object a, object b)
        {
            if (isFloat(a) || isFloat(b)) return toFloat(a) - toFloat(b);
            if (isInteger(a) || isInteger(b)) return toInteger(a) - toInteger(b);
            return Error(LineStatus.PlaceholderErrorArithmeticOpError);
        }
        object EvaluateXor(object a, object b)
        {
            if (isInteger(a) || isInteger(b)) return toInteger(a) ^ toInteger(b);
            return Error(LineStatus.PlaceholderErrorLogicalOpError);
        }
        object EvaluateCondition(object a, object b, object c)
            => toBool(a) ? b : c;
    }

}
