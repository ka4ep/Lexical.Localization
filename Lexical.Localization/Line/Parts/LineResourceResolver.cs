// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineResourceResolver"/>. 
    /// </summary>
    [Serializable]
    public class LineResourceResolver : LineBase, ILineResourceResolver, ILineArgument<IResourceResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IResourceResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IResourceResolver ResourceResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IResourceResolver Argument0 => resolver;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public LineResourceResolver(ILineFactory appender, ILine prevKey, IResourceResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineResourceResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("ResourceResolver", typeof(IResourceResolver)) as IResourceResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ResourceResolver", resolver);
        }
    }

    public partial class LineAppender : ILineFactory<ILineResourceResolver, IResourceResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IResourceResolver LineResolver, out ILineResourceResolver line)
        {
            line = new LineResourceResolver(appender, previous, LineResolver);
            return true;
        }
    }

    /// <summary>
    /// ResourceLocalizer part that carries <see cref="ILineResourceResolver"/>. 
    /// </summary>
    [Serializable]
    public class ResourceLocalizerLineResolver : StringLocalizerBase, ILineResourceResolver, ILineArgument<IResourceResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IResourceResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IResourceResolver ResourceResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IResourceResolver Argument0 => resolver;

        /// <summary>
        /// Create new ResourceLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="LineResolver"></param>
        public ResourceLocalizerLineResolver(ILineFactory appender, ILine prevKey, IResourceResolver LineResolver) : base(appender, prevKey)
        {
            this.resolver = LineResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ResourceLocalizerLineResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("ResourceResolver", typeof(IResourceResolver)) as IResourceResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ResourceResolver", resolver);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineResourceResolver, IResourceResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="LineResolver"></param>
        /// <param name="ResourceLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IResourceResolver LineResolver, out ILineResourceResolver ResourceLocalizer)
        {
            ResourceLocalizer = new ResourceLocalizerLineResolver(appender, previous, LineResolver);
            return true;
        }
    }


}
