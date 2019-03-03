using System;
using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LocalizationStringAsset_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1a
                // Create localization source
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                // Create asset
                IAsset asset = new LocalizationStringAsset(source, AssetKeyNameProvider.Default);
                #endregion Snippet_1a

                #region Snippet_1b
                // Create root 
                IAssetRoot root = new LocalizationRoot(asset);
                // Create key. Name policy converts to .
                IAssetKey key = root.Section("MyController").Key("hello").Culture("de");
                // Test what identity is produced, "de:MyController:hello"
                string id = AssetKeyNameProvider.Default.BuildName(key);
                // Query language string
                string localizedString = key.ToString();
                #endregion Snippet_1b
            }
            {
                var source = new Dictionary<string, string> {
                    { "MyController:hello", "Hello World!" },
                    { "en:MyController:hello", "Hello World!" },
                    { "de:MyController:hello", "Hallo Welt!" }
                };
                #region Snippet_2a
                // Create asset with name pattern
                IAsset asset = new LocalizationStringAsset(source, "{culture:}{type:}{Key}");
                #endregion Snippet_2a
                #region Snippet_2b
                // Extract all keys
                foreach (IAssetKey key in asset.GetAllKeys())
                    Console.WriteLine(key);
                #endregion Snippet_2b

                #region Snippet_3
                // Keys can be filtered
                foreach (IAssetKey key in asset.GetAllKeys(LocalizationRoot.Global.Culture("de")))
                    Console.WriteLine(key);
                #endregion Snippet_3

            }
        }
    }

}
