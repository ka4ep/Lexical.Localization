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
    /// Line part that carries <see cref="ILineStringResolverPart"/>. 
    /// </summary>
    [Serializable]
    public class LineStringResolverPart : LineBase, ILineStringResolverPart, ILineArguments<ILineStringResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected ILineStringResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public ILineStringResolver Resolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public ILineStringResolver Argument0 => resolver;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public LineStringResolverPart(ILineFactory appender, ILine prevKey, ILineStringResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineStringResolverPart(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("LineResolver", typeof(ILineStringResolver)) as ILineStringResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LineResolver", resolver);
        }
    }

    public partial class LineAppender : ILineFactory<ILineStringResolverPart, ILineStringResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineStringResolverPart, ILineStringResolver>.TryCreate(ILineFactory appender, ILine previous, ILineStringResolver LineResolver, out ILineStringResolverPart line)
        {
            line = new LineStringResolverPart(appender, previous, LineResolver);
            return true;
        }
    }

    /// <summary>
    /// StringLocalizer part that carries <see cref="ILineStringResolverPart"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerLineResolver : StringLocalizerBase, ILineStringResolverPart, ILineArguments<ILineStringResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected ILineStringResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public ILineStringResolver Resolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public ILineStringResolver Argument0 => resolver;

        /// <summary>
        /// Create new StringLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public StringLocalizerLineResolver(ILineFactory appender, ILine prevKey, ILineStringResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerLineResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("LineResolver", typeof(ILineStringResolver)) as ILineStringResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("LineResolver", resolver);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineStringResolverPart, ILineStringResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        bool ILineFactory<ILineStringResolverPart, ILineStringResolver>.TryCreate(ILineFactory appender, ILine previous, ILineStringResolver LineResolver, out ILineStringResolverPart StringLocalizer)
        {
            StringLocalizer = new StringLocalizerLineResolver(appender, previous, LineResolver);
            return true;
        }
    }


}
