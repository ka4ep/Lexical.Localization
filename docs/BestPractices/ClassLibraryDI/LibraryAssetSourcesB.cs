using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary2
{
    public class LibraryAssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSourcesB()
        {
            // Create source that reads embedded resource
            IAssetSource embeddedLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary2-de.xml");
            // Asset sources are added here
            Add(embeddedLocalizationSource);
        }

        public LibraryAssetSourcesB(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external file
            if (fileProvider != null)
            {
                // Create source that reads from file provider
                IAssetSource externalLocalizationSource = XmlLocalizationReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary2.xml", throwIfNotFound: false);
                Add(externalLocalizationSource);
            }
        }
    }
}
