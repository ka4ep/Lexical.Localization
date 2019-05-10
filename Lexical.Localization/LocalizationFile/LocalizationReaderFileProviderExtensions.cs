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
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationReaderFileProviderExtensions
    {
        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationFileProviderKeyLinesSource FileProviderReaderAsKeyLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderKeyLinesSource(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LocalizationFileProviderReaderLineTree FileProviderReaderAsLineTree(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderReaderLineTree(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationFileProviderStringLinesSource FileProviderReaderAsStringLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderStringLinesSource(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset</returns>
        public static IAsset FileProviderAsset(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationLineTreeTextReader || fileFormat is ILocalizationLineTreeStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileProviderReaderAsLineTree(fileProvider, filepath, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.FileProviderReaderAsStringLines(fileProvider, filepath, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset that reads from FileProvider.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LocalizationFileProviderSource FileProviderAssetSource(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationLineTreeTextReader || fileFormat is ILocalizationLineTreeStreamReader)
            {
                return new LocalizationFileProviderKeyLinesSource(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationFileProviderKeyLinesSource(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationFileProviderStringLinesSource(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileProviderKeyLinesSource FileProviderReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filepath)].FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileProviderReaderLineTree FileProviderReaderAsLineTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filepath)].FileProviderReaderAsLineTree(fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileProviderStringLinesSource FileProviderReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filepath)].FileProviderReaderAsStringLines(fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileProviderAsset(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filepath)].FileProviderAsset(fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads from FileProvider.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationFileProviderSource FileProviderAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy = default, bool throwIfNotFound = true, ILine prefix = null, ILine suffix = null)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(filepath)].FileProviderAssetSource(fileProvider, filepath, namePolicy, throwIfNotFound);

    }
}
