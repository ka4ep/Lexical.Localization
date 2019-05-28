using Lexical.Localization;

namespace TutorialLibrary3
{
    internal class Localization : StringLocalizerRoot.LinkedTo, ILineRoot
    {
        private static readonly Localization instance = new Localization(LineRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static Localization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LineRoot.Builder;

        Localization(ILineRoot linkedTo) : base(null, linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new AssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}
