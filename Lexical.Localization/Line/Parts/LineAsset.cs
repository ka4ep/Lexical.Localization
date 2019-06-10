// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Line part that carries <see cref="ILineAsset"/>. 
    /// </summary>
    [Serializable]
    public class LineAsset : LineBase, ILineAsset, ILineArgument<IAsset>
    {
        /// <summary>
        /// Asset.
        /// </summary>
        protected IAsset asset;

        /// <summary>
        /// ILineAsset property
        /// </summary>
        public IAsset Asset { get => asset; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IAsset Argument0 => asset;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="asset"></param>
        public LineAsset(ILineFactory appender, ILine prevKey, IAsset asset) : base(appender, prevKey)
        {
            this.asset = asset;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineAsset(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.asset = info.GetValue("Asset", typeof(IAsset)) as IAsset;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Asset", asset);
        }
    }

    public partial class LineAppender : ILineFactory<ILineAsset, IAsset>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="asset"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IAsset asset, out ILineAsset line)
        {
            line = new LineAsset(appender, previous, asset);
            return true;
        }
    }

    /// <summary>
    /// Line part that carries <see cref="ILineAsset"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerLineAsset : StringLocalizerBase, ILineAsset, ILineArgument<IAsset>
    {
        /// <summary>
        /// Asset.
        /// </summary>
        protected IAsset asset;

        /// <summary>
        /// ILineAsset property
        /// </summary>
        public IAsset Asset { get => asset; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IAsset Argument0 => asset;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="asset"></param>
        public StringLocalizerLineAsset(ILineFactory appender, ILine prevKey, IAsset asset) : base(appender, prevKey)
        {
            this.asset = asset;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerLineAsset(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.asset = info.GetValue("Asset", typeof(IAsset)) as IAsset;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Asset", asset);
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineAsset, IAsset>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="asset"></param>
        /// <param name="StringLocalizer"></param>
        /// <returns></returns>
        public virtual bool TryCreate(ILineFactory appender, ILine previous, IAsset asset, out ILineAsset StringLocalizer)
        {
            StringLocalizer = new StringLocalizerLineAsset(appender, previous, asset);
            return true;
        }
    }

}
