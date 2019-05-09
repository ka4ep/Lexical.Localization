// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.Localization;

namespace Lexical.Localization
{
    /// <summary>
    /// Static global singleton instance.
    /// 
    /// Singleton localization string is akin to interned string, they are both static.
    /// </summary>
    public partial class LineRoot
    {
        private static readonly LineRoot.Mutable instance;
        private static readonly dynamic dynamic;
        private static readonly AssetBuilder builder;

        /// <summary>
        /// Dynamic reference to the singleton instance of localization root.
        /// </summary>
        public static dynamic GlobalDynamic => dynamic;

        /// <summary>
        /// Singleton instance to localization root. 
        /// </summary>
        public static LineRoot Global => instance;

        /// <summary>
        /// Asset builder. 
        /// 
        /// Add <see cref="IAssetSource"/>s here.
        /// Then call <see cref="IAssetBuilder.Build"/>.
        /// </summary>
        public static IAssetBuilder Builder => builder;

        static LineRoot()
        {
            var _culturePolicy = new CulturePolicy();
            var _builder = new AssetBuilder.OneBuildInstance();
            builder = _builder;
            instance = new LineRoot.Mutable(LineAppender.Default, _builder.Asset, _culturePolicy, LocalizationResolver.Instance);
            dynamic = instance;
        }
    }

    /// <summary>
    /// Global singleton instance with compability to Lexical.Localization and Microsoft.Extensions.Localization.
    /// 
    /// This singleton provides keys for both frameworks, instances of <see cref="IStringLocalizer"/> and <see cref="ILine"/>.
    /// 
    /// This singleton uses the same asset and builder references as <see cref="LineRoot"/> singleton.
    /// </summary>
    public partial class StringLocalizerRoot
    {
        private static readonly StringLocalizerRoot instance = new StringLocalizerRoot.LinkedTo(StringLocalizerAppender.Default, LineRoot.Global);
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
        ///     <see cref="ILineRoot"/>
        /// </summary>
        public static StringLocalizerRoot Global => instance;

        /// <summary>
        /// Asset builder. 
        /// 
        /// Add <see cref="IAssetSource"/>s here.
        /// Then call <see cref="IAssetBuilder.Build"/>.
        /// </summary>
        public static IAssetBuilder Builder => LineRoot.Builder;

        static StringLocalizerRoot()
        {
            instance = new StringLocalizerRoot.LinkedTo(StringLocalizerAppender.Default, LineRoot.Global);
            dynamic = instance;
        }
    }
}
