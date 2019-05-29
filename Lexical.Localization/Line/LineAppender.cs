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
        private readonly static ILineFactory non_resolving = new LineAppender(null);
        private readonly static ILineFactory resolving = new LineAppender(ResolverSet.Instance);

        /// <summary>
        /// Default appender. Does not resolve parameters to respective instance.
        /// </summary>
        public static ILineFactory Default => non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolver.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "Value" is created as to <see cref="ILineValue"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// </summary>
        public static ILineFactory Resolving => resolving;

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
        private readonly static ILineFactory non_resolving = new StringLocalizerAppender(null);
        private readonly static ILineFactory resolving = new StringLocalizerAppender(ResolverSet.Instance);

        /// <summary>
        /// Default appender. Does not resolve parameters to respective instance.
        /// </summary>
        public static ILineFactory Default => non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolver.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "Value" is created as to <see cref="ILineValue"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// </summary>
        public static ILineFactory Resolving => resolving;

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
