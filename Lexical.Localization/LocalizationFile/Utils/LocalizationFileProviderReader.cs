//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Utils
{
    using Microsoft.Extensions.FileProviders;

    public abstract class LocalizationFileProviderReader : LocalizationReader
    {
        public string FilePath { get; protected set; }
        public IFileProvider FileProvider { get; protected set; }
        public bool ThrowIfNotFound { get; protected set; }

        public LocalizationFileProviderReader(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, namePolicy)
        {
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        public override string ToString()
            => FilePath;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderStringLines : LocalizationFileProviderReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationFileProviderReaderStringLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<string, string>> empty = new KeyValuePair<string, string>[0];

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyLines : LocalizationFileProviderReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationFileProviderReaderKeyLines(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<IAssetKey, string>> empty = new KeyValuePair<IAssetKey, string>[0];

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<IAssetKey, string>>)this).GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyTree : LocalizationFileProviderReader, IEnumerable<IKeyTree>
    {
        public LocalizationFileProviderReaderKeyTree(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<IKeyTree> empty = new IKeyTree[0];

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                {
                    IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                    if (tree == null) return empty.GetEnumerator();
                    IKeyTree[] trees = new IKeyTree[] { tree };
                    return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
                }
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }

        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<IKeyTree>)this).GetEnumerator();
    }

}
