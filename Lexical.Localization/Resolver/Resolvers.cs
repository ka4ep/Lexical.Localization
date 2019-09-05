// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.5.2019
// Url:            http://lexical.fi
// --------------------------------------------------------

using Lexical.Localization.StringFormat;

namespace Lexical.Localization.Resolver
{
    /// <summary>
    /// Set of resolvers that resolve parameters values into instances.
    /// Resolves following parameter names: "StringFormat", "Functions", "FormatProvider", "PluralRules", "Culture", "CulturePolicy", "StringResolver", "BinaryResolver".
    /// </summary>
    public class Resolvers : ResolverComposition
    {
        private static IResolver instance = new ResolverComposition()
            .Add(Lexical.Localization.StringFormat.StringFormatResolver.Default)
            .Add(Lexical.Localization.StringFormat.FunctionsResolver.Default)
            .Add(Lexical.Localization.StringFormat.FormatProviderResolver.Default)
            .Add(Lexical.Localization.Plurality.PluralRulesResolver.Default)
            .Add(Lexical.Localization.CultureResolver.Default)
            .Add(Lexical.Localization.CulturePolicyResolver.Default)
            .Add(Lexical.Localization.StringFormat.StringResolverResolver.Default)
            .Add(Lexical.Localization.Binary.BinaryResolverResolver.Default)
            //.Add(Lexical.Localization.StringFormat.TypeResolver.Default)  // <-- No practical reasons to resolve "Type" parameters. String keys do better. Enabling this makes unnecessary resolves.
            .ReadOnly();

        /// <summary>
        /// Default instance
        /// </summary>
        public static IResolver Default => instance;
    }
}
