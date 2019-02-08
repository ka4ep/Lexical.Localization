// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
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

        public virtual IAsset Build()
        {
            List<IAsset> list = new List<IAsset>(assets.Count + sources.Count);
            list.AddRange(assets);
            foreach (IAssetSource src in sources.ToArray())
                src.Build(list);

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
                List<IAsset> list = new List<IAsset>(assets.Count + sources.Count);
                list.AddRange(assets);
                foreach (IAssetSource src in sources.ToArray())
                    src.Build(list);

                IAsset built_asset;
                if (list.Count == 0) built_asset = new AssetComposition(); // Dummy
                else if (list.Count == 1) built_asset = list[0]; // as-is
                else built_asset = new AssetComposition(list);

                // Post-build
                IAsset post_built_asset = built_asset;
                foreach (IAssetSource src in sources.ToArray())
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
