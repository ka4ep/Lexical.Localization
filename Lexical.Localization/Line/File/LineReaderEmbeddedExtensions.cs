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
using Lexical.Localization.Asset;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILineFileFormat"/>.
    /// </summary>
    public static partial class LineReaderExtensions
    {
        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LineEmbeddedKeyLinesSource EmbeddedReader(this ILineFileFormat fileFormat, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new LineEmbeddedKeyLinesSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        public static LineEmbeddedLineTreeSource EmbeddedReaderAsLineTree(this ILineFileFormat fileFormat, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new LineEmbeddedLineTreeSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        public static LineEmbeddedStringLinesSource EmbeddedReaderAsStringLines(this ILineFileFormat fileFormat, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => new LineEmbeddedStringLinesSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset from embedded resource<paramref name="resourceName"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset EmbeddedAsset(this ILineFileFormat fileFormat, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new StringAsset().Add(fileFormat.EmbeddedReaderAsLineTree(assembly, resourceName, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new StringAsset().Add(fileFormat.EmbeddedReader(assembly, resourceName, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new StringAsset().Add(fileFormat.EmbeddedReaderAsStringLines(assembly, resourceName, lineFormat, throwIfNotFound), lineFormat).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        public static LineEmbeddedSource EmbeddedAssetSource(this ILineFileFormat fileFormat, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new LineEmbeddedLineTreeSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new LineEmbeddedKeyLinesSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);
            }
            else if (fileFormat is IUnformedLineTextReader || fileFormat is IUnformedLineStreamReader)
            {
                return new LineEmbeddedStringLinesSource(fileFormat, assembly, resourceName, lineFormat, throwIfNotFound);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineEmbeddedKeyLinesSource EmbeddedReader(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(resourceName)].EmbeddedReader(assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>tree</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineEmbeddedLineTreeSource EmbeddedReaderAsLineTree(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(resourceName)].EmbeddedReaderAsLineTree(assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create a reader that opens embedded <paramref name="resourceName"/> from <paramref name="assembly"/> on <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>lines</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineEmbeddedStringLinesSource EmbeddedReaderAsStringLines(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(resourceName)].EmbeddedReaderAsStringLines(assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset from embedded resource<paramref name="resourceName"/>.
        /// 
        /// File is reloaded if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>reloadable localization asset</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static IAsset EmbeddedAsset(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(resourceName)].EmbeddedAsset(assembly, resourceName, lineFormat, throwIfNotFound);

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormatProvider"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="lineFormat">(optional) </param>
        /// <param name="throwIfNotFound">if file is not found and value is true, <see cref="FileNotFoundException"/> is thrown, otherwise zero elements are returned</param>
        /// <returns>asset source</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If file format was not found in <paramref name="fileFormatProvider"/></exception>
        public static LineEmbeddedSource EmbeddedAssetSource(this IReadOnlyDictionary<string, ILineFileFormat> fileFormatProvider, Assembly assembly, string resourceName, ILineFormat lineFormat = default, bool throwIfNotFound = true)
            => fileFormatProvider[LineFileFormatMap.GetExtension(resourceName)].EmbeddedAssetSource(assembly, resourceName, lineFormat, throwIfNotFound);

    }

}
