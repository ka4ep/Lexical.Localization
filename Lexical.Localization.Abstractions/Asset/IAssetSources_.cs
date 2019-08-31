// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           30.8.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// </summary>
    public interface IAssetSources_
    {
        /// <summary>
        /// List of assets.
        /// </summary>
        IList<IAsset> Asset { get; }

        /// <summary>
        /// List of asset sources that are constructed lazily.
        /// </summary>
        IList<IAssetSource> Sources { get; }

        /// <summary>
        /// List of <see cref="IFileSystem"/>s that are used as file sources for asset files.
        /// </summary>
        IList<IFileSystem> FileSystems { get; }

        /// <summary>
        /// List of asset files. File is searched with a <see cref="IFileSystem"/>.
        /// </summary>
        IList<IAssetFile> AssetFiles { get; }

        /// <summary>
        /// List of asset file patterns. File is searched with a <see cref="IFileSystem"/>.
        /// </summary>
        IList<IAssetFilePattern> AssetFilePatterns { get; }

        /// <summary>
        /// List of asset post build actions.
        /// </summary>
        IList<IAssetPostBuild> AssetPostBuild { get; }

        /// <summary>
        /// Asset file observe policy to be attached to the built asset.
        /// </summary>
        IAssetFileObservePolicy ObservePolicy { get; set; }
    }
    // </doc>
}

