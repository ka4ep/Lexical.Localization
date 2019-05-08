// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
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
        /// Add "Type" as <see cref="ILineNonCanonicalKey"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="typeName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineNonCanonicalKey"/></exception>
        public static ILineNonCanonicalKey Type(this ILine part, string typeName)
            => part.Append<ILineNonCanonicalKey, string, string>("Type", typeName);

        /// <summary>
        /// Add <see cref="ILineType"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineType"/></exception>
        public static ILineType Type(this ILine part, Type type)
            => part.Append<ILineType, Type>(type);

        /// <summary>
        /// Add <see cref="ILine{T}"/> key.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>new key</returns>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine<T> Type<T>(this ILine part)
            => (ILine<T>)part.Append<ILineType, Type>(typeof(T));
        
        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineType"/> key or <see cref="ILineParameter"/> key with "Type".
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>key or null</returns>
        public static ILine GetTypeKey(this ILine tail)
        {
            ILine result = null;
            for (ILine part = tail; tail != null; tail = tail.GetPreviousPart())
            {
                if (part is ILineType asmKey && asmKey.Type != null) result = asmKey;
                else if (part is ILineParameter parameterKey && parameterKey.ParameterName == "Type" && parameterKey.ParameterValue != null) result = parameterKey;
            }
            return result;
        }

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILineType"/> key.
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
