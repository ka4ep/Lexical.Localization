//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.1.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lexical.Localization.LocalizationFile
{
    /// <summary>
    /// Represents a localization file format.
    /// </summary>
    public interface ILocalizationFileFormat
    {
        string Extension { get; }
    }

    /// <summary>
    /// Signals that file format can be read.
    /// </summary>
    public interface ILocalizationFileReader : ILocalizationFileFormat
    {
    }

    /// <summary>
    /// Reader that can open a read container from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationFileStreamReader : ILocalizationFileReader
    {
        /// <summary>
        /// Open read container from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <returns>readable container that must be disposed</returns>
        /// <exception cref="IOException"></exception>
        ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can open a read container from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationFileTextReader : ILocalizationFileReader
    {
        /// <summary>
        /// Open read container from a text reader.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <returns>readable container that must be disposed</returns>
        /// <exception cref="IOException"></exception>
        ILocalizationFileReadable OpenText(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Signals that file format can be written to.
    /// </summary>
    public interface ILocalizationFileWriter : ILocalizationFileFormat
    {
    }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILocalizationFileStreamWriter : ILocalizationFileWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <returns>writable container, disposing flushes closes the file.</returns>
        ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILocalizationFileTextWriter : ILocalizationFileWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <returns>writable container, disposing flushes closes the file.</returns>
        ILocalizationFileWritable CreateText(TextWriter text, IAssetKeyNamePolicy namePolicy = default);
    }

    public static class LocalizationFileFormatExtensions
    {
        public static ILocalizationFileReadable OpenText(this ILocalizationFileTextReader reader, TextReader text, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections)
            => reader.OpenText(text, namePolicy).DecorateSections(initialSections);
        public static ILocalizationFileReadable OpenStream(this ILocalizationFileStreamReader reader, Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections)
            => reader.OpenStream(stream, namePolicy).DecorateSections(initialSections);
    }

    public class LocalizationFileFormatMap : Dictionary<string, ILocalizationFileFormat>
    {
        static LocalizationFileFormatMap singleton = new LocalizationFileFormatMap(IniFileFormat.Singleton, JsonFileFormat.Singleton, ResourcesFileFormat.Singleton, ResXFileFormat.Singleton, XmlFileFormat.Singleton);
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
