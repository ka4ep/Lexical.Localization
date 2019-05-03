// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Hash-equals comparable key.
    /// </summary>
    public class LineKey : LineParameterPart, ILineKey
    {
        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineKey(ILinePartAppender appender, ILinePart previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }
    }
}
