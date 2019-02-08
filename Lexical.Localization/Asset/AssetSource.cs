// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts single <see cref="IAsset"/> to <see cref="IAssetSource"/>.
    /// </summary>
    public class AssetSource : IAssetSource
    {
        public readonly IAsset asset;

        public AssetSource(IAsset asset)
        {
            this.asset = asset ?? throw new ArgumentNullException(nameof(asset));
        }

        public void Build(IList<IAsset> list)
        {
            list.Add(asset);
        }

        public IAsset PostBuild(IAsset asset)
            => asset;

        public override string ToString() 
            => $"{GetType().Name}({asset.ToString()})";
    }


    public static partial class AssetExtensions_
    {
        /// <summary>
        /// Adapt <see cref="IAsset"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static IAssetSource ToSource(this IAsset asset)
            => new AssetSource(asset);

        /// <summary>
        /// Adapts <see cref="IAsset"/> to <see cref="IAssetSource"/> and adds to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="asset"></param>
        /// <returns>builder</returns>
        public static IAssetBuilder AddAsset(this IAssetBuilder builder, IAsset asset)
        {
            builder.Sources.Add(new AssetSource(asset));
            return builder;
        }

    }
}
