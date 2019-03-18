using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        public LibraryAssetSources() : base()
        {
            // Create source that reads an embedded resource.
            IAssetSource internalLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary3-de.xml");
            // Asset sources are added here
            Add(internalLocalizationSource);
        }

        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                IAssetSource externalLocalizationSource = XmlLocalizationReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(externalLocalizationSource);
            }
        }
    }
}
