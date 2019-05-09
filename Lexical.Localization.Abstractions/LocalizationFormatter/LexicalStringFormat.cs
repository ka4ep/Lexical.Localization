// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Lexical.Localization
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
        /// Create lazily parsed string <paramref name="formulationString"/>.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>preparsed</returns>
        public IFormulationString Parse(string formulationString)
        {
            if (formulationString == null) return Null.Instance;
            if (formulationString == "") return Empty.Instance;
            if (FormatProvider != null) return new FormulationStringWithFormatProvider(formulationString, FormatProvider);
            return new FormulationString(formulationString);
        }

        /// <summary>
        /// Print as string
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>string or null</returns>
        public string Print(IFormulationString formulationString)
        {
            return formulationString?.Text;
        }

        /// <summary>
        /// Lazily parsed formulation string.
        /// </summary>
        public class FormulationString : IFormulationString
        {
            /// <summary>
            /// Parsed arguments. Set in <see cref="Build"/>.
            /// </summary>
            IFormulationStringArgument[] arguments;

            /// <summary>
            /// String as sequence of parts. Set in <see cref="Build"/>.
            /// </summary>
            IFormulationStringPart[] parts;

            /// <summary>
            /// Parse status. Set in <see cref="Build"/>.
            /// </summary>
            LocalizationStatus status;

            /// <summary>
            /// Get the original formulation string.
            /// </summary>
            public string Text { get; internal set; }

            /// <summary>
            /// Get parse status.
            /// </summary>
            public LocalizationStatus Status { get { if (status == LocalizationStatus.FormulationFailedNoResult) Build(); return status; } }

            /// <summary>
            /// Formulation string broken into sequence of text and argument parts.
            /// </summary>
            public IFormulationStringPart[] Parts { get { if (status == LocalizationStatus.FormulationFailedNoResult) Build(); return parts; } }

            /// <summary>
            /// Get the parsed arguments.
            /// </summary>
            public IFormulationStringArgument[] Arguments { get { if (status == LocalizationStatus.FormulationFailedNoResult) Build(); return arguments; } }

            /// <summary>
            /// (optional) Get associated format provider. This is typically a plurality rules and  originates from a localization file.
            /// </summary>
            public virtual IFormatProvider FormatProvider => null;

            /// <summary>
            /// Create formulation string that parses arguments lazily.
            /// </summary>
            /// <param name="text"></param>
            public FormulationString(string text)
            {
                Text = text;
                status = text == null ? LocalizationStatus.FormulationFailedNull : LocalizationStatus.FormulationFailedNoResult;
            }

            /// <summary>
            /// Parse string
            /// </summary>
            protected void Build()
            {
                StructList8<IFormulationStringPart> parts = new StructList8<IFormulationStringPart>();

                // Create parser
                Parser parser = new Parser(this);

                // Read parts
                while (parser.HasMore)
                {
                    IFormulationStringPart part = parser.ReadNext();
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
                var partArray = new IFormulationStringPart[parts.Count];
                parts.CopyTo(partArray, 0);
                // Set PartsArrayIndex
                for (int i = 0; i < partArray.Length; i++)
                {
                    if (partArray[i] is TextPart textPart) textPart.PartsIndex = i;
                    else if (partArray[i] is Argument argPart) argPart.PartsIndex = i;
                }

                // Create arguments array
                int argumentCount = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is IFormulationStringArgument) argumentCount++;
                var argumentsArray = new IFormulationStringArgument[argumentCount];
                int j = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is Argument argPart) argumentsArray[j++] = argPart;
                Array.Sort(argumentsArray, ArgumentOrderComparer.Instance);
                for (int i = 0; i < argumentsArray.Length; i++) ((Argument)argumentsArray[i]).ArgumentsIndex = i;

                // Write status.
                Thread.MemoryBarrier();
                this.arguments = argumentsArray;
                this.parts = partArray;
                this.status = parser.status;
            }

            /// <summary>
            /// Comparer that compares first by argument index, then by occurance index.
            /// </summary>
            class ArgumentOrderComparer : IComparer<IFormulationStringArgument>
            {
                static IComparer<IFormulationStringArgument> instance = new ArgumentOrderComparer();
                public static IComparer<IFormulationStringArgument> Instance => instance;

                public int Compare(IFormulationStringArgument x, IFormulationStringArgument y)
                {
                    int c = x.ArgumentIndex - y.ArgumentIndex;
                    if (c < 0) return -1;
                    if (c > 0) return 1;
                    c = x.OccuranceIndex - y.OccuranceIndex;
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
                => FormulationStringComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => obj is IFormulationString other ? FormulationStringComparer.Instance.Equals(this, other) : false;

            /// <summary>
            /// Formulation string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => Text;
        }

        /// <summary>
        /// Version of formulation string that carries an associated format provider.
        /// </summary>
        public class FormulationStringWithFormatProvider : FormulationString
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
            /// Create lazily parsed formulation string that carries a <paramref name="formatProvider"/>.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="formatProvider"></param>
            public FormulationStringWithFormatProvider(string text, IFormatProvider formatProvider) : base(text)
            {
                this.formatProvider = formatProvider;
            }
        }

        /// <summary>
        /// Null formulation string.
        /// </summary>
        public class Null : IFormulationString
        {
            static IFormulationString instance => new Null();
            static IFormulationStringPart[] parts = new IFormulationStringPart[0];
            static IFormulationStringArgument[] arguments = new IFormulationStringArgument[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static IFormulationString Instance => instance;
            /// <summary />
            public LocalizationStatus Status => LocalizationStatus.FormulationFailedNull;
            /// <summary />
            public string Text => null;
            /// <summary />
            public IFormulationStringPart[] Parts => parts;
            /// <summary />
            public IFormulationStringArgument[] Arguments => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;

            /// <summary>
            /// Cached hashcode
            /// </summary>
            int hashcode => FormulationStringComparer.Instance.GetHashCode(this);

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
                => obj is IFormulationString other ? FormulationStringComparer.Instance.Equals(this, other) : false;

        }

        /// <summary>
        /// Empty formulation string.
        /// </summary>
        public class Empty : IFormulationString
        {
            static IFormulationString instance => new Empty();
            static IFormulationStringPart[] parts = new IFormulationStringPart[0];
            static IFormulationStringArgument[] arguments = new IFormulationStringArgument[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static IFormulationString Instance => instance;
            /// <summary />
            public LocalizationStatus Status => LocalizationStatus.FormulationFailedNull;
            /// <summary />
            public string Text => "";
            /// <summary />
            public IFormulationStringPart[] Parts => parts;
            /// <summary />
            public IFormulationStringArgument[] Arguments => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;

            /// <summary>
            /// Cached hashcode
            /// </summary>
            int hashcode => FormulationStringComparer.Instance.GetHashCode(this);

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
                => obj is IFormulationString other ? FormulationStringComparer.Instance.Equals(this, other) : false;
        }

        /// <summary>
        /// Text part
        /// </summary>
        public class TextPart : IFormulationStringPart
        {
            /// <summary>
            /// Unify two parts
            /// </summary>
            /// <param name="leftPart"></param>
            /// <param name="rightPart"></param>
            /// <returns></returns>
            internal static TextPart Unify(TextPart leftPart, TextPart rightPart)
                => new TextPart(leftPart.FormulationString, leftPart.Index, rightPart.Index - leftPart.Index + rightPart.Length);

            /// <summary>
            /// The 'parent' formulation string.
            /// </summary>
            public IFormulationString FormulationString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public FormulationStringPartKind Kind => FormulationStringPartKind.Text;

            /// <summary>
            /// Start index of first character of the argument in the formulation string.
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            /// Length of characters in the formulation string.
            /// </summary>
            public int Length { get; internal set; }

            /// <summary>
            /// The text part as it appears in the formulation string.
            /// </summary>
            public string Text => FormulationString.Text.Substring(Index, Length);

            /// <summary>
            /// Index in Parts array.
            /// </summary>
            public int PartsIndex { get; internal set; }

            /// <summary>
            /// Create text part.
            /// </summary>
            /// <param name="formulationString"></param>
            /// <param name="index">first character index</param>
            /// <param name="length">character length</param>
            public TextPart(IFormulationString formulationString, int index, int length)
            {
                FormulationString = formulationString;
                Index = index;
                Length = length;
            }

            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode() 
                => FormulationStringPartComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => FormulationStringPartComparer.Instance.Equals(obj);

            /// <summary>
            /// The text part as it appears in the formulation string.
            /// </summary>
            public override string ToString() => FormulationString.Text.Substring(Index, Length);
        }

        /// <summary>
        /// Parsed argument info.
        /// </summary>
        public class Argument : IFormulationStringArgument
        {
            /// <summary>
            /// The 'parent' formulation string.
            /// </summary>
            public IFormulationString FormulationString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public FormulationStringPartKind Kind => FormulationStringPartKind.Argument;

            /// <summary>
            /// The whole argument definition as it appears in the formulation string.
            /// </summary>
            public string Text => FormulationString.Text.Substring(Index, Length);

            /// <summary>
            /// Occurance index in <see cref="FormulationString"/>.
            /// </summary>
            public int OccuranceIndex { get; internal set; }

            /// <summary>
            /// Argument index. Refers to index in the args array of <see cref="ILineFormatArgsPart.Args"/>.
            /// </summary>
            public int ArgumentIndex { get; internal set; }

            /// <summary>
            /// Argument name.
            /// </summary>
            public string ArgumentName => null;

            /// <summary>
            /// Start index of first character of the argument in the formulation string.
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            /// Length of characters in the formulation string.
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
            /// <param name="formulationString"></param>
            /// <param name="index">first character index</param>
            /// <param name="length">character length</param>
            /// <param name="occuranceIndex"></param>
            /// <param name="argumentIndex"></param>
            /// <param name="function">(optional)</param>
            /// <param name="format">(optional)</param>
            /// <param name="alignment"></param>
            public Argument(IFormulationString formulationString, int index, int length, int occuranceIndex, int argumentIndex, string function, string format, int alignment)
            {
                FormulationString = formulationString ?? throw new ArgumentNullException(nameof(formulationString));
                Index = index;
                Length = length;
                Function = function;
                Format = format;
                OccuranceIndex = occuranceIndex;
                ArgumentIndex = argumentIndex;
                Alignment = alignment;
            }

            /// <summary>
            /// Calculate hashcode
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
                => FormulationStringPartComparer.Instance.GetHashCode(this);

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
                => FormulationStringPartComparer.Instance.Equals(obj);

            /// <summary>
            /// Print argument formulation as it is in the formulation string. Example "{0:x2}".
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => FormulationString.Text.Substring(Index, Length);
        }

        enum ParserState { Text, ArgumentStart, Function, Index, Alignment, Format, ArgumentEnd }

        /// <summary>
        /// Parser that breaks formulation string into parts
        /// </summary>
        struct Parser
        {
            /// <summary>
            /// Formulation string
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
            public LocalizationStatus status;

            /// <summary>
            /// Character index
            /// </summary>
            int i;

            /// <summary>
            /// Formulation string
            /// </summary>
            IFormulationString formulationString;

            /// <summary>
            /// Initialize parser
            /// </summary>
            /// <param name="formulationString"></param>
            public Parser(IFormulationString formulationString)
            {
                this.formulationString = formulationString;
                str = formulationString.Text;
                state = ParserState.Text;
                escaped = false;
                partIx = 0;
                occuranceIx = -1;
                functionStartIx = -1; functionEndIx = -1;
                indexStartIx = -1; indexEndIx = -1;
                alignmentStartIx = -1; alignmentEndIx = -1;
                formatStartIx = -1; formatEndIx = -1;
                i = -1;
                status = LocalizationStatus.FormulationOk;
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
            IFormulationStringPart CompletePart(int endIx)
            {
                // Calculate character length
                int length = endIx - partIx;
                // No parts
                if (length == 0) return null;
                // Return text part
                if (state == ParserState.Text)
                {
                    IFormulationStringPart part = new TextPart(formulationString, partIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Argument ended too soon '{}' or '{function}', return as text part and mark error
                if (state == ParserState.ArgumentStart || state == ParserState.Function)
                {
                    status = LocalizationStatus.FormulationErrorMalformed;
                    IFormulationStringPart part = new TextPart(formulationString, partIx, length);
                    ResetPartState(endIx);
                    return part;
                }
                // Complete at argument index
                if (state == ParserState.Index)
                {
                    indexEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LocalizationStatus.FormulationErrorMalformed;
                }
                // Complete at alignment
                else if (state == ParserState.Alignment)
                {
                    alignmentEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LocalizationStatus.FormulationErrorMalformed;
                } else if (state == ParserState.Format)
                {
                    formatEndIx = endIx;
                    // Unfinished, did not get '}'
                    status = LocalizationStatus.FormulationErrorMalformed;
                }

                // Error with argument index, return as text 
                if (indexStartIx<0||indexEndIx<0||indexStartIx>=indexEndIx)
                {
                    IFormulationStringPart part = new TextPart(formulationString, partIx, length);
                    status = LocalizationStatus.FormulationErrorMalformed;
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
                    IFormulationStringPart part = new TextPart(formulationString, partIx, length);
                    status = LocalizationStatus.FormulationErrorMalformed;
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
                        status = LocalizationStatus.FormulationErrorMalformed;
                    }
                }

                // Create argument part
                IFormulationStringPart argument = new Argument(formulationString, partIx, length, ++occuranceIx, argumentIndex, function, format, alignment);
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
            public IFormulationStringPart ReadNext()
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
                            IFormulationStringPart part = CompletePart(i);
                            // Start argument
                            state = ParserState.ArgumentStart;
                            // 
                            return part;
                        }
                        else
                        {
                            // Already in argument formulation and got unexpected unescaped '{'
                            status = LocalizationStatus.FormulationErrorMalformed;
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
                            IFormulationStringPart part = CompletePart(i+1);
                            //
                            return part;
                        }
                        else
                        {
                            // In text state and got unexpected unescaped '}'
                            //status = LocalizationStatus.FormulationErrorMalformed;
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
                        status = LocalizationStatus.FormulationErrorMalformed;
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
                        status = LocalizationStatus.FormulationErrorMalformed;
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
