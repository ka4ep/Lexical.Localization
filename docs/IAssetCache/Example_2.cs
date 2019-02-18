using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_2
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // Create asset
            var source = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            IAsset asset = new LocalizationDictionary(source);

            #region Snippet
            // Decorate with cache
            IAssetCache asset_cached = asset.CreateCache();
            #endregion Snippet

            // Assign the asset with cache decoration
            LocalizationRoot.Global.Asset = asset_cached;
        }
    }

}
