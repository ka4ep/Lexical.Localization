// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Asset builder compiles <see cref="IAsset"/>s from <see cref="IAssetSource"/> into one asset.
    /// </summary>
    public class AssetBuilder : IAssetBuilder
    {
        /// <summary>
        /// Asset sources
        /// </summary>
        protected List<IAssetSource> sources = new List<IAssetSource>();

        /// <summary>
        /// Fixed assets
        /// </summary>
        protected List<IAsset> assets = new List<IAsset>();

        /// <summary>
        /// List of asset sources
        /// </summary>
        public IList<IAssetSource> Sources => sources;
        
        /// <summary>
        /// Create asset builder.
        /// </summary>
        public AssetBuilder() : base() { }

        /// <summary>
        /// Create asset builder.
        /// </summary>
        /// <param name="list"></param>
        public AssetBuilder(IEnumerable<IAssetSource> list) : base() { if (list != null) this.sources.AddRange(list); }

        /// <summary>
        /// Create asset builder.
        /// </summary>
        /// <param name="list"></param>
        public AssetBuilder(params IAssetSource[] list) : base() { if (list != null) this.sources.AddRange(list); }

        /// <summary>
        /// Add fixed asset.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IAssetBuilder AddAsset(IAsset source)
        {
            assets.Add(source);
            return this;
        }

        /// <summary>
        /// Builds a list of assets. Adds the following:
        ///   1. The list of <see cref="assets"/> as is
        ///   2. Build from <see cref="sources"/> elements that dont' implement <see cref="ILineSource"/>
        ///   3. One asset for each <see cref="IStringLineSource"/> that share <see cref="ILineFormat"/>.
        ///   4. One asset for all <see cref="IKeyLineSource"/>.
        ///   
        /// </summary>
        /// <returns></returns>
        protected List<IAsset> BuildAssets()
        {
            // Result asset list
            List<IAsset> list = new List<IAsset>();

            // Add direct IAssets
            list.AddRange(assets);

            // Build IAssetSources
            foreach (IAssetSource src in sources.Where(s => s is ILineSource == false))
                src.Build(list);

            // Build one asset for all IEnumerable<KeyValuePair<ILine, IFormatString>> sources
            StringAsset __asset = null;
            foreach (IStringLineSource src in sources.Where(s => s is IStringLineSource).Cast<IStringLineSource>())
            {
                if (__asset == null) __asset = new StringAsset();
                __asset.Add(src, src.LineFormat);
            }
            // Build one asset for all IEnumerable<KeyValuePair<ILine, IFormatString>> sources
            foreach (IKeyLineSource src in sources.Where(s => s is IKeyLineSource).Cast<IKeyLineSource>())
            {
                if (__asset == null) __asset = new StringAsset();
                __asset.Add(src);
            }
            // ... and IEnumerable<ILineTree> sources
            foreach (ILineTreeSource src in sources.Where(s => s is ILineTreeSource).Cast<ILineTreeSource>())
            {
                if (__asset == null) __asset = new StringAsset();
                __asset.Add(src);
            }
            if (__asset != null) list.Add(__asset.Load());

            return list;
        }

        /// <summary>
        /// Build asset
        /// </summary>
        /// <returns></returns>
        public virtual IAsset Build()
        {
            // Create list of assets
            List<IAsset> list = BuildAssets();

            // Build
            if (list.Count == 0) return new AssetComposition.Immutable();
            if (list.Count == 1) return list[0];
            IAsset asset = new AssetComposition.Immutable(list);

            // Post-build
            foreach (IAssetSource src in sources.ToArray())
            {
                IAsset newAsset = src.PostBuild(asset);
                if (newAsset == null) throw new AssetException($"{src.GetType().Name}.{nameof(IAssetSource.PostBuild)} returned null");
                asset = newAsset;
            }

            return asset;
        }

        /// <summary>
        /// A version of <see cref="IAssetBuilder"/> that always returns the same instance when built.
        /// </summary>
        public class OneBuildInstance : AssetBuilder
        {
            /// <summary>
            /// One instance that can be refered even before building asset.
            /// </summary>
            public readonly IAssetComposition Asset;

            /// <summary>
            /// Create asset builder that always builds result to one instance <see cref="Asset"/>.
            /// </summary>
            public OneBuildInstance() : this(null, null) { }

            /// <summary>
            /// Create asset builder that always builds result to one instance <see cref="Asset"/>.
            /// </summary>
            public OneBuildInstance(IEnumerable<IAssetSource> list) : this(null, list) { }

            /// <summary>
            /// Create asset builder that always builds result to one instance <see cref="Asset"/>.
            /// </summary>
            public OneBuildInstance(IAssetComposition composition, IEnumerable<IAssetSource> list) : base(list)
            {
                this.Asset = composition ?? new AssetComposition();
            }

            /// <summary>
            /// Build assets. The contents of <see cref="Asset"/> is updated.
            /// </summary>
            /// <returns><see cref="Asset"/></returns>
            public override IAsset Build()
            {
                // Create list of assets
                List<IAsset> new_assets = BuildAssets();

                IAsset built_asset;
                if (new_assets.Count == 0) built_asset = new AssetComposition(); // Dummy
                else if (new_assets.Count == 1) built_asset = new_assets[0]; // as-is
                else built_asset = new AssetComposition(new_assets);

                // Post-build
                IAsset post_built_asset = built_asset;
                foreach (IAssetSource src in sources)
                {
                    post_built_asset = src.PostBuild(post_built_asset);
                    if (post_built_asset == null) throw new AssetException($"{src.GetType().Name}.{nameof(IAssetSource.PostBuild)} returned null");
                }

                // Get old assets
                HashSet<IAsset> old_assets = new HashSet<IAsset>(Asset);

                // Assign new assets
                if (built_asset != post_built_asset)
                {
                    // Post-Build did something
                    Asset.CopyFrom(new IAsset[] { post_built_asset });
                } else
                {
                    // Post-build did nothing
                    IEnumerable<IAsset> enumr = post_built_asset is IEnumerable<IAsset> casted ? casted : new IAsset[] { post_built_asset };
                    Asset.CopyFrom(enumr);
                }

                // Dispose removed assets
                foreach (IAsset asset in new_assets) old_assets.Remove(asset);
                foreach (IAsset asset in old_assets) asset.Dispose();
                // TODO? IS disposing of cache handled correctly?

                return Asset;
            }
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}";
    }
}
