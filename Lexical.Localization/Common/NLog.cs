// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    using NLog;
    using Lexical.Localization.Internal;

    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILine"/>.
    /// </summary>
    public static partial class NLogExtensions
    {
        /// <summary>
        /// Append <paramref name="nlog"/> logger to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nlog">NLog Logger</param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger NLog(this ILine line, ILogger nlog)
            => line.Logger(new NLogLocalizationLogger(nlog));

        /// <summary>
        /// Append <paramref name="nlogfactory"/> logger to <paramref name="line"/>.
        /// 
        /// Writes log messages to NLog in a way that type specific log events are directed to the respective type.
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nlogfactory">NLog Logger</param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger NLog(this ILine line, LogFactory nlogfactory)
            => line.Logger(new NLogFactoryLocalizationLogger(nlogfactory));
    }

}

namespace Lexical.Localization.Internal
{
    using Lexical.Localization.Resource;
    using NLog;

    /// <summary>
    /// Adapts localization log messages to NLog.
    /// </summary>
    public class NLogLocalizationLogger : IStringResolverLogger, IResourceResolverLogger
    {
        private ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public NLogLocalizationLogger(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            logger = null;
        }

        /// <summary>
        /// Resolver produced an exception.
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            // Get reference
            var _logger = logger;
            // Is disposed?
            if (_logger == null) return;
            // Is enabled?
            if (!_logger.IsErrorEnabled) return;
            // Write entry
            _logger.Error(error, "");
        }

        /// <summary>
        /// Log string
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
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
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
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
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
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

    }

    /// <summary>
    /// Adapts localization log messages to NLog in a way that type specific log events are directed to the respective type.
    /// </summary>
    public class NLogFactoryLocalizationLogger : IStringResolverLogger, IResourceResolverLogger
    {
        LogFactory logFactory;
        ILogger fallbackLogger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logFactory"></param>
        /// <param name="fallbackCategory"></param>
        public NLogFactoryLocalizationLogger(LogFactory logFactory, string fallbackCategory = "Lexical.Localization")
        {
            this.logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            this.fallbackLogger = logFactory.GetLogger(fallbackCategory);
        }

        public void OnCompleted()
        {
            logFactory = null;
            fallbackLogger = null;
        }

        /// <summary>
        /// Get type specific logger, or fallback logger.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>logger or null if has been disposed</returns>
        ILogger Logger(ILine line)
        {
            LogFactory _logFactory = logFactory;
            if (_logFactory == null) return null;
            ILine type = line.FindTypeKey();
            if (type is ILineType lineType && lineType.Type != null) return _logFactory.GetLogger(lineType.Type.FullName, lineType.Type);
            if (type is ILineParameter lineParameter && lineParameter.ParameterValue != null) return _logFactory.GetLogger(lineParameter.ParameterValue);
            return fallbackLogger;
        }

        /// <summary>
        /// Resolver produced an exception.
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            // Get reference
            var _logger = fallbackLogger;
            // Is disposed?
            if (_logger == null) return;
            // Is enabled?
            if (!_logger.IsErrorEnabled) return;
            // Write entry
            _logger.Error(error, "");
        }

        /// <summary>
        /// Log string
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceBytes value)
        {
            // Get reference
            var _logger = Logger(value.Line);
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineResourceStream value)
        {
            // Get reference
            var _logger = Logger(value.Line);
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(LineString value)
        {
            // Get reference
            var _logger = Logger(value.Line);
            // Is disposed?
            if (_logger == null) return;
            // Get severity
            LineStatusSeverity severity = value.Severity;
            // Write status
            if (severity == LineStatusSeverity.Ok && _logger.IsInfoEnabled) { _logger.Info(value.Exception, value.DebugInfo); return; }
            if (severity == LineStatusSeverity.Warning && _logger.IsWarnEnabled) { _logger.Warn(value.Exception, value.DebugInfo); return; }
            if (severity >= LineStatusSeverity.Error && _logger.IsErrorEnabled) { _logger.Error(value.Exception, value.DebugInfo); return; }
        }

    }

}
