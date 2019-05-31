// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// "Culture" key that carries <see cref="CultureInfo"/>. 
    /// </summary>
    [Serializable]
    public class LineCulture : LineKey, ILineCulture, ILineNonCanonicalKey, ILineArguments<ILineCulture, CultureInfo>
    {
        /// <summary>
        /// CultureInfo, null if non-standard culture.
        /// </summary>
        protected CultureInfo culture;

        /// <summary>
        /// Culture property
        /// </summary>
        public CultureInfo Culture { get => culture; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Culture property
        /// </summary>
        CultureInfo ILineArguments<ILineCulture, CultureInfo>.Argument0 => culture;

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public LineCulture(ILineFactory appender, ILine prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineCulture(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culture = info.GetValue("Culture", typeof(CultureInfo)) as CultureInfo;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Culture", culture);
        }
    }

    public partial class LineAppender : ILineFactory<ILineCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, CultureInfo culture, out ILineCulture line)
        {
            line = new LineCulture(appender, previous, culture);
            return true;
        }
    }

    /// <summary>
    /// "Culture" key that carries <see cref="CultureInfo"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerCulture : _StringLocalizerKey, ILineCulture, ILineNonCanonicalKey, ILineArguments<ILineCulture, CultureInfo>
    {
        /// <summary>
        /// CultureInfo, null if non-standard culture.
        /// </summary>
        protected CultureInfo culture;

        /// <summary>
        /// Culture property
        /// </summary>
        public CultureInfo Culture { get => culture; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Culture property
        /// </summary>
        CultureInfo ILineArguments<ILineCulture, CultureInfo>.Argument0 => culture;

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public StringLocalizerCulture(ILineFactory appender, ILine prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerCulture(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culture = info.GetValue("Culture", typeof(CultureInfo)) as CultureInfo;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Culture", culture);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, CultureInfo culture, out ILineCulture line)
        {
            line = new StringLocalizerCulture(appender, previous, culture);
            return true;
        }
    }

}
