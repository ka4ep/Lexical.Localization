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
        /// <param name="line"></param>
        /// <param name="typeName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineNonCanonicalKey"/></exception>
        public static ILineNonCanonicalKey Type(this ILine line, string typeName)
            => line.Append<ILineNonCanonicalKey, string, string>("Type", typeName);

        /// <summary>
        /// Add <see cref="ILineType"/> key.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineType"/></exception>
        public static ILineType Type(this ILine line, Type type)
            => line.Append<ILineType, Type>(type);

        /// <summary>
        /// Add <see cref="ILine{T}"/> key.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>new key</returns>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine<T> Type<T>(this ILine line)
            => (ILine<T>)line.Append<ILineType, Type>(typeof(T));

        /// <summary>
        /// Create "Type" as <see cref="ILineNonCanonicalKey"/> key.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="typeName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineNonCanonicalKey"/></exception>
        public static ILineNonCanonicalKey Type(this ILineFactory lineFactory, string typeName)
            => lineFactory.Create<ILineNonCanonicalKey, string, string>(null, "Type", typeName);

        /// <summary>
        /// Create <see cref="ILineType"/> key.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILineType"/></exception>
        public static ILineType Type(this ILineFactory lineFactory, Type type)
            => lineFactory.Create<ILineType, Type>(null, type);

        /// <summary>
        /// Create <see cref="ILine{T}"/> key.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <returns>new key</returns>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine<T> Type<T>(this ILineFactory lineFactory)
            => (ILine<T>)lineFactory.Create<ILineType, Type>(null, typeof(T));

        /// <summary>
        /// Get the effective (closest to root) non-null <see cref="ILineType"/> key or <see cref="ILineParameter"/> key with "Type".
        /// </summary>
        /// <param name="line"></param>
        /// <returns>key or null</returns>
        public static ILine GetTypeKey(this ILine line)
        {
            ILine result = null;
            for (ILine part = line; line != null; line = line.GetPreviousPart())
            {
                if (part is ILineType asmKey && asmKey.Type != null) result = asmKey;
                if (part is ILineParameter parameterKey && parameterKey.ParameterName == "Type" && parameterKey.ParameterValue != null) result = part;
                if (part is ILineParameterEnumerable lineParameters)
                    foreach (var lineParameter in lineParameters)
                        if (lineParameter.ParameterName == "Type" && lineParameter.ParameterValue != null) { result = part; break; }
            }
            return result;
        }

        /// <summary>
        /// Search linked list and finds the effective (left-most) <see cref="ILineType"/> key.
        /// 
        /// Returns parameter "Type" value as <see cref="System.Type"/>, if <paramref name="resolver"/> is provided.
        /// 
        /// If implements <see cref="ILineType"/> returns the type. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver">(optional) type resolver that resolves "Type" parameter into type. Returns null, if could not resolve, exception if resolve fails</param>
        /// <returns>type info or null</returns>
        /// <exception cref="Exception">from <paramref name="resolver"/></exception>
        public static Type GetType(this ILine line, Func<String, Type> resolver = null)
        {
            Type type = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineType part && part.Type != null) { type = part.Type; continue; }
                if (resolver != null && l is ILineParameterEnumerable lineParameters)
                {
                    Type tt = null;
                    foreach (ILineParameter parameter in lineParameters)
                    {
                        if (parameter.ParameterName == "Type" && parameter.ParameterValue != null)
                        {
                            tt = resolver(parameter.ParameterValue);
                            if (tt != null) break;
                        }
                    }
                    if (tt != null) { type = tt; continue; }
                }
                if (resolver != null && l is ILineParameter lineParameter && lineParameter.ParameterName == "Type" && lineParameter.ParameterValue != null)
                {
                    Type t = resolver(lineParameter.ParameterValue);
                    if (t != null) type = t;
                }
            }
            return type;
        }

        /// <summary>
        /// Get effective (closest to root) type name.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>type name or null</returns>
        public static string GetTypeName(this ILine line)
        {
            string result = null;
            for (ILine part = line; part != null; part = part.GetPreviousPart())
            {
                if (part is ILineParameterEnumerable lineParameters)
                {
                    foreach (ILineParameter lineParameter in lineParameters)
                        if (lineParameter.ParameterName == "Type" && lineParameter.ParameterValue != null) { result = lineParameter.ParameterValue; break; }
                }
                else if (part is ILineParameter parameter && parameter.ParameterName == "Type" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                else if (part is ILineType key && key.Type != null) result = key.Type.FullName;
            }
            return result;
        }


    }
}
