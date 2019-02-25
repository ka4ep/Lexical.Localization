// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization.LocalizationFile2
{
    public static partial class LocalizationFileFormatExtensions_
    {
        /// <summary>
        /// Create file source that reads file into memory every time <see cref="IEnumerator{T}"/> is acquired.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy">(optional) name policy argument for the reader</param>
        /// <returns>file reader</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> FileSource(this ILocalizationFileFormat fileFormat, string filepath, IAssetKeyNamePolicy namePolicy = default)
            => new _FileSource(fileFormat, filepath, namePolicy);
    }

    /// <summary>
    /// File source that reads file into memory every time <see cref="IEnumerator{T}"/> is acquired.
    /// </summary>
    internal class _FileSource : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        /// <summary>
        /// File name
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// File format
        /// </summary>
        public readonly ILocalizationFileFormat FileFormat;

        /// <summary>
        /// Name policy to use for reading file.
        /// </summary>
        public readonly IAssetKeyNamePolicy NamePolicy;

        public _FileSource(ILocalizationFileFormat fileFormat, string filepath, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.NamePolicy = namePolicy;
        }

        public IEnumerable<KeyValuePair<IAssetKey, string>> Read()
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return FileFormat.ReadLines(fs, NamePolicy);
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
            => Read().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Read().GetEnumerator();
    }
}
