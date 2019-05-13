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
    /// Hash-equals comparable key.
    /// </summary>
    [Serializable]
    public abstract class LineKey : LineParameterBase, ILineKey
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
        public class NonCanonical : LineKey, ILineNonCanonicalKey, ILineArguments<ILineNonCanonicalKey, string, string>
        {
            string ILineArguments<ILineNonCanonicalKey, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineNonCanonicalKey, string, string>.Argument1 => ParameterValue;

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
        public class Canonical : LineKey, ILineCanonicalKey, ILineArguments<ILineCanonicalKey, string, string>
        {
            string ILineArguments<ILineCanonicalKey, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineCanonicalKey, string, string>.Argument1 => ParameterValue;

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

    public partial class LineAppender : ILineFactory<ILineNonCanonicalKey, string, string>, ILineFactory<ILineCanonicalKey, string, string>
    {
        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineNonCanonicalKey key)
        {
            key = new LineKey.NonCanonical(appender, previous, parameterName, parameterValue);
            return true;
        }

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineCanonicalKey key)
        {
            key = new LineKey.Canonical(this, previous, parameterName, parameterValue);
            return true;
        }
    }


    /// <summary>
    /// Hash-equals comparable key.
    /// </summary>
    [Serializable]
    public abstract class _StringLocalizerKey : StringLocalizerParameterBase, ILineKey
    {
        /// <summary>
        /// Create parameter part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public _StringLocalizerKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public _StringLocalizerKey(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Non-canonically compared key
        /// </summary>
        public class NonCanonical : _StringLocalizerKey, ILineNonCanonicalKey, ILineArguments<ILineNonCanonicalKey, string, string>
        {
            string ILineArguments<ILineNonCanonicalKey, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineNonCanonicalKey, string, string>.Argument1 => ParameterValue;

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
        public class Canonical : _StringLocalizerKey, ILineCanonicalKey, ILineArguments<ILineCanonicalKey, string, string>
        {
            string ILineArguments<ILineCanonicalKey, string, string>.Argument0 => ParameterName;
            string ILineArguments<ILineCanonicalKey, string, string>.Argument1 => ParameterValue;

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

    public partial class StringLocalizerAppender : ILineFactory<ILineNonCanonicalKey, string, string>, ILineFactory<ILineCanonicalKey, string, string>
    {
        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineNonCanonicalKey key)
        {
            key = new _StringLocalizerKey.NonCanonical(appender, previous, parameterName, parameterValue);
            return true;
        }

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineCanonicalKey key)
        {
            key = new _StringLocalizerKey.Canonical(this, previous, parameterName, parameterValue);
            return true;
        }
    }


}
