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
using System.Text;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="ILocalizationFileFormat"/>.
    /// </summary>
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
            if (fileFormat is ILocalizationKeyLinesTextReader r1) return r1.ReadKeyLines(srcText, namePolicy).ToKeyTree(namePolicy);
            if (fileFormat is ILocalizationKeyTreeStreamReader r4) return r4.ReadKeyTree(srcText.ToStream(), namePolicy);
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(srcText.ToStream(), namePolicy).ToKeyTree(namePolicy);
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
            if (fileFormat is ILocalizationKeyLinesStreamReader r3) return r3.ReadKeyLines(stream, namePolicy).ToKeyTree(namePolicy);
            if (fileFormat is ILocalizationKeyLinesTextReader r1) using (var txt = stream.ToTextReader()) return r1.ReadKeyLines(txt, namePolicy).ToKeyTree(namePolicy);
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

    }

    internal class FileReaderStringLines : IEnumerable<KeyValuePair<string, string>>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderStringLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    internal class FileReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderKeyLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    internal class FileReaderKeyTree : IEnumerable<IKeyTree>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderKeyTree(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<IKeyTree> GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }
    }


}
