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
    /// <summary>
    /// Asset source that constructs an asset from re-openable IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt; string based key-value lines.
    /// </summary>
    public class StringLinesSource : IAssetSource, IStringLineSource
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILineFormat LineFormat { get; protected set; }

        /// <summary>
        /// Line source
        /// </summary>
        public IEnumerable<KeyValuePair<string, IFormulationString>> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt; into <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="lineSource"></param>
        /// <param name="lineFormat"></param>
        public StringLinesSource(IEnumerable<KeyValuePair<string, IFormulationString>> lineSource, ILineFormat lineFormat)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
            this.LineFormat = lineFormat;
        }

        IEnumerator<KeyValuePair<string, IFormulationString>> IEnumerable<KeyValuePair<string, IFormulationString>>.GetEnumerator()
            => LineSource.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => LineSource.GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset(this, LineFormat));

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;

        /// <summary>
        /// Print info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().FullName;
    }

    /// <summary>
    /// Asset source that provides an asset from re-openable IEnumerable&lt;ILine&lt;.
    /// </summary>
    public class KeyLineSource : IAssetSource, IKeyLineSource
    {
        /// <summary>
        /// Source of lines
        /// </summary>
        public IEnumerable<ILine> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;KeyValuePair&lt;ILine, String&lt;&lt;.
        /// </summary>
        /// <param name="lineSource"></param>
        public KeyLineSource(IEnumerable<ILine> lineSource)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
        }

        IEnumerator<ILine> IEnumerable<ILine>.GetEnumerator()
            => LineSource.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => LineSource.GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().Add(LineSource).Load());

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;
    }

    /// <summary>
    /// Asset source that provides an asset from re-openable IEnumerable&lt;ILineTree&lt;.
    /// </summary>
    public class LineTreeSource : IAssetSource, ILineTreeSource
    {
        /// <summary>
        /// Source of lines
        /// </summary>
        public IEnumerable<ILineTree> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;ILineTree&lt;.
        /// </summary>
        /// <param name="lineSource"></param>
        public LineTreeSource(IEnumerable<ILineTree> lineSource)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
        }

        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
            => LineSource.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => LineSource.GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().Add(LineSource).Load());

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;
    }

    /// <summary>
    /// Asset source that opens a <see cref="Stream"/> and converts to <see cref="IAsset"/> with <see cref="ILineFileFormat"/>.
    /// </summary>
    public class StreamProviderAssetSource : IAssetSource
    {
        /// <summary>
        /// File format
        /// </summary>
        public ILineFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Stream source
        /// </summary>
        protected Func<Stream> streamSource;

        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public ILineFormat NamePolicy { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="streamSource"></param>
        /// <param name="lineFormat"></param>
        public StreamProviderAssetSource(ILineFileFormat fileFormat, Func<Stream> streamSource, ILineFormat lineFormat)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.streamSource = streamSource ?? throw new ArgumentNullException(nameof(streamSource));
            this.NamePolicy = lineFormat;
        }

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
            => list.Add(FileFormat.StreamAsset(streamSource(), NamePolicy));

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;
    }

}
