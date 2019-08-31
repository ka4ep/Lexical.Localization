//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
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
    public abstract class LineFileSource : FileSource, IAssetSource, IEnumerable
    {
        /// <summary>
        /// Key policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILineFormat LineFormat { get; internal set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILineFileFormat FileFormat { get; internal set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public LineFileSource(ILineFileFormat fileFormat, string path, string filename, ILineFormat lineFormat, bool throwIfNotFound) : base(path, filename, throwIfNotFound)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.LineFormat = lineFormat;
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
    public class StringLineFileSource : LineFileSource, IStringLinesSource
    {
        /// <summary>
        /// Create localization file source that reads as string lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public StringLineFileSource(ILineFileFormat fileFormat, string path, string filename, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, path, filename, lineFormat, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, IString>> IEnumerable<KeyValuePair<string, IString>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, IString>> lines = LineReaderExtensions.ReadStringLines(FileFormat, FilePath, LineFormat, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, IString>> lines = LineReaderExtensions.ReadStringLines(FileFormat, FilePath, LineFormat, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new StringAsset(this, LineFormat));

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
            => new StringLineFileSource(FileFormat, newPath, FileName, LineFormat, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source that reads as IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;.
    /// </summary>
    public class KeyLineFileSource : LineFileSource, IKeyLinesSource
    {
        /// <summary>
        /// Create localization file source that reads as key lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public KeyLineFileSource(ILineFileFormat fileFormat, string path, string filename, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, path, filename, lineFormat, throwIfNotFound) { }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILine> IEnumerable<ILine>.GetEnumerator()
        {
            IEnumerable<ILine> lines = LineReaderExtensions.ReadLines(FileFormat, FilePath, LineFormat, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
        {
            IEnumerable lines = LineReaderExtensions.ReadLines(FileFormat, FilePath, LineFormat, ThrowIfNotFound).ToArray();
            return lines.GetEnumerator();
        }

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new StringAsset(this, LineFormat));

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
            => new KeyLineFileSource(FileFormat, newPath, FileName, LineFormat, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source that reads as ILineTree.
    /// </summary>
    public class LineTreeFileSource : LineFileSource, ITreeLinesSource
    {
        /// <summary>
        /// Create localization file source that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path">(optional) root folder</param>
        /// <param name="filename">non-rooted relative path, or rooted full path</param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public LineTreeFileSource(ILineFileFormat fileFormat, string path, string filename, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, path, filename, lineFormat, throwIfNotFound) { }

        static ILineTree[] no_trees = new ILineTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            ILineTree tree = LineReaderExtensions.ReadLineTree(FileFormat, FilePath, LineFormat, ThrowIfNotFound);
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
            ILineTree tree = LineReaderExtensions.ReadLineTree(FileFormat, FilePath, LineFormat, ThrowIfNotFound);
            ILineTree[] trees = tree == null ? no_trees : new ILineTree[] { tree };
            return ((IEnumerable<ILineTree>)trees).GetEnumerator();
        }

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new StringAsset(this, LineFormat));

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
            => new LineTreeFileSource(FileFormat, newPath, FileName, LineFormat, ThrowIfNotFound);
    }

    /// <summary>
    /// Localization file source.
    /// </summary>
    public abstract class LineFilePatternSource : FilePatternSource, IAssetSource
    {
        /// <summary>
        /// Format to use for converting lines.
        /// </summary>
        public ILineFormat LineFormat { get; protected set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILineFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="path"></param>
        /// <param name="filePattern"></param>
        /// <param name="lineFormat"></param>
        public LineFilePatternSource(ILineFileFormat fileFormat, string path, ILinePattern filePattern, ILineFormat lineFormat) : base(path, filePattern)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.LineFormat = lineFormat;
        }

        /// <summary>
        /// Convert parameters in <paramref name="key"/> into source.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract IAssetSource CreateSource(ILine key);
    }


}
