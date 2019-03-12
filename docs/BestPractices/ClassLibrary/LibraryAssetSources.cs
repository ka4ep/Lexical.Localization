using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Asset source to a local embedded resource.
        /// </summary>
        public readonly IAssetSource EmbeddedLocalizationSource = 
                LocalizationReaderMap.Instance.EmbeddedAssetSource(
                    asm: typeof(LibraryAssetSources).Assembly,
                    resourceName: "docs.TutorialLibrary1-de.xml");

        public LibraryAssetSources() : base()
        {
            // Asset sources are added here
            Add(EmbeddedLocalizationSource);
        }
    }
}
