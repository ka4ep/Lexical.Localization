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
    public interface ILocalizationKeyFormattable : ILinePart
    {
        /// <summary>
        /// Create a new ILocalizationKey with arguments attached.
        /// </summary>
        /// <param name="formatProvider">(optional) custom format provider</param>
        /// <param name="args">attach arguments</param>
        /// <returns>new key</returns>
        ILocalizationKeyFormatArgs Format(params object[] args);
    }

    /// <summary>
    /// Key (may have) has formats assigned.
    /// </summary>
    public interface ILocalizationKeyFormatArgs : ILinePart
    {
        /// <summary>
        /// Attached format arguments (may be null).
        /// </summary>
        Object[] Args { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Create a new <see cref="ILocalizationKeyFormatArgs"/> that has arguments attached.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="AssetKeyException">If key can't be formatted</exception>
        public static ILocalizationKeyFormatArgs Format(this ILinePart key, params object[] args)
            => key is ILocalizationKeyFormattable formattable ? formattable.Format(args) : throw new AssetKeyException(key, $"Key doesn't implement {nameof(ILocalizationKeyFormattable)}");

        /// <summary>
        /// Walks linked list and searches for culture policy.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture policy or null</returns>
        public static Object[] FindFormatArgs(this ILinePart key)
        {
            for (; key != null; key = key.PreviousPart)
                if (key is ILocalizationKeyFormatArgs casted && casted.Args != null) return casted.Args;
            return null;
        }

    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILocalizationKeyFormatArgs"/> values of keys.
    /// </summary>
    public class LocalizationKeyFormatArgsComparer : IEqualityComparer<ILinePart>
    {
        static IEqualityComparer<object[]> array_comparer = new ArrayComparer<object>(EqualityComparer<object>.Default);
        public bool Equals(ILinePart x, ILinePart y)
            => array_comparer.Equals(x?.FindFormatArgs(), y?.FindFormatArgs());
        public int GetHashCode(ILinePart obj)
            => array_comparer.GetHashCode(obj?.FindFormatArgs());
    }
}
