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
    /// Compares <see cref="ILinePatternMatch"/>es.
    /// </summary>
    public class LinePatternMatchEqualityComparer : IEqualityComparer<ILinePatternMatch>
    {
        private readonly static LinePatternMatchEqualityComparer instance = new LinePatternMatchEqualityComparer();

        /// <summary>
        /// Default comparer
        /// </summary>
        public static LinePatternMatchEqualityComparer Default => instance;

        static IEqualityComparer<string> stringComparer = EqualityComparer<string>.Default;

        /// <summary>
        /// Compare pattern matches.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILinePatternMatch x, ILinePatternMatch y)
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

        /// <summary>
        /// Calculate hash-code.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int GetHashCode(ILinePatternMatch match)
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

        const int FNVHashBasis = unchecked((int)2166136261);
        const int FNVHashPrime = 16777619;
    }

}
