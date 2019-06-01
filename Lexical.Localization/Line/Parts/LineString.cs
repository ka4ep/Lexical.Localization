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
    public class LineString : LineParameterBase, ILineString, ILineHint, ILineArguments<ILineString, IString>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected IString value;

        /// <summary>
        /// Value property
        /// </summary>
        public IString String { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IString Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public LineString(ILineFactory appender, ILine prevKey, IString value) : base(appender, prevKey, "String", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineString(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("String", typeof(IString)) as IString;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("String", value);
        }
    }

    public partial class LineAppender : ILineFactory<ILineString, IString>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IString value, out ILineString line)
        {
            line = new LineString(appender, previous, value);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer localization string value.
    /// </summary>
    [Serializable]
    public class StringLocalizerString : StringLocalizerParameterBase, ILineString, ILineHint, ILineArguments<ILineString, IString>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected IString value;

        /// <summary>
        /// Value property
        /// </summary>
        public IString String { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IString Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public StringLocalizerString(ILineFactory appender, ILine prevKey, IString value) : base(appender, prevKey, "String", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerString(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("String", typeof(IString)) as IString;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("String", value);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineString, IString>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IString value, out ILineString StringLocalizer)
        {
            StringLocalizer = new StringLocalizerString(appender, previous, value);
            return true;
        }
    }

    /// <summary></summary>
    public static partial class LineExtensions
    {
        /// <summary>
        /// Append raw non-formulated, non-placeholder string.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString String(this ILine part, String value)
            => part.Append<ILineString, IString>(TextFormat.Default.Parse(value));

        /// <summary>
        /// Create raw non-formulated, non-placeholder string.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString String(this ILineFactory lineFactory, String value)
            => lineFactory.Create<ILineString, IString>(null, TextFormat.Default.Parse(value));
    }
}
