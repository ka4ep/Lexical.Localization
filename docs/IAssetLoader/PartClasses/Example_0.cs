using Lexical.Localization;

namespace docs
{
    public class IAssetLoader_Example_0
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create asset loader
            IAssetLoader assetLoader = new AssetLoader();

            // Create part that reads strings from ".ini" files.
            IAssetLoaderPart part_1 = new AssetLoaderPartFileStrings("Assets/localization{-culture}.ini", "{anysection_0:}{anysection_1:}{anysection_n:}[key]").AddPath(".");

            // Add part to loader
            assetLoader.Add(part_1);

            // Create key
            IAssetKey key = new LocalizationRoot().Section("controller").Key("hello").SetCulture("de");

            // Issue request. Asset loader matches to filename "Assets\localization-de.ini", loads it,
            // and then searches for string with key "controller:hello".
            string str = assetLoader.GetString(key);
            #endregion Snippet
        }
    }

}
