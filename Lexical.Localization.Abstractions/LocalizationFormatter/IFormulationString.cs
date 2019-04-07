// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Preparsed formulation string.
    /// </summary>
    public interface IFormulationString
    {
        /// <summary>
        /// One of 
        /// <list type="table">
        /// <item><see cref="LocalizationStatus.FormulationErrorMalformed"/> if there is a problem in the stirng</item>
        /// <item><see cref="LocalizationStatus.FormulationOk"/> if formulation was parsed ok.</item>
        /// </list>
        /// </summary>
        LocalizationStatus Status { get; }

        /// <summary>
        /// Formulation string, for example "You received {plural:0} coin(s).".
        /// The notation depends on <see cref="ILocalizationArgumentFormatter"/>.
        /// </summary>
        string FormulationString { get; }

        /// <summary>
        /// Number of arguments
        /// </summary>
        int ArgumentCount { get; }

        /// <summary>
        /// (optional) Explicit plurality rules that should be applied to this formulation string.
        /// 
        /// Plurality rules are typically acquired from culture, but some asset files may enforce their own explicit plurality rules.
        /// </summary>
        IPluralityRuleSet PluralityRules { get; }

        /// <summary>
        /// Argument formatter that extracted the argments from formulation string.
        /// </summary>
        ILocalizationArgumentFormatter ArgumentFormatter { get; }

        /// <summary>
        /// Get argument info
        /// </summary>
        /// <param name="argumentIndex"></param>
        /// <returns></returns>
        IFormulationArgument this[int argumentIndex] { get; }
    }

    /// <summary>
    /// Formulation argument, e.g. "plural:0:X2"
    /// </summary>
    public interface IFormulationArgument
    {
        /// <summary>
        /// Argument index
        /// </summary>
        string ArgumentIndex { get; }

        /// <summary>
        /// Pluralization category, e.g. "plural", "optional", "range", "ordinal".
        /// </summary>
        string PluralizationCategory { get; }

        /// <summary>
        /// Function name
        /// </summary>
        string FunctionName { get; }

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character segment that defines argument.
        /// </summary>
        int Length { get; }
    }

}
