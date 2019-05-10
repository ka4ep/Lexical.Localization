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
        /// Key policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILinePolicy KeyPolicy { get; internal set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; internal set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="keyPolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileSource(ILocalizationFileFormat fileFormat, string path, string filename, ILinePolicy keyPolicy, bool throwIfNotFound) : base(path, filename, throwIfNotFound)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.KeyPolicy = keyPolicy;
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
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileStringLinesSource(ILocalizationFileFormat fileFormat, string path, string filename, ILinePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, IFormulationString>> IEnumerable<KeyValuePair<string, IFormulationString>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, IFormulationString>> lines = LocalizationReaderExtensions.ReadStringLines(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, IFormulationString>> lines = LocalizationReaderExtensions.ReadStringLines(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset(this, KeyPolicy));

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

        /// <summary>
        /// Create clone with new path.
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns>clone</returns>
        public override FileSource SetPath(string newPath)
            => new LocalizationFileStringLinesSource(FileFormat, newPath, FileName, KeyPolicy, ThrowIfNotFound);
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
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileKeyLinesSource(ILocalizationFileFormat fileFormat, string path, string filename, ILinePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<ILine, IFormulationString>> IEnumerable<KeyValuePair<ILine, IFormulationString>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<ILine, IFormulationString>> lines = LocalizationReaderExtensions.ReadKeyLines(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable lines = LocalizationReaderExtensions.ReadKeyLines(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset(this, KeyPolicy));

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

        /// <summary>
        /// Create clone with new path.
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns>clone</returns>
        public override FileSource SetPath(string newPath)
            => new LocalizationFileKeyLinesSource(FileFormat, newPath, FileName, KeyPolicy, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source that reads as ILineTree.
    /// </summary>
    public class LocalizationFileLineTreeSource : LocalizationFileSource, ILocalizationLineTreeSource
    {
        /// <summary>
        /// Create localization file source that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileLineTreeSource(ILocalizationFileFormat fileFormat, string path, string filename, ILinePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

        static ILineTree[] no_trees = new ILineTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            ILineTree tree = LocalizationReaderExtensions.ReadLineTree(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound);
            ILineTree[] trees = tree == null ? no_trees : new ILineTree[] { tree };
            return ((IEnumerable<ILineTree>)trees).GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            ILineTree tree = LocalizationReaderExtensions.ReadLineTree(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound);
            ILineTree[] trees = tree == null ? no_trees : new ILineTree[] { tree };
            return ((IEnumerable<ILineTree>)trees).GetEnumerator();
        }

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset(this, KeyPolicy));

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

        /// <summary>
        /// Create clone with new path.
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns>clone</returns>
        public override FileSource SetPath(string newPath)
            => new LocalizationFileLineTreeSource(FileFormat, newPath, FileName, KeyPolicy, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source.
    /// </summary>
    public abstract class LocalizationFilePatternSource : FilePatternSource, IAssetSource
    {
        /// <summary>
        /// Key policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILinePolicy KeyPolicy { get; protected set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path"></param>
        /// <param name="keyPattern"></param>
        /// <param name="namePolicy"></param>
        public LocalizationFilePatternSource(ILocalizationFileFormat fileFormat, string path, ILinePattern keyPattern, ILinePolicy namePolicy) : base(path, keyPattern)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.KeyPolicy = namePolicy;
        }

        /// <summary>
        /// Convert parameters in <paramref name="key"/> into source.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract IAssetSource CreateSource(ILine key);
    }


}
