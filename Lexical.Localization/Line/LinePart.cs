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
    /// Basic line part.
    /// </summary>
    [Serializable]
    public class LinePart : LineBase, ILinePart, ILineArguments<ILinePart>
    {
        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="appender">(optional) Explicit appender, if null uses the Appender in <paramref name="previousPart"/></param>
        /// <param name="previousPart">(optional) link to previous part.</param>
        public LinePart(ILineFactory appender, ILine previousPart) : base(appender, previousPart)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LinePart(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class LineAppender : ILineFactory<ILinePart>
    {
        /// <summary>
        /// Create <see cref="LinePart"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, out ILinePart part)
        {
            part = new LinePart(appender, previous);
            return true;
        }
    }

    /// <summary>
    /// Basic line part.
    /// </summary>
    [Serializable]
    public class StringLocalizerPart : StringLocalizerBase, ILinePart, ILineArguments<ILinePart>
    {
        /// <summary>
        /// Create line part.
        /// </summary>
        /// <param name="appender">(optional) Explicit appender, if null uses the Appender in <paramref name="previousPart"/></param>
        /// <param name="previousPart">(optional) link to previous part.</param>
        public StringLocalizerPart(ILineFactory appender, ILine previousPart) : base(appender, previousPart)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerPart(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILinePart>
    {
        /// <summary>
        /// Create <see cref="StringLocalizerPart"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool TryCreate(ILineFactory appender, ILine previous, out ILinePart part)
        {
            part = new StringLocalizerPart(appender, previous);
            return true;
        }
    }
}
