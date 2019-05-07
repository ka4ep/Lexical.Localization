// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// 
    /// Consumers of this interface should use the extension method <see cref="ILineExtensions.Type(ILine, string)"/> and others.
    /// </summary>
    [Obsolete]
    public interface IAssetKeyTypeAssignable : ILine
    {
        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="name">type</param>
        /// <returns>new key</returns>
        ILineType Type(string name);

        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>new key</returns>
        ILineType Type(Type type);

        /// <summary>
        /// Create type section for specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>new key</returns>
        ILine<T> Type<T>();
    }

    /// <summary>
    /// Line with <see cref="Type"/> assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// </summary>
    public interface ILineType : ILine
    {
        /// <summary>
        /// Type, or null.
        /// </summary>
        Type Type { get; set; }
    }

    /// <summary>
    /// Line with <typeparamref name="T"/> type assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// </summary>
    /// <typeparam name="T"/>
    public interface ILine<T> : ILine, ILineType
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Add <see cref="ILineKeyNonCanonicallyCompared"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="typeName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineKeyNonCanonicallyCompared"/></exception>
        public static ILineKeyNonCanonicallyCompared Type(this ILine part, string typeName)
            => part.Append<ILineKeyNonCanonicallyCompared, string, string>("Type", typeName);

        /// <summary>
        /// Add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineKeyType"/></exception>
        public static ILineKeyType Type(this ILine part, Type type)
            => part.Append<ILineKeyType, Type>(type);

        /// <summary>
        /// Add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>new key</returns>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="LineException">If key doesn't implement <see cref="IAssetKeyTypeAssignable"/></exception>
        public static ILineKey<T> Type<T>(this ILine part)
            => part.Append<ILineKeyType, Type>(typeof(T)) as ILineKey<T> ?? part.Append<ILineKey<T>>();

        /// <summary>
        /// Try to add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static ILineKeyType TryAppendType(this ILine part, Type type)
            => part.TryAppend<ILineKeyType, Type>(type);

        /// <summary>
        /// Try to add <see cref="ILineKeyNonCanonicallyCompared"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="typeName"></param>
        /// <returns>new key or null</returns>
        public static ILineKey TryAppendType(this ILine part, string typeName)
            => part.TryAppend<ILineKeyNonCanonicallyCompared, string, string>("Type", typeName);

        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineKeyType"/> key or <see cref="ILineParameter"/> key with "Type".
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>key or null</returns>
        public static ILine GetTypeKey(this ILine tail)
        {
            ILine result = null;
            for (ILine part = tail; tail != null; tail = tail.GetPreviousPart())
            {
                if (part is ILineKeyType asmKey && asmKey.Type != null) result = asmKey;
                else if (part is ILineParameter parameterKey && parameterKey.ParameterName == "Type" && parameterKey.ParameterValue != null) result = parameterKey;
            }
            return result;
        }

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILineKeyType"/> key.
        /// 
        /// If implements <see cref="ILineType"/> returns the type. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type info or null</returns>
        public static Type GetType(this ILine line)
        {
            Type type = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineType typeKey && typeKey.Type != null) type = typeKey.Type;
            }
            return type;
        }

        /// <summary>
        /// Get effective (closest to root) type value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type name or null</returns>
        public static string GetTypeName(this ILine line)
        {
            string result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineType typeKey && typeKey.Type != null) result = typeKey.Type.FullName;
                else if (l is ILineParameter parameter && parameter.ParameterName == "Type" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                else if (line is ILineParameterEnumerable lineParameters)
                {
                    var keys = lineParameters.Parameters;
                    if (keys != null)
                        foreach (var kv in keys)
                            if (kv.Key == "Type" && kv.Value != null) return kv.Value;
                }
            }
            return result;
        }


    }
}
