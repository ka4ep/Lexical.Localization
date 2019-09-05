// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Interface for reading binary resources.
    /// 
    /// Consumers of this interface should always call with <see cref="IAssetExtensions.GetBytes(IAsset, ILine)"/>.
    /// 
    /// See subinterfaces:
    /// <list type="bullet">
    ///     <item><see cref="IBinaryAssetKeysEnumerable"/></item>
    ///     <item><see cref="IBinaryAssetNamesEnumerable"/></item>
    /// </list>
    /// </summary>
    public interface IBinaryAsset : IAsset
    {
        /// <summary>
        /// Try to read a binary resource by matching <paramref name="key"/> to the asset's resources
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveBytes(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result info</returns>
        LineBinaryBytes GetBytes(ILine key);

        /// <summary>
        /// Try to open a stream to a binary resource by matching <paramref name="key"/> to the asset's resources.
        /// If Stream (<see cref="LineBinaryStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveStream(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result info</returns>
        LineBinaryStream GetStream(ILine key);
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
    public interface IBinaryAssetKeysEnumerable : IAsset
    {
        /// <summary>
        /// Get the binary resource keys that could be resolved. 
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
        IEnumerable<ILine> GetBinaryKeys(ILine filterKey = null);

        /// <summary>
        /// Get all binary resource keys, or if every key cannot be provided returns null.
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
        IEnumerable<ILine> GetAllBinaryKeys(ILine filterKey = null);
    }

    /// <summary>
    /// Interface to enumerate binary localization resources as strings identifier, which is typically a filename.
    /// 
    /// Note, that the intrinsic key class is <see cref="ILine"/> and not string, therefore
    /// is is encouraged to use and implement <see cref="IBinaryAssetKeysEnumerable"/> instead.
    /// </summary>
    public interface IBinaryAssetNamesEnumerable : IAsset
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
        IEnumerable<string> GetBinaryNames(ILine filterKey = null);

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
        IEnumerable<string> GetAllBinaryNames(ILine filterKey = null);
    }
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;
    using Lexical.Localization.Binary;

    public static partial class IAssetExtensions
    {
        /// <summary>
        /// Try to read a binary resource by matching <paramref name="key"/> to the asset's resources
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveBytes(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>result info</returns>
        public static LineBinaryBytes GetBytes(this IAsset asset, ILine key)
        {
            if (asset is IBinaryAsset casted)
            {
                LineBinaryBytes data = casted.GetBytes(key);
                if (data.Value != null) return data;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAsset component in composition.GetComponents<IBinaryAsset>(true) ?? Enumerable.Empty<IBinaryAsset>())
                {
                    LineBinaryBytes data = component.GetBytes(key);
                    if (data.Value != null) return data;
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            LineBinaryBytes data = loaded_asset.GetBytes(key);
                            if (data.Value != null) return data;
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
                        LineBinaryBytes data = loaded_asset.GetBytes(key);
                        if (data.Value != null) return data;
                    }
                }
            }

            // No result
            return new LineBinaryBytes(key, (Exception)null, LineStatus.ResolveFailedNoResult);
        }

        /// <summary>
        /// Try to open a stream to a resource by matching <paramref name="key"/> to the asset's resources.
        /// If Stream (<see cref="LineBinaryStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveStream(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>result info</returns>
        public static LineBinaryStream GetStream(this IAsset asset, ILine key)
        {
            if (asset is IBinaryAsset casted)
            {
                LineBinaryStream data = casted.GetStream(key);
                if (data.Value != null) return data;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAsset component in composition.GetComponents<IBinaryAsset>(true) ?? Enumerable.Empty<IBinaryAsset>())
                {
                    LineBinaryStream data = component.GetStream(key);
                    if (data.Value != null) return data;
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            LineBinaryStream data = loaded_asset.GetStream(key);
                            if (data.Value != null) return data;
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
                        LineBinaryStream data = loaded_asset.GetStream(key);
                        if (data.Value != null) return data;
                    }
                }
            }

            // No result
            return new LineBinaryStream(key, (Exception)null, LineStatus.ResolveFailedNoResult);
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
        public static IEnumerable<string> GetBinaryNames(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<string> result = null;
            if (asset is IBinaryAssetNamesEnumerable casted) result = casted.GetBinaryNames(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAssetNamesEnumerable component in composition.GetComponents<IBinaryAssetNamesEnumerable>(true) ?? Enumerable.Empty<IBinaryAssetNamesEnumerable>())
                {
                    IEnumerable<string> _result = component.GetBinaryNames(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<string> _result = loaded_asset.GetBinaryNames(filterKey);
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
                        IEnumerable<string> _result = loaded_asset.GetBinaryNames(filterKey);
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
        public static IEnumerable<string> GetAllBinaryNames(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<string> result = null;
            if (asset is IBinaryAssetNamesEnumerable casted) result = casted.GetAllBinaryNames(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAssetNamesEnumerable component in composition.GetComponents<IBinaryAssetNamesEnumerable>(true) ?? Enumerable.Empty<IBinaryAssetNamesEnumerable>())
                {
                    IEnumerable<string> _result = component.GetAllBinaryNames(filterKey);
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
                            IEnumerable<string> _result = loaded_asset.GetAllBinaryNames(filterKey);
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
                        IEnumerable<string> _result = loaded_asset.GetAllBinaryNames(filterKey);
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
        public static IEnumerable<ILine> GetBinaryKeys(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IBinaryAssetKeysEnumerable casted) result = casted.GetBinaryKeys(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAssetKeysEnumerable component in composition.GetComponents<IBinaryAssetKeysEnumerable>(true) ?? Enumerable.Empty<IBinaryAssetKeysEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetBinaryKeys(filterKey);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider component in composition.GetComponents<IAssetProvider>(true) ?? Enumerable.Empty<IAssetProvider>())
                {
                    IEnumerable<IAsset> assets = component.LoadAssets(filterKey);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<ILine> _result = loaded_asset.GetBinaryKeys(filterKey);
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
                        IEnumerable<ILine> _result = loaded_asset.GetBinaryKeys(filterKey);
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
        public static IEnumerable<ILine> GetAllBinaryKeys(this IAsset asset, ILine filterKey = null)
        {
            IEnumerable<ILine> result = null;
            if (asset is IBinaryAssetKeysEnumerable casted) result = casted.GetAllBinaryKeys(filterKey);
            if (asset is IAssetComposition composition)
            {
                foreach (IBinaryAssetKeysEnumerable component in composition.GetComponents<IBinaryAssetKeysEnumerable>(true) ?? Enumerable.Empty<IBinaryAssetKeysEnumerable>())
                {
                    IEnumerable<ILine> _result = component.GetAllBinaryKeys(filterKey);
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
                            IEnumerable<ILine> _result = loaded_asset.GetAllBinaryKeys(filterKey);
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
                        IEnumerable<ILine> _result = loaded_asset.GetAllBinaryKeys(filterKey);
                        if (_result == null) return null;
                        if (_result is Array _array && _array.Length == 0) continue;
                        result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }

    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Try to read a binary resource by matching <paramref name="line"/> to the asset's resources
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveBytes(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result info</returns>
        public static LineBinaryBytes GetBytes(this ILine line)
        {
            IAsset asset;
            if (line.TryGetAsset(out asset)) return asset.GetBytes(line);
            // No result
            return new LineBinaryBytes(line, (Exception)null, LineStatus.ResolveFailedNoResult);
        }

        /// <summary>
        /// Try to open a stream to a resource by matching <paramref name="line"/> to the asset's resources.
        /// If Stream (<see cref="LineBinaryStream.Value"/>) is provided, then the caller is responsible for disposing it.
        /// 
        /// Does not apply contextual information from the executing context. (See <see cref="ILineExtensions.ResolveStream(ILine)"/> to match in context.)
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        /// </list>
        /// </summary>
        /// <param name="line"></param>
        /// <returns>result info</returns>
        public static LineBinaryStream GetStream(this ILine line)
        {
            IAsset asset;
            if (line.TryGetAsset(out asset)) return asset.GetStream(line);
            // No result
            return new LineBinaryStream(line, (Exception)null, LineStatus.ResolveFailedNoResult);
        }

    }

}
