using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>, ILibraryAssetSources
    {
        public readonly IAssetSource EmbeddedLocalizationSource;

        public LibraryAssets() : base()
        {
            // Asset sources are added here
            EmbeddedLocalizationSource = XmlLocalizationReader.Instance.EmbeddedAssetSource(
                    asm: GetType().Assembly,
                    resourceName: "docs.LibraryLocalization1-de.xml");

            Add(EmbeddedLocalizationSource);
        }
    }
}
