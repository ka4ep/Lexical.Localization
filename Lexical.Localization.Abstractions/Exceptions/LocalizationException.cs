// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    public class LocalizationException : AssetException
    {
        public LocalizationException() :base(){ }
        public LocalizationException(string message) : base(message) { }
        public LocalizationException(string message, Exception innerException) : base(message, innerException) { }
        protected LocalizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
