// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen
// -----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Internal
{

    /// <summary>
    /// Comparer of parameter dictionaries.
    /// 
    /// As rule for parameters, if value is null, then it counts as non-existing.
    /// </summary>
    public class ParameterComparer : IEqualityComparer<IReadOnlyDictionary<string, string>>
    {
        private static ParameterComparer instance = new ParameterComparer();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ParameterComparer Instance => instance;

        /// <summary>
        /// Compare dictionaries (parameters) for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IReadOnlyDictionary<string, string> x, IReadOnlyDictionary<string, string> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // Compare match arrays
            if (x is ILinePatternMatch x_match && y is ILinePatternMatch y_match && x_match.Pattern == y_match.Pattern)
            {
                int c = x_match.Pattern.CaptureParts.Length;
                for (int i = 0; i < c; i++)
                    if (x_match[i] != y_match[i]) return false;
                return true;
            }

            // Compare dictionaries
            int x_value_count = 0;
            foreach (var x_kp in x)
            {
                // If value is null, then it's equivalent to not existing
                if (x_kp.Value == null) continue;

                // Calculate count
                x_value_count++;

                // Find matching y value
                string y_value;
                if (!y.TryGetValue(x_kp.Key, out y_value)) return false;

                // Match values
                if (x_kp.Value != y_value) return false;
            }

            // Now calculate y value count
            int y_value_count = 0;
            foreach (var y_kp in y)
            {
                // If value is null, then it's equivalent to not existing
                if (y_kp.Value == null) continue;

                y_value_count++;
                if (y_value_count > x_value_count) return false;
            }

            return x_value_count == y_value_count;
        }

        /// <summary>
        /// Calculates hashcode for parameters. If value is null, then key-value-pair is ignored.
        /// Hashing uses xor because order is not relevant.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(IReadOnlyDictionary<string, string> obj)
        {
            int hash = 0;
            foreach (var kp in obj)
            {
                if (kp.Value == null) continue;
                hash ^= kp.Key.GetHashCode();
                hash ^= kp.Value.GetHashCode();
            }
            return hash;
        }
    }
}
