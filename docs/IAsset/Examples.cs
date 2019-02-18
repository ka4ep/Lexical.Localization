using System.Collections.Generic;
using System.Reflection;
using Lexical.Localization;
using Lexical.Localization;

namespace docs
{
    public class IAsset_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                // Language string source
                Dictionary<string, string> src = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
                // Create Asset
                IAsset asset = new LocalizationDictionary(src);
                #endregion Snippet_1

                #region Snippet_2
                // Create key
                IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
                // Resolve string - Call to LocalizationAssetExtensions.GetString()
                string str = asset.GetString(key);
                #endregion Snippet_2

            }
        }
    }

}
