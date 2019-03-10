using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary2
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public readonly IAssetSource EmbeddedLocalizationSource;

        public LibraryAssets() : base()
        {
            // Asset sources are added here
            EmbeddedLocalizationSource = XmlLocalizationReader.Instance.EmbeddedAssetSource(
                    asm: GetType().Assembly,
                    resourceName: "docs.LibraryLocalization2-de.xml");

            Add(EmbeddedLocalizationSource);
        }
    }
}
