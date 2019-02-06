using Lexical.Asset;
using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_14
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create part that searches for localization.ini files in every folder.
            IAssetLoaderPart part = new AssetLoaderPartFileStrings("{location/}localization{-culture}.ini");
            // Set root path
            part.Options.AddPath(".");
            // Add option to match "{location}" parameter to existing files.
            part.Options.AddMatchParameter("location");
            // Create asset loader
            IAssetLoader assetLoader = new AssetLoader(part);
            #endregion Snippet

            // Create key
            IAssetKey key = new LocalizationRoot(assetLoader).Section("controller").Key("ok").SetCulture("de");
            // Asset loader loads filename "Assets/localization-de.ini" and then searches for key "controller:ok".
            string str = key.ToString();
        }
    }

}
