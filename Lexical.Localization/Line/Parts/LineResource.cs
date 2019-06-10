// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line embedded resource bytes.
    /// </summary>
    [Serializable]
    public class LineResource : LineBase, ILineResource, ILineArgument<ILineResource, byte[]>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected byte[] value;

        /// <summary>
        /// Value property
        /// </summary>
        public byte[] Resource { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public byte[] Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public LineResource(ILineFactory appender, ILine prevKey, byte[] value) : base(appender, prevKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineResource(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("Resource", typeof(byte[])) as byte[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Resource", value);
        }
    }

    public partial class LineAppender : ILineFactory<ILineResource, byte[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, byte[] value, out ILineResource line)
        {
            line = new LineResource(appender, previous, value);
            return true;
        }
    }

    /// <summary>
    /// ResourceLocalizer localization string value.
    /// </summary>
    [Serializable]
    public class StringLocalizerResource : StringLocalizerBase, ILineResource, ILineArgument<ILineResource, byte[]>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected byte[] value;

        /// <summary>
        /// Value property
        /// </summary>
        public byte[] Resource { get => value; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public byte[] Argument0 => value;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="value"></param>
        public StringLocalizerResource(ILineFactory appender, ILine prevKey, byte[] value) : base(appender, prevKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerResource(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.value = info.GetValue("Resource", typeof(byte[])) as byte[];
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Resource", value);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineResource, byte[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="ResourceLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, byte[] value, out ILineResource ResourceLocalizer)
        {
            ResourceLocalizer = new StringLocalizerResource(appender, previous, value);
            return true;
        }
    }

    /// <summary></summary>
    public static partial class LineExtensions
    {
        /// <summary>
        /// Add inlines resource line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en"</param>
        /// <param name="value">value to add, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, string subKeyText, byte[] value)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subkey = LineFormat.Parameters.Parse(subKeyText, line?.GetPreviousPart());
            if (value == null)
            {
                inlines.Remove(subkey);
            }
            else
            {
                inlines[subkey] = subkey.Resource(value);
            }
            return line;
        }

        /// <summary>
        /// Create inlined <paramref name="subkeyText"/> resource.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="subkeyText">subkey in parametrized format, e.g. "Culture:en"</param>
        /// <param name="value">(optional) value to append, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="lineFactory"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILineFactory lineFactory, string subkeyText, byte[] value)
        {
            ILineInlines inlines = lineFactory.Create<ILineInlines>(null);
            ILine subkey = LineFormat.ParametersInclString.Parse(subkeyText, null);
            if (value == null)
            {
                inlines.Remove(subkey);
            } else
            {
                inlines[subkey] = subkey.Resource(value);
            }
            return inlines;
        }
    }
}
