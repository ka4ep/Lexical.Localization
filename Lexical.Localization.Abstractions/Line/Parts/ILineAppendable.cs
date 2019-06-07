// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    #region Interface
    /// <summary>
    /// A line where new <see cref="ILine"/> can be appended.
    /// </summary>
    public interface ILineAppendable : ILine
    {
        /// <summary>
        /// (Optional) part constructor. If null, the caller should follow to <see cref="ILinePart.PreviousPart"/> for appender.
        /// </summary>
        ILineFactory Appender { get; set; }
    }
    #endregion Interface

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Get appender from <paramref name="line"/>.
        /// If its null, follows to previous part. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>appender</returns>
        /// <exception cref="LineException">Error if appender is not available</exception>
        public static ILineFactory GetAppender(this ILine line)
        {
            for (ILine p = line; p != null; p = p.GetPreviousPart())
                if (p is ILineAppendable appendable && appendable.Appender != null) return appendable.Appender;
            throw new LineException(line, "Could not find appender.");
        }

        /// <summary>
        /// Find appender from <paramref name="line"/>.
        /// If its null, follows to previous part. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>appender or null</returns>
        public static ILineFactory FindAppender(this ILine line)
        {
            for (ILine p = line; p != null; p = p.GetPreviousPart())
                if (p is ILineAppendable appendable && appendable.Appender != null) return appendable.Appender;
            return null;
        }

        /// <summary>
        /// Get appender from <paramref name="part"/>.
        /// If its null, follows to previous part. 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="appender"></param>
        /// <returns>true if appender was found</returns>
        public static bool TryGetAppender(this ILine part, out ILineFactory appender)
        {
            for (ILine p = part; p != null; p = p.GetPreviousPart())
                if (p is ILineAppendable appendable && appendable.Appender != null) { appender = appendable.Appender; return true; }
            appender = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException">If appender is null</exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Intf"/></exception>
        public static Intf Append<Intf>(this ILine line) where Intf : ILine
            => line.GetAppender().Create<Intf>(line);

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Intf"/></exception>
        /// <returns>Part</returns>
        public static Intf Append<Intf, A0>(this ILine line, A0 a0) where Intf : ILine
            => line.GetAppender().Create<Intf, A0>(line, a0);

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Intf"/></exception>
        /// <returns>Part</returns>
        public static Intf Append<Intf, A0, A1>(this ILine line, A0 a0, A1 a1) where Intf : ILine
            => line.GetAppender().Create<Intf, A0, A1>(line, a0, a1);

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="a2">argument 2</param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LineException">If appended could not append <typeparamref name="Intf"/></exception>
        /// <returns>Part</returns>
        public static Intf Append<Intf, A0, A1, A2>(this ILine line, A0 a0, A1 a1, A2 a2) where Intf : ILine
            => line.GetAppender().Create<Intf, A0, A1, A2>(line, a0, a1, a2);

        /// <summary>
        /// Append new <see cref="ILine"/> with <paramref name="arguments"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="arguments"></param>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException">If appender is null</exception>
        /// <exception cref="LineException">If append failed</exception>
        public static ILine Append(this ILine line, ILineArguments arguments)
        {
            ILine result;
            if (line.GetAppender().TryCreate(line, arguments, out result)) return result;
            throw new LineException(arguments, $"Could not append.");
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <typeparam name="Intf"></typeparam>
        /// <returns>true if append succeeded</returns>
        public static bool TryAppend<Intf>(this ILine line, out Intf result) where Intf : ILine
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryCreate<Intf>(line, out result)) return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="result"></param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <returns>true if append succeeded</returns>
        public static bool TryAppend<Intf, A0>(this ILine line, A0 a0, out Intf result) where Intf : ILine
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryCreate<Intf, A0>(line, a0, out result)) return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="result"></param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <returns>true if append succeeded</returns>
        public static bool TryAppend<Intf, A0, A1>(this ILine line, A0 a0, A1 a1, out Intf result) where Intf : ILine
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryCreate<Intf, A0, A1>(line, a0, a1, out result)) return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="a2">argument 2</param>
        /// <param name="result"></param>
        /// <typeparam name="Intf"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <returns>true if append succeeded</returns>
        public static bool TryAppend<Intf, A0, A1, A2>(this ILine line, A0 a0, A1 a1, A2 a2, out Intf result) where Intf : ILine
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender) && appender.TryCreate<Intf, A0, A1, A2>(line, a0, a1, a2, out result)) return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Append new <see cref="ILine"/> with <paramref name="arguments"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="arguments"></param>
        /// <param name="result"></param>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException">If appender is null</exception>
        /// <exception cref="LineException">If append failed</exception>
        public static bool TryAppend(this ILine line, ILineArguments arguments, out ILine result)
            => line.GetAppender().TryCreate(line, arguments, out result);

        /// <summary>
        /// Set new appender by appending a dummy <see cref="ILinePart"/> with the new <paramref name="appender"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="appender"></param>
        /// <returns>part with another appender</returns>
        public static ILine SetAppender(this ILine previous, ILineFactory appender)
            => appender.Create<ILinePart>(previous);

        /// <summary>
        /// Concatenate <paramref name="anotherLine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line">part to append to</param>
        /// <param name="anotherLine"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static ILine Concat(this ILine line, ILine anotherLine)
            => line.GetAppender().Concat(line, anotherLine);

        /// <summary>
        /// Concatenate <paramref name="anotherLine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line">part to append to</param>
        /// <param name="anotherLine"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static ILine ConcatIfNew(this ILine line, ILine anotherLine)
            => line.GetAppender().ConcatIfNew(line, anotherLine);

        /// <summary>
        /// Append <paramref name="anotherLine"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line">part to append to</param>
        /// <param name="anotherLine"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="LineException">on append error</exception>
        public static bool TryConcat(this ILine line, ILine anotherLine, out ILine result)
        {
            ILineFactory appender;
            if (line.TryGetAppender(out appender)) return appender.TryConcat(line, anotherLine, out result);
            result = null;
            return false;
        }


    }
}
