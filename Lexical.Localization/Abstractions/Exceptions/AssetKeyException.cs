// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Asset
{
    public class AssetKeyException : AssetException
    {
        public readonly IAssetKey Key;

        public AssetKeyException(IAssetKey Key) { this.Key = Key; }
        public AssetKeyException(IAssetKey Key, string message) : base(message) { this.Key = Key; }
        public AssetKeyException(IAssetKey Key, string message, Exception innerException) : base(message, innerException) { this.Key = Key; }
        protected AssetKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Key = info.GetValue("Key", typeof(IAssetKey)) as IAssetKey;
        }
        public override string ToString()
            => $"{base.ToString()} ({Key.Name})";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            base.GetObjectData(info, context);
        }
    }
}
