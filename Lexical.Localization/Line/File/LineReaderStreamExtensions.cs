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
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILineFileFormat"/>.
    /// </summary>
    public static partial class LineReaderExtensions
    {
        /// <summary>
        /// Read localization lines from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>localization asset</returns>
        public static IAsset StreamAsset(this ILineFileFormat fileFormat, Stream stream, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineTreeTextReader || fileFormat is ILineTreeStreamReader)
            {
                return new StringAsset().Add(new ILineTree[] { fileFormat.ReadLineTree(stream, lineFormat) }, lineFormat).Load();
            }
            else
            if (fileFormat is ILineTextReader || fileFormat is ILineStreamReader)
            {
                return new StringAsset().Add(fileFormat.ReadLines(stream, lineFormat), lineFormat).Load();
            }
            else
            if (fileFormat is ILineStringTextReader || fileFormat is ILineStringStreamReader)
            {
                return new StringAsset().Add(fileFormat.ReadStringLines(stream, lineFormat), lineFormat).Load();
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
        /// <param name="lineFormat">(optional) possibly needed for string and line conversions. Used also for choosing whether to instantiate parameter into hint or key</param>
        /// <returns>localization asset</returns>
        public static IAssetSource StreamAssetSource(this ILineFileFormat fileFormat, Func<Stream> streamSource, ILineFormat lineFormat = default)
            => new StreamProviderAssetSource(fileFormat, streamSource, lineFormat);

    }

}
