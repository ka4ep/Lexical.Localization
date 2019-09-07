//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILineFileFormat"/>.
    /// </summary>
    public static partial class LineReaderExtensions
    {
        /// <summary>
        /// Read strings from <paramref name="srcFilename"/> file source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcFilename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static IEnumerable<KeyValuePair<string, IString>> ReadUnformedLines(this ILineFileFormat fileFormat, string srcFilename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_stringlines;
            try
            {
                if (fileFormat is IUnformedLineTextReader r5) return r5.ReadUnformedLines(srcFilename.ReadText(), lineFormat);
                if (fileFormat is IUnformedLineStreamReader r6) return r6.ReadUnformedLines(srcFilename.ReadStream(), lineFormat);
                if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcFilename.ReadText(), lineFormat).ToStringLines(lineFormat);
                if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcFilename.ReadStream(), lineFormat).ToStringLines(lineFormat);
                if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), lineFormat).ToStringLines(lineFormat);
                if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), lineFormat).ToStringLines(lineFormat);
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
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>enumerable of lines</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static IEnumerable<ILine> ReadLines(this ILineFileFormat fileFormat, string srcFilename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_keylines;
            try
            {
                if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcFilename.ReadText(), lineFormat);
                if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcFilename.ReadStream(), lineFormat);
                if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), lineFormat).ToLines();
                if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), lineFormat).ToLines();
                if (fileFormat is IUnformedLineTextReader r5) return r5.ReadUnformedLines(srcFilename.ReadText(), lineFormat).ToLines(lineFormat);
                if (fileFormat is IUnformedLineStreamReader r6) return r6.ReadUnformedLines(srcFilename.ReadStream(), lineFormat).ToLines(lineFormat);
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
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise null is returned</param>
        /// <returns>tree or null if file was not found and error not thrown</returns>
        /// <exception cref="FileNotFoundException">thrown if file was not found and <paramref name="throwIfNotFound"/> is true</exception>
        /// <exception cref="IOException">on io error</exception>
        public static ILineTree ReadLineTree(this ILineFileFormat fileFormat, string srcFilename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return null;
            try
            {
                if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcFilename.ReadText(), lineFormat);
                if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcFilename.ReadText(), lineFormat).ToLineTree(lineFormat);
                if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcFilename.ReadStream(), lineFormat);
                if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcFilename.ReadStream(), lineFormat).ToLineTree(lineFormat);
                if (fileFormat is IUnformedLineTextReader r5) return r5.ReadUnformedLines(srcFilename.ReadText(), lineFormat).ToLineTree(lineFormat);
                if (fileFormat is IUnformedLineStreamReader r6) return r6.ReadUnformedLines(srcFilename.ReadStream(), lineFormat).ToLineTree(lineFormat);
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
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static KeyLineFileSource FileReader(this ILineFileFormat fileFormat, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new KeyLineFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LineTreeFileSource FileReaderAsLineTree(this ILineFileFormat fileFormat, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new LineTreeFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static StringLineFileSource FileReaderAsUnformedLines(this ILineFileFormat fileFormat, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new StringLineFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset FileAsset(this ILineFileFormat fileFormat, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileReaderAsLineTree(filename, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileReader(filename, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileReaderAsUnformedLines(filename, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LineFileSource FileAssetSource(this ILineFileFormat fileFormat, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new LineTreeFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new KeyLineFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringLineFileSource(fileFormat, null, filename, lineFormat, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LineFileSource FileAssetSource(this ILineFileFormat fileFormat, string path, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new LineTreeFileSource(fileFormat, path, filename, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new KeyLineFileSource(fileFormat, path, filename, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringLineFileSource(fileFormat, path, filename, lineFormat, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read file into Line lines.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<ILine> ReadLines(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].ReadLines(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Read file into a tree format.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static ILineTree ReadLineTree(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].ReadLineTree(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Read file into strings file.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<string, IString>> ReadUnformedLines(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].ReadUnformedLines(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static KeyLineFileSource FileReader(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].FileReader(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineTreeFileSource FileReaderAsLineTree(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].FileReaderAsLineTree(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static StringLineFileSource FileReaderAsUnformedLines(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].FileReaderAsUnformedLines(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileAsset(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].FileAsset(filename, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineFileSource FileAssetSource(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, string filename, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filename)].FileAssetSource(filename, lineFormat, throwIfNotFound);

        static ILineTree[] no_trees = new ILineTree[0];
        static ILine[] no_keylines = new ILine[0];
        static KeyValuePair<string, IString>[] no_stringlines = new KeyValuePair<string, IString>[0];

    }

}
