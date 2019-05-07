// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of assigning a new parameter.
    /// </summary>
    [Obsolete]
    public interface ILineParameterAssignable : ILine
    {
        /// <summary>
        /// Create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">parameter value</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If append failed</exception>
        ILineParameter AppendParameter(string parameterName, string parameterValue);
    }

    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// 
    /// Comparer may consider parameter as hash-equals comparable, if the 
    /// <see cref="ILineParameter.ParameterName"/> is key, such as "Culture".
    /// 
    /// If the parameter class implements <see cref="ILineNonCanonicalKey"/> or <see cref="ILineCanonicalKey"/> then
    /// the parameter will be considered as hash-equals comparable despite the value of the key.
    /// </summary>
    public interface ILineParameter : ILine
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        String ParameterName { get; }

        /// <summary>
        /// (optional) Parameter value.
        /// </summary>
        String ParameterValue { get; }
    }

    /// <summary>
    /// Interface for a line part that can enumerate parameters (from root towards tail).
    /// 
    /// Comparer may consider parameter as hash-equals comparable, if the 
    /// <see cref="ILineParameter.ParameterName"/> is key, such as "Culture".
    /// 
    /// If the parameter class implements <see cref="ILineNonCanonicalKey"/> or <see cref="ILineCanonicalKey"/> then
    /// the parameter will be considered as hash-equals comparable despite the <see cref="ILineParameter.ParameterName"/> of the key.
    /// 
    /// The enumerable must keys as well as parameters.
    /// </summary>
    public interface ILineParameterEnumerable : ILine, IEnumerable<ILineParameter>
    {
    }

    /// <summary>
    /// Function that visits parameters from root towards tail.
    /// </summary>
    /// <typeparam name="T">type for the visitor to choose</typeparam>
    /// <param name="parameter"></param>
    /// <param name="data"></param>
    public delegate void ParameterVisitor<T>(ILineParameter parameter, ref T data);

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(optional) parameter value</param>
        /// <returns>new parameter part</returns>
        /// <exception cref="LineException">If part could not be appended</exception>
        /// <returns>new part</returns>
        public static ILine Parameter(this ILine part, string parameterName, string parameterValue)
            => part.GetAppender().Create<ILineParameter, string, string>(part, parameterName, parameterValue);

        /// <summary>
        /// Append new parameter part as <see cref="ILineParameter"/>, as <see cref="ILineCanonicalKey"/>, or as <see cref="ILineNonCanonicalKey"/> depending
        /// on parameter name and policy in <paramref name="parameterInfos"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">parameter value</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new parameter part</returns>
        /// <exception cref="LineException">If part could not be appended</exception>
        /// <returns>new part</returns>
        public static ILine Parameter(this ILine part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical) return part.GetAppender().Create<ILineCanonicalKey, string, string>(part, parameterName, parameterValue);
                else if (info.IsNonCanonical) return part.GetAppender().Create<ILineNonCanonicalKey, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().Create<ILineParameter, string, string>(part, parameterName, parameterValue);
        }

        /// <summary>
        /// Try to create a new key by appending an another key with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <param name="line"></param>
        /// <returns>true if was appended to <paramref name="line"/></returns>
        public static bool TryAppendParameter(this ILine part, string parameterName, string parameterValue, out ILine line)
        {
            ILineParameter result = null;
            bool ok = part.GetAppender().TryCreate<ILineParameter, string, string>(part, parameterName, parameterValue, out result);
            line = result;
            return ok;
        }

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <param name="line"></param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static bool TryAppendParameter(this ILine part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos, out ILine line)
        {
            // Get appender 
            ILineFactory appender;
            if (!part.TryGetAppender(out appender)) { line = default; return false; }

            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical)
                {
                    ILineCanonicalKey result = null;
                    if (part.GetAppender().TryCreate<ILineCanonicalKey, string, string>(part, parameterName, parameterValue, out result))
                    {
                        line = result;
                        return true;
                    }
                }
                else if (info.IsNonCanonical)
                {
                    ILineNonCanonicalKey result = null;
                    if (part.GetAppender().TryCreate<ILineNonCanonicalKey, string, string>(part, parameterName, parameterValue, out result))
                    {
                        line = result;
                        return true;
                    }
                }
            }
            {
                ILineParameter result = null;
                if (part.GetAppender().TryCreate<ILineParameter, string, string>(part, parameterName, parameterValue, out result))
                {
                    line = result;
                    return true;
                }
            }
            line = default;
            return false;
        }

        /// <summary>
        /// Append enumeration of parameters.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static ILine Parameters(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            ILineFactory appender = part.GetAppender();
            if (appender == null) throw new LineException(part, "Appender is not found.");
            foreach (var parameter in parameters)
                part = appender.Create<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            return part;
        }

        /// <summary>
        /// Append enumeration of parameters and keys.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static ILine Parameters(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            ILineFactory appender = part.GetAppender();
            foreach (var parameter in parameters)
            {
                if (parameter.Key == null) continue;
                IParameterInfo info = null;
                if (parameterInfos != null && parameterInfos.TryGetValue(parameter.Key, out info) && (info.IsCanonical || info.IsNonCanonical))
                {
                    if (info.IsCanonical) part = appender.Create<ILineCanonicalKey, string, string>(part, parameter.Key, parameter.Value);
                    else if (info.IsNonCanonical) part = appender.Create<ILineNonCanonicalKey, string, string>(part, parameter.Key, parameter.Value);
                }
                else part = appender.Create<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            }
            return part;
        }

        /// <summary>
        /// Try to create a new key by appending an enumeration of parameters.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters"></param>
        /// <param name="parameterInfos">(optional) rules whether to create <see cref="ILineCanonicalKey"/>, <see cref="ILineNonCanonicalKey"/>, or <see cref="ILineParameter"/>. If null everything is instantiated as <see cref="ILineParameter"/></param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static bool TryAppendParameters(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters, IReadOnlyDictionary<string, IParameterInfo> parameterInfos, out ILine line)
        {
            ILine result = part;
            foreach(var parameter in parameters)
                if (!result.TryAppendParameter(parameter.Key, parameter.Value, parameterInfos, out result))
                    { line = null; return false; }
            line = result;
            return true;
        }

        /// <summary>
        /// Get effective parameter value (closest to root).
        /// </summary>
        /// <param name="line"></param>
        /// <param name="parameterName"></param>
        /// <returns>parameter or null</returns>
        public static string GetParameter(this ILine line, string parameterName)
        {
            string result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName == parameterName && lineParameter.ParameterValue != null) result = lineParameter.ParameterValue;
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == parameterName && parameter.ParameterValue != null) { result = parameter.ParameterValue; break; }
            }
            return result;
        }

        /// <summary>
        /// Get parameter name of the part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>name or null</returns>
        public static string GetParameterName(this ILine key)
            => key is ILineParameter parametrized ? parametrized.ParameterName : null;

        /// <summary>
        /// Get parameter name of the part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>name or null</returns>
        public static string GetParameterValue(this ILine key)
            => key is ILineParameter parametrized ? parametrized.ParameterValue : null;

        /// <summary>
        /// Get the number of parameters.
        /// </summary>
        /// <param name="line">(optional) line to count parameter count</param>
        /// <returns>number of parameters</returns>
        public static int GetParameterCount(this ILine line)
        {
            int result = 0;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null) result++;
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName != null && parameter.ParameterValue != null) result++;
            }
            return result;
        }

        /// <summary>
        /// Find the preceding part key in the linked list by <paramref name="parameterName"/>.
        /// Starts search from the tail and goes toward root.
        /// </summary>
        /// <param name="line">(optional)</param>
        /// <param name="parameterName">(optional)</param>
        /// <returns><see cref="ILineParameter"/>, <see cref="ILineParameterEnumerable"/> or null</returns>
        public static ILine GetParameterPart(this ILine line, string parameterName)
        {
            if (line == null || parameterName == null) return null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName == parameterName) return lineParameter;
                if (l is ILineParameterEnumerable lineParameters)
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == parameterName) return l;
            }
            return null;
        }

        /// <summary>
        /// Get preceding non-null parameter part.
        /// </summary>
        /// <param name="line"></param>
        /// <returns><see cref="ILineParameter"/>, <see cref="ILineParameterEnumerable"/> or null</returns>
        public static ILine GetPreviousParameterPart(this ILine line)
        {
            if (line == null) return null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null) return lineParameter;
                if (l is ILineParameterEnumerable lineParameters && lineParameters.FirstOrDefault() != null)
                    return l;
            }
            return null;
        }

        /// <summary>
        /// Get all parameters as parameterName,parameterValue as array with value from root to tail.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>array of parameters</returns>
        public static ILineParameter[] GetParameters(this ILine line)
        {
            int count = line.GetParameterCount();
            if (count == 0) return no_parameters;
            ILineParameter[] result = new ILineParameter[count];
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null) result[--count] = lineParameter;
                if (l is ILineParameterEnumerable lineParameters)
                {
                    int endIx = count;
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName != null && parameter.ParameterValue != null) result[--count] = parameter;

                    // Reverse between count .. endIx
                    int mid = count+ ((endIx-count)/2);
                    for (int i = count, j = endIx-1; i < mid; i++, j--)
                    {
                        // Swap result[i] and result[j]
                        ILineParameter tmp = result[i];
                        result[i] = result[j];
                        result[j] = tmp;
                    }
                }
            }
            return result;
        }
        static ILineParameter[] no_parameters = new ILineParameter[0];

        /// <summary>
        /// Get all parameters as parameterName,parameterValue as array with value from root to tail.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>array of parameters</returns>
        public static KeyValuePair<string, string>[] GetParameterAsKeyValues(this ILine line)
        {
            int count = line.GetParameterCount();
            if (count == 0) return no_parameter_keyvalues;
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null)
                    result[--count] = new KeyValuePair<string, string>(lineParameter.ParameterName, lineParameter.ParameterValue);

                if (l is ILineParameterEnumerable lineParameters)
                {
                    int endIx = count;
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName != null && parameter.ParameterValue != null)
                            result[--count] = new KeyValuePair<string, string>(parameter.ParameterName, parameter.ParameterValue);

                    // Reverse between count .. endIx
                    int mid = count + ((endIx - count) / 2);
                    for (int i = count, j = endIx - 1; i < mid; i++, j--)
                    {
                        // Swap result[i] and result[j]
                        KeyValuePair<string, string> tmp = result[i];
                        result[i] = result[j];
                        result[j] = tmp;
                    }
                }
            }

            return result;
        }
        static KeyValuePair<string, string>[] no_parameter_keyvalues = new KeyValuePair<string, string>[0];

        /// <summary>
        /// Visit parameter parts from root towards key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        public static void VisitParameters<T>(this ILine line, ParameterVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILine prevKey = line.GetPreviousPart();
            if (prevKey != null) VisitParameters(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            if (line is ILineParameterEnumerable enumr)
                foreach(var parameter_ in enumr)
                    if (parameter_.ParameterName != null && parameter_.ParameterValue != null)
                        visitor(parameter_, ref data);
            if (line is ILineParameter parameter && parameter.ParameterName!=null && parameter.ParameterValue != null)
                visitor(parameter, ref data);
        }

    }
}

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// <see cref="ILine"/> extension methods with internal class dependencies.
    /// </summary>
    public static partial class ILineParameterPartExtensions
    {
        /// <summary>
        /// Break <paramref name="part"/> into effective parameters and write to <paramref name="list"/>.
        /// The <paramref name="list"/> is allocated from stack by caller.
        /// 
        /// For non-canonical parameters, only the left-most is added, with occurance index 0.
        /// For canonical parameters, the left most occurance starts with index 0, and increments for every new occurance.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="list">(ParameterName, occuranceIndex, ParameterValue)</param>
        public static void GetEffectiveParameters(this ILine part, ref StructList12<(string, int, string)> list)
        {
            for (ILine k = part; k != null; k = k.GetPreviousPart())
            {
                // Parameter
                ILineParameter parametrized = k as ILineParameter;
                if (parametrized == null) continue;
                string parameterName = parametrized.ParameterName, parameterValue = parametrized.ParameterValue;
                if (parameterName == null) continue;

                // Canonical/Non-canonical
                bool isCanonical = k is ILineCanonicalKey, isNonCanonical = k is ILineNonCanonicalKey;
                if (!isCanonical && !isNonCanonical) continue;

                if (isNonCanonical)
                {
                    // Test if parameter is already in list
                    int ix = -1;
                    for (int i = 0; i < list.Count; i++)
                        if (list[i].Item1 == parameterName)
                        {
                            // Overwrite
                            list[i] = (parameterName, 0, parameterValue);
                            ix = i;
                            break;
                        }
                    // Add new
                    if (ix == -1)
                    {
                        list.Add((parameterName, 0, parameterValue));
                    }
                    continue;
                }

                if (isCanonical)
                {
                    // Add to list, fix occurance index later
                    list.Add((parameterName, -1, parameterValue));
                }
            }

            // Fix occurance indices
            for (int i = 0; i < list.Count; i++)
            {
                (string parameterName, int occurance, string parameterValue) = list[i];
                if (occurance >= 0) continue;
                int oix = 0;
                for (int j = i - 1; j >= 0; j--)
                {
                    (string parameterName_, int occurance_, string _) = list[j];
                    if (parameterName_ == parameterName) { oix = occurance_ + 1; break; }
                }
                list[i] = (parameterName, oix, parameterValue);
            }

        }

    }
}