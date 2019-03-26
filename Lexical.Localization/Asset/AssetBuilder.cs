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
    public class AssetBuilder : IAssetBuilder
    {
        protected List<IAssetSource> sources = new List<IAssetSource>();
        protected List<IAsset> assets = new List<IAsset>();

        public IList<IAssetSource> Sources => sources;
        
        public AssetBuilder() : base() { }

        public AssetBuilder(IEnumerable<IAssetSource> list) : base() { if (list != null) this.sources.AddRange(list); }

        public AssetBuilder(params IAssetSource[] list) : base() { if (list != null) this.sources.AddRange(list); }

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
            Dictionary<IAssetKeyNamePolicy, LoadableLocalizationStringAsset> keyLinesAssetMap = null;
            foreach (IAssetSource src in sources.Where(s => s is ILocalizationStringLinesSource))
            {
                ILocalizationStringLinesSource lineSrc = (ILocalizationStringLinesSource)src;
                if (keyLinesAssetMap == null) keyLinesAssetMap = new Dictionary<IAssetKeyNamePolicy, LoadableLocalizationStringAsset>();
                LoadableLocalizationStringAsset _asset = null;
                IAssetKeyNamePolicy policy = lineSrc.NamePolicy ?? LoadableLocalizationStringAsset.DefaultPolicy;
                if (!keyLinesAssetMap.TryGetValue(policy, out _asset)) keyLinesAssetMap[policy] = _asset = new LoadableLocalizationStringAsset(policy);
                _asset.AddLineStringSource(lineSrc, lineSrc.SourceHint);
            }
            if (keyLinesAssetMap != null)
                foreach (var _asset in keyLinesAssetMap.Values)
                    list.Add(_asset.Load());

            // Build one asset for all IEnumerable<KeyValuePair<IAssetKey, string>> sources
            LocalizationAsset __asset = null;
            foreach (IAssetSource src in sources.Where(s => s is ILocalizationKeyLinesSource))
            {
                if (__asset == null) __asset = new LocalizationAsset();
                __asset.AddKeyLinesSource((ILocalizationKeyLinesSource)src);
            }
            if (__asset != null) list.Add(__asset.Load());

            return list;
        }

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
        /// A version of ILanguageStringsBuilder that always returns the same instance when built.
        /// </summary>
        public class OneBuildInstance : AssetBuilder
        {
            public readonly IAssetComposition Asset;

            public OneBuildInstance() : this(null, null) { }
            public OneBuildInstance(IEnumerable<IAssetSource> list) : this(null, list) { }

            public OneBuildInstance(IAssetComposition composition, IEnumerable<IAssetSource> list) : base(list)
            {
                this.Asset = composition ?? new AssetComposition();
            }

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

        public override string ToString()
            => $"{GetType().Name}";
    }
}
