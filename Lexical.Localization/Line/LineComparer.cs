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
    /// Configurable <see cref="ILine"/> comparer.
    /// 
    /// Canonical and non-canonical comparers can be added as component comparers of <see cref="IEqualityComparer{ILine}"/>.
    /// 
    /// Canonical comparers compare each key link separately.
    /// 
    /// Non-canonical comparers compare key chain as a whole.
    /// </summary>
    public class LineComparer : IEqualityComparer<ILine>
    {
        private static LineComparer instance = new LineComparer().AddCanonicalComparer(ParameterComparer.Instance).AddComparer(NonCanonicalComparer.Instance).SetReadonly();
        private static LineComparer ignoreCulture = new LineComparer().AddCanonicalComparer(ParameterComparer.Instance).AddComparer(NonCanonicalComparer.IgnoreCulture).SetReadonly();

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
        public static LineComparer Default => instance;

        /// <summary>
        /// Comparer that is oblivious to "Culture" parameter.
        /// </summary>
        public static LineComparer IgnoreCulture => ignoreCulture;

        /// <summary>
        /// List of canonical comparers that compare <see cref="ILineCanonicalKey"/> parts separately.
        /// </summary>
        List<IEqualityComparer<ILine>> canonicalComparers = new List<IEqualityComparer<ILine>>();

        /// <summary>
        /// List of generic comparers.
        /// </summary>
        List<IEqualityComparer<ILine>> comparers = new List<IEqualityComparer<ILine>>();

        /// <summary>
        /// Is comparer locked to immutable state.
        /// </summary>
        bool immutable;

        /// <summary>
        /// Create new comparer. Canonical and non-canonical comparers must be added as components.
        /// </summary>
        public LineComparer()
        {
        }

        /// <summary>
        /// Lock comparer into non-modifiable state.
        /// </summary>
        /// <returns></returns>
        public LineComparer SetReadonly()
        {
            immutable = true;
            return this;
        }

        /// <summary>
        /// Add canonical comparer. Canonical comparer is applied to each key that implements <see cref="ILineCanonicalKey"/>.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public LineComparer AddCanonicalComparer(IEqualityComparer<ILine> comparer)
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
        public LineComparer AddComparer(IEqualityComparer<ILine> comparer)
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
        public bool Equals(ILine x, ILine y)
        {
            bool xIsNull = x == null, yIsNull = y == null;
            if (xIsNull && yIsNull) return true;
            if (xIsNull || yIsNull) return false;
            if (Object.ReferenceEquals(x, y)) return true;

            // Test if cached hash-codes mismatch.
            if (this == LineComparer.instance && x is ILineDefaultHashCode x_code && y is ILineDefaultHashCode y_code)
                if (x_code.GetDefaultHashCode() != y_code.GetDefaultHashCode()) return false;

            // Regular comparers
            foreach (var comparer in comparers)
            {
                if (!comparer.Equals(x, y)) return false;
            }

            // Canonical key part comparers
            if (x is ILine x_tail && y is ILine y_tail)
            {
                for (ILine x_key = x_tail.GetCanonicalKey(), y_key = y_tail.GetCanonicalKey(); x != null || y != null; x_key = x_key.GetPreviousCanonicalKey(), y_key = y_key.GetPreviousCanonicalKey())
                {
                    // Ran out of one or another
                    if (x_key == null && y_key == null) break;
                    if (x_key == null || y_key == null) return false;
                    // Reference are equal
                    if (Object.ReferenceEquals(x_key, y_key)) break;
                    // Run comparers
                    foreach (var comparer in canonicalComparers)
                        if (!comparer.Equals(x_key, y_key)) return false;
                }

                // Must be equal
                return true;
            }

            // Comparing ILine's 
            // Not implemented
            return false;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);
        const int FNVHashPrime = 0x1000193;

        /// <summary>
        /// Get or calculate <paramref name="line"/>'s hashcode.
        /// </summary>
        /// <param name="line">(optional) lineto calculate</param>
        /// <returns>hashcode or 0 if <paramref name="line"/> was null</returns>
        public int GetHashCode(ILine line)
        {
            // No key
            if (line == null) return 0;
            // Get-or-calculate cached hashcode with IAssetKeyDefaultHashCode.
            if (this == LineComparer.instance && line is ILineDefaultHashCode defaultHashCode) return defaultHashCode.GetDefaultHashCode();
            // Calculate new hashcode
            return CalculateHashCode(line);
        }

        /// <summary>
        /// Calculate <paramref name="line"/>'s hashcode.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>hashcode or 0 if <paramref name="line"/> was null</returns>
        public int CalculateHashCode(ILine line)
        {
            int result = FNVHashBasis;

            // Non-canonical hashing
            foreach (var comparer in comparers)
            {
                // hash in non-canonical comparer
                result ^= comparer.GetHashCode(line);
            }

            // Canonical hashing
            if (line is ILine tail)
            {
                for (ILine key = tail.GetCanonicalKey(); key != null; key = key.GetPreviousCanonicalKey())
                {
                    // hash in canonical comparer 
                    foreach (var comparer in canonicalComparers)
                        result ^= comparer.GetHashCode(key);
                    result *= FNVHashPrime;
                }

                return result;
            }

            // Calculate line's hashcode
            // Not implemented
            return -1;
        }
    }

    /// <summary>
    /// Compares two keys that are assumed to implement <see cref="ILineParameter"/>.
    /// </summary>
    public class ParameterComparer : IEqualityComparer<ILine>
    {
        private static ParameterComparer instance = new ParameterComparer();

        /// <summary>
        /// Get parameter comparer.
        /// </summary>
        public static ParameterComparer Instance => instance;

        /// <summary>
        /// Compare two keys for paramter name and value. 
        /// Keys are assumed to implement <see cref="ILineParameter"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
        {
            if (x is ILine x_tail && y is ILine y_tail)
            {
                ILine x_part = x_tail, y_part = y_tail;

                string x_parameter = x_tail.GetParameterName();
                string y_parameter = y_tail.GetParameterName();
                if (x_parameter == null && y_parameter == null) return true;
                if (x_parameter == null || y_parameter == null) return false;
                if (x_parameter != y_parameter) return false;
                return x_tail.GetParameterValue() == y_tail.GetParameterValue();
            }

            // Calculate ILines equality
            // not implemented
            return false;
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);

        /// <summary>
        /// Calculate hashcode for parameter name and value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
        {
            if (line is ILine part)
            {
                string parameterName = part.GetParameterName();
                if (parameterName == null) return 0;
                int hash = FNVHashBasis;
                hash ^= parameterName.GetHashCode();
                hash ^= part.GetParameterValue().GetHashCode();
                return hash;
            }

            // Calculate ILine
            // Not implemented
            return -1;
        }
    }

    /// <summary>
    /// Compares all non-canonical parameters keys against each other.
    /// 
    /// These are keys that implement <see cref="ILineParameter"/> and <see cref="ILineNonCanonicalKey"/>.
    /// 
    /// If <see cref="ILineNonCanonicalKey"/> occurs more than once, only the left-most is considered effective.
    /// </summary>
    public class NonCanonicalComparer : IEqualityComparer<ILine>
    {
        private static NonCanonicalComparer all = new NonCanonicalComparer(keyNamesToIgnore: null);
        private static NonCanonicalComparer Ignore_culture = new NonCanonicalComparer(keyNamesToIgnore: new string[] { "Culture" });

        /// <summary>
        /// Default instance that compares every non-canonical parameter.
        /// </summary>
        public static NonCanonicalComparer Instance => all;

        /// <summary>
        /// Instance that excludes "Culture" key from comparison.
        /// </summary>
        public static NonCanonicalComparer IgnoreCulture => Ignore_culture;

        /// <summary>
        /// List of parameter names to ignore.
        /// </summary>
        protected HashSet<string> parameterNamesToIgnore;

        /// <summary>
        /// Create new comparer of <see cref="ILineParameter"/> and <see cref="ILineNonCanonicalKey"/> keys.
        /// </summary>
        /// <param name="keyNamesToIgnore">(optional) list of parameter names to not to compare</param>
        public NonCanonicalComparer(IEnumerable<string> keyNamesToIgnore = null)
        {
            if (keyNamesToIgnore != null) this.parameterNamesToIgnore = new HashSet<string>(keyNamesToIgnore);
        }

        /// <summary>
        /// Compare two keys for all their non-canonical part parameter key-values.
        /// 
        /// Only the left-most value of each parameter name is considered effective.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if keys are equals in terms of non-canonical parameters</returns>
        public bool Equals(ILine x, ILine y)
        {
            if (x is ILine x_tail && y is ILine y_tail)
            {
                // Get x's (parameter, value) pairs
                StructList8<KeyValuePair<string, string>> x_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
                for (ILineNonCanonicalKey x_key = x_tail.GetNonCanonicalKey(); x_key != null; x_key = x_key.GetPreviousNonCanonicalKey())
                {
                    // Get parameter
                    string x_parameter_name = x_key.GetParameterName(), x_parameter_value = x_key.GetParameterValue();
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
                        x_parameters.Add(new KeyValuePair<string, string>(x_parameter_name, x_parameter_value));
                    }
                }

                // Get y's (parameter, value) pairs
                StructList8<KeyValuePair<string, string>> y_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
                for (ILineNonCanonicalKey y_key = y_tail.GetNonCanonicalKey(); y_key != null; y_key = y_key.GetPreviousNonCanonicalKey())
                {
                    // Is non-canonical
                    if (y_key is ILineNonCanonicalKey == false) continue;

                    // Get parameter
                    string y_parameter_name = y_key.GetParameterName(), y_parameter_value = y_key.GetParameterValue();
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
                for (int i = 0; i < x_parameters.Count; i++)
                {
                    KeyValuePair<string, string> x_parameter = x_parameters[i], y_parameter = y_parameters[i];
                    if (x_parameter.Key != y_parameter.Key || x_parameter.Value != y_parameter.Value) return false;
                }

                return true;
            }

            // Calculate ILines
            // Not implemented
            return false;
        }

        static StructListSorter<StructList8<KeyValuePair<string, string>>, KeyValuePair<string, string>> sorter = new StructListSorter<StructList8<KeyValuePair<string, string>>, KeyValuePair<string, string>>(KeyValuePairComparer<string, string>.Default);

        /// <summary>
        /// Calculate hash-code of every non-canonical parameters.
        /// 
        /// Only the left-most value of each parameter name is considered effective.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
        {
            if (line is ILine tail)
            {
                int hash = 0;
                // Get x's (parameter, value) pairs
                for (ILineNonCanonicalKey key = tail.GetNonCanonicalKey(); key != null; key = key.GetPreviousNonCanonicalKey())
                {
                    // Get parameters.
                    string parameterName = key.GetParameterName();
                    if (parameterName == null) continue;

                    // Is this parameter excluded
                    if (parameterNamesToIgnore != null && parameterNamesToIgnore.Contains(parameterName)) continue;

                    // Get value.
                    string parameter_value = key.GetParameterValue();
                    if (parameter_value == null) continue;

                    // Test if this parameter is yet to occure again towards left of the key
                    bool firstOccurance = true;
                    for (ILineNonCanonicalKey k = key.GetPreviousNonCanonicalKey(); k != null; k = k.GetPreviousNonCanonicalKey())
                        if (k.GetParameterName() == parameterName)
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

            // Compare ILines
            // Not implemented
            return -1;
        }
    }

    /// <summary>
    /// This interface accesses default hashcode that is cached in an instance of this interface.
    /// 
    /// The default hash-code is calculated by <see cref="LineComparer.Default"/>.
    /// This interface is only used by the instance of <see cref="LineComparer.Default"/>.
    /// 
    /// The purpose of this interface is to improve performance.
    /// </summary>
    public interface ILineDefaultHashCode
    {
        /// <summary>
        /// Get cached or calculate hashcode with <see cref="LineComparer.Default"/> and <see cref="LineComparer.CalculateHashCode(ILine)"/>.
        /// </summary>
        /// <returns>hash-code</returns>
        int GetDefaultHashCode();
    }

}
