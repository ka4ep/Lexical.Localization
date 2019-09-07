//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILineFileFormat"/>.
    /// </summary>
    public static partial class LineReaderFileProviderExtensions
    {
        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static KeyLineFileProviderSource FileProviderReader(this ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new KeyLineFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LineTreeFileProviderSource FileProviderReaderAsLineTree(this ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new LineTreeFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static StringLineFileProviderSource FileProviderReaderAsUnformedLines(this ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new StringLineFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset</returns>
        public static IAsset FileProviderAsset(this ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileProviderReaderAsLineTree(fileProvider, filepath, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileProviderReader(fileProvider, filepath, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringAsset().Add(fileFormat.FileProviderReaderAsUnformedLines(fileProvider, filepath, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset that reads from FileProvider.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LineFileProviderSource FileProviderAssetSource(this ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new KeyLineFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new KeyLineFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringLineFileProviderSource(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static KeyLineFileProviderSource FileProviderReader(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filepath)].FileProviderReader(fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineTreeFileProviderSource FileProviderReaderAsLineTree(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filepath)].FileProviderReaderAsLineTree(fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static StringLineFileProviderSource FileProviderReaderAsUnformedLines(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filepath)].FileProviderReaderAsUnformedLines(fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileProviderAsset(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filepath)].FileProviderAsset(fileProvider, filepath, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads from FileProvider.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineFileProviderSource FileProviderAssetSource(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(filepath)].FileProviderAssetSource(fileProvider, filepath, lineFormat, throwIfNotFound);

    }
}
