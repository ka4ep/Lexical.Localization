// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Interface for reading binary resources.
    /// 
    /// Consumers of this interface should always call with <see cref="IAssetExtensions.GetResource(IAsset, ILine)"/>.
    /// </summary>
    public interface IResourceAsset : IAsset
    {
        /// <summary>
        /// Try to read a binary resource by matching <paramref name="key"/> as is.
        /// 
        /// Does not apply contextual information from executing context. (See <see cref="ILineExtensions.ResolveBytes(ILine)"/> to match in context.)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved resource, or null or resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        byte[] GetResource(ILine key);

        /// <summary>
        /// Try to open a stream to a resource by matching <paramref name="key"/> as is.
        /// 
        /// Does not apply contextual information from executing context. (See <see cref="ILineExtensions.ResolveStream(ILine)"/> to match in context.)
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved resource in an open stream, or null if resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        Stream OpenStream(ILine key);
    }

    /// <summary>
    /// Interface to enumerate binary localization resources as <see cref="ILine"/> references.
    /// 
    /// If the implementing class uses filenames, the recommended guideline is to use "Location" parameters to refer to folder.
    /// The last "Location" parameter refers to a file.
    /// if the implementation handles following folder:
    /// <list type="bullet">
    /// <item>Assets/</item>
    /// <item>Assets/Gfx/</item>
    /// <item>Assets/Gfx/Logo.svg</item>
    /// </list>
    /// 
    /// Then it would return a key as <code>root.Location("Assets").Location("Gfx").Location("Logo.svg");</code>.
    /// 
    /// If the implementing class returns embedded resources from internal assemblies, then the recommended guideline is to return
    /// "Assembly" part to refer to the <see cref="AssemblyName.Name"/>, and "BaseName" to refer to any resource part in the .dll.
    /// 
    /// For example, for resource file "mylib.dll/mylib.resources.Logo.svg" the key could be composed as 
    /// <code>root.Assembly("mylib").BaseName("mylib.resources").Key("Logo.svg");</code>.
    /// </summary>
    public interface IResourceAssetKeysEnumerable : IAsset
    {
        /// <summary>
        /// Get the resource keys that could be resolved. 
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        IEnumerable<ILine> GetResourceKeys(ILine filterKey = null);

        /// <summary>
        /// Get all resource keys, or if every key cannot be provided returns null.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        IEnumerable<ILine> GetAllResourceKeys(ILine filterKey = null);
    }

    /// <summary>
    /// Interface to enumerate binary localization resources as strings identifier, which is typically a filename.
    /// 
    /// Note, that the intrinsic key class is <see cref="ILine"/> and not string, therefore
    /// is is encouraged to use and implement <see cref="IResourceAssetKeysEnumerable"/> instead.
    /// </summary>
    public interface IResourceAssetNamesEnumerable : IAsset
    {
        /// <summary>
        /// Get all resource names as string keys.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        IEnumerable<string> GetResourceNames(ILine filterKey = null);

        /// <summary>
        /// Get all resource names.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If filterKey has a parameter with value "", then the comparand key must not have the key, or have it with value "".
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>all resource names, or null</returns>
        IEnumerable<string> GetAllResourceNames(ILine filterKey = null);
    }
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    public static partial class IAssetExtensions
    {
        /// <summary>
        /// Try to read a resource.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>resolved string or null or resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        public static byte[] GetResource(this IAsset asset, ILine key)
        {
            if (asset is IResourceAsset casted)
            {
                byte[] data = casted.GetResource(key);
                if (data != null) return data;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAsset component in composition.GetComponents<IResourceAsset>(true) ?? Enumerable.Empty<IResourceAsset>())
                {
                    byte[] data = component.GetResource(key);
                    if (data != null) return data;
                }
                foreach(IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            byte[] data = loaded_asset.GetResource(key);
                            if (data != null) return data;
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
                        byte[] data = loaded_asset.GetResource(key);
                        if (data != null) return data;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Try to open a stream to a resource.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>resolved string or null if resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        public static Stream OpenStream(this IAsset asset, ILine key)
        {
            if (asset is IResourceAsset casted)
            {
                Stream steam = casted.OpenStream(key);
                if (steam != null) return steam;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAsset component in composition.GetComponents<IResourceAsset>(true) ?? Enumerable.Empty<IResourceAsset>())
                {
                    Stream steam = component.OpenStream(key);
                    if (steam != null) return steam;
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            Stream steam = loaded_asset.OpenStream(key);
                            if (steam != null) return steam;
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
                        Stream steam = loaded_asset.OpenStream(key);
                        if (steam != null) return steam;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get resource names as string keys. If all cannot be returned, returns what is available.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If the parameter has value "", then the result must be filtered to keys that have "" for the same parameter, or don't have that same parameter assigned.
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        public static IEnumerable<string> GetResourceNames(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<string> result = null;
            if (asset is IResourceAssetNamesEnumerable casted) result = casted.GetResourceNames(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAssetNamesEnumerable component in composition.GetComponents<IResourceAssetNamesEnumerable>(true) ?? Enumerable.Empty<IResourceAssetNamesEnumerable>())
                {
                    IEnumerable<string> _result = component.GetResourceNames(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<string> _result = loaded_asset.GetResourceNames(filterKey);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(filterKey);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<string> _result = loaded_asset.GetResourceNames(filterKey);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all resource names as string keys. If all cannot be returned, returns null.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If the parameter has value "", then the result must be filtered to keys that have "" for the same parameter, or don't have that same parameter assigned.
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        public static IEnumerable<string> GetAllResourceNames(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<string> result = null;
            if (asset is IResourceAssetNamesEnumerable casted) result = casted.GetAllResourceNames(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAssetNamesEnumerable component in composition.GetComponents<IResourceAssetNamesEnumerable>(true) ?? Enumerable.Empty<IResourceAssetNamesEnumerable>())
                {
                    IEnumerable<string> _result = component.GetAllResourceNames(filterKey);
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
                            IEnumerable<string> _result = loaded_asset.GetAllResourceNames(filterKey);
                            if (_result == null) return null;
                            if (_result is Array _array && _array.Length == 0) continue;
                            result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(filterKey);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<string> _result = loaded_asset.GetAllResourceNames(filterKey);
                        if (_result == null) return null;
                        if (_result is Array _array && _array.Length == 0) continue;
                        result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get resource names as string keys. If all cannot be returned, returns what is available.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If the parameter has value "", then the result must be filtered to keys that have "" for the same parameter, or don't have that same parameter assigned.
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        public static IEnumerable<ILine> GetResourceKeys(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IResourceAssetKeysEnumerable casted) result = casted.GetResourceKeys(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAssetKeysEnumerable component in composition.GetComponents<IResourceAssetKeysEnumerable>(true) ?? Enumerable.Empty<IResourceAssetKeysEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetResourceKeys(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<ILine> _result = loaded_asset.GetResourceKeys(filterKey);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(filterKey);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<ILine> _result = loaded_asset.GetResourceKeys(filterKey);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all resource names as string keys. If all cannot be returned, returns null.
        /// 
        /// If <paramref name="filterKey"/> is provided, then the resulted lines are filtered based on the parameters in the <paramref name="filterKey"/>.
        /// If <paramref name="filterKey"/> has parameter assignment(s) <see cref="ILineParameter"/>, then result must be filtered to lines that have matching value for each parameter.
        /// If the parameter has value "", then the result must be filtered to keys that have "" for the same parameter, or don't have that same parameter assigned.
        /// 
        /// The returned enumerable must be multi-thread safe. If the implementing class is mutable or <see cref="IAssetReloadable"/>, then
        /// it must return an enumerable that is a snapshot and will not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filterKey">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        public static IEnumerable<ILine> GetAllResourceKeys(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IResourceAssetKeysEnumerable casted) result = casted.GetAllResourceKeys(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IResourceAssetKeysEnumerable component in composition.GetComponents<IResourceAssetKeysEnumerable>(true) ?? Enumerable.Empty<IResourceAssetKeysEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetAllResourceKeys(filterKey);
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
                            IEnumerable<ILine> _result = loaded_asset.GetAllResourceKeys(filterKey);
                            if (_result == null) return null;
                            if (_result is Array _array && _array.Length == 0) continue;
                            result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(filterKey);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<ILine> _result = loaded_asset.GetAllResourceKeys(filterKey);
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
