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
        private static AssetKeyComparer instance = new AssetKeyComparer().AddCanonicalComparer(ParametrizedComparer.Instance).AddNonCanonicalComparer(ParametrizedNonCanonicalComparer.Instance);
        private static AssetKeyComparer ignore_culture = new AssetKeyComparer().AddCanonicalComparer(ParametrizedComparer.Instance).AddNonCanonicalComparer(ParametrizedNonCanonicalComparer.IgnoreCulture);

        /// <summary>
        /// Makes comparisons on interface level. 
        /// Compares parametrized properties of keys.
        /// 
        /// This comparer uses the following pattern for comparisons:
        ///    Key                      canonical compare
        ///    Section                  canonical compare
        ///    Type                     canonical compare
        ///    Resource                 canonical compare
        ///    Location                 canonical compare
        ///    Assembly                 non-canonical compare
        ///    Culture                  non-canonical compare
        ///    Format Args              non-canonical compare
        ///    Inlining                 non-canonical compare
        ///    CulturePolicy            not compared
        ///    Root                     not compared
        /// </summary>
        public static AssetKeyComparer Default => instance;

        /// <summary>
        /// Comparer that is oblivious to "Culture" parameter.
        /// </summary>
        public static AssetKeyComparer IgnoreCulture => ignore_culture;

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
            // ^ Todo: unwrap recursion.

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
                  // ^ Todo: unwrap recursion.

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
    /// 
    /// If <see cref="IAssetKeyNonCanonicallyCompared"/> occurs more than once, only the left-most is effective for compare purposes.
    /// </summary>
    public class ParametrizedNonCanonicalComparer : IEqualityComparer<IAssetKey>
    {
        private static ParametrizedNonCanonicalComparer all = new ParametrizedNonCanonicalComparer(parameterNamesToIgnore: null);
        private static ParametrizedNonCanonicalComparer Ignore_culture = new ParametrizedNonCanonicalComparer(parameterNamesToIgnore: new string[] { "Culture" });

        /// <summary>
        /// Default instance that compares every non-canonical parameter.
        /// </summary>
        public static ParametrizedNonCanonicalComparer Instance => all;

        /// <summary>
        /// Instance that excludes "Culture" parameter from comparison.
        /// </summary>
        public static ParametrizedNonCanonicalComparer IgnoreCulture => Ignore_culture;

        /// <summary>
        /// List of parameter names to ignore.
        /// </summary>
        protected HashSet<string> parameterNamesToIgnore;

        /// <summary>
        /// Create new comparer of <see cref="IAssetKeyParameterAssigned"/> and <see cref="IAssetKeyNonCanonicallyCompared"/> keys.
        /// </summary>
        /// <param name="parameterNamesToIgnore">(optional) list of parameter names to not to compare</param>
        public ParametrizedNonCanonicalComparer(IEnumerable<string> parameterNamesToIgnore = null)
        {
            if (parameterNamesToIgnore != null) this.parameterNamesToIgnore = new HashSet<string>(parameterNamesToIgnore);
        }

        public bool Equals(IAssetKey x, IAssetKey y)
        {
            // Get x's (parameter, value) pairs
            StructList8<KeyValuePair<string, string>> x_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
            for (IAssetKey x_node = x; x_node != null; x_node = x_node.GetPreviousKey())
            {
                // Is non-canonical
                if (x_node is IAssetKeyNonCanonicallyCompared == false) continue;

                // Get parameter
                string x_parameter_name = x_node.GetParameterName(), x_parameter_value = x_node.Name;
                if (x_parameter_name == null || x_parameter_value == null) continue;

                // Is this parameter excluded
                if (parameterNamesToIgnore != null && parameterNamesToIgnore.Contains(x_parameter_name)) continue;

                // Previous occurance x_parameters table index
                int ix = -1;
                // Has this parameter been added already. 
                for (int i = 0; i < x_parameters.Count; i++) if (x_parameters[i].Key == x_parameter_name) { ix = i; break; }
                // Left-most value stands.
                if (ix >= 0)
                {
                    // Update table
                    x_parameters[ix] = new KeyValuePair<string, string>(x_parameter_name, x_parameter_value);
                }
                else
                {
                    // Add to list
                    x_parameters.Add( new KeyValuePair<string, string>(x_parameter_name, x_parameter_value) );
                }
            }

            // Get y's (parameter, value) pairs
            StructList8<KeyValuePair<string, string>> y_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
            for (IAssetKey y_node = y; y_node != null; y_node = y_node.GetPreviousKey())
            {
                // Is non-canonical
                if (y_node is IAssetKeyNonCanonicallyCompared == false) continue;

                // Get parameter
                string y_parameter_name = y_node.GetParameterName(), y_parameter_value = y_node.Name;
                if (y_parameter_name == null || y_parameter_value == null) continue;

                // Is this parameter excluded
                if (parameterNamesToIgnore != null && parameterNamesToIgnore.Contains(y_parameter_name)) continue;

                // Previous occurance y_parameters table index
                int ix = -1;
                // Has this parameter been added already. 
                for (int i = 0; i < y_parameters.Count; i++) if (y_parameters[i].Key == y_parameter_name) { ix = i; break; }
                // Left-most value stands.
                if (ix >= 0)
                {
                    // Update table
                    y_parameters[ix] = new KeyValuePair<string, string>(y_parameter_name, y_parameter_value);
                }
                else
                {
                    // Add to list
                    y_parameters.Add(new KeyValuePair<string, string>(y_parameter_name, y_parameter_value));
                }
            }

            // Remove values where parameterValue==""
            for (int ix = 0; ix < x_parameters.Count;) if (x_parameters[ix].Value == "") x_parameters.RemoveAt(ix); else ix++;
            for (int ix = 0; ix < y_parameters.Count;) if (y_parameters[ix].Value == "") y_parameters.RemoveAt(ix); else ix++;

            // Compare count
            if (x_parameters.Count != y_parameters.Count) return false;

            // Sort arrays
            sorter.Sort(ref x_parameters);
            sorter.Sort(ref y_parameters);

            // Pair comparison to sorted lists
            for (int i=0; i<x_parameters.Count; i++)
            {
                KeyValuePair<string, string> x_parameter = x_parameters[i], y_parameter = y_parameters[i];
                if (x_parameter.Key != y_parameter.Key || x_parameter.Value != y_parameter.Value) return false;
            }

            return true;
        }

        static StructListSorter<StructList8<KeyValuePair<string, string>>, KeyValuePair<string, string>> sorter = new StructListSorter<StructList8<KeyValuePair<string, string>>, KeyValuePair<string, string>>(KeyValuePairComparer<string, string>.Default);
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

                // Is this parameter excluded
                if (parameterNamesToIgnore != null && parameterNamesToIgnore.Contains(parameterName)) continue;

                // Get value.
                string parameter_value = k.Name;
                if (parameter_value == null) continue;

                // Test if this parameter is yet to occure again towards left of the key
                bool firstOccurance = true;
                for (IAssetKey kk = k.GetPreviousKey(); kk != null; kk = kk.GetPreviousKey())
                    if (kk is IAssetKeyNonCanonicallyCompared && kk.GetParameterName() == parameterName)
                    {
                        firstOccurance = false;
                        break;
                    }
                // Ignore this occurance as this non-canonical part occurs again.
                if (!firstOccurance) continue;

                // Hash in only if value is non-""
                if (parameter_value != "")
                {
                    hash ^= parameterName.GetHashCode();
                    hash ^= parameter_value.GetHashCode();
                }
            }
            return hash;
        }
    }

}
