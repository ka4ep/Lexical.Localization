//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationFileProviderReaderExtensions
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
        public static IEnumerable<KeyValuePair<IAssetKey, string>> FileProviderReaderAsKeyLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderReaderKeyLines(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> FileProviderReaderAsKeyTree(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderReaderKeyTree(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> FileProviderReaderAsStringLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileProviderReaderStringLines(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset</returns>
        public static IAsset FileProviderAsset(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.FileProviderReaderAsKeyTree(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.FileProviderReaderAsStringLines(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy)).Load();
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
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource FileProviderAssetSource(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filepath);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filepath);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.FileProviderReaderAsStringLines(fileProvider, filepath, namePolicy, throwIfNotFound).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, filepath);
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
        public static IEnumerable<KeyValuePair<IAssetKey, string>> FileProviderReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filepath)].FileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy, throwIfNotFound);

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
        public static IEnumerable<IKeyTree> FileProviderReaderAsKeyTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filepath)].FileProviderReaderAsKeyTree(fileProvider, filepath, namePolicy, throwIfNotFound);

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
        public static IEnumerable<KeyValuePair<string, string>> FileProviderReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filepath)].FileProviderReaderAsStringLines(fileProvider, filepath, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileProviderAsset(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filepath)].FileProviderAsset(fileProvider, filepath, namePolicy, throwIfNotFound, prefix, suffix);

        /// <summary>
        /// Create localization asset that reads from FileProvider.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAssetSource FileProviderAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filepath)].FileProviderAssetSource(fileProvider, filepath, namePolicy, throwIfNotFound, prefix, suffix);

    }
}
