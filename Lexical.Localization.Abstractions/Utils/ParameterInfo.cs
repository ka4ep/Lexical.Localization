// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, IParameterInfo>
    {
        private static IReadOnlyDictionary<string, IParameterInfo> instance = new ParameterInfos()
            .Add("", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -8000, pattern: null)
            .Add("Culture", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: -6000, pattern: new Regex(@"^([a-z]{2,5})(-([A-Za-z]{2,7}))?$", RegexOptions.CultureInvariant | RegexOptions.Compiled))
            .Add("Location", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: -4000, pattern: null)
            .Add("Assembly", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: -2000, pattern: null)
            .Add("Resource", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: 0000, pattern: null)
            .Add("Section", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: 2000, pattern: null)
            .Add("Type", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: 2000, pattern: new Regex("[^:0-9][^:]*", RegexOptions.CultureInvariant | RegexOptions.Compiled))
            .Add("Key", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: false, sortingOrder: 6000, pattern: null)
            .Add("N", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8000, pattern: null)
            .Add("N1", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8100, pattern: null)
            .Add("N2", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8200, pattern: null)
            .Add("N3", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8300, pattern: null)
            .Add("N4", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8400, pattern: null)
            .Add("N5", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8500, pattern: null)
            .Add("N6", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8600, pattern: null)
            .Add("N7", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8700, pattern: null)
            .Add("N8", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8800, pattern: null)
            .Add("N9", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8900, pattern: null)
            .Add("N10", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9000, pattern: null)
            .Add("N11", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9100, pattern: null)
            .Add("N12", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9200, pattern: null)
            .Add("N13", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9300, pattern: null)
            .Add("N14", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9400, pattern: null)
            .Add("N15", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9500, pattern: null)
            .Add("N16", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9600, pattern: null)
            .Add("N17", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9700, pattern: null)
            .Add("N18", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9800, pattern: null)
            .Add("N19", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9900, pattern: null)
            .Add("PluralRules", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -22000, pattern: null)
            .Add("StringFormatFunctions", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -21000, pattern: null)
            .Add("StringFormat", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -200000, pattern: null)
            as IReadOnlyDictionary<string, IParameterInfo>;

        /// <summary>
        /// Default instance that contains info about well-known parameters.
        /// </summary>
        public static IReadOnlyDictionary<string, IParameterInfo> Default => instance;
    }

    /// <summary>
    /// Interface for basic parameter info.
    /// </summary>
    public interface IParameterInfo
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        String ParameterName { get; }

        /// <summary>
        /// Is this parameter canonically compared
        /// </summary>
        bool IsCanonical { get; }

        /// <summary>
        /// Is this parameter non-canonically compared 
        /// </summary>
        bool IsNonCanonical { get; }

        /// <summary>
        /// Matches against "anysection"
        /// </summary>
        bool IsSection { get; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Default capture pattern for IParameterPattern.
        /// </summary>
        Regex Pattern { get; }
    }

    /// <summary>
    /// That that contains parameter info.
    /// </summary>
    public class ParameterInfo : IParameterInfo
    {
        /// <summary>
        /// The parameter name
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Is this parameter canonically compared
        /// </summary>
        public bool IsCanonical { get; private set; }

        /// <summary>
        /// Is this parameter non-canonically compared 
        /// </summary>
        public bool IsNonCanonical { get; private set; }

        /// <summary>
        /// Matches against "anysection"
        /// </summary>
        public bool IsSection { get; private set; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Default capture pattern for IParameterPattern.
        /// </summary>
        public Regex Pattern { get; private set; }

        /// <summary>
        /// Create basic parameter info
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="isCanonicalCompared"></param>
        /// <param name="isNonCanonicalCompared"></param>
        /// <param name="isSection"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="pattern"></param>
        public ParameterInfo(string parameterName, bool isCanonicalCompared, bool isNonCanonicalCompared, bool isSection, int sortingOrder, Regex pattern)
        {
            ParameterName = parameterName;
            IsCanonical = isCanonicalCompared;
            IsNonCanonical = isNonCanonicalCompared;
            IsSection = isSection;
            Order = sortingOrder;
            Pattern = pattern;
        }
    }

    /// <summary>
    /// Extension methods for parameter infos.
    /// </summary>
    public static class IParameterInfoExtensions
    {
        /// <summary>
        /// Add <paramref name="info"/> entry to <paramref name="infos"/>.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="info"></param>
        /// <returns><paramref name="infos"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IDictionary<string, IParameterInfo> Add(this IDictionary<string, IParameterInfo> infos, IParameterInfo info)
        {
            if (infos.ContainsKey(info.ParameterName)) throw new ArgumentException($"Parameter {info.ParameterName} already exists", nameof(info));
            infos[info.ParameterName] = info;
            return infos;
        }

        /// <summary>
        /// Add info entry to <paramref name="infos"/>.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parameterName"></param>
        /// <param name="isCanonicalCompare"></param>
        /// <param name="isNonCanonicalCompare"></param>
        /// <param name="isSection"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="pattern">(optional) capture pattern for <see cref="IParameterPattern"/></param>
        /// <returns><paramref name="infos"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IDictionary<string, IParameterInfo> Add(this IDictionary<string, IParameterInfo> infos, string parameterName, bool isCanonicalCompare, bool isNonCanonicalCompare, bool isSection, int sortingOrder, Regex pattern)
        {
            if (infos.ContainsKey(parameterName)) throw new ArgumentException($"Parameter {parameterName} already exists", nameof(parameterName));
            infos[parameterName] = new ParameterInfo(parameterName, isCanonicalCompare, isNonCanonicalCompare, isSection, sortingOrder, pattern);
            return infos;
        }

        /// <summary>
        /// Canonical parameters.
        /// </summary>
        public static IEnumerable<IParameterInfo> Canonicals(this IReadOnlyDictionary<string, IParameterInfo> infos)
            => infos.Values.Where(pi => pi.IsCanonical);

        /// <summary>
        /// Non-canonical parameters.
        /// </summary>
        public static IEnumerable<IParameterInfo> NonCanonicals(this IReadOnlyDictionary<string, IParameterInfo> infos)
            => infos.Values.Where(pi => pi.IsNonCanonical);

        /// <summary>
        /// Comparable parameters (non-root).
        /// </summary>
        public static IEnumerable<IParameterInfo> Comparables(this IReadOnlyDictionary<string, IParameterInfo> infos)
            => infos.Values.Where(pi => pi.IsNonCanonical|pi.IsCanonical);


    }
}
