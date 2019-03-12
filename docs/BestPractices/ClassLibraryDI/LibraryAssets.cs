using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.Logging;

namespace TutorialLibrary2
{
    public class LibraryAssets : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// This is an asset source to local embedded resource
        /// </summary>
        public readonly IAssetSource EmbeddedLocalizationSource = XmlLocalizationReader.Instance.EmbeddedAssetSource(
                    asm: typeof(LibraryAssets).Assembly,
                    resourceName: "docs.LibraryLocalization2-de.xml");

        public LibraryAssets() : base()
        {
            // Asset sources are added here
            Add(EmbeddedLocalizationSource);
        }

        public LibraryAssets(ILogger<LibraryAssets> logger) : this()
        {
            // Use service from dependency injection
            logger?.LogInformation("Initializing LibraryAssets.");
        }
    }
}
