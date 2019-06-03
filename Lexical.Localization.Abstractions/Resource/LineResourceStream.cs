// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.IO;
using System.Text;

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Result of an operation that resolves a <see cref="ILine"/> into a string within an executing context, such as one that includes current active culture.
    /// </summary>
    public struct LineResourceStream
    {
        /// <summary>
        /// Status code
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// (optional) The line that was requested to be resolved.
        /// </summary>
        public ILine Line;

        /// <summary>
        /// Resolved resource.
        /// 
        /// Null, if value was not available.
        /// </summary>
        public Stream Value;

        /// <summary>
        /// Unexpected exception.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Tests if there is a result, be that successful or an error.
        /// </summary>
        public bool HasResult => Status != LineStatus.NoResult;

        /// <summary>
        /// Highest severity value out of each category.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity Severity => Status.Severity();

        /// <summary>
        /// Result has ok state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Produced ok value.
        /// </summary>
        public bool Ok => Status.Ok();

        /// <summary>
        /// Result has warning state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Warning state has a value, but there was something occured during the resolve that may need attention.
        /// </summary>
        public bool Warning => Status.Warning();

        /// <summary>
        /// Result has error state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Error state has some kind of fallback value, but it is bad quality.
        /// </summary>
        public bool Error => Status.Error();

        /// <summary>
        /// Result has failed state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Failed state has no value.
        /// </summary>
        public bool Failed => Status.Failed();

        /// <summary>
        /// Create resource result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="value">resolved bytes</param>
        /// <param name="status">resolve reslut</param>
        public LineResourceStream(ILine line, Stream value, LineStatus status)
        {
            Line = line;
            Value = value;
            Status = status;
            Exception = null;
        }

        /// <summary>
        /// Create resource error result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="error">error</param>
        /// <param name="status">resolve reslut</param>
        public LineResourceStream(ILine line, Exception error, LineStatus status)
        {
            Line = line;
            Value = null;
            Status = status;
            Exception = error;
        }

        /// <summary>
        /// Create resource error result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="status">resolve reslut</param>
        public LineResourceStream(ILine line, LineStatus status)
        {
            Line = line;
            Value = null;
            Status = status;
            Exception = null;
        }

        /// <summary>
        /// Create resource result
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="value">resolved bytes</param>
        /// <param name="error"></param>
        /// <param name="status">resolve reslut</param>
        public LineResourceStream(ILine line, Stream value, Exception error, LineStatus status)
        {
            Line = line;
            Value = value;
            Status = status;
            Exception = error;
        }

        /// <summary>
        /// Return Value or ""
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => DebugInfo;

        /// <summary>
        /// Print debug information about the formatting result.
        /// </summary>
        /// <returns></returns>
        public string DebugInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                // Append status
                Status.AppendFlags(sb);

                // Append key
                if (Line != null)
                {
                    sb.Append(" ");
                    StructList12<ILineParameter> list = new StructList12<ILineParameter>();
                    Line.GetParameterParts<StructList12<ILineParameter>>(ref list);
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var parameter = list[i];
                        if (parameter.ParameterName == "String") continue;
                        if (i < list.Count - 1) sb.Append(':');
                        sb.Append(parameter.ParameterName);
                        sb.Append(':');
                        sb.Append(parameter.ParameterValue);
                    }
                }

                // Append result
                if (Value != null)
                {
                    sb.Append(Value.ToString());
                }

                // Compile string
                return sb.ToString();
            }
        }
    }

}
