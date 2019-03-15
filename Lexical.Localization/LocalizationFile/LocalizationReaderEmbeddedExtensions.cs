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
        public static IEnumerable<KeyValuePair<IAssetKey, string>> EmbeddedReaderAsKeyLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedReaderKeyLines(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> EmbeddedReaderAsKeyTree(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedReaderKeyTree(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> EmbeddedReaderAsStringLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationEmbeddedReaderStringLines(fileFormat, asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset from embedded resource<paramref name="resourceName"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset EmbeddedAsset(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.EmbeddedReaderAsKeyTree(asm, resourceName, namePolicy, throwIfNotFound)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.EmbeddedReaderAsKeyLines(asm, resourceName, namePolicy, throwIfNotFound)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.EmbeddedReaderAsStringLines(asm, resourceName, namePolicy, throwIfNotFound)).Load();
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
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static IAssetSource EmbeddedAssetSource(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.EmbeddedReaderAsKeyLines(asm, resourceName, namePolicy, throwIfNotFound).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.EmbeddedReaderAsKeyLines(asm, resourceName, namePolicy, throwIfNotFound).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.EmbeddedReaderAsStringLines(asm, resourceName, namePolicy, throwIfNotFound).ToAssetSource(namePolicy, resourceName);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> EmbeddedReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(resourceName)].EmbeddedReaderAsKeyLines(asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<IKeyTree> EmbeddedReaderAsKeyTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(resourceName)].EmbeddedReaderAsKeyTree(asm, resourceName, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<string, string>> EmbeddedReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(resourceName)].EmbeddedReaderAsStringLines(asm, resourceName, namePolicy, throwIfNotFound);

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
            => fileFormatProvider[LocalizationReaderMap.GetExtension(resourceName)].EmbeddedAsset(asm, resourceName, namePolicy, throwIfNotFound);

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
        public static IAssetSource EmbeddedAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(resourceName)].EmbeddedAssetSource(asm, resourceName, namePolicy, throwIfNotFound);

    }

}
