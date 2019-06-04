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

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// C# string format, with one difference, plural category is configurable at the left of placeholder with ':' separator. 
    /// Corresponding plural case must be configured in the supplying localization lines.
    /// 
    ///     "Text {[pluralCategory:]argumentIndex:[,alignment][:format]} text".
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
    ///  4. On the left-side there pluralCategory. Category start with a number, cannot contain unescaped colon ':'.
    ///     "There are {cardinal:0} cats."
    /// 
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=netframework-4.7.2"/>
    /// </summary>
    public partial class CSharpFormat : IStringFormatParser, IStringFormatPrinter
    {
        private static IStringFormatParser instance => new CSharpFormat();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IStringFormatParser Default => instance;

        /// <summary>
        /// Name of this string format.
        /// </summary>
        public string Name => "csharp";

        /// <summary>
        /// Format provider to add to each parsed line.
        /// </summary>
        public virtual IFormatProvider FormatProvider { get; protected set; }

        IString _null, _empty;

        /// <summary>
        /// Create default format.
        /// </summary>
        public CSharpFormat()
        {
            _null = new NullString(this);
            _empty = new EmptyString(this);
        }

        /// <summary>
        /// Create format with default <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="formatProvider">(optional) provider to add to each parsed line.</param>
        public CSharpFormat(IFormatProvider formatProvider) : this()
        {
            this.FormatProvider = formatProvider;
        }

        /// <summary>
        /// Create lazily parsed string <paramref name="formatString"/>.
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns>preparsed</returns>
        public IString Parse(string formatString)
        {
            if (formatString == null) return _null;
            if (formatString == "") return _empty;
            if (FormatProvider != null) return new CSharpString.WithFormatProvider(formatString, this, FormatProvider);
            return new CSharpString(formatString, this);
        }

        /// <summary>
        /// Print as string
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string or null</returns>
        public LineString Print(IString str)
        {
            if (str is CSharpFormat && str.Text != null) return new LineString(null, str.Text, LineStatus.StringFormatOkString);

            StringBuilder sb = new StringBuilder();
            LineStatus status = 0UL;
            foreach(var part in str.Parts)
            {
                // Escape '{' '}' and '\' to '\{' '\}' '\\'
                if (part.Kind == StringPartKind.Text)
                {
                    string s = part.Text;
                    for (int i=0; i<s.Length; i++)
                    {
                        char c = s[i];
                        if (c == '{' || c == '}' || c == '\\') sb.Append('\\');
                        sb.Append(c);
                    }
                    continue;
                }

                // Placeholder
                if (part.Kind == StringPartKind.Placeholder && part is IPlaceholder placeholder)
                {
                    int argumentIx = -1;
                    string format = null;
                    int alignment = 0;
                    StructList8<IExpression> queue = new StructList8<IExpression>();
                    queue.Add(placeholder.Expression);
                    while (queue.Count > 0)
                    {
                        IExpression e = queue.Dequeue();

                        if (e is IArgumentIndexExpression arg)
                        {
                            // Second argument index
                            if (argumentIx >= 0) status.Up(LineStatus.StringFormatErrorPrintNoCapabilityMultipleArguments); 
                            else argumentIx = arg.Index;
                            continue;
                        }

                        if (e is ICallExpression call)
                        {
                            if (call.Name == "Format" && call.Args != null && call.Args.Length == 2 && call.Args[1] is IConstantExpression formatExp && formatExp.Value is string formatStr)
                            {
                                queue.Add(call.Args[0]);
                                format = formatStr;
                            }
                            else if (call.Name == "Alignment" && call.Args != null && call.Args.Length == 2 && call.Args[1] is IConstantExpression alignmentExp)
                            {
                                queue.Add(call.Args[0]);
                                if (alignmentExp.Value is long longValue) alignment = (int)longValue;
                                else if (alignmentExp.Value is int intValue) alignment = intValue;
                                else status.Up(LineStatus.StringFormatErrorPrintUnsupportedExpression);
                            }
                            else status.Up(LineStatus.StringFormatErrorPrintUnsupportedExpression);
                            continue;
                        }

                        status.Up(LineStatus.StringFormatErrorPrintUnsupportedExpression);
                    }

                    if (argumentIx>=0)
                    {
                        sb.Append('{');
                        if (placeholder.PluralCategory != null) { sb.Append(placeholder.PluralCategory); sb.Append(':'); }
                        sb.Append(argumentIx);
                        if (alignment!=0) { sb.Append(','); sb.Append(alignment); }
                        if (format != null) { sb.Append(":"); sb.Append(format); }
                        sb.Append('}');
                    }

                    continue;
                }

                // Malformed
                status.Up(LineStatus.StringFormatErrorMalformed);
            }

            return new LineString(null, sb.ToString(), status);
        }

        enum ParserState { Text, ArgumentStart, PluralCategory, Index, Alignment, Format, ArgumentEnd }

        /// <summary>
        /// Parser that breaks format string into parts
        /// </summary>
        public struct CSharpParser
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
            /// Placeholder index
            /// </summary>
            int placeholderIndex;

            /// <summary>
            /// Part's start index
            /// </summary>
            int strIx;

            /// <summary>
            /// Function text indices
            /// </summary>
            int pluralCategoryStartIx, pluralCategoryEndIx;

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
            IString formatString;

            /// <summary>
            /// Initialize parser
            /// </summary>
            /// <param name="formatString"></param>
            public CSharpParser(IString formatString)
            {
                this.formatString = formatString;
                str = formatString.Text;
                state = ParserState.Text;
                escaped = false;
                strIx = 0;
                placeholderIndex = -1;
                pluralCategoryStartIx = -1; pluralCategoryEndIx = -1;
                indexStartIx = -1; indexEndIx = -1;
                alignmentStartIx = -1; alignmentEndIx = -1;
                formatStartIx = -1; formatEndIx = -1;
                i = -1;
                status = LineStatus.StringFormatOk;
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
            IStringPart CompletePart(int endIx)
            {
                // Calculate character length
                int length = endIx - strIx;
                // No parts
                if (length == 0) return null;
                // Return text part
                if (state == ParserState.Text)
                {
                    IStringPart part = new CSharpString.TextPart(formatString, strIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Argument ended too soon '{}' or '{function}', return as text part and mark error
                if (state == ParserState.ArgumentStart || state == ParserState.PluralCategory)
                {
                    status = LineStatus.StringFormatErrorMalformed;
                    IStringPart part = new CSharpString.TextPart(formatString, strIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Complete at argument index
                if (state == ParserState.Index)
                {
                    indexEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.StringFormatErrorMalformed;
                }
                // Complete at alignment
                else if (state == ParserState.Alignment)
                {
                    alignmentEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.StringFormatErrorMalformed;
                }
                else if (state == ParserState.Format)
                {
                    formatEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LineStatus.StringFormatErrorMalformed;
                }

                // Error with argument index, return as text 
                if (indexStartIx < 0 || indexEndIx < 0 || indexStartIx >= indexEndIx)
                {
                    IStringPart part = new CSharpString.TextPart(formatString, strIx, length);
                    status = LineStatus.StringFormatErrorMalformed;
                    ResetPartState(endIx);
                    return part;
                }

                // Parse argument index
                int argumentIndex;
                try
                {
                    string argumentIndexText = str.Substring(indexStartIx, indexEndIx - indexStartIx);
                    argumentIndex = int.Parse(argumentIndexText, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    // Parse failed, probably too large number
                    IStringPart part = new CSharpString.TextPart(formatString, strIx, length);
                    status = LineStatus.StringFormatErrorMalformed;
                    ResetPartState(endIx);
                    return part;
                }

                // Function text
                string pluralCategory = pluralCategoryStartIx >= 0 && pluralCategoryEndIx >= 0 && pluralCategoryStartIx < pluralCategoryEndIx ? str.Substring(pluralCategoryStartIx, pluralCategoryEndIx - pluralCategoryStartIx) : null;

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
                        status = LineStatus.StringFormatErrorMalformed;
                    }
                }

                // Create argument part
                IExpression exp = new ArgumentIndexExpression(argumentIndex);
                if (format != null) exp = new CallExpression("Format", exp, new ConstantExpression(format));
                if (alignment != 0) exp = new CallExpression("Alignment", exp, new ConstantExpression(alignment));
                IStringPart part_ = new Placeholder(formatString, strIx, length, -1, ++placeholderIndex, pluralCategory, exp);
                // Reset to 'Text' state
                ResetPartState(endIx);
                // Return the constructed argument
                return part_;
            }

            /// <summary>
            /// Reset part state
            /// </summary>
            void ResetPartState(int startIx)
            {
                strIx = startIx;
                pluralCategoryStartIx = -1; pluralCategoryEndIx = -1;
                indexStartIx = -1; indexEndIx = -1;
                alignmentStartIx = -1; alignmentEndIx = -1;
                formatStartIx = -1; formatEndIx = -1;
                state = ParserState.Text;
            }

            /// <summary>
            /// Read next character
            /// </summary>
            /// <returns>possible part</returns>
            public IStringPart ReadNext()
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
                            IStringPart part = CompletePart(i);
                            // Start argument
                            state = ParserState.ArgumentStart;
                            // 
                            return part;
                        }
                        else
                        {
                            // Already in argument format and got unexpected unescaped '{'
                            status = LineStatus.StringFormatErrorMalformed;
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
                            IStringPart part = CompletePart(i + 1);
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
                            if (pluralCategoryStartIx < 0) pluralCategoryStartIx = i;
                            pluralCategoryEndIx = i + 1;
                            state = ParserState.PluralCategory;
                        }
                        continue;
                    }

                    // At PluralCategory state
                    if (state == ParserState.PluralCategory)
                    {
                        // Change to Index state
                        if (ch == ':')
                        {
                            state = ParserState.Index;
                        }
                        else
                        // Move indices
                        {
                            if (pluralCategoryStartIx < 0) pluralCategoryStartIx = i;
                            pluralCategoryEndIx = i + 1;
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
                        status = LineStatus.StringFormatErrorMalformed;
                        return CompletePart(i);
                    }

                    // At Alignment state
                    if (state == ParserState.Alignment)
                    {
                        // Move indices
                        if ((ch >= '0' && ch <= '9') || (ch == '-'))
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
                        status = LineStatus.StringFormatErrorMalformed;
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
