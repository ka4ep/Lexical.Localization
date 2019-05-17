// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Lexical string format similiar to C# string format, with one difference, function descriptions can be added to the argument description.
    /// 
    ///     "Text {[function:]0:[,alignment][:format]} text".
    /// 
    /// Rules:
    ///  1. Arguments are numbered and inside braces:
    ///     "Hello {0} and {1}"
    ///  
    ///  2. Braces can be escaped:
    ///     "Control characters are \\{\\}."
    /// 
    ///  3. On the right-side of argument are format specifiers:
    ///     "Hex value {0:X4}."
    /// 
    ///  4. On the left-side of argument a function can be placed. Function cannot start with a number, cannot contain unescaped colon ':'.
    ///     "There are {optional:0} cats."
    /// 
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=netframework-4.7.2"/>
    /// </summary>
    public class CSharpFormat : IStringFormatParser, IStringFormatPrinter
    {
        private static IStringFormatParser instance => new CSharpFormat();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IStringFormatParser Instance => instance;

        /// <summary>
        /// Name of this string format.
        /// </summary>
        public string Name => "csharp";

        /// <summary>
        /// Format provider to add to each parsed line.
        /// </summary>
        public virtual IFormatProvider FormatProvider { get; protected set; }

        /// <summary>
        /// Create default format.
        /// </summary>
        public CSharpFormat()
        {
        }

        /// <summary>
        /// Create format with default <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="formatProvider">(optional) provider to add to each parsed line.</param>
        public CSharpFormat(IFormatProvider formatProvider)
        {
            this.FormatProvider = formatProvider;
        }

        /// <summary>
        /// Create lazily parsed string <paramref name="formatString"/>.
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns>preparsed</returns>
        public IFormatString Parse(string formatString)
        {
            if (formatString == null) return Null.Instance;
            if (formatString == "") return Empty.Instance;
            if (FormatProvider != null) return new FormatStringWithFormatProvider(formatString, FormatProvider);
            return new FormatString(formatString);
        }

        /// <summary>
        /// Print as string
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns>string or null</returns>
        public string Print(IFormatString formatString)
        {
            return formatString?.Text;
        }

        /// <summary>
        /// Lazily parsed format string.
        /// </summary>
        public class FormatString : IFormatString
        {
            /// <summary>
            /// Parsed arguments. Set in <see cref="Build"/>.
            /// </summary>
            IPlaceholder[] arguments;

            /// <summary>
            /// String as sequence of parts. Set in <see cref="Build"/>.
            /// </summary>
            IFormatStringPart[] parts;

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
            public IFormatStringPart[] Parts { get { if (status == LineStatus.FormatFailedNoResult) Build(); return parts; } }

            /// <summary>
            /// Get the parsed arguments.
            /// </summary>
            public IPlaceholder[] Placeholders { get { if (status == LineStatus.FormatFailedNoResult) Build(); return arguments; } }

            /// <summary>
            /// (optional) Get associated format provider. This is typically a plurality rules and  originates from a localization file.
            /// </summary>
            public virtual IFormatProvider FormatProvider => null;

            /// <summary>
            /// Create format string that parses arguments lazily.
            /// </summary>
            /// <param name="text"></param>
            public FormatString(string text)
            {
                Text = text;
                status = text == null ? LineStatus.FormatFailedNull : LineStatus.FormatFailedNoResult;
            }

            /// <summary>
            /// Parse string
            /// </summary>
            protected void Build()
            {
                StructList8<IFormatStringPart> parts = new StructList8<IFormatStringPart>();

                // Create parser
                Parser parser = new Parser(this);

                // Read parts
                while (parser.HasMore)
                {
                    IFormatStringPart part = parser.ReadNext();
                    if (part != null) parts.Add(part);
                }

                // Unify text parts
                for (int i=1; i<parts.Count;)
                {
                    if (parts[i - 1] is TextPart left && parts[i] is TextPart right)
                    {
                        parts[i - 1] = TextPart.Unify(left, right);
                        parts.RemoveAt(i);
                    }
                    else i++;
                }

                // Create parts array
                var partArray = new IFormatStringPart[parts.Count];
                parts.CopyTo(partArray, 0);
                // Set PartsArrayIndex
                for (int i = 0; i < partArray.Length; i++)
                {
                    if (partArray[i] is TextPart textPart) textPart.PartsIndex = i;
                    else if (partArray[i] is PlaceHolder argPart) argPart.PartsIndex = i;
                }

                // Create arguments array
                int argumentCount = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is IPlaceholder) argumentCount++;
                var argumentsArray = new IPlaceholder[argumentCount];
                int j = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is PlaceHolder argPart) argumentsArray[j++] = argPart;
                Array.Sort(argumentsArray, PlaceholderOrderComparer.Instance);
                for (int i = 0; i < argumentsArray.Length; i++) ((PlaceHolder)argumentsArray[i]).ArgumentsIndex = i;

                // Write status.
                Thread.MemoryBarrier();
                this.arguments = argumentsArray;
                this.parts = partArray;
                this.status = parser.status;
            }

            /// <summary>
            /// Comparer that compares first by placeholder index, then by occurance index.
            /// </summary>
            class PlaceholderOrderComparer : IComparer<IPlaceholder>
            {
                static IComparer<IPlaceholder> instance = new PlaceholderOrderComparer();
                public static IComparer<IPlaceholder> Instance => instance;

                public int Compare(IPlaceholder x, IPlaceholder y)
                {
                    int c = x.ArgumentIndex - y.ArgumentIndex;
                    if (c < 0) return -1;
                    if (c > 0) return 1;
                    c = x.PlaceholderIndex - y.PlaceholderIndex;
                    if (c < 0) return -1;
                    if (c > 0) return 1;
                    return c;
                }
            }

            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() 
                => FormatStringComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => obj is IFormatString other ? FormatStringComparer.Instance.Equals(this, other) : false;

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
        public class FormatStringWithFormatProvider : FormatString
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
            /// <param name="formatProvider"></param>
            public FormatStringWithFormatProvider(string text, IFormatProvider formatProvider) : base(text)
            {
                this.formatProvider = formatProvider;
            }
        }

        /// <summary>
        /// Null format string.
        /// </summary>
        public class Null : IFormatString
        {
            static IFormatString instance => new Null();
            static IFormatStringPart[] parts = new IFormatStringPart[0];
            static IPlaceholder[] arguments = new IPlaceholder[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static IFormatString Instance => instance;
            /// <summary />
            public LineStatus Status => LineStatus.FormatFailedNull;
            /// <summary />
            public string Text => null;
            /// <summary />
            public IFormatStringPart[] Parts => parts;
            /// <summary />
            public IPlaceholder[] Placeholders => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;

            /// <summary>
            /// Cached hashcode
            /// </summary>
            int hashcode => FormatStringComparer.Instance.GetHashCode(this);

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
                => obj is IFormatString other ? FormatStringComparer.Instance.Equals(this, other) : false;

        }

        /// <summary>
        /// Empty format string.
        /// </summary>
        public class Empty : IFormatString
        {
            static IFormatString instance => new Empty();
            static IFormatStringPart[] parts = new IFormatStringPart[0];
            static IPlaceholder[] arguments = new IPlaceholder[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static IFormatString Instance => instance;
            /// <summary />
            public LineStatus Status => LineStatus.FormatFailedNull;
            /// <summary />
            public string Text => "";
            /// <summary />
            public IFormatStringPart[] Parts => parts;
            /// <summary />
            public IPlaceholder[] Placeholders => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;

            /// <summary>
            /// Cached hashcode
            /// </summary>
            int hashcode => FormatStringComparer.Instance.GetHashCode(this);

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
                => obj is IFormatString other ? FormatStringComparer.Instance.Equals(this, other) : false;
        }

        /// <summary>
        /// Text part
        /// </summary>
        public class TextPart : IFormatStringPart
        {
            /// <summary>
            /// Unify two parts
            /// </summary>
            /// <param name="leftPart"></param>
            /// <param name="rightPart"></param>
            /// <returns></returns>
            internal static TextPart Unify(TextPart leftPart, TextPart rightPart)
                => new TextPart(leftPart.FormatString, leftPart.Index, rightPart.Index - leftPart.Index + rightPart.Length);

            /// <summary>
            /// The 'parent' format string.
            /// </summary>
            public IFormatString FormatString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public FormatStringPartKind Kind => FormatStringPartKind.Text;

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
            public string Text => FormatString.Text.Substring(Index, Length);

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
            public TextPart(IFormatString formatString, int index, int length)
            {
                FormatString = formatString;
                Index = index;
                Length = length;
            }

            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() 
                => FormatStringPartComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => FormatStringPartComparer.Instance.Equals(obj);

            /// <summary>
            /// The text part as it appears in the format string.
            /// </summary>
            public override string ToString() => FormatString.Text.Substring(Index, Length);
        }

        /// <summary>
        /// Parsed argument info.
        /// </summary>
        public class PlaceHolder : IPlaceholder
        {
            /// <summary>
            /// The 'parent' format string.
            /// </summary>
            public IFormatString FormatString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public FormatStringPartKind Kind => FormatStringPartKind.Placeholder;

            /// <summary>
            /// The whole argument definition as it appears in the format string.
            /// </summary>
            public string Text => FormatString.Text.Substring(Index, Length);

            /// <summary>
            /// Occurance index in <see cref="FormatString"/>.
            /// </summary>
            public int PlaceholderIndex { get; internal set; }

            /// <summary>
            /// Argument index. Refers to index in the args array of <see cref="ILineFormatArgs.Args"/>.
            /// </summary>
            public int ArgumentIndex { get; internal set; }

            /// <summary>
            /// Argument name.
            /// </summary>
            public string ArgumentName => null;

            /// <summary>
            /// Start index of first character of the argument in the format string.
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            /// Length of characters in the format string.
            /// </summary>
            public int Length { get; internal set; }

            /// <summary>
            /// (Optional) The function name that is passed to <see cref="IArgumentFormatter"/>.
            /// E.g. "plural", "optional", "range", "ordinal".
            /// </summary>
            public string Function { get; internal set; }

            /// <summary>
            /// (Optional) The format text that is passed to <see cref="ICustomFormatter"/>, <see cref="IFormattable"/> and <see cref="IArgumentFormatter"/>.
            /// E.g. "x2".
            /// </summary>
            public string Format { get; internal set; }

            /// <summary>
            /// Alignment is an integer that defines field width. If negative then field is left-aligned, if positive then right-aligned.
            /// </summary>
            public int Alignment { get; internal set; }

            /// <summary>
            /// Get default value
            /// </summary>
            public string DefaultValue => null;

            /// <summary>
            /// Index in Arguments array.
            /// </summary>
            public int ArgumentsIndex { get; internal set; }

            /// <summary>
            /// Index in parts array.
            /// </summary>
            public int PartsIndex { get; internal set; }

            /// <summary>
            /// Create argument info.
            /// </summary>
            /// <param name="formatString"></param>
            /// <param name="index">first character index</param>
            /// <param name="length">character length</param>
            /// <param name="occuranceIndex"></param>
            /// <param name="argumentIndex"></param>
            /// <param name="function">(optional)</param>
            /// <param name="format">(optional)</param>
            /// <param name="alignment"></param>
            public PlaceHolder(IFormatString formatString, int index, int length, int occuranceIndex, int argumentIndex, string function, string format, int alignment)
            {
                FormatString = formatString ?? throw new ArgumentNullException(nameof(formatString));
                Index = index;
                Length = length;
                Function = function;
                Format = format;
                PlaceholderIndex = occuranceIndex;
                ArgumentIndex = argumentIndex;
                Alignment = alignment;
            }

            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
                => FormatStringPartComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => FormatStringPartComparer.Instance.Equals(obj);

            /// <summary>
            /// Print argument format as it is in the format string. Example "{0:x2}".
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => FormatString.Text.Substring(Index, Length);
        }

        enum ParserState { Text, ArgumentStart, Function, Index, Alignment, Format, ArgumentEnd }

        /// <summary>
        /// Parser that breaks format string into parts
        /// </summary>
        struct Parser
        {
            /// <summary>
            /// Format string
            /// </summary>
            String str;

            /// <summary>
            /// Reader state
            /// </summary>
            ParserState state;

            /// <summary>
            /// Is previous character escape
            /// </summary>
            bool escaped;

            /// <summary>
            /// Argument occurance index
            /// </summary>
            int occuranceIx;

            /// <summary>
            /// Part's start index
            /// </summary>
            int partIx;

            /// <summary>
            /// Function text indices
            /// </summary>
            int functionStartIx, functionEndIx;

            /// <summary>
            /// Argument index indices
            /// </summary>
            int indexStartIx, indexEndIx;

            /// <summary>
            /// Alignment indices
            /// </summary>
            int alignmentStartIx, alignmentEndIx;

            /// <summary>
            /// Format text indices
            /// </summary>
            int formatStartIx, formatEndIx;

            /// <summary>
            /// Status
            /// </summary>
            public LineStatus status;

            /// <summary>
            /// Character index
            /// </summary>
            int i;

            /// <summary>
            /// Format string
            /// </summary>
            IFormatString formatString;

            /// <summary>
            /// Initialize parser
            /// </summary>
            /// <param name="formatString"></param>
            public Parser(IFormatString formatString)
            {
                this.formatString = formatString;
                str = formatString.Text;
                state = ParserState.Text;
                escaped = false;
                partIx = 0;
                occuranceIx = -1;
                functionStartIx = -1; functionEndIx = -1;
                indexStartIx = -1; indexEndIx = -1;
                alignmentStartIx = -1; alignmentEndIx = -1;
                formatStartIx = -1; formatEndIx = -1;
                i = -1;
                status = LineStatus.FormatOk;
            }

            /// <summary>
            /// Has more characters
            /// </summary>
            public bool HasMore => i < str.Length;

            /// <summary>
            /// Complete collected part and reset state.
            /// </summary>
            /// <param name="endIx">end index</param>
            /// <returns>new part or null</returns>
            IFormatStringPart CompletePart(int endIx)
            {
                // Calculate character length
                int length = endIx - partIx;
                // No parts
                if (length == 0) return null;
                // Return text part
                if (state == ParserState.Text)
                {
                    IFormatStringPart part = new TextPart(formatString, partIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Argument ended too soon '{}' or '{function}', return as text part and mark error
                if (state == ParserState.ArgumentStart || state == ParserState.Function)
                {
                    status = LineStatus.FormatErrorMalformed;
                    IFormatStringPart part = new TextPart(formatString, partIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Complete at argument index
                if (state == ParserState.Index)
                {
                    indexEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.FormatErrorMalformed;
                }
                // Complete at alignment
                else if (state == ParserState.Alignment)
                {
                    alignmentEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.FormatErrorMalformed;
                } else if (state == ParserState.Format)
                {
                    formatEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.FormatErrorMalformed;
                }

                // Error with argument index, return as text 
                if (indexStartIx<0||indexEndIx<0||indexStartIx>=indexEndIx)
                {
                    IFormatStringPart part = new TextPart(formatString, partIx, length);
                    status = LineStatus.FormatErrorMalformed;
                    ResetPartState(endIx);
                    return part;
                }

                // Parse argument index
                int argumentIndex;
                try
                {
                    string argumentIndexText = str.Substring(indexStartIx, indexEndIx-indexStartIx);
                    argumentIndex = int.Parse(argumentIndexText, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    // Parse failed, probably too large number
                    IFormatStringPart part = new TextPart(formatString, partIx, length);
                    status = LineStatus.FormatErrorMalformed;
                    ResetPartState(endIx);
                    return part;
                }

                // Function text
                string function = functionStartIx >= 0 && functionEndIx >= 0 && functionStartIx < functionEndIx ? str.Substring(functionStartIx, functionEndIx - functionStartIx) : null;

                // Format text
                string format = formatStartIx >= 0 && formatEndIx >= 0 && formatStartIx < formatEndIx ? str.Substring(formatStartIx, formatEndIx - formatStartIx) : null;

                // Alignment
                int alignment = 0;
                if (alignmentStartIx >= 0 && alignmentEndIx >= 0 && alignmentStartIx < alignmentEndIx)
                {
                    try
                    {
                        string alignmentText = str.Substring(alignmentStartIx, alignmentEndIx - alignmentStartIx);
                        alignment = int.Parse(alignmentText, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        // Parse failed, probably too large number
                        status = LineStatus.FormatErrorMalformed;
                    }
                }

                // Create argument part
                IFormatStringPart argument = new PlaceHolder(formatString, partIx, length, ++occuranceIx, argumentIndex, function, format, alignment);
                // Reset to 'Text' state
                ResetPartState(endIx);
                // Return the constructed argument
                return argument;
            }

            /// <summary>
            /// Reset part state
            /// </summary>
            void ResetPartState(int startIx)
            {
                partIx = startIx;
                functionStartIx = -1; functionEndIx = -1;
                indexStartIx = -1; indexEndIx = -1;
                alignmentStartIx = -1; alignmentEndIx = -1;
                formatStartIx = -1; formatEndIx = -1;
                state = ParserState.Text;
            }

            /// <summary>
            /// Read next character
            /// </summary>
            /// <returns>possible part</returns>
            public IFormatStringPart ReadNext()
            {
                while (HasMore)
                {
                    // Move to next index
                    i++;
                    // Beyond end
                    if (i >= str.Length) return CompletePart(str.Length);
                    // End escape
                    if (escaped) { escaped = false; continue; }
                    // Read char
                    char ch = str[i];
                    // Begin escape
                    if (ch == '\\') { escaped = true; continue; }

                    // Open brace
                    if (ch == '{')
                    {
                        // Start argument
                        if (state == ParserState.Text)
                        {
                            // Complate previous part, and reset state
                            IFormatStringPart part = CompletePart(i);
                            // Start argument
                            state = ParserState.ArgumentStart;
                            // 
                            return part;
                        }
                        else
                        {
                            // Already in argument format and got unexpected unescaped '{'
                            status = LineStatus.FormatErrorMalformed;
                            //
                            continue;
                        }
                    }

                    // Close brace
                    if (ch == '}')
                    {
                        // End argument
                        if (state != ParserState.Text)
                        {
                            // End argument
                            state = ParserState.ArgumentEnd;
                            // Complete previous part, and reset state
                            IFormatStringPart part = CompletePart(i+1);
                            //
                            return part;
                        }
                        else
                        {
                            // In text state and got unexpected unescaped '}'
                            //status = LocalizationStatus.FormatErrorMalformed;
                            // Go on
                            continue;
                        }
                    }

                    // Nothing further for text part
                    if (state == ParserState.Text) continue;

                    // At ArgumentStart, choose next state
                    if (state == ParserState.ArgumentStart)
                    {
                        // index char
                        if (ch >= '0' && ch <= '9')
                        {
                            if (indexStartIx < 0) indexStartIx = i;
                            indexEndIx = i + 1;
                            state = ParserState.Index;
                        }
                        else
                        // function char
                        {
                            if (functionStartIx < 0) functionStartIx = i;
                            functionEndIx = i + 1;
                            state = ParserState.Function;
                        }
                        continue;
                    }

                    // At Function state
                    if (state == ParserState.Function)
                    {
                        // Change to Index state
                        if (ch == ':')
                        {
                            state = ParserState.Index;
                        }
                        else
                        // Move indices
                        {
                            if (functionStartIx < 0) functionStartIx = i;
                            functionEndIx = i + 1;
                        }
                        continue;
                    }

                    // At Index state
                    if (state == ParserState.Index)
                    {
                        // Move indices
                        if (ch >= '0' && ch <= '9')
                        {
                            if (indexStartIx < 0) indexStartIx = i;
                            indexEndIx = i + 1;
                            continue;
                        }
                        // Change to Alignment state
                        if (ch == ',')
                        {
                            state = ParserState.Alignment;
                            continue;
                        }
                        // Change to Format state
                        if (ch == ':')
                        {
                            state = ParserState.Format;
                            continue;
                        }
                        // Unexpected character
                        status = LineStatus.FormatErrorMalformed;
                        return CompletePart(i);
                    }

                    // At Alignment state
                    if (state == ParserState.Alignment)
                    {
                        // Move indices
                        if (ch >= '0' && ch <= '9')
                        {
                            if (alignmentStartIx < 0) alignmentStartIx = i;
                            alignmentEndIx = i + 1;
                            continue;
                        }
                        // Change to Format state
                        if (ch == ':')
                        {
                            state = ParserState.Format;
                            continue;
                        }
                        // Unexpected character
                        status = LineStatus.FormatErrorMalformed;
                        return CompletePart(i);
                    }

                    // At Format state
                    if (state == ParserState.Format)
                    {
                        // Move indices
                        if (formatStartIx < 0) formatStartIx = i;
                        formatEndIx = i + 1;
                        continue;
                    }
                }
                return null;
            }

        }

    }
}
