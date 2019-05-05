// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Hash-equals comparable key.
    /// </summary>
    [Serializable]
    public abstract class LineKey : LineParameter, ILineKey
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

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineKey(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Non-canonically compared key
        /// </summary>
        public class NonCanonical : LineKey, ILineKeyNonCanonicallyCompared
        {
            /// <summary>
            /// Appending arguments.
            /// </summary>
            public override IEnumerable<Object[]> GetAppendArguments() { yield return new Object[] { typeof(ILineKeyNonCanonicallyCompared), ParameterName, ParameterValue }; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="previousPart"></param>
            /// <param name="parameterName"></param>
            /// <param name="parameterValue"></param>
            public NonCanonical(ILinePartAppender appender, ILinePart previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public NonCanonical(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        /// <summary>
        /// Canonically compared key
        /// </summary>
        public class Canonical : LineKey, ILineKeyCanonicallyCompared
        {
            /// <summary>
            /// Appending arguments.
            /// </summary>
            public override IEnumerable<Object[]> GetAppendArguments() { yield return new Object[] { typeof(ILineKeyCanonicallyCompared), ParameterName, ParameterValue }; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="previousPart"></param>
            /// <param name="parameterName"></param>
            /// <param name="parameterValue"></param>
            public Canonical(ILinePartAppender appender, ILinePart previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public Canonical(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }

    public partial class LinePartAppender : ILinePartAppender2<ILineKeyNonCanonicallyCompared, string, string>, ILinePartAppender2<ILineKeyCanonicallyCompared, string, string>
    {
        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineKeyNonCanonicallyCompared ILinePartAppender2<ILineKeyNonCanonicallyCompared, string, string>.Append(ILinePart previous, string parameterName, string parameterValue)
            => new LineKey.NonCanonical(this, previous, parameterName, parameterValue);

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineKeyCanonicallyCompared ILinePartAppender2<ILineKeyCanonicallyCompared, string, string>.Append(ILinePart previous, string parameterName, string parameterValue)
            => new LineKey.Canonical(this, previous, parameterName, parameterValue);
    }


}
