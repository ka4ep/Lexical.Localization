// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           2.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class LineAppender : ILineFactoryResolver, ILineFactoryParameterInfos
    {
        private readonly static ILineFactory non_resolving = new LineAppender(null, Lexical.Localization.Utils.ParameterInfos.Default);
        private readonly static ILineFactory resolving = new LineAppender(Resolvers.Default, Lexical.Localization.Utils.ParameterInfos.Default);

        /// <summary>
        /// Default appender. Does not resolve parameters to instances.
        /// </summary>
        public static ILineFactory NonResolving => non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolvers.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "String" is created as to <see cref="ILineString"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// </summary>
        public static ILineFactory Default => resolving;

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        public IResolver Resolver { get => resolver; set => throw new InvalidOperationException("Immutable"); }

        /// <summary>
        /// (optional) Used for instantiating Hint, non-canonical key or canonical key, when parameter is requested.
        /// </summary>
        public IParameterInfos ParameterInfos { get => parameterInfos; set => throw new InvalidOperationException("Immutable"); }

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        protected IResolver resolver;

        /// <summary>
        /// (optional) Used for instantiating Hint, non-canonical key or canonical key, when parameter is requested.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="parameterInfos"></param>
        public LineAppender(IResolver resolver = null, IParameterInfos parameterInfos = null)
        {
            this.resolver = resolver;
            this.parameterInfos = parameterInfos;
        }
    }

    /// <summary>
    /// Default part appender.
    /// </summary>
    public partial class StringLocalizerAppender : ILineFactoryResolver, ILineFactoryParameterInfos
    {
        private readonly static ILineFactory non_resolving = new StringLocalizerAppender(null, Lexical.Localization.Utils.ParameterInfos.Default);
        private readonly static ILineFactory resolving = new StringLocalizerAppender(Resolvers.Default, Lexical.Localization.Utils.ParameterInfos.Default);

        /// <summary>
        /// Default appender. Does not resolve parameters to respective instance.
        /// </summary>
        public static ILineFactory NonResolving => non_resolving;

        /// <summary>
        /// Appender that resolves parameters to respective instances with default resolver.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "String" is created as to <see cref="ILineString"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// </summary>
        public static ILineFactory Default => resolving;

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        public IResolver Resolver { get => resolver; set => throw new InvalidOperationException("Immutable"); }

        /// <summary>
        /// (optional) Used for instantiating Hint, non-canonical key or canonical key, when parameter is requested.
        /// </summary>
        public IParameterInfos ParameterInfos { get => parameterInfos; set => throw new InvalidOperationException("Immutable"); }

        /// <summary>
        /// (optional) Type and parameter resolver
        /// </summary>
        protected IResolver resolver;

        /// <summary>
        /// (optional) Used for instantiating Hint, non-canonical key or canonical key, when parameter is requested.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Create new part factory with default factories.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="parameterInfos"></param>
        public StringLocalizerAppender(IResolver resolver = null, IParameterInfos parameterInfos = null)
        {
            this.resolver = resolver;
            this.parameterInfos = parameterInfos;
        }

    }

}
