// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Description of plurality rule.
    /// 
    /// <list type="bullet">
    ///   <item><see cref="IPluralCase"/></item>
    ///   <item><see cref="IPluralCategoryPart"/></item>
    ///   <item><see cref="IPluralRuleEvaluatable"/></item>
    ///   <item><see cref="IPluralRuleExpression"/></item>
    /// </list>
    /// </summary>
    public interface IPluralRule
    {
        /// <summary>
        /// (Optional) Name of the rule. "§One" "§Other".
        /// 
        /// E.g. "zero", "one".
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Plurality case.
    /// </summary>
    public interface IPluralCase : IPluralRule
    {
        /// <summary>
        /// Is case optional.
        /// </summary>
        bool Optional { get; }
    }

    /// <summary>
    /// A plural case that carries info about its part as member of <see cref="IPluralCategory"/>.
    /// </summary>
    public interface IPluralCategoryPart : IPluralRule
    {
        /// <summary>
        /// The the category this case is part of.
        /// </summary>
        IPluralCategory Category { get; }

        /// <summary>
        /// Index in <see cref="IPluralCategory.Cases"/>.
        /// </summary>
        int CaseIndex { get; }
    }

    /// <summary>
    /// Interface for classes that evaluate whether argument value and text matches the plurality case.
    /// </summary>
    public interface IPluralRuleEvaluatable : IPluralRule
    {
        /// <summary>
        /// Evaluate whether the case applies to <paramref name="number"/>.
        /// </summary>
        /// <param name="number">numeric and text representation of numberic value</param>
        /// <returns>true or false</returns>
        bool Evaluate(IPluralNumber number);
    }

    /// <summary>
    /// Plural rule expression.
    /// 
    /// e.g. "§one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// </summary>
    public interface IPluralRuleExpression : IExpression, IPluralRule
    {
        /// <summary>
        /// Rule expression that can evaluate parameter
        /// </summary>
        IExpression Rule { get; }

        /// <summary>
        /// Samples
        /// </summary>
        ISamplesExpression[] Samples { get; }
    }

    /// <summary>
    /// e.g. "@decimal 1.0, 1.00, 1.000, 1.0000"
    /// </summary>
    public interface ISamplesExpression : IExpression
    {
        /// <summary>
        /// Name of sample group, e.g. "integer" and "decimal"
        /// 
        /// Known groups:
        /// <list type="bullet">
        ///     <item>integer</item>
        ///     <item>decimal</item>
        /// </list>
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Each value is one of:
        /// <list>
        /// <item><see cref="IConstantExpression"/></item>
        /// <item><see cref="IRangeExpression"/></item>
        /// <item><see cref="IInfiniteExpression"/></item>
        /// </list>
        /// 
        /// If list ends with <see cref="IInfiniteExpression"/> then there are infinite possible values.
        /// If not, then all the possible samples are listed in the samples list.
        /// </summary>
        IExpression[] Samples { get; }
    }

    /// <summary>
    /// Marks the list as infinite.
    /// 
    /// "…" or "..."
    /// </summary>
    public interface IInfiniteExpression : IExpression
    {
    }

}
