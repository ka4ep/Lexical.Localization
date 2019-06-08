// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using System;
using System.Diagnostics;
using System.IO;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="TextWriter"/>.
    /// </summary>
    public class LineTextLogger : IStringResolverLogger, IResourceResolverLogger
    {
        /// <summary>
        /// Text output stream.
        /// </summary>
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
        LineStatusSeverity severity;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="severity"></param>
        public LineTextLogger(TextWriter logger, LineStatusSeverity severity)
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
            // Severity under threshold?
            if (error == null || severity < LineStatusSeverity.Error) return;
            // Write exception
            _logger.WriteLine("{0}: {1}", error.GetType().Name, error.Message);
        }

        /// <summary>
        /// Log string resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Severity under threshold?
            if (severity < this.severity) return;
            // Write status
            if (value.Exception==null)
                _logger.WriteLine(value.DebugInfo);
            else
                _logger.WriteLine("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception.Message);
        }

        /// <summary>
        /// Log byte resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceBytes value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Severity under threshold?
            if (severity < this.severity) return;
            // Write status
            if (value.Exception == null)
                _logger.WriteLine(value.DebugInfo);
            else
                _logger.WriteLine("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception.Message);
        }

        /// <summary>
        /// Log stream resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceStream value)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Severity under threshold?
            if (severity < this.severity) return;
            // Write status
            if (value.Exception == null)
                _logger.WriteLine(value.DebugInfo);
            else
                _logger.WriteLine("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception.Message);
        }
    }

    /// <summary>
    /// Observes resolved keys and writes log lines to <see cref="Trace"/>.
    /// </summary>
    public class LineDiagnosticsTrace : IStringResolverLogger, IResourceResolverLogger
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
        LineStatusSeverity severity;

        /// <summary>
        /// disposed
        /// </summary>
        bool disposed;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="severity"></param>
        public LineDiagnosticsTrace(LineStatusSeverity severity)
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
        /// Log string resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            if (disposed) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Threshold
            if (severity < this.severity) return;
            // Write status
            if (value.Exception == null)
            {
                // No exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError(value.DebugInfo);
                        return;
                }
            }
            else
            {
                // With exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                }
            }
        }

        /// <summary>
        /// Log resource resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceBytes value)
        {
            if (disposed) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Threshold
            if (severity < this.severity) return;
            // Write status
            if (value.Exception == null)
            {
                // No exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError(value.DebugInfo);
                        return;
                }
            }
            else
            {
                // With exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                }
            }
        }

        /// <summary>
        /// Log resource resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceStream value)
        {
            if (disposed) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Threshold
            if (severity < this.severity) return;
            // Write status
            if (value.Exception == null)
            {
                // No exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning(value.DebugInfo);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError(value.DebugInfo);
                        return;
                }
            }
            else
            {
                // With exception
                switch (severity)
                {
                    case LineStatusSeverity.Ok:
                        Trace.TraceInformation("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Warning:
                        Trace.TraceWarning("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                    case LineStatusSeverity.Error:
                    case LineStatusSeverity.Failed:
                        Trace.TraceError("{0} {1}: {2}", value.DebugInfo, value.Exception.GetType().Name, value.Exception);
                        return;
                }
            }
        }
    }

}

