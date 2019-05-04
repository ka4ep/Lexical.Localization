// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that represents a parameter key-value pair.
    /// </summary>
    public class LineParameterPart : LinePart, ILineParameter
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
        public LineParameterPart(ILinePartAppender appender, ILinePart previousPart, string parameterName, string parameterValue) : base(appender, previousPart)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
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
        public StringLocalizerParameterPart(ILinePartAppender appender, ILinePart previousPart, string parameterName, string parameterValue) : base(appender, previousPart, "")
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue;
        }
    }

    public partial class LinePartAppender : ILinePartAppender2<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameterPart"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public ILineParameter Append(ILinePart previous, string parameterName, string parameterValue)
            => new LineParameterPart(this, previous, parameterName, parameterValue);
    }

    public partial class StringLocalizerPartAppender : ILinePartAppender2<ILineParameter, string, string>
    {
        /// <summary>
        /// Append <see cref="LineParameterPart"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public ILineParameter Append(ILinePart previous, string parameterName, string parameterValue)
            => new StringLocalizerParameterPart(this, previous, parameterName, parameterValue);
    }

}
