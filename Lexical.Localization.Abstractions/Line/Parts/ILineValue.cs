// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Line's format argument assignments. Arguments are matched to placeholders in format string.
    /// 
    /// For example for string "Hello, {0}" the placeholder {0} is matched to argument at index 0.
    /// </summary>
    public interface ILineValue : ILine
    {
        /// <summary>
        /// Attached format arguments (may be null).
        /// </summary>
        Object[] Value { get; set;  }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append format arguments. Format arguments 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="LineException">If key can't be formatted</exception>
        public static ILineValue Value(this ILine line, params object[] args)
            => line.Append<ILineValue, Object[]>(args);

        /// <summary>
        /// Append format arguments. Format arguments 
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="LineException">If key can't be formatted</exception>
        public static ILineValue Value(this ILineFactory lineFactory, params object[] args)
            => lineFactory.Create<ILineValue, Object[]>(null, args);

        /// <summary>
        /// Walks linked list and searches for culture policy.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture policy or null</returns>
        public static Object[] FindValues(this ILine line)
        {
            if (line is ILineValue args) return args.Value;
            if (line is ILine tail)
            {
                for (ILine p = tail; p != null; p = p.GetPreviousPart())
                    if (p is ILineValue casted && casted.Value != null) return casted.Value;
            }
            return null;
        }

    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILineValue"/> values of keys.
    /// </summary>
    public class LineValueComparer : IEqualityComparer<ILine>
    {
        static IEqualityComparer<object[]> array_comparer = new ArrayComparer<object>(EqualityComparer<object>.Default);

        private static LineValueComparer instance = new LineValueComparer();

        /// <summary>
        /// Get the value arguments comparer.
        /// </summary>
        public static LineValueComparer Default => instance;

        /// <summary>
        /// Compare last format args value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
            => array_comparer.Equals(x?.FindValues(), y?.FindValues());

        /// <summary>
        /// Calculate hash of last format args value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
            => array_comparer.GetHashCode(line.FindValues());
    }
}
