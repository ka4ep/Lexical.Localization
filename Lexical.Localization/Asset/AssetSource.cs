// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Adapts single <see cref="IAsset"/> to <see cref="IAssetSource"/>.
    /// </summary>
    public class AssetSource : IAssetSource
    {
        /// <summary>
        /// Asset to add
        /// </summary>
        public readonly IAsset asset;

        /// <summary>
        /// Create adapter that adapts <paramref name="asset"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="asset"></param>
        public AssetSource(IAsset asset)
        {
            this.asset = asset ?? throw new ArgumentNullException(nameof(asset));
        }

        /// <summary>
        /// Add <see cref="asset"/> to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
        {
            list.Add(asset);
        }

        /// <summary>
        /// Post build.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
            => asset;

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Add localization file source.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional)</param>
        /// <param name="fileFormat">(optional) overriding file format to use in case format cannot be infered from file extension</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">thrown if fileformat was not found</exception>
        public static IAssetBuilder AddLocalizationFile(this IAssetBuilder assetBuilder, string filename, IAssetKeyNamePolicy namePolicy = default, ILocalizationFileFormat fileFormat = null)
        {
            if (fileFormat == null) fileFormat = LocalizationReaderMap.Instance.GetFormatByFilename(filename);
            assetBuilder.AddSource( fileFormat.FileAssetSource(filename, namePolicy) );
            return assetBuilder;
        }

        /// <summary>
        /// Add localization file source.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional)</param>
        /// <param name="fileFormat">(optional) overriding file format to use in case format cannot be infered from file extension</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">thrown if fileformat was not found</exception>
        public static IAssetBuilder AddEmbeddedLocalizationFile(this IAssetBuilder assetBuilder, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy = default, ILocalizationFileFormat fileFormat = null)
        {
            if (fileFormat == null) fileFormat = LocalizationReaderMap.Instance.GetFormatByFilename(resourceName);
            assetBuilder.AddSource( fileFormat.EmbeddedAssetSource(assembly, resourceName, namePolicy) );
            return assetBuilder;
        }

    }
}
