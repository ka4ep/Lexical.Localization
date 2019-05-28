// --------------------------------------------------------
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
        /// Append "Key" non-canonical key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Key(this ILine line, string key)
            => line.Append<ILineCanonicalKey, string, string>("Key", key);

        /// <summary>
        /// Append "Section" key.
        /// 
        /// Section is a key that points to a folder is used when loading assets from files, embedded resources, and withint language string dictionaries.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Section(this ILine line, string location)
            => line.Append<ILineCanonicalKey, string, string>("Section", location);

        /// <summary>
        /// Append "Location" key.
        /// 
        /// Location is a key that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Location(this ILine line, string location)
            => line.Append<ILineCanonicalKey, string, string>("Location", location);


        /// <summary>
        /// Append "BaseName" key.
        /// 
        /// BaseName means a part of a path to assembly's embedded resource.
        /// For instance, resource hint matches in name pattern "[Assembly.][BaseName.]{Type.}{Section.}{Key}".
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey BaseName(this ILine line, string resource)
            => line.Append<ILineCanonicalKey, string, string>("BaseName", resource);

        /// <summary>
        /// Append "Key" non-canonical key.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="LineException"></exception>
        public static ILine Key(this ILineFactory lineFactory, string key)
            => lineFactory.Create<ILineCanonicalKey, string, string>(null, "Key", key);

        /// <summary>
        /// Create "Section" key.
        /// 
        /// Section is a key that points to a folder is used when loading assets from files, embedded resources, and withint language string dictionaries.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Section(this ILineFactory lineFactory, string location)
            => lineFactory.Create<ILineCanonicalKey, string, string>(null, "Section", location);

        /// <summary>
        /// Create "Location" key.
        /// 
        /// Location is a key that points to folder where asset is to be loaded.
        /// For example adding "Icons" location section, would mean that when key is matched to file assets, only "Icons" folder is used.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="location"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey Location(this ILineFactory lineFactory, string location)
            => lineFactory.Create<ILineCanonicalKey, string, string>(null, "Location", location);

        /// <summary>
        /// Create "BaseName" key.
        /// 
        /// BaseName means a part of a path to assembly's embedded resource.
        /// For instance, resource hint matches in name pattern "[Assembly.][BaseName.]{Type.}{Section.}{Key}".
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key could not be appended</exception>
        public static ILineCanonicalKey BaseName(this ILineFactory lineFactory, string resource)
            => lineFactory.Create<ILineCanonicalKey, string, string>(null, "BaseName", resource);

        /// <summary>
        /// Tests if <paramref name="linePart"/> is canonical key.
        /// </summary>
        /// <param name="linePart"></param>
        /// <param name="parameterInfos">(optional) If provided, then is used to evaluate <see cref="ILineParameter"/></param>
        /// <returns></returns>
        public static bool IsCanonicalKey(this ILine linePart, IParameterInfos parameterInfos = null)
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
        public static bool IsNonCanonicalKey(this ILine linePart, IParameterInfos parameterInfos = null)
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
        public static string GetNonCanonicalKey(this ILine line, string parameterName, IParameterInfos parameterInfos = null)
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
        public static IReadOnlyDictionary<string, string> GetNonCanonicalKeys(this ILine line, IParameterInfos parameterInfos = null)
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
        public static void GetCanonicalKeys<LIST>(this ILine line, ref LIST list, IParameterInfos parameterInfos = null) where LIST : IList<ILineParameter>
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
                if (l is ILineParameter lineParameter && l.IsCanonicalKey(parameterInfos))
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
        public static void GetCanonicalKeyPairs<LIST>(this ILine line, ref LIST list, IParameterInfos parameterInfos = null) where LIST : IList<KeyValuePair<string, string>>
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
                if (l is ILineParameter lineParameter && l.IsCanonicalKey(parameterInfos))
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
        public static void GetNonCanonicalKeyPairs<LIST>(this ILine line, ref LIST result, IParameterInfos parameterInfos = null) where LIST : IList<KeyValuePair<string, string>>
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
                            if (ix >= 0)
                            {
                                result[ix] = new KeyValuePair<string, string>(name, value);
                                break;
                            }
                            else result.Add(new KeyValuePair<string, string>(name, value));
                        }
                    }
                }
                if (l is ILineParameter lineParameter && l.IsNonCanonicalKey(parameterInfos))
                {
                    string name = lineParameter.ParameterName, value = lineParameter.ParameterValue;
                    if (lineParameter.IsNonCanonicalKey(parameterInfos) && name != null && value != null)
                    {
                        int ix = -1;
                        for (int i = 0; i < result.Count; i++) if (result[i].Key == name) { ix = i; break; }
                        if (ix >= 0)
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
        public static KeyValuePair<string, string>[] GetNonCanonicalsArray(this ILine line, IParameterInfos parameterInfos = null)
        {
            StructList12<KeyValuePair<string, string>> result = new StructList12<KeyValuePair<string, string>>();
            line.GetNonCanonicalKeyPairs<StructList12<KeyValuePair<string, string>>>(ref result, parameterInfos);
            return result.ToReverseArray();

        }
        static KeyValuePair<string, string>[] no_keys = new KeyValuePair<string, string>[0];

    }
}
