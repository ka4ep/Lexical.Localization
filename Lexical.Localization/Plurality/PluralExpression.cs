// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.Unicode
{
    /*
    /// <summary>
    /// Expression that describes a formula.
    /// It can evaluate, if value matches the expression.
    /// </summary>
    public abstract class PluralityBoolExpression : IPluralityCaseEvaluator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formattedValue"></param>
        /// <param name="numberFormat"></param>
        /// <returns></returns>
        public bool Evaluate(object value, string formattedValue, NumberFormatInfo numberFormat)
        {
            IPluralNumber number = new IPluralNumber(value, formattedValue, numberFormat);
            return Evaluate(ref number);
        }

        /// <summary>
        /// Evaluate with <paramref name="number"/>.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        protected abstract bool Evaluate(IPluralNumber number);

        /// <summary>
        /// And operator
        /// </summary>
        public class And : PluralityBoolExpression
        {
            /// <summary>
            /// Left side
            /// </summary>
            public readonly PluralityBoolExpression[] exps;

            /// <summary>
            /// Create and expression
            /// </summary>
            /// <param name="exps"></param>
            public And(params PluralityBoolExpression[] exps)
            {
                this.exps = exps ?? throw new ArgumentNullException(nameof(exps));
            }

            /// <summary>
            /// Evaluate
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            protected override bool Evaluate(IPluralNumber number)
            {
                foreach (var exp in exps)
                    if (!exp.Evaluate(ref number)) return false;
                return true;
            }
        }

        /// <summary>
        /// Or operator
        /// </summary>
        public class Or : PluralityBoolExpression
        {
            /// <summary>
            /// Exps
            /// </summary>
            public readonly PluralityBoolExpression[] exps;

            /// <summary>
            /// Create and expression
            /// </summary>
            /// <param name="exps"></param>
            public Or(params PluralityBoolExpression[] exps)
            {
                this.exps = exps ?? throw new ArgumentNullException(nameof(exps));
            }

            /// <summary>
            /// Evaluate
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            protected override bool Evaluate(IPluralNumber number)
            {
                foreach (var exp in exps)
                    if (exp.Evaluate(ref number)) return true;
                return false;
            }
        }

        /// <summary>
        /// Complement expression
        /// </summary>
        public class Not : PluralityBoolExpression
        {
            /// <summary>
            /// Component expression
            /// </summary>
            public readonly PluralityBoolExpression exp;

            /// <summary>
            /// Create complement
            /// </summary>
            /// <param name="exp"></param>
            public Not(PluralityBoolExpression exp)
            {
                this.exp = exp ?? throw new ArgumentNullException(nameof(exp));
            }

            /// <summary>
            /// Evaluate
            /// </summary>
            /// <param name="number"></param>
            /// <returns></returns>
            protected override bool Evaluate(IPluralNumber number)
                => !exp.Evaluate(ref number);
        }

        public abstract class PluralityValueExpression
        {
            public abstract IPluralNumber Evaluate(IPluralNumber number);

            public class Operand : PluralityValueExpression
            {
                public readonly char operand;
                public Operand(char operand)
                {
                    this.operand = operand;
                }

                public override IPluralNumber Evaluate(IPluralNumber number)
                    => number[operand];
            }

            public class IntegerValue : PluralityValueExpression
            {
                public readonly object valueObj;
                string valueText;

                public IntegerValue(object value)
                {
                    this.valueObj = value;
                    this.valueText = valueObj is IFormattable formattable ? formattable.ToString("", CultureInfo.InvariantCulture) : valueObj.ToString();
                }

                public override IPluralNumber Evaluate(IPluralNumber number)
                    => new IntegerValue();
            }

            public class DecimalValue : PluralityValueExpression
            {
                public readonly object valueObj;
                string valueText;

                public DecimalValue(object value)
                {
                    this.valueObj = value;
                    this.valueText = value is IFormattable formattable ? formattable.ToString("", CultureInfo.InvariantCulture) : value.ToString();
                }

                public override DecimalValue Evaluate(IPluralNumber number)
                    => new DecimalValue { Text = valueText, startIx = 0, endIx = valueText.Length, Number = valueObj, isFloat = true };
            }
        }

    }*/
}
