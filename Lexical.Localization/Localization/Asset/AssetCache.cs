// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Asset
{
    /// <summary>
    /// Bases for cache. Individual cache features for different interfaces need to be added separately.
    /// </summary>
    public class AssetCache : AssetComposition, IAssetCache
    {
        public AssetCacheOptions Options { get; }
        public IAsset Source { get; internal set; }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose requests are cached.</param>
        public AssetCache(IAsset source)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        public AssetCache(IAsset source, params IAssetCachePart[] cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        public AssetCache(IAsset source, IEnumerable<IAssetCachePart> cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }

    /// <summary>
    /// Add <see cref="AssetCacheSource"/> to <see cref="IAssetBuilder"/> to have it add cache instances on new asset builds.
    /// </summary>
    public class AssetCacheSource : IAssetSource
    {
        Action<IAssetCache> configurer;

        public AssetCacheSource(Action<IAssetCache> configurer)
        {
            this.configurer = configurer;
        }

        public void Build(IList<IAsset> list)
        {
        }

        public IAsset PostBuild(IAsset asset)
        {
            AssetCache cache = new AssetCache(asset);
            configurer(cache);
            return cache;
        }
    }

    /*
    public static partial class AssetCacheExtensions_
    {
        /// <summary>
        /// Add cache feature to asset builder.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="configureCache">add here options and cache features, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart/param>
        /// <returns></returns>
        public static IAssetBuilder AddCache(this IAssetBuilder assetBuilder, Action<IAssetCache> configureCache)
        {
            assetBuilder.Sources.Add(new AssetCacheSource(configureCache));
            return assetBuilder;
        }

        /// <summary>
        /// Wrap asset in cache decorator
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="configureCache">add here options and cache features, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart/param>
        /// <returns>cache decorator</returns>
        public static IAsset AddCache(this IAsset asset, Action<IAssetCache> configureCache)
        {
            AssetCache cache = new AssetCache(asset);
            configureCache(cache);
            return cache;
        }
    }*/

}
