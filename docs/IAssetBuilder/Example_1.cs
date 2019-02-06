using System;
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAssetBuilder_Example_1
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // Create dictionary of strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };

            #region Snippet
            // Create AssetBuilder
            IAssetBuilder builder = new AssetBuilder();
            // Add IAssetSource that adds cache 
            builder.AddCache();
            // Add IAssetSource that adds strings
            builder.AddDictionary(strings, AssetKeyNameProvider.Default);
            #endregion Snippet

            // Instantiate IAsset
            IAsset asset = builder.Build();

            // Create string key
            IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
            // Request string
            string str = asset.GetString(key);
            // Print result
            Console.WriteLine(str);
        }
    }

}
