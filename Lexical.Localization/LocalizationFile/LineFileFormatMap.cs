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
    public class LineFileFormatMap : ConcurrentDictionary<string, ILineFileFormat>
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
        public LineFileFormatMap() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Create new file format map.
        /// </summary>
        /// <param name="fileFormats"></param>
        public LineFileFormatMap(params ILineFileFormat[] fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(fileFormats);
        }

        /// <summary>
        /// Create new file format map.
        /// </summary>
        /// <param name="fileFormats"></param>
        public LineFileFormatMap(IEnumerable<ILineFileFormat> fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.AddRange(fileFormats);
        }
    }

    /// <summary>
    /// Extenions for <see cref="LineFileFormatMap"/>.
    /// </summary>
    public static partial class LineFileFormatMapExtensions
    {
        /// <summary>
        /// All supported extensions.
        /// </summary>
        public static IEnumerable<string> Extensions(this IReadOnlyDictionary<string, ILineFileFormat> map)
            => map.Values.Select(f => f.Extension);

        /// <summary>
        /// Infer and get <see cref="ILineFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filename"></param>
        /// <returns>file format</returns>
        /// <exception cref="ArgumentException">thrown if fileformat was not found</exception>
        public static ILineFileFormat GetFormatByFilename(this IReadOnlyDictionary<string, ILineFileFormat> map, string filename)
        {
            string ext = LineFileFormatMap.GetExtension(filename);
            ILineFileFormat fileFormat;
            if (ext != null && map.TryGetValue(ext, out fileFormat)) return fileFormat;
            throw new ArgumentException("Could not resolve file format for filename \"{filename}\"");
        }

        /// <summary>
        /// Try to infer <see cref="ILineFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filename"></param>
        /// <returns>file format or null</returns>
        public static ILineFileFormat TryGetFormatByFilename(this IReadOnlyDictionary<string, ILineFileFormat> map, string filename)
        {
            string ext = LineFileFormatMap.GetExtension(filename);
            ILineFileFormat fileFormat;
            if (ext != null && map.TryGetValue(ext, out fileFormat)) return fileFormat;
            return null;
        }

        /// <summary>
        /// Try get file format by extension. 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="extension">extension to search, case insensitive</param>
        /// <returns>file format or null</returns>
        public static ILineFileFormat TryGet(this IReadOnlyDictionary<string, ILineFileFormat> map, string extension)
        {
            ILineFileFormat result = null;
            map.TryGetValue(extension, out result);
            return result;
        }

        /// <summary>
        /// Add <paramref name="fileFormat"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        public static IDictionary<string, ILineFileFormat> Add(this IDictionary<string, ILineFileFormat> map, ILineFileFormat fileFormat)
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
        public static IDictionary<string, ILineFileFormat> AddRange(this IDictionary<string, ILineFileFormat> map, IEnumerable<ILineFileFormat> fileFormats)
        {
            foreach (ILineFileFormat fileFormat in fileFormats)
                map.Add(fileFormat);
            return map;
        }

        /// <summary>
        /// Add <paramref name="fileFormats"/>.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileFormats"></param>
        /// <returns></returns>
        public static IDictionary<string, ILineFileFormat> AddRange(this IDictionary<string, ILineFileFormat> map, params ILineFileFormat[] fileFormats)
        {
            foreach (ILineFileFormat fileFormat in fileFormats)
                map.Add(fileFormat);
            return map;
        }

        /// <summary>
        /// Create clone 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static LineFileFormatMap Clone(this IReadOnlyDictionary<string, ILineFileFormat> map)
            => new LineFileFormatMap(map.Values);
    }
}
