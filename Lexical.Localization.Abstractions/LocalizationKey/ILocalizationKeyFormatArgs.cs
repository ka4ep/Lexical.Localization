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
    /// Key has capability of format arguments assignment.
    /// </summary>
    [Obsolete]
    public interface ILocalizationKeyFormattable : ILinePart
    {
        /// <summary>
        /// Create a new ILocalizationKey with arguments attached.
        /// </summary>
        /// <param name="formatProvider">(optional) custom format provider</param>
        /// <param name="args">attach arguments</param>
        /// <returns>new key</returns>
        ILineFormatArgsPart Format(params object[] args);
    }

    /// <summary>
    /// Key (may have) has formats assigned.
    /// </summary>
    public interface ILineFormatArgs : ILine
    {
        /// <summary>
        /// Attached format arguments (may be null).
        /// </summary>
        Object[] Args { get; }
    }

    /// <summary>
    /// Key (may have) has formats assigned.
    /// </summary>
    public interface ILineFormatArgsPart : ILineFormatArgs, ILinePart
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Create a new <see cref="ILineFormatArgsPart"/> that has arguments attached.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="LineException">If key can't be formatted</exception>
        public static ILineFormatArgsPart Format(this ILinePart key, params object[] args)
            => key is ILocalizationKeyFormattable formattable ? formattable.Format(args) : throw new LineException(key, $"Key doesn't implement {nameof(ILocalizationKeyFormattable)}");

        /// <summary>
        /// Walks linked list and searches for culture policy.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture policy or null</returns>
        public static Object[] GetFormatArgs(this ILine line)
        {
            if (line is ILineFormatArgs args) return args.Args;
            if (line is ILinePart tail)
            {
                for (ILinePart p = tail; p != null; p = p.PreviousPart)
                    if (p is ILineFormatArgsPart casted && casted.Args != null) return casted.Args;
            }
            return null;
        }

    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILineFormatArgsPart"/> values of keys.
    /// </summary>
    public class LocalizationKeyFormatArgsComparer : IEqualityComparer<ILine>
    {
        static IEqualityComparer<object[]> array_comparer = new ArrayComparer<object>(EqualityComparer<object>.Default);

        /// <summary>
        /// Compare last format args value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
            => array_comparer.Equals(x?.GetFormatArgs(), y?.GetFormatArgs());

        /// <summary>
        /// Calculate hash of last format args value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetHashCode(ILine line)
            => array_comparer.GetHashCode(line.GetFormatArgs());
    }
}
