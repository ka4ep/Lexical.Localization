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
    /// <summary>
    /// Asset source that constructs an asset from re-openable <see cref="IEnumerable{KeyValuePair{String, String}}"/> string based key-value lines.
    /// 
    /// This source also implements <see cref="ILocalizationStringLinesSource"/> which makes it capable of enumerating lines without instantating
    /// an <see cref="IAsset"/>.
    /// </summary>
    public class LocalizationStringLinesSource : IAssetSource, ILocalizationStringLinesSource
    {
        public string SourceHint { get; protected set; }
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        public IEnumerable<KeyValuePair<string, string>> LineSource { get; protected set; }

        public LocalizationStringLinesSource(IEnumerable<KeyValuePair<string, string>> lineSource, IAssetKeyNamePolicy namePolicy, string sourceHint = null)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
            this.NamePolicy = namePolicy;
            this.SourceHint = sourceHint;
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
            => LineSource.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => LineSource.GetEnumerator();
        public void Build(IList<IAsset> list)
            => list.Add(new LoadableLocalizationStringAsset(NamePolicy).AddLineStringSource(LineSource).Load());
        public IAsset PostBuild(IAsset asset)
            => asset;
        public override string ToString()
            => SourceHint ?? $"{GetType().FullName}";
    }

    /// <summary>
    /// Asset source that provides an asset from re-openable <see cref="IEnumerable{KeyValuePair{IAssetKey, String}}"/>.
    /// 
    /// This source also implements <see cref="ILocalizationKeyLinesSource"/> which makes it capable of enumerating lines without instantating
    /// an <see cref="IAsset"/>.
    /// </summary>
    public class LocalizationKeyLinesSource : IAssetSource, ILocalizationKeyLinesSource
    {
        public string SourceHint { get; protected set; }
        public IEnumerable<KeyValuePair<IAssetKey, string>> LineSource { get; protected set; }

        public LocalizationKeyLinesSource(IEnumerable<KeyValuePair<IAssetKey, string>> lineSource, string sourceHint = null)
        {
            this.LineSource = lineSource ?? throw new ArgumentNullException(nameof(lineSource));
            this.SourceHint = sourceHint;
        }

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
            => LineSource.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => LineSource.GetEnumerator();
        public void Build(IList<IAsset> list)
            => list.Add(new LoadableLocalizationAsset().AddKeyLinesSource(LineSource).Load());
        public IAsset PostBuild(IAsset asset)
            => asset;
        public override string ToString()
            => SourceHint ?? $"{GetType().FullName}";
    }

    /// <summary>
    /// Asset source that opens a <see cref="Stream"/> and converts to <see cref="IAsset"/> with <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public class StreamProviderAssetSource : IAssetSource
    {
        public ILocalizationFileFormat FileFormat { get; protected set; }
        protected Func<Stream> streamSource;
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        protected IAssetKey prefix;
        protected IAssetKey suffix;

        public StreamProviderAssetSource(ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IAssetKeyNamePolicy namePolicy, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.streamSource = streamSource ?? throw new ArgumentNullException(nameof(streamSource));
            this.NamePolicy = namePolicy;
            this.prefix = prefix;
            this.suffix = suffix;
        }
        public void Build(IList<IAsset> list)
            => list.Add(FileFormat.CreateAsset(streamSource(), NamePolicy, prefix, suffix));

        public IAsset PostBuild(IAsset asset)
            => asset;
    }

}
