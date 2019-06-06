// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.StringFormat
{
    #region Interface
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
        /// Name of the format name, e.g. "csharp", "c", or "lexical"
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Parses arguments from format strings. Handles escaping.
    /// 
    /// For example "You received {caridnal:0} coin(s)." is a format string
    /// that parsed into argument and non-argument sections.
    /// </summary>
    public interface IStringFormatParser : IStringFormat
    {
        /// <summary>
        /// Parse format string into an <see cref="IString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LineStatus.StringFormatErrorMalformed"/>.
        /// If parse succeeds, the returned instance has state <see cref="LineStatus.StringFormatOkString"/> or some other format state.
        /// If <paramref name="str"/> is null then stat is <see cref="LineStatus.StringFormatFailedNull"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>format string</returns>
        IString Parse(string str);
    }

    /// <summary>
    /// Prints <see cref="IString"/> into the format.
    /// </summary>
    public interface IStringFormatPrinter : IStringFormat
    {
        /// <summary>
        /// Print <paramref name="str"/> into string that represents the notation of this <see cref="IStringFormat"/>.
        /// 
        /// If print fails status is:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPluralCategory"/></item>
        ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPlaceholder"/></item>
        ///     <item><see cref="LineStatus.StringFormatErrorPrintUnsupportedExpression"/></item>
        ///     <item><see cref="LineStatus.StringFormatFailed"/></item>
        /// </list>
        /// 
        /// If formulated ok, status is <see cref="LineStatus.StringFormatOkString"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>format string</returns>
        LineString Print(IString str);
    }
    #endregion Interface

    /// <summary>
    /// A map of formats
    /// </summary>
    public interface IStringFormatMap : IDictionary<string, IStringFormat>
    {
    }

    /// <summary>
    /// Extenions for <see cref="IStringFormat"/>.
    /// </summary>
    public static partial class ILocalizationStringFormatExtensions
    {
        /// <summary>
        /// Parse format string into an <see cref="IString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LineStatus.StringFormatErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LineStatus.StringFormatOk"/> or some other format state.
        /// If <paramref name="str"/> is null then stat is <see cref="LineStatus.StringFormatFailedNull"/>.
        /// If <paramref name="stringFormat"/> is not <see cref="IStringFormatParser"/>, then stat is <see cref="LineStatus.StringFormatFailedNoParser"/>.
        /// </summary>
        /// <param name="stringFormat"></param>
        /// <param name="str"></param>
        /// <returns>format string</returns>
        /// <exception cref="ArgumentException">If <paramref name="stringFormat"/> doesn't implement <see cref="IStringFormatParser"/></exception>
        public static IString Parse(this IStringFormat stringFormat, string str)
        {
            if (str == null) return new StatusString(str, LineStatus.StringFormatFailedNull);
            if (stringFormat is IStringFormatParser parser) return parser.Parse(str);
            return new StatusString(str, LineStatus.StringFormatFailedNoParser);
        }

        /// <summary>
        /// Print <paramref name="str"/> into string that represents the notation of this <see cref="IStringFormat"/>.
        /// 
        /// If print fails, then states is set:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPluralCategory"/></item>
        ///     <item><see cref="LineStatus.StringFormatErrorPrintNoCapabilityPlaceholder"/></item>
        ///     <item><see cref="LineStatus.StringFormatErrorPrintUnsupportedExpression"/></item>
        ///     <item><see cref="LineStatus.StringFormatFailedNoPrinter"/></item>
        /// </list>
        /// </summary>
        /// <param name="stringFormat"></param>
        /// <param name="str"></param>
        /// <returns>format string</returns>
        public static LineString Print(this IStringFormat stringFormat, IString str)
        {
            if (stringFormat is IStringFormatPrinter printer) return printer.Print(str);
            return new LineString(null, (Exception)null, LineStatus.StringFormatFailedNoPrinter);
        }

        /// <summary>
        /// Parse format string into an <see cref="IString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LineStatus.StringFormatErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LineStatus.StringFormatOk"/> or some other format state.
        /// If <paramref name="formatString"/> is null then stat is <see cref="LineStatus.StringFormatFailedNull"/>.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="formatName"></param>
        /// <param name="formatString"></param>
        /// <returns>format string</returns>
        /// <exception cref="ArgumentException">If <paramref name="formatName"/> doesn't implement <see cref="IStringFormatParser"/></exception>
        public static IString Parse(this IReadOnlyDictionary<string, IStringFormat> formats, string formatName, string formatString)
        {
            IStringFormat format;
            if (!formats.TryGetValue(formatName, out format))
                throw new ArgumentException(formatName);

            if (formats is IStringFormatParser parser) return parser.Parse(formatString);
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
