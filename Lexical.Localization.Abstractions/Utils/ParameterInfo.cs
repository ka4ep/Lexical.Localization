// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, IParameterInfo>
    {
        private static IReadOnlyDictionary<string, IParameterInfo> instance = new ParameterInfos()
            .Add(new ParameterInfo("Root", isCanonicalCompare: false, isNonCanonicalCompare: false, isSection: false, sortingOrder: -8000))
            .Add(new ParameterInfo("Culture", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: -7000))
            .Add(new ParameterInfo("Assembly", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: -6000))
            .Add(new ParameterInfo("Location", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: -5000))
            .Add(new ParameterInfo("Resource", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: -4000))
            .Add(new ParameterInfo("Type", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: true, sortingOrder: -3000))
            .Add(new ParameterInfo("Section", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: true, sortingOrder: -2000))
            .Add(new ParameterInfo("Key", isCanonicalCompare: true, isNonCanonicalCompare: false, isSection: false, sortingOrder: 1000))
            .Add(new ParameterInfo("N", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5000))
            .Add(new ParameterInfo("N1", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5100))
            .Add(new ParameterInfo("N2", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5200))
            .Add(new ParameterInfo("N3", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5300))
            .Add(new ParameterInfo("N4", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5400))
            .Add(new ParameterInfo("N5", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5500))
            .Add(new ParameterInfo("N6", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5600))
            .Add(new ParameterInfo("N7", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5700))
            .Add(new ParameterInfo("N8", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5800))
            .Add(new ParameterInfo("N9", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 5900))
            .Add(new ParameterInfo("N10", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 6000))
            .Add(new ParameterInfo("N11", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 6100))
            .Add(new ParameterInfo("N12", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 6200))
            .Add(new ParameterInfo("N13", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 6300))
            .Add(new ParameterInfo("N14", isCanonicalCompare: false, isNonCanonicalCompare: true, isSection: false, sortingOrder: 6400))
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
        bool IsCanonicalCompare { get; }

        /// <summary>
        /// Is this parameter non-canonically compared 
        /// </summary>
        bool IsNonCanonicalCompare { get; }

        /// <summary>
        /// Matches against "anysection"
        /// </summary>
        bool IsSection { get; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        int SortingOrder { get; }
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
        public bool IsCanonicalCompare { get; private set; }

        /// <summary>
        /// Is this parameter non-canonically compared 
        /// </summary>
        public bool IsNonCanonicalCompare { get; private set; }

        /// <summary>
        /// Matches against "anysection"
        /// </summary>
        public bool IsSection { get; private set; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        public int SortingOrder { get; private set; }

        /// <summary>
        /// Create basic parameter info
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="isCanonicalCompare"></param>
        /// <param name="isNonCanonicalCompare"></param>
        /// <param name="isSection"></param>
        /// <param name="sortingOrder"></param>
        public ParameterInfo(string parameterName, bool isCanonicalCompare, bool isNonCanonicalCompare, bool isSection, int sortingOrder)
        {
            ParameterName = parameterName;
            IsCanonicalCompare = isCanonicalCompare;
            IsNonCanonicalCompare = isNonCanonicalCompare;
            IsSection = isSection;
            SortingOrder = sortingOrder;
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
        public static IDictionary<string, IParameterInfo> Add(this IDictionary<string, IParameterInfo> infos, IParameterInfo info)
        {
            if (infos.ContainsKey(info.ParameterName)) throw new ArgumentException($"Parameter {info.ParameterName} already exists", nameof(info));
            infos[info.ParameterName] = info;
            return infos;
        }
    }
}
