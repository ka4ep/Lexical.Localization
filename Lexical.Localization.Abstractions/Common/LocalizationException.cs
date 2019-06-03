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

    /// <summary>
    /// Error related to <see cref="ILine"/>.
    /// </summary>
    public class LineException : LocalizationException
    {
        /// <summary>
        /// Associated line
        /// </summary>
        public readonly ILine Line;

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="line"></param>
        public LineException(ILine line) { this.Line = line; }

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        public LineException(ILine line, string message) : base(message) { this.Line = line; }

        /// <summary>
        /// Create exception
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public LineException(ILine line, string message, Exception innerException) : base(message, innerException) { this.Line = line; }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Line = info.GetValue(nameof(Line), typeof(ILine)) as ILine;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Line), Line);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Print debug info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Message == null ? $"{base.ToString()} ({Line})" : $"{base.ToString()} ({Line}: {Message})";
    }

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
