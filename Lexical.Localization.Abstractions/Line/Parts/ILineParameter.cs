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
    /// Line part that represents a parameter key-value pair.
    /// 
    /// Implementing classes may implement one of sub-interfaces to detemine compare policy:
    /// <list type="bullet">
    ///     <item><see cref="ILineHint"/>not used with comparison.</item>
    ///     <item><see cref="ILineCanonicalKey"/>hash-equals comparable</item>
    ///     <item><see cref="ILineNonCanonicalKey"/>hash-equals comparable</item>
    /// </list>
    /// 
    /// If the class doesn't implement any of the sub-interfaces, comparer may consider the parameter as hash-equals comparable, if the 
    /// <see cref="ILineParameter.ParameterName"/> is known key, such as "Culture".
    /// 
    /// If the parameter class implements <see cref="ILineNonCanonicalKey"/> or <see cref="ILineCanonicalKey"/> then
    /// the parameter will be considered as hash-equals comparable regardless of the ParameterName.
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
        public static ILine Parameter(this ILine part, string parameterName, string parameterValue, IParameterInfos parameterInfos)
        {
            IParameterInfo info = null;
            if (parameterInfos != null && parameterInfos.TryGetValue(parameterName, out info))
            {
                if (info.InterfaceType == typeof(ILineHint)) return part.GetAppender().Create<ILineHint, string, string>(part, parameterName, parameterValue);
                if (info.InterfaceType == typeof(ILineCanonicalKey)) return part.GetAppender().Create<ILineCanonicalKey, string, string>(part, parameterName, parameterValue);
                if (info.InterfaceType == typeof(ILineNonCanonicalKey)) return part.GetAppender().Create<ILineNonCanonicalKey, string, string>(part, parameterName, parameterValue);
            }
            return part.GetAppender().Create<ILineParameter, string, string>(part, parameterName, parameterValue);
        }

        /// <summary>
        /// Append enumeration of parameters.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters">enumeration of parameters to append</param>
        /// <returns>new key that is appended to this key</returns>
        /// <exception cref="LineException">If key doesn't implement ILineParameterAssignable, or append failed</exception>
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
        /// <exception cref="LineException">If key doesn't implement ILineParameterAssignable, or append failed</exception>
        public static ILine Parameters(this ILine part, IEnumerable<KeyValuePair<string, string>> parameters, IParameterInfos parameterInfos)
        {
            ILineFactory appender = part.GetAppender();
            foreach (var parameter in parameters)
            {
                if (parameter.Key == null) continue;
                part = part.Parameter(parameter.Key, parameter.Value, parameterInfos);
            }
            return part;
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
            for (ILine l = line.GetPreviousPart(); l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null) return lineParameter;
                if (l is ILineParameterEnumerable lineParameters && lineParameters.FirstOrDefault() != null)
                    return l;
            }
            return null;
        }

        /// <summary>
        /// Get all parameters as parameterName,parameterValue as array with value from tail to root.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <param name="list"></param>
        /// <returns>array of parameters</returns>
        public static void GetParameterParts<LIST>(this ILine line, ref LIST list) where LIST : IList<ILineParameter>
        {
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                {
                    StructList8<ILineParameter> tmp = new StructList8<ILineParameter>();
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName != null && parameter.ParameterValue != null) tmp.Add(parameter);
                    for (int i = tmp.Count - 1; i >= 0; i--)
                        list.Add(tmp[i]);
                }
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null) list.Add(lineParameter);
            }           
        }

        /// <summary>
        /// Break <paramref name="line"/> into parameters and write with occurance index to <paramref name="list"/>.
        /// 0 is the first occurance for tha parameter name, 1 the second, etc.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="list">(Parameter, occuranceIndex) of parameters in order of from tail to root</param>
        public static void GetParameterPartsWithOccurance<LIST>(this ILine line, ref LIST list) where LIST : IList<(ILineParameter, int)>
        {
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineParameterEnumerable lineParameters)
                {
                    // Enumerate to invert order
                    StructList4<ILineParameter> tmp = new StructList4<ILineParameter>();
                    foreach (ILineParameter _parameter in lineParameters)
                        if (!String.IsNullOrEmpty(_parameter.ParameterName) && _parameter.ParameterValue != null) tmp.Add(_parameter);
                    // Add to list
                    for(int i=tmp.Count-1; i>=0; i--)
                    {
                        string name = tmp[i].ParameterName;
                        int c = list.Count;
                        list.Add((tmp[i], 0));

                        // Fix occurance
                        int occIx = 0;
                        for (int j=c; j>=0; j--)
                            if (list[j].Item1.ParameterName == name) list[j] = (list[j].Item1, ++occIx);
                    }
                }

                if (part is ILineParameter parameter && parameter.ParameterName != null)
                {
                    string name = parameter.ParameterName;
                    int c = list.Count;
                    list.Add((parameter, 0));

                    // Fix occurance
                    int occIx = 0;
                    for (int j = c; j >= 0; j--)
                        if (list[j].Item1.ParameterName == name) list[j] = (list[j].Item1, ++occIx);
                }
            }
        }



        /// <summary>
        /// Get all parameters as parameterName,parameterValue as array with value from root to tail.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>array of parameters</returns>
        public static ILineParameter[] GetParameterArray(this ILine line)
        {
            StructList12<ILineParameter> list = new StructList12<ILineParameter>();
            line.GetParameterParts<StructList12<ILineParameter>>(ref list);
            return list.ToReverseArray();
        }

        /// <summary>
        /// Get all parameters as parameterName,parameterValue as array with value from root to tail.
        /// </summary>
        /// <param name="line">(optional) line to read parameters of</param>
        /// <returns>array of parameters</returns>
        public static KeyValuePair<string, string>[] GetParameterAsKeyValues(this ILine line)
        {
            StructList12<ILineParameter> list = new StructList12<ILineParameter>();
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineParameterEnumerable lineParameters)
                {
                    StructList8<ILineParameter> tmp = new StructList8<ILineParameter>();
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName != null && parameter.ParameterValue != null) tmp.Add(parameter);
                    for (int i = tmp.Count - 1; i >= 0; i--)
                        list.Add(tmp[i]);
                }
                if (l is ILineParameter lineParameter && lineParameter.ParameterName != null && lineParameter.ParameterValue != null) list.Add(lineParameter);
            }
            
            // No parameters
            if (list.Count == 0) return no_parameter_keyvalues;
            // Reverse order and print as key-values
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[list.Count];
            for (int i = list.Count - 1, j = 0; i >= 0; i--, j++)
            {
                var item = list[i];
                result[j] = new KeyValuePair<string, string>(item.ParameterName, item.ParameterValue);
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

        /// <summary>
        /// Get parameters from root to tail.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetParameters(this ILine line)
        {
            StructList12<ILineParameter> pars = new StructList12<ILineParameter>();
            line.GetParameterParts<StructList12<ILineParameter>>(ref pars);
            for (int i = pars.Count - 1; i >= 0; i--)
            {
                ILineParameter p = pars[i];
                yield return new KeyValuePair<string, string>(p.ParameterName, p.ParameterValue);
            }
        }

    }
}

