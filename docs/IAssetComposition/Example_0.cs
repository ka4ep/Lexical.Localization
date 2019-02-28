using Lexical.Localization;
using System.Collections.Generic;
using System.Reflection;

namespace docs
{
    public class IAssetComposition_Example
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create individual assets
            IAsset asset_1 = new LocalizationStringAsset(new Dictionary<string, string> { { "en:hello", "Hello World!" } });
            IAsset asset_2 = new AssetResourceDictionary(new Dictionary<string, byte[]> { { "en:Hello.Icon", new byte[] { 1, 2, 3 } } });

            // Create composition asset
            IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2);

            // Assign the composition to root
            IAssetRoot root = new LocalizationRoot(asset_composition, new CulturePolicy());
            #endregion Snippet
        }
    }

}
