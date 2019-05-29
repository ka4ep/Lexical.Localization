// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class LineAppender
    {
        private readonly static ILineFactory instance_non_resolving = new LineAppender(null);
        private readonly static ILineFactory instance_resolving = new LineAppender(ResolverSet.Instance);

        /// <summary>
        /// Default appender. Does not resolve parameters to respective instance.
        /// </summary>
        public static ILineFactory Default => instance_non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolver.
        /// </summary>
        public static ILineFactory Resolving => instance_resolving;

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        public readonly IResolver Resolver;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        public LineAppender(IResolver resolver = null)
        {
            this.Resolver = resolver;
        }
    }

    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class StringLocalizerAppender
    {
        private readonly static ILineFactory instance_non_resolving = new StringLocalizerAppender(null);
        private readonly static ILineFactory instance_resolving = new StringLocalizerAppender(ResolverSet.Instance);

        /// <summary>
        /// Default appender. Does not resolve parameters to respective instance.
        /// </summary>
        public static ILineFactory Default => instance_non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolver.
        /// </summary>
        public static ILineFactory Resolving => instance_resolving;

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        public readonly IResolver Resolver;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        public StringLocalizerAppender(IResolver resolver = null)
        {
            this.Resolver = resolver;
        }

    }

}
