using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace docs
{
    public class IAssetLoader_Example_8
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Create part that reads strings from ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, @"Assets\localization{-culture}.ini", AssetFileConstructors.Ini);

            // Create loader
            IAssetLoader assetLoader = new AssetLoader(part);
            #endregion Snippet

            // Create key
            IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("de");
            // Issue request. Asset loader matches filename "Assets\localization-de.ini".
            assetLoader.GetString(key);
        }
    }

}
