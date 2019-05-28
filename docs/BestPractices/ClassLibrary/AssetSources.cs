using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class AssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LineEmbeddedSource LocalizationSource = 
            LineReaderMap.Instance.EmbeddedAssetSource(typeof(AssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        public AssetSources() : base()
        {
            // Asset sources are added here
            Add(LocalizationSource);
        }
    }
}
