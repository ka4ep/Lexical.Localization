// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries .Format arguments.
    /// </summary>
    [Serializable]
    public class LineValue : LineBase, ILineValue, ILineArgument<ILineValue, object[]>
    {
        /// <summary>
        /// Format arguments.
        /// </summary>
        protected object[] values;

        /// <summary>
        /// FormatArgs property
        /// </summary>
        public object[] Value { get => values; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public object[] Argument0 => values;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="values"></param>
        public LineValue(ILineFactory appender, ILine prevKey, object[] values) : base(appender, prevKey)
        {
            this.values = values;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.values = info.GetValue("Value", typeof(object[])) as object[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", values);
        }
    }

    public partial class LineAppender : ILineFactory<ILineValue, object[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="args"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, object[] args, out ILineValue line)
        {
            line = new LineValue(appender, previous, args);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries .Format arguments.
    /// </summary>
    [Serializable]
    public class StringLocalizerValue : StringLocalizerBase, ILineValue, ILineArgument<ILineValue, object[]>
    {
        /// <summary>
        /// Format arguments.
        /// </summary>
        protected object[] values;

        /// <summary>
        /// FormatArgs property
        /// </summary>
        public object[] Value { get => values; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public object[] Argument0 => values;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="values"></param>
        public StringLocalizerValue(ILineFactory appender, ILine prevKey, object[] values) : base(appender, prevKey)
        {
            this.values = values;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.values = info.GetValue("Value", typeof(object[])) as object[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", values);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineValue, object[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="args"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, object[] args, out ILineValue StringLocalizer)
        {
            StringLocalizer = new StringLocalizerValue(appender, previous, args);
            return true;
        }
    }

}
