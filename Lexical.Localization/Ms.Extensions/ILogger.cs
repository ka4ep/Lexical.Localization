// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;
using Microsoft.Extensions.Logging;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILine"/>.
    /// </summary>
    public static partial class ILineLoggerMsExtensions
    {
        /// <summary>
        /// Append <paramref name="logger"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger ILogger(this ILine line, ILogger logger)
            => line.Logger(new LineILogger(logger));

        /// <summary>
        /// Append <paramref name="loggerFactory"/> to <paramref name="line"/>.
        /// 
        /// Uses type specific <see cref="ILogger{TCategoryName}"/> by using the type in <see cref="ILineType"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="loggerFactory"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger ILogger(this ILine line, ILoggerFactory loggerFactory)
            => line.Logger(new LineILoggerFactory(loggerFactory));
    }

    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="ILogger"/>.
    /// </summary>
    public class LineILogger : ILocalizationLogger, IStringResolverLogger, IResourceResolverLogger
    {
        ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public LineILogger(ILogger logger)
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
            if (!_logger.IsEnabled(LogLevel.Error)) return;
            // Write status
            _logger.LogError(error, error.Message, error.Data);
        }

        /// <summary>
        /// Log string
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
        }
    }

    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="ILogger"/>.
    /// 
    /// Uses type specific <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    public class LineILoggerFactory : ILocalizationLogger, IStringResolverLogger, IResourceResolverLogger
    {
        ILoggerFactory loggerFactory;
        ILogger fallbackLogger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="fallbackCategory">(optional) category to use if Type is not found in key</param>
        public LineILoggerFactory(ILoggerFactory loggerFactory, string fallbackCategory = "Lexical.Localization")
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.fallbackLogger = loggerFactory.CreateLogger(fallbackCategory);
        }

        /// <summary>
        /// Logging has ended
        /// </summary>
        public void OnCompleted()
        {
            loggerFactory = null;
            fallbackLogger = null;
        }

        /// <summary>
        /// Get type specific logger, or fallback logger.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>logger or null if has been disposed</returns>
        ILogger Logger(ILine line)
        {
            ILoggerFactory _loggerFactory = loggerFactory;
            if (_loggerFactory == null) return null;
            ILine type = line.FindTypeKey();
            if (type is ILineType lineType && lineType.Type != null) return _loggerFactory.CreateLogger(lineType.Type);
            if (type is ILineParameter lineParameter && lineParameter.ParameterValue != null) return _loggerFactory.CreateLogger(lineParameter.ParameterValue);
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
            if (!_logger.IsEnabled(LogLevel.Error)) return;
            // Write status
            _logger.LogError(error, error.Message, error.Data);
        }

        /// <summary>
        /// Log string resolve
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
        }

        /// <summary>
        /// Log resource resolve
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == LineStatusSeverity.Ok) { _logger.LogInformation(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == LineStatusSeverity.Warning) { _logger.LogWarning(value.Exception, value.DebugInfo); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= LineStatusSeverity.Error) { _logger.LogError(value.Exception, value.DebugInfo); return; }
        }
    }

}
