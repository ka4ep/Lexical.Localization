// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Lexical string format similiar to C# string format, with one difference; arguments can be prefixed with function name.
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
    ///  4. On the left-side of argument is function name:
    ///     "There are {optional:0} cats."
    /// 
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=netframework-4.7.2"/>
    /// </summary>
    public class LexicalStringFormat : ILocalizationStringFormatParser
    {
        private static ILocalizationStringFormatParser instance => new LexicalStringFormat();

        /// <summary>
        /// Default instance.
        /// </summary>
        public static ILocalizationStringFormatParser Instance => instance;

        /// <summary>
        /// Name of this string format.
        /// </summary>
        public string Name => "lexical";

        /// <summary>
        /// Format provider to add to each parsed line.
        /// </summary>
        public virtual IFormatProvider FormatProvider { get; protected set; }

        /// <summary>
        /// Create default format.
        /// </summary>
        public LexicalStringFormat()
        {
        }

        /// <summary>
        /// Create format with default <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="formatProvider">(optional) provider to add to each parsed line.</param>
        public LexicalStringFormat(IFormatProvider formatProvider)
        {
            this.FormatProvider = formatProvider;
        }

        /// <summary>
        /// Create lazily parsed string <paramref name="formulationString"/>.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>preparsed</returns>
        public ILocalizationFormulationString Parse(string formulationString)
        {
            if (formulationString == null) return Null.Instance;
            if (formulationString == "") return Empty.Instance;
            if (FormatProvider != null) return new FormulationStringWithFormatProvider(formulationString, FormatProvider);
            return new FormulationString(formulationString);
        }

        /// <summary>
        /// Lazily parsed formulation string.
        /// </summary>
        public class FormulationString : ILocalizationFormulationString
        {
            /// <summary>
            /// Parsed arguments. Set in <see cref="Build"/>.
            /// </summary>
            ILocalizationFormulationStringArgument[] arguments;

            /// <summary>
            /// String as sequence of parts. Set in <see cref="Build"/>.
            /// </summary>
            ILocalizationFormulationStringPart[] parts;

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
            public ILocalizationFormulationStringPart[] Parts { get { if (status == LocalizationStatus.FormulationFailedNoResult) Build(); return parts; } }

            /// <summary>
            /// Get the parsed arguments.
            /// </summary>
            public ILocalizationFormulationStringArgument[] Arguments { get { if (status == LocalizationStatus.FormulationFailedNoResult) Build(); return arguments; } }

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
            protected  void Build()
            {
                string str = Text;

                LocalizationStatus status = LocalizationStatus.FormulationOk;
                StructList8<ILocalizationFormulationStringPart> parts = new StructList8<ILocalizationFormulationStringPart>();

                int state = 0;
                int braceLevel = 0;
                int startIx = 0;
                // reader state: 0-text, 1-argumentStart, 2-function, 3-index, 4-alignment, 5-format, 6-
                Parser parser = new Parser();

                // Create parts array
                var partArray = new ILocalizationFormulationStringPart[parts.Count];
                parts.CopyTo(partArray, 0);

                // Create arguments array
                int argumentCount = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is ILocalizationFormulationStringArgument) argumentCount++;
                var argumentsArray = new ILocalizationFormulationStringArgument[argumentCount];
                int j = 0;
                for (int i = 0; i < parts.Count; i++) if (parts[i] is ILocalizationFormulationStringArgument argPart) argumentsArray[j++] = argPart;

                // Write status.
                Thread.MemoryBarrier();
                this.arguments = argumentsArray;
                this.parts = partArray;
                this.status = status;
            }

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
        public class Null : ILocalizationFormulationString
        {
            static ILocalizationFormulationString instance => new Null();
            static ILocalizationFormulationStringPart[] parts = new ILocalizationFormulationStringPart[0];
            static ILocalizationFormulationStringArgument[] arguments = new ILocalizationFormulationStringArgument[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static ILocalizationFormulationString Instance => instance;
            /// <summary />
            public LocalizationStatus Status => LocalizationStatus.FormulationFailedNull;
            /// <summary />
            public string Text => null;
            /// <summary />
            public ILocalizationFormulationStringPart[] Parts => parts;
            /// <summary />
            public ILocalizationFormulationStringArgument[] Arguments => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;
        }

        /// <summary>
        /// Empty formulation string.
        /// </summary>
        public class Empty : ILocalizationFormulationString
        {
            static ILocalizationFormulationString instance => new Empty();
            static ILocalizationFormulationStringPart[] parts = new ILocalizationFormulationStringPart[0];
            static ILocalizationFormulationStringArgument[] arguments = new ILocalizationFormulationStringArgument[0];
            /// <summary>
            /// Default instance.
            /// </summary>
            public static ILocalizationFormulationString Instance => instance;
            /// <summary />
            public LocalizationStatus Status => LocalizationStatus.FormulationFailedNull;
            /// <summary />
            public string Text => "";
            /// <summary />
            public ILocalizationFormulationStringPart[] Parts => parts;
            /// <summary />
            public ILocalizationFormulationStringArgument[] Arguments => arguments;
            /// <summary />
            public IFormatProvider FormatProvider => null;
        }

        /// <summary>
        /// Text part
        /// </summary>
        public class TextPart : ILocalizationFormulationStringPart
        {
            /// <summary>
            /// The 'parent' formulation string.
            /// </summary>
            public ILocalizationFormulationString FormulationString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public LocalizationFormulationStringPartKind Kind => LocalizationFormulationStringPartKind.Text;

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
            /// Create text part.
            /// </summary>
            /// <param name="formulationString"></param>
            /// <param name="index">first character index</param>
            /// <param name="length">character length</param>
            public TextPart(ILocalizationFormulationString formulationString, int index, int length)
            {
                FormulationString = formulationString;
                Index = index;
                Length = length;
            }

            /// <summary>
            /// The text part as it appears in the formulation string.
            /// </summary>
            public override string ToString() => FormulationString.Text.Substring(Index, Length);
        }

        /// <summary>
        /// Parsed argument info.
        /// </summary>
        public class Argument : ILocalizationFormulationStringArgument
        {
            /// <summary>
            /// The 'parent' formulation string.
            /// </summary>
            public ILocalizationFormulationString FormulationString { get; internal set; }

            /// <summary>
            /// Part type
            /// </summary>
            public LocalizationFormulationStringPartKind Kind => LocalizationFormulationStringPartKind.Argument;

            /// <summary>
            /// The whole argument definition as it appears in the formulation string.
            /// </summary>
            public string Text => FormulationString.Text.Substring(Index, Length);

            /// <summary>
            /// Occurance index in <see cref="FormulationString"/>.
            /// </summary>
            public int OccuranceIndex { get; internal set; }

            /// <summary>
            /// Argument index. Refers to index in the args array of <see cref="ILocalizationKeyFormatArgs.Args"/>.
            /// </summary>
            public int ArgumentIndex { get; internal set; }

            /// <summary>
            /// Argument name.
            /// </summary>
            public string ArgumentName => ArgumentIndex.ToString();

            /// <summary>
            /// Start index of first character of the argument in the formulation string.
            /// </summary>
            public int Index { get; internal set; }

            /// <summary>
            /// Length of characters in the formulation string.
            /// </summary>
            public int Length { get; internal set; }

            /// <summary>
            /// Associated format provider for this particular argument.
            /// </summary>
            public IFormatProvider FormatProvider => throw new NotImplementedException();

            /// <summary>
            /// (Optional) The function name that is passed to <see cref="ILocalizationArgumentFormatter"/>.
            /// E.g. "plural", "optional", "range", "ordinal".
            /// </summary>
            public string Function { get; internal set; }

            /// <summary>
            /// (Optional) The format text that is passed to <see cref="ICustomFormatter"/>, <see cref="IFormattable"/> and <see cref="ILocalizationArgumentFormatter"/>.
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
            public Argument(ILocalizationFormulationString formulationString, int index, int length, int occuranceIndex, int argumentIndex, string function, string format, int alignment)
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
            /// Print argument formulation as it is in the formulation string. Example "{0:x2}".
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => FormulationString.Text.Substring(Index, Length);
        }

        /// <summary>
        /// Parser that breaks formulation string into parts
        /// </summary>
        public struct Parser 
        {
        }
    }
}
