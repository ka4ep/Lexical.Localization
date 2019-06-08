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
    public class LineStringPart : LineParameterBase, ILineString, ILineHint, ILineArguments<ILineString, IString>
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
        public LineStringPart(ILineFactory appender, ILine prevKey, IString value) : base(appender, prevKey, "String", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineStringPart(SerializationInfo info, StreamingContext context) : base(info, context)
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
            line = new LineStringPart(appender, previous, value);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer localization string value.
    /// </summary>
    [Serializable]
    public class StringLocalizerStringPart : StringLocalizerParameterBase, ILineString, ILineHint, ILineArguments<ILineString, IString>
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
        public StringLocalizerStringPart(ILineFactory appender, ILine prevKey, IString value) : base(appender, prevKey, "String", value?.Text)
        {
            this.value = value;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerStringPart(SerializationInfo info, StreamingContext context) : base(info, context)
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
            StringLocalizer = new StringLocalizerStringPart(appender, previous, value);
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
        public static ILineString Text(this ILine part, String value)
            => part.Append<ILineString, IString>(TextFormat.Default.Parse(value));

        /// <summary>
        /// Create raw non-formulated, non-placeholder string.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString Text(this ILineFactory lineFactory, String value)
            => lineFactory.Create<ILineString, IString>(null, TextFormat.Default.Parse(value));

        /// <summary>
        /// Append formulation string that uses C# string format <see cref="CSharpFormat"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString Format(this ILine part, String value)
            => part.Append<ILineString, IString>(CSharpFormat.Default.Parse(value));

        /// <summary>
        /// Append formulation string that uses C# string format <see cref="CSharpFormat"/>.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILineString Format(this ILineFactory lineFactory, String value)
            => lineFactory.Create<ILineString, IString>(null, CSharpFormat.Default.Parse(value));


        /// <summary>
        /// Append <see cref="ILineString"/> and <see cref="ILineValue"/> using string interpolation, e.g. $"Hello, {user}".
        /// </summary>
        /// <param name="part"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILine Format(this ILine part, FormattableString value)
        {
            ILine line = part;
            if (value.Format != null) line = line.Append<ILineString, IString>(CSharpFormat.Default.Parse(value.Format));
            if (value.ArgumentCount > 0) line = line.Append<ILineValue, object[]>(value.GetArguments());
            return line;
        }

        /// <summary>
        /// Create <see cref="ILineString"/> and <see cref="ILineValue"/> using string interpolation, e.g. $"Hello, {user}".
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ILine Format(this ILineFactory lineFactory, FormattableString value)
        {
            ILine line = null;
            if (value.Format != null) line = lineFactory.Create<ILineString, IString>(line, CSharpFormat.Default.Parse(value.Format));
            if (value.ArgumentCount > 0) line = lineFactory.Create<ILineValue, object[]>(line, value.GetArguments());
            return line;
        }

    }
}
