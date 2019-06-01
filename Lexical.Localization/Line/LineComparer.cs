// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
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
        private static LineComparer instance = new LineComparer(ParameterInfos.Default).AddCanonicalComparer(ParameterComparer.Default).AddComparer(NonCanonicalComparer.AllParameters).SetReadonly();
        private static LineComparer ignoreCulture = new LineComparer(ParameterInfos.Default).AddCanonicalComparer(ParameterComparer.Default).AddComparer(NonCanonicalComparer.IgnoreCulture).SetReadonly();
        private static LineComparer parameters = new LineComparer(null).AddParameterComparer(ParameterComparer.Default).SetReadonly();

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
        ///    Format Args              not compared (<see cref="LineValueComparer"/>.
        ///    Inlining                 not compared
        ///    CulturePolicy            not compared
        ///    Root                     not compared
        /// </summary>
        public static LineComparer Default => instance;

        /// <summary>
        /// Comparer that compares parameters.
        /// </summary>
        public static LineComparer Parameters => parameters;

        /// <summary>
        /// Comparer that is oblivious to "Culture" parameter.
        /// </summary>
        public static LineComparer IgnoreCulture => ignoreCulture;

        /// <summary>
        /// Compares line's effective values for hash-equality.
        /// </summary>
        public static IEqualityComparer<ILine> Value => LineValueComparer.Default;

        /// <summary>
        /// List of canonical comparers that compare <see cref="ILineCanonicalKey"/> parts.
        /// </summary>
        List<IEqualityComparer<ILine>> canonicalComparers = new List<IEqualityComparer<ILine>>();

        /// <summary>
        /// List of parameter comparers that compare <see cref="ILineParameter"/> parts.
        /// </summary>
        List<IEqualityComparer<ILine>> parameterComparers = new List<IEqualityComparer<ILine>>();

        /// <summary>
        /// List of generic whole line comparers.
        /// </summary>
        List<IEqualityComparer<ILine>> comparers = new List<IEqualityComparer<ILine>>();

        /// <summary>
        /// (optional) Parameter infos for determining if parameter is key.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Is comparer locked to immutable state.
        /// </summary>
        bool immutable;

        /// <summary>
        /// Create new comparer. Canonical and non-canonical comparers must be added as components.
        /// </summary>
        /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key. <see cref="ParameterInfos.Default"/> for default infos.</param>
        public LineComparer(IParameterInfos parameterInfos)
        {
            this.parameterInfos = parameterInfos;
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
        /// Add parameter comparer. Parameter comparer is applied to each key that implements <see cref="ILineParameter"/>.
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public LineComparer AddParameterComparer(IEqualityComparer<ILine> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (immutable) throw new InvalidOperationException("immutable");
            parameterComparers.Add(comparer);
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
            if (canonicalComparers.Count > 0)
            {
                StructList12<ILineParameter> x_canonicals = new StructList12<ILineParameter>(), y_canonicals = new StructList12<ILineParameter>();
                x.GetCanonicalKeys<StructList12<ILineParameter>>(ref x_canonicals, parameterInfos);
                y.GetCanonicalKeys<StructList12<ILineParameter>>(ref y_canonicals, parameterInfos);
                if (x_canonicals.Count != y_canonicals.Count) return false;
                for (int i = 0; i < x_canonicals.Count; i++)
                {
                    ILineParameter x_key = x_canonicals[i], y_key = y_canonicals[i];
                    // Run comparers
                    for (int j=0; j<canonicalComparers.Count; j++)
                        if (!canonicalComparers[j].Equals(x_key, y_key))
                            return false;
                }
            }

            // Parameter part comparers
            if (parameterComparers.Count > 0)
            {
                StructList12<ILineParameter> x_parameters = new StructList12<ILineParameter>(), y_parameters = new StructList12<ILineParameter>();
                x.GetParameterParts<StructList12<ILineParameter>>(ref x_parameters);
                y.GetParameterParts<StructList12<ILineParameter>>(ref y_parameters);
                if (x_parameters.Count != y_parameters.Count) return false;
                for (int i = 0; i < x_parameters.Count; i++)
                {
                    ILineParameter x_key = x_parameters[i], y_key = y_parameters[i];
                    // Run comparers
                    for (int j = 0; j < parameterComparers.Count; j++)
                        if (!parameterComparers[j].Equals(x_key, y_key))
                            return false;
                }
            }

            return true;
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
            // Get-or-calculate cached hashcode with ILineDefaultHashCode.
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
            if (canonicalComparers.Count > 0)
            {
                StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                line.GetCanonicalKeys<StructList12<ILineParameter>>(ref list, parameterInfos);
                for (int i = 0; i < list.Count; i++)
                {
                    var key = list[i];
                    // hash in canonical comparer 
                    foreach (var comparer in canonicalComparers)
                    {
                        // Use canonical comparer (the order of canonical comparisons should not be hashed in)
                        result ^= comparer.GetHashCode(key);
                    }
                }
            }

            // Parameter hashing
            if (parameterComparers.Count > 0)
            {
                StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                line.GetParameterParts<StructList12<ILineParameter>>(ref list);
                for (int i = 0; i < list.Count; i++)
                {
                    var key = list[i];
                    // hash in canonical comparer 
                    foreach (var comparer in parameterComparers)
                    {
                        result ^= comparer.GetHashCode(key);
                        result *= FNVHashPrime;
                    }
                }
            }

            return result;
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
        public static ParameterComparer Default => instance;

        /// <summary>
        /// Compare two keys for paramter name and value. 
        /// Keys are assumed to implement <see cref="ILineParameter"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
        {
            string x_parameter = x.GetParameterName();
            string y_parameter = y.GetParameterName();
            if (x_parameter == null && y_parameter == null) return true;
            if (x_parameter == null || y_parameter == null) return false;
            if (x_parameter != y_parameter) return false;
            return x.GetParameterValue() == y.GetParameterValue();
        }

        const int FNVHashBasis = unchecked((int)0x811C9DC5);

        /// <summary>
        /// Calculate hashcode for parameter name and value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
        {
            string parameterName = line.GetParameterName();
            if (parameterName == null) return 0;
            int hash = FNVHashBasis;
            hash ^= parameterName.GetHashCode();
            string parameterValue = line.GetParameterValue();
            hash ^= parameterValue == null ? 0 : parameterValue.GetHashCode();
            return hash;
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
        private static NonCanonicalComparer all = new NonCanonicalComparer(parameterInfos: ParameterInfos.Default, keyNamesToIgnore: null);
        private static NonCanonicalComparer Ignore_culture = new NonCanonicalComparer(parameterInfos: ParameterInfos.Default, keyNamesToIgnore: new string[] { "Culture" });

        /// <summary>
        /// Default instance that compares every non-canonical parameter.
        /// </summary>
        public static NonCanonicalComparer AllParameters => all;

        /// <summary>
        /// Instance that excludes "Culture" key from comparison.
        /// </summary>
        public static NonCanonicalComparer IgnoreCulture => Ignore_culture;

        /// <summary>
        /// List of parameter names to ignore.
        /// </summary>
        protected HashSet<string> parameterNamesToIgnore;

        /// <summary>
        /// (optional) Parameter infos for determining if parameter is key.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Create new comparer of <see cref="ILineParameter"/> and <see cref="ILineNonCanonicalKey"/> keys.
        /// </summary>
        /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key. <see cref="ParameterInfos.Default"/> for default infos.</param>
        /// <param name="keyNamesToIgnore">(optional) list of parameter names to not to compare</param>
        public NonCanonicalComparer(IParameterInfos parameterInfos = null, IEnumerable<string> keyNamesToIgnore = null)
        {
            this.parameterInfos = parameterInfos;
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
            StructList8<KeyValuePair<string, string>> x_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
            StructList8<KeyValuePair<string, string>> y_parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
            x.GetNonCanonicalKeyPairs<StructList8<KeyValuePair<string, string>>>(ref x_parameters, parameterInfos);
            y.GetNonCanonicalKeyPairs<StructList8<KeyValuePair<string, string>>>(ref y_parameters, parameterInfos);

            // Compare count
            if (x_parameters.Count != y_parameters.Count) return false;

            // Sort lists
            sorter.Sort(ref x_parameters);
            sorter.Sort(ref y_parameters);

            // Pair comparison of the sorted lists
            for (int i = 0; i < x_parameters.Count; i++)
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
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
        {
            StructList8<KeyValuePair<string, string>> parameters = new StructList8<KeyValuePair<string, string>>(KeyValuePairEqualityComparer<string, string>.Default);
            line.GetNonCanonicalKeyPairs<StructList8<KeyValuePair<string, string>>>(ref parameters, parameterInfos);
            int hash = 0;
            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                // Hash in only if value is non-""
                if (!String.IsNullOrEmpty(param.Value))
                {
                    hash ^= param.Key.GetHashCode();
                    hash ^= param.Value.GetHashCode();
                }
            }
            return hash;
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

    /// <summary>
    /// Compares effective value for equality and calculate comparable hash-code.
    /// 
    /// The last part that defines "String" or <see cref="ILineString"/> is considered effective.
    /// </summary>
    public class LineValueComparer : IEqualityComparer<ILine>
    {
        private static LineValueComparer instance = new LineValueComparer();

        /// <summary>
        /// Get parameter comparer.
        /// </summary>
        public static LineValueComparer Default => instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
        {
            string x_value = null, y_value = null;
            if (x.TryGetStringText(out x_value) != y.TryGetStringText(out y_value)) return false;
            // XXX Ignores StringFormat -- not really essential
            return x_value == y_value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
        {
            string text;
            if (line.TryGetStringText(out text)) return text.GetHashCode();
            // XXX Ignores StringFormat -- not really essential
            return 0;
        }
    }

}
