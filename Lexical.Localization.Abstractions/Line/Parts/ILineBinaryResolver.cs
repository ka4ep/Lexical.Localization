// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Binary;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineBinaryResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        IBinaryResolver BinaryResolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append binary resource resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="binaryResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineBinaryResolver BinaryResolver(this ILine line, IBinaryResolver binaryResolver)
            => line.Append<ILineBinaryResolver, IBinaryResolver>(binaryResolver);

        /// <summary>
        /// Create binary resource  resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="binaryResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineBinaryResolver BinaryResolver(this ILineFactory lineFactory, IBinaryResolver binaryResolver)
            => lineFactory.Create<ILineBinaryResolver, IBinaryResolver>(null, binaryResolver);

        /// <summary>
        /// Append binary resource  resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="binaryResolver">Assembly qualified class name to <see cref="IBinaryResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint BinaryResolver(this ILine line, string binaryResolver)
            => line.Append<ILineHint, string, string>("BinaryResolver", binaryResolver);

        /// <summary>
        /// Create binary resource  resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="binaryResolver">Assembly qualified class name to <see cref="IBinaryResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint BinaryResolver(this ILineFactory lineFactory, string binaryResolver)
            => lineFactory.Create<ILineHint, string, string>(null, "BinaryResolver", binaryResolver);
    }
}
