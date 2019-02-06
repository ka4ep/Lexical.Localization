using System.Linq;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_12
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create asset loader
            IAssetLoader assetLoader = new AssetLoader();

            // Create part(s)
            IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
                .Path(".")                                                  // Add root directory to search files from
                .FilePattern("Assets/localization{-culture}.ini")           // Add file name pattern
                .KeyPattern("{anysection:}[key]")                           // Add key pattern to match within files
                .Strings()                                                  // Make this part return strings
                .Build().ToArray();                                         // Build part(s)

            // Add part(s)
            assetLoader.AddRange(parts);

            // Create key
            IAssetKey key = new LocalizationRoot(assetLoader).Section("controller").Key("ok").SetCulture("de");

            // Asset loader loads filename "Assets/localization-de.ini" and then searches for key "controller:ok".
            string str = key.ToString();
            #endregion Snippet
        }
    }

}
