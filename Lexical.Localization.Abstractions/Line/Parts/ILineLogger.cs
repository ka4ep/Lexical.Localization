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
        IObserver<LineString> Logger { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append observer that monitors resolving of localization strings.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">Append logger</exception>
        public static ILineLogger Logger(this ILine line, IObserver<LineString> logger)
            => line.Append<ILineLogger, IObserver<LineString>>(logger);
    }
}
