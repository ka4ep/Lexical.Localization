// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Binary;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineBinaryResolver"/>. 
    /// </summary>
    [Serializable]
    public class LineBinaryResolver : LineBase, ILineBinaryResolver, ILineArgument<IBinaryResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IBinaryResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IBinaryResolver BinaryResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IBinaryResolver Argument0 => resolver;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="binaryresolver"></param>
        public LineBinaryResolver(ILineFactory appender, ILine prevKey, IBinaryResolver binaryresolver) : base(appender, prevKey)
        {
            this.resolver = binaryresolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineBinaryResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("BinaryResolver", typeof(IBinaryResolver)) as IBinaryResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("BinaryResolver", resolver);
        }
    }

    public partial class LineAppender : ILineFactory<ILineBinaryResolver, IBinaryResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="binaryResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IBinaryResolver binaryResolver, out ILineBinaryResolver line)
        {
            line = new LineBinaryResolver(appender, previous, binaryResolver);
            return true;
        }
    }

    /// <summary>
    /// ResourceLocalizer part that carries <see cref="ILineBinaryResolver"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerBinaryResolver : StringLocalizerBase, ILineBinaryResolver, ILineArgument<IBinaryResolver>
    {
        /// <summary>
        /// Localization resolver.
        /// </summary>
        protected IBinaryResolver resolver;

        /// <summary>
        /// ILineLineResolver property
        /// </summary>
        public IBinaryResolver BinaryResolver { get => resolver; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IBinaryResolver Argument0 => resolver;

        /// <summary>
        /// Create new ResourceLocalizer part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="binaryResolver"></param>
        public StringLocalizerBinaryResolver(ILineFactory appender, ILine prevKey, IBinaryResolver binaryResolver) : base(appender, prevKey)
        {
            this.resolver = binaryResolver;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerBinaryResolver(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.resolver = info.GetValue("BinaryResolver", typeof(IBinaryResolver)) as IBinaryResolver;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("BinaryResolver", resolver);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineBinaryResolver, IBinaryResolver>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="binaryResolver"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IBinaryResolver binaryResolver, out ILineBinaryResolver line)
        {
            line = new StringLocalizerBinaryResolver(appender, previous, binaryResolver);
            return true;
        }
    }


}
