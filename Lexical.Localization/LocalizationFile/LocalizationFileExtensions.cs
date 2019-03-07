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

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static class LocalizationFileExtensions_
    {
        /// <summary>
        /// Read file into assetkey lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadFileAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadKeyLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Read file into a tree format.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IKeyTree ReadFileAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadKeyTree(fileFormat, fs, namePolicy);
        }

        /// <summary>
        /// Read file into strings file.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadFileAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadStringLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateFileReaderAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateFileReaderAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyTree(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateFileReaderAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderStringLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateEmbeddedReaderAsKeyLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderKeyLines(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateEmbeddedReaderAsKeyTree(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderKeyTree(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateEmbeddedReaderAsStringLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderStringLines(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset CreateFileAsset(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.CreateFileReaderAsKeyTree(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.CreateFileReaderAsKeyLines(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.CreateFileReaderAsStringLines(filename, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy)).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource CreateFileAssetSource(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.CreateFileReaderAsKeyTree(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.CreateFileReaderAsKeyLines(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.CreateFileReaderAsStringLines(filename, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, filename);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource CreateEmbeddedAssetSource(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsKeyLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsKeyLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsStringLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, resourceName);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAsset CreateAsset(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyTree(stream, namePolicy).ToKeyLines().AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyLines(stream, namePolicy).AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationStringAsset(fileFormat.ReadStringLines(stream, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy), namePolicy);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="streamSource"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAssetSource CreateAssetSource(this ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
            => new StreamProviderAssetSource(fileFormat, streamSource, namePolicy, prefix, suffix);

    }
    
}

namespace Lexical.Localization.Ms.Extensions
{
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static class LocalizationFileExtensions_
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
            => new FileProviderReaderKeyLines(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateFileProviderReaderAsKeyTree(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new FileProviderReaderKeyTree(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filepath"/> from <paramref name="fileProvider"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateFileProviderReaderAsStringLines(this ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new FileProviderReaderStringLines(fileFormat, fileProvider, filepath, namePolicy);

        /// <summary>
        /// Create localization asset source that reads FileProvider resource at <paramref name="filepath"/>.
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
