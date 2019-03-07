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
        /// Gets keys in the asset. 
        /// 
        /// This method return as many keys as it can gather from its sources, but is not guaranteed to
        /// return all of them.
        /// 
        /// If <paramref name="filterCriteria"/> is provided, then the result will contain
        /// all keys that contain the parameters in the <paramref name="filterCriteria"/>.
        /// For example if there are "Culture:en" and "Assembly:somelib.dll", then
        /// keys from that lib and localization are returned.
        /// </summary>
        /// <param name="filterCriteria">(optional) key criteria to match with</param>
        /// <returns>all or some keys, or null request was not supported</returns>
        IEnumerable<IAssetKey> GetKeys(IAssetKey filterCriteria = null);

        /// <summary>
        /// Gets all keys in the asset. 
        /// 
        /// This method is guaranteed to either return all keys that matched the <paramref name="filterCriteria"/>
        /// or return null.
        /// 
        /// If <paramref name="filterCriteria"/> is provided, then the result will contain
        /// all keys that contain the parameters in the <paramref name="filterCriteria"/>.
        /// For example if there are "Culture"="en" and "asset"="somelib.dll", then
        /// keys from that lib and localization are returned.
        /// </summary>
        /// <param name="filterCriteria">(optional) key criteria to match with</param>
        /// <returns>all keys or null</returns>
        IEnumerable<IAssetKey> GetAllKeys(IAssetKey filterCriteria = null);
    }

    public static partial class LocalizationAssetExtensions
    {
        /// <summary>
        /// Gets keys in the asset. 
        /// 
        /// This method return as many keys as it can gather from its sources, but is not guaranteed to
        /// return all of them.
        /// 
        /// If <paramref name="filterCriteria"/>> is null, return matching keys in the <paramref name="asset"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterCriteria">(optional) key criteria to match with</param>
        /// <returns>resolved string or null</returns>
        public static IEnumerable<IAssetKey> GetKeys(this IAsset asset, IAssetKey filterCriteria = null)
        {
            IEnumerable<IAssetKey> result = null;
            if (asset is IAssetKeyCollection casted) result = casted.GetKeys(filterCriteria);
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetKeyCollection strs in composition.GetComponents<IAssetKeyCollection>(true))
                {
                    IEnumerable<IAssetKey> _result = strs.GetKeys(filterCriteria);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(filterCriteria);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<IAssetKey> _result = loaded_asset.GetKeys(filterCriteria);
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
                        IEnumerable<IAssetKey> _result = loaded_asset.GetKeys(filterCriteria);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all keys in the asset. 
        /// 
        /// This method is guaranteed to either return all keys that matched the <paramref name="filterCriteria"/>
        /// or return null.
        /// 
        /// If <paramref name="filterCriteria"/>> is null, return all keys in the <paramref name="asset"/>.
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
                    if (_result == null) return null;
                    result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(filterCriteria);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<IAssetKey> _result = loaded_asset.GetAllKeys(filterCriteria);
                            if (_result == null) return null;
                            result = result == null ? _result : result.Concat(_result);
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
                        if (_result == null) return null;
                        result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// This implementation can be used for filtering an enumerable of keys.
        /// </summary>
        /// <param name="keys">keys to filter</param>
        /// <param name="criteriaKey">criteria key to use for filtering</param>
        /// <returns>filtered keys</returns>
        public static IEnumerable<IAssetKey> FilterKeys(this IEnumerable<IAssetKey> keys, IAssetKey criteriaKey = null)
        {
            // Filter
            if (criteriaKey != null)
            {
                KeyValuePair<string, string>[] filterParameters = criteriaKey.GetParameters(true);
                if (filterParameters.Length > 0) keys = FilterKeys(keys, filterParameters);
            }
            return keys;
        }

        /// <summary>
        /// This implementation can be used for filtering an enumerable of keys.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public static IEnumerable<IAssetKey> FilterKeys(this IEnumerable<IAssetKey> keys, KeyValuePair<string, string>[] filterParameters)
        {
            foreach (IAssetKey key in keys)
            {
                // Filter by criteria key
                bool ok = true;
                // Iterate all criteria parameters (key,value)
                foreach (var filterParameter in filterParameters)
                {
                    bool okk = false;
                    for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                    {
                        if (k.GetParameterName() == filterParameter.Key)
                        {
                            okk = k.Name == filterParameter.Value;
                            break;
                        }
                    }

                    // criteria did not match, go to next line
                    ok &= okk;
                    if (!ok) break;
                }

                if (ok) yield return key;
            }
        }

    }


}
