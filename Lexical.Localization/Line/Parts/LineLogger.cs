﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// "Logger" key that carries <see cref="Logger"/>. 
    /// </summary>
    [Serializable]
    public class LineLogger : LineBase, ILineLogger, ILineArguments<ILineLogger, IObserver<LineString>>
    {
        /// <summary>
        /// Logger, null if non-standard assembly.
        /// </summary>
        protected IObserver<LineString> logger;

        /// <summary>
        /// Logger property
        /// </summary>
        public IObserver<LineString> Logger { get => logger; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IObserver<LineString> Argument0 => logger;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="logger"></param>
        public LineLogger(ILineFactory appender, ILine prevKey, IObserver<LineString> logger) : base(appender, prevKey)
        {
            this.logger = logger;
        }
    }

    public partial class LineAppender : ILineFactory<ILineLogger, IObserver<LineString>>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="logger"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineLogger, IObserver<LineString>>.TryCreate(ILineFactory appender, ILine previous, IObserver<LineString> logger, out ILineLogger line)
        {
            line = new LineLogger(appender, previous, logger);
            return true;
        }
    }

    /// <summary>
    /// "Logger" key that carries <see cref="Logger"/>. 
    /// </summary>
    [Serializable]
    public class StringLocalizerLogger : StringLocalizerBase, ILineLogger, ILineArguments<ILineLogger, IObserver<LineString>>
    {
        /// <summary>
        /// Logger, null if non-standard assembly.
        /// </summary>
        protected IObserver<LineString> logger;

        /// <summary>
        /// Logger property
        /// </summary>
        public IObserver<LineString> Logger { get => logger; set => throw new InvalidOperationException(); }

        /// <summary>
        /// Appending arguments.
        /// </summary>
        public IObserver<LineString> Argument0 => logger;

        /// <summary>
        /// Create new line part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="prevKey"></param>
        /// <param name="logger"></param>
        public StringLocalizerLogger(ILineFactory appender, ILine prevKey, IObserver<LineString> logger) : base(appender, prevKey)
        {
            this.logger = logger;
        }
    }

    public partial class StringLocalizerAppender : ILineFactory<ILineLogger, IObserver<LineString>>
    {
        /// <summary>
        /// Append part.
        /// </summary>
        /// <param name="appender"></param>
        /// <param name="previous"></param>
        /// <param name="logger"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        bool ILineFactory<ILineLogger, IObserver<LineString>>.TryCreate(ILineFactory appender, ILine previous, IObserver<LineString> logger, out ILineLogger line)
        {
            line = new StringLocalizerLogger(appender, previous, logger);
            return true;
        }
    }

    /// <summary>
    /// Extension methods for adding loggers.
    /// </summary>
    public static partial class LineLoggerExtensions
    {

    }
}

