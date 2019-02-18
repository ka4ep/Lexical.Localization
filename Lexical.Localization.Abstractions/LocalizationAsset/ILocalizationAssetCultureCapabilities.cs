// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    public interface ILocalizationAssetCultureCapabilities : IAsset
    {
        /// <summary>
        /// List supported cultures.
        /// 
        /// Same cultures may be returned multiple times.
        /// Caller can use <see cref="Enumerable.Distinct"/> to remove reoccurances.
        /// </summary>
        /// <returns>cultures or null of feature is not supported</returns>
        IEnumerable<CultureInfo> GetSupportedCultures();
    }

    public static partial class LocalizationAssetExtensions
    {
        /// <summary>
        /// List supported cultures.
        /// 
        /// Same cultures may be returned multiple times.
        /// Caller can use <see cref="Enumerable.Distinct"/> to remove reoccurances.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>cultures or null of feature is not supported</returns>
        public static IEnumerable<CultureInfo> GetSupportedCultures(this IAsset asset)
        {
            IEnumerable<CultureInfo> result = null;
            if (asset is ILocalizationAssetCultureCapabilities casted) result = casted.GetSupportedCultures();
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationAssetCultureCapabilities _ in composition.GetComponents<ILocalizationAssetCultureCapabilities>(true))
                {
                    IEnumerable<CultureInfo> _result = _.GetSupportedCultures();
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(null);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<CultureInfo> _result = loaded_asset.GetSupportedCultures();
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(null);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<CultureInfo> _result = loaded_asset.GetSupportedCultures();
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }
    }

}
