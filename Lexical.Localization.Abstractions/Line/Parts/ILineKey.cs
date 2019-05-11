﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the keypart is used as a key, and is hash-equals comparable
    /// </summary>
    public interface ILineKey : ILineParameter
    {
    }

    /// <summary>
    /// Signals that the parameter is a key (hash-equals compared) in a way that
    /// the occurance position of the key is irrelevant (non-canonical).
    /// 
    /// However, as a rule, if key by same name occurs more than once (with non-null value),
    /// then only the key that is closest to the root is effective.
    /// </summary>
    public interface ILineNonCanonicalKey : ILineKey
    {
    }

    /// <summary>
    /// Signals that the parameter is a key (hash-equals compared) in a way that
    /// the occurance position of the key is relevant (canonical).
    /// 
    /// However, as a rule, if key by same name occurs more than once (with non-null value),
    /// then only the key that is closest to the root is effective.
    /// </summary>
    public interface ILineCanonicalKey : ILineKey
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Tests if <paramref name="linePart"/> is canonical key.
        /// </summary>
        /// <param name="linePart"></param>
        /// <param name="parameterInfos">(optional) If provided, then is used to evaluate <see cref="ILineParameter"/></param>
        /// <returns></returns>
        public static bool IsCanonicalKey(this ILine linePart, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            // Assert arguments
            if (linePart == null) return false;
            // Implements ILineCanonicalKey
            if (linePart is ILineCanonicalKey) return true;
            // Doesn't implement ILineHint and ILineCanonicalKey, and the ParameterName is ILineNonCanonicalKey in the dictionary
            IParameterInfo info;
            return parameterInfos != null && linePart is ILineParameter lineParameter && (linePart is ILineHint == false && linePart is ILineNonCanonicalKey == false) && parameterInfos.TryGetValue(lineParameter.ParameterName, out info) && info.InterfaceType == typeof(ILineCanonicalKey);
        }

        /// <summary>
        /// Tests if <paramref name="linePart"/> is canonical key.
        /// </summary>
        /// <param name="linePart"></param>
        /// <param name="parameterInfos">(optional) If provided, then is used to evaluate <see cref="ILineParameter"/></param>
        /// <returns></returns>
        public static bool IsNonCanonicalKey(this ILine linePart, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            // Assert arguments
            if (linePart == null) return false;
            // Implements ILineNonCanonicalKey
            if (linePart is ILineNonCanonicalKey) return true;
            // Doesn't implement ILineHint and ILineCanonicalKey, and the ParameterName is ILineNonCanonicalKey in the dictionary
            IParameterInfo info;
            return parameterInfos != null && linePart is ILineParameter lineParameter && (linePart is ILineHint == false && linePart is ILineCanonicalKey == false) && parameterInfos.TryGetValue(lineParameter.ParameterName, out info) && info.InterfaceType == typeof(ILineNonCanonicalKey);
        }

        /// <summary>
        /// Clone <see cref="ILineKey"/> parts.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="appender">(optional) appender to use for clone. If null uses the appender of <paramref name="line"/></param>
        /// <returns>clone of key parts</returns>
        /// <exception cref="LineException">If cloning failed.</exception>
        public static ILine CloneKey(this ILine line, ILineFactory appender = default)
        {
            if (appender == null) appender = line.GetAppender();
            ILine result = null;

            StructList16<ILine> args = new StructList16<ILine>();
            for (ILine l = line; l != null; l = l.GetPreviousPart()) if (l is ILineArguments || l is ILineArgumentsEnumerable) args.Add(l);

            for (int i = args.Count - 1; i >= 0; i--)
            {
                ILine l = args[i];
                if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                        if (lineParameter is ILineKey && lineParameter is ILineArguments argsi) result = appender.Create(result, argsi);
                }
                if (l is ILineKey && l is ILineArguments arg) result = appender.Create(result, arg);
            }
            return result ?? appender.Create<ILinePart>(null);
        }

        /// <summary>
        /// Get effective value of non-canonical key by <paramref name="parameterName"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <returns>value or null</returns>
        public static string GetNonCanonicalKey(this ILine line, string parameterName, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            string result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (var parameter in lineParameters)
                        if (parameter.IsNonCanonicalKey(parameterInfos)) { result = parameter.ParameterValue; break; }
                if (l.IsNonCanonicalKey(parameterInfos) && l is ILineParameter lineParameter && lineParameter.ParameterName == parameterName && lineParameter.ParameterValue != null) result = lineParameter.ParameterValue;
            }
            return result;
        }

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue dictionary.
        /// </summary>
        /// <param name="line">line to read parameters of</param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <returns>dictionary of keys</returns>
        public static IReadOnlyDictionary<string, string> GetNonCanonicalKeys(this ILine line, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (var k in lineParameters)
                        if (k.IsNonCanonicalKey(parameterInfos)) { result[k.ParameterName] = k.ParameterValue; break; }
                if (l is ILineParameter key && l.IsNonCanonicalKey(parameterInfos) && key.ParameterName != null && key.ParameterValue != null) result[key.ParameterName] = key.ParameterValue;
            }
            return result;
        }
        static IReadOnlyDictionary<string, string> no_noncanonicalkeys = new Dictionary<string, string>(0);

        /// <summary>
        /// Get all canonical keys from tail towards root.
        /// </summary>
        /// <param name="line">line to read keys from</param>
        /// <param name="list">result list to write results to</param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <typeparam name="LIST">List type</typeparam>
        /// <returns>dictionary of keys</returns>
        public static void GetCanonicalKeys<LIST>(this ILine line, ref LIST list, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null) where LIST : IList<ILineParameter>
        {
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                {
                    StructList4<ILineParameter> tmp = new StructList4<ILineParameter>();
                    foreach (var parameter in lineParameters)
                    {
                        if (parameter.IsCanonicalKey(parameterInfos) && parameter.ParameterName != null && parameter.ParameterValue != null) tmp.Add(parameter);
                    }
                    for (int i = tmp.Count - 1; i >= 0; i--) list.Add(tmp[i]);
                }
                else if (l is ILineParameter lineParameter && l.IsCanonicalKey(parameterInfos))
                {
                    if (lineParameter.IsCanonicalKey(parameterInfos) && lineParameter.ParameterName != null && lineParameter.ParameterValue != null)
                        list.Add(lineParameter);
                }
            }
        }

        /// <summary>
        /// Get all canonical keys as parameterName,parameterValue from tail towards root.
        /// </summary>
        /// <param name="line">line to read keys from</param>
        /// <param name="list">result list to write results to</param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <typeparam name="LIST">List type</typeparam>
        /// <returns>dictionary of keys</returns>
        public static void GetCanonicalKeyPairs<LIST>(this ILine line, ref LIST list, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null) where LIST : IList<KeyValuePair<string, string>>
        {
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                {
                    StructList4<KeyValuePair<string, string>> tmp = new StructList4<KeyValuePair<string, string>>();
                    foreach (var parameter in lineParameters)
                    {
                        string name = parameter.ParameterName, value = parameter.ParameterValue;
                        if (parameter.IsCanonicalKey(parameterInfos) && name != null && value != null) tmp.Add(new KeyValuePair<string, string>(name, value));
                    }
                    for (int i = tmp.Count - 1; i >= 0; i--) list.Add(tmp[i]);
                }
                else if (l is ILineParameter lineParameter && l.IsCanonicalKey(parameterInfos))
                {
                    string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;
                    if (lineParameter.IsCanonicalKey(parameterInfos) && name != null && value != null)
                        list.Add(new KeyValuePair<string, string>(name, value));
                }
            }
        }

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue. Values that are closer to root are effective
        /// </summary>
        /// <param name="line">line to read keys from</param>
        /// <param name="result">result list to write results to</param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <typeparam name="LIST">List type</typeparam>
        /// <returns>dictionary of keys</returns>
        public static void GetNonCanonicalKeyPairs<LIST>(this ILine line, ref LIST result, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null) where LIST : IList<KeyValuePair<string, string>>
        {
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                    {
                        string name = parameter.ParameterName, value = parameter.ParameterValue;
                        if (parameter.IsNonCanonicalKey(parameterInfos) && name != null && value != null)
                        {
                            int ix = -1;
                            for (int i = 0; i < result.Count; i++) if (result[i].Key == name) { ix = i; break; }
                            if (ix < 0)
                            {
                                result[ix] = new KeyValuePair<string, string>(name, value);
                                break;
                            }
                            else result.Add(new KeyValuePair<string, string>(name, value));
                        }
                    }
                }
                else if (l is ILineParameter lineParameter && l.IsCanonicalKey(parameterInfos))
                {
                    string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;
                    if (lineParameter.IsNonCanonicalKey(parameterInfos) && name != null && value != null)
                    {
                        int ix = -1;
                        for (int i = 0; i < result.Count; i++) if (result[i].Key == name) { ix = i; break; }
                        if (ix < 0)
                        {
                            result[ix] = new KeyValuePair<string, string>(name, value);
                            break;
                        }
                        else result.Add(new KeyValuePair<string, string>(name, value));
                    }
                }
            }
        }

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue in order of from root towards tail.
        /// </summary>
        /// <param name="line">line to read parameters of</param>
        /// <param name="parameterInfos">(optional) map of infos for determining if parameter is key</param>
        /// <returns>dictionary of keys</returns>
        public static KeyValuePair<string, string>[] GetNonCanonicalsArray(this ILine line, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            StructList12<KeyValuePair<string, string>> result = new StructList12<KeyValuePair<string, string>>();
            line.GetNonCanonicalKeyPairs<StructList12<KeyValuePair<string, string>>>(ref result, parameterInfos);
            return result.ToReverseArray();

        }
        static KeyValuePair<string, string>[] no_keys = new KeyValuePair<string, string>[0];

    }
}
