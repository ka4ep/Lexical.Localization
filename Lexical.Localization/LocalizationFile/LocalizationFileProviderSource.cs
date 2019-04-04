//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Localization source and reader that reads lines from file provider.
    /// </summary>
    public abstract class LocalizationFileProviderSource : FileProviderSource, IAssetSource, IEnumerable
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Create (abstract) source to localization file in a file provider.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileProvider, filepath, throwIfNotFound)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.NamePolicy = namePolicy;
        }

        /// <summary>
        /// Create reader.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;
    /// </summary>
    public class LocalizationFileProviderStringLinesSource : LocalizationFileProviderSource, ILocalizationStringLinesSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderStringLinesSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<string, string>> empty = new KeyValuePair<string, string>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
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

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationStringAsset(NamePolicy).AddSource(this).Load());

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;.
    /// </summary>
    public class LocalizationFileProviderKeyLinesSource : LocalizationFileProviderSource, ILocalizationKeyLinesSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderKeyLinesSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<IAssetKey, string>> empty = new KeyValuePair<IAssetKey, string>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
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

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<IAssetKey, string>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().AddSource(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyTree : LocalizationFileProviderSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderReaderKeyTree(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<IKeyTree> empty = new IKeyTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
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

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<IKeyTree>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().AddSource(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }

}
