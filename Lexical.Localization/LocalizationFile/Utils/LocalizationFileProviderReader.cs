//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lexical.Localization.Utils
{
    using Microsoft.Extensions.FileProviders;

    public abstract class LocalizationFileProviderReader : LocalizationReader
    {
        public string FilePath { get; protected set; }
        public IFileProvider FileProvider { get; protected set; }

        public LocalizationFileProviderReader(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy) : base(fileFormat, namePolicy)
        {
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        public override string ToString()
            => FilePath;
    }
    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderStringLines : LocalizationFileProviderReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationFileProviderReaderStringLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy) : base(fileFormat, fileProvider, filepath, namePolicy) { }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        public override string ToString()
            => FilePath;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyLines : LocalizationFileProviderReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationFileProviderReaderKeyLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy) : base(fileFormat, fileProvider, filepath, namePolicy) { }

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        public override string ToString()
            => FilePath;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyTree : LocalizationFileProviderReader, IEnumerable<IKeyTree>
    {
        public LocalizationFileProviderReaderKeyTree(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy) : base(fileFormat, fileProvider, filepath, namePolicy) { }

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        public override string ToString()
            => FilePath;
    }

}
