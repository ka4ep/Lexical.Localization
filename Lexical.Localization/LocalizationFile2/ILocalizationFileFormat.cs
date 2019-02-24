//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.1.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lexical.Localization.LocalizationFile2
{
    /// <summary>
    /// Represents a localization file format.
    /// </summary>
    public interface ILocalizationFileFormat
    {
        /// <summary>
        /// Extension of the file format without separator. e.g. "xml".
        /// </summary>
        string Extension { get; }
    }

    /// <summary>
    /// Signals that file format can be read localization files.
    /// </summary>
    public interface ILocalizationReader : ILocalizationFileFormat { }

    /// <summary>
    /// Reader that can read localization lines from a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationLinesStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into lines.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy. </param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can read localization into tree format format a <see cref="Stream"/>.
    /// </summary>
    public interface ILocalizationTreeStreamReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="stream"/> into tree structuer.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        KeyTree ReadTree(Stream stream, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can open a read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationLinesTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>the read lines</returns>
        /// <exception cref="IOException"></exception>
        IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Reader that can open a read localization lines from a <see cref="TextReader"/>.
    /// </summary>
    public interface ILocalizationTreeTextReader : ILocalizationReader
    {
        /// <summary>
        /// Read <paramref name="text"/> into lines.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <returns>lines in tree structure</returns>
        /// <exception cref="IOException"></exception>
        KeyTree ReadTree(TextReader text, IAssetKeyNamePolicy namePolicy = default);
    }

    /// <summary>
    /// Signals that file format can write localization files.
    /// </summary>
    public interface ILocalizationWriter : ILocalizationFileFormat { }

    /// <summary>
    /// Writer that can write localization key-values to streams.
    /// </summary>
    public interface ILocalizationLinesStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="lines"/> to <paramref name="stream"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, Stream stream, IAssetKeyNamePolicy namePolicy);
    }

    /// <summary>
    /// Writer that can write localization key-values with text writers.
    /// </summary>
    public interface ILocalizationLinesTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <exception cref="IOException"></exception>
        void WriteLines(IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextWriter text, IAssetKeyNamePolicy namePolicy);
    }

    /// <summary>
    /// Writer that can write localization tree structure to streams.
    /// </summary>
    public interface ILocalizationTreeStreamWriter : ILocalizationWriter
    {
        /// <summary>
        /// Write <paramref name="tree"/> to <paramref name="stream"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) name policy.</param>
        /// <exception cref="IOException"></exception>
        void WriteTree(KeyTree tree, Stream stream, IAssetKeyNamePolicy namePolicy);
    }

    /// <summary>
    /// Writer that can write localization tree structure to text writer.
    /// </summary>
    public interface ILocalizationTreeTextWriter : ILocalizationWriter
    {
        /// <summary>
        /// Create a container where localization key-values can be written to.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy">(optional) name policy. If null, uses the default policy for the file format.</param>
        /// <exception cref="IOException"></exception>
        void WriteTree(KeyTree tree, TextWriter text, IAssetKeyNamePolicy namePolicy);
    }

    public static class LocalizationFileFormatExtensions
    {
        /// <summary>
        /// Read lines from <<paramref name="text"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadLines(this ILocalizationFileFormat fileFormat, TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationLinesTextReader r1) return r1.ReadLines(text, namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(text, namePolicy).ToKeyValues(true);
            if (fileFormat is ILocalizationLinesStreamReader r3) return r3.ReadLines(new MemoryStream(Encoding.UTF8.GetBytes(text.ReadToEnd())), namePolicy);
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(new MemoryStream(Encoding.UTF8.GetBytes(text.ReadToEnd())), namePolicy).ToKeyValues(true);
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
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(stream, namePolicy).ToKeyValues(true);
            if (fileFormat is ILocalizationLinesTextReader r1) return r1.ReadLines(new StreamReader(stream, Encoding.UTF8), namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(new StreamReader(stream, Encoding.UTF8), namePolicy).ToKeyValues(true);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="text"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static KeyTree ReadTree(this ILocalizationFileFormat fileFormat, TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(text, namePolicy);
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(new MemoryStream(Encoding.UTF8.GetBytes(text.ReadToEnd())), namePolicy);
            if (fileFormat is ILocalizationLinesTextReader r1) return KeyTree.Create(r1.ReadLines(text, namePolicy));
            if (fileFormat is ILocalizationLinesStreamReader r3) return KeyTree.Create(r3.ReadLines(new MemoryStream(Encoding.UTF8.GetBytes(text.ReadToEnd())), namePolicy));
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static KeyTree ReadTree(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeStreamReader r4) return r4.ReadTree(stream, namePolicy);
            if (fileFormat is ILocalizationTreeTextReader r2) return r2.ReadTree(new StreamReader(stream, Encoding.UTF8), namePolicy);
            if (fileFormat is ILocalizationLinesStreamReader r3) return KeyTree.Create(r3.ReadLines(stream, namePolicy));
            if (fileFormat is ILocalizationLinesTextReader r1) return KeyTree.Create(r1.ReadLines(new StreamReader(stream, Encoding.UTF8), namePolicy));
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="text"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="lines"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static void WriteLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextWriter text, IAssetKeyNamePolicy namePolicy)
        {
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(lines, text, namePolicy); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(KeyTree.Create(lines), text, namePolicy); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteLines(lines, ms, namePolicy); ms.Position = 0L; text.Write(Encoding.UTF8.GetString(ms.GetBuffer())); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteTree(KeyTree.Create(lines), ms, namePolicy); ms.Position = 0L; text.Write(Encoding.UTF8.GetString(ms.GetBuffer())); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="lines"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static void WriteLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>>  lines, Stream stream, IAssetKeyNamePolicy namePolicy)
        {
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteLines(lines, stream, namePolicy); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteTree(KeyTree.Create(lines), stream, namePolicy); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(lines, new StreamWriter(stream, Encoding.UTF8), namePolicy); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(KeyTree.Create(lines), new StreamWriter(stream, Encoding.UTF8), namePolicy); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="text"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="tree"></param>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static void WriteTree(this ILocalizationFileFormat fileFormat, KeyTree tree, TextWriter text, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(tree, text, namePolicy); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteTree(tree, ms, namePolicy); ms.Position = 0L; text.Write(Encoding.UTF8.GetString(ms.GetBuffer())); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(tree.ToKeyValues(true), text, namePolicy); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteLines(tree.ToKeyValues(true), ms, namePolicy); ms.Position = 0L; text.Write(Encoding.UTF8.GetString(ms.GetBuffer())); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Write lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="tree"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static void WriteTree(this ILocalizationFileFormat fileFormat, KeyTree tree, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteTree(tree, stream, namePolicy); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteTree(tree, new StreamWriter(stream, Encoding.UTF8), namePolicy); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteLines(tree.ToKeyValues(true), stream, namePolicy); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteLines(tree.ToKeyValues(true), new StreamWriter(stream, Encoding.UTF8), namePolicy); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}");
        }

    }

}
