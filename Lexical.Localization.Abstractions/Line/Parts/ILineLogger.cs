// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// Logger hint. Used by <see cref="IStringResolver"/>.
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

        /// <summary>
        /// Append observer that monitors resolving of localization strings.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="logger"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">Append logger</exception>
        public static ILineLogger Logger(this ILineFactory lineFactory, IObserver<LineString> logger)
            => lineFactory.Create<ILineLogger, IObserver<LineString>>(null, logger);

        /// <summary>
        /// Try to add a <paramref name="logger"/> to <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="logger">writer such as Console.Out</param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns>disposable subscription handle, or null if <paramref name="line"/> cannot be observed</returns>
        public static ILineLogger Logger(this ILine line, TextWriter logger, LineStatusSeverity severity = LineStatusSeverity.Warning)
            => line.Logger(new LineTextLogger(logger, severity));

        /// <summary>
        /// Append <see cref="System.Diagnostics.Trace"/> logger. 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="severity">
        ///     <list type="bullet">
        ///         <item>0 - OK</item>
        ///         <item>1 - Warning</item>
        ///         <item>2 - Error</item>
        ///         <item>3 - Failed</item>
        ///     </list>
        /// </param>
        /// <returns></returns>
        public static ILineLogger DiagnosticsTrace(this ILine line, LineStatusSeverity severity = LineStatusSeverity.Warning)
            => line.Logger(new LineDiagnosticsTrace(severity));
    }
}
