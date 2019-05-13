// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// </summary>
    [Serializable]
    public class LineParameter : LineParameterBase, ILineArguments<ILineParameter, string, string>
    {
        string ILineArguments<ILineParameter, string, string>.Argument0 => ParameterName;
        string ILineArguments<ILineParameter, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineParameter(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineParameter(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Line part that contains a parameter key-value pair.
    /// </summary>
    public class LineParameterBase : LineBase, ILineParameter
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string ParameterName { get; protected set; }

        /// <summary>
        /// (optional) Parameter value.
        /// </summary>
        public string ParameterValue { get; protected set; }

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineParameterBase(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineParameterBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.ParameterName = info.GetString(nameof(ParameterName));
            this.ParameterValue = info.GetString(nameof(ParameterValue));
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ParameterName), ParameterName);
            info.AddValue(nameof(ParameterValue), ParameterValue);
        }
    }

    public partial class LineAppender : ILineFactory<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameter"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineParameter result)
        {
            result = new LineParameter(appender, previous, parameterName, parameterValue);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that represents a parameter key-value pair.
    /// </summary>
    [Serializable]
    public class StringLocalizerParameter : StringLocalizerParameterBase, ILineArguments<ILineParameter, string, string>
    {
        string ILineArguments<ILineParameter, string, string>.Argument0 => ParameterName;
        string ILineArguments<ILineParameter, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public StringLocalizerParameter(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerParameter(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// StringLocalizer part that contains a parameter key-value pair.
    /// </summary>
    public class StringLocalizerParameterBase : StringLocalizerBase, ILineParameter
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string ParameterName { get; protected set; }

        /// <summary>
        /// (optional) Parameter value.
        /// </summary>
        public string ParameterValue { get; protected set; }

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public StringLocalizerParameterBase(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerParameterBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.ParameterName = info.GetString(nameof(ParameterName));
            this.ParameterValue = info.GetString(nameof(ParameterValue));
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ParameterName), ParameterName);
            info.AddValue(nameof(ParameterValue), ParameterValue);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="StringLocalizerParameter"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineParameter result)
        {
            result = new StringLocalizerParameter(appender, previous, parameterName, parameterValue);
            return true;
        }
    }

}
