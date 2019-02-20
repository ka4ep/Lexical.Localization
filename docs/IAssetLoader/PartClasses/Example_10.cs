using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace docs
{
    public class IAssetLoader_Example_10
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            #region Snippet
            // Create part that reads strings from ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartFileProviderResources(fileProvider, @"Assets\icon{-key}{-culture}.png");

            // Create loader
            IAssetLoader assetLoader = new AssetLoader(part);
            #endregion Snippet

            // Create key
            IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
            // Issue request. Asset loader matches to filename "Assets\icon-ok-de.png".
            byte[] data = assetLoader.GetResource(key);
        }
    }

}
