using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class AssetKeyComparer_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                IEqualityComparer<IAssetKey> comparer = AssetKeyComparer.Default;
                #endregion Snippet_0
            }
            {
                #region Snippet_1
                IAssetKey key = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                int hash = AssetKeyComparer.Default.GetHashCode(key);
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                IAssetKey key1 = new LocalizationRoot().Type("MyClass").Key("OK");
                IAssetKey key2 = Key.Create("Type", "MyClass").Append("Key", "OK");
                IAssetKey key3 = LocalizationRoot.Global.Type("MyClass").Key("OK");
                IAssetKey key4 = StringLocalizerRoot.Global.Type("MyClass").Key("OK");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
                bool equals23 = AssetKeyComparer.Default.Equals(key2, key3);
                bool equals34 = AssetKeyComparer.Default.Equals(key3, key4);
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
                int hash3 = AssetKeyComparer.Default.GetHashCode(key3);
                int hash4 = AssetKeyComparer.Default.GetHashCode(key4);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                IAssetKey key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("en");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
                #endregion Snippet_3
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_4
                IAssetKey key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                IAssetKey key2 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2);
                #endregion Snippet_4
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_5
                #endregion Snippet_5
            }
            {
                #region Snippet_6
                #endregion Snippet_6
            }
        }
    }

}
