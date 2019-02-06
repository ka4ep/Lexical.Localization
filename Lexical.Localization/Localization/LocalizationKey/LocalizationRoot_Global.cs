// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;

namespace Lexical.Localization
{
    /// <summary>
    /// Static global singleton instance.
    /// 
    /// Localization is one of those cases where global singleton instance make sense.
    /// Even in inversion of control use case.
    /// 
    /// This is because localized string is considered to be akin to interned string. 
    /// They are both static and something that should be compile-time data. 
    /// Language strings, however, are run-time data due to practical logistics.
    /// </summary>
    public partial class LocalizationRoot
    {
        private static readonly LocalizationRoot.Mutable instance;
        private static readonly dynamic dynamic;
        private static readonly AssetBuilder builder;

        /// <summary>
        /// Dynamic reference to the singleton instance of localization root.
        /// </summary>
        public static dynamic GlobalDynamic => dynamic;

        /// <summary>
        /// Singleton instance to localization root. 
        /// </summary>
        public static LocalizationRoot.Mutable Global => instance;

        /// <summary>
        /// Asset builder. 
        /// 
        /// Add <see cref="IAssetSource"/>s here.
        /// Then call <see cref="IAssetBuilder.Build"/>.
        /// </summary>
        public static IAssetBuilder Builder => builder;

        static LocalizationRoot()
        {
            var _culturePolicy = new CulturePolicy();
            var _builder = new AssetBuilder.OneBuildInstance();
            builder = _builder;
            instance = new LocalizationRoot.Mutable(_builder.Asset, _culturePolicy);
            dynamic = instance;
        }
    }
}
