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
    public class LineCultureKey : LineKey, ILineKeyCulture, ILineKeyNonCanonicallyCompared
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
        /// Appending arguments.
        /// </summary>
        public override object[] GetAppendArguments() 
            => new object[] { Tuple.Create<Type, CultureInfo>(typeof(ILineKeyCulture), Culture) };

        /// <summary>
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public LineCultureKey(ILineFactory appender, ILine prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineCultureKey(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class LinePartAppender : ILineFactory<ILineKeyCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        ILineKeyCulture ILineFactory<ILineKeyCulture, CultureInfo>.Create(ILineFactory appender, ILine previous, CultureInfo culture)
            => new LineCultureKey(appender, previous, culture);
    }

    /*
    /// <summary>
    /// "Culture" key that carries <see cref="CultureInfo"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerCultureKey : StringLocalizerKey, ILineKeyCulture
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
        /// Create new culture key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="culture"></param>
        public StringLocalizerCultureKey(ILineFactory appender, ILine prevKey, CultureInfo culture) : base(appender, prevKey, "Culture", culture?.Name)
        {
            this.culture = culture;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerCultureKey(SerializationInfo info, StreamingContext context) : base(info, context)
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

    public partial class StringLocalizerPartAppender : ILineFactory1<ILineKeyCulture, CultureInfo>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public ILineKeyCulture Append(ILine previous, CultureInfo culture)
            => new StringLocalizerCultureKey(this, previous, culture);
    }
*/
}
