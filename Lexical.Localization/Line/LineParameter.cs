// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// </summary>
    [Serializable]
    public class LineParameter : LinePart, ILineParameter, ILineArguments<ILineParameter, string, string>
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string ParameterName { get; protected set; }

        /// <summary>
        /// (optional) Parameter value.
        /// </summary>
        public string ParameterValue { get; protected set; }

        string ILineArguments<ILineParameter, string, string>.Argument0 => ParameterName;
        string ILineArguments<ILineParameter, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public override object[] GetAppendArguments() => new object[] { Tuple.Create<Type, string, string>(typeof(ILineParameter), ParameterName, ParameterValue) };

        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineParameter(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineParameter(SerializationInfo info, StreamingContext context) : base(info, context)
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

    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// </summary>
    public class StringLocalizerParameterPart : StringLocalizerKey, ILineParameter
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
        public StringLocalizerParameterPart(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, "")
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
        }
    }

    public partial class LinePartAppender : ILineFactory<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameter"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineParameter ILineFactory<ILineParameter, string, string>.Create(ILineFactory appender, ILine previous, string parameterName, string parameterValue)
            => new LineParameter(appender, previous, parameterName, parameterValue);
    }

    public partial class StringLocalizerPartAppender : ILineFactory<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameter"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineParameter ILineFactory<ILineParameter, string, string>.Create(ILineFactory appender, ILine previous, string parameterName, string parameterValue)
            => new StringLocalizerParameterPart(appender, previous, parameterName, parameterValue);
    }

}
