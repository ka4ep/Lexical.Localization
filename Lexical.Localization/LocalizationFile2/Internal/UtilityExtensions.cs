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
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization.LocalizationFile2.Internal
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Read content in <paramref name="srcText"/> and write to memory stream snapshot.
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns>stream that doesn't need dispose</returns>
        public static MemoryStream ToStream(this TextReader srcText)
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
        public static TextReader ToTextReader(this Stream s)
            => new StreamReader(s, Encoding.UTF8, true, 32 * 1024);

        public static byte[] ReadFully(this Stream s)
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
        public static void WriteText(this MemoryStream ms, TextWriter dstText)
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
        public static TextWriter ToTextWriter(this Stream s)
            => new StreamWriter(s, Encoding.UTF8, 16 * 1024, true);
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
