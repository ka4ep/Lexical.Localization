// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using Lexical.Localization.StringFormat;
using Microsoft.Extensions.Logging;

namespace Lexical.Localization.Internal
{
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
                _logger.WriteLine($"{error.GetType().Name}: {error.Message}");
        }

        /// <summary>
        /// Formatter supplies format result.
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
                _logger.WriteLine(value.DebugInfo);
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
        /// Formatter supplies format result.
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
        /// Formatter supplies format result.
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

