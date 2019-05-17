//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
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
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<ILine> ReadString(this ILineFileFormat fileFormat, string srcText, ILineFormat namePolicy = default)
            => ReadLines(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read tree from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static ILineTree ReadStringAsLineTree(this ILineFileFormat fileFormat, string srcText, ILineFormat namePolicy = default)
            => ReadLineTree(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read key-values as strings from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, IFormatString>> ReadStringAsStringLines(this ILineFileFormat fileFormat, string srcText, ILineFormat namePolicy = default)
            => ReadStringLines(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read lines from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> ReadLines(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, namePolicy);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), namePolicy);
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, namePolicy).ToLines();
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), namePolicy).ToLines();
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, namePolicy).ToLines(namePolicy);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), namePolicy).ToLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> ReadLines(this ILineFileFormat fileFormat, Stream stream, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, namePolicy);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, namePolicy);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, namePolicy).ToLines();
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, namePolicy).ToLines();
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, namePolicy).ToLines(namePolicy);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, namePolicy).ToLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static ILineTree ReadLineTree(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, namePolicy);
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), namePolicy);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), namePolicy).ToLineTree(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static ILineTree ReadLineTree(this ILineFileFormat fileFormat, Stream stream, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, namePolicy);
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, namePolicy);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, namePolicy).ToLineTree(namePolicy);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, namePolicy).ToLineTree(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }


        /// <summary>
        /// Read strings from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IFormatString>> ReadStringLines(this ILineFileFormat fileFormat, TextReader srcText, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineStringTextReader r5) return r5.ReadStringLines(srcText, namePolicy);
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(srcText.ReadStream(), namePolicy);
            if (fileFormat is ILineTextReader r1) return r1.ReadLines(srcText, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(srcText.ReadStream(), namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineTreeTextReader r2) return r2.ReadLineTree(srcText, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(srcText.ReadStream(), namePolicy).ToStringLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read strings from <paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IFormatString>> ReadStringLines(this ILineFileFormat fileFormat, Stream stream, ILineFormat namePolicy = default)
        {
            if (fileFormat is ILineStringStreamReader r6) return r6.ReadStringLines(stream, namePolicy);
            if (fileFormat is ILineStringTextReader r5) using (var txt = stream.ReadText()) return r5.ReadStringLines(txt, namePolicy);
            if (fileFormat is ILineStreamReader r3) return r3.ReadLines(stream, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineTreeStreamReader r4) return r4.ReadLineTree(stream, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineTextReader r1) using (var txt = stream.ReadText()) return r1.ReadLines(txt, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILineTreeTextReader r2) using (var txt = stream.ReadText()) return r2.ReadLineTree(txt, namePolicy).ToStringLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

    }


}
