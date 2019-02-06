using System.Reflection;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_6
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create loader that can read ".png" files.
            IAssetLoaderPart part = new AssetLoaderPartEmbeddedResources("[Assembly.]Assets.icon{-key}{-culture}.png").AddAssembly(Assembly.GetExecutingAssembly());

            // Create asset loader
            IAssetLoader assetLoader = new AssetLoader(part);

            // Create key
            IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
            // Issue request. Asset loader matches to filename "docs.Assets.icon-ok-de.png".
            byte[] data = assetLoader.GetResource(key);
            #endregion Snippet
        }
    }

}
