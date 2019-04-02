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
        ///   2. Build from <see cref="sources"/> elements that dont' implement <see cref="ILocalizationSource"/>
        ///   3. One asset for each <see cref="ILocalizationStringLinesSource"/> that share <see cref="IAssetKeyNamePolicy"/>.
        ///   4. One asset for all <see cref="ILocalizationKeyLinesSource"/>.
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
            foreach (IAssetSource src in sources.Where(s => s is ILocalizationSource == false))
                src.Build(list);

            // Build one asset for each <see cref="ILocalizationStringLinesSource"/> that share <see cref="IAssetKeyNamePolicy"/>.
            Dictionary<IAssetKeyNamePolicy, LocalizationStringAsset> keyLinesAssetMap = null;
            foreach (ILocalizationStringLinesSource src in sources.Where(s => s is ILocalizationStringLinesSource).Cast<ILocalizationStringLinesSource>())
            {
                if (keyLinesAssetMap == null) keyLinesAssetMap = new Dictionary<IAssetKeyNamePolicy, LocalizationStringAsset>();
                LocalizationStringAsset _asset = null;
                IAssetKeyNamePolicy policy = src.NamePolicy ?? ParameterNamePolicy.Instance;
                if (!keyLinesAssetMap.TryGetValue(policy, out _asset)) keyLinesAssetMap[policy] = _asset = new LocalizationStringAsset(policy);
                _asset.AddSource(src);
            }
            if (keyLinesAssetMap != null)
                foreach (var _asset in keyLinesAssetMap.Values)
                    list.Add(_asset.Load());

            // Build one asset for all IEnumerable<KeyValuePair<IAssetKey, string>> sources
            LocalizationAsset __asset = null;
            foreach (ILocalizationKeyLinesSource src in sources.Where(s => s is ILocalizationKeyLinesSource).Cast<ILocalizationKeyLinesSource>())
            {
                if (__asset == null) __asset = new LocalizationAsset();
                __asset.AddSource(src);
            }
            // ... and IEnumerable<IKeyTree> sources
            foreach (ILocalizationKeyTreeSource src in sources.Where(s => s is ILocalizationKeyTreeSource).Cast<ILocalizationKeyTreeSource>())
            {
                if (__asset == null) __asset = new LocalizationAsset();
                __asset.AddSource(src);
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
                List<IAsset> list = BuildAssets();

                IAsset built_asset;
                if (list.Count == 0) built_asset = new AssetComposition(); // Dummy
                else if (list.Count == 1) built_asset = list[0]; // as-is
                else built_asset = new AssetComposition(list);

                // Post-build
                IAsset post_built_asset = built_asset;
                foreach (IAssetSource src in sources)
                {
                    post_built_asset = src.PostBuild(post_built_asset);
                    if (post_built_asset == null) throw new AssetException($"{src.GetType().Name}.{nameof(IAssetSource.PostBuild)} returned null");
                }

                // Post-Build did something
                if (built_asset != post_built_asset)
                {
                    Asset.CopyFrom(new IAsset[] { post_built_asset });
                } else
                {
                    // Post-build did nothing
                    IEnumerable<IAsset> enumr = post_built_asset is IEnumerable<IAsset> casted ? casted : new IAsset[] { post_built_asset };
                    Asset.CopyFrom(enumr);
                }

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
