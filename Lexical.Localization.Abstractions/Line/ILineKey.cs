// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Clone <see cref="ILineKey"/> parts.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="appender">(optional) appender to use for clone. If null uses the appender of <see cref="line"/></param>
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
        /// Get part that implements <see cref="ILineCanonicalKey"/>, either this or preceding, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns><paramref name="part"/>, or preceding canonical part or null</returns>
        [Obsolete("Doesn't address ILineCanonicalKeyEnumerable")]
        public static ILineCanonicalKey GetCanonicalKey(this ILine part)
        {
            for (ILine p = part; p != null; p = p is ILine linkedKey ? linkedKey.GetPreviousPart() : null)
            {
                if (p is ILineCanonicalKey kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding part that implements <see cref="ILineCanonicalKey"/>, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>preceding canonical part or null</returns>
        [Obsolete("Doesn't address ILineCanonicalKeyEnumerable")]
        public static ILineCanonicalKey GetPreviousCanonicalKey(this ILine part)
        {
            for (ILine k = part is ILine lkk ? lkk.GetPreviousPart() : null; k != null; k = k is ILine nlkk ? nlkk.GetPreviousPart() : null)
            {
                if (k is ILineCanonicalKey kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get part that implements <see cref="ILineNonCanonicalKey"/>, either this or preceding one, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns><paramref name="part"/>, or preceding non-canonical part or null</returns>
        [Obsolete("Doesn't address ILineNonCanonicalKeyEnumerable")]
        public static ILineNonCanonicalKey GetNonCanonicalKey(this ILine part)
        {
            for (ILine k = part; k != null; k = k is ILine linkedKey ? linkedKey.GetPreviousPart() : null)
            {
                if (k is ILineNonCanonicalKey kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get preceding part that implements <see cref="ILineNonCanonicalKey"/>, or null if not found.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>preceding non-canonical part or null</returns>
        [Obsolete("Doesn't address ILineNonCanonicalKeyEnumerable")]
        public static ILineNonCanonicalKey GetPreviousNonCanonicalKey(this ILine part)
        {
            for (ILine k = part is ILine lkk ? lkk.GetPreviousPart() : null; k != null; k = k is ILine nlkk ? nlkk.GetPreviousPart() : null)
            {
                if (k is ILineNonCanonicalKey kk) return kk;
            }
            return null;
        }

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue dictionary.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>dictionary of keys</returns>
        public static IReadOnlyDictionary<string, string> GetNonCanonicalKeys(this ILine line)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineNonCanonicalKeyEnumerable lineParameters)
                    foreach (var k in lineParameters)
                        if (k.ParameterName != null && k.ParameterValue != null) result[k.ParameterName] = k.ParameterValue;
                if (l is ILineNonCanonicalKey key && key.ParameterName != null && key.ParameterValue != null) result[key.ParameterName] = key.ParameterValue;
            }
            return result;
        }
        static IReadOnlyDictionary<string, string> no_noncanonicalkeys = new Dictionary<string, string>(0);

        /// <summary>
        /// Get value of non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="parameterName"></param>
        /// <returns>value or null</returns>
        public static string GetNonCanonicalKey(this ILine line, string parameterName)
        {
            string result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineNonCanonicalKeyEnumerable lineParameters)
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == parameterName && parameter.ParameterValue != null) { result = parameter.ParameterValue; break; }
                if (l is ILineNonCanonicalKey lineParameter && lineParameter.ParameterName == parameterName && lineParameter.ParameterValue != null) result = lineParameter.ParameterValue;
            }
            return result;
        }

        /// <summary>
        /// Get all canonical keys as parameterName,parameterValue list from 
        /// </summary>
        /// <param name="line">line to read parameters of</param>
        /// <param name="parameterInfos">(optional) parameter infos, that have rules which parameters are considered as keys</param>
        /// <returns>dictionary of keys</returns>
        public static KeyValuePair<string, string>[] GetCanonicalKeys(this ILine line, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            StructList12<KeyValuePair<string, string>> result = new StructList12<KeyValuePair<string, string>>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                IParameterInfo info;
                if (parameterInfos != null && l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null && parameterInfos.TryGetValue(lineParameter.ParameterName, out info) && info.IsCanonical)
                    result.Add( new KeyValuePair<string, string>(lineParameter.ParameterName, lineParameter.ParameterValue) );
                else if (l is ILineCanonicalKey key && key.ParameterName != null && key.ParameterValue != null)
                    result.Add(new KeyValuePair<string, string>(key.ParameterName, key.ParameterValue));

                if (parameterInfos != null && l is ILineParameterEnumerable lineParameters)
                {
                    StructList8<KeyValuePair<string, string>> tmp = new StructList8<KeyValuePair<string, string>>();
                    foreach (var parameter in lineParameters)
                        if (parameter is ILineParameter p && p.ParameterName != null && p.ParameterValue != null && parameterInfos.TryGetValue(p.ParameterName, out info) && info.IsCanonical)
                            tmp.Add(new KeyValuePair<string, string>(p.ParameterName, p.ParameterValue));
                        else if (l is ILineCanonicalKey k && k.ParameterName != null && k.ParameterValue != null)
                            tmp.Add(new KeyValuePair<string, string>(k.ParameterName, k.ParameterValue));
                    // Copy tmp to result
                    for (int i = tmp.Count-1; i >= 0; i--) result.Add(tmp[i]);
                }
                else if (l is ILineCanonicalKeyEnumerable keys)
                {
                    StructList8<KeyValuePair<string, string>> tmp = new StructList8<KeyValuePair<string, string>>();
                    foreach (var k in keys)
                        if (k.ParameterName != null && k.ParameterValue != null)
                            tmp.Add(new KeyValuePair<string, string>(k.ParameterName, k.ParameterValue));
                    // Copy tmp to result
                    for (int i = tmp.Count - 1; i >= 0; i--) result.Add(tmp[i]);
                }
            }
            return result.ToReverseArray();
        }

        /// <summary>
        /// Get all non-canonical keys as parameterName,parameterValue list from 
        /// </summary>
        /// <param name="line">line to read parameters of</param>
        /// <param name="parameterInfos">(optional) parameter infos, that have rules which parameters are considered as keys</param>
        /// <returns>dictionary of keys</returns>
        public static KeyValuePair<string, string>[] GetNonCanonicalKeys(this ILine line, IReadOnlyDictionary<string, IParameterInfo> parameterInfos = null)
        {
            StructList12<KeyValuePair<string, string>> result = new StructList12<KeyValuePair<string, string>>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                IParameterInfo info;
                if (parameterInfos != null && l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null && parameterInfos.TryGetValue(lineParameter.ParameterName, out info) && info.IsNonCanonical)
                    result.Add(new KeyValuePair<string, string>(lineParameter.ParameterName, lineParameter.ParameterValue));
                else if (l is ILineNonCanonicalKey key && key.ParameterName != null && key.ParameterValue != null)
                    result.Add(new KeyValuePair<string, string>(key.ParameterName, key.ParameterValue));

                if (parameterInfos != null && l is ILineParameterEnumerable lineParameters)
                {
                    StructList8<KeyValuePair<string, string>> tmp = new StructList8<KeyValuePair<string, string>>();
                    foreach (var parameter in lineParameters)
                        if (parameter is ILineParameter p && p.ParameterName != null && p.ParameterValue != null && parameterInfos.TryGetValue(p.ParameterName, out info) && info.IsNonCanonical)
                            tmp.Add(new KeyValuePair<string, string>(p.ParameterName, p.ParameterValue));
                        else if (l is ILineNonCanonicalKey k && k.ParameterName != null && k.ParameterValue != null)
                            tmp.Add(new KeyValuePair<string, string>(k.ParameterName, k.ParameterValue));
                    // Copy tmp to result
                    for (int i = tmp.Count - 1; i >= 0; i--) result.Add(tmp[i]);
                }
                else if (l is ILineNonCanonicalKeyEnumerable keys)
                {
                    StructList8<KeyValuePair<string, string>> tmp = new StructList8<KeyValuePair<string, string>>();
                    foreach (var k in keys)
                        if (k.ParameterName != null && k.ParameterValue != null)
                            tmp.Add(new KeyValuePair<string, string>(k.ParameterName, k.ParameterValue));
                    // Copy tmp to result
                    for (int i = tmp.Count - 1; i >= 0; i--) result.Add(tmp[i]);
                }
            }
            return result.ToReverseArray();
        }
        static KeyValuePair<string, string>[] no_keys = new KeyValuePair<string, string>[0];

    }
}
