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
                        .Inline("{0} cat(s)")
                        .Inline("N:Zero", "no cats")
                        .Inline("N:One", "a cat")
                        .Inline("N:Plural", "{0} cats");

                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Format(cats));
                #endregion Snippet_0
            }

            {
                #region Snippet_1
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample0.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("Cats");

                for (int cats = 0; cats<=2; cats++)
                    Console.WriteLine(key.Format(cats));
                #endregion Snippet_1
            }

            {
                // Plurality permutations for argument 0
                #region Snippet_2
                IAssetKey key = LocalizationRoot.Global.Key("CatsDogs")
                        .Inline("{0} cats and {1} dog(s)")
                        .Inline("N:Zero", "no cats and {1} dog(s)")
                        .Inline("N:One", "a cat and {1} dog(s)")
                        .Inline("N:Plural", "{0} cats and {1} dog(s)");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_2
            }

            {
                // Plurality permutations for argument 0
                #region Snippet_3
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample1.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs1");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_3
            }
            {
                // Plurality permutations for argument 1
                #region Snippet_4
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample2.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs2");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_4
            }
            {
                // Plurality permutations for argument 0 and argument 1
                #region Snippet_5
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample3.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs3");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_5
            }
            {
                // Plurality for 4 arguments
                #region Snippet_6
                IAsset asset = XmlFileFormat.Instance.CreateFileAsset("PluralityExample4.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogsPoniesHorses");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        for (int ponies = 0; ponies <= 2; ponies++)
                            for (int horses = 0; horses <= 2; horses++)
                                Console.WriteLine(key.Format(cats, dogs, ponies, horses));
                #endregion Snippet_6
            }
            {
                #region Snippet_7
                #endregion Snippet_7
            }
        }
    }

}
