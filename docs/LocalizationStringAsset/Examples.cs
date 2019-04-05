using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lexical.Localization;
using Microsoft.Extensions.Localization;

namespace docs
{
    public class LocalizationStringAsset_Examples
    {
        public static void Main(string[] args)
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
                IAsset asset = new LocalizationAsset(source, AssetKeyNameProvider.Default);
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
                IAsset asset = new LocalizationAsset(source, "{Culture:}[Type:][Key]");
                #endregion Snippet_2a
                #region Snippet_2b
                // Extract all keys
                foreach (IAssetKey _key in asset.GetKeyLines(null).Select(line => line.Key))
                    Console.WriteLine(_key);
                #endregion Snippet_2b

                #region Snippet_3
                // Keys can be filtered
                IAssetKey filterKey = LocalizationRoot.Global.Culture("de");
                foreach (IAssetKey _key in asset.GetKeyLines(filterKey).Select(line => line.Key))
                    Console.WriteLine(_key);
                #endregion Snippet_3

            }
        }
    }

}
