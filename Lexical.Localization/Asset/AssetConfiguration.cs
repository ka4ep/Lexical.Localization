// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System.Collections.Generic;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Configuration that describes asset sources. Used with <see cref="IAssetBuilder"/> to construct an <see cref="IAsset"/> in dependency-injection environment.
    /// </summary>
    public class AssetConfiguration : IAssetConfiguration
    {
        /// <summary>
        /// List of <see cref="IAsset"/>.
        /// </summary>
        public virtual IList<IAsset> Assets { get; } = new CopyOnWriteList<IAsset>();

        /// <summary>
        /// List of <see cref="IAssetFactory"/>.
        /// </summary>
        public virtual IList<IAssetFactory> AssetFactories { get; } = new CopyOnWriteList<IAssetFactory>();

        /// <summary>
        /// List of <see cref="IAssetSource"/>.
        /// </summary>
        public virtual IList<IAssetSource> AssetSources { get; } = new CopyOnWriteList<IAssetSource>();

        /// <summary>
        /// List of <see cref="IFileSystem"/>.
        /// </summary>
        public virtual IList<IFileSystem> FileSystems { get; } = new CopyOnWriteList<IFileSystem>();

        /// <summary>
        /// List of <see cref="IAssetPostBuild"/>.
        /// </summary>
        public virtual IList<IAssetPostBuild> AssetPostBuilds { get; } = new CopyOnWriteList<IAssetPostBuild>();

        /// <summary>
        /// Observe policy
        /// </summary>
        public virtual IAssetObservePolicy AssetObservePolicy { get; set; }

        /// <summary>
        /// Create asset configuration.
        /// </summary>
        public AssetConfiguration()
        {
        }

        /// <summary>
        /// Create asset configuration.
        /// </summary>
        /// <param name="list"></param>
        public AssetConfiguration(IEnumerable<IAssetSource> list)
        {
            if (list != null)
                foreach (var source in list)
                    this.AssetSources.Add(source);
        }

        /// <summary>
        /// Create asset configuration.
        /// </summary>
        /// <param name="list"></param>
        public AssetConfiguration(params IAssetSource[] list)
        {
            if (list != null)
                foreach (var source in list)
                    this.AssetSources.Add(source);
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}";
    }
}
