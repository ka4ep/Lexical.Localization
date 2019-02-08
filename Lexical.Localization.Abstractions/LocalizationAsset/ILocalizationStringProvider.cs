// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface read localization string.
    /// 
    /// Consumers of this interface should always call with <see cref="LocalizationAssetExtensions.GetString(IAsset, IAssetKey)"/>.
    /// </summary>
    public interface ILocalizationStringProvider : IAsset
    {
        /// <summary>
        /// Try to read a localization string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        string GetString(IAssetKey key);
    }

    /// <summary>
    /// Interface to enumerate localization strings.
    /// </summary>
    public interface ILocalizationStringCollection : IAsset
    {
        /// <summary>
        /// Gets all localization strings.
        /// </summary>
        /// <param name="key">(optional) key to get strings for</param>
        /// <returns>key to language string mapping, or null</returns>
        IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key = null);
    }

    public static partial class LocalizationAssetExtensions
    {
        /// <summary>
        /// Try to read a localization string.
        /// 
        /// Uses key as is and ignores culture policy.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        public static string GetString(this IAsset asset, IAssetKey key)
        {
            if (asset is ILocalizationStringProvider casted) 
            {
                string result = casted.GetString(key);
                if (result != null) return result;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationStringProvider strs in composition.GetComponents<ILocalizationStringProvider>(true))
                {
                    string result = strs.GetString(key);
                    if (result != null) return result;
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            string result = loaded_asset.GetString(key);
                            if (result != null) return result;
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAssets(key);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        string result = loaded_asset.GetString(key);
                        if (result != null) return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get strings of specific section described by key.
        /// If key is null, return all strings in the asset.
        /// If key has a selected <paramref name="culture"/>, then return strings of that culture.
        /// If key doesn't have a selected culture, then return strings of requested section from all cultures.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <param name="culture">culture to resolve to</param>
        /// <returns>resolved string or null</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetAllStrings(this IAsset asset, IAssetKey key = null)
        {
            IEnumerable<KeyValuePair<string, string>> result = null;
            if (asset is ILocalizationStringCollection casted) result = casted.GetAllStrings(key);
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationStringCollection strs in composition.GetComponents<ILocalizationStringCollection>(true))
                {
                    IEnumerable<KeyValuePair<string, string>> _result = strs.GetAllStrings(key);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<KeyValuePair<string, string>> _result = loaded_asset.GetAllStrings(key);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(key);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<KeyValuePair<string, string>> _result = loaded_asset.GetAllStrings(key);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

    }

}
