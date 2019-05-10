// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface to read localization strings.
    /// 
    /// Consumers of this interface can use <see cref="IAssetExtensions.GetString(IAsset, ILine)"/> with uncasted <see cref="IAsset"/>.
    /// </summary>
    public interface ILocalizationStringProvider : IAsset
    {
        /// <summary>
        /// Try to read a localization string. 
        /// 
        /// The returned string is in formulation format where possible arguments are numbered and inide brace parenthesis, e.g. "Welcome {0}."
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string or null</returns>
        IFormulationString GetString(ILine key);
    }

    /// <summary>
    /// Interface to enumerate localization strings as <see cref="KeyValuePair{ILine, String}"/> lines. 
    /// 
    /// This interface is used by classes that use <see cref="ILine"/> as intrinsic keys.
    /// </summary>
    public interface ILocalizationKeyLinesEnumerable : IAsset
    {
        /// <summary>
        /// Get the lines this asset can provide. If cannot return all lines, returns null.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns>all lines, or null</returns>
        IEnumerable<ILine> GetKeyLines(ILine filterKey = null);

        /// <summary>
        /// Get all localization lines. If cannot return all lines, return what is availale.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns>lines, or null</returns>
        IEnumerable<ILine> GetAllKeyLines(ILine filterKey = null);
    }

    /// <summary>
    /// Interface to enumerate localization strings as <see cref="KeyValuePair{String, String}"/> lines.
    /// 
    /// This interface is used by classes that use <see cref="String"/> as intrinsic keys.
    /// </summary>
    public interface ILocalizationStringLinesEnumerable : IAsset
    {
        /// <summary>
        /// Gets localization key-value pairs as string keys. If cannot return all lines, then return what is available.
        /// 
        /// If the implementation cannot filter with an <see cref="ILine"/>, then it returns all available lines.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key used for filtering results</param>
        /// <returns>key to language string mapping, or null</returns>
        IEnumerable<KeyValuePair<string, IFormulationString>> GetStringLines(ILine filterKey = null);

        /// <summary>
        /// Gets all localization lines. If cannot return all keys, then returns null.
        /// 
        /// If the implementation cannot filter with an <see cref="ILine"/>, then it returns all lines.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key to get strings for</param>
        /// <returns>key to language string mapping, or null</returns>
        IEnumerable<KeyValuePair<string, IFormulationString>> GetAllStringLines(ILine filterKey = null);
    }

    public static partial class IAssetExtensions
    {
        /// <summary>
        /// Try to read a localization string.
        /// 
        /// Uses key as is and ignores culture policy.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        public static IFormulationString GetString(this IAsset asset, ILine key)
        {
            if (asset is ILocalizationStringProvider casted) 
            {
                IFormulationString result = casted.GetString(key);
                if (result != null) return result;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationStringProvider component in composition.GetComponents<ILocalizationStringProvider>(true) ?? Enumerable.Empty<ILocalizationStringProvider>())
                {
                    IFormulationString result = component.GetString(key);
                    if (result != null) return result;
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IFormulationString result = loaded_asset.GetString(key);
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
                        IFormulationString result = loaded_asset.GetString(key);
                        if (result != null) return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets localization lines as <see cref="ILine"/> keys. If it cannot return all keys, then returns what is available.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key used for filtering results</param>
        /// <returns>key to language string mapping, or null</returns>
        public static IEnumerable<ILine> GetKeyLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is ILocalizationKeyLinesEnumerable casted) result = casted.GetKeyLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationKeyLinesEnumerable component in composition.GetComponents<ILocalizationKeyLinesEnumerable>(true) ?? Enumerable.Empty<ILocalizationKeyLinesEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetKeyLines(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<ILine> _result = loaded_asset.GetKeyLines(filterKey);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(filterKey);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<ILine> _result = loaded_asset.GetKeyLines(filterKey);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Gets localization lines as <see cref="ILine"/> keys. If it cannot return all keys, then returns null.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key used for filtering results</param>
        /// <returns>key to language string mapping, or null</returns>
        public static IEnumerable<ILine> GetAllKeyLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is ILocalizationKeyLinesEnumerable casted) result = casted.GetAllKeyLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationKeyLinesEnumerable component in composition.GetComponents<ILocalizationKeyLinesEnumerable>(true) ?? Enumerable.Empty<ILocalizationKeyLinesEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetAllKeyLines(filterKey);
                    if (_result == null) return null;
                    if (_result is Array _array && _array.Length == 0) continue;
                    result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<ILine> _result = loaded_asset.GetAllKeyLines(filterKey);
                            if (_result == null) return null;
                            if (_result is Array _array && _array.Length == 0) continue;
                            result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(filterKey);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<ILine> _result = loaded_asset.GetAllKeyLines(filterKey);
                        if (_result == null) return null;
                        if (_result is Array _array && _array.Length == 0) continue;
                        result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Gets localization lines with string keys. If it cannot return all lines, then returns what is available.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key used for filtering results</param>
        /// <returns>key to language string mapping, or null</returns>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> GetStringLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<KeyValuePair<string, IFormulationString>> result = null;
            if (asset is ILocalizationStringLinesEnumerable casted) result = casted.GetStringLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationStringLinesEnumerable component in composition.GetComponents<ILocalizationStringLinesEnumerable>(true) ?? Enumerable.Empty<ILocalizationStringLinesEnumerable>())
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> _result = component.GetStringLines(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<KeyValuePair<string, IFormulationString>> _result = loaded_asset.GetStringLines(filterKey);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(filterKey);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<KeyValuePair<string, IFormulationString>> _result = loaded_asset.GetStringLines(filterKey);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets localization lines with string keys. If it cannot return all keys, then returns what is available.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If the parameter has value "", then the result must be filtered to keys that have "" for the same parameter, or don't have that same parameter assigned.
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key used for filtering results</param>
        /// <returns>key to language string mapping, or null</returns>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> GetAllStringLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<KeyValuePair<string, IFormulationString>> result = null;
            if (asset is ILocalizationStringLinesEnumerable casted) result = casted.GetAllStringLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (ILocalizationStringLinesEnumerable component in composition.GetComponents<ILocalizationStringLinesEnumerable>(true) ?? Enumerable.Empty<ILocalizationStringLinesEnumerable>())
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> _result = component.GetAllStringLines(filterKey);
                    if (_result == null) return null;
                    if (_result is Array _array && _array.Length == 0) continue;
                    result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<KeyValuePair<string, IFormulationString>> _result = loaded_asset.GetAllStringLines(filterKey);
                            if (_result == null) return null;
                            if (_result is Array _array && _array.Length == 0) continue;
                            result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> loaded_assets = provider.LoadAllAssets(filterKey);
                if (loaded_assets != null)
                {
                    foreach (IAsset loaded_asset in loaded_assets)
                    {
                        IEnumerable<KeyValuePair<string, IFormulationString>> _result = loaded_asset.GetAllStringLines(filterKey);
                        if (_result == null) return null;
                        if (_result is Array _array && _array.Length == 0) continue;
                        result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

    }

}
