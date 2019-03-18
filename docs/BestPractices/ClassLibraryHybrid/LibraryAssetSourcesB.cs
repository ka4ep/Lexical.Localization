using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class LibraryAssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSourcesB() : base()
        {
            // Create source that reads an embedded resource.
            IAssetSource InternalLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary3-de.xml");
            // Asset sources are added here
            Add(InternalLocalizationSource);
        }

        public LibraryAssetSourcesB(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external file
            if (fileProvider != null)
            {
                IAssetSource externalLocalizationSource = XmlLocalizationReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(externalLocalizationSource);
            }
        }
    }
}
