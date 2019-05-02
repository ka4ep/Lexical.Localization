// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
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
        /// Part appender
        /// </summary>
        public ILinePartAppender Appender { get; protected set; }

        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="previousPart"></param>
        /// <param name="appender"></param>
        public LinePart(ILinePart previousPart, ILinePartAppender appender)
        {
            PreviousPart = previousPart;
            Appender = appender;
        }
    }


}
