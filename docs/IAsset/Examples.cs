using Lexical.Localization;
using System.Collections.Generic;

namespace docs
{
    public class IAsset_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1
                // Language string source
                Dictionary<string, string> src = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
                // Create Asset
                IAsset asset = new LocalizationStringAsset(src, AssetKeyNameProvider.Default);
                #endregion Snippet_1

                #region Snippet_2
                // Create key
                IAssetKey key = new LocalizationRoot().Key("hello").Culture("en");
                // Resolve string - Call to LocalizationAssetExtensions.GetString()
                string str = asset.GetString(key);
                #endregion Snippet_2

            }
        }
    }

}
