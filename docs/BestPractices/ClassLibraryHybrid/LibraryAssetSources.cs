using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;

namespace TutorialLibrary3
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// A local embedded localization file.
        /// </summary>
        public IAssetSource InternalLocalizationSource = LocalizationReaderMap.Instance.EmbeddedAssetSource(
                asm: typeof(LibraryAssetSources).Assembly,
                resourceName: "docs.TutorialLibrary3-de.xml");

        /// <summary>
        /// External optional localization file.
        /// </summary>
        public IAssetSource ExternalLocalizationSource;

        public LibraryAssetSources() : base()
        {
            // Asset sources are added here
            Add(InternalLocalizationSource);
        }

        /// <summary>
        /// Constructor that uses services from dependency injection.
        /// </summary>
        /// <param name="fileProvider"></param>
        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for an optional external file
            if (fileProvider != null)
            {
                string filepath = "Resources/TutorialLibrary3.xml";
                ExternalLocalizationSource = XmlLocalizationReader.Instance.FileProviderAssetSource(
                    fileProvider: fileProvider,
                    filepath: filepath,
                    throwIfNotFound: false);
                Add(ExternalLocalizationSource);
            }
        }
    }
}
