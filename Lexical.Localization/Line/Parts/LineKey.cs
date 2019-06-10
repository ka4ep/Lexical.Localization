// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;
using Lexical.Localization.StringFormat;

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
    }

    /// <summary>
    /// Non-canonically compared key
    /// </summary>
    public class LineNonCanonicalKey : LineKey, ILineNonCanonicalKey, ILineArgument<ILineNonCanonicalKey, string, string>
    {
        string ILineArgument<ILineNonCanonicalKey, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineNonCanonicalKey, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create non-canonical key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineNonCanonicalKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineNonCanonicalKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Canonically compared key
    /// </summary>
    public class LineCanonicalKey : LineKey, ILineCanonicalKey, ILineArgument<ILineCanonicalKey, string, string>
    {
        string ILineArgument<ILineCanonicalKey, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineCanonicalKey, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// Create canonical key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public LineCanonicalKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineCanonicalKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
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
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineNonCanonicalKey result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineNonCanonicalKey castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            result = new LineNonCanonicalKey(appender, previous, parameterName, parameterValue);
            return true;
        }

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineCanonicalKey result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineCanonicalKey castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            result = new LineCanonicalKey(this, previous, parameterName, parameterValue);
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
     }

    /// <summary>
    /// Non-canonically compared key
    /// </summary>
    public class StringLocalizerNonCanonicalKey : _StringLocalizerKey, ILineNonCanonicalKey, ILineArgument<ILineNonCanonicalKey, string, string>
    {
        string ILineArgument<ILineNonCanonicalKey, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineNonCanonicalKey, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public StringLocalizerNonCanonicalKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerNonCanonicalKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Canonically compared key
    /// </summary>
    public class StringLocalizerCanonicalKey : _StringLocalizerKey, ILineCanonicalKey, ILineArgument<ILineCanonicalKey, string, string>
    {
        string ILineArgument<ILineCanonicalKey, string, string>.Argument0 => ParameterName;
        string ILineArgument<ILineCanonicalKey, string, string>.Argument1 => ParameterValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previousPart"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public StringLocalizerCanonicalKey(ILineFactory appender, ILine previousPart, string parameterName, string parameterValue) : base(appender, previousPart, parameterName, parameterValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerCanonicalKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
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
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineNonCanonicalKey result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineNonCanonicalKey castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            result = new StringLocalizerNonCanonicalKey(appender, previous, parameterName, parameterValue);
            return true;
        }

        /// <summary>
        /// Append key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, string parameterName, string parameterValue, out ILineCanonicalKey result)
        {
            // Try resolve
            ILineArgument args;
            ILine resolved;
            if (Resolver.TryResolveParameter(previous, parameterName, parameterValue, out args) && this.TryCreate(previous, args, out resolved) && resolved is ILineCanonicalKey castedResolved)
            {
                // Return as parameter and as resolved instance
                result = castedResolved;
                return true;
            }

            result = new StringLocalizerCanonicalKey(this, previous, parameterName, parameterValue);
            return true;
        }
    }

}
