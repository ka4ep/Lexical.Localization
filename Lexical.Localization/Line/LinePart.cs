// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Basic line part.
    /// </summary>
    public class LinePart : ILinePart
    {
        /// <summary>
        /// Previous part.
        /// </summary>
        public ILinePart PreviousPart { get; protected set; }

        /// <summary>
        /// Appender
        /// </summary>
        protected ILinePartAppender appender;

        /// <summary>
        /// Get part appender.
        /// </summary>
        public virtual ILinePartAppender Appender { get => appender ?? PreviousPart?.Appender; set => throw new InvalidOperationException(nameof(Appender) + " is read-only"); }

        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="appender">(optional) Explicit appender, if null uses the Appender in <paramref name="previousPart"/></param>
        /// <param name="previousPart">(optional) link to previous part.</param>
        public LinePart(ILinePartAppender appender, ILinePart previousPart)
        {
            Appender = appender;
            PreviousPart = previousPart;
        }
    }


}
