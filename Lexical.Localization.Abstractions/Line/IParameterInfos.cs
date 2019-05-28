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
    /// Collection of parameter infos
    /// </summary>
    public interface IParameterInfos
    {
    }

    /// <summary>
    /// Interface for reading parameters
    /// </summary>
    public interface IParameterInfosMap : IReadOnlyDictionary<string, IParameterInfo>
    {
    }

    /// <summary>
    /// Readable parameter info map.
    /// </summary>
    public interface IParameterInfosEnumerable : IParameterInfos, IEnumerable<IParameterInfo>
    {
    }

    /// <summary>
    /// Writable parameter infos
    /// </summary>
    public interface IParameterInfosWritable : IParameterInfos
    {
        /// <summary>
        /// Read parameter info
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IParameterInfo this[string key] { get; set; }
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
        /// Type of the parameter:
        /// <list type="bullet">
        ///     <item><see cref="ILineHint"/>not used with comparison.</item>
        ///     <item><see cref="ILineCanonicalKey"/>hash-equals comparable key (reoccuring)</item>
        ///     <item><see cref="ILineNonCanonicalKey"/>hash-equals comparable key</item>
        /// </list>
        /// </summary>
        Type InterfaceType { get; }

        /// <summary>
        /// Suggested sorting order. Smaller number is sorted to left, higher to right when formulating a string.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Default capture pattern for ILinePattern.
        /// </summary>
        Regex Pattern { get; }
    }

    /// <summary>
    /// Extension methods for parameter infos.
    /// </summary>
    public static class IParameterInfoExtensions
    {
        /// <summary>
        /// Try to read parameters
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parameterName"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool TryGetValue(this IParameterInfos infos, string parameterName, out IParameterInfo info)
        {
            if (infos is IParameterInfosMap readable) return readable.TryGetValue(parameterName, out info);
            info = null; return false;
        }

        /// <summary>
        /// Try to read parameters
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static IParameterInfo GetValue(this IParameterInfos infos, string parameterName)
        {
            IParameterInfo info;
            if (infos is IParameterInfosMap readable && readable.TryGetValue(parameterName, out info)) return info;
            throw new KeyNotFoundException(parameterName);
        }

        /// <summary>
        /// Add <paramref name="info"/> entry to <paramref name="infos"/>.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="info"></param>
        /// <returns><paramref name="infos"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IParameterInfos Add(this IParameterInfos infos, IParameterInfo info)
        {
            if (infos is IParameterInfosMap map && map.ContainsKey(info.ParameterName)) throw new ArgumentException($"Parameter {info.ParameterName} already exists", nameof(info));
            if (infos is IParameterInfosWritable writable) writable[info.ParameterName] = info; else throw new InvalidOperationException($"doesn't implement {typeof(IParameterInfosWritable)}");
            return infos;
        }

        /// <summary>
        /// Add info entry to <paramref name="infos"/>.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="parameterName"></param>
        /// <param name="interfaceType"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="pattern">(optional) capture pattern for <see cref="ILinePattern"/></param>
        /// <returns><paramref name="infos"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IParameterInfos Add(this IParameterInfos infos, string parameterName, Type interfaceType, int sortingOrder, Regex pattern)
        {
            if (infos is IParameterInfosMap map && map.ContainsKey(parameterName)) throw new ArgumentException($"Parameter {parameterName} already exists", nameof(parameterName));
            if (infos is IParameterInfosWritable writable) writable[parameterName] = new ParameterInfo(parameterName, interfaceType, sortingOrder, pattern); else throw new InvalidOperationException($"doesn't implement {typeof(IParameterInfosWritable)}");
            return infos;
        }

        /// <summary>
        /// Canonical parameters.
        /// </summary>
        public static IEnumerable<IParameterInfo> Canonicals(this IParameterInfos infos)
            => infos is IEnumerable<IParameterInfo> enumr ? enumr.Where(pi => pi.InterfaceType == typeof(ILineCanonicalKey)) : throw new InvalidOperationException($"not {nameof(IParameterInfosEnumerable)}");

        /// <summary>
        /// Non-canonical parameters.
        /// </summary>
        public static IEnumerable<IParameterInfo> NonCanonicals(this IParameterInfos infos)
            => infos is IEnumerable<IParameterInfo> enumr ? enumr.Where(pi => pi.InterfaceType == typeof(ILineNonCanonicalKey)) : throw new InvalidOperationException($"not {nameof(IParameterInfosEnumerable)}");

        /// <summary>
        /// Comparable parameters.
        /// </summary>
        public static IEnumerable<IParameterInfo> Comparables(this IParameterInfos infos)
            => infos is IEnumerable<IParameterInfo> enumr ? enumr.Where(pi => pi.InterfaceType == typeof(ILineCanonicalKey) || pi.InterfaceType == typeof(ILineNonCanonicalKey)) : throw new InvalidOperationException($"not {nameof(IParameterInfosEnumerable)}");

    }
}
