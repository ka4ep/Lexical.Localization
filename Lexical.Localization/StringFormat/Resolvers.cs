// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// A group of resolvers
    /// </summary>
    public class Resolvers
    {
        private static Resolvers instance = new Resolvers();

        /// <summary>
        /// Default instance
        /// </summary>
        public static Resolvers Instance => instance;

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
        public Resolvers()
        {
            this.StringFormatResolver = Lexical.Localization.StringFormat.StringFormatResolver.Default;
            this.FunctionsResolver = Lexical.Localization.StringFormat.FunctionsResolver.Default;
            this.FormatProviderResolver = Lexical.Localization.StringFormat.FormatProviderResolver.Default;
            this.PluralRulesResolver = Lexical.Localization.Plurality.PluralRulesResolver.Default;
        }

        /// <summary>
        /// Create string resolver 
        /// </summary>
        /// <param name="stringFormatResolver"></param>
        /// <param name="functionsResolver"></param>
        /// <param name="formatProviderResolver"></param>
        /// <param name="pluralRulesResolver"></param>
        public Resolvers(IResolver<IStringFormat> stringFormatResolver, IResolver<IFunctions> functionsResolver, IResolver<IFormatProvider> formatProviderResolver, IResolver<IPluralRules> pluralRulesResolver)
        {
            this.StringFormatResolver = stringFormatResolver;
            this.FunctionsResolver = functionsResolver;
            this.FormatProviderResolver = formatProviderResolver;
            this.PluralRulesResolver = pluralRulesResolver;
        }


    }
}
