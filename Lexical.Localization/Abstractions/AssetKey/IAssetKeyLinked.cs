// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Asset
{
    public interface IAssetKeyLinked : IAssetKey
    {
        /// <summary>
        /// Link to previous key. This is a linked list towards the root.
        /// </summary>
        IAssetKey PreviousKey { get; }
    }

    /// <summary>
    /// Signal for implementing class that this link is not to be canonically compared. 
    /// Instead the comparer should jump to next key in the chain.
    /// 
    /// Some links, such as culture and inlining can be added anywhere and should be compared by their position.
    /// </summary>
    public interface IAssetKeyNonCanonicallyCompared
    {
    }

    public delegate void AssetKeyVisitor<T>(IAssetKey key, ref T data);

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Enumerate linked list towards root.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<IAssetKey> EnumerateToRoot(this IAssetKey key)
        {
            while (key != null)
            {
                yield return key;
                key = key.GetPreviousKey();
            }
        }

        /// <summary>
        /// Visit key chain from root towards key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="visitor"></param>
        public static void VisitFromRoot<T>(this IAssetKey key, AssetKeyVisitor<T> visitor, ref T data)
        {
            // Push to stack
            IAssetKey prevKey = key.GetPreviousKey();
            if (prevKey != null) VisitFromRoot(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            visitor(key, ref data);
        }

        /// <summary>
        /// Return an array of links from root.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includeNonCanonical">include all keys that implement ILocalizationKeyNonCanonicallyCompared</param>
        /// <returns>array of keys</returns>
        public static IAssetKey[] ArrayFromRoot(this IAssetKey key, bool includeNonCanonical=true)
        {
            // Count the number of keys
            int count = 0;
            if (includeNonCanonical)
                for (IAssetKey k = key; k != null; k = k.GetPreviousKey()) count++;
            else
                for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                    if (k is IAssetKeyNonCanonicallyCompared == false) count++;

            // Create result
            IAssetKey[] result = new IAssetKey[count];
            int ix = count - 1;
            if (includeNonCanonical)
                for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                    result[ix--] = k;
            else
                for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                    if (k is IAssetKeyNonCanonicallyCompared == false)
                        result[ix--] = k;

            return result;
        }

        /// <summary>
        /// Get previous key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Previous key or null</returns>
        public static IAssetKey GetPreviousKey(this IAssetKey key)
            => key is IAssetKeyLinked linkedKey ? linkedKey.PreviousKey : null;

        /// <summary>
        /// Get the first key in the linked list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>key or first key</returns>
        public static IAssetKey GetFirstKey(this IAssetKey key)
        {
            while (true)
            {
                IAssetKey prevKey = key.GetPreviousKey();
                if (prevKey == null) return key;
                key = prevKey;
            }
        }

        /// <summary>
        /// Finds key that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T or null</returns>
        public static T Find<T>(this IAssetKey key) where T : IAssetKey
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is T casted) return casted;
            return default;
        }

        /// <summary>
        /// Finds key that implements T when walking towards root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        /// <exception cref="AssetException">if T is not found</exception>
        public static T Get<T>(this IAssetKey key) where T : IAssetKey
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is T casted) return casted;
            throw new AssetException($"{typeof(T).CanonicalName()} is not found.");
        }

        /// <summary>
        /// Finds key that implements T when walking towards root, start from previous key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T or null</returns>
        public static T FindPrev<T>(this IAssetKey key) where T : IAssetKey
        {
            for (IAssetKey k = key.GetPreviousKey(); k != null; k = k.GetPreviousKey())
                if (k is T casted) return casted;
            return default;
        }

    }
}
