// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Internal;
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Preparsed or lazy parsed format string. 
    /// 
    /// For example "Welcome, {0}!" is a format string. 
    /// When it's in parsed format the argument "{0}" is extracted and the string can be processed more efficiently.
    /// 
    /// <see cref="IString"/> is produced by <see cref="IStringFormatParser"/>.
    /// </summary>
    public interface IString
    {
        /// <summary>
        /// (optional) String format.
        /// </summary>
        IStringFormat StringFormat { get; }

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
        IStringPart[] Parts { get; }

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
    public enum StringPartKind
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
    public interface IStringPart
    {
        /// <summary>
        /// The 'parent' format string.
        /// </summary>
        IString String { get; }

        /// <summary>
        /// Part type.
        /// </summary>
        StringPartKind Kind { get; }

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
        /// The index in the <see cref="IString.Parts"/>.
        /// </summary>
        int PartsIndex { get; }
    }

    /// <summary>
    /// Text part of a string
    /// </summary>
    public interface IStringTextPart : IStringPart
    {
    }

    /// <summary>
    /// Argument placeholder in an <see cref="IString"/> in a format string.
    /// 
    /// For example "Hello, {0}", the {0} is placeholder.
    /// 
    /// The default CSharpFormat uses format "{[function:]0[,alignment][:format]}"
    /// </summary>
    public interface IPlaceholder : IStringPart
    {
        /// <summary>
        /// Index in <see cref="IString.Placeholders"/>
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


        /// <summary>
        /// Tests if placeholders have pluralrules category
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns>true if has one</returns>
        public static bool HasPluralRules(this IString formatString)
        {
            foreach (IPlaceholder ph in formatString.Placeholders)
                if (ph.PluralCategory != null) return true;
            return false;
        }

        /// <summary>
        /// Get all the argument indices.
        /// </summary>
        /// <param name="ph"></param>
        public static int[] GetArgumentIndices(this IPlaceholder ph)
        {
            StructList8<int> argumentIndices = new StructList8<int>();
            StructList8<IExpression> stack = new StructList8<IExpression>();
            stack.Add(ph.Expression);
            while (stack.Count > 0)
            {
                IExpression e = stack.Dequeue();
                if (e is IArgumentIndexExpression arg) argumentIndices.AddIfNew(arg.Index);
                if (e is ICompositeExpression compositeExpression)
                    for (int i = 0; i < compositeExpression.ComponentCount; i++)
                    {
                        IExpression ce = compositeExpression.GetComponent(i);
                        if (ce != null) stack.Add(ce);
                    }
            }
            return argumentIndices.ToArray();
        }


        /// <summary>
        /// Get all the argument indices.
        /// </summary>
        /// <param name="str"></param>
        public static int[] GetArgumentIndices(this IString str)
        {
            StructList8<int> argumentIndices = new StructList8<int>();
            StructList12<IExpression> stack = new StructList12<IExpression>();
            for(int i=str.Placeholders.Length-1; i>=0; i--) stack.Add(str.Placeholders[i].Expression);
            while (stack.Count > 0)
            {
                IExpression e = stack.Dequeue();
                if (e is IArgumentIndexExpression arg) argumentIndices.AddIfNew(arg.Index);
                if (e is ICompositeExpression compositeExpression)
                    for (int i = 0; i < compositeExpression.ComponentCount; i++)
                    {
                        IExpression ce = compositeExpression.GetComponent(i);
                        if (ce != null) stack.Add(ce);
                    }
            }
            return argumentIndices.ToArray();
        }

    }

}
