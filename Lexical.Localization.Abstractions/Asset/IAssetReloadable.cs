// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           11.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------

using System.Linq;

namespace Lexical.Localization
{
    public interface IAssetReloadable : IAsset
    {
        /// <summary>
        /// Reload from source files and clear caches.
        /// </summary>
        /// <returns>this</returns>
        IAsset Reload();
    }

    public static partial class AssetExtensions
    {
        /// <summary>
        /// Reload source files and clear caches.
        /// </summary>
        /// <returns></returns>
        public static IAsset Reload(this IAsset asset)
        {
            if (asset is IAssetReloadable casted) casted.Reload();
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetReloadable _ in composition.GetComponents<IAssetReloadable>(true) ?? Enumerable.Empty<IAssetReloadable>())
                    _.Reload();
            }
            return asset;
        }
    }
}
