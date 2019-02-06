using System.Collections.Generic;
using Lexical.Asset;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_3
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create asset
            var source = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            IAsset asset = new LocalizationStringDictionary(source);

            // Create cache
            IAssetCache asset_cached = asset.CreateCache();

            // Configure options
            asset_cached.Options.SetCloneKeys(true);
            asset_cached.Options.SetCacheStreams(true);
            asset_cached.Options.SetMaxResourceCount(1024);
            asset_cached.Options.SetMaxResourceSize(1024 * 1024);
            asset_cached.Options.SetMaxResourceTotalSize(1024 * 1024 * 1024);

            // Assign the asset with cache decoration
            LocalizationRoot.Global.Asset = asset_cached;
            #endregion Snippet
        }
    }

}
