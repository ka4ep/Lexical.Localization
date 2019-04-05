using System.Collections.Generic;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_0
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create asset
            var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
            IAsset asset = new LocalizationAsset(source, ParameterNamePolicy.Instance);

            // Create cache
            IAssetCache asset_cached = new AssetCache(asset);
            // Adds feature to cache IAssetResourceProvider specific requests
            asset_cached.Add(new AssetCachePartResources(asset_cached.Source, asset_cached.Options));
            // Adds feature to cache ILocalizationStringProvider specific requests
            asset_cached.Add(new AssetCachePartStrings(asset_cached.Source, asset_cached.Options));
            // Adds feature to cache ILocalizationAssetCultureCapabilities specific requests
            asset_cached.Add(new AssetCachePartCultures(asset_cached.Source, asset_cached.Options));

            // Assign the cached asset
            LocalizationRoot.Global.Asset = asset_cached;
            #endregion Snippet
        }
    }

}
