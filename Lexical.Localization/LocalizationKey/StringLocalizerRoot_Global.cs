// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.Localization;

namespace Lexical.Localization
{
    /// <summary>
    /// Global singleton instance with compability to Lexical.Localization and Microsoft.Extensions.Localization.
    /// 
    /// This singleton provides keys for both frameworks, instances of <see cref="IStringLocalizer"/> and <see cref="ILine"/>.
    /// 
    /// This singleton uses the same asset and builder references as <see cref="LocalizationRoot"/> singleton.
    /// </summary>
    public partial class StringLocalizerRoot
    {
        private static readonly StringLocalizerRoot instance = new StringLocalizerRoot.LinkedTo(StringLocalizerPartAppender.Instance, LocalizationRoot.Global);
        private static readonly dynamic dynamic;

        /// <summary>
        /// Dynamic reference to the singleton instance of localization root.
        /// </summary>
        public static dynamic GlobalDynamic => dynamic;

        /// <summary>
        /// Singleton instance to localization root. 
        /// Implements 
        ///     <see cref="IStringLocalizer"/>
        ///     <see cref="IStringLocalizerFactory"/>
        ///     <see cref="ILine"/>
        ///     <see cref="IAssetRoot"/>
        /// </summary>
        public static StringLocalizerRoot Global => instance;

        /// <summary>
        /// Asset builder. 
        /// 
        /// Add <see cref="IAssetSource"/>s here.
        /// Then call <see cref="IAssetBuilder.Build"/>.
        /// </summary>
        public static IAssetBuilder Builder => LocalizationRoot.Builder;

        static StringLocalizerRoot()
        {
            instance = new StringLocalizerRoot.LinkedTo(StringLocalizerPartAppender.Instance, LocalizationRoot.Global);
            dynamic = instance;
        }
    }
}
