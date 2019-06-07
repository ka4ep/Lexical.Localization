// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineStringResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        IStringResolver StringResolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="stringResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver StringResolver(this ILine line, IStringResolver stringResolver)
            => line.Append<ILineStringResolver, IStringResolver>(stringResolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="stringResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver StringResolver(this ILineFactory lineFactory, IStringResolver stringResolver)
            => lineFactory.Create<ILineStringResolver, IStringResolver>(null, stringResolver);

        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="stringResolver">Assembly qualified class name to <see cref="IStringResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint StringResolver(this ILine line, string stringResolver)
            => line.Append<ILineHint, string, string>("StringResolver", stringResolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="stringResolver">Assembly qualified class name to <see cref="IStringResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint StringResolver(this ILineFactory lineFactory, string stringResolver)
            => lineFactory.Create<ILineHint, string, string>(null, "StringResolver", stringResolver);

    }
}
