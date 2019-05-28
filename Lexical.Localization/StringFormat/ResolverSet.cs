// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Plurality;
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// A group of resolvers
    /// </summary>
    public class ResolverSet
    {
        private static ResolverSet instance = new ResolverSet();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ResolverSet Instance => instance;

        /// <summary>
        /// String resolver
        /// </summary>
        public IStringResolver StringResolver { get; protected set; }

        /// <summary>
        /// String format resolver
        /// </summary>
        public IResolver<IStringFormat> StringFormatResolver { get; protected set; }

        /// <summary>
        /// Functions resolver
        /// </summary>
        public IResolver<IFunctions> FunctionsResolver { get; protected set; }

        /// <summary>
        /// Format provider resolver
        /// </summary>
        public IResolver<IFormatProvider> FormatProviderResolver { get; protected set; }

        /// <summary>
        /// Plural rules resolver
        /// </summary>
        public IResolver<IPluralRules> PluralRulesResolver { get; protected set; }

        /// <summary>
        /// Create string resolver 
        /// </summary>
        public ResolverSet()
        {
            this.StringResolver = Lexical.Localization.StringFormat.StringResolver.Default;
            this.StringFormatResolver = Lexical.Localization.StringFormat.StringFormatResolver.Default;
            this.FunctionsResolver = Lexical.Localization.StringFormat.FunctionsResolver.Default;
            this.FormatProviderResolver = Lexical.Localization.StringFormat.FormatProviderResolver.Default;
            this.PluralRulesResolver = Lexical.Localization.Plurality.PluralRulesResolver.Default;
        }

        /// <summary>
        /// Create string resolver 
        /// </summary>
        /// <param name="stringResolver"></param>
        /// <param name="stringFormatResolver"></param>
        /// <param name="functionsResolver"></param>
        /// <param name="formatProviderResolver"></param>
        /// <param name="pluralRulesResolver"></param>
        public ResolverSet(IStringResolver stringResolver, IResolver<IStringFormat> stringFormatResolver, IResolver<IFunctions> functionsResolver, IResolver<IFormatProvider> formatProviderResolver, IResolver<IPluralRules> pluralRulesResolver)
        {
            this.StringResolver = stringResolver;
            this.StringFormatResolver = stringFormatResolver;
            this.FunctionsResolver = functionsResolver;
            this.FormatProviderResolver = formatProviderResolver;
            this.PluralRulesResolver = pluralRulesResolver;
        }

        /// <summary>
        /// Resolves parameters and keys into instances.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is resolved to <see cref="ILineCulture"/></item>
        ///     <item>Parameter "Value" is resolved to <see cref="ILineValue"/></item>
        ///     <item>Parameter "StringFormat" is resolved to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is resolved to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is resolved to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is resolved to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// 
        /// Parts that don't need resolving may be need to be cloned. 
        /// If the line appender of <paramref name="line"/> fails cloning, then the operation 
        /// fails and returns false.
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="modifiedLine"></param>
        /// <returns>true if operation was successful, <paramref name="modifiedLine"/> contains new line. If operation failed, <paramref name="modifiedLine"/> contains the reference to <paramref name="line"/>.</returns>
        public bool TryResolveParameters(ILine line, out ILine modifiedLine)
        {
            // TODO IMPLEMENT
            modifiedLine = line;
            return false;
        }


    }
}
