using Lexical.Localization;

namespace TutorialLibrary2
{
    internal class LibraryLocalization : LocalizationRoot.LinkedTo, IAssetRoot
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

        LibraryLocalization(IAssetRoot linkedTo) : base(linkedTo, null, null, null, null, null)
        {
            // Add library's internal assets here
            Builder.AddSources(new LibraryAssetSources(null));
            // Apply changes
            Builder.Build();
        }
    }
}
