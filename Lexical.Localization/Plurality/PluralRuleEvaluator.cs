// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Rule expression evaluator.
    /// </summary>
    public struct PluralRuleEvaluator
    {
        /// <summary>
        /// Number to evaluate
        /// </summary>
        public IPluralNumber Number;

        /// <summary>
        /// Evaluate rule to number.
        /// </summary>
        /// <param name="rule">Rule to evaluate against.</param>
        /// <returns>true if rule matches</returns>
        /// <exception cref="NotSupportedException">Problem with expression</exception>
        public bool EvaluateBoolean(IExpression rule)
            => rule switch
            {
                IParenthesisExpression pexp => EvaluateBoolean(pexp.Element),
                IUnaryOpExpression uop =>
                    uop.Op switch
                    {
                        UnaryOp.Not => !EvaluateBoolean(uop.Element),
                        _ => throw new NotSupportedException($"Cannote valuate {nameof(IUnaryOpExpression)} with Op={uop.Op} to boolean result")
                    },
                IBinaryOpExpression bop => bop.Op switch
                {
                    BinaryOp.And => EvaluateBoolean(bop.Left) && EvaluateBoolean(bop.Right),
                    BinaryOp.Or => EvaluateBoolean(bop.Left) || EvaluateBoolean(bop.Right),
                    BinaryOp.ExclusiveOr => EvaluateBoolean(bop.Left) ^ EvaluateBoolean(bop.Right),
                    BinaryOp.Equal => false,
                    BinaryOp.NotEqual => false,
                    BinaryOp.LessThan => false,
                    BinaryOp.LessThanOrEqual => false,
                    BinaryOp.GreaterThan => false,
                    BinaryOp.GreaterThanOrEqual => false,
                    _ => throw new NotSupportedException($"Cannote valuate {nameof(IBinaryOpExpression)} with Op={bop.Op} to boolean result")
                },
                _ => false
            };

        /// <summary>
        /// Evaluate rule to number.
        /// </summary>
        /// <param name="rule">Rule to evaluate against.</param>
        /// <returns>number</returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public IPluralNumber EvaluateAsNumber(IExpression rule)
            => rule switch
            {
                //IUnaryOpExpression uop => uop.Op switch { UnaryOp. },
                IParenthesisExpression pexp => EvaluateAsNumber(pexp.Element),
                IBinaryOpExpression bop => bop.Op switch { BinaryOp.Modulo => Modulo(EvaluateAsNumber(bop.Left), EvaluateAsNumber(bop.Right)), _ => throw new NotSupportedException($"BinaryOpExpression '{bop.Op}' is not supported") },
                IArgumentNameExpression arg => arg.Name switch { "n" => Number.N, "i" => Number.I, "e" => Number.E, "f" => Number.F, "t" => Number.T, "v" => new DecimalNumber.Long(Number.F_Digits), "w" => new DecimalNumber.Long(Number.T_Digits), _ => throw new NotSupportedException($"Argument '{arg.Name}' is not supported") },
                IConstantExpression c => (IPluralNumber)c.Value,
                _ => throw new NotSupportedException($"{rule.GetType()} not supported.")
            };

        /// <summary>
        /// Evaluate modulo operation
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IPluralNumber Modulo(IPluralNumber left, IPluralNumber right)
        {
            long rightValue;
            if (!right.TryGet(out rightValue)) throw new InvalidOperationException("Failed to evaluate expression to numeric value.");
            if (rightValue < int.MinValue|| rightValue > int.MaxValue) throw new InvalidOperationException($"Modulo divider out of integer range (value={right})");
            return left.Modulo((int)rightValue);
        }

        //bool EvaluateBinaryOp(BinaryOp bop, IPlural)
    }

}
