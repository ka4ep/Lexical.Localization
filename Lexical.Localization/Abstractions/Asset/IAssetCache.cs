// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    #region interfaces
    /// <summary>
    /// Asset cache is decorator that caches requests of source object.
    /// 
    /// The interface is used as signal for extension methods.
    /// </summary>
    public interface IAssetCache : IAssetComposition
    {
        IAsset Source { get; }

        AssetCacheOptions Options { get; }
    }

    public interface IAssetCachePart : IAsset
    {
    }
    #endregion interfaces

    #region options
    public class AssetCacheOptions : Dictionary<string, object>
    {
        /// <summary>
        /// Get a parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) { object value; return TryGetValue(key, out value) ? (T)value : default; }

        /// <summary>
        /// Set a parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AssetCacheOptions Set<T>(string key, T value) { this[key] = value; return this; }

        public override string ToString()
            => $"{GetType().Name}({String.Join(", ", this.Select(kp=>$"{kp.Key}={kp.Value}"))})";
    }
    #endregion options

    #region extensions
    public static partial class AssetCacheExtensions
    {
        /// <summary>
        /// Extension method for chain calls.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="configurer"></param>
        /// <returns></returns>
        public static IAssetCache ConfigureOptions(this IAssetCache cache, Action<AssetCacheOptions> configurer)
        {
            configurer(cache.Options);
            return cache;
        }
    }
    #endregion extensions
}
