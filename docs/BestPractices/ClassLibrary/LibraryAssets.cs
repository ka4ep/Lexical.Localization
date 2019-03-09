using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    [AssetSources]
    public class LibraryAssets : List<IAssetSource>
    {
        public LibraryAssets() : base()
        {
            // Asset sources are added here
            Add(XmlLocalizationReader.Instance.CreateEmbeddedAssetSource(
                    asm: GetType().Assembly, 
                    resourceName: "docs.LibraryLocalization1-de.xml")
            );
        }
    }
}
