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
                #region Snippet_0a
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample0a.xml");
                IAssetRoot root = new LocalizationRoot(asset);
                IAssetKey key = root.Key("Cats").Inline("{0} cat(s)");

                // Print with the default string (without culture policy)
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Format(cats));

                // Print Culture "en"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("en").Format(cats));

                // Print Culture "fi"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("fi").Format(cats));
                #endregion Snippet_0a
            }

            {
                #region Snippet_0b
                IAssetRoot root = new LocalizationRoot();
                IAssetKey key = root.Key("Cats")
                        .Inline("{0} cat(s)")  // Default string
                        .Inline("N:Zero", "no cats")
                        .Inline("N:One", "a cat")
                        .Inline("N:Plural", "{0} cats");
                #endregion Snippet_0b
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Format(cats));
            }

            {
                #region Snippet_0c
                IAssetRoot root = new LocalizationRoot();
                IAssetKey key = root.Key("Cats")
                        .Inline("{0} cat(s)")   // Default string
                        .Inline("Culture:en:N:Zero", "no cats")
                        .Inline("Culture:en:N:One", "a cat")
                        .Inline("Culture:en:N:Plural", "{0} cats")
                        .Inline("Culture:fi:N:Zero", "ei kissoja")
                        .Inline("Culture:fi:N:One", "yksi kissa")
                        .Inline("Culture:fi:N:Plural", "{0} kissaa");
                #endregion Snippet_0c
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Format(cats));
            }

            {
                #region Snippet_1a
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample0b.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("Cats");

                for (int cats = 0; cats<=2; cats++)
                    Console.WriteLine(key.Format(cats));
                #endregion Snippet_1a
            }


            {
                // Plurality permutations for argument 0
                #region Snippet_2
                IAssetKey key = LocalizationRoot.Global.Key("CatsDogs")
                        .Inline("{0} cat(s) and {1} dog(s)")
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
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample1.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_3
            }
            {
                // Plurality permutations for argument 1
                #region Snippet_4
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample2.xml");
                IAssetKey key = new LocalizationRoot(asset).Key("CatsDogs");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Format(cats, dogs));
                #endregion Snippet_4
            }
            {
                // Plurality permutations for argument 0 and argument 1
                #region Snippet_5
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample2-en.xml");
                IAssetRoot root = new LocalizationRoot(asset);
                IAssetKey key = root.Key("CatsDogs").Inline("{0} cat(s) and {1} dog(s)");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Culture("en").Format(cats, dogs));
                #endregion Snippet_5
            }
            {
                // Plurality for 4 arguments
                #region Snippet_6
                IAsset asset = LocalizationXmlReader.Instance.FileAsset("PluralityExample4.xml");
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
