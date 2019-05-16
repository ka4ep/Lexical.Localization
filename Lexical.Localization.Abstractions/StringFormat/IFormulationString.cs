// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Preparsed formulation string. 
    /// 
    /// For example "Welcome, {0}!" is a formulation string. 
    /// When it's in parsed format the argument "{0}" is extracted and the string can be processed more efficiently.
    /// 
    /// <see cref="IFormulationString"/> is produced by <see cref="IStringFormatParser"/>.
    /// </summary>
    public interface IFormulationString
    {
        /// <summary>
        /// Parse result. One of:
        /// <list type="table">
        /// <item><see cref="LineStatus.FormulationErrorMalformed"/> if there is a problem in the stirng</item>
        /// <item><see cref="LineStatus.FormulationOk"/> if formulation was parsed ok.</item>
        /// </list>
        /// </summary>
        LineStatus Status { get; }

        /// <summary>
        /// Formulation string as it appears, for example "You received {plural:0} coin(s).".
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Formulation string as sequence of text and argument parts.
        /// </summary>
        IFormulationStringPart[] Parts { get; }

        /// <summary>
        /// Placeholders in order of occurance.
        /// </summary>
        IPlaceholder[] Placeholders { get; }

        /// <summary>
        /// (optional) Formatters to apply to the formulation string.
        /// Some asset files may enforce their own rules.
        /// 
        /// The formatter is requested for following interfaces (Depends on <see cref="IStringResolver"/> implementation.)
        /// <list type="bullet">
        /// <item><see cref="IArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// </list>
        /// 
        /// <see cref="IStringResolver"/> combines format providers from asset and key.
        /// The format provider that comes from <see cref="IFormulationString"/> has the highest priority.
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }

    /// <summary>
    /// Type of string part
    /// </summary>
    public enum FormulationStringPartKind
    {
        /// <summary>
        /// Text
        /// </summary>
        Text,

        /// <summary>
        /// Argument placeholder
        /// </summary>
        Placeholder
    }

    /// <summary>
    /// A part in formulation string.
    /// </summary>
    public interface IFormulationStringPart
    {
        /// <summary>
        /// The 'parent' formulation string.
        /// </summary>
        IFormulationString FormulationString { get; }

        /// <summary>
        /// Part type.
        /// </summary>
        FormulationStringPartKind Kind { get; }

        /// <summary>
        /// Character index in the formulation string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character sequence that defines part.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The whole part as it appears in the formulation string.
        /// </summary>
        /// <returns></returns>
        string Text { get; }

        /// <summary>
        /// The index in the <see cref="IFormulationString.Parts"/>.
        /// </summary>
        int PartsIndex { get; }
    }

    /// <summary>
    /// Argument placeholder in an <see cref="IFormulationString"/> in a formulation string.
    /// 
    /// For example "Hello, {0}", the {0} is placeholder.
    /// 
    /// The default CSharpFormat uses format "{[function:]0[,alignment][:format]}"
    /// </summary>
    public interface IPlaceholder : IFormulationStringPart
    {
        /// <summary>
        /// Index in <see cref="IFormulationString.Placeholders"/>
        /// </summary>
        int PlaceholderIndex { get; }

        /// <summary>
        /// Expression that evaluates to a string.
        /// </summary>
        IExpression Expression { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class IFormulationStringExtensions
    {
        /// <summary>
        /// Get default value (as " ?? value" expression at the end) , if has one.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <returns>Default value or null</returns>
        public static object DefaultValue(this IPlaceholder placeholder)
            => placeholder.Expression is IBinaryOpExpression bop && bop.Op == BinaryOp.Coalesce ? bop.Right is IConstantExpression ce ? ce.Value : null : null;

    }

}
