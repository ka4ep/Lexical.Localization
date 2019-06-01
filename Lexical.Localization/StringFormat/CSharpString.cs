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
    }

    /// <summary>
    /// Version of format string that carries an associated format provider.
    /// </summary>
    public class CSharpStringWithFormatProvider : CSharpString
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
        public CSharpStringWithFormatProvider(string text, IStringFormat stringFormat, IFormatProvider formatProvider) : base(text, stringFormat)
        {
            this.formatProvider = formatProvider;
        }
    }

}
