// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that can be assigned with an observer for logging purposes.
    /// 
    /// Observable is sent every resolved <see cref="LocalizationString"/> by <see cref="ILocalizationResolver"/>.
    /// </summary>
    public interface ILocalizationKeyLoggerAssignable : ILinePart
    {
        /// <summary>
        /// Append observer that monitors resolving of localization strings.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns>key that is assigned with <paramref name="logger"/></returns>
        ILocalizationKeyLoggerAssigned Logger(IObserver<LocalizationString> logger);
    }

    /// <summary>
    /// A key that has been assigned with logger.
    /// </summary>
    public interface ILocalizationKeyLoggerAssigned : ILinePart
    {
        /// <summary>
        /// (Optional) The assigned logger.
        /// </summary>
        IObservable<LocalizationString> Logger { get; }
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Append observer that monitors resolving of localization strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="logger"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILocalizationKeyLoggerAssignable"/></exception>
        public static ILocalizationKeyLoggerAssigned Logger(this ILinePart key, IObserver<LocalizationString> logger)
        {
            if (key is ILocalizationKeyLoggerAssignable casted) return casted.Logger(logger);
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyLoggerAssignable)}.");
        }
    }
}
