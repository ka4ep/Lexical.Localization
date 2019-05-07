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
        public LineKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
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
        public class NonCanonical : LineKey, ILineKeyNonCanonicallyCompared, ILineArguments<ILineKeyNonCanonicallyCompared, string, string>
        {
            string ILineArguments<ILineKeyNonCanonicallyCompared, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineKeyNonCanonicallyCompared, string, string>.Argument1 => ParameterValue;

            /// <summary>
            /// Appending arguments.
            /// </summary>
            public override object[] GetAppendArguments() => new object[] { Tuple.Create<Type, string, string>(typeof(ILineKeyNonCanonicallyCompared), ParameterName, ParameterValue) };

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="previousPart"></param>
            /// <param name="parameterName"></param>
            /// <param name="parameterValue"></param>
            public NonCanonical(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
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
        public class Canonical : LineKey, ILineKeyCanonicallyCompared, ILineArguments<ILineKeyNonCanonicallyCompared, string, string>
        {
            string ILineArguments<ILineKeyNonCanonicallyCompared, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineKeyNonCanonicallyCompared, string, string>.Argument1 => ParameterValue;

            /// <summary>
            /// Appending arguments.
            /// </summary>
            public override object[] GetAppendArguments() => new object[] { Tuple.Create<Type, string, string>(typeof(ILineKeyCanonicallyCompared), ParameterName, ParameterValue) };

            /// <summary>
            /// 
            /// </summary>
            /// <param name="appender"></param>
            /// <param name="previousPart"></param>
            /// <param name="parameterName"></param>
            /// <param name="parameterValue"></param>
            public Canonical(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
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

    public partial class LineAppender : ILineFactory<ILineKeyNonCanonicallyCompared, string, string>, ILineFactory<ILineKeyCanonicallyCompared, string, string>
    {
        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineKeyNonCanonicallyCompared ILineFactory<ILineKeyNonCanonicallyCompared, string, string>.Create(ILineFactory appender, ILine previous, string parameterName, string parameterValue)
            => new LineKey.NonCanonical(appender, previous, parameterName, parameterValue);

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILineKeyCanonicallyCompared ILineFactory<ILineKeyCanonicallyCompared, string, string>.Create(ILineFactory appender, ILine previous, string parameterName, string parameterValue)
            => new LineKey.Canonical(this, previous, parameterName, parameterValue);
    }


}
