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
    /// Default part appender.
    /// </summary>
    public partial class StringLocalizerPartAppender : LinePartAppenderList
    {
        private readonly static ILinePartAppender instance = new StringLocalizerPartAppender().ReadOnly();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ILinePartAppender Instance => instance;

        /// <summary>
        /// Create new part appender
        /// </summary>
        public StringLocalizerPartAppender()
        {
        }

    }
}
