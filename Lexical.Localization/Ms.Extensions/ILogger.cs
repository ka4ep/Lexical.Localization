// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Binary;
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
        public static ILineLogger ILogger(this ILine line, Microsoft.Extensions.Logging.ILogger logger)
            => line.Logger(new MSLogger(logger));

        /// <summary>
        /// Append <paramref name="loggerFactory"/> to <paramref name="line"/>.
        /// 
        /// Uses type specific <see cref="ILogger{TCategoryName}"/> by using the type in <see cref="ILineType"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="fallbackCategory">(optional) logger name to use when Type cannot be derived</param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger ILogger(this ILine line, ILoggerFactory loggerFactory, string fallbackCategory = "Lexical.Localization")
            => line.Logger(new MSLoggerFactory(loggerFactory, fallbackCategory));
    }
}

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Observes resolved localization strings and logs into <see cref="Microsoft.Extensions.Logging.ILogger"/>.
    /// </summary>
    public class MSLogger : Common.ILogger, IStringResolverLogger, IBinaryResolverLogger
    {
        Microsoft.Extensions.Logging.ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public MSLogger(Microsoft.Extensions.Logging.ILogger logger)
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
        public void OnNext(LineBinaryBytes value)
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
        public void OnNext(LineBinaryStream value)
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
    /// Observes resolved localization strings and logs into <see cref="Microsoft.Extensions.Logging.ILogger"/>.
    /// 
    /// Uses type specific <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    public class MSLoggerFactory : Common.ILogger, IStringResolverLogger, IBinaryResolverLogger
    {
        Microsoft.Extensions.Logging.ILoggerFactory loggerFactory;
        Microsoft.Extensions.Logging.ILogger fallbackLogger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="fallbackCategory">(optional) category to use if Type is not found in key</param>
        public MSLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory, string fallbackCategory = "Lexical.Localization")
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
        Microsoft.Extensions.Logging.ILogger Logger(ILine line)
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
        public void OnNext(LineBinaryBytes value)
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
        public void OnNext(LineBinaryStream value)
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
