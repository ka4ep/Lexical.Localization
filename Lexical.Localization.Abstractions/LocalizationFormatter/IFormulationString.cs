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
        /// Formulation string as sequence of text and argument parts.
        /// </summary>
        ILocalizationFormulationStringPart[] Parts { get; }

        /// <summary>
        /// Arguments ordered by argument index first, then occurance index.
        /// 
        /// If there are gaps in argument index, gaps are not included.
        /// E.g. "Value={0}, Description={5}" returns argument {0} first then {5}.
        /// 
        /// If same argument occurs twice, then ordered by occurance index.
        /// E.g. "Value={0,10}, x={0:X8}" returns first {0,10} then second {0:X8}.
        /// 
        /// They are parsed from <see cref="Text"/> by an <see cref="ILocalizationStringFormatParser"/>.
        /// </summary>
        ILocalizationFormulationStringArgument[] Arguments { get; }

        /// <summary>
        /// (optional) Formatters to apply to the formulation string.
        /// Some asset files may enforce their own rules.
        /// 
        /// The formatter is requested for following interfaces (Depends on <see cref="ILocalizationResolver"/> implementation.)
        /// <list type="bullet">
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityRuleMap"/></item>
        /// <item><see cref="IPluralityCategory"/></item>
        /// </list>
        /// 
        /// <see cref="ILocalizationResolver"/> combines format providers from asset and key.
        /// The format provider that comes from <see cref="ILocalizationFormulationString"/> has the highest priority.
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }

    /// <summary>
    /// Type of string part
    /// </summary>
    public enum LocalizationFormulationStringPartKind
    {
        /// <summary>
        /// Text
        /// </summary>
        Text,

        /// <summary>
        /// Argument
        /// </summary>
        Argument
    }

    /// <summary>
    /// A part in formulation string.
    /// </summary>
    public interface ILocalizationFormulationStringPart
    {
        /// <summary>
        /// The 'parent' formulation string.
        /// </summary>
        ILocalizationFormulationString FormulationString { get; }

        /// <summary>
        /// Part type.
        /// </summary>
        LocalizationFormulationStringPartKind Kind { get; }

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character segment that defines argument.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The whole part as it appears in the formulation string.
        /// </summary>
        /// <returns></returns>
        string Text { get; }

        /// <summary>
        /// The index in the <see cref="ILocalizationFormulationString.Parts"/>.
        /// </summary>
        int PartsIndex { get; }
    }

    /// <summary>
    /// Formulation of an argument e.g. "{[function:]0[,alignment][:format]}"
    /// </summary>
    public interface ILocalizationFormulationStringArgument : ILocalizationFormulationStringPart
    {
        /// <summary>
        /// Occurance index in formulation string.
        /// </summary>
        int OccuranceIndex { get; }

        /// <summary>
        /// If format is argument based, then this is the argument index, otherwise -1.
        /// Also index in <see cref="ILocalizationKeyFormatArgs.Args"/>.
        /// </summary>
        int ArgumentIndex { get; }

        /// <summary>
        /// Argument reference by index. -1 if arguments are not refered by index.
        /// The index correspondes to index in <see cref="ILocalizationFormulationString.Arguments"/>.
        /// </summary>
        int ArgumentsIndex { get; }

        /// <summary>
        /// Argument reference by name, or null.
        /// </summary>
        string ArgumentName { get; }

        /// <summary>
        /// (Optional) The function name that is passed to <see cref="ILocalizationArgumentFormatter"/>.
        /// E.g. "plural", "optional", "range", "ordinal".
        /// </summary>
        string Function { get; }

        /// <summary>
        /// Alignment is an integer that defines field width. If negative then field is left-aligned, if positive then right-aligned.
        /// </summary>
        int Alignment { get; }

        /// <summary>
        /// (Optional) The format text that is passed to <see cref="ICustomFormatter"/>, <see cref="IFormattable"/> and <see cref="ILocalizationArgumentFormatter"/>.
        /// E.g. "x2".
        /// </summary>
        string Format { get; }

        /// <summary>
        /// (Optional) Default value, used if argument is not provided.
        /// </summary>
        string DefaultValue { get; }
    }

    /// <summary/>
    public static partial class LocalizationFormulationStringExtensions
    {
    }

}
