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
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineResourceResolver Resolver(this ILine line, IResourceResolver resolver)
            => line.Append<ILineResourceResolver, IResourceResolver>(resolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineResourceResolver Resolver(this ILineFactory lineFactory, IResourceResolver resolver)
            => lineFactory.Create<ILineResourceResolver, IResourceResolver>(null, resolver);
    }
}
