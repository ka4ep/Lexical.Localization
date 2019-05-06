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
    public class LocalizationStringLinesSource : IAssetSource, ILocalizationStringLinesSource
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IParameterPolicy KeyPolicy { get; protected set; }

        /// <summary>
        /// Line source
        /// </summary>
        public IEnumerable<KeyValuePair<string, IFormulationString>> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;KeyValuePair&lt;String, String&gt;&gt; into <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="lineSource"></param>
        /// <param name="namePolicy"></param>
        public LocalizationStringLinesSource(IEnumerable<KeyValuePair<string, IFormulationString>> lineSource, IParameterPolicy namePolicy)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
            this.KeyPolicy = namePolicy;
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
            => list.Add(new LocalizationAsset(this, KeyPolicy));

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
    /// Asset source that provides an asset from re-openable IEnumerable&lt;KeyValuePair&lt;IAssetKey, String&lt;&lt;.
    /// </summary>
    public class LocalizationKeyLinesSource : IAssetSource, ILocalizationKeyLinesSource
    {
        /// <summary>
        /// Source of lines
        /// </summary>
        public IEnumerable<KeyValuePair<ILine, IFormulationString>> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;KeyValuePair&lt;IAssetKey, String&lt;&lt;.
        /// </summary>
        /// <param name="lineSource"></param>
        public LocalizationKeyLinesSource(IEnumerable<KeyValuePair<ILine, IFormulationString>> lineSource)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
        }

        IEnumerator<KeyValuePair<ILine, IFormulationString>> IEnumerable<KeyValuePair<ILine, IFormulationString>>.GetEnumerator()
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
    /// Asset source that provides an asset from re-openable IEnumerable&lt;IKeyTree&lt;.
    /// </summary>
    public class LocalizationKeyTreeSource : IAssetSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Source of lines
        /// </summary>
        public IEnumerable<IKeyTree> LineSource { get; protected set; }

        /// <summary>
        /// Create adapter that adapts IEnumerable&lt;IKeyTree&lt;.
        /// </summary>
        /// <param name="lineSource"></param>
        public LocalizationKeyTreeSource(IEnumerable<IKeyTree> lineSource)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
        }

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
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
    /// Asset source that opens a <see cref="Stream"/> and converts to <see cref="IAsset"/> with <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public class StreamProviderAssetSource : IAssetSource
    {
        /// <summary>
        /// File format
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Stream source
        /// </summary>
        protected Func<Stream> streamSource;

        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IParameterPolicy NamePolicy { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="streamSource"></param>
        /// <param name="namePolicy"></param>
        public StreamProviderAssetSource(ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IParameterPolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.streamSource = streamSource ?? throw new ArgumentNullException(nameof(streamSource));
            this.NamePolicy = namePolicy;
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
