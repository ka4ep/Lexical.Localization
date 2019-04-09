// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Represents an interface to a localization string format. 
    /// 
    /// For example C# format that uses numbered arguments "{0[,parameters]}" that are written inside braces and have
    /// parameters after number.
    /// 
    /// It has following sub-interfaces:
    /// <list type="bullet">
    /// <item><see cref="ILocalizationStringFormatParser"/></item>
    /// </list>
    /// </summary>
    public interface ILocalizationStringFormat
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
    public interface ILocalizationStringFormatParser : ILocalizationStringFormat
    {
        /// <summary>
        /// Parse formulation string into an <see cref="ILocalizationFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// </summary>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        ILocalizationFormulationString Parse(string formulationString);
    }

    /// <summary>
    /// Extenions for <see cref="ILocalizationStringFormat"/>.
    /// </summary>
    public static partial class LocalizationStringFormatExtensions
    {
        /// <summary>
        /// Parse formulation string into an <see cref="ILocalizationFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="ArgumentException">If <paramref name="format"/> doesn't implement <see cref="ILocalizationStringFormatParser"/></exception>
        public static ILocalizationFormulationString Parse(this ILocalizationStringFormat format, string formulationString)
        {
            if (format is ILocalizationStringFormatParser parser) return parser.Parse(formulationString);
            throw new ArgumentException($"{format} doesn't implement {nameof(ILocalizationStringFormatParser)}.");
        }
    }

}
