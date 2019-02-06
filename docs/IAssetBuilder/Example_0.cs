using System;
using System.Collections.Generic;
using Lexical.Asset;
using Lexical.Localization;

namespace docs
{
    public class IAssetBuilder_Example_0
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create dictionary of strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };

            // Create IAssetSource that adds cache 
            IAssetSource assetSource_0 = new AssetCacheSource(c => c.AddResourceCache().AddStringsCache().AddCulturesCache());
            // Create IAssetSource that static reference of IAsset (string dictionary)
            IAssetSource assetSource_1 = new AssetSource(new LocalizationStringDictionary(strings, AssetKeyNameProvider.Default) );

            // Create AssetBuilder
            IAssetBuilder builder = new AssetBuilder(assetSource_0, assetSource_1);
            // Instantiate IAsset
            IAsset asset = builder.Build();

            // Create string key
            IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
            // Request string
            string str = asset.GetString( key );
            // Print result
            Console.WriteLine(str);
            #endregion Snippet
        }
    }

}
