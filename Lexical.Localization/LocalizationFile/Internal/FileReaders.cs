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
    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class FileReaderStringLines : IEnumerable<KeyValuePair<string, string>>
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
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions_.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions_.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class FileReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
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
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions_.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions_.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IKeyTree, string}}"/>.
    /// </summary>
    public class FileReaderKeyTree : IEnumerable<IKeyTree>
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
            IKeyTree tree = LocalizationFileExtensions_.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions_.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class EmbeddedReaderStringLines : IEnumerable<KeyValuePair<string, string>>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderStringLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class EmbeddedReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderKeyLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class EmbeddedReaderKeyTree : IEnumerable<IKeyTree>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderKeyTree(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<IKeyTree> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }
    }

}

namespace Lexical.Localization.Ms.Extensions
{
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class FileProviderReaderStringLines : IEnumerable<KeyValuePair<string, string>>
    {
        public readonly string FilePath;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly IFileProvider FileProvider;

        public FileProviderReaderStringLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class FileProviderReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public readonly string FilePath;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly IFileProvider FileProvider;

        public FileProviderReaderKeyLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class FileProviderReaderKeyTree : IEnumerable<IKeyTree>
    {
        public readonly string FilePath;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly IFileProvider FileProvider;

        public FileProviderReaderKeyTree(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<IKeyTree> GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = FileProvider.GetFileInfo(FilePath).CreateReadStream())
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }
    }

}
