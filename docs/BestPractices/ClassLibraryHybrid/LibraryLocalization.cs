using Lexical.Localization;

namespace TutorialLibrary3
{
    internal class LibraryLocalization : StringLocalizerRoot.LinkedTo, ILineRoot
    {
        private static readonly LibraryLocalization instance = new LibraryLocalization(LineRoot.Global);

        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static LibraryLocalization Root => instance;

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        public new static IAssetBuilder Builder => LineRoot.Builder;

        LibraryLocalization(ILineRoot linkedTo) : base(null, linkedTo)
        {
            // Add library's internal assets here
            Builder.AddSources(new LibraryAssetSources());
            // Apply changes
            Builder.Build();
        }
    }
}
