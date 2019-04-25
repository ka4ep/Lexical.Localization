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
    ///   <item><see cref="IPluralRuleEvaluatable"/></item>
    /// </list>
    /// </summary>
    public interface IPluralRule
    {
        /// <summary>
        /// Metadata info record.
        /// </summary>
        PluralRuleInfo Info { get; }
    }

    /// <summary>
    /// Interface for classes that evaluate whether argument value and text matches the plurality case.
    /// </summary>
    public interface IPluralRuleEvaluatable : IPluralRule
    {
        /// <summary>
        /// Evaluate whether the rule applies to <paramref name="number"/>.
        /// </summary>
        /// <param name="number">numeric and text representation of numberic value</param>
        /// <returns>true or false</returns>
        bool Evaluate(IPluralNumber number);
    }

}
