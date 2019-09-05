// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Asset builder compiles an <see cref="IAsset"/> from <see cref="IAssetConfiguration"/>.
    /// </summary>
    public class AssetBuilder : IAssetBuilder
    {
        /// <summary>
        /// Create asset builder.
        /// </summary>
        public AssetBuilder() : base() { }

        /// <summary>
        /// Build asset.
        /// 
        /// String assets are constructed into <see cref="StringAsset"/>, binary assets are into <see cref="BinaryAsset"/>.
        /// 
        /// Adds assets in following order:
        /// <list type="number">
        ///     <item>1. <see cref="IAssetConfiguration.Assets"/></item>
        ///     <item>2. <see cref="IAssetConfiguration.AssetFactories"/></item>
        ///     <item>3. <see cref="IAssetConfiguration.AssetSources"/></item>
        ///     <item>4. <see cref="IAssetConfiguration.AssetPostBuilds"/></item>
        /// </list>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="AssetException">If doesn't know how to build <paramref name="configuration"/> into <see cref="IAsset"/>.</exception>
        public virtual IAsset Build(IAssetConfiguration configuration)
        {
            // Create list of assets
            IList<IAsset> list = BuildList(configuration);

            // Compose assets into one IAsset reference
            if (list.Count == 0) return new AssetComposition.Immutable();
            if (list.Count == 1) return list[0];
            IAsset asset = new AssetComposition.Immutable(list);

            // Post-build
            foreach (IAssetPostBuild src in configuration.AssetPostBuilds)
            {
                IAsset newAsset = src.PostBuild(asset);
                if (newAsset == null) throw new AssetException($"{src.GetType().Name}.{nameof(IAssetPostBuild.PostBuild)} returned null");
                asset = newAsset;
            }

            return asset;
        }

        /// <summary>
        /// Build into a list of assets.
        /// 
        /// String assets are constructed into <see cref="StringAsset"/>, binary assets are into <see cref="BinaryAsset"/>.
        /// 
        /// Add assets into the following order:
        /// <list type="number">
        ///     <item>1. <see cref="IAssetConfiguration.Assets"/></item>
        ///     <item>2. <see cref="IAssetConfiguration.AssetFactories"/></item>
        ///     <item>3. <see cref="IAssetConfiguration.AssetSources"/></item>
        ///     <item>4. <see cref="IAssetConfiguration.AssetPostBuilds"/></item>
        /// </list>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="AssetException">If doesn't know how to build <paramref name="configuration"/> into <see cref="IAsset"/>.</exception>
        protected virtual IList<IAsset> BuildList(IAssetConfiguration configuration)
        {
            // Sorted lists
            List<IStringAsset> stringAssets = new List<IStringAsset>();
            List<IBinaryAsset> binaryAssets = new List<IBinaryAsset>();
            List<IAsset> otherAssets = new List<IAsset>();
            List<IStringAssetSource> stringAssetSources = new List<IStringAssetSource>();
            List<IBinaryAssetSource> binaryAssetSources = new List<IBinaryAssetSource>();
            List<IAssetSource> otherAssetSources = new List<IAssetSource>();

            // Sort Assets
            foreach(IAsset asset in configuration.Assets)
            {
                IStringAsset asStringAsset = asset as IStringAsset;
                IBinaryAsset asBinaryAsset = asset as IBinaryAsset;

                if (asStringAsset != null) stringAssets.Add(asStringAsset);
                if (asBinaryAsset != null) binaryAssets.Add(asBinaryAsset);
                if (asStringAsset == null && asBinaryAsset == null) otherAssets.Add(asset);
            }

            // Run AssetFactories
            foreach (IAssetFactory assetFactory in configuration.AssetFactories)
            {
                foreach (IAsset asset in configuration.AssetFactories)
                {
                    IStringAsset asStringAsset = asset as IStringAsset;
                    IBinaryAsset asBinaryAsset = asset as IBinaryAsset;

                    if (asStringAsset != null) stringAssets.Add(asStringAsset);
                    if (asBinaryAsset != null) binaryAssets.Add(asBinaryAsset);
                    if (asStringAsset == null && asBinaryAsset == null) otherAssets.Add(asset);
                }
            }

            // Sort AssetSources
            foreach (IAssetSource assetSource in configuration.AssetSources)
            {
                IStringAssetSource asStringAssetSource = assetSource as IStringAssetSource;
                IBinaryAssetSource asBinaryAssetSource = assetSource as IBinaryAssetSource;

                if (asStringAssetSource != null) stringAssetSources.Add(asStringAssetSource);
                if (asBinaryAssetSource != null) binaryAssetSources.Add(asBinaryAssetSource);
                if (asStringAssetSource == null && asBinaryAssetSource == null) otherAssetSources.Add(assetSource);
            }

            // Build BinaryAsset
            if (binaryAssets.Count > 0 || binaryAssetSources.Count > 0)
            {
                BinaryAsset binaryAsset = new BinaryAsset();
                // TODO add binary assets
                otherAssets.Insert(0, binaryAsset);
            }

            // Build StringAsset
            if (stringAssets.Count>0 || stringAssetSources.Count>0)
            {
                StringAsset stringAsset = new StringAsset();
                // TODO add string assets
                otherAssets.Insert(0, stringAsset);
            }

            return otherAssets;
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
            /// Build asset.
            /// 
            /// String assets are constructed into <see cref="StringAsset"/>, binary assets are into <see cref="BinaryAsset"/>.
            /// 
            /// Adds assets in following order:
            /// <list type="number">
            ///     <item>1. <see cref="IAssetConfiguration.Assets"/></item>
            ///     <item>2. <see cref="IAssetConfiguration.AssetFactories"/></item>
            ///     <item>3. <see cref="IAssetConfiguration.AssetSources"/></item>
            ///     <item>4. <see cref="IAssetConfiguration.AssetPostBuilds"/></item>
            /// </list>
            /// </summary>
            /// <param name="configuration"></param>
            /// <returns></returns>
            /// <exception cref="AssetException">If doesn't know how to build <paramref name="configuration"/> into <see cref="IAsset"/>.</exception>
            public override IAsset Build(IAssetConfiguration configuration)
            {
                // Create list of assets
                IList<IAsset> new_assets = BuildList(configuration);

                // Compose assets into one IAsset reference
                IAsset built_asset;
                if (new_assets.Count == 0) built_asset = new AssetComposition(); // Dummy
                else if (new_assets.Count == 1) built_asset = new_assets[0]; // as-is
                else built_asset = new AssetComposition(new_assets);

                // Post-build
                IAsset post_built_asset = built_asset;
                foreach (IAssetPostBuild src in configuration.AssetPostBuilds)
                {
                    post_built_asset = src.PostBuild(post_built_asset);
                    if (post_built_asset == null) throw new AssetException($"{src.GetType().Name}.{nameof(IAssetPostBuild.PostBuild)} returned null");
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
