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
    public interface ILineParameterAssignable : ILinePart
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
    /// Interface for a line that can enumerate parameters (from root towards tail).
    /// 
    /// Parameter is may or may not be a key. Key is hash-equals comparable part.
    /// If parameter part is not a <see cref="ILineKey"/>, then the parameter is a hint and not used for comparisons.
    /// </summary>
    public interface ILineParameters : ILine
    {
        /// <summary>
        /// Parameters of the line, this includes <see cref="ILineKeysCanonicallyCompared"/> and <see cref="ILineKeysNonCanonicallyCompared"/>.
        /// 
        /// The preferred implementation is of <see cref="IList{T}"/>.
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }
    }

    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// 
    /// Parameter is may or may not be a key. Key is hash-equals comparable part.
    /// If parameter part is not a <see cref="ILineKey"/>, then the parameter is a hint and not used for comparisons.
    /// </summary>
    public interface ILineParameter : ILinePart
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
    /// Function that visits one parametrized key part.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    /// <param name="data"></param>
    public delegate void KeyParameterVisitor<T>(string parameterName, string parameterValue, ref T data);

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Get parameter name of the part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>name or null</returns>
        public static string GetParameterName(this ILinePart key)
            => key is ILineParameter parametrized ? parametrized.ParameterName : null;

        /// <summary>
        /// Get parameter name of the part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>name or null</returns>
        public static string GetParameterValue(this ILinePart key)
            => key is ILineParameter parametrized ? parametrized.ParameterValue : null;

        /// <summary>
        /// Get the number of parameters.
        /// </summary>
        /// <param name="line">(optional) line to count parameter count</param>
        /// <returns>number of parameters</returns>
        public static int GetParameterCount(this ILine line)
        {
            if (line is ILineParameters lineParameters) return lineParameters.Parameters.Count();

            // Count parts
            if (line is ILinePart part)
            {
                int count = 0;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p.GetParameterName() != null) count++;
                return count;
            }

            return 0;
        }

        /// <summary>
        /// Find key in the linked list by <paramref name="parameterName"/>.
        /// Starts search from the tail and goes toward root.
        /// </summary>
        /// <param name="tail">(optional)</param>
        /// <param name="parameterName">(optional)</param>
        /// <returns>key or null</returns>
        public static ILineParameter GetParameterPart(this ILinePart tail, string parameterName)
        {
            if (tail == null || parameterName == null) return null;
            for (ILinePart p = tail; p != null; p = p.PreviousPart)
                if (p is ILineParameter part && part.ParameterName == parameterName) return part;
            return null;
        }

        /// <summary>
        /// Get previous valid parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>key or null</returns>
        public static ILineParameter GetPreviousParameterPart(this ILinePart part)
        {
            if (part == null) return null;
            for (ILinePart k = part.PreviousPart; k != null; k = k.PreviousPart)
                if (k is ILineParameter parameterAssigned)
                    if (!string.IsNullOrEmpty(parameterAssigned.ParameterName))
                        return parameterAssigned;
            return null;
        }

        /// <summary>
        /// Get all parameters as parameterName,parameterValue array.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>array of parameters</returns>
        public static IList<KeyValuePair<string, string>> GetParameters(this ILine line)
        {
            if (line is ILineParameters lineParameters)
            {
                var enumr = lineParameters.Parameters;
                return enumr is IList<KeyValuePair<string, string>> list ? list : enumr.ToList();
            }

            if (line is ILinePart part)
            {
                int count = 0;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineParameter parameter && parameter.ParameterName != null && parameter.ParameterValue != null) count++;

                if (count == 0) return no_parameters;

                KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
                int ix = count;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineParameter parameter && parameter.ParameterName != null && parameter.ParameterValue != null)
                        result[--ix] = new KeyValuePair<string, string>(parameter.ParameterName, parameter.ParameterValue);

                return result;
            }

            return no_parameters;
        }
        static KeyValuePair<string, string>[] no_parameters = new KeyValuePair<string, string>[0];

        /// <summary>
        /// Visit parameter parts from root towards key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="visitor"></param>
        /// <param name="data"></param>
        public static void VisitParameters<T>(this ILinePart key, KeyParameterVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILinePart prevKey = key.PreviousPart;
            if (prevKey != null) VisitParameters(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            if (key is ILineParameter parameter && parameter.ParameterName!=null) visitor(parameter.ParameterName, parameter.ParameterValue, ref data);
        }

        /// <summary>
        /// Append new parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(optional) parameter value</param>
        /// <returns>new parameter part</returns>
        /// <exception cref="LineException">If part could not be appended</exception>
        /// <returns>new part</returns>
        public static ILinePart Parameter(this ILinePart part, string parameterName, string parameterValue)
            => part.GetAppender().Append<ILineParameter, string, string>(part, parameterName, parameterValue);

        /// <summary>
        /// Append new parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(optional) parameter value</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new parameter part</returns>
        /// <exception cref="LineException">If part could not be appended</exception>
        /// <returns>new part</returns>
        public static ILinePart Parameter(this ILinePart part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical) return part.GetAppender().Append<ILineKeyCanonicallyCompared, string, string>(part, parameterName, parameterValue);
                else if (info.IsNonCanonical) return part.GetAppender().Append<ILineKeyNonCanonicallyCompared, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().Append<ILineParameter, string, string>(part, parameterName, parameterValue);
        }

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILinePart TryAppendParameter(this ILinePart part, string parameterName, string parameterValue)
            => part.GetAppender().TryAppend<ILineParameter, string, string>(part, parameterName, parameterValue);

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILinePart TryAppendParameter(this ILinePart part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical) return part.GetAppender().TryAppend<ILineKeyCanonicallyCompared, string, string>(part, parameterName, parameterValue);
                else if (info.IsNonCanonical) return part.GetAppender().TryAppend<ILineKeyNonCanonicallyCompared, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().TryAppend<ILineParameter, string, string>(part, parameterName, parameterValue);

        }

        /// <summary>
        /// Appending an enumeration of parameters.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static ILinePart Parameters(this ILinePart part, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            ILinePartAppender appender = part.GetAppender();
            if (appender == null) throw new LineException(part, "Appender is not found.");
            foreach (var parameter in parameters)
                part = appender.Append<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            return part;
        }

        /// <summary>
        /// Appending an enumeration of parameters and keys.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static ILinePart Parameter(this ILinePart part, IEnumerable<KeyValuePair<string, string>> parameters, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            ILinePartAppender appender = part.GetAppender();
            if (appender == null) throw new LineException(part, "Appender is not found.");
            foreach (var parameter in parameters)
            {
                if (parameter.Key == null) continue;
                IParameterInfo info = null;
                if (parameterInfos!=null && parameterInfos.TryGetValue(parameter.Key, out info) && (info.IsCanonical||info.IsNonCanonical))
                {
                    if (info.IsCanonical) part = appender.Append<ILineKeyCanonicallyCompared, string, string>(part, parameter.Key, parameter.Value);
                    else if (info.IsNonCanonical) part = appender.Append<ILineKeyNonCanonicallyCompared, string, string>(part, parameter.Key, parameter.Value);
                } else part = appender.Append<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            }
            return part;
        }

        /// <summary>
        /// Try to create a new key by appending an enumeration of parameters.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters"></param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILinePart TryAppendParameters(this ILinePart part, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            ILinePartAppender appender = part.GetAppender();
            if (appender == null) return null;
            foreach (var parameter in parameters)
            {
                if (part == null) return null;
                part = appender.TryAppend<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            }
            return part;
        }

        /// <summary>
        /// Create a new key by appending an enumeration of parameters.
        /// 
        /// If non-canonical parameter is already in the <paramref name="left"/>, then its not appended.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right">enumeration of parameters to append</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable, or append failed</exception>
        public static ILinePart ConcatIfNew(this ILinePart left, ILinePart right)
        {
            if (right == null) return left;
            if (left == null) return right;
            ILinePart result = left;
            foreach (ILinePart k in right.ArrayFromRoot(includeNonCanonical: true))
            {
                if (k is ILineParameter parameter)
                {
                    string parameterName = parameter.ParameterName, parameterValue = k.GetParameterValue();
                    if (string.IsNullOrEmpty(parameterName) || parameterValue == null) continue;

                    // Check if parameterName of k already exists in "result"/"left".
                    if (k is ILineKeyNonCanonicallyCompared && left.GetParameterPart(parameterName) != null) continue;

                    result = result.Parameter(parameterName, parameterValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Concatenate <paramref name="anotherKey"/> to this <paramref name="part"/> and return the concatenated key.
        /// If <paramref name="anotherKey"/> contains non-parametrizable nodes such as <see cref="ILineInlines"/> or <see cref="ILineFormatArgsPart"/>
        /// then these keys are not appended to the result.
        /// </summary>
        /// <param name="part">Key that must implement <see cref="ILineParameterAssignable"/>.</param>
        /// <param name="anotherKey"></param>
        /// <returns>concatenated key</returns>
        /// <exception cref="LineException">If key doesn't implement IAssetKeyParameterAssignable</exception>
        public static ILinePart Concat(this ILinePart part, ILinePart anotherKey)
        {
            ILinePart result = part;
            anotherKey.VisitFromRoot(_concatVisitor, ref result);
            return result;
        }

        /// <summary>
        /// Try concatenate <paramref name="anotherKey"/> to this <paramref name="part"/> and return the concatenated key.
        /// If <paramref name="anotherKey"/> contains non-parametrizable nodes such as <see cref="ILineInlines"/> or <see cref="ILineFormatArgsPart"/>
        /// then these keys are not appended to the result.
        /// </summary>
        /// <param name="part">Key that must implement <see cref="ILineParameterAssignable"/>.</param>
        /// <param name="anotherKey"></param>
        /// <returns>concatenated key or null</returns>
        public static ILinePart TryConcat(this ILinePart part, ILinePart anotherKey)
        {
            ILinePartAppender appender = part.GetAppender();
            if (appender == null) return null;

            ILinePart result = part;
            anotherKey.VisitFromRoot(_tryConcatVisitor, ref result);
            return result;
        }

        static LinePartVisitor<ILinePart> _concatVisitor = concatVisitor, _tryConcatVisitor = tryConcatVisitor;
        private static void concatVisitor(ILinePart part, ref ILinePart result)
        {
            if (part is ILineParameter keyParametrized)
                result = part.Parameter(keyParametrized.ParameterName, part.GetParameterValue());
        }
        private static void tryConcatVisitor(ILinePart key, ref ILinePart result)
        {
            if (key is ILineParameter keyParametrized)
                result = result?.TryAppendParameter(keyParametrized.ParameterName, key.GetParameterValue());
        }

        /// <summary>
        /// Find value for a parameter.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name, e.g. "Culture"</param>
        /// <param name="rootMost">If true returns the value that is closest to root, if false then one closes to tail</param>
        /// <returns>name or null</returns>
        public static string FindParameterValue(this ILinePart part, string parameterName, bool rootMost)
        {
            if (rootMost)
            {
                string result = null;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineParameter _parameter && _parameter.ParameterName == parameterName && p.GetParameterValue() != null)
                        result = p.GetParameterValue();
                return result;
            }
            else
            {
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineParameter parametrized && parametrized.ParameterName == parameterName && p.GetParameterValue() != null)
                        return p.GetParameterValue();
                return null;
            }
        }

    }
}

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// <see cref="ILinePart"/> extension methods with internal class dependencies.
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
        public static void GetEffectiveParameters(this ILinePart part, ref StructList12<(string, int, string)> list)
        {
            for (ILinePart k = part; k != null; k = k.PreviousPart)
            {
                // Parameter
                ILineParameter parametrized = k as ILineParameter;
                if (parametrized == null) continue;
                string parameterName = parametrized.ParameterName, parameterValue = parametrized.ParameterValue;
                if (parameterName == null) continue;

                // Canonical/Non-canonical
                bool isCanonical = k is ILineKeyCanonicallyCompared, isNonCanonical = k is ILineKeyNonCanonicallyCompared;
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