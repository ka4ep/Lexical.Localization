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

namespace Lexical.Localization.Internal
{
    public abstract class LocalizationReader : IEnumerable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        public ILocalizationFileFormat FileFormat { get; protected set; }

        public LocalizationReader(ILocalizationFileFormat fileFormat, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.NamePolicy = namePolicy;
        }

        public abstract IEnumerator GetEnumerator();
    }

    public abstract class LocalizationFileReader : LocalizationReader
    {
        public string FileName { get; protected set; }

        public LocalizationFileReader(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy) : base(fileFormat, namePolicy)
        {
            this.FileName = filename ?? throw new ArgumentNullException(nameof(FileName));
        }

        public override string ToString()
            => FileName;
    }

    public abstract class LocalizationEmbeddedReader : LocalizationReader
    {
        public string ResourceName { get; protected set; }
        public Assembly Asm { get; protected set; }

        public LocalizationEmbeddedReader(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy) : base(fileFormat, namePolicy)
        {
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        }

        public override string ToString()
            => ResourceName;
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderStringLines : LocalizationFileReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationFileReaderStringLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy) : base(fileFormat, filename, namePolicy) { }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions_.ReadFileAsStringLines(FileFormat, FileName, NamePolicy);
            return lines.GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions_.ReadFileAsStringLines(FileFormat, FileName, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderKeyLines : LocalizationFileReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationFileReaderKeyLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy) : base(fileFormat, filename, namePolicy) { }

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions_.ReadFileAsKeyLines(FileFormat, FileName, NamePolicy);
            return lines.GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IEnumerable lines = LocalizationFileExtensions_.ReadFileAsKeyLines(FileFormat, FileName, NamePolicy);
            return lines.GetEnumerator();
        }

        public override string ToString()
            => FileName;
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IKeyTree, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderKeyTree : LocalizationFileReader, IEnumerable<IKeyTree>
    {
        public LocalizationFileReaderKeyTree(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy) : base(fileFormat, filename, namePolicy) { }

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions_.ReadFileAsKeyTree(FileFormat, FileName, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions_.ReadFileAsKeyTree(FileFormat, FileName, NamePolicy);
            return new IKeyTree[] { tree }.GetEnumerator();
        }

        public override string ToString()
            => FileName;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderStringLines : LocalizationEmbeddedReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationEmbeddedReaderStringLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy) : base(fileFormat, asm, resourceName, namePolicy) { }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        public override string ToString()
            => ResourceName;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderKeyLines : LocalizationEmbeddedReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationEmbeddedReaderKeyLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy) : base(fileFormat, asm, resourceName, namePolicy) { }

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        public override string ToString()
            => ResourceName;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderKeyTree : LocalizationEmbeddedReader, IEnumerable<IKeyTree>
    {
        public LocalizationEmbeddedReaderKeyTree(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy) : base(fileFormat, asm, resourceName, namePolicy) { }

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        public override string ToString()
            => ResourceName;
    }

}

namespace Lexical.Localization.Ms.Extensions
{
    using Microsoft.Extensions.FileProviders;
    using Lexical.Localization.Internal;

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
