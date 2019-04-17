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
    /// Simple expression. Used for string format functions and plural rules.
    /// </summary>
    public interface IExpression
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UnaryOp {
        /// <summary>+</summary>
        Plus,
        /// <summary>-</summary>
        Negate,
        /// <summary>~</summary>
        OnesComplement,
        /// <summary>!</summary>
        Not
    };

    /// <summary>
    /// 
    /// </summary>
    public enum BinaryOp
    {
        // Arithmetic operands
        /// <summary>+</summary>
        Add,
        /// <summary>-</summary>
        Subtract,                         // x = 2-2
        /// <summary>*</summary>
        Multiply,
        /// <summary>/</summary>
        Divide,
        /// <summary>%</summary>
        Modulo,
        /// <summary>pow</summary>
        Power,

        // Logical operands
        /// <summary>&amp;</summary>
        And,
        /// <summary>|</summary>
        Or,
        /// <summary>^</summary>
        ExclusiveOr,

        // Other operands
        /// <summary>&lt;&lt;</summary>
        LeftShift,
        /// <summary>&gt;&gt;</summary>
        RightShift,

        // Comparison operands
        /// <summary>&lt;</summary>
        LessThan,
        /// <summary>&gt;</summary>
        GreaterThan,
        /// <summary>&lt;=</summary>
        LessThanOrEqual,
        /// <summary>&gt;=</summary>
        GreaterThanOrEqual,
        /// <summary>!=</summary>
        NotEqual,
        /// <summary>==</summary>
        Equal,

        /// <summary>=, in group (right side) comparer</summary>
        In,
    };


    /// <summary>
    /// 
    /// </summary>
    public interface IUnaryOpExpression : IExpression
    {
        /// <summary>
        /// 
        /// </summary>
        UnaryOp Op { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression Element { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBinaryOpExpression : IExpression
    {
        /// <summary>
        /// 
        /// </summary>
        BinaryOp Op { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression Left { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression Right { get; }
    }

    /// <summary>
    /// Argument by name
    /// </summary>
    public interface IArgumentNameExpression : IExpression
    {
        /// <summary>
        /// Name of the argument
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Argument by index
    /// </summary>
    public interface IArgumentIndexExpression : IExpression
    {
        /// <summary>
        /// Index of the argument
        /// </summary>
        int Index { get; }
    }

    /// <summary>
    /// Function call expression
    /// </summary>
    public interface IFunctionExpression : IExpression
    {
        /// <summary>
        /// 
        /// </summary>
        IExpression[] Args { get; }
    }

    /// <summary>
    /// Literal constant
    /// </summary>
    public interface IConstantExpression : IExpression
    {
        /// <summary>
        /// 
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// Expression for multiple values.
    /// </summary>
    public interface IValuesExpression : IExpression
    {

    }

    /// <summary>
    /// Range of interger values.
    /// </summary>
    public interface IRangeExpression : IValuesExpression
    {
        /// <summary>
        /// Start of range (inclusive)
        /// </summary>
        IConstantExpression MinValue { get; }

        /// <summary>
        /// End of range (inclusive)
        /// </summary>
        IConstantExpression MaxValue { get; }
    }

    /// <summary>
    /// Group of values.
    /// </summary>
    public interface IGroupExpression : IValuesExpression
    {
        /// <summary>
        /// Values.
        /// </summary>
        IExpression[] Values { get; }
    }

    /// <summary>
    /// e.g. "@decimal 1.0, 1.00, 1.000, 1.0000"
    /// </summary>
    public interface ISamplesExpression : IExpression
    {
        /// <summary>
        /// Name of sample group, e.g. "decimal"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Each value is one of:
        /// <list>
        /// <item><see cref="IConstantExpression"/></item>
        /// <item><see cref="IRangeExpression"/></item>
        /// </list>
        /// </summary>
        IExpression[] Samples { get; }
    }

}
