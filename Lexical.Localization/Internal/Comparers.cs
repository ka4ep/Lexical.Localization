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

}
