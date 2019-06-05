//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="ILineFileFormat"/>.
    /// </summary>
    public static partial class LineReaderExtensions
    {
        /// <summary>
        /// Read lines from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns>lines</returns>
        public static IEnumerable<ILine> ReadString(this ILineFileFormat fileFormat, string srcText, ILineFormat lineFormat = default)
            => ReadLines(fileFormat, new StringReader(srcText), lineFormat);

        /// <summary>
        /// Read tree from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns>tree</returns>
        public static ILineTree ReadStringAsLineTree(this ILineFileFormat fileFormat, string srcText, ILineFormat lineFormat = default)
            => ReadLineTree(fileFormat, new StringReader(srcText), lineFormat);

        /// <summary>
        /// Read key-values as strings from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, IString>> ReadStringAsStringLines(this ILineFileFormat fileFormat, string srcText, ILineFormat lineFormat = default)
            => ReadStringLines(fileFormat, new StringReader(srcText), lineFormat);

        /// <summary>
        /// Read lines from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> ReadLines(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, lineFormat);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), lineFormat);
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, lineFormat).ToLines();
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), lineFormat).ToLines();
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, lineFormat).ToLines(lineFormat);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), lineFormat).ToLines(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> ReadLines(this ILineFileFormat fileFormat, Stream stream, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, lineFormat);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, lineFormat);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, lineFormat).ToLines();
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, lineFormat).ToLines();
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, lineFormat).ToLines(lineFormat);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, lineFormat).ToLines(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static ILineTree ReadLineTree(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, lineFormat);
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), lineFormat);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), lineFormat).ToLineTree(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static ILineTree ReadLineTree(this ILineFileFormat fileFormat, Stream stream, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, lineFormat);
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, lineFormat);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, lineFormat).ToLineTree(lineFormat);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, lineFormat).ToLineTree(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }


        /// <summary>
        /// Read strings from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IString>> ReadStringLines(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, lineFormat);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), lineFormat);
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), lineFormat).ToStringLines(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read strings from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IString>> ReadStringLines(this ILineFileFormat fileFormat, Stream stream, ILineFormat lineFormat = default)
        {
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, lineFormat);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, lineFormat);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, lineFormat).ToStringLines(lineFormat);
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, lineFormat).ToStringLines(lineFormat);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

    }


}
