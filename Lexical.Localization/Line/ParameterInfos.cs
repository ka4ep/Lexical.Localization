// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Resolver;
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Utils
{
    /// <summary>
    /// Information about parameters.
    /// </summary>
    public class ParameterInfos : Dictionary<string, IParameterInfo>, IParameterInfosEnumerable, IParameterInfosMap, IParameterInfosWritable, 
        IParameterResolver, 
        ILineFactory<ILineParameter, string, string>, ILineFactory<ILineHint, string, string>, ILineFactory<ILineNonCanonicalKey, string, string>, ILineFactory<ILineCanonicalKey, string, string>, ILineFactoryParameterInfos, ILineFactoryResolver
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
            .Add("StringFormat", interfaceType: typeof(ILineHint), sortingOrder: -18000, pattern: null)
            .Add("CulturePolicy", interfaceType: typeof(ILineHint), sortingOrder: -19000, pattern: null)
            .Add("FormatProvider", interfaceType: typeof(ILineHint), sortingOrder: -20000, pattern: null)
            .Add("Functions", interfaceType: typeof(ILineHint), sortingOrder: -21000, pattern: null)
            .Add("PluralRules", interfaceType: typeof(ILineHint), sortingOrder: -22000, pattern: null)
            .Add("StringResolver", interfaceType: typeof(ILineHint), sortingOrder: -201000, pattern: null)
            .Add("BinaryResolver", interfaceType: typeof(ILineHint), sortingOrder: -202000, pattern: null)
            as ParameterInfos;

        /// <summary>
        /// Default instance that contains info about well-known parameters.
        /// </summary>
        public static ParameterInfos Default => instance;

        string[] parameterNames;

        /// <summary>
        /// Get supported parameter names
        /// </summary>
        public string[] ParameterNames => parameterNames ?? (parameterNames = Values.Where(pi=>pi.InterfaceType!=null).Select(pi=>pi.ParameterName).ToArray());

        /// <summary>
        /// Associated parameter infos (this)
        /// </summary>
        IParameterInfos ILineFactoryParameterInfos.ParameterInfos { get => this; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Associated resolver (this)
        /// </summary>
        IResolver ILineFactoryResolver.Resolver { get => this; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Try to resolve parameter into arguments.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="resolvedLineArgument"></param>
        /// <returns></returns>
        public bool TryResolveParameter(ILine previous, string parameterName, string parameterValue, out ILineArgument resolvedLineArgument)
        {
            IParameterInfo pi;
            if (TryGetValue(parameterName, out pi) && pi.InterfaceType != null)
            {
                if (pi.InterfaceType == typeof(ILineParameter)) resolvedLineArgument = new LineArgument<ILineParameter, string, string>(parameterName, parameterValue);
                else if (pi.InterfaceType == typeof(ILineCanonicalKey)) resolvedLineArgument = new LineArgument<ILineCanonicalKey, string, string>(parameterName, parameterValue);
                else if (pi.InterfaceType == typeof(ILineNonCanonicalKey)) resolvedLineArgument = new LineArgument<ILineNonCanonicalKey, string, string>(parameterName, parameterValue);
                else if (pi.InterfaceType == typeof(ILineHint)) resolvedLineArgument = new LineArgument<ILineHint, string, string>(parameterName, parameterValue);
                else resolvedLineArgument = (ILineArgument) Activator.CreateInstance(typeof(LineArgument<,,>).MakeGenericType(new Type[] { pi.InterfaceType, typeof(string), typeof(string) }));
                return true;
            }
            resolvedLineArgument = default;
            return false;
        }

        IEnumerator<IParameterInfo> IEnumerable<IParameterInfo>.GetEnumerator() => Values.GetEnumerator();

        /// <summary>
        /// Create line part
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory factory, ILine previous, string parameterName, string parameterValue, out ILineParameter line)
        {
            IParameterInfo pi;
            if (TryGetValue(parameterName, out pi) && pi.InterfaceType != null)
            {
                if (pi.InterfaceType == typeof(ILineParameter)) { line = new LineParameter(this, previous, parameterName, parameterValue); return true; }
                else if (pi.InterfaceType == typeof(ILineCanonicalKey)) { line = new LineCanonicalKey(this, previous, parameterName, parameterValue); return true; }
                else if (pi.InterfaceType == typeof(ILineNonCanonicalKey)) { line = new LineNonCanonicalKey(this, previous, parameterName, parameterValue); return true; }
                else if (pi.InterfaceType == typeof(ILineHint)) { line = new LineHint(this, previous, parameterName, parameterValue); return true; }
            }
            line = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory factory, ILine previous, string parameterName, string parameterValue, out ILineHint line)
        {
            IParameterInfo pi;
            if (TryGetValue(parameterName, out pi) && pi.InterfaceType != null)
            {
                if (pi.InterfaceType == typeof(ILineHint)) { line = new LineHint(this, previous, parameterName, parameterValue); return true; }
            }
            line = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory factory, ILine previous, string parameterName, string parameterValue, out ILineNonCanonicalKey line)
        {
            IParameterInfo pi;
            if (TryGetValue(parameterName, out pi) && pi.InterfaceType != null)
            {
                if (pi.InterfaceType == typeof(ILineNonCanonicalKey)) { line = new LineNonCanonicalKey(this, previous, parameterName, parameterValue); return true; }
            }
            line = default;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory factory, ILine previous, string parameterName, string parameterValue, out ILineCanonicalKey line)
        {
            IParameterInfo pi;
            if (TryGetValue(parameterName, out pi) && pi.InterfaceType != null)
            {
                if (pi.InterfaceType == typeof(ILineCanonicalKey)) { line = new LineCanonicalKey(this, previous, parameterName, parameterValue); return true; }
            }
            line = default;
            return false;
        }
    }


}
