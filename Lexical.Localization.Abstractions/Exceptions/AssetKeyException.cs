// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
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
            => Message == null ? $"{base.ToString()} ({Key.Name})" : $"{base.ToString()} ({Key.Name}: {Message})";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            base.GetObjectData(info, context);
        }
    }
}
