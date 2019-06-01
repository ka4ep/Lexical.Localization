// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Internal;
using System;
using System.Globalization;
using System.Threading;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Null format string.
    /// </summary>
    public class NullString : IString
    {
        static IStringPart[] parts = new IStringPart[0];
        static IPlaceholder[] arguments = new IPlaceholder[0];
        /// <summary />
        public LineStatus Status => LineStatus.FormatFailedNull;
        /// <summary />
        public string Text => null;
        /// <summary />
        public IStringPart[] Parts => parts;
        /// <summary />
        public IPlaceholder[] Placeholders => arguments;
        /// <summary />
        public IFormatProvider FormatProvider => null;

        /// <summary>
        /// Cached hashcode
        /// </summary>
        int hashcode => FormatStringComparer.Default.GetHashCode(this);

        /// <summary>
        /// String format
        /// </summary>
        public IStringFormat StringFormat { get; protected set; }

        /// <summary>
        /// Calculate hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
                => hashcode;

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is IString other ? FormatStringComparer.Default.Equals(this, other) : false;

        /// <summary>
        /// Create null string
        /// </summary>
        /// <param name="stringFormat"></param>
        public NullString(IStringFormat stringFormat)
        {
            StringFormat = stringFormat ?? throw new ArgumentNullException(nameof(StringFormat));
        }
    }

    /// <summary>
    /// Empty format string.
    /// </summary>
    public class EmptyString : IString
    {
        static IStringPart[] parts = new IStringPart[0];
        static IPlaceholder[] arguments = new IPlaceholder[0];
        /// <summary />
        public LineStatus Status => LineStatus.FormatFailedNull;
        /// <summary />
        public string Text => "";
        /// <summary />
        public IStringPart[] Parts => parts;
        /// <summary />
        public IPlaceholder[] Placeholders => arguments;
        /// <summary />
        public IFormatProvider FormatProvider => null;

        /// <summary>
        /// String format
        /// </summary>
        public IStringFormat StringFormat { get; protected set; }

        /// <summary>
        /// Create empty string
        /// </summary>
        /// <param name="stringFormat"></param>
        public EmptyString(IStringFormat stringFormat)
        {
            StringFormat = stringFormat ?? throw new ArgumentNullException(nameof(stringFormat));
        }

        /// <summary>
        /// Cached hashcode
        /// </summary>
        int hashcode => FormatStringComparer.Default.GetHashCode(this);


        /// <summary>
        /// Calculate hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
                => hashcode;

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is IString other ? FormatStringComparer.Default.Equals(this, other) : false;
    }

    /// <summary>
    /// Generic placeholder implementation.
    /// </summary>
    public class Placeholder : IPlaceholder
    {
        /// <summary>
        /// The 'parent' format string.
        /// </summary>
        public IString String { get; internal set; }

        /// <summary>
        /// Part type
        /// </summary>
        public StringPartKind Kind => StringPartKind.Placeholder;

        /// <summary>
        /// The whole argument definition as it appears in the format string.
        /// </summary>
        public string Text => String.Text.Substring(Index, Length);

        /// <summary>
        /// Occurance index in <see cref="IString"/>.
        /// </summary>
        public int PlaceholderIndex { get; internal set; }

        /// <summary>
        /// Start index of first character of the argument in the format string.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Length of characters in the format string.
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        /// Plural category, such as: cardinal, ordinal, optional (with default ruies)
        /// </summary>
        public string PluralCategory { get; internal set; }

        /// <summary>
        /// Index in parts array.
        /// </summary>
        public int PartsIndex { get; internal set; }

        /// <summary>
        /// Expression that describes a function that evaluates to string within the evaluation context.
        /// </summary>
        public IExpression Expression { get; internal set; }

        /// <summary>
        /// Create argument info.
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="index">first character index</param>
        /// <param name="length">character length</param>
        /// <param name="partsIndex"></param>
        /// <param name="placeholderIndex"></param>
        /// <param name="pluralCategory"></param>
        /// <param name="expression">(optional)</param>
        public Placeholder(IString formatString, int index, int length, int partsIndex, int placeholderIndex, string pluralCategory, IExpression expression)
        {
            String = formatString ?? throw new ArgumentNullException(nameof(formatString));
            Index = index;
            Length = length;
            PluralCategory = pluralCategory;
            Expression = expression;
            PartsIndex = partsIndex;
            PlaceholderIndex = placeholderIndex;
        }

        /// <summary>
        /// Calculate hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => FormatStringPartComparer.Default.GetHashCode(this);

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => FormatStringPartComparer.Default.Equals(obj);

        /// <summary>
        /// Print argument format as it is in the format string. Example "{0:x2}".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => String.Text.Substring(Index, Length);
    }

    /// <summary>
    /// Generic text part implementation of <see cref="IString"/>.
    /// </summary>
    public class TextPart : IStringPart
    {
        /// <summary>
        /// Unify two text parts
        /// </summary>
        /// <param name="leftPart"></param>
        /// <param name="rightPart"></param>
        /// <returns></returns>
        internal static TextPart Unify(TextPart leftPart, TextPart rightPart)
            => new TextPart(leftPart.String, leftPart.Index, rightPart.Index - leftPart.Index + rightPart.Length);

        /// <summary>
        /// The 'parent' format string.
        /// </summary>
        public IString String { get; internal set; }

        /// <summary>
        /// Part type
        /// </summary>
        public StringPartKind Kind => StringPartKind.Text;

        /// <summary>
        /// Start index of first character of the argument in the format string.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Length of characters in the format string.
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        /// The text part as it appears in the format string.
        /// </summary>
        public string Text => String.Text.Substring(Index, Length);

        /// <summary>
        /// Index in Parts array.
        /// </summary>
        public int PartsIndex { get; internal set; }

        /// <summary>
        /// Create text part.
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="index">first character index</param>
        /// <param name="length">character length</param>
        public TextPart(IString formatString, int index, int length)
        {
            String = formatString;
            Index = index;
            Length = length;
        }

        /// <summary>
        /// Calculate hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => FormatStringPartComparer.Default.GetHashCode(this);

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => FormatStringPartComparer.Default.Equals(obj);

        /// <summary>
        /// The text part as it appears in the format string.
        /// </summary>
        public override string ToString() => String.Text.Substring(Index, Length);
    }


}
