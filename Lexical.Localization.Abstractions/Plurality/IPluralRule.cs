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
    ///   <item><see cref="IPluralRuleEvaluator"/></item>
    ///   <item><see cref="IPluralRuleExpression"/></item>
    /// </list>
    /// </summary>
    public interface IPluralRule
    {
    }

    /// <summary>
    /// Plurality case.
    /// </summary>
    public interface IPluralCase : IPluralRule
    {
        /// <summary>
        /// Name of the case, e.g. "Zero", "One"
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Is case optional.
        /// </summary>
        bool Optional { get; }
    }

    /// <summary>
    /// A single case of a <see cref="IPluralCategory"/>.
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
    public interface IPluralRuleEvaluator : IPluralRule
    {
        /// <summary>
        /// Evaluate whether the case applies to <paramref name="number"/>.
        /// </summary>
        /// <param name="number">numeric and text representation of numberic value</param>
        /// <returns>true or false</returns>
        bool Evaluate(IPluralNumber number);
    }

    /// <summary>
    /// Plural rule, e.g. "v = 0 and i % 10 = 1".
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

}
