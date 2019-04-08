// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILocalizationStringResolver"/>s.
    /// </summary>
    public static partial class LocalizationFormatterLoggerExtensions_
    {
        /// <summary>
        /// Try to add a observer to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="observer"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="formatter"/> cannot be observed</returns>
        public static IDisposable TryAddObserver(this ILocalizationStringResolver formatter, IObserver<LocalizationString> observer)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (formatter is IObservable<LocalizationString> observable)
                return observable.Subscribe(observer);
            return null;
        }

        /// <summary>
        /// Try to add a <paramref name="logger"/> to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="logger">writer such as Console.Out</param>
        /// <param name="logOk"></param>
        /// <param name="logWarning"></param>
        /// <param name="logError"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="formatter"/> cannot be observed</returns>
        public static IDisposable TryAddLogger(this ILocalizationStringResolver formatter, TextWriter logger, bool logOk, bool logWarning, bool logError)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (formatter is IObservable<LocalizationString> observable)
                return observable.Subscribe(new TextLogger(logger, logOk, logWarning, logError));
            return null;
        }
    }

    internal class TextLogger : IObserver<LocalizationString>
    {
        TextWriter logger;

        bool logOk;
        bool logWarning;
        bool logError;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="logOk"></param>
        /// <param name="logWarning"></param>
        /// <param name="logError"></param>
        public TextLogger(TextWriter logger, bool logOk, bool logWarning, bool logError)
        {
            this.logger = logger;
            this.logOk = logOk;
            this.logWarning = logWarning;
            this.logError = logError;
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
            if ((logOk && severity == 0) || (logWarning && severity == 1) || (logError && severity>=2) )
                _logger.Write(value);
        }
    }
}

namespace Lexical.Localization
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods for adding loggers to <see cref="ILocalizationStringResolver"/>s.
    /// </summary>
    public static partial class LocalizationFormatterLoggerExtensions___
    {
        /// <summary>
        /// Try to add a <paramref name="logger"/> to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="logger"></param>
        /// <returns>disposable subscription handle, or null if <paramref name="formatter"/> cannot be observed</returns>
        public static IDisposable TryAddLogger(this ILocalizationStringResolver formatter, ILogger logger)
        {
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (formatter is IObservable<LocalizationString> observable)
                return observable.Subscribe(new FormatterLogger(logger));
            return null;
        }
    }

    internal class FormatterLogger : IObserver<LocalizationString>
    {
        ILogger logger;

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public FormatterLogger(ILogger logger)
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
            if (_logger.IsEnabled(LogLevel.Trace) && severity == 0) { _logger.LogError(value.ToString()); return; }
            if (_logger.IsEnabled(LogLevel.Warning) && severity == 1) { _logger.LogWarning(value.ToString()); return; }
            if (_logger.IsEnabled(LogLevel.Error) && severity >= 2) { _logger.LogError(value.ToString()); return; }
        }
    }
}
