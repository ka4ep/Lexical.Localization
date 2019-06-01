// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           1.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// String format that uses no placeholders.
    /// </summary>
    public class NoFormat : IStringFormatParser, IStringFormatPrinter
    {
        private static IStringFormatParser instance => new CSharpFormat();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IStringFormatParser Default => instance;

        /// <summary>
        /// Name of this string format.
        /// </summary>
        public string Name => "string";

        IString _null, _empty;

        /// <summary>
        /// Create plain string format.
        /// </summary>
        public NoFormat()
        {
            _null = new NullString(this);
            _empty = new EmptyString(this);
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public string Print(IString formatString)
        {
            return formatString.Text;
        }

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public IString Parse(string formatString)
        {
            return new TextString(formatString, this);
        }

        /// <summary>
        /// Unformulated text. Contains no placeholders or escaping.
        /// </summary>
        public class TextString : IString, IStringTextPart
        {
            /// <summary>
            /// Placeholders
            /// </summary>
            static IPlaceholder[] placeholders = new IPlaceholder[0];

            /// <summary>
            /// Parts
            /// </summary>
            IStringPart[] parts;

            /// <summary>
            /// Get the original format string.
            /// </summary>
            public string Text { get; internal set; }

            /// <summary>
            /// Get parse status.
            /// </summary>
            public LineStatus Status => LineStatus.FormatOkString;

            /// <summary>
            /// Format string broken into sequence of text and argument parts.
            /// </summary>
            public IStringPart[] Parts => parts ?? (parts = new IStringPart[] { this });

            /// <summary>
            /// Get placeholders.
            /// </summary>
            public IPlaceholder[] Placeholders => placeholders;

            /// <summary>
            /// (optional) Get associated format provider. This is typically a plurality rules and  originates from a localization file.
            /// </summary>
            public virtual IFormatProvider FormatProvider => null;

            /// <summary>
            /// Parent object
            /// </summary>
            public IStringFormat StringFormat { get; protected set; }

            /// <summary></summary>
            IString IStringPart.String => this;

            /// <summary></summary>
            public StringPartKind Kind => StringPartKind.Text;

            /// <summary></summary>
            public int Index => 0;

            /// <summary></summary>
            public int Length => Text.Length;

            /// <summary></summary>
            public int PartsIndex => 0;

            /// <summary>
            /// Create format string that parses formulation <paramref name="text"/> lazily.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="stringFormat"></param>
            public TextString(string text, IStringFormat stringFormat)
            {
                Text = text ?? throw new ArgumentNullException(nameof(text));
                this.StringFormat = stringFormat;
            }
        }

    }
}
