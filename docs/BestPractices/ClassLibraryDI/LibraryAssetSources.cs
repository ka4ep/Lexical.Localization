using System.Collections.Generic;
using Lexical.Localization;
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

        public LibraryAssetSources(ILogger<LibraryAssetSources> logger) : this()
        {
            // Use service from dependency injection
            logger?.LogInformation("Initializing LibraryAssetSources.");
        }
    }
}
