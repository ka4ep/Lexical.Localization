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
    /// Collection of file formats
    /// </summary>
    public class LocalizationFileFormatMap : ConcurrentDictionary<string, ILocalizationFileFormat>
    {
        static LocalizationFileFormatMap singleton = new LocalizationFileFormatMap( XmlLocalizationReader.Instance, ResXLocalizationReader.Instance, ResourcesLocalizationReader.Instance, JsonLocalizationReader.Instance, IniLocalizationReader.Instance );

        /// <summary>
        /// Global singleton instance.
        /// </summary>
        public static LocalizationFileFormatMap Singleton => singleton;

        /// <summary>
        /// All supported extensions.
        /// </summary>
        public IEnumerable<string> Extensions => Values.Select(f => f.Extension);

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
        /// Infer and get <see cref="ILocalizationFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>file format</returns>
        /// <exception cref="ArgumentException">thrown if fileformat was not found</exception>
        public ILocalizationFileFormat GetFormatByFilename(string filename)
        {
            string ext = GetExtension(filename);
            ILocalizationFileFormat fileFormat;
            if (ext != null && TryGetValue(ext, out fileFormat)) return fileFormat;
            throw new ArgumentException("Could not resolve file format for filename \"{filename}\"");
        }

        /// <summary>
        /// Try to infer <see cref="ILocalizationFileFormat"/> from <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>file format or null</returns>
        public ILocalizationFileFormat TryGetFormatByFilename(string filename)
        {
            string ext = GetExtension(filename);
            ILocalizationFileFormat fileFormat;
            if (ext != null && TryGetValue(ext, out fileFormat)) return fileFormat;
            return null;
        }

        public LocalizationFileFormatMap() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public LocalizationFileFormatMap(params ILocalizationFileFormat[] fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            AddRange(fileFormats);
        }

        public LocalizationFileFormatMap(IEnumerable<ILocalizationFileFormat> fileFormats) : base(StringComparer.InvariantCultureIgnoreCase)
        {
            AddRange(fileFormats);
        }

        /// <summary>
        /// Try get file format by extension. 
        /// </summary>
        /// <param name="extension">extension to search, case insensitive</param>
        /// <returns>file format or null</returns>
        public ILocalizationFileFormat TryGet(string extension)
        {
            ILocalizationFileFormat result = null;
            base.TryGetValue(extension, out result);
            return result;
        }

        public LocalizationFileFormatMap Add(ILocalizationFileFormat fileFormat)
        {
            string ext = fileFormat.Extension;
            if (ext == null) throw new ArgumentNullException("Extension");
            if (this.ContainsKey(ext)) throw new InvalidOperationException($"Already contains file format for {ext}.");
            this[ext] = fileFormat;
            return this;
        }

        public LocalizationFileFormatMap AddRange(IEnumerable<ILocalizationFileFormat> fileFormats)
        {
            foreach (ILocalizationFileFormat fileFormat in fileFormats)
                Add(fileFormat);
            return this;
        }

        public LocalizationFileFormatMap AddRange(params ILocalizationFileFormat[] fileFormats)
        {
            foreach (ILocalizationFileFormat fileFormat in fileFormats)
                Add(fileFormat);
            return this;
        }
    }
}
