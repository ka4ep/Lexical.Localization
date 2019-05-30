using Lexical.Localization;
using System.Collections.Generic;

namespace docs
{
    public class LineComparer_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                IEqualityComparer<ILine> comparer = LineComparer.Default;
                #endregion Snippet_0
            }
            {
                #region Snippet_1
                ILine key = LineRoot.Global.Culture("en").Type("MyClass").Key("OK");
                int hash = LineComparer.Default.GetHashCode(key);
                #endregion Snippet_1
            }
            {
                #region Snippet_2
                ILine key1 = new LineRoot().Type("MyClass").Key("OK");
                ILine key2 = LineAppender.NonResolving.Type("MyClass").Key("OK");
                ILine key3 = LineRoot.Global.Type("MyClass").Key("OK");
                ILine key4 = StringLocalizerRoot.Global.Type("MyClass").Key("OK");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                bool equals23 = LineComparer.Default.Equals(key2, key3); // Are equal
                bool equals34 = LineComparer.Default.Equals(key3, key4); // Are equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
                int hash3 = LineComparer.Default.GetHashCode(key3);
                int hash4 = LineComparer.Default.GetHashCode(key4);
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                ILine key1 = LineRoot.Global.Culture("en").Type("MyClass").Key("OK");
                ILine key2 = LineRoot.Global.Type("MyClass").Key("OK").Culture("en");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_3
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_4
                ILine key1 = LineRoot.Global.Culture("en").Type("MyClass").Key("OK");
                ILine key2 = LineRoot.Global.Culture("en").Type("MyClass").Key("OK").Culture("de");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                #endregion Snippet_4
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
            }
            {
                #region Snippet_5
                ILine key1 = LineRoot.Global.Type("MyClass").Key("OK");
                ILine key2 = LineRoot.Global.Type("MyClass").Key("OK").Culture("");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
                #endregion Snippet_5
            }
            {
                #region Snippet_5b
                ILine key1 = LineRoot.Global.Type("MyClass").Key("OK");
                ILine key2 = LineRoot.Global.Type("MyClass").Key("OK").Culture("");
                string str1 = key1.Culture("fi").ToString();  // <- Selects a culture
                string str2 = key2.Culture("fi").ToString();  // <- Doesn't change the effective culture
                #endregion Snippet_5b
            }
            {
                #region Snippet_6
                ILine key1 = LineRoot.Global.Section("").Key("OK");
                ILine key2 = LineRoot.Global.Key("OK");

                bool equals12 = LineComparer.Default.Equals(key1, key2); // Are not equal
                int hash1 = LineComparer.Default.GetHashCode(key1);
                int hash2 = LineComparer.Default.GetHashCode(key2);
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
