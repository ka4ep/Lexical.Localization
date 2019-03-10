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
    public static class LocalizationFileProviderReaderExtensions
    {
        /// <summary>
        /// Create reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateFileProviderReaderAsKeyLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new LocalizationFileProviderReaderKeyLines(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateFileProviderReaderAsKeyTree(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new LocalizationFileProviderReaderKeyTree(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateFileProviderReaderAsStringLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new LocalizationFileProviderReaderStringLines(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset</returns>
        public static IAsset CreateFileProviderAsset(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.CreateFileProviderReaderAsKeyTree(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.CreateFileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.CreateFileProviderReaderAsStringLines(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy)).Load();
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
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource CreateFileProviderAssetSource(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.CreateFileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filepath);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.CreateFileProviderReaderAsKeyLines(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filepath);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.CreateFileProviderReaderAsStringLines(fileProvider, filepath, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, filepath);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }
    }
}
