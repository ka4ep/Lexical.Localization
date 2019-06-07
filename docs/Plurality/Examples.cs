using Lexical.Localization;
using Lexical.Localization.Asset;
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
                #region Snippet_A1
                ILine root = LineRoot.Global.PluralRules(CLDR35.Instance);
                #endregion Snippet_A1
            }
            {
                #region Snippet_A2
                ILine root = LineRoot.Global.PluralRules("Lexical.Localization.CLDR35");
                #endregion Snippet_A2
            }
            {
                #region Snippet_A3
                ILine root = LineRoot.Global.PluralRules("[Category=cardinal,Case=zero,Optional=1]n=0[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true");
                #endregion Snippet_A3
            }

            {
                #region Snippet_0a
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
                ILineRoot root = new LineRoot(asset);
                ILine key = root.Key("Cats").Format("{0} cat(s)");

                // Print with the default string (without culture policy)
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Value(cats));

                // Print Culture "en"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("en").Value(cats));

                // Print Culture "fi"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("fi").Value(cats));
                #endregion Snippet_0a
            }
            {
                #region Snippet_0a2
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.json");
                ILineRoot root = new LineRoot(asset);
                ILine key = root.Key("Cats").Format("{0} cat(s)");

                // Print with the default string (without culture policy)
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Value(cats));

                // Print Culture "en"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("en").Value(cats));

                // Print Culture "fi"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("fi").Value(cats));
                #endregion Snippet_0a2
            }
            {
                #region Snippet_0a3
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.ini");
                ILineRoot root = new LineRoot(asset);
                ILine key = root.Key("Cats").Format("{0} cat(s)");

                // Print with the default string (without culture policy)
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Value(cats));

                // Print Culture "en"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("en").Value(cats));

                // Print Culture "fi"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("fi").Value(cats));
                #endregion Snippet_0a3
            }

            {
                #region Snippet_0b
                ILineRoot root = new LineRoot();
                ILine key = root.Key("Cats")
                        .PluralRules(CLDR35.Instance)
                        .Format("{cardinal:0} cat(s)")  // Default string
                        .Inline("N:zero", "no cats")
                        .Inline("N:one", "a cat")
                        .Inline("N:other", "{0} cats");
                #endregion Snippet_0b
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Value(cats));
            }

            {
                #region Snippet_0c
                ILineRoot root = new LineRoot();
                ILine key = root.Key("Cats")
                        .PluralRules(CLDR35.Instance)
                        .Format("{0} cat(s)")   // Default string
                        .Inline("Culture:en", "{cardinal:0} cat(s)")
                        .Inline("Culture:en:N:zero", "no cats")
                        .Inline("Culture:en:N:one", "a cat")
                        .Inline("Culture:en:N:other", "{0} cats")
                        .Inline("Culture:fi", "{cardinal:0} kissa(a)")
                        .Inline("Culture:fi:N:zero", "ei kissoja")
                        .Inline("Culture:fi:N:one", "yksi kissa")
                        .Inline("Culture:fi:N:other", "{0} kissaa");

                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(key.Culture("en").Value(cats));
                #endregion Snippet_0c
            }

            {
                // CLDR35 has rules for "" culture
                #region Snippet_1a
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0b.xml");
                ILine key = new LineRoot(asset).Key("Cats");

                for (int cats = 0; cats<=2; cats++)
                    Console.WriteLine(key.Value(cats));
                #endregion Snippet_1a
            }


            {
                // Plurality permutations for argument 0
                #region Snippet_2
                ILine key = LineRoot.Global.Key("CatsDogs")
                        .PluralRules(CLDR35.Instance)
                        .Format("{cardinal:0} cat(s) and {1} dog(s)")
                        .Inline("N:zero", "no cats and {1} dog(s)")
                        .Inline("N:one", "a cat and {1} dog(s)")
                        .Inline("N:other", "{0} cats and {1} dog(s)");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Value(cats, dogs));
                #endregion Snippet_2
            }

            {
                // Plurality permutations for argument 0
                #region Snippet_3
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample1.xml");
                ILine key = new LineRoot(asset).Key("CatsDogs");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Value(cats, dogs));
                #endregion Snippet_3
            }

            {
                // Plurality permutations for argument 0 and argument 1
                #region Snippet_5
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample2.xml");
                ILineRoot root = new LineRoot(asset);
                ILine key = root.Key("CatsDogs").Format("{0} cat(s) and {1} dog(s)");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Culture("en").Value(cats, dogs));
                #endregion Snippet_5
            }
            {
                // Plurality for 4 arguments
                #region Snippet_6
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample4.xml");
                ILine key = new LineRoot(asset).Key("CatsDogsPoniesHorses");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        for (int ponies = 0; ponies <= 2; ponies++)
                            for (int horses = 0; horses <= 2; horses++)
                                Console.WriteLine(key.Value(cats, dogs, ponies, horses));
                #endregion Snippet_6
            }
            {
                // Plural expression in localization file
                // [Category=cardinal,Case=zero,Optional=1]n=0[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true
                #region Snippet_7
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample5.xml");
                ILine key = new LineRoot(asset).Key("CatsDogs");

                for (int cats = 0; cats <= 2; cats++)
                    for (int dogs = 0; dogs <= 2; dogs++)
                        Console.WriteLine(key.Value(cats, dogs));
                #endregion Snippet_7
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
        }
    }

}
