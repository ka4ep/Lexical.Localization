// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, IParameterInfo>, IParameterInfosEnumerable, IParameterInfosMap, IParameterInfosWritable
    {
        private static ParameterInfos instance = new ParameterInfos()
            .Add("Culture", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: -6000, pattern: new Regex(@"^([a-z]{2,5})(-([A-Za-z]{2,7}))?$", RegexOptions.CultureInvariant | RegexOptions.Compiled))
            .Add("Location", interfaceType: typeof(ILineCanonicalKey), sortingOrder: -4000, pattern: null)
            .Add("Assembly", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: -2000, pattern: null)
            .Add("BaseName", interfaceType: typeof(ILineCanonicalKey), sortingOrder: 0000, pattern: null)
            .Add("Section", interfaceType: typeof(ILineCanonicalKey), sortingOrder: 2000, pattern: null)
            .Add("Type", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 2000, pattern: new Regex("[^:0-9][^:]*", RegexOptions.CultureInvariant | RegexOptions.Compiled))
            .Add("Key", interfaceType: typeof(ILineCanonicalKey), sortingOrder: 6000, pattern: null)
            .Add("N", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8000, pattern: null)
            .Add("N1", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8100, pattern: null)
            .Add("N2", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8200, pattern: null)
            .Add("N3", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8300, pattern: null)
            .Add("N4", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8400, pattern: null)
            .Add("N5", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8500, pattern: null)
            .Add("N6", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8600, pattern: null)
            .Add("N7", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8700, pattern: null)
            .Add("N8", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8800, pattern: null)
            .Add("N9", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 8900, pattern: null)
            .Add("N10", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9000, pattern: null)
            .Add("N11", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9100, pattern: null)
            .Add("N12", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9200, pattern: null)
            .Add("N13", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9300, pattern: null)
            .Add("N14", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9400, pattern: null)
            .Add("N15", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9500, pattern: null)
            .Add("N16", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9600, pattern: null)
            .Add("N17", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9700, pattern: null)
            .Add("N18", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9800, pattern: null)
            .Add("N19", interfaceType: typeof(ILineNonCanonicalKey), sortingOrder: 9900, pattern: null)
            .Add("PluralRules", interfaceType: typeof(ILineHint), sortingOrder: -22000, pattern: null)
            .Add("StringFormatFunctions", interfaceType: typeof(ILineHint), sortingOrder: -21000, pattern: null)
            .Add("StringFormat", interfaceType: typeof(ILineHint), sortingOrder: -200000, pattern: null)
            as ParameterInfos;

        /// <summary>
        /// Default instance that contains info about well-known parameters.
        /// </summary>
        public static IParameterInfos Default => instance;

        IEnumerator<IParameterInfo> IEnumerable<IParameterInfo>.GetEnumerator() => Values.GetEnumerator();
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
        /// Describes how the parameter is to be compared.
        /// <list type="bullet">
        ///     <item><see cref="ILineHint"/>not used with comparison.</item>
        ///     <item><see cref="ILineCanonicalKey"/>hash-equals comparable</item>
        ///     <item><see cref="ILineNonCanonicalKey"/>hash-equals comparable</item>
        /// </list>
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Default capture pattern for ILinePattern.
        /// </summary>
        public Regex Pattern { get; private set; }

        /// <summary>
        /// Create basic parameter info
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="interfaceType">Describes how the parameter is to be compared.
        /// <list type="bullet">
        ///     <item><see cref="ILineHint"/>not used with comparison.</item>
        ///     <item><see cref="ILineCanonicalKey"/>hash-equals comparable</item>
        ///     <item><see cref="ILineNonCanonicalKey"/>hash-equals comparable</item>
        /// </list>
        /// </param>
        /// <param name="sortingOrder"></param>
        /// <param name="pattern"></param>
        public ParameterInfo(string parameterName, Type interfaceType, int sortingOrder, Regex pattern)
        {
            ParameterName = parameterName;
            Order = sortingOrder;
            Pattern = pattern;
            InterfaceType = interfaceType;
        }
    }

}
