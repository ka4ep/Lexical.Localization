// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    public class AssetException : Exception
    {
        public AssetException() { }
        public AssetException(string message) : base(message) { }
        public AssetException(string message, Exception innerException) : base(message, innerException) { }
        protected AssetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
