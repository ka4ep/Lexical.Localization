using Lexical.Localization;
using Lexical.Localization.Asset;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace docs
{
    public class Tutorial_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1a
                ILine line = LineRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
                #endregion Snippet_1a
                #region Snippet_1b
                Console.WriteLine(line.Value("Corellia Melody"));
                #endregion Snippet_1b
                #region Snippet_1c
                Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Value(DateTime.Now));
                Console.WriteLine(String.Format("It is now {0:d} at {0:t}", DateTime.Now));
                #endregion Snippet_1c
                #region Snippet_1c1
                DateTime time = DateTime.Now;
                Console.WriteLine(LineRoot.Global.Key("Time").Format($"It is now {time:d} at {time:t}"));
                #endregion Snippet_1c1
                #region Snippet_1c2
                Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Culture("sv").Value(DateTime.Now));
                Console.WriteLine(LineRoot.Global.Format("It is now {0:d} at {0:t}").Culture("de").Value(DateTime.Now));
                #endregion Snippet_1c2
                #region Snippet_1d
                LineString resolved_string = line.Value("Corellia Melody").ResolveString();
                Console.WriteLine(resolved_string.Status);
                #endregion Snippet_1d
            }
            {
                #region Snippet_2
                ILine line = LineRoot.Global
                    .Key("hello")
                    .Format("Hello, {0}.")
                    .Inline("Culture:fi", "Hei, {0}")
                    .Inline("Culture:de", "Hallo, {0}");
                Console.WriteLine(line.Value("Corellia Melody"));
                #endregion Snippet_2
            }
            {
                #region Snippet_3
                ILine line = LineRoot.Global.PluralRules("Unicode.CLDR35")
                        .Type("MyClass").Key("Cats")
                        .Format("{cardinal:0} cat(s)")
                        .Inline("N:zero", "no cats")
                        .Inline("N:one", "a cat")
                        .Inline("N:other", "{0} cats");
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(line.Value(cats));
                #endregion Snippet_3
            }
            {
                #region Snippet_4
                ILine line = LineRoot.Global.PluralRules("Unicode.CLDR35")
                        .Type("MyClass").Key("Cats")
                        .Format("{0} cat(s)")
                        .Inline("Culture:en", "{cardinal:0} cat(s)")
                        .Inline("Culture:en:N:zero", "no cats")
                        .Inline("Culture:en:N:one", "a cat")
                        .Inline("Culture:en:N:other", "{0} cats")
                        .Inline("Culture:fi", "{cardinal:0} kissa(a)")
                        .Inline("Culture:fi:N:zero", "ei kissoja")
                        .Inline("Culture:fi:N:one", "yksi kissa")
                        .Inline("Culture:fi:N:other", "{0} kissaa");

                // Print with plurality and to culture "en"
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(line.Culture("en").Value(cats));
                #endregion Snippet_4
            }
            {
                #region Snippet_5a
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
                LineRoot.Builder.AddAsset(asset).Build();
                ILine line = LineRoot.Global.Key("Cats").Format("{0} cat(s)");
                // Print with plurality
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(line.Culture("fi").Value(cats));
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b
                IAsset asset = LineReaderMap.Default.FileAsset("PluralityExample0a.xml");
                ILineRoot root = new LineRoot(asset, new CulturePolicy());
                ILine line = root.Key("Cats").Format("{0} cat(s)");
                #endregion Snippet_5b
                // Print with plurality
                for (int cats = 0; cats <= 2; cats++)
                    Console.WriteLine(line.Culture("fi").Value(cats));
            }
            {
                #region Snippet_6
                List<ILine> lines = new List<ILine> {
                    LineAppender.Default.Type("MyClass").Key("Hello").Format("Hello, {0}"),
                    LineAppender.Default.Type("MyClass").Key("Hello").Culture("fi").Format("Hei, {0}"),
                    LineAppender.Default.Type("MyClass").Key("Hello").Culture("de").Format("Hallo, {0}")
                };
                IAsset asset = new StringAsset().Add(lines).Load();
                ILineRoot root = new LineRoot(asset, new CulturePolicy());
                ILine line = root.Type("MyClass").Key("Hello");
                Console.WriteLine(line.Value("Corellia Melody"));
                #endregion Snippet_6
            }
            {
                #region Snippet_7
                ILine root = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
                ILine line = root.Type("MyClass").Key("hello").Format("Hello, {0}.");
                Console.WriteLine(line.Value("Corellia Melody"));
                #endregion Snippet_7
            }
            {
                #region Snippet_8
                ILine line1a = LineRoot.Global.Key("Cats").Format("{0} cat(s)");
                IString string1 = CSharpFormat.Default.Parse("{0} cat(s)");
                ILine line1b = LineRoot.Global.Key("Cats").String(string1);

                ILine line2a = LineRoot.Global.Key("Ok").Text("{in braces}");
                IString string2 = TextFormat.Default.Parse("{in braces}");
                ILine line2b = LineRoot.Global.Key("Ok").String(string2);
                #endregion Snippet_8
            }
            {
                #region Snippet_9
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentThreadUICulture();
                ILine root = new LineRoot(null, culturePolicy: culturePolicy);
                #endregion Snippet_9
            }
            {
                #region Snippet_10
                ILine line = StringLocalizerRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
                IStringLocalizer localizer = line.AsStringLocalizer();
                IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
                #endregion Snippet_10
            }
            {
                #region Snippet_11
                ILineRoot root = new StringLocalizerRoot(null, new CulturePolicy());
                ILine line = root.Type("MyClass").Key("hello").Format("Hello, {0}.");
                IStringLocalizer localizer = line.AsStringLocalizer();
                IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
                #endregion Snippet_11
            }
            {
                #region Snippet_12
                ILine line = LineRoot.Global.Type("MyClass").Key("hello").Format("Hello, {0}.");
                IStringLocalizer localizer = line.AsStringLocalizer();
                IStringLocalizerFactory localizerFactory = line.AsStringLocalizerFactory();
                #endregion Snippet_12
            }
            {
                #region Snippet_13
                #endregion Snippet_13
            }
            {
                #region Snippet_14
                #endregion Snippet_14
            }
            {
                #region Snippet_15
                #endregion Snippet_15
            }

        }
    }




}
