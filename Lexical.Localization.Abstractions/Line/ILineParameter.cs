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
    /// If the parameter class implements <see cref="ILineKeyNonCanonicallyCompared"/> or <see cref="ILineKeyCanonicallyCompared"/> then
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
    /// If the parameter class implements <see cref="ILineKeyNonCanonicallyCompared"/> or <see cref="ILineKeyCanonicallyCompared"/> then
    /// the parameter will be considered as hash-equals comparable despite the <see cref="ILineParameter.ParameterName"/> of the key.
    /// 
    /// The enumerable must keys as well as parameters.
    /// </summary>
    public interface ILineParameterEnumerable : ILine, IEnumerable<ILineParameter>
    {
    }

    /// <summary>
    /// Function that visits parameters
    /// </summary>
    /// <typeparam name="T">type for the visitor to choose</typeparam>
    /// <param name="parameter"></param>
    /// <param name="data"></param>
    public delegate void ParameterVisitor<T>(ILineParameter parameter, ref T data);

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append new parameter part.
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
        /// Append new parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(optional) parameter value</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new parameter part</returns>
        /// <exception cref="LineException">If part could not be appended</exception>
        /// <returns>new part</returns>
        public static ILine Parameter(this ILine part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical) return part.GetAppender().Create<ILineKeyCanonicallyCompared, string, string>(part, parameterName, parameterValue);
                else if (info.IsNonCanonical) return part.GetAppender().Create<ILineKeyNonCanonicallyCompared, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().Create<ILineParameter, string, string>(part, parameterName, parameterValue);
        }

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILine TryAppendParameter(this ILine part, string parameterName, string parameterValue)
            => part.GetAppender().TryCreate<ILineParameter, string, string>(part, parameterName, parameterValue);

        /// <summary>
        /// Try to create a new key by appending an another key node with <paramref name="parameterName"/> and <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterValue">(otional) parameter value.</param>
        /// <param name="parameterInfos">(optional) instructions on whether to instantiate as parameter or key. See <see cref="ParameterInfos.Default"/> for default configuration</param>
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILine TryAppendParameter(this ILine part, string parameterName, string parameterValue, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info) && (info.IsCanonical || info.IsNonCanonical))
            {
                if (info.IsCanonical) return part.GetAppender().TryCreate<ILineKeyCanonicallyCompared, string, string>(part, parameterName, parameterValue);
                else if (info.IsNonCanonical) return part.GetAppender().TryCreate<ILineKeyNonCanonicallyCompared, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().TryCreate<ILineParameter, string, string>(part, parameterName, parameterValue);

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
        public static ILine Parameter(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            ILineFactory appender = part.GetAppender();
            if (appender == null) throw new LineException(part, "Appender is not found.");
            foreach (var parameter in parameters)
            {
                if (parameter.Key == null) continue;
                IParameterInfo info = null;
                if (parameterInfos != null && parameterInfos.TryGetValue(parameter.Key, out info) && (info.IsCanonical || info.IsNonCanonical))
                {
                    if (info.IsCanonical) part = appender.Create<ILineKeyCanonicallyCompared, string, string>(part, parameter.Key, parameter.Value);
                    else if (info.IsNonCanonical) part = appender.Create<ILineKeyNonCanonicallyCompared, string, string>(part, parameter.Key, parameter.Value);
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
        /// <returns>new key that is appended to this key, or null if could not be appended.</returns>
        public static ILine TryAppendParameters(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            ILineFactory appender = part.GetAppender();
            if (appender == null) return null;
            foreach (var parameter in parameters)
            {
                if (part == null) return null;
                part = appender.TryCreate<ILineParameter, string, string>(part, parameter.Key, parameter.Value);
            }
            return part;
        }

        /// <summary>
        /// Get effective (closes to root) parameter value
        /// </summary>
        /// <param name="line"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string GetParameter(this ILine line, string parameterName)
        {
            string result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName == parameterName && lineParameter.ParameterValue != null) result = lineParameter.ParameterValue;
                if (l is ILineParameterEnumerable lineParameters)
                {
                    var parms = lineParameters.Parameters;
                    if (parms != null)
                        foreach (var p in parms)
                            if (p.Key == parameterName && p.Value != null) result = p.Value;
                }
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
            if (line is ILineParameterEnumerable lineParameters) return lineParameters.Parameters.Count();

            // Count parts
            if (line is ILine part)
            {
                int count = 0;
                for (ILine p = part; p != null; p = p.GetPreviousPart())
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
        public static ILineParameter GetParameterPart(this ILine tail, string parameterName)
        {
            if (tail == null || parameterName == null) return null;
            for (ILine p = tail; p != null; p = p.GetPreviousPart())
                if (p is ILineParameter part && part.ParameterName == parameterName) return part;
            return null;
        }

        /// <summary>
        /// Get previous valid parameter part.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>key or null</returns>
        public static ILineParameter GetPreviousParameterPart(this ILine part)
        {
            if (part == null) return null;
            for (ILine k = part.GetPreviousPart(); k != null; k = k.GetPreviousPart())
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
            if (line is ILineParameterEnumerable lineParameters)
            {
                var enumr = lineParameters.Parameters;
                return enumr is IList<KeyValuePair<string, string>> list ? list : enumr.ToList();
            }

            if (line is ILine part)
            {
                int count = 0;
                for (ILine p = part; p != null; p = p.GetPreviousPart())
                    if (p is ILineParameter parameter && parameter.ParameterName != null && parameter.ParameterValue != null) count++;

                if (count == 0) return no_parameters;

                KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[count];
                int ix = count;
                for (ILine p = part; p != null; p = p.GetPreviousPart())
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
        public static void VisitParameters<T>(this ILine key, ParameterVisitor<T> visitor, ref T data)
        {
            // Push to stack
            ILine prevKey = key.GetPreviousPart();
            if (prevKey != null) VisitParameters(prevKey, visitor, ref data);

            // Pop from stack in reverse order
            if (key is ILineParameter parameter && parameter.ParameterName!=null) visitor(parameter.ParameterName, parameter.ParameterValue, ref data);
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
        public static ILine ConcatIfNew(this ILine left, ILine right)
        {
            if (right == null) return left;
            if (left == null) return right;
            ILine result = left;
            foreach (ILine k in right.ToArray())
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
        public static ILine Concat(this ILine part, ILine anotherKey)
        {
            ILine result = part;
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
        public static ILine TryConcat(this ILine part, ILine anotherKey)
        {
            ILineFactory appender = part.GetAppender();
            if (appender == null) return null;

            ILine result = part;
            anotherKey.VisitFromRoot(_tryConcatVisitor, ref result);
            return result;
        }

        static LinePartVisitor<ILine> _concatVisitor = concatVisitor, _tryConcatVisitor = tryConcatVisitor;
        private static void concatVisitor(ILine part, ref ILine result)
        {
            if (part is ILineParameter keyParametrized)
                result = part.Parameter(keyParametrized.ParameterName, part.GetParameterValue());
        }
        private static void tryConcatVisitor(ILine key, ref ILine result)
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
        public static string FindParameterValue(this ILine part, string parameterName, bool rootMost)
        {
            if (rootMost)
            {
                string result = null;
                for (ILine p = part; p != null; p = p.GetPreviousPart())
                    if (p is ILineParameter _parameter && _parameter.ParameterName == parameterName && p.GetParameterValue() != null)
                        result = p.GetParameterValue();
                return result;
            }
            else
            {
                for (ILine p = part; p != null; p = p.GetPreviousPart())
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