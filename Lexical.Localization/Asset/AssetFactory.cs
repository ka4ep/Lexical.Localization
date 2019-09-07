// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Default asset factory.
    /// </summary>
    public class AssetFactory : IAssetFactory
    {
        /// <summary>
        /// Asset to add
        /// </summary>
        public readonly IAsset[] Assets;

        /// <summary>
        /// Create adapter that adapts <paramref name="asset"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="asset"></param>
        public AssetFactory(IAsset asset)
        {
            this.Assets = new IAsset[] { asset ?? throw new ArgumentNullException(nameof(asset)) };
        }

        /// <summary>
        /// Create adapter that adapts <paramref name="assets"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="assets"></param>
        public AssetFactory(params IAsset[] assets)
        {
            this.Assets = assets ?? throw new ArgumentNullException(nameof(assets));
        }

        /// <summary>
        /// Return the asset.
        /// </summary>
        /// <returns>the asset</returns>
        public IEnumerable<IAsset> Create()
        {
            return Assets;
        }

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().Name + "(" + String.Join<IAsset>(", ", Assets) + ")";
    }
}
