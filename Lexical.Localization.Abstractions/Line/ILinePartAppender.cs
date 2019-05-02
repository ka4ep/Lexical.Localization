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
    /// Signals that the class can append parts.
    /// </summary>
    public interface ILinePartAppender
    {
    }

    /// <summary>
    /// Zero argument count line part appender.
    /// </summary>
    public interface ILinePartAppender0 : ILinePartAppender { }

    /// <summary>
    /// Zero argument count line part appender.
    /// </summary>
    /// <typeparam name="Part">the interface type of the line part that can be appended </typeparam>
    public interface ILinePartAppender0<Part> : ILinePartAppender0 where Part : ILinePart
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="previous"></param>
        /// <returns>new part</returns>
        Part Append(ILinePart previous);
    }

    /// <summary>
    /// Signals that appender is composed of component appenders.
    /// </summary>
    public interface ILinePartAppenderEnumerable : IEnumerable<ILinePartAppender>
    {
    }

    /// <summary>
    /// One argument count line part appender.
    /// </summary>
    public interface ILinePartAppender1 : ILinePartAppender { }

    /// <summary>
    /// One argument count line part appender.
    /// </summary>
    /// <typeparam name="Part">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    public interface ILinePartAppender1<Part, A0> : ILinePartAppender1 where Part : ILinePart
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        Part Append(ILinePart previous, A0 a0);
    }

    /// <summary>
    /// Two argument count line part appender.
    /// </summary>
    public interface ILinePartAppender2 : ILinePartAppender { }

    /// <summary>
    /// Two argument count line part appender.
    /// </summary>
    /// <typeparam name="Part">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    public interface ILinePartAppender2<Part, A0, A1> : ILinePartAppender1 where Part : ILinePart
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        Part Append(ILinePart previous, A0 a0, A1 a1);
    }

    /// <summary>
    /// Three argument count line part appender.
    /// </summary>
    public interface ILinePartAppender3 : ILinePartAppender { }

    /// <summary>
    /// Two argument count line part appender.
    /// </summary>
    /// <typeparam name="Part">the part type</typeparam>
    /// <typeparam name="A0"></typeparam>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    public interface ILinePartAppender3<Part, A0, A1, A2> : ILinePartAppender2 where Part : ILinePart
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        Part Append(ILinePart previous, A0 a0, A1 a1, A2 a2);
    }

    /// <summary>
    /// Adapts to different <see cref="ILinePartAppender"/> types.
    /// </summary>
    public interface ILinePartAppenderAdapter : ILinePartAppender
    {
        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <returns>appender or null</returns>
        ILinePartAppender0<Part> Cast<Part>() where Part : ILinePart;

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <returns>appender or null</returns>
        ILinePartAppender1<Part, A0> Cast<Part, A0>() where Part : ILinePart;

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <returns>appender or null</returns>
        ILinePartAppender2<Part, A0, A1> Cast<Part, A0, A1>() where Part : ILinePart;

        /// <summary>
        /// Get appender for part type <typeparamref name="Part"/>.
        /// </summary>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0">argument 0 type</typeparam>
        /// <typeparam name="A1">argument 1 type</typeparam>
        /// <typeparam name="A2">argument 2 type</typeparam>
        /// <returns>appender or null</returns>
        ILinePartAppender3<Part, A0, A1, A2> Cast<Part, A0, A1, A2>() where Part : ILinePart;
    }

    /// <summary></summary>
    public static partial class LinePartAppenderExtensions
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        public static Part Append<Part>(this ILinePartAppender appender, ILinePart previous) where Part : ILinePart
        {
            if (appender == null) throw new ArgumentNullException(nameof(appender));
            ILinePartAppender0<Part> casted = (appender as ILinePartAppender0<Part>) ?? ((appender as ILinePartAppenderAdapter).Cast<Part>()) ?? throw new LocalizationException($"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append<Part>(previous);
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0>(this ILinePartAppender appender, ILinePart previous, A0 a0) where Part : ILinePart
        {
            if (appender == null) throw new ArgumentNullException(nameof(appender));
            ILinePartAppender1<Part, A0> casted = (appender as ILinePartAppender1<Part, A0>) ?? ((appender as ILinePartAppenderAdapter).Cast<Part, A0>()) ?? throw new LocalizationException($"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0);
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1) where Part : ILinePart
        {
            if (appender == null) throw new ArgumentNullException(nameof(appender));
            ILinePartAppender2<Part, A0, A1> casted = (appender as ILinePartAppender2<Part, A0, A1>) ?? ((appender as ILinePartAppenderAdapter).Cast<Part, A0, A1>()) ?? throw new LocalizationException($"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0, a1);
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="LocalizationException">If appended could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1, A2>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1, A2 a2) where Part : ILinePart
        {
            if (appender == null) throw new ArgumentNullException(nameof(appender));
            ILinePartAppender3<Part, A0, A1, A2> casted = (appender as ILinePartAppender3<Part, A0, A1, A2>) ?? ((appender as ILinePartAppenderAdapter).Cast<Part, A0, A1, A2>()) ?? throw new LocalizationException($"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0, a1, a2);
        }

    }
}
