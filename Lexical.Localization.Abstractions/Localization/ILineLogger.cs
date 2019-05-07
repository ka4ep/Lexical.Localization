// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with logger.
    /// </summary>
    public interface ILineLogger : ILine
    {
        /// <summary>
        /// (Optional) The assigned logger.
        /// </summary>
        IObservable<LocalizationString> Logger { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append observer that monitors resolving of localization strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logger"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILocalizationKeyLoggerAssignable"/></exception>
        public static ILineLogger Logger(this ILine key, IObserver<LocalizationString> logger)
        {
            if (key is ILocalizationKeyLoggerAssignable casted) return casted.Logger(logger);
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyLoggerAssignable)}.");
        }
    }
}
