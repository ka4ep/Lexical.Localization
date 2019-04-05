//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationReaderExtensions_
    {
        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationEmbeddedKeyLinesSource EmbeddedReaderAsKeyLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedKeyLinesSource(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LocalizationEmbeddedKeyTreeSource EmbeddedReaderAsKeyTree(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedKeyTreeSource(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LocalizationEmbeddedStringLinesSource EmbeddedReaderAsStringLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedStringLinesSource(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset from embedded resource<paramref name="resourceName"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset EmbeddedAsset(this ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.EmbeddedReaderAsKeyTree(assembly, resourceName, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.EmbeddedReaderAsKeyLines(assembly, resourceName, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationAsset().Add(fileFormat.EmbeddedReaderAsStringLines(assembly, resourceName, namePolicy, throwIfNotFound), namePolicy).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LocalizationEmbeddedSource EmbeddedAssetSource(this ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationEmbeddedKeyTreeSource(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationEmbeddedKeyLinesSource(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationEmbeddedStringLinesSource(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationEmbeddedKeyLinesSource EmbeddedReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(resourceName)].EmbeddedReaderAsKeyLines(assembly, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationEmbeddedKeyTreeSource EmbeddedReaderAsKeyTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(resourceName)].EmbeddedReaderAsKeyTree(assembly, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationEmbeddedStringLinesSource EmbeddedReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(resourceName)].EmbeddedReaderAsStringLines(assembly, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset from embedded resource<paramref name="resourceName"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset EmbeddedAsset(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(resourceName)].EmbeddedAsset(asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LocalizationEmbeddedSource EmbeddedAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationFileFormatMap.GetExtension(resourceName)].EmbeddedAssetSource(asm, resourceName, namePolicy, throwIfNotFound);

    }

}
