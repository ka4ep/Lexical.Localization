// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;

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
        /// <summary>
        /// Try to add a <paramref name="logger"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger">writer such as Console.Out</param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger Logger(this ILine line, TextWriter logger, int severity = 1)
            => line.Logger(new LineTextLogger(logger, severity));

        /// <summary>
        /// Append <see cref="System.Diagnostics.Trace"/> logger. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns></returns>
        public static ILineLogger DiagnosticsTrace(this ILine line, int severity = 1)
            => line.Logger(new LineDiagnosticsTrace(severity));
    }

    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="TextWriter"/>.
    /// </summary>
    public class LineTextLogger : IObserver<LineString>
    {
        TextWriter logger;

        /// <summary>
        /// Severity to log
        /// 
        /// <list type="bullet">
        ///     <item>0 - OK</item>
        ///     <item>1 - Warning</item>
        ///     <item>2 - Error</item>
        ///     <item>3 - Failed</item>
        /// </list>
        /// </summary>
        int severity;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="severity"></param>
        public LineTextLogger(TextWriter logger, int severity)
        {
            this.logger = logger;
            this.severity = severity;
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            logger = null;
        }

        /// <summary>
        /// Formatter produced exception
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            if (3 >= this.severity)
                _logger.Write($"{error.GetType().Name}: {error.Message}");
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            int severity = value.Severity;
            // Write status
            if (severity >= this.severity)
                _logger.Write(value.DebugInfo);
        }
    }

    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="Trace"/>.
    /// </summary>
    public class LineDiagnosticsTrace : IObserver<LineString>
    {
        /// <summary>
        /// Severity to log
        /// 
        /// <list type="bullet">
        ///     <item>0 - OK</item>
        ///     <item>1 - Warning</item>
        ///     <item>2 - Error</item>
        ///     <item>3 - Failed</item>
        /// </list>
        /// </summary>
        int severity;

        /// <summary>
        /// disposed
        /// </summary>
        bool disposed;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="severity"></param>
        public LineDiagnosticsTrace(int severity)
        {
            this.severity = severity;
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            disposed = true;
        }

        /// <summary>
        /// Formatter produced exception
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            Trace.TraceError(error.Message);
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            if (disposed) return;
            // Get severity
            int severity = value.Severity;
            // Threshold
            if (severity < this.severity) return;
            // Write status
            switch (severity)
            {
                case 0:
                    Trace.TraceInformation(value.DebugInfo);
                    return;
                case 1:
                    Trace.TraceWarning(value.DebugInfo);
                    return;
                case 2:
                case 3:
                    Trace.TraceError(value.DebugInfo);
                    return;
            }
        }
    }

}

namespace Lexical.Localization
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILine"/>.
    /// </summary>
    public static partial class LineILoggerExtensions
    {
        /// <summary>
        /// Append <paramref name="logger"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger Logger(this ILine line, ILogger logger)
            => line.Logger(new LineILogger(logger));
    }

    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="ILogger"/>.
    /// </summary>
    public class LineILogger : IObserver<LineString>
    {
        ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public LineILogger(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            logger = null;
        }

        /// <summary>
        /// Formatter produced exception
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Write status
            if (_logger.IsEnabled(LogLevel.Error)) _logger.LogError(error, error.Message, error.Data);
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            int severity = value.Severity;
            // Write status
            if (_logger.IsEnabled(LogLevel.Trace) && severity == 0) { _logger.LogError(value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == 1) { _logger.LogWarning(value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= 2) { _logger.LogError(value.DebugInfo); return; }
        }
    }
}

