//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization.LocalizationFile2
{
    public static partial class LocalizationFileFormatExtensions_
    {
        /// <summary>
        /// Read lines from <<paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(this ILocalizationFileFormat fileFormat, TextReader srcText, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationLinesTextReader r1) return r1.ReadLines(srcText, namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(srcText, namePolicy).ToLines(true);
            if (fileFormat is ILocalizationLinesStreamReader r3) return r3.ReadLines(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(srcText.ToStream(), namePolicy).ToLines(true);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationLinesStreamReader r3) return r3.ReadLines(stream, namePolicy);
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(stream, namePolicy).ToLines(true);
            if (fileFormat is ILocalizationLinesTextReader r1) return r1.ReadLines(stream.ToTextReader(), namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(stream.ToTextReader(), namePolicy).ToLines(true);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IKeyTree ReadTree(this ILocalizationFileFormat fileFormat, TextReader srcText, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(srcText, namePolicy);
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationLinesTextReader r1) return KeyTree.Create(r1.ReadLines(srcText, namePolicy));
            if (fileFormat is ILocalizationLinesStreamReader r3) return KeyTree.Create(r3.ReadLines(srcText.ToStream(), namePolicy));
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IKeyTree ReadTree(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(stream, namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(stream.ToTextReader(), namePolicy);
            if (fileFormat is ILocalizationLinesStreamReader r3) return KeyTree.Create(r3.ReadLines(stream, namePolicy));
            if (fileFormat is ILocalizationLinesTextReader r1) return KeyTree.Create(r1.ReadLines(stream.ToTextReader(), namePolicy));
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="lines"></param>
        /// <param name="srcText">(optional) source text, used if previous content are updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        public static void WriteLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(lines, srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(KeyTree.Create(lines), srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteLines(lines, srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteTree(KeyTree.Create(lines), srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="dstStream"/>.
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="lines"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream">stream to write to.</param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        public static void WriteLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>> lines, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteLines(lines, srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteTree(KeyTree.Create(lines), srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { using (var tw = dstStream.ToTextWriter()) r1.WriteLines(lines, srcStream.ToTextReader(), tw, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { using (var tw = dstStream.ToTextWriter()) r2.WriteTree(KeyTree.Create(lines), srcStream.ToTextReader(), tw, namePolicy, flags); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="stream"/> source. 
        /// 
        /// If <paramref name="srcText"/> contains previous content, it is updated and rewritten to <paramref name="dstText"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="tree"></param>
        /// <param name="srcText">(optional) source text, used if previous content is updated.</param>
        /// <param name="dstText"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        public static void WriteTree(this ILocalizationFileFormat fileFormat, IKeyTree tree, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(tree, srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteTree(tree, srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(tree.ToLines(true), srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteLines(tree.ToLines(true), srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="text"/> source. 
        /// 
        /// If <paramref name="srcStream"/> contains previous content, it is updated and rewritten to <paramref name="dstStream"/> according to rules in <paramref name="flags"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="tree"></param>
        /// <param name="srcStream">(optional) source data, used if previous content is updated</param>
        /// <param name="dstStream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <param name="flags"></param>
        /// <exception cref="IOException"></exception>
        public static void WriteTree(this ILocalizationFileFormat fileFormat, IKeyTree tree, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteTree(tree, srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { using (var tw = dstStream.ToTextWriter()) r2.WriteTree(tree, srcStream.ToTextReader(), tw, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteLines(tree.ToLines(true), srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { using (var tw = dstStream.ToTextWriter()) r1.WriteLines(tree.ToLines(true), srcStream.ToTextReader(), tw, namePolicy, flags); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read content in <paramref name="srcText"/> and write to memory stream snapshot.
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns>stream that doesn't need dispose</returns>
        static internal MemoryStream ToStream(this TextReader srcText)
        {
            if (srcText == null) return null;
            byte[] data = Encoding.UTF8.GetBytes(srcText.ReadToEnd());
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Flush();
            ms.Position = 0L;
            return ms;
        }

        /// <summary>
        /// Read content in <paramref name="s"/> and decode into string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>string reader that doesn't need dispose</returns>
        static internal TextReader ToTextReader(this Stream s)
        {
            long l = s.Length - s.Position;
            if (l > Int32.MaxValue) throw new OutOfMemoryException();
            byte[] data = new byte[l];
            string str = Encoding.UTF8.GetString(data);
            return new StringReader(str);
        }

        /// <summary>
        /// Write contents in <paramref name="ms"/> into <paramref name="dstText"/>.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="dstText"></param>
        static internal void WriteText(this MemoryStream ms, TextWriter dstText)
        {
            ms.Position = 0L;
            dstText.Write(Encoding.UTF8.GetString(ms.GetBuffer()));
            dstText.Flush();
        }

        /// <summary>
        /// Create writer that converts text to stream.
        /// Result must be flushed and disposed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>writer that must be disposed.</returns>
        static internal TextWriter ToTextWriter(this Stream s)
            => new StreamWriter(s, Encoding.UTF8, 16 * 1024, true);

    }

}
