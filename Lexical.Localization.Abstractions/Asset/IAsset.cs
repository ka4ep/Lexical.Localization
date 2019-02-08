// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// This interface is a signal that the object carries an asset that is queryable with other interfaces.
    /// 
    /// There are more specific interfaces on how. 
    ///  See <see cref="IAsset"/>
    ///      <see cref="ILocalizationStringCollection"/>
    ///      <see cref="ILocalizationAssetCultureCapabilities"/>.
    /// </summary>
    public interface IAsset
    {
    }

    public static partial class AssetExtensions
    {
        /// <summary>
        /// Finds first instance of <typeparamref name="T"/>.
        /// Does not look inside <see cref="IAssetProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns>instance of T</returns>
        /// <exception cref="AssetException">If instance was not found</exception>
        public static T GetInstance<T>(this IAsset asset) where T : IAsset
        {
            if (asset is T casted) return casted;

            // Go into composition
            if (asset is IAssetComposition composition)
            {
                foreach (IAsset i in composition)
                {
                    if (i is T casted_) return casted_;

                    // Drill into nested composition
                    if (i is IAssetComposition composition_)
                    {
                        T result = i.TryGetInstance<T>();
                        if (result != null) return result;
                    }
                }
            }
            throw new AssetException($"{typeof(T).CanonicalName()} was not found.");
        }

        /// <summary>
        /// Find first instance of <paramref name="type"/>. 
        /// Does not look inside <see cref="IAssetProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <param name="type">type to search</param>
        /// <returns>instance of T</returns>
        /// <exception cref="AssetException">If instance was not found</exception>
        public static IAsset GetInstance(this IAsset asset, Type type)
        {
            if (type.IsAssignableFrom(asset.GetType())) return asset;

            // Go into composition
            if (asset is IAssetComposition composition)
            {
                foreach (IAsset i in composition)
                {
                    if (type.IsAssignableFrom(i.GetType())) return i;

                    // Drill into nested composition
                    if (i is IAssetComposition composition_)
                    {
                        IAsset result = i.TryGetInstance(type);
                        if (result != null) return result;
                    }
                }
            }
            throw new AssetException($"{type.CanonicalName()} was not found.");
        }

        /// <summary>
        /// Try to find first instance of <typeparamref name="T"/>.
        /// Does not look inside <see cref="IAssetProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns>instance of T or null</returns>
        public static T TryGetInstance<T>(this IAsset asset) where T : IAsset
        {
            if (asset is T casted) return casted;

            // Go into composition
            if (asset is IAssetComposition composition)
            {
                foreach (IAsset i in composition)
                {
                    if (i is T casted_) return casted_;

                    // Drill into nested composition
                    if (i is IAssetComposition composition_)
                    {
                        T result = i.TryGetInstance<T>();
                        if (result != null) return result;
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// Try to find first instance of <paramref name="type"/>.
        /// Does not look inside <see cref="IAssetProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <param name="type">type to search</param>
        /// <returns>instance of T or null</returns>
        public static IAsset TryGetInstance(this IAsset asset, Type type)
        {
            if (type.IsAssignableFrom(asset.GetType())) return asset;

            // Go into composition
            if (asset is IAssetComposition composition)
            {
                foreach (IAsset i in composition)
                {
                    if (type.IsAssignableFrom(i.GetType())) return i;

                    // Drill into nested composition
                    if (i is IAssetComposition composition_)
                    {
                        IAsset result = i.TryGetInstance(type);
                        if (result != null) return result;
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// Get all instance of <typeparamref name="T"/>.
        /// Does not look inside <see cref="IAssetProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns>enumerable of Ts</returns>
        public static IEnumerable<T> GetAllInstances<T>(this IAsset asset) where T : IAsset
        {
            if (asset is T casted) yield return casted;

            // Go into composition
            if (asset is IAssetComposition composition)
                foreach (T i in composition.GetComponents<T>(true))
                    yield return i;
        }

    }
}
