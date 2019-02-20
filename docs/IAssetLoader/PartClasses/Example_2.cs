using Lexical.Localization;
using System.Reflection;

namespace docs
{
    public class IAssetLoader_Example_2
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create part that reads strings from embedded ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartEmbeddedStrings(@"[assembly.]Assets.localization{-culture}.ini")
                .AddAssembly(Assembly.GetExecutingAssembly());

            // Create asset loader
            AssetLoader assetLoader = new AssetLoader( part );

            // Create key
            IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("de");

            // Issue request. Asset loader matches culture "de" to filename and tries to load "namespace.Assets.localization-de.ini".
            assetLoader.GetString(key);
            #endregion Snippet
        }
    }

}
