// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.Exp
{
    /// <summary>
    /// General purpose expression interface.
    /// 
    /// The actual rules of printing, parsing and evaluation depends on the context the expression is used in.
    /// </summary>
    public interface IExpression
    {
    }

    /// <summary>
    /// Expression that contains component expressions.
    /// </summary>
    public interface ICompositeExpression : IExpression
    {
        /// <summary>
        /// Number of component expressions.
        /// </summary>
        int ComponentCount { get; }

        /// <summary>
        /// Get component expression.
        /// </summary>
        /// <param name="ix"></param>
        /// <returns></returns>
        IExpression GetComponent(int ix);
    }

    /// <summary>
    /// Parenthesis expression "(exp)"
    /// </summary>
    public interface IParenthesisExpression : ICompositeExpression
    {
        /// <summary>
        /// Inner expression
        /// </summary>
        IExpression Element { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UnaryOp
    {
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
        Subtract,
        /// <summary>*</summary>
        Multiply,
        /// <summary>/</summary>
        Divide,
        /// <summary>%</summary>
        Modulo,
        /// <summary>pow</summary>
        Power,
        /// <summary>&amp;</summary>
        And,
        /// <summary>|</summary>
        Or,
        /// <summary>^</summary>
        Xor,

        // Logical operands
        /// <summary>&amp;&amp;</summary>
        LogicalAnd,
        /// <summary>||</summary>
        LogicalOr,

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
        /// <summary>??</summary>
        Coalesce,

        /// <summary>=, in group (right side) comparer</summary>
        In,
    };

    /// <summary>
    /// 
    /// </summary>
    public enum TrinaryOp
    {
        /// <summary>
        /// ?: condition operator, e.g. "condition_exp ? true_exp : false_exp"
        /// </summary>
        Condition
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IUnaryOpExpression : ICompositeExpression
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
    public interface IBinaryOpExpression : ICompositeExpression
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
    /// "b ? x : y" 
    /// </summary>
    public interface ITrinaryOpExpression : ICompositeExpression
    {
        /// <summary>
        /// 
        /// </summary>
        TrinaryOp Op { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression A { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression B { get; }

        /// <summary>
        /// 
        /// </summary>
        IExpression C { get; }
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
    public interface ICallExpression : ICompositeExpression
    {
        /// <summary>
        /// Function name
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Function arguments
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

}
