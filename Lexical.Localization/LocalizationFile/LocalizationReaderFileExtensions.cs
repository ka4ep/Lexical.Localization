//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationReaderExtensions
    {
        /// <summary>
        /// Read strings from <paramref name="srcFilename"/> file source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcFilename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(this ILocalizationFileFormat fileFormat, string srcFilename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_stringlines;
            try
            {
                if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationLineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationLineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), namePolicy).ToStringLines(namePolicy);
            }
            catch (FileNotFoundException) when (!throwIfNotFound)
            {
                return no_stringlines;
            }
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="srcFilename"/> file source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcFilename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>enumerable of lines</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static IEnumerable<KeyValuePair<ILine, IFormulationString>> ReadKeyLines(this ILocalizationFileFormat fileFormat, string srcFilename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_keylines;
            try
            {
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationLineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), namePolicy).ToKeyLines();
                if (fileFormat is ILocalizationLineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), namePolicy).ToKeyLines();
                if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcFilename.ReadText(), namePolicy).ToKeyLines(namePolicy);
                if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcFilename.ReadStream(), namePolicy).ToKeyLines(namePolicy);
            }
            catch (FileNotFoundException) when (!throwIfNotFound)
            {
                return no_keylines;
            }
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="srcFilename"/> file source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcFilename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise null is returned</param>
        /// <returns>tree or null if file was not found and error not thrown</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static ILineTree ReadLineTree(this ILocalizationFileFormat fileFormat, string srcFilename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return null;
            try
            {
                if (fileFormat is ILocalizationLineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy).ToLineTree(namePolicy);
                if (fileFormat is ILocalizationLineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy).ToLineTree(namePolicy);
                if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcFilename.ReadText(), namePolicy).ToLineTree(namePolicy);
                if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcFilename.ReadStream(), namePolicy).ToLineTree(namePolicy);
            }
            catch (FileNotFoundException) when (!throwIfNotFound)
            {
                return null;
            }
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename">relative non-rooted filepath, or rooted absolute file path</param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationFileKeyLinesSource FileReaderAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileKeyLinesSource(fileFormat, null, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LocalizationFileLineTreeSource FileReaderAsLineTree(this ILocalizationFileFormat fileFormat, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileLineTreeSource(fileFormat, null, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationFileStringLinesSource FileReaderAsStringLines(this ILocalizationFileFormat fileFormat, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileStringLinesSource(fileFormat, null, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset FileAsset(this ILocalizationFileFormat fileFormat, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationLineTreeTextReader || fileFormat is ILocalizationLineTreeStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileReaderAsLineTree(filename, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileReaderAsKeyLines(filename, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileReaderAsStringLines(filename, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LocalizationFileSource FileAssetSource(this ILocalizationFileFormat fileFormat, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationLineTreeTextReader || fileFormat is ILocalizationLineTreeStreamReader)
            {
                return new LocalizationFileLineTreeSource(fileFormat, null, filename, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationFileKeyLinesSource(fileFormat, null, filename, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationFileStringLinesSource(fileFormat, null, filename, namePolicy, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LocalizationFileSource FileAssetSource(this ILocalizationFileFormat fileFormat, string path, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationLineTreeTextReader || fileFormat is ILocalizationLineTreeStreamReader)
            {
                return new LocalizationFileLineTreeSource(fileFormat, path, filename, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationFileKeyLinesSource(fileFormat, path, filename, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationFileStringLinesSource(fileFormat, path, filename, namePolicy, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read file into assetkey lines.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<ILine, IFormulationString>> ReadKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].ReadKeyLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Read file into a tree format.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static ILineTree ReadLineTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].ReadLineTree(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Read file into strings file.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].ReadStringLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileKeyLinesSource FileReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].FileReaderAsKeyLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileLineTreeSource FileReaderAsLineTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].FileReaderAsLineTree(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileStringLinesSource FileReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].FileReaderAsStringLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileAsset(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].FileAsset(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileSource FileAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, ILinePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filename)].FileAssetSource(filename, namePolicy, throwIfNotFound);

        static ILineTree[] no_trees = new ILineTree[0];
        static KeyValuePair<ILine, IFormulationString>[] no_keylines = new KeyValuePair<ILine, IFormulationString>[0];
        static KeyValuePair<string, IFormulationString>[] no_stringlines = new KeyValuePair<string, IFormulationString>[0];

    }

}
