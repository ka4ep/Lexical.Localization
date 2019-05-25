// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line localization string value.
    /// </summary>
    [Serializable]
    public class LineValue : LineParameterBase, ILineValue, ILineHint, ILineArguments<ILineValue, IFormatString>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected IFormatString value;

        /// <summary>
        /// Value property
        /// </summary>
        public IFormatString Value { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFormatString Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public LineValue(ILineFactory appender, ILine prevKey, IFormatString value) : base(appender, prevKey, "Value", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("Value", typeof(IFormatString)) as IFormatString;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", value);
        }
    }

    public partial class LineAppender : ILineFactory<ILineValue, IFormatString>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFormatString value, out ILineValue line)
        {
            line = new LineValue(appender, previous, value);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer localization string value.
    /// </summary>
    [Serializable]
    public class StringLocalizerValue : StringLocalizerParameterBase, ILineValue, ILineHint, ILineArguments<ILineValue, IFormatString>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected IFormatString value;

        /// <summary>
        /// Value property
        /// </summary>
        public IFormatString Value { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFormatString Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public StringLocalizerValue(ILineFactory appender, ILine prevKey, IFormatString value) : base(appender, prevKey, "Value", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerValue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("Value", typeof(IFormatString)) as IFormatString;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", value);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineValue, IFormatString>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFormatString value, out ILineValue StringLocalizer)
        {
            StringLocalizer = new StringLocalizerValue(appender, previous, value);
            return true;
        }
    }

}
