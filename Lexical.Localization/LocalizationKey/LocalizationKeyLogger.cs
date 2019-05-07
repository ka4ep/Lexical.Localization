// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILocalizationResolver"/>s.
    /// </summary>
    public static partial class LocalizationKeyLoggerExtensions
    {
        /// <summary>
        /// Try to add a <paramref name="logger"/> to <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logger">writer such as Console.Out</param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns>disposable subscription handle, or null if <paramref name="key"/> cannot be observed</returns>
        public static ILineLogger Logger(this ILine key, TextWriter logger, int severity = 1)
        {
            if (key is ILocalizationKeyLoggerAssignable casted) return casted.Logger(new LocalizationTextLogger(logger, severity));
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyLoggerAssignable)}.");
        }

        /// <summary>
        /// Append <see cref="System.Diagnostics.Trace"/> logger. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns></returns>
        public static ILineLogger DiagnosticsTrace(this ILine key, int severity = 1)
        {
            if (key is ILocalizationKeyLoggerAssignable casted) return casted.Logger(new LocalizationDiagnosticsTrace(severity));
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyLoggerAssignable)}.");
        }
    }

    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="TextWriter"/>.
    /// </summary>
    public class LocalizationTextLogger : IObserver<LocalizationString>
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
        public LocalizationTextLogger(TextWriter logger, int severity)
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
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LocalizationString value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            int severity = value.Severity;
            // Write status
            if (severity>=this.severity)
                _logger.Write(value.DebugInfo);
        }
    }

    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="Trace"/>.
    /// </summary>
    public class LocalizationDiagnosticsTrace : IObserver<LocalizationString>
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
        public LocalizationDiagnosticsTrace(int severity)
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
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LocalizationString value)
        {
            if (disposed) return;
            // Get severity
            int severity = value.Severity;
            // Threshold
            if (severity < this.severity) return;
            // Write status
            switch(severity)
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
    /// Extension methods for adding loggers to <see cref="ILocalizationKey"/>s.
    /// </summary>
    public static partial class LocalizationKeyLoggerExtensions
    {
        /// <summary>
        /// Append <paramref name="logger"/> to <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logger"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="key"/> cannot be observed</returns>
        public static ILineLogger Logger(this ILine key, ILogger logger)
        {
            if (key is ILocalizationKeyLoggerAssignable casted) return casted.Logger(new LocalizationLogger(logger));
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyLoggerAssignable)}.");
        }
    }

    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="ILogger"/>.
    /// </summary>
    public class LocalizationLogger : IObserver<LocalizationString>
    {
        ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public LocalizationLogger(ILogger logger)
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
        }

        /// <summary>
        /// Formatter supplies formulation result.
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LocalizationString value)
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
