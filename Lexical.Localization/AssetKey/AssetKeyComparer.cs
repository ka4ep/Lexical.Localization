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
    /// This interface accesses default hashcode that is cached to an instance of the class.
    /// The default hash-code is calculated by <see cref="AssetKeyComparer.Default"/>.
    /// The purpose of this interface is to improve performance.
    /// </summary>
    public interface IAssetKeyDefaultHashCode
    {
        /// <summary>
        /// Get cached or calculate hashcode with <see cref="AssetKeyComparer.Default"/> and <see cref="AssetKeyComparer.CalculateHashCode(IAssetKey)"/>.
        /// </summary>
        /// <returns>hash-code</returns>
        int GetDefaultHashCode();
    }

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
        private static AssetKeyComparer instance = new AssetKeyComparer().AddCanonicalComparer(ParameterComparer.Instance).AddComparer(ParametrizedNonCanonicalComparer.Instance).SetReadonly();
        private static AssetKeyComparer ignoreCulture = new AssetKeyComparer().AddCanonicalComparer(ParameterComparer.Instance).AddComparer(ParametrizedNonCanonicalComparer.IgnoreCulture).SetReadonly();

        /// <summary>
        /// Makes comparisons on interface level. 
        /// Compares parametrized properties of keys.
        /// 
        /// This comparer uses the following pattern for comparisons:
        ///    Key                      canonical compare
        ///    Section                  canonical compare
        ///    Resource                 canonical compare
        ///    Location                 canonical compare
        ///    Type                     non-canonical compare
        ///    Assembly                 non-canonical compare
        ///    Culture                  non-canonical compare
        ///    Format Args              not compared (<see cref="LocalizationKey.FormatArgsComparer"/>.
        ///    Inlining                 not compared
        ///    CulturePolicy            not compared
        ///    Root                     not compared
        /// </summary>
        public static AssetKeyComparer Default => instance;

        /// <summary>
        /// Comparer that is oblivious to "Culture" parameter.
        /// </summary>
        public static AssetKeyComparer IgnoreCulture => ignoreCulture;

        /// <summary>
        /// List of canonical comparers that set to compare each <see cref="IAssetKeyCanonicallyCompared"/> part separately.
        /// </summary>
        List<IEqualityComparer<IAssetKey>> canonicalComparers = new List<IEqualityComparer<IAssetKey>>();

        /// <summary>
        /// List of generic comparers.
        /// </summary>
        List<IEqualityComparer<IAssetKey>> comparers = new List<IEqualityComparer<IAssetKey>>();

        /// <summary>
        /// Is comparer locked to immutable state.
        /// </summary>
        bool immutable;

        /// <summary>
        /// Create new comparer. Canonical and non-canonical comparers must be added as components.
        /// </summary>
        public AssetKeyComparer()
        {
        }

        /// <summary>
        /// Lock comparer into non-modifiable state.
        /// </summary>
        /// <returns></returns>
        public AssetKeyComparer SetReadonly()
        {
            immutable = true;
            return this;
        }

        /// <summary>
        /// Add canonical comparer. Canonical comparer is applied to each key part that implements <see cref="IAssetKeyCanonicallyCompared"/>.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public AssetKeyComparer AddCanonicalComparer(IEqualityComparer<IAssetKey> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (immutable) throw new InvalidOperationException("immutable");
            canonicalComparers.Add(comparer);
            return this;
        }

        /// <summary>
        /// Add generic key comparer that evaluates hash-equals for the full keys.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public AssetKeyComparer AddComparer(IEqualityComparer<IAssetKey> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (immutable) throw new InvalidOperationException("immutable");
            comparers.Add(comparer);
            return this;
        }

        /// <summary>
        /// Compare <paramref name="x"/> and <paramref name="y"/> for equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if equal</returns>
        public bool Equals(IAssetKey x, IAssetKey y)
        {
            bool xIsNull = x == null, yIsNull = y == null;
            if (xIsNull && yIsNull) return true;
            if (xIsNull || yIsNull) return false;
            if (Object.ReferenceEquals(x, y)) return true;

            // Regular comparers
            foreach(var comparer in comparers)
            {
                if (!comparer.Equals(x, y)) return false;
            }

            // Canonical key part comparers
            for (IAssetKey _x = x.GetCanonicalKey(), _y=y.GetCanonicalKey(); x!=null || y!=null; _x=_x.GetPreviousCanonicalKey(), _y=_y.GetPreviousCanonicalKey())
            {
                // Ran out of one or another
                if (_x == null && _y == null) break;
                if (_x == null || _y == null) return false;
                // Reference are equal
                if (Object.ReferenceEquals(_x, _y)) break;
                // Run comparers
                foreach (var comparer in canonicalComparers)
                    if (!comparer.Equals(_x, _y)) return false;
            }

            // Must be equal
            return true;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;

        /// <summary>
        /// Get or calculate <paramref name="key"/>'s hashcode.
        /// </summary>
        /// <param name="key">(optional) key to calculate</param>
        /// <returns>hashcode or 0 if <paramref name="key"/> was null</returns>
        public int GetHashCode(IAssetKey key)
        {
            // No key
            if (key == null) return 0;
            // Get-or-calculate hashcode from IAssetKeyDefaultHashCode.
            if (this == AssetKeyComparer.instance && key is IAssetKeyDefaultHashCode defaultHashCode) return defaultHashCode.GetDefaultHashCode();
            // Calculate new hashcode
            return CalculateHashCode(key);
        }

        /// <summary>
        /// Calculate <paramref name="key"/>'s hashcode.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>hashcode or 0 if <paramref name="key"/> was null</returns>
        public int CalculateHashCode(IAssetKey key)
        {
            int result = FNVHashBasis;

            // Non-canonical hashing
            foreach (var comparer in comparers)
            {
                // hash in non-canonical comparer
                result ^= comparer.GetHashCode(key);
            }

            // Canonical hashing
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                if (k is IAssetKeyCanonicallyCompared)
                {
                    // hash in canonical comparer 
                    foreach (var comparer in canonicalComparers)
                        result ^= comparer.GetHashCode(k);
                    result *= FNVHashPrime;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Compares two keys that are assumed to implement <see cref="IAssetKeyParameterAssigned"/>.
    /// </summary>
    public class ParameterComparer : IEqualityComparer<IAssetKey>
    {
        private static ParameterComparer instance = new ParameterComparer();

        /// <summary>
        /// Get parameter comparer.
        /// </summary>
        public static ParameterComparer Instance => instance;

        /// <summary>
        /// Compare two keys for paramter name and value. 
        /// Keys are assumed to implement <see cref="IAssetKeyParameterAssigned"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Calculate hashcode for parameter name and value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
    /// Compares all non-canonical parameters keys against each other.
    /// 
    /// These are keys that implement <see cref="IAssetKeyParameterAssigned"/> and <see cref="IAssetKeyNonCanonicallyCompared"/>.
    /// 
    /// If <see cref="IAssetKeyNonCanonicallyCompared"/> occurs more than once, only the left-most is considered effective.
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

        /// <summary>
        /// Compare two keys for all their non-canonical part parameter key-values.
        /// 
        /// Only the left-most value of each parameter name is considered effective.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if keys are equals in terms of non-canonical parameters</returns>
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

        /// <summary>
        /// Calculate hash-code of every non-canonical parameters.
        /// 
        /// Only the left-most value of each parameter name is considered effective.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
