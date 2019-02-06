// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    /// <summary>
    /// Static global singleton instance.
    /// </summary>
    public partial class AssetRoot
    {
        private static readonly AssetRoot.Mutable instance;
        private static readonly dynamic dynamic;
        private static readonly AssetBuilder builder;

        /// <summary>
        /// Dynamic reference to the singleton instance of asset root.
        /// </summary>
        public static dynamic GlobalDynamic => dynamic;

        /// <summary>
        /// Singleton instance to asset root. 
        /// </summary>
        public static AssetRoot.Mutable Global => instance;

        /// <summary>
        /// Asset builder. 
        /// 
        /// Add <see cref="IAssetSource"/>s here.
        /// Then call <see cref="IAssetBuilder.Build"/>.
        /// </summary>
        public static IAssetBuilder Builder => builder;

        static AssetRoot()
        {
            var _builder = new AssetBuilder.OneBuildInstance();
            builder = _builder;
            instance = new AssetRoot.Mutable(_builder.Asset);
            dynamic = instance;
        }
    }
}
