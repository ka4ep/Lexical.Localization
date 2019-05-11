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
    public class LineType : LineKey, ILineType, ILineNonCanonicalKey, ILineArguments<ILineType, Type>
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
        /// Get construction argument.
        /// </summary>
        public Type Argument0 => type;

        /// <summary>
        /// Create new type key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="type"></param>
        public LineType(ILineFactory appender, ILine prevKey, Type type) : base(appender, prevKey, "Type", type?.FullName)
        {
            this.type = type;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineType(SerializationInfo info, StreamingContext context) : base(info, context)
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
    public class LineType<T> : LineType, ILineType, ILine<T>
    {
        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        public LineType(ILineFactory appender, ILine prevKey) : base(appender, prevKey, typeof(T))
        {
        }

        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public LineType(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class LineAppender : ILineFactory<ILineType, Type>
    {
        /// <summary>
        /// Constructor of runtime types.
        /// </summary>
        static RuntimeConstructor<ILineFactory, ILine, LineType> typeConstructor = new RuntimeConstructor<ILineFactory, ILine, LineType>(typeof(LineType<>));

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool ILineFactory<ILineType, Type>.TryCreate(ILineFactory appender, ILine previous, Type type, out ILineType result)
        {
            result = typeConstructor.Create(type, appender, previous);
            return true;
        }

        /// <summary>
        /// Address ILine{T} request.
        /// </summary>
        protected override void PostConstruction() => Add(new TypeAppender());

        class TypeAppender : ILineFactoryCastable
        {
            static RuntimeConstructor<TypeAppender> typeConstructor = new RuntimeConstructor<TypeAppender>(typeof(TypeAppender<>));
            public ILineFactory<Part> Cast<Part>() where Part : ILine
            {
                if (typeof(Part).GetGenericTypeDefinition() == typeof(ILine<>))
                {
                    Type[] args = typeof(Part).GetGenericArguments();
                    if (args != null && args.Length == 1) return (ILineFactory<Part>)typeConstructor.Create(args[0]);
                }
                return null;
            }
            public ILineFactory<Part, A0> Cast<Part, A0>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1> Cast<Part, A0, A1>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1, A2> Cast<Part, A0, A1, A2>() where Part : ILine => null;
        }

        class TypeAppender<T> : ILineFactory<ILine<T>>
        {
            public bool TryCreate(ILineFactory appender, ILine previous, out ILine<T> result)
            {
                result = new LineType<T>(appender, previous);
                return true;
            }
        }
    }

    /// <summary>
    /// "Type" key that carries <see cref="Type"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerType : _StringLocalizerKey, ILineType, ILineNonCanonicalKey, ILineArguments<ILineType, Type>
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
        /// Get construction argument.
        /// </summary>
        public Type Argument0 => type;

        /// <summary>
        /// Create new type key.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="type"></param>
        public StringLocalizerType(ILineFactory appender, ILine prevKey, Type type) : base(appender, prevKey, "Type", type?.FullName)
        {
            this.type = type;
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerType(SerializationInfo info, StreamingContext context) : base(info, context)
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
    public class StringLocalizerType<T> : StringLocalizerType, ILineType, ILine<T>
    {
        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        public StringLocalizerType(ILineFactory appender, ILine prevKey) : base(appender, prevKey, typeof(T))
        {
        }

        /// <summary>
        /// Create type key
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public StringLocalizerType(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineType, Type>
    {
        /// <summary>
        /// Constructor of runtime types.
        /// </summary>
        static RuntimeConstructor<ILineFactory, ILine, StringLocalizerType> typeConstructor = new RuntimeConstructor<ILineFactory, ILine, StringLocalizerType>(typeof(StringLocalizerType<>));

        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool ILineFactory<ILineType, Type>.TryCreate(ILineFactory appender, ILine previous, Type type, out ILineType result)
        {
            result = typeConstructor.Create(type, appender, previous);
            return true;
        }

        /// <summary>
        /// Address ILine{T} request.
        /// </summary>
        protected override void PostConstruction() => Add(new TypeAppender());

        class TypeAppender : ILineFactoryCastable
        {
            static RuntimeConstructor<TypeAppender> typeConstructor = new RuntimeConstructor<TypeAppender>(typeof(TypeAppender<>));
            public ILineFactory<Part> Cast<Part>() where Part : ILine
            {
                if (typeof(Part).GetGenericTypeDefinition() == typeof(ILine<>))
                {
                    Type[] args = typeof(Part).GetGenericArguments();
                    if (args != null && args.Length == 1) return (ILineFactory<Part>)typeConstructor.Create(args[0]);
                }
                return null;
            }
            public ILineFactory<Part, A0> Cast<Part, A0>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1> Cast<Part, A0, A1>() where Part : ILine => null;
            public ILineFactory<Part, A0, A1, A2> Cast<Part, A0, A1, A2>() where Part : ILine => null;
        }

        class TypeAppender<T> : ILineFactory<ILine<T>>
        {
            public bool TryCreate(ILineFactory appender, ILine previous, out ILine<T> result)
            {
                result = new StringLocalizerType<T>(appender, previous);
                return true;
            }
        }
    }

}
