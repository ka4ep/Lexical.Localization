// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    public static partial class AssetCacheExtensions
    {
        /// <summary>
        /// Add cache feature to asset builder.
        /// 
        /// Adds <see cref="AssetCachePartResources"/>, <see cref="AssetStringsCachePart"/> and <see cref="AssetCulturesCachePart"/>.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="configureCache">(optional) add custom cache features here/param>
        /// <returns></returns>
        public static IAssetBuilder AddCache(this IAssetBuilder assetBuilder, Action<IAssetCache> configureCache = default)
        {
            Action<IAssetCache> func = cache =>
            {
                IAssetCache c = cache.AddResourceCache().AddStringsCache().AddCulturesCache();
                if (configureCache != null) configureCache(c);
            };
            assetBuilder.Sources.Add(new AssetCacheSource(func));
            return assetBuilder;
        }

        /// <summary>
        /// Wrap asset in cache decorator.
        /// 
        /// Adds <see cref="AssetCachePartResources"/>, <see cref="AssetStringsCachePart"/> and <see cref="AssetCulturesCachePart"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="configureCache">(optional) add custom cache features here/param>
        /// <returns>asset with cache</returns>
        public static IAssetCache CreateCache(this IAsset asset, Action<IAssetCache> configureCache = default)
        {
            IAssetCache cache = new AssetCache(asset)
                .AddResourceCache()
                .AddStringsCache()
                .AddCulturesCache();
            if (configureCache!=null) configureCache(cache);
            return cache;
        }
    }
}
