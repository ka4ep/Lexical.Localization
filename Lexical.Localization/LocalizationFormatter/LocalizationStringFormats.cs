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
    public class LocalizationStringFormats : ConcurrentDictionary<string, ILocalizationStringFormat>, ILocalizationStringFormats
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
    /// Extensions for <see cref="LocalizationStringFormats"/>.
    /// </summary>
    public static partial class LocalizationStringFormatsExtensions
    {
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
