using System.Collections.Generic;
using Lexical.Localization;

namespace TutorialLibrary1
{
    public class LibraryAssetSources : List<IAssetSource>, ILibraryAssetSources
    {
        /// <summary>
        /// Localization source reference to embedded resource.
        /// </summary>
        public readonly LocalizationEmbeddedSource LocalizationSource = 
            LocalizationReaderMap.Instance.EmbeddedAssetSource(typeof(LibraryAssetSources).Assembly, "docs.TutorialLibrary1-de.xml");

        public LibraryAssetSources() : base()
        {
            // Asset sources are added here
            Add(LocalizationSource);
        }
    }
}
