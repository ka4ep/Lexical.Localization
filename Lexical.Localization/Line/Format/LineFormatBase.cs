// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           31.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Base implementation for line formats. 
    /// </summary>
    public abstract class LineFormatBase : ILineFormatPrinter, ILineFormatParser, ILineFormatAppendParser, ILineFormatFactory
    {
        /// <summary>
        /// Parameter qualifier that excludes parameter "String".
        /// </summary>
        protected static readonly ILineQualifier ExcludeValue = new LineParameterQualifier.IsEqualTo("String", -1, "");

        /// <summary>
        /// Qualifier that validates parameters.
        /// </summary>
        public virtual ILineQualifier Qualifier { get => qualifier; set => new InvalidOperationException("immutable"); }

        /// <summary>
        /// Line appender that creates line parts from parsed strings.
        /// 
        /// Appender implementation may also resolve parameter into instance, for example "Culture" into <see cref="CultureInfo"/>.
        /// </summary>
        public virtual ILineFactory LineFactory { get => lineFactory; set => throw new InvalidOperationException("immutable"); }

        /// <summary>
        /// (optional) Qualifier that validates parameters.
        /// </summary>
        protected ILineQualifier qualifier;

        /// <summary>
        /// Line appender
        /// </summary>
        protected ILineFactory lineFactory;

        /// <summary>
        /// Create new string serializer
        /// </summary>
        /// <param name="lineAppender">line appender that can append <see cref="ILineParameter"/> and <see cref="ILineString"/></param>
        /// <param name="qualifier">(optional) qualifier</param>
        public LineFormatBase(ILineFactory lineAppender, ILineQualifier qualifier)
        {
            this.lineFactory = lineAppender ?? throw new ArgumentNullException(nameof(lineAppender));
            this.qualifier = qualifier;
        }

        /// <summary>
        /// Print parameters into <paramref name="sb"/>.
        /// </summary>
        /// <param name="parameters">parameters in order of from tail to root</param>
        /// <param name="sb"></param>
        public abstract void Print(StructList12<ILineParameter> parameters, StringBuilder sb);

        /// <summary>
        /// Print parameters into string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string Print(ILine key)
        {
            StringBuilder sb = new StringBuilder();
            StructList12<ILineParameter> parameters = new StructList12<ILineParameter>();            
            key.GetEffectiveParameterParts<StructList12<ILineParameter>>(ref parameters, null, qualifier);
            Print(parameters, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Print <paramref name="key"/> into <paramref name="sb"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sb"></param>
        /// <returns><paramref name="sb"/></returns>
        public StringBuilder Print(ILine key, StringBuilder sb)
        {
            StructList12<ILineParameter> parameters = new StructList12<ILineParameter>();
            key.GetEffectiveParameterParts<StructList12<ILineParameter>>(ref parameters, null, qualifier);
            Print(parameters, sb);
            return sb;
        }

        /// <summary>
        /// Print parameters into string.
        /// </summary>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public virtual string PrintParameters(IEnumerable<KeyValuePair<string, string>> keyParameters)
        {
            StringBuilder sb = new StringBuilder();
            PrintParameters(keyParameters, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Print parameters into <paramref name="sb"/>.
        /// </summary>
        /// <param name="keyParameters"></param>
        /// <param name="sb"></param>
        public virtual void PrintParameters(IEnumerable<KeyValuePair<string, string>> keyParameters, StringBuilder sb)
        {
            StructList12<ILineParameter> parameters = new StructList12<ILineParameter>();
            StructList8<(string, int)> occurances = new StructList8<(string, int)>();
            if (Qualifier.NeedsOccuranceIndex())
            {
                foreach (var parameter in keyParameters)
                {
                    int occ = AddOccurance(ref occurances, parameter.Key);
                    ILineParameter lineParameter = new ParameterArgument(parameter.Key, parameter.Value);
                    if (!Qualifier.QualifyParameter(lineParameter, occ)) continue;
                    parameters.Add(lineParameter);
                }
            }
            else
            {
                foreach (var parameter in keyParameters)
                {
                    ILineParameter lineParameter = new ParameterArgument(parameter.Key, parameter.Value);
                    if (!Qualifier.QualifyParameter(lineParameter, -1)) continue;
                    parameters.Add(lineParameter);
                }
            }
            Print(parameters, sb);
        }

        /// <summary>
        /// Parse string into key values. Throws exception on unexpected syntax.
        /// 
        /// The implementation mustn't do parameter qualification. The caller will.
        /// </summary>
        /// <param name="str">(optional) string to parse</param>
        /// <param name="parameters">list where to put parameters. Order is from root to tail.</param>
        /// <exception cref="LineException">The parameter is not of the correct format.</exception>
        public abstract void Parse(string str, ref StructList12<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Try parse string into key values. Returns false on unexpected syntax.
        /// 
        /// The implementation mustn't do parameter qualification. The caller will.
        /// </summary>
        /// <param name="str">(optional) string to parse</param>
        /// <param name="parameters">list to where to put parameters. Order is from root to tail.</param>
        /// <returns>true if parse was successful</returns>
        public abstract bool TryParse(string str, ref StructList12<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Append part
        /// </summary>
        /// <param name="appenders"></param>
        /// <param name="prevPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        /// <exception cref="LineException">If part could not append parameter</exception>
        static ILineParameter Append(ref StructList3<ILineFactory> appenders, ILine prevPart, string parameterName, string parameterValue)
        {
            ILineParameter result;
            for (int i = 0; i < appenders.Count; i++)
                if (appenders[i].TryCreate<ILineParameter, string, string>(prevPart, parameterName, parameterValue, out result)) return result;
            throw new LineException(prevPart, $"Appender doesn't have capability to append {nameof(ILineParameter)}.");
        }

        /// <summary>
        /// Try append part
        /// </summary>
        /// <param name="appenders"></param>
        /// <param name="prevPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <exception cref="LineException">If part could not append parameter</exception>
        static bool TryAppend(ref StructList3<ILineFactory> appenders, ILine prevPart, string parameterName, string parameterValue, out ILineParameter result)
        {
            for (int i = 0; i < appenders.Count; i++)
                if (appenders[i].TryCreate<ILineParameter, string, string>(prevPart, parameterName, parameterValue, out result)) return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Parse string into ILine.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>result key, or null if it contained no parameters and <paramref name="prevPart"/> was null.</returns>
        /// <exception cref="LineException">The parameter is not of the correct format.</exception>
        public virtual ILine Parse(string str, ILine prevPart = default, ILineFactory appender = default)
        {
            // Get appenders
            StructList3<ILineFactory> appenders = new StructList3<ILineFactory>();
            if (appender != null) appenders.Add(appender);
            ILineFactory _appender;
            if (prevPart.TryGetAppender(out _appender)) appenders.AddIfNew(_appender);
            if (this.LineFactory != null) appenders.AddIfNew(this.LineFactory);
            
            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            Parse(str, ref parameters);

            // With qualifier
            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList8<(string, int)> occuranceList = new StructList8<(string, int)>();
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = Append(ref appenders, prevPart, parameter.Key, parameter.Value);
                    int occ = AddOccurance(ref occuranceList, parameter.Key);
                    if (!Qualifier.QualifyParameter(lineParameter, occ)) continue;
                    prevPart = lineParameter;
                }
            }
            else
            // No qualifier
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = Append(ref appenders, prevPart, parameter.Key, parameter.Value);
                    if (!Qualifier.QualifyParameter(lineParameter, -1)) continue;
                    prevPart = lineParameter;
                }
            }
            return prevPart;
        }

        /// <summary>
        /// Parse to parameter into arguments.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public virtual IEnumerable<ILineArguments> ParseArgs(string str)
        {
            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            Parse(str, ref parameters);

            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList12<ILineArguments> result = new StructList12<ILineArguments>();
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                ILine prev = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prev /*<-*/, parameter.Key, parameter.Value);
                    int occ = AddOccurance(ref list, parameter.Key);
                    if (!Qualifier.QualifyParameter(lineParameter, occ)) continue;
                    result.Add(LineArguments.ToArguments(lineParameter));
                    prev = lineParameter;
                }
                return result.ToArray();
            }
            else
            {
                ILineArguments[] result = new ILineArguments[parameters.Count];
                ILine prev = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prev /*<-*/, parameter.Key, parameter.Value);
                    if (!Qualifier.QualifyParameter(lineParameter, -1)) continue;
                    result[i] = LineArguments.ToArguments(lineParameter);
                    prev = lineParameter;
                }
                return result;
            }
        }

        /// <summary>
        /// Try parse string into ILine.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result">result key, or null if it contained no parameters and <paramref name="prevPart"/> was null.</param>
        /// <param name="prevPart">(optional) previous part to append to</param>
        /// <param name="appender">(optional) line appender to append with. If null, uses appender from <paramref name="prevPart"/>. If null, uses default appender.</param>
        /// <returns>true if parse was successful</returns>
        public virtual bool TryParse(string str, out ILine result, ILine prevPart = default, ILineFactory appender = default)
        {
            // Get appenders
            StructList3<ILineFactory> appenders = new StructList3<ILineFactory>();
            if (appender != null) appenders.Add(appender);
            ILineFactory _appender;
            if (prevPart.TryGetAppender(out _appender)) appenders.AddIfNew(_appender);
            if (this.LineFactory != null) appenders.AddIfNew(this.LineFactory);

            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            if (!TryParse(str, ref parameters)) { result = default; return false; }

            // With qualifier
            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList8<(string, int)> occuranceList = new StructList8<(string, int)>();
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter;
                    if (!TryAppend(ref appenders, prevPart, parameter.Key, parameter.Value, out lineParameter)) { result = default; return false; }
                    int occ = AddOccurance(ref occuranceList, parameter.Key);
                    if (!Qualifier.QualifyParameter(lineParameter, occ)) continue;
                    prevPart = lineParameter;
                }
            }
            else
            // No qualifier
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter;
                    if (!TryAppend(ref appenders, prevPart, parameter.Key, parameter.Value, out lineParameter)) { result = default; return false; }
                    if (!Qualifier.QualifyParameter(lineParameter, -1)) continue;
                    prevPart = lineParameter;
                }
            }

            result = prevPart;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool TryParseArgs(string str, out IEnumerable<ILineArguments> args)
        {
            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            if (!TryParse(str, ref parameters)) { args = default; return false; }

            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList12<ILineArguments> result = new StructList12<ILineArguments>();
                StructList8<(string, int)> list = new StructList8<(string, int)>();
                ILine prev = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prev /*<-*/, parameter.Key, parameter.Value);
                    int occ = AddOccurance(ref list, parameter.Key);
                    if (!Qualifier.QualifyParameter(lineParameter, occ)) continue;
                    result.Add(LineArguments.ToArguments(lineParameter));
                    prev = lineParameter;
                }
                args = result.ToArray();
                return true;
            }
            else
            {
                ILineArguments[] result = new ILineArguments[parameters.Count];
                ILine prev = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prev /*<-*/, parameter.Key, parameter.Value);
                    if (!Qualifier.QualifyParameter(lineParameter, -1)) continue;
                    result[i] = LineArguments.ToArguments(lineParameter);
                    prev = lineParameter;
                }
                args = result;
                return true;
            }
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>parameters</returns>
        /// <exception cref="LineException">The parameter is not of the correct format.</exception>
        public virtual IEnumerable<KeyValuePair<string, string>> ParseParameters(string str)
        {
            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            Parse(str, ref parameters);

            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList8<(string, int)> occuranceList = new StructList8<(string, int)>();
                ILine prevPart = null;
                int i = 0;
                while (i < parameters.Count)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prevPart, parameter.Key, parameter.Value);
                    int occ = AddOccurance(ref occuranceList, parameter.Key);
                    if (Qualifier.QualifyParameter(lineParameter, occ)) { i++; prevPart = lineParameter; } else parameters.RemoveAt(i);
                }
            } else if (Qualifier is ILineParameterQualifier)
            {
                ILine prevPart = null;
                int i = 0;
                while (i < parameters.Count)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prevPart, parameter.Key, parameter.Value);
                    if (Qualifier.QualifyParameter(lineParameter, -1)) { i++; prevPart = lineParameter; } else parameters.RemoveAt(i);
                }
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Parse string "parameterName:parameterValue:..." into parameter key value pairs.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result">result to write result to</param>
        /// <returns>true if successful, if false then parse failed and <paramref name="result"/>is in undetermined state</returns>
        public virtual bool TryParseParameters(string str, ICollection<KeyValuePair<string, string>> result)
        {
            if (str == null) return false;

            StructList12<KeyValuePair<string, string>> parameters = new StructList12<KeyValuePair<string, string>>();
            if (!TryParse(str, ref parameters)) return false;

            if (Qualifier.NeedsOccuranceIndex())
            {
                StructList8<(string, int)> occuranceList = new StructList8<(string, int)>();
                ILine prevPart = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prevPart, parameter.Key, parameter.Value);
                    int occ = AddOccurance(ref occuranceList, parameter.Key);
                    if (Qualifier.QualifyParameter(lineParameter, occ)) { result.Add(parameter); prevPart = lineParameter; } else parameters.RemoveAt(i);
                }
            }
            else if (Qualifier is ILineParameterQualifier)
            {
                ILine prevPart = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    KeyValuePair<string, string> parameter = parameters[i];
                    ILineParameter lineParameter = lineFactory.Create<ILineParameter, string, string>(prevPart, parameter.Key, parameter.Value);
                    if (Qualifier.QualifyParameter(lineParameter, -1)) { result.Add(parameter); prevPart = lineParameter; } else parameters.RemoveAt(i);
                }
            }
            else
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    result.Add(parameters[i]);
                }
            }
            return true;
        }

        /// <summary>
        /// Uses a list to boost performance of occurance calculation.
        /// </summary>
        /// <param name="paramOccurances">catalog of parameter occurances</param>
        /// <param name="parameterName"></param>
        /// <returns>occurance of <paramref name="parameterName"/></returns>
        protected static int AddOccurance(ref StructList8<(string, int)> paramOccurances, string parameterName)
        {
            for (int i = 0; i < paramOccurances.Count; i++)
            {
                (string name, int occ) = paramOccurances[i];
                if (name == parameterName)
                {
                    paramOccurances[i] = (name, occ + 1);
                    return occ + 1;
                }
            }
            paramOccurances.Add((parameterName, 0));
            return 0;
        }

        /// <summary></summary>
        protected class ParameterArgument : ILineArguments<ILineParameter, string, string>, ILineParameter
        {
            /// <summary></summary>
            public string Argument0 => ParameterName;
            /// <summary></summary>
            public string Argument1 => ParameterValue;
            /// <summary></summary>
            public string ParameterName { get; set; }
            /// <summary></summary>
            public string ParameterValue { get; set; }
            /// <summary></summary>
            public ParameterArgument(string parameterName, string parameterValue) { ParameterName = parameterName; ParameterValue = parameterValue; }
        }

    }

}
