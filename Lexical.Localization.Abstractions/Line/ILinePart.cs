// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// A part that together with other parts composes a <see cref="ILine"/>.
    /// 
    /// Forms a linked list and trie of <see cref="ILinePart"/>s.
    /// </summary>
    public interface ILinePart : ILine
    {
        /// <summary>
        /// Previous part.
        /// </summary>
        ILinePart PreviousPart { get; }

        /// <summary>
        /// Part appender
        /// </summary>
        ILinePartAppender Appender { get; }
    }

    /// <summary></summary>
    public static partial class LinePartExtensions
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException">If appender is null</exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        public static Part Append<Part>(this ILinePart part) where Part : ILinePart
            => part.Appender.Append<Part>(part);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0>(this ILinePart part, A0 a0) where Part : ILinePart
            => part.Appender.Append<Part, A0>(part, a0);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1>(this ILinePart part, A0 a0, A1 a1) where Part : ILinePart
            => part.Appender.Append<Part, A0, A1>(part, a0, a1);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="a2">argument 2</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1, A2>(this ILinePart part, A0 a0, A1 a1, A2 a2) where Part : ILinePart
            => part.Appender.Append<Part, A0, A1, A2>(part, a0, a1, a2);

    }
}
