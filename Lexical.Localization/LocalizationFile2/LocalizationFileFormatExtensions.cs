//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lexical.Localization.Internal;

namespace Lexical.Localization.LocalizationFile2
{
    public static partial class LocalizationFileFormatExtensions_
    {
        /// <summary>
        /// Read lines from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(this ILocalizationFileFormat fileFormat, string srcText, IAssetKeyNamePolicy namePolicy = default)
            => ReadKeyLines(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read tree from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IKeyTree ReadKeyTree(this ILocalizationFileFormat fileFormat, string srcText, IAssetKeyNamePolicy namePolicy = default)
            => ReadKeyTree(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read key-values as strings from text.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadStringLines(this ILocalizationFileFormat fileFormat, string srcText, IAssetKeyNamePolicy namePolicy = default)
            => ReadStringLines(fileFormat, new StringReader(srcText), namePolicy);

        /// <summary>
        /// Read lines from file.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadFileAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadKeyLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Read tree from file.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IKeyTree ReadFileAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadKeyTree(fileFormat, fs, namePolicy);
        }

        /// <summary>
        /// Read strings from file.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadFileAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadStringLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Read lines from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(this ILocalizationFileFormat fileFormat, TextReader srcText, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcText, namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcText, namePolicy).ToKeyLines(true);
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcText.ToStream(), namePolicy).ToKeyLines(true);
            if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcText, namePolicy).ToKeyLines(namePolicy);
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcText.ToStream(), namePolicy).ToKeyLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(stream, namePolicy);
            if (fileFormat is ILocalizationKeyLinesTextReader r1) using (var txt = stream.ToTextReader()) return r1.ReadKeyLines(txt, namePolicy);
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(stream, namePolicy).ToKeyLines(true);
            if (fileFormat is ILocalizationKeyTreeTextReader r2) using (var txt = stream.ToTextReader()) return r2.ReadKeyTree(txt, namePolicy).ToKeyLines(true);
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(stream, namePolicy).ToKeyLines(namePolicy);
            if (fileFormat is ILocalizationStringLinesTextReader r5) using (var txt = stream.ToTextReader()) return r5.ReadStringLines(txt, namePolicy).ToKeyLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IKeyTree ReadKeyTree(this ILocalizationFileFormat fileFormat, TextReader srcText, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcText, namePolicy);
            if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcText, namePolicy).ToKeyTree();
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcText.ToStream(), namePolicy).ToKeyTree();
            if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcText, namePolicy).ToKeyTree(namePolicy);
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcText.ToStream(), namePolicy).ToKeyTree(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read lines from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IKeyTree ReadKeyTree(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(stream, namePolicy);
            if (fileFormat is ILocalizationKeyTreeTextReader r2) using (var txt = stream.ToTextReader()) return r2.ReadKeyTree(txt, namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(stream, namePolicy).ToKeyTree();
            if (fileFormat is ILocalizationKeyLinesTextReader r1) using (var txt = stream.ToTextReader()) return r1.ReadKeyLines(txt, namePolicy).ToKeyTree();
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(stream, namePolicy).ToKeyTree(namePolicy);
            if (fileFormat is ILocalizationStringLinesTextReader r5) using (var txt = stream.ToTextReader()) return r5.ReadStringLines(txt, namePolicy).ToKeyTree(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read strings from <paramref name="srcText"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadStringLines(this ILocalizationFileFormat fileFormat, TextReader srcText, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationStringLinesTextReader r5) return r5.ReadStringLines(srcText, namePolicy);
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcText, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcText.ToStream(), namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyTreeTextReader r2) return r2.ReadKeyTree(srcText, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcText.ToStream(), namePolicy).ToStringLines(namePolicy);
            throw new FileLoadException($"Cannot read localization with {fileFormat.GetType().FullName}");
        }

        /// <summary>
        /// Read strings from <<paramref name="stream"/> source. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadStringLines(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationStringLinesStreamReader r6) return r6.ReadStringLines(stream, namePolicy);
            if (fileFormat is ILocalizationStringLinesTextReader r5) using (var txt = stream.ToTextReader()) return r5.ReadStringLines(txt, namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(stream, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(stream, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyLinesTextReader r1) using (var txt = stream.ToTextReader()) return r1.ReadKeyLines(txt, namePolicy).ToStringLines(namePolicy);
            if (fileFormat is ILocalizationKeyTreeTextReader r2) using (var txt = stream.ToTextReader()) return r2.ReadKeyTree(txt, namePolicy).ToStringLines(namePolicy);
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
        public static void WriteKeyLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>> lines, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteKeyLines(lines, srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteKeyTree(KeyTree.Create(lines), srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteKeyLines(lines, srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteKeyTree(KeyTree.Create(lines), srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}. Have you checked Lexical.Localization.Plus for writer class.");
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
        public static void WriteKeyLines(this ILocalizationFileFormat fileFormat, IEnumerable<KeyValuePair<IAssetKey, string>> lines, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteKeyLines(lines, srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteKeyTree(KeyTree.Create(lines), srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { using (var srcTxt = srcStream.ToTextReader()) using (var tw = dstStream.ToTextWriter()) r1.WriteKeyLines(lines, srcTxt, tw, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { using (var srcTxt = srcStream.ToTextReader()) using (var tw = dstStream.ToTextWriter()) r2.WriteKeyTree(KeyTree.Create(lines), srcTxt, tw, namePolicy, flags); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}. Have you checked Lexical.Localization.Plus for writer class.");
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
        public static void WriteKeyTree(this ILocalizationFileFormat fileFormat, IKeyTree tree, TextReader srcText, TextWriter dstText, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationTreeTextWriter r2) { r2.WriteKeyTree(tree, srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeStreamWriter r4) { MemoryStream ms = new MemoryStream(); r4.WriteKeyTree(tree, srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { r1.WriteKeyLines(tree.ToKeyLines(true), srcText, dstText, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { MemoryStream ms = new MemoryStream(); r3.WriteKeyLines(tree.ToKeyLines(true), srcText.ToStream(), ms, namePolicy, flags); ms.WriteText(dstText); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}. Have you checked Lexical.Localization.Plus for writer class.");
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
        public static void WriteKeyTree(this ILocalizationFileFormat fileFormat, IKeyTree tree, Stream srcStream, Stream dstStream, IAssetKeyNamePolicy namePolicy, WriteFlags flags)
        {
            if (fileFormat is ILocalizationTreeStreamWriter r4) { r4.WriteKeyTree(tree, srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationTreeTextWriter r2) { using (var srcTxt = srcStream.ToTextReader()) using (var tw = dstStream.ToTextWriter()) r2.WriteKeyTree(tree, srcTxt, tw, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesStreamWriter r3) { r3.WriteKeyLines(tree.ToKeyLines(true), srcStream, dstStream, namePolicy, flags); return; }
            if (fileFormat is ILocalizationLinesTextWriter r1) { using (var srcTxt = srcStream.ToTextReader()) using (var tw = dstStream.ToTextWriter()) r1.WriteKeyLines(tree.ToKeyLines(true), srcTxt, tw, namePolicy, flags); return; }
            throw new FileLoadException($"Cannot write localization with {fileFormat.GetType().FullName}. Have you checked Lexical.Localization.Plus for writer class.");
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
        /// <returns>string reader that need to be disposed</returns>
        static internal TextReader ToTextReader(this Stream s)
            => new StreamReader(s, Encoding.UTF8, true, 32 * 1024);

        internal static byte[] ReadFully(this Stream s)
        {
            if (s == null) return null;

            // Try to read stream completely.
            int len_ = (int)s.Length;
            if (len_ > 2147483647) throw new IOException("File size over 2GB");
            byte[] data = new byte[len_];

            // Read chunks
            int ix = 0;
            while (ix < len_)
            {
                int count = s.Read(data, ix, len_ - ix);

                // "returns zero (0) if the end of the stream has been reached."
                if (count == 0) break;

                ix += count;
            }
            if (ix == len_) return data;
            throw new AssetException("Failed to read stream fully");
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

        static IAssetKeyNamePolicy DefaultPolicy = AssetKeyNameProvider.Default;

        public static IEnumerable<KeyValuePair<string, string>> ToStringLines(this IEnumerable<KeyValuePair<IAssetKey, string>> keyLines, IAssetKeyNamePolicy policy)
            => keyLines.Select(line => new KeyValuePair<string, string>((policy?? DefaultPolicy).BuildName(line.Key), line.Value));
        public static IEnumerable<KeyValuePair<string, string>> ToStringLines(this IKeyTree keyTree, IAssetKeyNamePolicy policy)
            => keyTree.ToKeyLines(true).ToStringLines(policy);

        /// <summary>
        /// Flatten tree as lines of key-value pairs.
        /// </summary>
        /// <param name="skipRoot"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ToKeyLines(this IKeyTree node, bool skipRoot = true)
        {
            Queue<(IKeyTree, IAssetKey)> queue = new Queue<(IKeyTree, IAssetKey)>();
            queue.Enqueue((node, skipRoot && node.Key.Name == "root" ? null : node.Key));
            while (queue.Count > 0)
            {
                // Next element
                (IKeyTree, IAssetKey) current = queue.Dequeue();

                // Yield values
                if (current.Item2 != null && current.Item1.HasValues)
                {
                    foreach (string value in current.Item1.Values)
                        yield return new KeyValuePair<IAssetKey, string>(current.Item2, value);
                }

                // Enqueue children
                if (current.Item1.HasChildren)
                {
                    foreach (IKeyTree child in current.Item1.Children)
                    {
                        IAssetKey childKey = current.Item2 == null ? child.Key : current.Item2.Concat(child.Key);
                        queue.Enqueue((child, childKey));
                    }
                }
            }
        }
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ToKeyLines(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy)
        {
            foreach(var line in lines)
            {
                IAssetKey kk;
                if (policy.TryParse(line.Key, out kk))
                    yield return new KeyValuePair<IAssetKey, string>(kk, line.Value);
            }
        }
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<IAssetKey, string>> lines)
            => KeyTree.Create(lines);
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<string, string>> lines, IAssetKeyNamePolicy policy)
            => KeyTree.Create(lines.ToKeyLines(policy));

    }

}
