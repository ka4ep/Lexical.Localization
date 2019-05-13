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
    public class LineStringFormat : LineParameterBase, ILineStringFormat, ILineHint, ILineArguments<ILineStringFormat, IStringFormat>
    {
        /// <summary>
        /// String format
        /// </summary>
        protected IStringFormat formatProvider;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IStringFormat StringFormat { get => formatProvider; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IStringFormat Argument0 => formatProvider;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="formatProvider"></param>
        public LineStringFormat(ILineFactory appender, ILine prevKey, IStringFormat formatProvider) : base(appender, prevKey, "StringFormat", formatProvider?.GetType()?.FullName)
        {
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineStringFormat(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.formatProvider = info.GetValue("StringFormat", typeof(IStringFormat)) as IStringFormat;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StringFormat", formatProvider);
        }
    }

    public partial class LineAppender : ILineFactory<ILineStringFormat, IStringFormat>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IStringFormat formatProvider, out ILineStringFormat line)
        {
            line = new LineStringFormat(appender, previous, formatProvider);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries format provider.
    /// </summary>
    [Serializable]
    public class StringLocalizerStringFormat : StringLocalizerParameterBase, ILineStringFormat, ILineHint, ILineArguments<ILineStringFormat, IStringFormat>
    {
        /// <summary>
        /// Format provider
        /// </summary>
        protected IStringFormat formatProvider;

        /// <summary>
        /// Assembly property
        /// </summary>
        public IStringFormat StringFormat { get => formatProvider; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IStringFormat Argument0 => formatProvider;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="formatProvider"></param>
        public StringLocalizerStringFormat(ILineFactory appender, ILine prevKey, IStringFormat formatProvider) : base(appender, prevKey, "StringFormat", formatProvider?.GetType()?.FullName)
        {
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerStringFormat(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.formatProvider = info.GetValue("StringFormat", typeof(IStringFormat)) as IStringFormat;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StringFormat", formatProvider);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineStringFormat, IStringFormat>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="formatProvider"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IStringFormat formatProvider, out ILineStringFormat StringLocalizer)
        {
            StringLocalizer = new StringLocalizerStringFormat(appender, previous, formatProvider);
            return true;
        }
    }

}
