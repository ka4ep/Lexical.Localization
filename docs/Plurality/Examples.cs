using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class Plurality_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0
                IAssetKey key = LocalizationRoot.Global.Key("Cats")
                        .Inline("{0} cats")
                        .Inline("N:Zero", "no cats")
                        .Inline("N:One", "a cat")
                        .Inline("N:Plural", "{0} cats");

                Console.WriteLine(key.Format(0));
                Console.WriteLine(key.Format(1));
                Console.WriteLine(key.Format(2));
                #endregion Snippet_0
            }

            {
                #region Snippet_1
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("Plurality/localization.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("Cats");

                Console.WriteLine(key.Format(0));
                Console.WriteLine(key.Format(1));
                Console.WriteLine(key.Format(2));
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                #endregion Snippet_2
            }

            {
                #region Snippet_3
                #endregion Snippet_3
            }
            {
                #region Snippet_4
                #endregion Snippet_4
            }
            {
                #region Snippet_5
                #endregion Snippet_5
            }
            {
                #region Snippet_6
                #endregion Snippet_6
            }
            {
                #region Snippet_7
                #endregion Snippet_7
            }
        }
    }

}
