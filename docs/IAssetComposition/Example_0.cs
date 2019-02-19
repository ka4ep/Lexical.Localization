using System.Collections.Generic;
using System.Reflection;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAssetComposition_Example
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create individual assets
            IAsset asset_1 = new LocalizationStringDictionary(new Dictionary<string, string> { { "en:hello", "Hello World!" } });
            IAsset asset_2 = new AssetResourceDictionary(new Dictionary<string, byte[]> { { "en:Hello.Icon", new byte[] { 1, 2, 3 } } });
            IAsset asset_3 = new AssetLoader().Add( new AssetLoaderPartEmbeddedStrings("[assembly.]localization{-culture}.ini", AssetKeyNameProvider.Default ).AddAssembly(Assembly.GetExecutingAssembly()).AddMatchParameters("assembly"));

            // Create composition asset
            IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2, asset_3);

            // Assign the composition to root
            IAssetRoot root = new LocalizationRoot(asset_composition, new CulturePolicy());
            #endregion Snippet
        }
    }

}
