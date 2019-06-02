using Lexical.Localization;
using Lexical.Localization.Asset;
using System.Collections.Generic;
using System.Reflection;

namespace docs
{
    public class IAssetComposition_Example
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create individual assets
            IAsset asset_1 = new StringAsset(new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } }, LineFormat.Parameters);
            IAsset asset_2 = new ResourceStringDictionary(new Dictionary<string, byte[]> { { "Culture:en:Key:Hello.Icon", new byte[] { 1, 2, 3 } } }, LineFormat.Parameters);

            // Create composition asset
            IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2);

            // Assign the composition to root
            ILineRoot root = new LineRoot(asset_composition, new CulturePolicy());
            #endregion Snippet
        }
    }

}
