//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Localization file source.
    /// </summary>
    public abstract class LocalizationFileSource : FileSource, IAssetSource, IEnumerable
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
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(filename, throwIfNotFound)
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
    /// Localization file source that reads as IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;.
    /// </summary>
    public class LocalizationFileStringLinesSource : LocalizationFileSource, ILocalizationStringLinesSource
    {
        /// <summary>
        /// Create localization file source that reads as string lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileStringLinesSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationReaderExtensions_.ReadStringLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationReaderExtensions_.ReadStringLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

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
    /// Localization file source that reads as IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;.
    /// </summary>
    public class LocalizationFileKeyLinesSource : LocalizationFileSource, ILocalizationKeyLinesSource
    {
        /// <summary>
        /// Create localization file source that reads as key lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileKeyLinesSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationReaderExtensions_.ReadKeyLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable lines = LocalizationReaderExtensions_.ReadKeyLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

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
    /// Localization file source that reads as IKeyTree.
    /// </summary>
    public class LocalizationFileKeyTreeSource : LocalizationFileSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Create localization file source that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileKeyTreeSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }

        static IKeyTree[] no_trees = new IKeyTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            IKeyTree tree = LocalizationReaderExtensions_.ReadKeyTree(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            IKeyTree[] trees = tree == null ? no_trees : new IKeyTree[] { tree };
            return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IKeyTree tree = LocalizationReaderExtensions_.ReadKeyTree(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            IKeyTree[] trees = tree == null ? no_trees : new IKeyTree[] { tree };
            return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
        }

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
