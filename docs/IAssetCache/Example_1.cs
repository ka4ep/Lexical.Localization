using System.Collections.Generic;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_1
    {
        public static void Main(string[] args)
        {
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new LocalizationAsset(source, LineFormat.Instance);

            #region Snippet
            // Create cache decorator
            IAssetCache asset_cached = new AssetCache(asset).AddResourceCache().AddStringsCache().AddCulturesCache();
            #endregion Snippet

            // Assign the cached asset
            LineRoot.Global.Asset = asset_cached;
        }
    }

}
