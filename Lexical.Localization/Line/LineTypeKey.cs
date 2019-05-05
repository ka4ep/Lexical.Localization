// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Runtime.Serialization;

namespace Lexical.Localization
{
    /// <summary>
    /// "Type" key that carries <see cref="Type"/>. 
    /// </summary>
    [Serializable]
    public class LineTypeKey : LineKey, ILineKeyType
    {
        /// <summary>
        /// Type, null if non-standard type.
        /// </summary>
        protected Type type;

        /// <summary>
        /// Type property
        /// </summary>
        public Type Type { get => type; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Create new type key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="type"></param>
        public LineTypeKey(ILinePartAppender appender, ILinePart prevKey, Type type) : base(appender, prevKey, "Type", type?.FullName)
        {
            this.type = type;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineTypeKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.type = info.GetValue("Type", typeof(Type)) as Type;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type", type);
        }
    }

    /// <summary>
    /// Create type key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LineTypeKey<T> : LineTypeKey, ILineKeyType, ILineKey<T>
    {
        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        public LineTypeKey(ILinePartAppender appender, ILinePart prevKey) : base(appender, prevKey, typeof(T))
        {
        }

        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineTypeKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class LinePartAppender : ILinePartAppender1<ILineKeyType, Type>
    {
        /// <summary>
        /// Constructor of runtime types.
        /// </summary>
        static RuntimeConstructor<ILinePartAppender, ILinePart, LineTypeKey> typeConstructor = new RuntimeConstructor<ILinePartAppender, ILinePart, LineTypeKey>(typeof(LineTypeKey<>));

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILineKeyType Append(ILinePart previous, Type type)
            => typeConstructor.Create(type, this, previous);
    }

    /*
    /// <summary>
    /// "Type" key that carries <see cref="Type"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerTypeKey : StringLocalizerKey, ILineKeyType
    {
        /// <summary>
        /// Type, null if non-standard type.
        /// </summary>
        protected Type type;

        /// <summary>
        /// Type property
        /// </summary>
        public Type Type { get => type; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Create new type key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="type"></param>
        public StringLocalizerTypeKey(ILinePartAppender appender, ILinePart prevKey, Type type) : base(appender, prevKey, "Type", type?.Name)
        {
            this.type = type;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerTypeKey(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.type = info.GetValue("Type", typeof(Type)) as Type;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type", type);
        }
    }

    public partial class StringLocalizerPartAppender : ILinePartAppender1<ILineKeyType, Type>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILineKeyType Append(ILinePart previous, Type type)
            => new StringLocalizerTypeKey(this, previous, type);
    }
*/
}
