﻿// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen
// -----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Match result.
    /// </summary>
    public class ParameterPatternMatch : IParameterPatternMatch
    {
        /// <summary>
        /// Reference to name pattern.
        /// </summary>
        public IParameterPattern Pattern { get; internal set; }

        /// <summary>
        /// Matches part values. One for each corresponding pattern.CaptureParts.
        /// </summary>
        public string[] PartValues { get; }

        /// <summary>
        /// Part values by <see cref="IParameterPattern.CaptureParts" />
        /// </summary>
        /// <param name="ix"></param>
        /// <returns></returns>
        public string this[int ix] => PartValues[ix];

        /// <summary>
        /// Get part value by part identifier.
        /// </summary>
        /// <param name="identifier">identifier, e.g. "Culture", "Type"</param>
        /// <returns>value or null</returns>
        public string this[string identifier] { get { IParameterPatternPart part; if (Pattern.PartMap.TryGetValue(identifier, out part)) return PartValues[part.CaptureIndex]; return null; } }

        /// <summary>
        /// Cached result.
        /// </summary>
        bool? success;

        /// <summary>
        /// Where all parts found.
        /// </summary>
        public bool Success => (bool)(success ?? (success = TestSuccess()));

        /// <summary>
        /// Cached string
        /// </summary>
        string str = null;

        /// <summary>
        /// Get match as string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => str ?? (str = Pattern.Print(PartValues));

        /// <summary>
        /// Construct new match.
        /// </summary>
        /// <param name="pattern"></param>
        public ParameterPatternMatch(IParameterPattern pattern)
        {
            this.Pattern = pattern;
            this.PartValues = new string[pattern.CaptureParts.Length];
        }

        /// <summary>
        /// Test if all parts were found.
        /// </summary>
        /// <returns></returns>
        bool TestSuccess()
        {
            if (PartValues == null) return false;
            for (int i = 0; i < Pattern.CaptureParts.Length; i++)
            {
                if (!Pattern.CaptureParts[i].Required) continue;
                if (PartValues[i] == null) return false;
            }
            return true;
        }

        /// <summary>
        /// All capture part identifiers
        /// </summary>
        public IEnumerable<string> Keys => Pattern.CaptureParts.Select(p => p.Identifier);

        /// <summary>
        /// All capture part values
        /// </summary>
        public IEnumerable<string> Values => PartValues;

        /// <summary>
        /// Number of capture parts
        /// </summary>
        public int Count => PartValues.Length;

        /// <summary>
        /// Test if contains capture part by identifier
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key) => Pattern.PartMap.ContainsKey(key);

        /// <summary>
        /// Try get matched value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out string value)
        {
            IParameterPatternPart p;
            if (Pattern.PartMap.TryGetValue(key, out p)) { value = PartValues[p.CaptureIndex]; return true; }
            value = default;
            return false;
        }

        /// <summary>
        /// GEt enumerator of capture keys and values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => new MatchEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new MatchEnumerator(this);

        class MatchEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            int ix = -1;
            ParameterPatternMatch match;
            public MatchEnumerator(ParameterPatternMatch match) { this.match = match; }
            public KeyValuePair<string, string> Current
            {
                get
                {
                    if (ix < 0 || ix >= match.Pattern.CaptureParts.Length) return new KeyValuePair<string, string>(null, null);
                    IParameterPatternPart part = match.Pattern.CaptureParts[ix];
                    return new KeyValuePair<string, string>(part.Identifier, match.PartValues[part.CaptureIndex]);
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    if (ix < 0 || ix >= match.Pattern.CaptureParts.Length) return null;
                    IParameterPatternPart part = match.Pattern.CaptureParts[ix];
                    return new KeyValuePair<string, string>(part.Identifier, match.PartValues[part.CaptureIndex]);
                }
            }
            public void Dispose() => match = null;
            public bool MoveNext() => (++ix) < match.Pattern.CaptureParts.Length;
            public void Reset() => ix = -1;
        }
    }

    /// <summary>
    /// Compares specific parameters, and those only.
    /// </summary>
    public class ParameterPatternMatchComparer : IEqualityComparer<IReadOnlyDictionary<string, string>>
    {
        /// <summary>
        /// The pattern this is comparer to
        /// </summary>
        public readonly IParameterPattern Pattern;

        /// <summary>
        /// Create comparer to <paramref name="pattern"/>.
        /// </summary>
        /// <param name="pattern"></param>
        public ParameterPatternMatchComparer(IParameterPattern pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        /// <summary>
        /// Compare matches for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IReadOnlyDictionary<string, string> x, IReadOnlyDictionary<string, string> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // Cast if x and y are pattern matches
            IParameterPatternMatch x_match = x as IParameterPatternMatch;
            IParameterPatternMatch y_match = y as IParameterPatternMatch;
            if (x_match?.Pattern != Pattern) x_match = null;
            if (y_match?.Pattern != Pattern) y_match = null;

            // Match each capture part
            for (int i = 0; i < Pattern.CaptureParts.Length; i++)
            {
                string x_value = null, y_value = null;

                if (x_match != null) x_value = x_match.PartValues[i]; else x.TryGetValue(Pattern.CaptureParts[i].Identifier, out x_value);
                if (y_match != null) y_value = y_match.PartValues[i]; else y.TryGetValue(Pattern.CaptureParts[i].Identifier, out y_value);

                if (x_value != y_value) return false;
            }

            return true;
        }

        /// <summary>
        /// Hash prime factor.
        /// </summary>
        public const int FNVHashBasis = unchecked((int)2166136261);

        /// <summary>
        /// Hash initial value.
        /// </summary>
        public const int FNVHashPrime = 16777619;

        /// <summary>
        /// Calculate hashcode for match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(IReadOnlyDictionary<string, string> obj)
        {
            // Cast if is match
            IParameterPatternMatch match = obj as IParameterPatternMatch;
            if (match?.Pattern != Pattern) match = null;

            // hash
            int hash = FNVHashBasis;

            // Match each capture part
            for (int i = 0; i < Pattern.CaptureParts.Length; i++)
            {
                string value = null;
                if (match != null) value = match.PartValues[i]; else obj.TryGetValue(Pattern.CaptureParts[i].Identifier, out value);
                hash *= FNVHashPrime;
                if (value != null) hash ^= value.GetHashCode();
            }

            return hash;
        }
    }

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
            if (x is IParameterPatternMatch x_match && y is IParameterPatternMatch y_match && x_match.Pattern == y_match.Pattern)
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