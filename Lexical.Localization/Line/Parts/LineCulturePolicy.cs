// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           1.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// "CulturePolicy" key that carries <see cref="ICulturePolicy"/>. 
    /// </summary>
    [Serializable]
    public class LineCulturePolicy : LineBase, ILineCulturePolicy, ILineArguments<ILineCulturePolicy, ICulturePolicy>
    {
        /// <summary>
        /// ICulturePolicy, null if non-standard culture.
        /// </summary>
        protected ICulturePolicy culturePolicy;

        /// <summary>
        /// CulturePolicy property
        /// </summary>
        public ICulturePolicy CulturePolicy { get => culturePolicy; set => throw new InvalidOperationException(); }

        /// <summary>
        /// CulturePolicy property
        /// </summary>
        ICulturePolicy ILineArguments<ILineCulturePolicy, ICulturePolicy>.Argument0 => culturePolicy;

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culturePolicy"></param>
        public LineCulturePolicy(ILineFactory appender, ILine prevKey, ICulturePolicy culturePolicy) : base(appender, prevKey)
        {
            this.culturePolicy = culturePolicy;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineCulturePolicy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culturePolicy = info.GetValue("CulturePolicy", typeof(ICulturePolicy)) as ICulturePolicy;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CulturePolicy", culturePolicy);
        }
    }

    public partial class LineAppender : ILineFactory<ILineCulturePolicy, ICulturePolicy>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, ICulturePolicy culture, out ILineCulturePolicy line)
        {
            line = new LineCulturePolicy(appender, previous, culture);
            return true;
        }
    }

    /// <summary>
    /// "CulturePolicy" key that carries <see cref="ICulturePolicy"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerCulturePolicy : StringLocalizerBase, ILineCulturePolicy, ILineArguments<ILineCulturePolicy, ICulturePolicy>
    {
        /// <summary>
        /// ICulturePolicy, null if non-standard culture.
        /// </summary>
        protected ICulturePolicy culturePolicy;

        /// <summary>
        /// CulturePolicy property
        /// </summary>
        public ICulturePolicy CulturePolicy { get => culturePolicy; set => throw new InvalidOperationException(); }

        /// <summary>
        /// CulturePolicy property
        /// </summary>
        ICulturePolicy ILineArguments<ILineCulturePolicy, ICulturePolicy>.Argument0 => culturePolicy;

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culturePolicy"></param>
        public StringLocalizerCulturePolicy(ILineFactory appender, ILine prevKey, ICulturePolicy culturePolicy) : base(appender, prevKey)
        {
            this.culturePolicy = culturePolicy;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerCulturePolicy(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.culturePolicy = info.GetValue("CulturePolicy", typeof(ICulturePolicy)) as ICulturePolicy;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("CulturePolicy", culturePolicy);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineCulturePolicy, ICulturePolicy>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, ICulturePolicy culture, out ILineCulturePolicy line)
        {
            line = new StringLocalizerCulturePolicy(appender, previous, culture);
            return true;
        }
    }

}
