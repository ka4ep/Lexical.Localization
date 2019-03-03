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
    /// Configurable <see cref="IAssetKey"/> comparer.
    /// 
    /// Canonical and non-canonical comparers can be added as component comparers of <see cref="IEqualityComparer{IAssetKey}"/>.
    /// 
    /// Canonical comparers compare each key link separately.
    /// 
    /// Non-canonical comparers compare key chain as a whole.
    /// </summary>
    public class AssetKeyComparer : IEqualityComparer<IAssetKey>
    {
        private static AssetKeyComparer instance = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();

        /// <summary>
        /// Makes comparisons on interface level. 
        /// Compares parametrized properties of keys.
        /// 
        /// This comparer uses the following pattern for comparisons:
        ///    Key                      canonical compare
        ///    Section                  canonical compare
        ///    TypeSection              canonical compare
        ///    Resource                 canonical compare
        ///    LocationSection          canonical compare
        ///    AssemblySection          non-canonical compare
        ///    Culture                  non-canonical compare
        ///    Format Args              non-canonical compare
        ///    Inlining                 non-canonical compare
        ///    CulturePolicy            not compared
        ///    Root                     not compared
        /// </summary>
        public static AssetKeyComparer Default => instance;

        /// List of non-canonical comparers
        List<IEqualityComparer<IAssetKey>> canonicalComparers = new List<IEqualityComparer<IAssetKey>>();

        // List of canonical comparers.
        List<IEqualityComparer<IAssetKey>> noncanonicalComparers = new List<IEqualityComparer<IAssetKey>>();

        public AssetKeyComparer()
        {
        }

        /// <summary>
        /// Add canonical comparer. Canonical comparer is applied to each non-canonical link of a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public AssetKeyComparer AddCanonicalComparer(IEqualityComparer<IAssetKey> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            canonicalComparers.Add(comparer);
            return this;
        }

        /// <summary>
        /// Add non-canonical comparer. Non-canonical comparer is applied to the leaf of the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public AssetKeyComparer AddNonCanonicalComparer(IEqualityComparer<IAssetKey> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            noncanonicalComparers.Add(comparer);
            return this;
        }

        public AssetKeyComparer AddCanonicalParametrizedComparer()
        {
            canonicalComparers.Add(ParametrizedComparer.Instance);
            return this;
        }

        public AssetKeyComparer AddNonCanonicalParametrizedComparer()
        {
            noncanonicalComparers.Add(ParametrizedNonCanonicalComparer.Instance);
            return this;
        }

        public virtual bool Equals(IAssetKey x, IAssetKey y)
        {
            bool xIsNull = x == null, yIsNull = y == null;
            if (xIsNull && yIsNull) return true;
            if (xIsNull || yIsNull) return false;

            // Non-canonical comparisons first
            foreach(var comparer in noncanonicalComparers)
            {
                if (!comparer.Equals(x, y)) return false;
            }

            // Canonical Comparisons
            IAssetKey xLink = x, yLink = y;
            while (xLink != null && xLink is IAssetKeyCanonicallyCompared == false) xLink = xLink.GetPreviousKey();
            while (yLink != null && yLink is IAssetKeyCanonicallyCompared == false) yLink = yLink.GetPreviousKey();
            if (!CanonicalEquals(xLink, yLink)) return false;

            return true;
        }

        protected virtual bool CanonicalEquals(IAssetKey x, IAssetKey y)
        {
            bool xIsNull = x == null, yIsNull = y == null;
            if (xIsNull && yIsNull) return true;
            if (xIsNull || yIsNull) return false;

            // Non-canonical comparisons first
            foreach (var comparer in canonicalComparers)
            {
                if (!comparer.Equals(x, y)) return false;
            }

            // Compare next canonical keys
            IAssetKey xLink = x.GetPreviousKey(), yLink = y.GetPreviousKey();
            while (xLink != null && xLink is IAssetKeyCanonicallyCompared == false) xLink = xLink.GetPreviousKey();
            while (yLink != null && yLink is IAssetKeyCanonicallyCompared == false) yLink = yLink.GetPreviousKey();
            if (xLink != null || yLink != null)
                if (!CanonicalEquals(xLink, yLink)) return false;

            return true;
        }

        public const int FNVHashBasis = unchecked((int)0x811C9DC5);
        public const int FNVHashPrime = 0x1000193;

        public virtual int GetHashCode(IAssetKey key)
        {
            if (key == null) return 0;
            int result = FNVHashBasis;

            // Non-canonical hashing
            foreach (var comparer in noncanonicalComparers)
            {
                result ^= comparer.GetHashCode(key);
            }

            // Canonical hashing
            for(IAssetKey k = key; k!=null; k=k.GetPreviousKey())
                if (k is IAssetKeyCanonicallyCompared)
                    foreach (var comparer in canonicalComparers)
                    {
                        result ^= comparer.GetHashCode(k);
                        result *= FNVHashPrime;
                    }

            return result;
        }

    }

    /// <summary>
    /// Compares key that implements <see cref="IAssetKeyParameterAssigned"/> against another.
    /// </summary>
    public class ParametrizedComparer : IEqualityComparer<IAssetKey>
    {
        private static ParametrizedComparer instance = new ParametrizedComparer();
        public static ParametrizedComparer Instance => instance;

        public bool Equals(IAssetKey x, IAssetKey y)
        {
            IAssetKey xLink = x, yLink = y;

            string x_parameter = x.GetParameterName();
            string y_parameter = y.GetParameterName();
            if (x_parameter == null && y_parameter == null) return true;
            if (x_parameter == null || y_parameter == null) return false;
            if (x_parameter != y_parameter) return false;
            return x.Name == y.Name;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;
        public int GetHashCode(IAssetKey key)
        {
            string parameterName = key.GetParameterName();
            if (parameterName == null) return 0;
            int hash = FNVHashBasis;
            hash ^= parameterName.GetHashCode();
            hash ^= key.Name.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Compares all non-canonical parameters keys agains each other.
    /// These are keys that implement <see cref="IAssetKeyParameterAssigned"/> and <see cref="IAssetKeyNonCanonicallyCompared"/>.
    /// Root key is not compared.
    /// </summary>
    public class ParametrizedNonCanonicalComparer : IEqualityComparer<IAssetKey>
    {
        private static ParametrizedNonCanonicalComparer instance = new ParametrizedNonCanonicalComparer();
        public static ParametrizedNonCanonicalComparer Instance => instance;

        public bool Equals(IAssetKey x, IAssetKey y)
        {
            LazyList<(string, string)> x_parameters = new LazyList<(string, string)>();

            // Get x's (parameter, value) pairs
            for (IAssetKey x_link = x; x_link != null; x_link = x_link.GetPreviousKey())
            {
                // Is non-canonical
                if (x_link is IAssetKeyNonCanonicallyCompared == false) continue;

                // Get parameter
                string x_parameter_name = x_link.GetParameterName(), x_parameter_value = x_link.Name;
                if (x_parameter_name == null || x_parameter_value == null) continue;

                // Has this parameter been added already. 
                int ix = -1;
                for (int i = 0; i < x_parameters.Count; i++) if (x_parameters[i].Item1 == x_parameter_name) { ix = i; break; }
                // Last value stands.
                if (ix >= 0) break;

                // Add to list
                x_parameters.Add((x_parameter_name, x_parameter_value));
            }

            // Match against y's
            int count = 0;
            for (IAssetKey y_link = y; y_link != null; y_link = y_link.GetPreviousKey())
            {
                // Is non-canonical
                if (y_link is IAssetKeyNonCanonicallyCompared == false) continue;

                // Get parameter name
                string y_parameter_name = y_link.GetParameterName(), y_parameter_value = y_link.Name;
                if (y_parameter_name == null || y_parameter_value == null) continue;

                // Test if x had one corresponding one
                string x_value = null;
                int ix = -1;
                for (int i = 0; i < x_parameters.Count; i++)
                {
                    if (x_parameters[i].Item1 == y_parameter_name)
                    {
                        x_value = x_parameters[i].Item2;
                        ix = i;
                        break;
                    }
                }

                // y has a y_parameter_name that doesn't have a corresponding equivalent in x
                if (ix < 0) return false;

                // y_parameter_name has already been matched. It was ok. 
                // This is second time for this parameter, the last in chain value only matters.
                // Thereok, we can ignore this
                if (x_value == null) continue;

                // Value differs
                if (x_value != y_parameter_value) return false;

                // Value is same. We null x_parameter[ix].value as a signal that the paramter name has already been checked.
                x_parameters[ix] = (y_parameter_name, null);
                count++;
            }

            // X had some values Y didn't
            if (count != x_parameters.Count) return false;

            return true;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;
        public int GetHashCode(IAssetKey key)
        {
            int hash = 0;
            // Get x's (parameter, value) pairs
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                if (k is IAssetKeyNonCanonicallyCompared == false) continue;

                // Get parameters.
                string parameterName = k.GetParameterName();
                if (parameterName == null) continue;

                // Get value.
                string parameter_value = k.Name;
                if (parameter_value == null) continue;

                hash ^= parameterName.GetHashCode();
                hash ^= parameter_value.GetHashCode();
            }
            return hash;
        }
    }

}
