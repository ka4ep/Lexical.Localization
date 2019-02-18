using System.Collections.Generic;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_1
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // Create asset
            var source = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            IAsset asset = new LocalizationDictionary(source);

            #region Snippet
            // Create cache decorator
            IAssetCache asset_cached = new AssetCache(asset).AddResourceCache().AddStringsCache().AddKeysCache().AddCulturesCache();
            #endregion Snippet

            // Assign the cached asset
            LocalizationRoot.Global.Asset = asset_cached;
        }
    }

}
