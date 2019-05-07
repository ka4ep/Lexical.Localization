// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Static global singleton instance.
    /// 
    /// Singleton localization string is akin to interned string, they are both static.
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
        public static LocalizationRoot Global => instance;

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
            instance = new LocalizationRoot.Mutable(LineAppender.Instance, _builder.Asset, _culturePolicy, LocalizationResolver.Instance);
            dynamic = instance;
        }
    }
}
