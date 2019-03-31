// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Interface for key classes that can form a linked list.
    /// </summary>
    public interface IAssetKeyLinked : IAssetKey
    {
        /// <summary>
        /// Link to previous key. This is a linked list towards the root.
        /// </summary>
        IAssetKey PreviousKey { get; }
    }

    /// <summary>
    /// A visitor delegate that is used when key is visited.
    /// Visitation starts from tail and proceeds towards root.
    /// Visitation is stack allocated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="data"></param>
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
        /// Get key that implements <see cref="IAssetKeyCanonicallyCompared"/>, either this or preceding, or null if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><paramref name="key"/>, or preceding canonical key or null</returns>
        public static IAssetKeyCanonicallyCompared GetCanonicalKey(this IAssetKey key)
        {
            for (IAssetKey k = key; k != null; k = k is IAssetKeyLinked linkedKey ? linkedKey.PreviousKey : null)
            {
                if (k is IAssetKeyCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding key that implements <see cref="IAssetKeyCanonicallyCompared"/>, or null if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>preceding canonical key or null</returns>
        public static IAssetKeyCanonicallyCompared GetPreviousCanonicalKey(this IAssetKey key)
        {
            for (IAssetKey k = key is IAssetKeyLinked lkk ? lkk.PreviousKey : null; k != null; k = k is IAssetKeyLinked nlkk ? nlkk.PreviousKey : null)
            {
                if (k is IAssetKeyCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get key that implements <see cref="IAssetNonKeyCanonicallyCompared"/>, either this or preceding one, or null if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><paramref name="key"/>, or preceding non-canonical key or null</returns>
        public static IAssetKeyNonCanonicallyCompared GetNonCanonicalKey(this IAssetKey key)
        {
            for (IAssetKey k = key; k != null; k = k is IAssetKeyLinked linkedKey ? linkedKey.PreviousKey : null)
            {
                if (k is IAssetKeyNonCanonicallyCompared kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding key that implements <see cref="IAssetKeyNonCanonicallyCompared"/>, or null if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>preceding non-canonical key or null</returns>
        public static IAssetKeyNonCanonicallyCompared GetPreviousNonCanonicalKey(this IAssetKey key)
        {
            for (IAssetKey k = key is IAssetKeyLinked lkk ? lkk.PreviousKey : null; k != null; k = k is IAssetKeyLinked nlkk ? nlkk.PreviousKey : null)
            {
                if (k is IAssetKeyNonCanonicallyCompared kk) return kk;
            }
            return null;
        }

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
        /// <exception cref="AssetKeyException">if T is not found</exception>
        public static T Get<T>(this IAssetKey key) where T : IAssetKey
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is T casted) return casted;
            throw new AssetKeyException(key, $"{typeof(T).FullName} is not found.");
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

        /// <summary>
        /// Scan key towards root, returns <paramref name="index"/>th key from tail.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index">the index of key to return starting from tail.</param>
        /// <returns>key</returns>
        /// <exception cref="IndexOutOfRangeException">if <paramref name="index"/> goes over root</exception>
        public static IAssetKey GetAt(this IAssetKey key, int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            for (int i = 0; i < index; i++)
                key = key.GetPreviousKey();
            if (key == null) throw new IndexOutOfRangeException();
            return key;
        }

    }
}
