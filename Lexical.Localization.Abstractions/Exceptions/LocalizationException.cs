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
    /// Error related to this class library
    /// </summary>
    public class LocalizationException : Exception
    {
        /// <summary>
        /// Create exception.
        /// </summary>
        public LocalizationException() :base(){ }

        /// <summary>
        /// Create exception.
        /// </summary>
        /// <param name="message"></param>
        public LocalizationException(string message) : base(message) { }

        /// <summary>
        /// Create exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public LocalizationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LocalizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
