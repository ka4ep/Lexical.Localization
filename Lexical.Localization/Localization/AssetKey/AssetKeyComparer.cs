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
        private static AssetKeyComparer instance =
            new AssetKeyComparer()
            .AddCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton)
            .AddNonCanonicalParametrizerComparer(AssetKeyParametrizer.Singleton);

        /// <summary>
        /// Makes comparisons on interface level. 
        /// Compares parametrized properties of keys.
        /// 
        /// This comparer uses the following pattern for comparisons:
        ///    Key                      canonical compare
        ///    Section                  canonical compare
        ///    TypeSection              canonical compare
        ///    AssemblySection          canonical compare
        ///    Resource                 canonical compare
        ///    Root                     not compare
        ///    Culture                  non-canonical compre
        ///    Format Args              not compared
        ///    CulturePolicy            not compared
        ///    Inlining                 not compared
        ///    LocalizationAsset        not compared
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

        public AssetKeyComparer AddCanonicalParametrizerComparer(IAssetKeyParametrizer parametrizer = default)
        {
            ParametrizerCanonicalComparer<IAssetKey> comparer = new ParametrizerCanonicalComparer<IAssetKey>(parametrizer ?? AssetKeyParametrizer.Singleton);
            canonicalComparers.Add(comparer);
            return this;
        }

        public AssetKeyComparer AddNonCanonicalParametrizerComparer(IAssetKeyParametrizer parametrizer = default)
        {
            ParametrizerNonCanonicalComparer comparer = new ParametrizerNonCanonicalComparer(parametrizer ?? AssetKeyParametrizer.Singleton);
            noncanonicalComparers.Add(comparer);
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
            while (xLink is IAssetKeyNonCanonicallyCompared) xLink = xLink.GetPreviousKey();
            while (yLink is IAssetKeyNonCanonicallyCompared) yLink = yLink.GetPreviousKey();
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
            while (xLink is IAssetKeyNonCanonicallyCompared) xLink = xLink.GetPreviousKey();
            while (yLink is IAssetKeyNonCanonicallyCompared) yLink = yLink.GetPreviousKey();
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

            /// Non-canonical hashing
            foreach (var comparer in noncanonicalComparers)
            {
                result ^= comparer.GetHashCode(key);
            }

            /// Canonical hashing
            for(IAssetKey k = key; k!=null; k=k.GetPreviousKey())
                if (k is IAssetKeyNonCanonicallyCompared == false)
                    foreach (var comparer in canonicalComparers)
                    {
                        result ^= comparer.GetHashCode(k);
                        result *= FNVHashPrime;
                    }

            return result;
        }

    }

    /// <summary>
    /// Compares one part instance against another. Does not iterate key links towards root. 
    /// </summary>
    public class ParametrizerCanonicalComparer<T> : IEqualityComparer<T>
    {
        IAssetKeyParametrizer parametrizer;

        public ParametrizerCanonicalComparer(IAssetKeyParametrizer parametrizer)
        {
            this.parametrizer = parametrizer ?? throw new ArgumentException(nameof(parametrizer));
        }

        public bool Equals(T x, T y)
        {
            string[] x_parameters = parametrizer.GetPartParameters(x), y_parameters = parametrizer.GetPartParameters(y);
            if (x_parameters == null && y_parameters == null) return true;
            if (x_parameters == null || y_parameters == null) return false;
            if (x_parameters.Length != y_parameters.Length) return false;
            for (int i=0; i<x_parameters.Length; i++)
            {
                string x_parameter = x_parameters[i], y_parameter = y_parameters[i];
                if (x_parameter != y_parameter) return false;
                string x_value = parametrizer.GetPartValue(x, x_parameter), y_value = parametrizer.GetPartValue(y, y_parameter);
                if (x_value != y_value) return false;
            }
            return true;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;
        public int GetHashCode(T obj)
        {
            if (obj == null) return 0;
            string[] parameters = parametrizer.GetPartParameters(obj);
            if (parameters == null || parameters.Length == 0) return 0;
            int hash = FNVHashBasis;
            foreach(string parameter in parameters)
            {
                string value = parametrizer.GetPartValue(obj, parameter);
                if (value==null) continue;
                hash ^= parameter.GetHashCode();
                hash ^= value.GetHashCode();
                //hash *= FNVHashPrime;
            }
            return hash;
        }
    }

    /// <summary>
    /// Compares all non-canonical parameters.
    /// </summary>
    public class ParametrizerNonCanonicalComparer : IEqualityComparer<IAssetKey>
    {
        IAssetKeyParametrizer parametrizer;

        public ParametrizerNonCanonicalComparer(IAssetKeyParametrizer parametrizer)
        {
            this.parametrizer = parametrizer ?? throw new ArgumentException(nameof(parametrizer));
        }

        public bool Equals(IAssetKey x, IAssetKey y)
        {
            LazyList<(string, string)> x_parameters = new LazyList<(string, string)>();

            // Get x's (parameter, value) pairs
            for (IAssetKey x_part = x; x_part != null; x_part = x_part.GetPreviousKey())
            {
                // Get parameters
                string[] x_part_parametres = parametrizer.GetPartParameters(x_part);
                if (x_part_parametres == null || x_part_parametres.Length == 0) continue;

                foreach(string x_part_parameter in x_part_parametres)
                {
                    // Is non-canonical
                    if (parametrizer.IsCanonical(x_part, x_part_parameter)) continue;

                    // Get value
                    string x_part_parameter_value = parametrizer.GetPartValue(x_part, x_part_parameter);
                    if (x_part_parameter_value == null) continue;

                    // Has this parameter been added already. 
                    int ix = -1;
                    for (int i = 0; i < x_parameters.Count; i++) if (x_parameters[i].Item1 == x_part_parameter) { ix = i; break; }
                    // Last value stands.
                    if (ix >= 0) break;

                    // Add to list
                    x_parameters.Add((x_part_parameter, x_part_parameter_value));
                }
            }

            // Match against y's
            int count = 0;
            for (IAssetKey y_part = y; y_part != null; y_part = y_part.GetPreviousKey())
            {
                // Get parameters
                string[] y_part_parametres = parametrizer.GetPartParameters(y_part);
                if (y_part_parametres == null || y_part_parametres.Length == 0) continue;

                foreach(string y_part_parameter in y_part_parametres)
                {
                    // Is non-canonical
                    if (parametrizer.IsCanonical(y_part, y_part_parameter)) continue;

                    // Get y value
                    string y_value = parametrizer.GetPartValue(y_part, y_part_parameter);
                    if (y_value == null) continue;

                    // Test if x had one
                    string x_value = null;
                    int ix = -1;
                    for (int i = 0; i < x_parameters.Count; i++)
                    {
                        if (x_parameters[i].Item1 == y_part_parameter)
                        {
                            x_value = x_parameters[i].Item2;
                            ix = i;
                            break;
                        }
                    }

                    // Y has y_part_parameter that x doesn'y have a corresponding value
                    if (ix < 0) return false;

                    // y_part_parameter has already been matched. It was ok. 
                    // This is second time for this parameter, the last in chain value only matters.
                    // Thereok, we can ignore this
                    if (x_value == null) continue;

                    // Value differs
                    if (x_value != y_value) return false;

                    // Value is same. We null x_parameter[ix].value as a signal that the paramter name has already been checked.
                    x_parameters[ix] = (y_part_parameter, null);
                    count++;
                }
            }

            // X had some values Y didn't
            if (count != x_parameters.Count) return false;

            return true;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;
        public int GetHashCode(IAssetKey obj)
        {
            int hash = 0;
            // Get x's (parameter, value) pairs
            for (IAssetKey part = obj; part != null; part = part.GetPreviousKey())
            {
                // Get parameters
                string[] part_parametres = parametrizer.GetPartParameters(part);
                if (part_parametres == null || part_parametres.Length == 0) continue;

                foreach (string part_parameter in part_parametres)
                {
                    // Is non-canonical
                    if (parametrizer.IsCanonical(part, part_parameter)) continue;

                    // Get value
                    string part_parameter_value = parametrizer.GetPartValue(part, part_parameter);
                    if (part_parameter_value == null) continue;

                    hash ^= part_parameter.GetHashCode();
                    hash ^= part_parameter_value.GetHashCode();
                }
            }
            return hash;
        }
    }

}
