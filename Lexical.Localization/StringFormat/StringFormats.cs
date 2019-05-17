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
    public class StringFormats : ConcurrentDictionary<string, IStringFormat>, IStringFormatMap
    {
        private static StringFormats instance;

        static StringFormats()
        {
            instance = new StringFormats();
            instance["charp"] = CSharpFormat.Instance;
            instance[CSharpFormat.Instance.Name] = CSharpFormat.Instance;
        }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IReadOnlyDictionary<string, IStringFormat> Instance => instance;

        /// <summary>
        /// Create new string format map.
        /// </summary>
        public StringFormats() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Create new string format map.
        /// </summary>
        /// <param name="stringFormats"></param>
        public StringFormats(params IStringFormat[] stringFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(stringFormats);
        }

        /// <summary>
        /// Create new string format map.
        /// </summary>
        /// <param name="stringFormats"></param>
        public StringFormats(IEnumerable<IStringFormat> stringFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(stringFormats);
        }
    }

    /// <summary>
    /// Extensions for <see cref="StringFormats"/>.
    /// </summary>
    public static partial class LocalizationStringFormatsExtensions
    {
        /// <summary>
        /// Create clone 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static StringFormats Clone(this IReadOnlyDictionary<string, IStringFormat> map)
        {
            StringFormats result = new StringFormats();
            foreach (var line in map)
                result[line.Key] = line.Value;
            return result;
        }
    }
}
