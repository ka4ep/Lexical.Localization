//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Collection of localization formats.
    /// </summary>
    public class LocalizationStringFormats : ConcurrentDictionary<string, ILocalizationStringFormat>
    {
        private static LocalizationStringFormats instance;

        static LocalizationStringFormats()
        {
            instance = new LocalizationStringFormats();
            instance["charp"] = LexicalStringFormat.Instance;
            instance[LexicalStringFormat.Instance.Name] = LexicalStringFormat.Instance;
        }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IReadOnlyDictionary<string, ILocalizationStringFormat> Instance => instance;

        /// <summary>
        /// Create new string format map.
        /// </summary>
        public LocalizationStringFormats() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Create new string format map.
        /// </summary>
        /// <param name="stringFormats"></param>
        public LocalizationStringFormats(params ILocalizationStringFormat[] stringFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(stringFormats);
        }

        /// <summary>
        /// Create new string format map.
        /// </summary>
        /// <param name="stringFormats"></param>
        public LocalizationStringFormats(IEnumerable<ILocalizationStringFormat> stringFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(stringFormats);
        }
    }

    /// <summary>
    /// Extenions for <see cref="LocalizationStringFormats"/>.
    /// </summary>
    public static partial class LocalizationStringFormatMapExtensions_
    {
        /// <summary>
        /// Parse formulation string into an <see cref="ILocalizationFormulationString"/>.
        /// 
        /// If parse fails this method should return an instance where state is <see cref="LocalizationStatus.FormulationErrorMalformed"/>.
        /// If parse succeeds, the returned instance should have state <see cref="LocalizationStatus.FormulationOk"/> or some other formulation state.
        /// If <paramref name="formulationString"/> is null then stat is <see cref="LocalizationStatus.FormulationFailedNull"/>.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="formatName"></param>
        /// <param name="formulationString"></param>
        /// <returns>formulation string</returns>
        /// <exception cref="ArgumentException">If <paramref name="formatName"/> doesn't implement <see cref="ILocalizationStringFormatParser"/></exception>
        public static ILocalizationFormulationString Parse(this IReadOnlyDictionary<string, ILocalizationStringFormat> formats, string formatName, string formulationString)
        {
            ILocalizationStringFormat format;
            if (!formats.TryGetValue(formatName, out format))
                throw new ArgumentException(formatName);

            if (formats is ILocalizationStringFormatParser parser) return parser.Parse(formulationString);
            throw new ArgumentException($"{formats} doesn't implement {nameof(ILocalizationStringFormatParser)}.");
        }

        /// <summary>
        /// Add format to map.
        /// </summary>
        /// <param name="formats"></param>
        /// <param name="format"></param>
        /// <returns><paramref name="formats"/></returns>
        public static IDictionary<string, ILocalizationStringFormat> Add(this IDictionary<string, ILocalizationStringFormat> formats, ILocalizationStringFormat format)
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
        public static IDictionary<string, ILocalizationStringFormat> AddRange(this IDictionary<string, ILocalizationStringFormat> formats, IEnumerable<ILocalizationStringFormat> formatsToAdd)
        {
            foreach(var format in formatsToAdd)
                formats[format.Name] = format;
            return formats;
        }

        /// <summary>
        /// Create clone 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static LocalizationStringFormats Clone(this IReadOnlyDictionary<string, ILocalizationStringFormat> map)
        {
            LocalizationStringFormats result = new LocalizationStringFormats();
            foreach (var line in map)
                result[line.Key] = line.Value;
            return result;
        }
    }
}
