// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineResourceResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        IResourceResolver ResourceResolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resourceResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineResourceResolver ResourceResolver(this ILine line, IResourceResolver resourceResolver)
            => line.Append<ILineResourceResolver, IResourceResolver>(resourceResolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resourceResolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineResourceResolver ResourceResolver(this ILineFactory lineFactory, IResourceResolver resourceResolver)
            => lineFactory.Create<ILineResourceResolver, IResourceResolver>(null, resourceResolver);

        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resourceResolver">Assembly qualified class name to <see cref="IResourceResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint ResourceResolver(this ILine line, string resourceResolver)
            => line.Append<ILineHint, string, string>("ResourceResolver", resourceResolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resourceResolver">Assembly qualified class name to <see cref="IResourceResolver"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineHint ResourceResolver(this ILineFactory lineFactory, string resourceResolver)
            => lineFactory.Create<ILineHint, string, string>(null, "ResourceResolver", resourceResolver);
    }
}
