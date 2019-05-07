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
    /// Key has been set with <see cref="ICulturePolicy"/> hint.
    /// </summary>
    public interface ILineCulturePolicy : ILine
    {
        /// <summary>
        /// Selected culture policy, or null.
        /// </summary>
        ICulturePolicy CulturePolicy { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append culture policy
        /// </summary>
        /// <param name="line"></param>
        /// <param name="culturePolicy"></param>
        /// <returns></returns>
        public static ILineCulturePolicy CulturePolicy(this ILine line, ICulturePolicy culturePolicy)
            => line.Append<ILineCulturePolicy, ICulturePolicy>(culturePolicy);

        /// <summary>
        /// Walks linked list and searches for culture policy setting.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture policy or null</returns>
        public static ICulturePolicy FindCulturePolicy(this ILine line)
        {
            for (;line!=null; line=line.GetPreviousPart())
                if (line is ILineCulturePolicy cultureKey && cultureKey.CulturePolicy != null) return cultureKey.CulturePolicy;
            return null;
        }
    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILineCulturePolicy"/> values of keys.
    /// </summary>
    public class LineCulturePolicyComparer : IEqualityComparer<ILine>
    {
        static IEqualityComparer<ICulturePolicy> comparer = new ReferenceComparer<ICulturePolicy>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
            => comparer.Equals(x?.FindCulturePolicy(), y?.FindCulturePolicy());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(ILine obj)
            => comparer.GetHashCode(obj?.FindCulturePolicy());
    }

}
