// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
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
    public interface IStringAsset : IAsset
    {
        /// <summary>
        /// Try to read a localization string. 
        /// 
        /// The returned string is in format format where possible arguments are numbered and inide brace parenthesis, e.g. "Welcome {0}."
        /// </summary>
        /// <param name="key"></param>
        /// <returns>line that has matching key or null</returns>
        ILine GetString(ILine key);
    }

    /// <summary>
    /// Interface to enumerate localization strings as <see cref="KeyValuePair{ILine, String}"/> lines. 
    /// 
    /// This interface is used by classes that use <see cref="ILine"/> as intrinsic keys.
    /// </summary>
    public interface IStringAssetLinesEnumerable : IAsset
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
        IEnumerable<ILine> GetLines(ILine filterKey = null);

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
        IEnumerable<ILine> GetAllLines(ILine filterKey = null);
    }

    /// <summary>
    /// Interface to enumerate localization strings as <see cref="KeyValuePair{String, String}"/> lines.
    /// 
    /// This interface is used by classes that use <see cref="String"/> as intrinsic keys.
    /// </summary>
    public interface IStringAssetStringLinesEnumerable : IAsset
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
        IEnumerable<KeyValuePair<string, IString>> GetStringLines(ILine filterKey = null);

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
        IEnumerable<KeyValuePair<string, IString>> GetAllStringLines(ILine filterKey = null);
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
        /// <returns>line with matching key or null</returns>
        public static ILine GetString(this IAsset asset, ILine key)
        {
            if (asset is IStringAsset casted) 
            {
                ILine result = casted.GetString(key);
                if (result != null) return result;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IStringAsset component in composition.GetComponents<IStringAsset>(true) ?? Enumerable.Empty<IStringAsset>())
                {
                    ILine result = component.GetString(key);
                    if (result != null) return result;
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            ILine result = loaded_asset.GetString(key);
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
                        ILine result = loaded_asset.GetString(key);
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
        public static IEnumerable<ILine> GetLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IStringAssetLinesEnumerable casted) result = casted.GetLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IStringAssetLinesEnumerable component in composition.GetComponents<IStringAssetLinesEnumerable>(true) ?? Enumerable.Empty<IStringAssetLinesEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetLines(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<ILine> _result = loaded_asset.GetLines(filterKey);
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
                        IEnumerable<ILine> _result = loaded_asset.GetLines(filterKey);
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
        public static IEnumerable<ILine> GetAllLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IStringAssetLinesEnumerable casted) result = casted.GetAllLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IStringAssetLinesEnumerable component in composition.GetComponents<IStringAssetLinesEnumerable>(true) ?? Enumerable.Empty<IStringAssetLinesEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetAllLines(filterKey);
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
                            IEnumerable<ILine> _result = loaded_asset.GetAllLines(filterKey);
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
                        IEnumerable<ILine> _result = loaded_asset.GetAllLines(filterKey);
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
        public static IEnumerable<KeyValuePair<string, IString>> GetStringLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<KeyValuePair<string, IString>> result = null;
            if (asset is IStringAssetStringLinesEnumerable casted) result = casted.GetStringLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IStringAssetStringLinesEnumerable component in composition.GetComponents<IStringAssetStringLinesEnumerable>(true) ?? Enumerable.Empty<IStringAssetStringLinesEnumerable>())
                {
                    IEnumerable<KeyValuePair<string, IString>> _result = component.GetStringLines(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<KeyValuePair<string, IString>> _result = loaded_asset.GetStringLines(filterKey);
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
                        IEnumerable<KeyValuePair<string, IString>> _result = loaded_asset.GetStringLines(filterKey);
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
        public static IEnumerable<KeyValuePair<string, IString>> GetAllStringLines(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<KeyValuePair<string, IString>> result = null;
            if (asset is IStringAssetStringLinesEnumerable casted) result = casted.GetAllStringLines(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IStringAssetStringLinesEnumerable component in composition.GetComponents<IStringAssetStringLinesEnumerable>(true) ?? Enumerable.Empty<IStringAssetStringLinesEnumerable>())
                {
                    IEnumerable<KeyValuePair<string, IString>> _result = component.GetAllStringLines(filterKey);
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
                            IEnumerable<KeyValuePair<string, IString>> _result = loaded_asset.GetAllStringLines(filterKey);
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
                        IEnumerable<KeyValuePair<string, IString>> _result = loaded_asset.GetAllStringLines(filterKey);
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
