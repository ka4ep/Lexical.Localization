using Lexical.Localization;
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
                IEqualityComparer<ILinePart> comparer = AssetKeyComparer.Default;
                #endregion Snippet_0
            }
            {
                #region Snippet_1
                ILinePart key = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                int hash = AssetKeyComparer.Default.GetHashCode(key);
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                ILinePart key1 = new LocalizationRoot().Type("MyClass").Key("OK");
                ILinePart key2 = Key.Create("Type", "MyClass").Append("Key", "OK");
                ILinePart key3 = LocalizationRoot.Global.Type("MyClass").Key("OK");
                ILinePart key4 = StringLocalizerRoot.Global.Type("MyClass").Key("OK");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
                bool equals23 = AssetKeyComparer.Default.Equals(key2, key3); // Are equal
                bool equals34 = AssetKeyComparer.Default.Equals(key3, key4); // Are equal
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
                int hash3 = AssetKeyComparer.Default.GetHashCode(key3);
                int hash4 = AssetKeyComparer.Default.GetHashCode(key4);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                ILinePart key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                ILinePart key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("en");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_3
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_4
                ILinePart key1 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK");
                ILinePart key2 = LocalizationRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_4
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_5
                ILinePart key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
                ILinePart key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are equal
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
                #endregion Snippet_5
            }
            {
                #region Snippet_5b
                ILinePart key1 = LocalizationRoot.Global.Type("MyClass").Key("OK");
                ILinePart key2 = LocalizationRoot.Global.Type("MyClass").Key("OK").Culture("");
                string str1 = key1.Culture("fi").ToString();  // <- Selects a culture
                string str2 = key2.Culture("fi").ToString();  // <- Doesn't change the effective culture
                #endregion Snippet_5b
            }
            {
                #region Snippet_6
                ILinePart key1 = LocalizationRoot.Global.Section("").Key("OK");
                ILinePart key2 = LocalizationRoot.Global.Key("OK");

                bool equals12 = AssetKeyComparer.Default.Equals(key1, key2); // Are not equal
                int hash1 = AssetKeyComparer.Default.GetHashCode(key1);
                int hash2 = AssetKeyComparer.Default.GetHashCode(key2);
                #endregion Snippet_6
            }
            {
                #region Snippet_6b
                #endregion Snippet_6b
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
            {
                #region Snippet_9
                #endregion Snippet_9
            }
        }
    }

}
