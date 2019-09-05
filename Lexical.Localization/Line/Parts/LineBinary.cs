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
    /// Line embedded binary resource.
    /// </summary>
    [Serializable]
    public class LineBinary : LineBase, ILineBinary, ILineArgument<ILineBinary, byte[]>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected byte[] value;

        /// <summary>
        /// Value property
        /// </summary>
        public byte[] Binary { get => value; set => throw new InvalidOperationException(); }

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
        public LineBinary(ILineFactory appender, ILine prevKey, byte[] value) : base(appender, prevKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineBinary(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class LineAppender : ILineFactory<ILineBinary, byte[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, byte[] value, out ILineBinary line)
        {
            line = new LineBinary(appender, previous, value);
            return true;
        }
    }

    /// <summary>
    /// ResourceLocalizer localization string value.
    /// </summary>
    [Serializable]
    public class StringLocalizerBinary : StringLocalizerBase, ILineBinary, ILineArgument<ILineBinary, byte[]>
    {
        /// <summary>
        /// Value
        /// </summary>
        protected byte[] value;

        /// <summary>
        /// Value property
        /// </summary>
        public byte[] Binary { get => value; set => throw new InvalidOperationException(); }

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
        public StringLocalizerBinary(ILineFactory appender, ILine prevKey, byte[] value) : base(appender, prevKey)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerBinary(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class StringLocalizerAppender : ILineFactory<ILineBinary, byte[]>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="value"></param>
        /// <param name="ResourceLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, byte[] value, out ILineBinary ResourceLocalizer)
        {
            ResourceLocalizer = new StringLocalizerBinary(appender, previous, value);
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
                inlines[subkey] = subkey.Binary(value);
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
                inlines[subkey] = subkey.Binary(value);
            }
            return inlines;
        }
    }
}
