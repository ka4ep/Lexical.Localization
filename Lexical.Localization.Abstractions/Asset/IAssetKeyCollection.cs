// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface for assets that can enumerate all its keys.
    /// </summary>
    public interface IAssetKeyCollection : IAsset
    {
        /// <summary>
        /// Gets all keys in the asset.
        /// </summary>
        /// <param name="filterCriteria">(optional) key criteria to match with</param>
        /// <returns>keys in context-free format, or null if not supported</returns>
        IEnumerable<IAssetKey> GetAllKeys(IAssetKey filterCriteria = null);
    }

    public static partial class LocalizationAssetExtensions
    {
        /// <summary>
        /// Get all keys in asset..
        /// If <paramref name="filterCriteria"/>> is null, return all keys in the <paramref name="asset"/>.
        /// If <paramref name="filterCriteria"/>> has a selected <paramref name="culture"/>, then return <paramref name="filterCriteria"/> of that culture.
        /// If <paramref name="filterCriteria"/> doesn't have a selected culture, then return all keys of requested section from all cultures.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterCriteria">(optional) key criteria to match with</param>
        /// <returns>resolved string or null</returns>
        public static IEnumerable<IAssetKey> GetAllKeys(this IAsset asset, IAssetKey filterCriteria = null)
        {
            IEnumerable<IAssetKey> result = null;
            if (asset is IAssetKeyCollection casted) result = casted.GetAllKeys(filterCriteria);
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetKeyCollection strs in composition.GetComponents<IAssetKeyCollection>(true))
                {
                    IEnumerable<IAssetKey> _result = strs.GetAllKeys(filterCriteria);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(filterCriteria);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<IAssetKey> _result = loaded_asset.GetAllKeys(filterCriteria);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(filterCriteria);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<IAssetKey> _result = loaded_asset.GetAllKeys(filterCriteria);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

    }


}
