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
    /// Consumers of this interface should use the extension method <see cref="ILinePartExtensions.Type(ILinePart, string)"/> and others.
    /// </summary>
    [Obsolete]
    public interface IAssetKeyTypeAssignable : ILinePart
    {
        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="name">type</param>
        /// <returns>new key</returns>
        ILineKeyType Type(string name);

        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>new key</returns>
        ILineKeyType Type(Type type);

        /// <summary>
        /// Create type section for specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>new key</returns>
        ILineKey<T> Type<T>();
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

    /// <summary>
    /// Line key part with "Type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// </summary>
    public interface ILineKeyType : ILineType, ILineKeyNonCanonicallyCompared
    {
    }

    /// <summary>
    /// Type section for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILineKey<T> : ILine<T>, ILineKeyType
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Add <see cref="ILineKeyNonCanonicallyCompared"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="typeName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineKeyNonCanonicallyCompared"/></exception>
        public static ILineKeyNonCanonicallyCompared Type(this ILinePart part, string typeName)
            => part.Append<ILineKeyNonCanonicallyCompared, string, string>("Type", typeName);

        /// <summary>
        /// Add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineKeyType"/></exception>
        public static ILineKeyType Type(this ILinePart part, Type type)
            => part.Append<ILineKeyType, Type>(type);

        /// <summary>
        /// Add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>new key</returns>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="LineException">If key doesn't implement <see cref="IAssetKeyTypeAssignable"/></exception>
        public static ILineKey<T> Type<T>(this ILinePart part)
            => part.Append<ILineKeyType, Type>(typeof(T)) as ILineKey<T> ?? part.Append<ILineKey<T>>();

        /// <summary>
        /// Try to add <see cref="ILineKeyType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static ILineKeyType TryAppendType(this ILinePart part, Type type)
            => part.TryAppend<ILineKeyType, Type>(type);

        /// <summary>
        /// Try to add <see cref="ILineKeyNonCanonicallyCompared"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="typeName"></param>
        /// <returns>new key or null</returns>
        public static ILineKey TryAppendType(this ILinePart part, string typeName)
            => part.TryAppend<ILineKeyNonCanonicallyCompared, string, string>("Type", typeName);

        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineKeyType"/> key or <see cref="ILineParameter"/> key with "Type".
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>key or null</returns>
        public static ILinePart GetTypeKey(this ILinePart tail)
        {
            ILinePart result = null;
            for (ILinePart part = tail; tail != null; tail = tail.PreviousPart)
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
            if (line is ILineType lineType && lineType.Type != null) return lineType.Type;

            if (line is ILinePart part)
            {
                Type type = null;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                    if (p is ILineKeyType typeKey && typeKey.Type != null) type = typeKey.Type;
                if (type != null) return type;
            }

            return null;
        }

        /// <summary>
        /// Get effective (closest root) type value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type name or null</returns>
        public static string GetTypeName(this ILine line)
        {
            if (line is ILineType lineType && lineType.Type != null) return lineType.Type.FullName;

            if (line is ILineParameters lineParameters)
            {
                var keys = lineParameters.Parameters;
                if (keys != null)
                    foreach (var kv in keys)
                        if (kv.Key == "Type" && kv.Value != null) return kv.Value;
            }

            if (line is ILinePart part)
            {
                string result = null;
                for (ILinePart p = part; p != null; p = p.PreviousPart)
                {
                    if (p is ILineKeyType typeKey && typeKey.Type != null) result = typeKey.Type.FullName;
                    else if (p is ILineParameter parameterKey && parameterKey.ParameterName == "Type" && parameterKey.ParameterValue != null) result = parameterKey.ParameterValue;
                }
                if (result != null) return result;
            }

            return null;
        }


    }
}
