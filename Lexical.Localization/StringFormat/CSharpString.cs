// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Exp;
using Lexical.Localization.Internal;
using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Lexical.Localization.StringFormat
{
    public partial class CSharpFormat
    {
        /// <summary>
        /// Lazily parsed format string that uses C# String.Format notation.
        /// </summary>
        public class CSharpString : IString
        {
            /// <summary>
            /// Parsed arguments. Set in <see cref="Build"/>.
            /// </summary>
            IPlaceholder[] placeholders;

            /// <summary>
            /// String as sequence of parts. Set in <see cref="Build"/>.
            /// </summary>
            IStringPart[] parts;

            /// <summary>
            /// Parse status. Set in <see cref="Build"/>.
            /// </summary>
            LineStatus status;

            /// <summary>
            /// Get the original format string.
            /// </summary>
            public string Text { get; internal set; }

            /// <summary>
            /// Get parse status.
            /// </summary>
            public LineStatus Status { get { if (status == LineStatus.FormatFailedNoResult) Build(); return status; } }

            /// <summary>
            /// Format string broken into sequence of text and argument parts.
            /// </summary>
            public IStringPart[] Parts { get { if (status == LineStatus.FormatFailedNoResult) Build(); return parts; } }

            /// <summary>
            /// Get placeholders.
            /// </summary>
            public IPlaceholder[] Placeholders { get { if (status == LineStatus.FormatFailedNoResult) Build(); return placeholders; } }

            /// <summary>
            /// (optional) Get associated format provider. This is typically a plurality rules and  originates from a localization file.
            /// </summary>
            public virtual IFormatProvider FormatProvider => null;

            /// <summary>
            /// Associated string format instance
            /// </summary>
            public IStringFormat StringFormat { get; protected set; }

            /// <summary>
            /// Create format string that parses formulation <paramref name="text"/> lazily.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="stringFormat"></param>
            public CSharpString(string text, IStringFormat stringFormat)
            {
                Text = text;
                status = text == null ? LineStatus.FormatFailedNull : LineStatus.FormatFailedNoResult;
                this.StringFormat = stringFormat;
            }

            /// <summary>
            /// Parse string
            /// </summary>
            protected void Build()
            {
                StructList8<IStringPart> parts = new StructList8<IStringPart>();

                // Create parser
                CSharpFormat.CSharpParser parser = new CSharpFormat.CSharpParser(this);

                // Read parts
                while (parser.HasMore)
                {
                    IStringPart part = parser.ReadNext();
                    if (part != null) parts.Add(part);
                }

                // Unify text parts
                for (int i = 1; i < parts.Count;)
                {
                    if (parts[i - 1] is TextPart left && parts[i] is TextPart right)
                    {
                        parts[i - 1] = TextPart.Unify(left, right);
                        parts.RemoveAt(i);
                    }
                    else i++;
                }

                // Create parts array
                var partArray = new IStringPart[parts.Count];
                parts.CopyTo(partArray, 0);
                // Set PartsArrayIndex
                for (int i = 0; i < partArray.Length; i++)
                {
                    if (partArray[i] is TextPart textPart) textPart.PartsIndex = i;
                    else if (partArray[i] is Placeholder argPart) argPart.PartsIndex = i;
                }

                // Create arguments array
                int argumentCount = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is IPlaceholder) argumentCount++;
                var placeholdersArray = new IPlaceholder[argumentCount];
                int j = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is Placeholder argPart) placeholdersArray[j++] = argPart;
                Array.Sort(placeholdersArray, FormatStringPartComparer.Default);
                for (int i = 0; i < placeholdersArray.Length; i++) ((Placeholder)placeholdersArray[i]).PlaceholderIndex = i;

                // Write status.
                Thread.MemoryBarrier();
                this.placeholders = placeholdersArray;
                this.parts = partArray;
                this.status = parser.status;
            }


            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
                => FormatStringComparer.Default.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => obj is IString other ? FormatStringComparer.Default.Equals(this, other) : false;

            /// <summary>
            /// Format string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => Text;

            /// <summary>
            /// Generic text part implementation of <see cref="IString"/>.
            /// </summary>
            public class TextPart : IStringTextPart
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
                /// Unescaped text
                /// </summary>
                protected string unescapedText;

                /// <summary>
                /// Unescaped text
                /// </summary>
                public string UnescapedText => unescapedText ?? (unescapedText = Unescape());

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
                /// Unescape text sequence
                /// </summary>
                /// <returns></returns>
                protected string Unescape()
                {
                    StringBuilder sb = new StringBuilder(Length);
                    String s = String.Text;
                    int end = Index + Length;
                    for (int i = Index; i < end; i++)
                    {
                        char c = s[i];
                        if (c == '\\' && i < end - 1)
                        {
                            char n = s[i + 1];
                            if (n == '\\' || n == '{' || n == '}') continue;
                        }
                        sb.Append(c);
                    }
                    return sb.ToString();
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

            /// <summary>
            /// Version of format string that carries an associated format provider.
            /// </summary>
            public class WithFormatProvider : CSharpString
            {
                /// <summary>
                /// (optional) Associated format provider.
                /// </summary>
                IFormatProvider formatProvider;

                /// <summary>
                /// (optional) Get associated format provider. This is typically a plurality rules and  originates from a localization file.
                /// </summary>
                public override IFormatProvider FormatProvider => formatProvider;

                /// <summary>
                /// Create lazily parsed format string that carries a <paramref name="formatProvider"/>.
                /// </summary>
                /// <param name="text"></param>
                /// <param name="stringFormat"></param>
                /// <param name="formatProvider"></param>
                public WithFormatProvider(string text, IStringFormat stringFormat, IFormatProvider formatProvider) : base(text, stringFormat)
                {
                    this.formatProvider = formatProvider;
                }

            }
        }
    }

}
