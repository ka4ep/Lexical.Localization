using System;
using System.Collections.Generic;
using Lexical.Asset;
using Lexical.Localization;

namespace docs
{
    public class IAssetCache_Example_4
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create asset
            var source = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            IAsset asset = new LocalizationStringDictionary(source);

            // Cache it
            asset = asset.CreateCache();

            // Issue a request which will be cached.
            IAssetKey key = new LocalizationRoot().Key("hello");
            string str = asset.GetString( key.SetCulture("en") );
            Console.WriteLine(str);

            // Clear cache
            asset.Reload();
            #endregion Snippet
        }
    }

}
