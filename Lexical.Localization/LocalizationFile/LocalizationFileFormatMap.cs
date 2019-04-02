//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.1.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Collection of localization readers.
    /// </summary>
    public class LocalizationFileFormatMap : ConcurrentDictionary<string, ILocalizationFileFormat>
    {
        /// <summary>
        /// Get file extension without "."
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetExtension(string filename)
        {
            if (filename == null) return null;
            int ix = filename.LastIndexOf('.');
            if (ix < 0) return "";
            return filename.Substring(ix + 1);
        }

        /// <summary>
        /// Create new file format map.
        /// </summary>
        public LocalizationFileFormatMap() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Create new file format map.
        /// </summary>
        /// <param name="fileFormats"></param>
        public LocalizationFileFormatMap(params ILocalizationFileFormat[] fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(fileFormats);
        }

        /// <summary>
        /// Create new file format map.
        /// </summary>
        /// <param name="fileFormats"></param>
        public LocalizationFileFormatMap(IEnumerable<ILocalizationFileFormat> fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(fileFormats);
        }
    }

    /// <summary>
    /// Extenions for <see cref="LocalizationFileFormatMap"/>.
    /// </summary>
    public static partial class LocalizationFileFormatMapExtensions_
    {
        /// <summary>
        /// All supported extensions.
        /// </summary>
        public static IEnumerable<string> Extensions(this IReadOnlyDictionary<string, ILocalizationFileFormat> map)
            => map.Values.Select(f => f.Extension);

        /// <summary>
        /// Infer and get <see cref="ILocalizationFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filename"></param>
        /// <returns>file format</returns>
        /// <exception cref="ArgumentException">thrown if fileformat was not found</exception>
        public static ILocalizationFileFormat GetFormatByFilename(this IReadOnlyDictionary<string, ILocalizationFileFormat> map, string filename)
        {
            string ext = LocalizationFileFormatMap.GetExtension(filename);
            ILocalizationFileFormat fileFormat;
            if (ext != null && map.TryGetValue(ext, out fileFormat)) return fileFormat;
            throw new ArgumentException("Could not resolve file format for filename \"{filename}\"");
        }

        /// <summary>
        /// Try to infer <see cref="ILocalizationFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filename"></param>
        /// <returns>file format or null</returns>
        public static ILocalizationFileFormat TryGetFormatByFilename(this IReadOnlyDictionary<string, ILocalizationFileFormat> map, string filename)
        {
            string ext = LocalizationFileFormatMap.GetExtension(filename);
            ILocalizationFileFormat fileFormat;
            if (ext != null && map.TryGetValue(ext, out fileFormat)) return fileFormat;
            return null;
        }

        /// <summary>
        /// Try get file format by extension. 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="extension">extension to search, case insensitive</param>
        /// <returns>file format or null</returns>
        public static ILocalizationFileFormat TryGet(this IReadOnlyDictionary<string, ILocalizationFileFormat> map, string extension)
        {
            ILocalizationFileFormat result = null;
            map.TryGetValue(extension, out result);
            return result;
        }

        /// <summary>
        /// Add <paramref name="fileFormat"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        public static IDictionary<string, ILocalizationFileFormat> Add(this IDictionary<string, ILocalizationFileFormat> map, ILocalizationFileFormat fileFormat)
        {
            string ext = fileFormat.Extension;
            if (ext == null) throw new ArgumentNullException("Extension");
            if (map.ContainsKey(ext)) throw new InvalidOperationException($"Already contains file format for {ext}.");
            map[ext] = fileFormat;
            return map;
        }

        /// <summary>
        /// Add <paramref name="fileFormats"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileFormats"></param>
        /// <returns></returns>
        public static IDictionary<string, ILocalizationFileFormat> AddRange(this IDictionary<string, ILocalizationFileFormat> map, IEnumerable<ILocalizationFileFormat> fileFormats)
        {
            foreach (ILocalizationFileFormat fileFormat in fileFormats)
                map.Add(fileFormat);
            return map;
        }

        /// <summary>
        /// Add <paramref name="fileFormats"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileFormats"></param>
        /// <returns></returns>
        public static IDictionary<string, ILocalizationFileFormat> AddRange(this IDictionary<string, ILocalizationFileFormat> map, params ILocalizationFileFormat[] fileFormats)
        {
            foreach (ILocalizationFileFormat fileFormat in fileFormats)
                map.Add(fileFormat);
            return map;
        }

        /// <summary>
        /// Create clone 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static LocalizationFileFormatMap Clone(this IReadOnlyDictionary<string, ILocalizationFileFormat> map)
            => new LocalizationFileFormatMap(map.Values);
    }
}
