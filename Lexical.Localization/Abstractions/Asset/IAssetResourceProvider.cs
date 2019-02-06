// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface for reading binary resources.
    /// 
    /// Consumers of this interface should always call with <see cref="AssetExtensions.GetResource(IAsset, IAssetKey)"/>.
    /// </summary>
    public interface IAssetResourceProvider : IAsset
    {
        /// <summary>
        /// Try to read a binary resource.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null or resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        byte[] GetResource(IAssetKey key);

        /// <summary>
        /// Try to open a stream to a resource.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null if resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        Stream OpenStream(IAssetKey key);
    }

    /// <summary>
    /// Interface to enumerate binary localization resources.
    /// </summary>
    public interface IAssetResourceCollection : IAsset
    {
        /// <summary>
        /// Get all resource names. Filter by parameters in key.
        /// </summary>
        /// <param name="key">(optional) key as filter</param>
        /// <returns>resource names, or null</returns>
        IEnumerable<string> GetResourceNames(IAssetKey key = null);
    }

    public static partial class AssetExtensions
    {
        /// <summary>
        /// Try to read a resource.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key"></param>
        /// <returns>resolved string or null or resource was not found</returns>
        /// <exception cref="AssetException">If resource was found, but there was a problem opening the stream</exception>
        public static byte[] GetResource(this IAsset asset, IAssetKey key)
        {
            if (asset is IAssetResourceProvider casted)
            {
                byte[] data = casted.GetResource(key);
                if (data != null) return data;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetResourceProvider _ in composition.GetComponents<IAssetResourceProvider>(true))
                {
                    byte[] data = _.GetResource(key);
                    if (data != null) return data;
                }
                foreach(IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(key);
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
        public static Stream OpenStream(this IAsset asset, IAssetKey key)
        {
            if (asset is IAssetResourceProvider casted)
            {
                Stream steam = casted.OpenStream(key);
                if (steam != null) return steam;
            }
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetResourceProvider _ in composition.GetComponents<IAssetResourceProvider>(true))
                {
                    Stream steam = _.OpenStream(key);
                    if (steam != null) return steam;
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(key);
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
        /// Get all resource names.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="key">(optional) key to get strings for</param>
        /// <returns>resource names by culture, or null</returns>
        public static IEnumerable<string> GetResourceNames(this IAsset asset, IAssetKey key = null)
        {
            IEnumerable<string> result = null;
            if (asset is IAssetResourceCollection casted) result = casted.GetResourceNames(key);
            if (asset is IAssetComposition composition)
            {
                foreach (IAssetResourceCollection strs in composition.GetComponents<IAssetResourceCollection>(true))
                {
                    IEnumerable<string> _result = strs.GetResourceNames(key);
                    if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                }
                foreach (IAssetProvider _ in composition.GetComponents<IAssetProvider>(true))
                {
                    IEnumerable<IAsset> assets = _.LoadAssets(key);
                    if (assets != null)
                    {
                        foreach (IAsset loaded_asset in assets)
                        {
                            IEnumerable<string> _result = loaded_asset.GetResourceNames(key);
                            if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                        }
                    }
                }
            }
            if (asset is IAssetProvider provider)
            {
                IEnumerable<IAsset> assets = provider.LoadAllAssets(key);
                if (assets != null)
                {
                    foreach (IAsset loaded_asset in assets)
                    {
                        IEnumerable<string> _result = loaded_asset.GetResourceNames(key);
                        if (_result != null && (_result is Array _array ? _array.Length > 0 : true)) result = result == null ? _result : result.Concat(_result);
                    }
                }
            }
            return result;
        }
    }

}
