// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, IParameterInfo>
    {
        private static IReadOnlyDictionary<string, IParameterInfo> instance = new ParameterInfos()
            .Add("Root", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -8000)
            .Add("Culture", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: -6000)
            .Add("Location", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: -4000)
            .Add("Assembly", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: -2000)
            .Add("Resource", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: 0000)
            .Add("Section", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: 2000)
            .Add("Type", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: 2000)
            .Add("Key", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: false, sortingOrder: 6000)
            .Add("N", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8000)
            .Add("N1", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8100)
            .Add("N2", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8200)
            .Add("N3", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8300)
            .Add("N4", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8400)
            .Add("N5", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8500)
            .Add("N6", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8600)
            .Add("N7", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8700)
            .Add("N8", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8800)
            .Add("N9", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 8900)
            .Add("N10", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9000)
            .Add("N11", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9100)
            .Add("N12", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9200)
            .Add("N13", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9300)
            .Add("N14", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 9400)
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
        /// Create basic parameter info
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="isCanonicalCompared"></param>
        /// <param name="isNonCanonicalCompared"></param>
        /// <param name="isSection"></param>
        /// <param name="sortingOrder"></param>
        public ParameterInfo(string parameterName, bool isCanonicalCompared, bool isNonCanonicalCompared, bool isSection, int sortingOrder)
        {
            ParameterName = parameterName;
            IsCanonical = isCanonicalCompared;
            IsNonCanonical = isNonCanonicalCompared;
            IsSection = isSection;
            Order = sortingOrder;
        }
    }

    /// <summary>
    /// Extension methods for parameter infos.
    /// </summary>
    public static class ParameterInfoExtensions
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
        /// <returns><paramref name="infos"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IDictionary<string, IParameterInfo> Add(this IDictionary<string, IParameterInfo> infos, string parameterName, bool isCanonicalCompare, bool isNonCanonicalCompare, bool isSection, int sortingOrder)
        {
            if (infos.ContainsKey(parameterName)) throw new ArgumentException($"Parameter {parameterName} already exists", nameof(parameterName));
            infos[parameterName] = new ParameterInfo(parameterName, isCanonicalCompare, isNonCanonicalCompare, isSection, sortingOrder);
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
