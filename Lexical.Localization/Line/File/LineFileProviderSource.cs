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
    using Lexical.Localization.Asset;
    using Lexical.Localization.StringFormat;
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Localization source and reader that reads lines from file provider.
    /// </summary>
    public abstract class LineFileProviderSource : FileProviderSource, IAssetSource, IEnumerable
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILineFormat LineFormat { get; internal set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILineFileFormat FileFormat { get; internal set; }

        /// <summary>
        /// Create (abstract) source to localization file in a file provider.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public LineFileProviderSource(ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat, bool throwIfNotFound) : base(fileProvider, filepath, throwIfNotFound)
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
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;
    /// </summary>
    public class StringLineFileProviderSource : LineFileProviderSource, IStringLineSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public StringLineFileProviderSource(ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<string, IString>> empty = new KeyValuePair<string, IString>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, IString>> IEnumerable<KeyValuePair<string, IString>>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadStringLines(s, LineFormat).GetEnumerator();
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
            => ((IEnumerable<KeyValuePair<string, IString>>)this).GetEnumerator();

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
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;ILine, string&gt;&gt;.
    /// </summary>
    public class KeyLineFileProviderSource : LineFileProviderSource, IKeyLineSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public KeyLineFileProviderSource(ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound) { }

        static IEnumerable<ILine> empty = new ILine[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILine> IEnumerable<ILine>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadLines(s, LineFormat).GetEnumerator();
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
            => ((IEnumerable<ILine>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new StringAsset().Add(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{ILineTree}"/>.
    /// </summary>
    public class LineTreeFileProviderSource : LineFileProviderSource, ILineTreeSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="lineFormat"></param>
        /// <param name="throwIfNotFound"></param>
        public LineTreeFileProviderSource(ILineFileFormat fileFormat, IFileProvider fileProvider, string filepath, ILineFormat lineFormat, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, lineFormat, throwIfNotFound) { }

        static IEnumerable<ILineTree> empty = new ILineTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                {
                    ILineTree tree = FileFormat.ReadLineTree(s, LineFormat);
                    if (tree == null) return empty.GetEnumerator();
                    ILineTree[] trees = new ILineTree[] { tree };
                    return ((IEnumerable<ILineTree>)trees).GetEnumerator();
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
            => ((IEnumerable<ILineTree>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new StringAsset().Add(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }

}
