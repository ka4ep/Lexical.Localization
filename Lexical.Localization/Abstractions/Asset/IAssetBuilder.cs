// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Asset
{
    #region interface
    /// <summary>
    /// Builder that can create <see cref="IAsset"/> instance(s).
    /// 
    /// For dependency injection.
    /// </summary>
    public interface IAssetBuilder
    {
        /// <summary>
        /// List of asset sources that can construct assets.
        /// </summary>
        IList<IAssetSource> Sources { get; }

        /// <summary>
        /// Build language strings.
        /// </summary>
        /// <returns></returns>
        IAsset Build();
    }
    #endregion interface

    public static partial class AssetBuilderExtensions
    {
        /// <summary>
        /// Add <paramref name="assetSource"/> to sources.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="assetSource"></param>
        public static IAssetBuilder AddSource(this IAssetBuilder assetBuilder, IAssetSource assetSource)
        {
            assetBuilder.Sources.Add(assetSource);
            return assetBuilder;
        }
    }
}
