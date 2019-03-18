using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class LibraryAssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSourcesB() : base()
        {
            // Create source that reads embedded resource.
            IAssetSource internalLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary1-de.xml");
            // Asset sources are added here
            Add(internalLocalizationSource);

            // Create source that searches for a file
            IAssetSource externalLocalizationSource = LocalizationReaderMap.Instance.FileAssetSource("docs.TutoarialLibrary1.xml");
            // Asset sources are added here
            Add(externalLocalizationSource);
        }
    }
}
