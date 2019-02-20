using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_15
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create part
            IAssetLoaderPart part =
                new AssetLoaderPartFileStrings(
                    filenamePattern: "localization{-culture}.ini",
                    keyNamePattern: AssetKeyNameProvider.None_Colon_Colon)
                .AddPath(".");

            // Create asset laoder
            IAsset asset = new AssetLoader(part);

            // Create key
            IAssetKey key = new LocalizationRoot(asset).Section("controller").Key("hello").SetCulture("de");

            // Issue request. Asset loader matches to filename "localization-de.ini", loads it,
            // and then searches for string with key "controller:hello".
            string str = key.ToString();
            #endregion Snippet
        }
    }

}
