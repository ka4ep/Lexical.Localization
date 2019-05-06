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
    public class LineTypeKey : LineKey, ILineKeyType, ILineKeyNonCanonicallyCompared
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
        /// Appending arguments.
        /// </summary>
        public override object[] GetAppendArguments() => new object[] { Tuple.Create<Type, Type>(typeof(ILineKeyType), Type) };

        /// <summary>
        /// Create new type key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="type"></param>
        public LineTypeKey(ILineFactory appender, ILine prevKey, Type type) : base(appender, prevKey, "Type", type?.FullName)
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
        /// Appending arguments.
        /// </summary>
        public override object[] GetAppendArguments() => new object[] { Tuple.Create<Type>(typeof(ILineKey<T>)) };

        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        public LineTypeKey(ILineFactory appender, ILine prevKey) : base(appender, prevKey, typeof(T))
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

    public partial class LinePartAppender : ILineFactory<ILineKeyType, Type>
    {
        /// <summary>
        /// Constructor of runtime types.
        /// </summary>
        static RuntimeConstructor<ILineFactory, ILine, LineTypeKey> typeConstructor = new RuntimeConstructor<ILineFactory, ILine, LineTypeKey>(typeof(LineTypeKey<>));

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ILineKeyType ILineFactory<ILineKeyType, Type>.Create(ILineFactory appender, ILine previous, Type type)
            => typeConstructor.Create(type, appender, previous);

        /// <summary>
        /// 
        /// </summary>
        protected override void PostConstruction() => Add(new TypeAppender());

        class TypeAppender : ILineFactoryCastable
        {
            public ILineFactory<Part> Cast<Part>() where Part : ILine
            {
                if (typeof(Part).GetGenericTypeDefinition() == typeof(ILineKey<>))
                {
                    Type[] args = typeof(Part).GetGenericArguments();
                    if (args != null && args.Length == 1)
                        return (ILineFactory<Part>) Activator.CreateInstance(typeof(TypeAppender).MakeGenericType(args));
                }
                return null;
            }
            public ILineFactory<Part, A0> Cast<Part, A0>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1> Cast<Part, A0, A1>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1, A2> Cast<Part, A0, A1, A2>() where Part : ILine => null;
        }

        class TypeAppender<T> : ILineFactory<ILineKey<T>>
        {
            public ILineKey<T> Create(ILineFactory appender, ILine previous) => new LineTypeKey<T>(appender, previous);
        }
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
        public StringLocalizerTypeKey(ILineFactory appender, ILine prevKey, Type type) : base(appender, prevKey, "Type", type?.Name)
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

    public partial class StringLocalizerPartAppender : ILineFactory1<ILineKeyType, Type>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILineKeyType Append(ILine previous, Type type)
            => new StringLocalizerTypeKey(this, previous, type);
    }
*/
}
