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
        public IParameterPolicy KeyPolicy { get; internal set; }

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
        public LocalizationFileSource(ILocalizationFileFormat fileFormat, string path, string filename, IParameterPolicy keyPolicy, bool throwIfNotFound) : base(path, filename, throwIfNotFound)
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
        public LocalizationFileStringLinesSource(ILocalizationFileFormat fileFormat, string path, string filename, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

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
        public LocalizationFileKeyLinesSource(ILocalizationFileFormat fileFormat, string path, string filename, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

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
    /// Localization file source that reads as IKeyTree.
    /// </summary>
    public class LocalizationFileKeyTreeSource : LocalizationFileSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Create localization file source that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileKeyTreeSource(ILocalizationFileFormat fileFormat, string path, string filename, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, path, filename, namePolicy, throwIfNotFound) { }

        static IKeyTree[] no_trees = new IKeyTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            IKeyTree tree = LocalizationReaderExtensions.ReadKeyTree(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound);
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
            IKeyTree tree = LocalizationReaderExtensions.ReadKeyTree(FileFormat, FilePath, KeyPolicy, ThrowIfNotFound);
            IKeyTree[] trees = tree == null ? no_trees : new IKeyTree[] { tree };
            return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
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
            => new LocalizationFileKeyTreeSource(FileFormat, newPath, FileName, KeyPolicy, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source.
    /// </summary>
    public abstract class LocalizationFilePatternSource : FilePatternSource, IAssetSource
    {
        /// <summary>
        /// Key policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IParameterPolicy KeyPolicy { get; protected set; }

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
        public LocalizationFilePatternSource(ILocalizationFileFormat fileFormat, string path, IParameterPattern keyPattern, IParameterPolicy namePolicy) : base(path, keyPattern)
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
