using Lexical.Localization;

namespace TutorialLibrary3
{
    internal class LibraryLocalization : StringLocalizerRoot.LinkedTo, IAssetRoot
    {
        private static readonly LibraryLocalization instance = new LibraryLocalization(LocalizationRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static LibraryLocalization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LocalizationRoot.Builder;

        LibraryLocalization(IAssetRoot linkedTo) : base(null, linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new LibraryAssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}
