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
        /// <exception cref="AssetKeyException">If append failed</exception>
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
        /// <exception cref="AssetKeyException">If append failed</exception>
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
        /// <exception cref="AssetKeyException">If append failed</exception>
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
        /// <exception cref="AssetKeyException">If append failed</exception>
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
    public static partial class ILinePartAppenderExtensions
    {
        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part</returns>
        /// <exception cref="AssetKeyException">If part could not append <typeparamref name="Part"/></exception>
        public static Part Append<Part>(this ILinePartAppender appender, ILinePart previous) where Part : ILinePart
        {
            if (appender == null) throw new AssetKeyException(previous, "Appender is not found.");
            ILinePartAppender0<Part> casted = ((appender as ILinePartAppenderAdapter)?.Cast<Part>()) ?? (appender as ILinePartAppender0<Part>) ?? throw new AssetKeyException(previous, $"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous);
        }

        /// <summary>
        /// Try append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <typeparam name="Part"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part>(this ILinePartAppender appender, ILinePart previous) where Part : ILinePart
        {
            if (appender == null) return default;
            ILinePartAppender0<Part> casted = (appender as ILinePartAppenderAdapter)?.Cast<Part>() ?? (appender as ILinePartAppender0<Part>);
            if (casted == null) return default;
            try { return casted.Append(previous); } catch (AssetKeyException) { return default; }
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <exception cref="AssetKeyException">If part could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0>(this ILinePartAppender appender, ILinePart previous, A0 a0) where Part : ILinePart
        {
            if (appender == null) throw new AssetKeyException(previous, "Appender is not found.");
            ILinePartAppender1<Part, A0> casted = ((appender as ILinePartAppenderAdapter)?.Cast<Part, A0>()) ?? (appender as ILinePartAppender1<Part, A0>) ?? throw new AssetKeyException(previous, $"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0);
        }

        /// <summary>
        /// Try append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0 </param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0>(this ILinePartAppender appender, ILinePart previous, A0 a0) where Part : ILinePart
        {
            if (appender == null) return default;
            ILinePartAppender1<Part, A0> casted = (appender as ILinePartAppenderAdapter)?.Cast<Part, A0>() ?? (appender as ILinePartAppender1<Part, A0>);
            if (casted == null) return default;
            try { return casted.Append(previous, a0); } catch (AssetKeyException) { return default; }
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
        /// <exception cref="AssetKeyException">If part could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1) where Part : ILinePart
        {
            if (appender == null) throw new AssetKeyException(previous, "Appender is not found.");
            ILinePartAppender2<Part, A0, A1> casted = ((appender as ILinePartAppenderAdapter)?.Cast<Part, A0, A1>()) ?? (appender as ILinePartAppender2<Part, A0, A1>) ?? throw new AssetKeyException(previous, $"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0, a1);
        }

        /// <summary>
        /// Try append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0, A1>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1) where Part : ILinePart
        {
            if (appender == null) return default;
            ILinePartAppender2<Part, A0, A1> casted = (appender as ILinePartAppenderAdapter)?.Cast<Part, A0, A1>() ?? (appender as ILinePartAppender2<Part, A0, A1>);
            if (casted == null) return default;
            try { return casted.Append(previous, a0, a1); } catch (AssetKeyException) { return default; }
        }

        /// <summary>
        /// Append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="a2">argument 2</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <exception cref="AssetKeyException">If part could not append <typeparamref name="Part"/></exception>
        /// <returns>Part</returns>
        public static Part Append<Part, A0, A1, A2>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1, A2 a2) where Part : ILinePart
        {
            if (appender == null) throw new AssetKeyException(previous, "Appender is not found.");
            ILinePartAppender3<Part, A0, A1, A2> casted = ((appender as ILinePartAppenderAdapter)?.Cast<Part, A0, A1, A2>()) ?? (appender as ILinePartAppender3<Part, A0, A1, A2>) ?? throw new AssetKeyException(previous, $"Appender doesn't have capability to adapt to {nameof(Part)}.");
            return casted.Append(previous, a0, a1, a2);
        }

        /// <summary>
        /// Try append new <see cref="ILinePart"/> to <paramref name="previous"/>.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="a0">argument 0</param>
        /// <param name="a1">argument 1</param>
        /// <param name="a2">argument 2</param>
        /// <typeparam name="Part"></typeparam>
        /// <typeparam name="A0"></typeparam>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <returns>Part or null</returns>
        public static Part TryAppend<Part, A0, A1, A2>(this ILinePartAppender appender, ILinePart previous, A0 a0, A1 a1, A2 a2) where Part : ILinePart
        {
            if (appender == null) return default;
            ILinePartAppender3<Part, A0, A1, A2> casted = (appender as ILinePartAppenderAdapter)?.Cast<Part, A0, A1, A2>() ?? (appender as ILinePartAppender3<Part, A0, A1, A2>);
            if (casted == null) return default;
            try { return casted.Append(previous, a0, a1, a2); } catch (AssetKeyException) { return default; }
        }

    }
}
