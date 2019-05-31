using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class AssetSourcesB : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Default.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        /// <summary>
        /// (Optional) External file localization source.
        /// </summary>
        public readonly LineFileSource ExternalLocalizationSource = 
            LineReaderMap.Default.FileAssetSource("Localization.xml", throwIfNotFound: false);

        public AssetSourcesB() : base()
        {
            // Add internal localization source
            Add(LocalizationSource);
            // Add optional external localization source
            Add(ExternalLocalizationSource);
        }
    }
}
