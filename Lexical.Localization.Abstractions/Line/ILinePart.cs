// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILineAppendArguments : ILine
    {
        /// <summary>
        /// Get arguments that can be used to clone this part.
        /// The first element contains the <see cref="Type"/>, other elements the arguments.
        /// </summary>
        /// <return>Enumerable of append arguments, or null</return>
        IEnumerable<Object[]> GetAppendArguments()
    }

    /// <summary>
    /// A part that together with other parts composes a <see cref="ILine"/>.
    /// 
    /// Forms a linked list and trie of <see cref="ILinePart"/>s.
    /// </summary>
    public partial interface ILinePart : ILine, ILineAppendArguments
    {
        /// <summary>
        /// (Optional) Previous part.
        /// </summary>
        ILinePart PreviousPart { get; }

        /// <summary>
        /// (Optional) Part appender. If null, the caller should follow to <see cref="PreviousPart"/> for appender.
        /// </summary>
        ILinePartAppender Appender { get; }
    }

    /// <summary></summary>
    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Get previous part
        /// </summary>
        /// <param name="line"></param>
        /// <returns>part or null</returns>
        public static ILinePart GetPreviousPart(this ILine line)
            => line is ILinePart part ? part.PreviousPart : null;

        /// <summary>
        /// Get appender from <paramref name="part"/>.
        /// If its null, follows to previous part. 
        /// </summary>
        /// <param name="part"></param>
        /// <returns>appender or null</returns>
        public static ILinePartAppender GetAppender(this ILinePart part)
        {
            for(ILinePart p = part; p!=null; p=p.PreviousPart)
            {
                ILinePartAppender appender = p.Appender;
                if (appender != null) return appender;
            }
            return null;
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException">If appender is null</exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Part"/></exception>
        public static Part Append<Part>(this ILinePart part) where Part : ILinePart
            => part.GetAppender().Append<Part>(part);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0>(this ILinePart part, A0 a0) where Part : ILinePart
            => part.GetAppender().Append<Part, A0>(part, a0);

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
        /// <exception cref="LineException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1>(this ILinePart part, A0 a0, A1 a1) where Part : ILinePart
            => part.GetAppender().Append<Part, A0, A1>(part, a0, a1);

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
        /// <exception cref="LineException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1, A2>(this ILinePart part, A0 a0, A1 a1, A2 a2) where Part : ILinePart
            => part.GetAppender().Append<Part, A0, A1, A2>(part, a0, a1, a2);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part>(this ILinePart part) where Part : ILinePart
            => part.GetAppender().TryAppend<Part>(part);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0>(this ILinePart part, A0 a0) where Part : ILinePart
            => part.GetAppender().TryAppend<Part, A0>(part, a0);

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0, A1>(this ILinePart part, A0 a0, A1 a1) where Part : ILinePart
            => part.GetAppender().TryAppend<Part, A0, A1>(part, a0, a1);

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
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0, A1, A2>(this ILinePart part, A0 a0, A1 a1, A2 a2) where Part : ILinePart
            => part.GetAppender().TryAppend<Part, A0, A1, A2>(part, a0, a1, a2);

        /// <summary>
        /// Set new appender by appending a dummy <see cref="ILinePart"/> with the new <paramref name="appender"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="appender"></param>
        /// <returns>part with another appender</returns>
        public static ILinePart SetAppender(this ILinePart previous, ILinePartAppender appender)
            => appender.Append<ILinePart>(previous);

        /// <summary>
        /// Append <paramref name="anotherLine"/> to <paramref name="part"/>.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="anotherLine"></param>
        /// <returns></returns>
        public static ILinePart Append(this ILinePart part, ILine anotherLine = null)
        {
            ILinePartAppender appender = part.GetAppender();
            if (appender == null) throw new LineException();
            for (ILine l = anotherLine; l != null; l=l.GetPreviousPart())
            {
                if (l is ILineAppendArguments args)
                {
                    var enumr = args.GetAppendArguments();
                    if (enumr != null)
                    {
                        foreach(var arg in args)
                        {
                            part.GetAppender()
                        }
                    }
                }
            }
            return part;
        }
    }
}
