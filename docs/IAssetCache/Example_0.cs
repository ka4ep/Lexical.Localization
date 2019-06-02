using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace docs
{
    public class IAssetCache_Example_0
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new StringAsset(source, LineFormat.Parameters);

            // Create cache
            IAssetCache asset_cached = new AssetCache(asset);
            // Adds feature to cache IResourceAsset specific requests
            asset_cached.Add(new AssetCachePartResources(asset_cached.Source, asset_cached.Options));
            // Adds feature to cache IStringAsset specific requests
            asset_cached.Add(new AssetCachePartStrings(asset_cached.Source, asset_cached.Options));
            // Adds feature to cache IAssetCultureEnumerable specific requests
            asset_cached.Add(new AssetCachePartCultures(asset_cached.Source, asset_cached.Options));

            // Assign the cached asset
            LineRoot.Global.Asset = asset_cached;
            #endregion Snippet
        }
    }

}
