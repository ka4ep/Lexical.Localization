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
    public class LineFormatArgs : LineBase, ILineFormatArgs, ILineArguments<ILineFormatArgs, object[]>
    {
        /// <summary>
        /// Format arguments.
        /// </summary>
        protected object[] args;

        /// <summary>
        /// FormatArgs property
        /// </summary>
        public object[] Args { get => args; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public object[] Argument0 => args;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="args"></param>
        public LineFormatArgs(ILineFactory appender, ILine prevKey, object[] args) : base(appender, prevKey)
        {
            this.args = args;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineFormatArgs(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.args = info.GetValue("Args", typeof(object[])) as object[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Args", args);
        }
    }

    public partial class LineAppender : ILineFactory<ILineFormatArgs, object[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="args"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineFormatArgs, object[]>.TryCreate(ILineFactory appender, ILine previous, object[] args, out ILineFormatArgs line)
        {
            line = new LineFormatArgs(appender, previous, args);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries .Format arguments.
    /// </summary>
    [Serializable]
    public class StringLocalizerFormatArgs : StringLocalizerBase, ILineFormatArgs, ILineArguments<ILineFormatArgs, object[]>
    {
        /// <summary>
        /// Format arguments.
        /// </summary>
        protected object[] args;

        /// <summary>
        /// FormatArgs property
        /// </summary>
        public object[] Args { get => args; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public object[] Argument0 => args;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="args"></param>
        public StringLocalizerFormatArgs(ILineFactory appender, ILine prevKey, object[] args) : base(appender, prevKey)
        {
            this.args = args;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerFormatArgs(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.args = info.GetValue("Args", typeof(object[])) as object[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Args", args);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineFormatArgs, object[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="args"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        bool ILineFactory<ILineFormatArgs, object[]>.TryCreate(ILineFactory appender, ILine previous, object[] args, out ILineFormatArgs StringLocalizer)
        {
            StringLocalizer = new StringLocalizerFormatArgs(appender, previous, args);
            return true;
        }
    }

}
