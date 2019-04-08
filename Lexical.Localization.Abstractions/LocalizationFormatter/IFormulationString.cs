// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Preparsed formulation string. 
    /// 
    /// For example "Welcome, {0}!" is a formulation string. 
    /// When it's in parsed format the argument "{0}" is extracted and the string can be processed more efficiently.
    /// 
    /// <see cref="ILocalizationFormulationString"/> is produced by <see cref="ILocalizationStringFormatParser"/>.
    /// </summary>
    public interface ILocalizationFormulationString
    {
        /// <summary>
        /// Parse result. One of:
        /// <list type="table">
        /// <item><see cref="LocalizationStatus.FormulationErrorMalformed"/> if there is a problem in the stirng</item>
        /// <item><see cref="LocalizationStatus.FormulationOk"/> if formulation was parsed ok.</item>
        /// </list>
        /// </summary>
        LocalizationStatus Status { get; }

        /// <summary>
        /// Formulation string as it appears, for example "You received {plural:0} coin(s).".
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Arguments order of occurance that are parsed from <see cref="Text"/> by an <see cref="ILocalizationStringFormatParser"/>.
        /// </summary>
        IFormulationArgument[] Arguments { get; }

        /// <summary>
        /// (optional) Formatters to apply to the formulation string.
        /// Some asset files may enforce their own rules.
        /// 
        /// The formatter is requested for following interfaces (Depends on <see cref="ILocalizationStringResolver"/> implementation.)
        /// <list type="bullet">
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityFunctionMap"/></item>
        /// <item><see cref="IPluralityFunction"/></item>
        /// </list>
        /// 
        /// </summary>
        IFormatProvider FormatterProvider { get; }
    }

    /// <summary>
    /// Formulation argument, e.g. "plural:0:X2"
    /// </summary>
    public interface IFormulationArgument
    {
        /// <summary>
        /// The whole argument definition as it appears in the formulation string.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Occurance index in formulation string.
        /// </summary>
        int OccuranceIndex { get; }

        /// <summary>
        /// If argument is index based, the index of the argument, otherwise -1.
        /// </summary>
        int ArgumentIndex { get; }

        /// <summary>
        /// Argument name as string, or null if not available.
        /// </summary>
        string ArgumentName { get; }

        /// <summary>
        /// Formatter name, e.g. "plural", "optional", "range", "ordinal".
        /// </summary>
        string FormatterName { get; }

        /// <summary>
        /// Default value, used if argument is not provided.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character segment that defines argument.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Format provider that can provide argument formatters.
        /// Is searched for types
        /// <list type="bullet">
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// </list>
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }


}
