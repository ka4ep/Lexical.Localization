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
    /// Plural rule expression.
    /// 
    /// e.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one] v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// </summary>
    public interface IPluralRuleExpression : IExpression
    {
        /// <summary>
        /// (Optional) Rule infos.
        /// 
        /// E.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
        /// </summary>
        IPluralRuleInfosExpression Infos { get; }

        /// <summary>
        /// (Optional) Rule expression that can evaluate a number
        /// </summary>
        IExpression Rule { get; }

        /// <summary>
        /// (Optioanl) Samples
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

    /// <summary>
    /// Rule infos expression.
    /// 
    /// e.g. "[RuleSet=Unicode.CLDRv35,Category=cardinal,Culture=fi,Case=one]"
    /// </summary>
    public interface IPluralRuleInfosExpression : IExpression
    {
        /// <summary>
        /// Array of infos.
        /// </summary>
        IPluralRuleInfoExpression[] Infos { get; }
    }

    /// <summary>
    /// Expression for one info entry.
    /// </summary>
    public interface IPluralRuleInfoExpression : IExpression
    {
        /// <summary>
        /// Info name. Valid values are: "RuleSet", "Category", "Culture", "Case".
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Info value. 
        /// </summary>
        String Value { get; }
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
        IExpression MinValue { get; }

        /// <summary>
        /// End of range (inclusive)
        /// </summary>
        IExpression MaxValue { get; }
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
}
