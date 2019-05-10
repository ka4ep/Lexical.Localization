using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary3-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LineFileProviderSource ExternalLocalizationSource;

        public LibraryAssetSources() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
        }

        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external localization source
            if (fileProvider != null)
            {
                ExternalLocalizationSource = 
                    LineXmlReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary3.xml", throwIfNotFound: false);
                Add(ExternalLocalizationSource);
            }
        }
    }
}
