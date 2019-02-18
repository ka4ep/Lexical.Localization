using System;
using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LocalizationDictionary_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                // Create localization source
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                // Create asset
                IAsset asset = new LocalizationDictionary(source);
                #endregion Snippet_1
                // Create root 
                IAssetRoot root = new LocalizationRoot(asset);
            }
            {
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                #region Snippet_2
                // Create asset with name pattern
                IAsset asset = new LocalizationDictionary(source, "{culture:}{type:}{key}");
                // Extract all keys
                foreach (IAssetKey key in asset.GetAllKeys())
                    Console.WriteLine(key);
                #endregion Snippet_2

                #region Snippet_3
                // Keys can be filtered
                foreach (IAssetKey key in asset.GetAllKeys(LocalizationRoot.Global.SetCulture("de")))
                    Console.WriteLine(key);
                #endregion Snippet_3

            }
        }
    }

}
