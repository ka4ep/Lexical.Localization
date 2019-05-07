// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// Error related to <see cref="IAsset"/>.
    /// </summary>
    public class AssetException : LocalizationException
    {
        /// <summary>
        /// Create exception
        /// </summary>
        public AssetException() { }

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="message"></param>
        public AssetException(string message) : base(message) { }

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AssetException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AssetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
