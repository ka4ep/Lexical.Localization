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
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class LocalizationReaderExtensions_
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
        public static IEnumerable<KeyValuePair<string, string>> ReadStringLines(this ILocalizationFileFormat fileFormat, string srcFilename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_stringlines;
            try
            {
                if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcFilename.ReadText(), namePolicy).ToStringLines(namePolicy);
                if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcFilename.ReadStream(), namePolicy).ToStringLines(namePolicy);
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
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(this ILocalizationFileFormat fileFormat, string srcFilename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return no_keylines;
            try
            {
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcFilename.ReadText(), namePolicy).ToKeyLines(true);
                if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcFilename.ReadStream(), namePolicy).ToKeyLines(true);
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
        public static IKeyTree ReadKeyTree(this ILocalizationFileFormat fileFormat, string srcFilename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
        {
            if (!throwIfNotFound && !File.Exists(srcFilename)) return null;
            try
            {
                if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcFilename.ReadText(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcFilename.ReadText(), namePolicy).ToKeyTree(namePolicy);
                if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcFilename.ReadStream(), namePolicy);
                if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcFilename.ReadStream(), namePolicy).ToKeyTree(namePolicy);
                if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcFilename.ReadText(), namePolicy).ToKeyTree(namePolicy);
                if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcFilename.ReadStream(), namePolicy).ToKeyTree(namePolicy);
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
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> FileReaderAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileReaderKeyLines(fileFormat, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> FileReaderAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileReaderKeyTree(fileFormat, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> FileReaderAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => new LocalizationFileReaderStringLines(fileFormat, filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset FileAsset(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.FileReaderAsKeyTree(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.FileReaderAsKeyLines(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.FileReaderAsStringLines(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy)).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource FileAssetSource(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.FileReaderAsKeyTree(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.FileReaderAsKeyLines(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.FileReaderAsStringLines(filename, namePolicy, throwIfNotFound).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, filename);
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
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].ReadKeyLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Read file into a tree format.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IKeyTree ReadKeyTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].ReadKeyTree(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Read file into strings file.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<string, string>> ReadStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].ReadStringLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> FileReaderAsKeyLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].FileReaderAsKeyLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<IKeyTree> FileReaderAsKeyTree(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].FileReaderAsKeyTree(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens <paramref name="filename"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IEnumerable<KeyValuePair<string, string>> FileReaderAsStringLines(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].FileReaderAsStringLines(filename, namePolicy, throwIfNotFound);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>reloadable localization asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset FileAsset(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].FileAsset(filename, namePolicy, throwIfNotFound, prefix, suffix);

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAssetSource FileAssetSource(this IReadOnlyDictionary<string, ILocalizationFileFormat> fileFormatProvider, string filename, IAssetKeyNamePolicy namePolicy = default, bool throwIfNotFound = true, IAssetKey prefix = null, IAssetKey suffix = null)
            => fileFormatProvider[LocalizationReaderMap.GetExtension(filename)].FileAssetSource(filename, namePolicy, throwIfNotFound, prefix, suffix);

        static IKeyTree[] no_trees = new IKeyTree[0];
        static KeyValuePair<IAssetKey, string>[] no_keylines = new KeyValuePair<IAssetKey, string>[0];
        static KeyValuePair<string, string>[] no_stringlines = new KeyValuePair<string, string>[0];

    }

}
