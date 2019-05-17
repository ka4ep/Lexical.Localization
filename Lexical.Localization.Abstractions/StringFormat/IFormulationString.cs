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
    /// Preparsed or lazy parsed format string. 
    /// 
    /// For example "Welcome, {0}!" is a format string. 
    /// When it's in parsed format the argument "{0}" is extracted and the string can be processed more efficiently.
    /// 
    /// <see cref="IFormatString"/> is produced by <see cref="IStringFormatParser"/>.
    /// </summary>
    public interface IFormatString
    {
        /// <summary>
        /// Parse result. One of:
        /// <list type="table">
        /// <item><see cref="LineStatus.FormatErrorMalformed"/> if there is a problem in the stirng</item>
        /// <item><see cref="LineStatus.FormatOk"/> if format was parsed ok.</item>
        /// </list>
        /// </summary>
        LineStatus Status { get; }

        /// <summary>
        /// Format string as it appears, for example "You received {plural:0} coin(s).".
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Format string as sequence of text and argument parts.
        /// </summary>
        IFormatStringPart[] Parts { get; }

        /// <summary>
        /// Placeholders in order of occurance.
        /// </summary>
        IPlaceholder[] Placeholders { get; }

        /// <summary>
        /// (optional) Formatters to apply to the format string.
        /// Some asset files may enforce their own rules.
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }

    /// <summary>
    /// Type of string part
    /// </summary>
    public enum FormatStringPartKind
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
    /// A part in format string.
    /// </summary>
    public interface IFormatStringPart
    {
        /// <summary>
        /// The 'parent' format string.
        /// </summary>
        IFormatString FormatString { get; }

        /// <summary>
        /// Part type.
        /// </summary>
        FormatStringPartKind Kind { get; }

        /// <summary>
        /// Character index in the format string where argument starts.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Length of the character sequence that defines part.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The whole part as it appears in the format string.
        /// </summary>
        /// <returns></returns>
        string Text { get; }

        /// <summary>
        /// The index in the <see cref="IFormatString.Parts"/>.
        /// </summary>
        int PartsIndex { get; }
    }

    /// <summary>
    /// Argument placeholder in an <see cref="IFormatString"/> in a format string.
    /// 
    /// For example "Hello, {0}", the {0} is placeholder.
    /// 
    /// The default CSharpFormat uses format "{[function:]0[,alignment][:format]}"
    /// </summary>
    public interface IPlaceholder : IFormatStringPart
    {
        /// <summary>
        /// Index in <see cref="IFormatString.Placeholders"/>
        /// </summary>
        int PlaceholderIndex { get; }

        /// <summary>
        /// Expression that evaluates to a string.
        /// </summary>
        IExpression Expression { get; }

        /// <summary>
        /// Plural category, such as: cardinal, ordinal, optional (with default ruies)
        /// </summary>
        String PluralCategory { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class IFormatStringExtensions
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
