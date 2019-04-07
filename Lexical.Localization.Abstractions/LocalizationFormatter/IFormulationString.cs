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
    /// Result of parse
    /// </summary>
    public enum FormulationStringState
    {
        /// <summary>
        /// String is not parsed
        /// </summary>
        NotParsed,

        /// <summary>
        /// String is parsed ok
        /// </summary>
        Ok,

        /// <summary>
        /// Parse is not complete as string is malformed.
        /// </summary>
        Malformed
    }

    /// <summary>
    /// Preparsed formulation string.
    /// </summary>
    public interface IFormulationString
    {
        /// <summary>
        /// Type
        /// </summary>
        FormulationStringState Status { get; }

        /// <summary>
        /// Formulation string, for example "You received {cardinal:0} coin(s).".
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
        FormulationArgument this[int argumentIndex] { get; }
    }

    /// <summary>
    /// Formulation argument, e.g. "cardinal:0:X2"
    /// </summary>
    public class FormulationArgument
    {
        /// <summary>
        /// Argument index
        /// </summary>
        public readonly string ArgumentIndex;

        /// <summary>
        /// Pluralization category, e.g. "cardinal", "optional", "range", "ordinal".
        /// </summary>
        public readonly string PluralizationCategory;

        /// <summary>
        /// Function name
        /// </summary>
        public readonly string FunctionName;

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Length of the character segment that defines argument.
        /// </summary>
        public readonly int Length;
    }

}
