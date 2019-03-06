using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;

namespace TutorialLibrary3
{
    internal class LibraryLocalization : StringLocalizerRoot.LinkedTo
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

        LibraryLocalization(IAssetRoot linkedTo) : base(linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new LibraryAssets());
            // Apply changes
            Builder.Build();
        }
    }
}
