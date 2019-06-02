// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Source that can send events.
    /// </summary>
    public interface IAssetObservable : IAsset, IObservable<IAssetEvent>
    {
    }

    /// <summary>
    /// Source that can send events.
    /// </summary>
    public interface IAssetSourceObservable : IAssetSource, IObservable<IAssetSourceEvent>
    {
    }

    /// <summary>
    /// Asset source event.
    /// </summary>
    public interface IAssetEvent
    {
        /// <summary>
        /// Affected asset
        /// </summary>
        IAsset Asset { get; }
    }

    /// <summary>
    /// Asset source event.
    /// </summary>
    public interface IAssetSourceEvent
    {
        /// <summary>
        /// Affected source
        /// </summary>
        IAssetSource Source { get; }
    }

    /// <summary>
    /// Change event, for <see cref="IAssetEvent"/> and <see cref="IAssetSourceEvent"/>.
    /// </summary>
    public interface IAssetChangeEvent
    {
        /// <summary>
        /// Change type
        /// </summary>
        WatcherChangeTypes ChangeType { get; }
    }

    /// <summary>
    /// Source has changed.
    /// </summary>
    public class AssetChangedEvent : IAssetEvent, IAssetChangeEvent
    {
        /// <summary>
        /// Affected asset
        /// </summary>
        public IAsset Asset { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        public WatcherChangeTypes ChangeType { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="changeType"></param>
        public AssetChangedEvent(IAsset asset, WatcherChangeTypes changeType)
        {
            Asset = asset;
            ChangeType = changeType;
        }
    }

    /// <summary>
    /// Source has changed.
    /// </summary>
    public class AssetSourceChangedEvent : IAssetSourceEvent, IAssetChangeEvent
    {
        /// <summary>
        /// Affected source
        /// </summary>
        public IAssetSource Source { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        public WatcherChangeTypes ChangeType { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="changeType"></param>
        public AssetSourceChangedEvent(IAssetSource source, WatcherChangeTypes changeType)
        {
            Source = source;
            ChangeType = changeType;
        }
    }

    public static partial class IAssetExtensions
    {
    }
}
