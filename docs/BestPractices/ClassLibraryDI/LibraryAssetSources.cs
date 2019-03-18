using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary2
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSources() : base()
        {
            // Create source that reads embedded resource
            IAssetSource embeddedLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary2-de.xml");
            // Asset sources are added here
            Add(embeddedLocalizationSource);
        }
    }
}
