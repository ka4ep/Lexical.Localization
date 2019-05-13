// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries format provider.
    /// </summary>
    [Serializable]
    public class LineFormatProvider : LineParameterBase, ILineFormatProvider, ILineHint, ILineArguments<ILineFormatProvider, IFormatProvider>
    {
        /// <summary>
        /// Format provider
        /// </summary>
        protected IFormatProvider formatProvider;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IFormatProvider FormatProvider { get => formatProvider; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFormatProvider Argument0 => formatProvider;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="formatProvider"></param>
        public LineFormatProvider(ILineFactory appender, ILine prevKey, IFormatProvider formatProvider) : base(appender, prevKey, "FormatProvider", formatProvider?.GetType()?.FullName)
        {
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineFormatProvider(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.formatProvider = info.GetValue("FormatProvider", typeof(IFormatProvider)) as IFormatProvider;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FormatProvider", formatProvider);
        }
    }

    public partial class LineAppender : ILineFactory<ILineFormatProvider, IFormatProvider>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFormatProvider formatProvider, out ILineFormatProvider line)
        {
            line = new LineFormatProvider(appender, previous, formatProvider);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries format provider.
    /// </summary>
    [Serializable]
    public class StringLocalizerFormatProvider : StringLocalizerParameterBase, ILineFormatProvider, ILineHint, ILineArguments<ILineFormatProvider, IFormatProvider>
    {
        /// <summary>
        /// Format provider
        /// </summary>
        protected IFormatProvider formatProvider;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IFormatProvider FormatProvider { get => formatProvider; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IFormatProvider Argument0 => formatProvider;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="formatProvider"></param>
        public StringLocalizerFormatProvider(ILineFactory appender, ILine prevKey, IFormatProvider formatProvider) : base(appender, prevKey, "FormatProvider", formatProvider?.GetType()?.FullName)
        {
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerFormatProvider(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.formatProvider = info.GetValue("FormatProvider", typeof(IFormatProvider)) as IFormatProvider;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FormatProvider", formatProvider);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineFormatProvider, IFormatProvider>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IFormatProvider formatProvider, out ILineFormatProvider StringLocalizer)
        {
            StringLocalizer = new StringLocalizerFormatProvider(appender, previous, formatProvider);
            return true;
        }
    }

}
