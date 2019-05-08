// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineLocalizationResolver"/>. 
    /// </summary>
    [Serializable]
    public class LineLocalizationResolver : LineBase, ILineLocalizationResolver, ILineArguments<ILocalizationResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected ILocalizationResolver resolver;

        /// <summary>
        /// ILineLocalizationResolver property
        /// </summary>
        public ILocalizationResolver Resolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public ILocalizationResolver Argument0 => resolver;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="localizationResolver"></param>
        public LineLocalizationResolver(ILineFactory appender, ILine prevKey, ILocalizationResolver localizationResolver) : base(appender, prevKey)
        {
            this.resolver = localizationResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineLocalizationResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("LocalizationResolver", typeof(ILocalizationResolver)) as ILocalizationResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LocalizationResolver", resolver);
        }
    }

    public partial class LineAppender : ILineFactory<ILineLocalizationResolver, ILocalizationResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="localizationResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineLocalizationResolver, ILocalizationResolver>.TryCreate(ILineFactory appender, ILine previous, ILocalizationResolver localizationResolver, out ILineLocalizationResolver line)
        {
            line = new LineLocalizationResolver(appender, previous, localizationResolver);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries <see cref="ILineLocalizationResolver"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerLocalizationResolver : StringLocalizerBase, ILineLocalizationResolver, ILineArguments<ILocalizationResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected ILocalizationResolver resolver;

        /// <summary>
        /// ILineLocalizationResolver property
        /// </summary>
        public ILocalizationResolver Resolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public ILocalizationResolver Argument0 => resolver;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="localizationResolver"></param>
        public StringLocalizerLocalizationResolver(ILineFactory appender, ILine prevKey, ILocalizationResolver localizationResolver) : base(appender, prevKey)
        {
            this.resolver = localizationResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerLocalizationResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("LocalizationResolver", typeof(ILocalizationResolver)) as ILocalizationResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LocalizationResolver", resolver);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineLocalizationResolver, ILocalizationResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="localizationResolver"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        bool ILineFactory<ILineLocalizationResolver, ILocalizationResolver>.TryCreate(ILineFactory appender, ILine previous, ILocalizationResolver localizationResolver, out ILineLocalizationResolver StringLocalizer)
        {
            StringLocalizer = new StringLocalizerLocalizationResolver(appender, previous, localizationResolver);
            return true;
        }
    }


}
