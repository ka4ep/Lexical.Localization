using Lexical.Asset;
using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_4
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create loader that can read ".png" files.
            IAssetLoaderPart part = new AssetLoaderPartFileResources(@"Assets/icon{-key}{-culture}.png").AddPaths(".");

            // Create asset loader
            IAssetLoader assetLoader = new AssetLoader(part);

            // Create key
            IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
            // Issue request. Asset loader matches to filename "Assets\icon-ok-de.png".
            byte[] data = assetLoader.GetResource(key);
            #endregion Snippet
        }
    }

}
