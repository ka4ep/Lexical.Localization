// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Result of an operation that resolves a <see cref="ILine"/> into a string within an executing context, such as one that includes current active culture.
    /// </summary>
    public struct LineString
    {
        /// <summary>
        /// Return string <see cref="Value"/>.
        /// </summary>
        /// <param name="str"></param>
        public static implicit operator string(LineString str)
            => str.Value;

        /// <summary>
        /// Status code
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// (optional) The line that was requested to be resolved.
        /// </summary>
        public ILine Line;

        /// <summary>
        /// Resolved string.
        /// 
        /// Depending on what was requested, either format string as is, or formatted string with arguments applied to the format.
        /// 
        /// Null, if value was not available.
        /// </summary>
        public String Value;

        /// <summary>
        /// Severity for the step that resolves <see cref="ILine"/> into format string.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity ResolveSeverity => Status.ResolveSeverity();

        /// <summary>
        /// Severity for the step that matches culture.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity CultureSeverity => Status.CultureSeverity();

        /// <summary>
        /// Severity for the step applies Plurality_.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity PluralitySeverity => Status.PluralitySeverity();

        /// <summary>
        /// Severity for the step that converts placeholder expressions into strings
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity PlaceholderSeverity => Status.PlaceholderSeverity();

        /// <summary>
        /// Severity for the step that parses format string and applies arguments.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity FormatSeverity => Status.FormatSeverity();

        /// <summary>
        /// Severity for implementation specific "Custom0" status.
        /// 
        /// "Custom0" is a status code that is specific to the <see cref="IStringResolver"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity Custom0Severity => Status.Custom0Severity();

        /// <summary>
        /// Severity for implementation specific "Custom1" status.
        /// 
        /// "Custom1" is a status code that is specific to the <see cref="IStringResolver"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public LineStatusSeverity Custom1Severity => Status.Custom1Severity();

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
        /// Tests if there is no result.
        /// </summary>
        public bool HasResult => Status != LineStatus.NoResult;

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
        /// Create new localization string.
        /// </summary>
        /// <param name="line">(optional) source line</param>
        /// <param name="value">resolved string</param>
        /// <param name="status">resolve reslut</param>
        public LineString(ILine line, string value, LineStatus status)
        {
            Line = line;
            Value = value;
            Status = status;
        }

        /// <summary>
        /// Return Value or ""
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Value ?? "";

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
                    for(int i = list.Count - 1; i >= 0; i--)
                    {
                        var parameter = list[i];
                        if (parameter.ParameterName == "Value") continue;
                        if (i < list.Count - 1) sb.Append(':');
                        sb.Append(parameter.ParameterName);
                        sb.Append(':');
                        sb.Append(parameter.ParameterValue);
                    }
                }

                // Append result
                if (Value != null)
                {
                    sb.Append(" = \"");
                    sb.Append(Value);
                    sb.Append("\"");
                }

                // Compile string
                return sb.ToString();
            }
        }
    }

}
