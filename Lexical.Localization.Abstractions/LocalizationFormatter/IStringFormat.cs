// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// String format of string value. 
    /// 
    /// For example C# format that uses numbered arguments "{0[,parameters]}" that are written inside braces and have
    /// parameters after number.
    /// 
    /// It has following sub-interfaces:
    /// <list type="bullet">
    /// <item><see cref="IStringFormatParser"/></item>
    /// </list>
    /// </summary>
    public interface IStringFormat
    {
        /// <summary>
        /// Name of the formulation name, e.g. "csharp", "c", or "lexical"
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Parses arguments from formulation strings. Handles escaping.
    /// 
    /// For example "You received {plural:0} coin(s)." is a formulation string
    /// that parsed into argument and non-argument sections.
    /// </summary>
    public interface IStringFormatParser : IStringFormat
    {
        /// <summary>
        /// Parse formulation string into an <see cref="IFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        IFormulationString Parse(string formulationString);
    }

    /// <summary>
    /// Prints <see cref="IFormulationString"/> into the format.
    /// </summary>
    public interface IStringFormatPrinter : IStringFormat
    {
        /// <summary>
        /// Print formulation string into string.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        string Print(IFormulationString formulationString);
    }

    /// <summary>
    /// A map of formats
    /// </summary>
    public interface IStringFormats : IDictionary<string, IStringFormat>
    {
    }

    /// <summary>
    /// Resolves name to <see cref="IStringFormat"/>.
    /// </summary>
    public interface IStringFormatResolver
    {
        /// <summary>
        /// Resolve <paramref name="name"/> to <see cref="IStringFormat"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string format or null</returns>
        IStringFormat Resolve(string name);
    }

    /// <summary>
    /// Extenions for <see cref="IStringFormat"/>.
    /// </summary>
    public static partial class ILocalizationStringFormatExtensions
    {
        /// <summary>
        /// Parse formulation string into an <see cref="IFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// If <paramref name="format"/> is not <see cref="IStringFormatParser"/>, then stat is <see cref="LocalizationStatus.FormulationFailedNoParser"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="ArgumentException">If <paramref name="format"/> doesn't implement <see cref="IStringFormatParser"/></exception>
        public static IFormulationString Parse(this IStringFormat format, string formulationString)
        {
            if (formulationString == null) return new FormulationStringStatus(formulationString, LocalizationStatus.FormulationFailedNull);
            if (format is IStringFormatParser parser) return parser.Parse(formulationString);
            return new FormulationStringStatus(formulationString, LocalizationStatus.FormulationFailedNoParser);
        }

        /// <summary>
        /// Print formulation string into string.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        public static string Print(this IStringFormat format, IFormulationString formulationString)
        {
            if (format is IStringFormatPrinter printer) return printer.Print(formulationString);
            throw new ArgumentException($"{format} doesn't implement {nameof(IStringFormatPrinter)}.");
        }

        /// <summary>
        /// Parse formulation string into an <see cref="IFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="formatName"></param>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="ArgumentException">If <paramref name="formatName"/> doesn't implement <see cref="IStringFormatParser"/></exception>
        public static IFormulationString Parse(this IReadOnlyDictionary<string, IStringFormat> formats, string formatName, string formulationString)
        {
            IStringFormat format;
            if (!formats.TryGetValue(formatName, out format))
                throw new ArgumentException(formatName);

            if (formats is IStringFormatParser parser) return parser.Parse(formulationString);
            throw new ArgumentException($"{formats} doesn't implement {nameof(IStringFormatParser)}.");
        }

        /// <summary>
        /// Add format to map.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="format"></param>
        /// <returns><paramref name="formats"/></returns>
        public static IDictionary<string, IStringFormat> Add(this IDictionary<string, IStringFormat> formats, IStringFormat format)
        {
            formats[format.Name] = format;
            return formats;
        }

        /// <summary>
        /// Add format to map.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="formatsToAdd"></param>
        /// <returns><paramref name="formats"/></returns>
        public static IDictionary<string, IStringFormat> AddRange(this IDictionary<string, IStringFormat> formats, IEnumerable<IStringFormat> formatsToAdd)
        {
            foreach (var format in formatsToAdd)
                formats[format.Name] = format;
            return formats;
        }

    }

}
