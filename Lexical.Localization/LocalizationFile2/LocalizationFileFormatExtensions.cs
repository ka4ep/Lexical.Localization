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
using Lexical.Localization.LocalizationFile.Internal;

namespace Lexical.Localization.LocalizationFile2
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
        /// Read file into assetkey lines.
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
        /// Read file into a tree format.
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
        /// Read file into strings file.
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
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateFileReaderAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateFileReaderAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyTree(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateFileReaderAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderStringLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IAsset CreateFileAsset(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationStringAsset(fileFormat.CreateFileReaderAsStringLines(filename, namePolicy), namePolicy);
            }
            else
            if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset().AddKeyLinesSource(fileFormat.CreateFileReaderAsKeyLines(filename, namePolicy)).Load();
            }
            else
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationAsset().AddKeyTreeSource(fileFormat.CreateFileReaderAsKeyTree(filename, namePolicy)).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IAssetSource CreateFileAssetSource(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new AssetFileSource(fileFormat, filename, namePolicy);

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
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileFormatExtensions_.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileFormatExtensions_.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
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
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileFormatExtensions_.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileFormatExtensions_.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
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
            IKeyTree tree = LocalizationFileFormatExtensions_.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IKeyTree tree = LocalizationFileFormatExtensions_.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }
    }

    internal class AssetFileSource : IAssetSource
    {
        public readonly ILocalizationFileFormat FileFormat;
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public AssetFileSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            this.NamePolicy = namePolicy;
        }
        public void Build(IList<IAsset> list) => list.Add(FileFormat.CreateFileAsset(Filename, NamePolicy));
        public IAsset PostBuild(IAsset asset) => asset;
    }

}
