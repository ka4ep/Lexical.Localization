using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary2
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LocalizationEmbeddedSource LocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary2-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LocalizationFileProviderSource ExternalLocalizationSource;

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
                ExternalLocalizationSource = LocalizationXmlReader.Instance.FileProviderAssetSource(fileProvider, "Resources/TutorialLibrary2.xml", throwIfNotFound: false);
                Add(ExternalLocalizationSource);
            }
        }
    }
}
