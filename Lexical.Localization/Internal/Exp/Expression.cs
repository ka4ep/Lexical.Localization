// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Exp
{
    /// <summary>
    /// 
    /// </summary>
    public class Expression : IExpression
    {

        /// <summary>
        /// Unary operation expression
        /// </summary>
        public class UnaryOp : IUnaryOpExpression
        {
            /// <summary> </summary>
            public Exp.UnaryOp Op { get; internal set; }
            /// <summary> </summary>
            public IExpression Element { get; internal set; }
            /// <summary>
            /// Create unary operator expression
            /// </summary>
            /// <param name="op"></param>
            /// <param name="component"></param>
            public UnaryOp(Exp.UnaryOp op, IExpression component)
            {
                Op = op;
                Element = component ?? throw new ArgumentNullException(nameof(component));
            }
        }

        /// <summary>
        /// Binary operation expression
        /// </summary>
        public class BinaryOp : IBinaryOpExpression
        {
            /// <summary> </summary>
            public Exp.BinaryOp Op { get; internal set; }
            /// <summary> </summary>
            public IExpression Left { get; internal set; }
            /// <summary> </summary>
            public IExpression Right { get; internal set; }
            /// <summary>
            /// Create expression
            /// </summary>
            /// <param name="op"></param>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public BinaryOp(Exp.BinaryOp op, IExpression left, IExpression right)
            {
                Op = op;
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }
        }

        /// <summary>
        /// Argument name
        /// </summary>
        public class ArgumentName : IArgumentNameExpression
        {
            /// <summary> </summary>
            public string Name { get; internal set; }
            /// <summary>
            /// Create expression
            /// </summary>
            /// <param name="name"></param>
            public ArgumentName(string name)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
            }
        }

        /// <summary>
        /// Argument index reference expression
        /// </summary>
        public class ArgumentIndex : IArgumentIndexExpression
        {
            /// <summary> </summary>
            public int Index { get; internal set; }
            /// <summary>
            /// Create expression
            /// </summary>
            /// <param name="index"></param>
            public ArgumentIndex(int index)
            {
                Index = index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Function : IFunctionExpression
        {
            /// <summary> </summary>
            public IExpression[] Args { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public Function(IExpression[] args)
            {
                Args = args ?? throw new ArgumentNullException(nameof(args));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Constant : IConstantExpression
        {
            /// <summary> </summary>
            public object Value { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public Constant(object value)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Range : IRangeExpression
        {
            /// <summary> </summary>
            public IConstantExpression MinValue { get; internal set; }
            /// <summary> </summary>
            public IConstantExpression MaxValue { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="minValue"></param>
            /// <param name="maxValue"></param>
            public Range(IConstantExpression minValue, IConstantExpression maxValue)
            {
                MinValue = minValue ?? throw new ArgumentNullException(nameof(minValue));
                MaxValue = maxValue ?? throw new ArgumentNullException(nameof(maxValue));
            }
        }

        /// <summary>
        /// Group expression
        /// </summary>
        public class Group : IGroupExpression
        {
            /// <summary> </summary>
            public IExpression[] Values { get; internal set; }
            /// <summary>
            /// Create group
            /// </summary>
            /// <param name="values"></param>
            public Group(IExpression[] values)
            {
                Values = values ?? throw new ArgumentNullException(nameof(values));
            }
        }

        /// <summary>
        /// Plural rule
        /// </summary>
        public class PluralRule : IPluralRuleExpression
        {
            /// <summary> </summary>
            public IExpression Rule { get; internal set; }
            /// <summary> </summary>
            public ISamplesExpression[] Samples { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rule"></param>
            /// <param name="samples"></param>
            public PluralRule(IExpression rule, ISamplesExpression[] samples)
            {
                Rule = rule ?? throw new ArgumentNullException(nameof(rule));
                Samples = samples ?? throw new ArgumentNullException(nameof(samples));
            }
        }
    }
}
