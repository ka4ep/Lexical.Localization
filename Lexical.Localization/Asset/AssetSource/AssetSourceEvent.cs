// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Asset source event.
    /// </summary>
    public class AssetSourceEvent : IAssetSourceEvent
    {
        /// <summary>
        /// The asset source that is being observed
        /// </summary>
        public IAssetSource AssetSource { get; protected set; }

        /// <summary>
        /// Change events
        /// </summary>
        public WatcherChangeTypes ChangeEvents { get; protected set; }

        /// <summary>
        /// Create event.
        /// </summary>
        /// <param name="assetSource"></param>
        /// <param name="changeEvents"></param>
        public AssetSourceEvent(IAssetSource assetSource, WatcherChangeTypes changeEvents)
        {
            AssetSource = assetSource;
            ChangeEvents = changeEvents;
        }
    }
}
