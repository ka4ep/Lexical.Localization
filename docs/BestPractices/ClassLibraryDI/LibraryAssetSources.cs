using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace TutorialLibrary2
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Asset source to a local embedded resource.
        /// </summary>
        public readonly IAssetSource EmbeddedLocalizationSource = 
                LocalizationReaderMap.Instance.EmbeddedAssetSource(
                    asm: typeof(LibraryAssetSources).Assembly,
                    resourceName: "docs.LibraryLocalization2-de.xml");

        public LibraryAssetSources() : base()
        {
            // Asset sources are added here
            Add(EmbeddedLocalizationSource);
        }

        /// <summary>
        /// Constructor that uses services from dependency injection.
        /// </summary>
        /// <param name="fileProvider"></param>
        public LibraryAssetSources(IFileProvider fileProvider) : this()
        {
            // Use file provider from dependency injection and search for
            // possible well-known localization file for this class library.
            if (fileProvider!=null)
            {
                string filepath = "App_GlobalResources/TutorialLibrary2.xml";
                if (fileProvider.GetFileInfo(filepath).Exists)
                {
                    IAssetSource externalLocalizationSource =
                        XmlLocalizationReader.Instance.FileProviderAssetSource(
                            fileProvider: fileProvider,
                            filepath: filepath);
                    Add(externalLocalizationSource);
                }
            }
        }
    }
}
