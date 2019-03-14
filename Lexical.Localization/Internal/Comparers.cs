//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Compares <see cref="IAssetNamePatternMatch"/>es.
    /// </summary>
    public class AssetNamePatternMatchEqualityComparer : IEqualityComparer<IAssetNamePatternMatch>
    {
        private readonly static AssetNamePatternMatchEqualityComparer instance = new AssetNamePatternMatchEqualityComparer();
        public static AssetNamePatternMatchEqualityComparer Default => instance;
        static IEqualityComparer<string> stringComparer = EqualityComparer<string>.Default;

        public bool Equals(IAssetNamePatternMatch x, IAssetNamePatternMatch y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Pattern.Pattern != y.Pattern.Pattern) return false;
            int len = x.Pattern.CaptureParts.Length;
            string[] x_values = x.PartValues, y_values = y.PartValues;
            for (int ix = 0; ix < len; ix++)
            {
                if (!stringComparer.Equals(x_values[ix], y_values[ix])) return false;
            }
            return true;
        }

        public int GetHashCode(IAssetNamePatternMatch match)
        {
            if (match == null) return 0;
            int result = FNVHashBasis;
            foreach (string value in match.PartValues)
            {
                result ^= stringComparer.GetHashCode(value);
                result *= FNVHashPrime;
            }
            return result;
        }

        public const int FNVHashBasis = unchecked((int)2166136261);
        public const int FNVHashPrime = 16777619;
    }

    /// <summary>
    /// Order comparer between <see cref="KeyValuePair{Key, Value}".
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class KeyValuePairComparer<Key, Value> : IComparer<KeyValuePair<Key, Value>>
    {
        private static KeyValuePairComparer<Key, Value> instance;
        public static KeyValuePairComparer<Key, Value> Default => instance ?? (instance = new KeyValuePairComparer<Key, Value>(Comparer<Key>.Default, Comparer<Value>.Default));
        public readonly IComparer<Key> keyComparer;
        public readonly IComparer<Value> valueComparer;

        public KeyValuePairComparer(IComparer<Key> keyComparer, IComparer<Value> valueComparer)
        {
            this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
            this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        }
        public int Compare(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
        {
            int compare = 0;
            compare = keyComparer.Compare(x.Key, y.Key);
            if (compare != 0) return compare;
            compare = valueComparer.Compare(x.Value, y.Value);
            if (compare != 0) return compare;
            return 0;
        }
    }

    /// <summary>
    /// Equality comparer between <see cref="KeyValuePair{Key, Value}".
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    public class KeyValuePairEqualityComparer<Key, Value> : IEqualityComparer<KeyValuePair<Key, Value>>
    {
        private static KeyValuePairEqualityComparer<Key, Value> instance;
        public static KeyValuePairEqualityComparer<Key, Value> Default => instance ?? (instance = new KeyValuePairEqualityComparer<Key, Value>(EqualityComparer<Key>.Default, EqualityComparer<Value>.Default));

        public readonly IEqualityComparer<Key> keyComparer;
        public readonly IEqualityComparer<Value> valueComparer;

        public KeyValuePairEqualityComparer(IEqualityComparer<Key> keyComparer, IEqualityComparer<Value> valueComparer)
        {
            this.keyComparer = keyComparer ?? throw new ArgumentNullException(nameof(keyComparer));
            this.valueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        }

        public bool Equals(KeyValuePair<Key, Value> x, KeyValuePair<Key, Value> y)
        {
            if (!keyComparer.Equals(x.Key, y.Key)) return false;
            if (!valueComparer.Equals(x.Value, y.Value)) return false;
            return true;
        }

        public int GetHashCode(KeyValuePair<Key, Value> obj)
            => (obj.Key == null ? 0 : 11 * obj.Key.GetHashCode()) + (obj.Value == null ? 0 : 13 * obj.Value.GetHashCode());
    }
}
