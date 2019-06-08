using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Lexical.Localization;
using Lexical.Localization.Asset;
#region Snippet_7b1
using Lexical.Localization.Inlines;
using Lexical.Localization.Plurality;
using Lexical.Localization.Resolver;
using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;
#endregion Snippet_7b1

namespace docs
{
    public class ILine_Examples
    {
        public static void Main(string[] args)
        {
            // 1. Line
            {
                #region Snippet_1a
                ILine key = LineRoot.Global.PluralRules("Unicode.CLDR35").Key("Key").Format("Hello, {0}");
                #endregion Snippet_1a
            }

            {
                #region Snippet_1b
                ILineRoot root = new LineRoot();
                ILine hint1 = root.PluralRules("Unicode.CLDR35");
                ILine section1 = hint1.Section("Section2");
                ILine section1_1 = hint1.Section("Section1.1");
                ILine key1_1_1 = section1_1.Key("Key1");
                ILine key1_1_2 = section1_1.Key("Key2");
                ILine value1_1_1 = key1_1_1.Format("Hello, {0}");
                // ...
                #endregion Snippet_1b
            }

            // 2. Parts
            {
                #region Snippet_2a
                IAsset resourceAsset = new ResourceDictionary( new Dictionary<ILine, byte[]>() );
                ILine line = LineRoot.Global.Asset(resourceAsset);
                #endregion Snippet_2a
            }
            {
                #region Snippet_2b
                ILine line = LineRoot.Global.Logger(Console.Out, LineStatusSeverity.Ok);
                #endregion Snippet_2b
            }
            {
                #region Snippet_2c
                ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
                ILine line = LineRoot.Global.CulturePolicy(culturePolicy);
                #endregion Snippet_2c
            }
            {
                #region Snippet_2d
                #endregion Snippet_2d
            }
            {
                #region Snippet_2e
                #endregion Snippet_2e
            }
            {
                #region Snippet_2f
                #endregion Snippet_2f
            }
            {
                #region Snippet_2g
                #endregion Snippet_2g
            }
            {
                #region Snippet_2h
                #endregion Snippet_2h
            }

            // 3. Hints
            {
                #region Snippet_3_
                IAsset asset = LineReaderMap.Default.FileAsset("ILine\\Hints.ini");
                ILineRoot root = new LineRoot(asset);
                Console.WriteLine(root.Key("Error").Value(DateTime.Now));
                Console.WriteLine(root.Key("Ok"));
                #endregion Snippet_3_
            }
            {
                #region Snippet_3a
                ILine line1 = LineRoot.Global.StringFormat(TextFormat.Default).String("Text");
                ILine line2 = LineRoot.Global.StringFormat("Lexical.Localization.StringFormat.TextFormat,Lexical.Localization")
                    .String("Text");
                #endregion Snippet_3a
                Console.WriteLine(line2);
            }
            {
                #region Snippet_3b
                IPluralRules pluralRules = PluralRulesResolver.Default.Resolve("Unicode.CLDR35");
                ILine line1 = LineRoot.Global.PluralRules(pluralRules);
                ILine line2 = LineRoot.Global.PluralRules("Unicode.CLDR35");
                ILine line3 = LineRoot.Global.PluralRules("[Category=cardinal,Case=one]n=1[Category=cardinal,Case=other]true");
                #endregion Snippet_3b
                Console.WriteLine(line2);
            }
            {
                #region Snippet_3c
                IFormatProvider customFormat = new CustomFormat();
                ILine line1 = LineRoot.Global.FormatProvider(customFormat).Format("{0:DATE}").Value(DateTime.Now);
                ILine line2 = LineRoot.Global.FormatProvider("docs.CustomFormat,docs").Format("{0:DATE}").Value(DateTime.Now);
                #endregion Snippet_3c
                Console.WriteLine(line1);
                Console.WriteLine(line2);
            }
            {
                #region Snippet_3e
                ResolveSource[] resolveSequence = 
                    new ResolveSource[] { ResolveSource.Inline, ResolveSource.Asset, ResolveSource.Line };

                IStringResolver stringResolver = new StringResolver(Resolvers.Default, resolveSequence);
                ILine line = LineRoot.Global.StringResolver(stringResolver);
                #endregion Snippet_3e
                Console.WriteLine(line);
            }
            {
                #region Snippet_3f
                ILine line = LineRoot.Global.StringResolver("Lexical.Localization.StringFormat.StringResolver");
                #endregion Snippet_3f
                Console.WriteLine(line);
            }
            {
                #region Snippet_3g
                ResolveSource[] resolveSequence = 
                    new ResolveSource[] { ResolveSource.Inline, ResolveSource.Asset, ResolveSource.Line };

                IResourceResolver resourceResolver = new ResourceResolver(Resolvers.Default, resolveSequence);
                ILine line = LineRoot.Global.ResourceResolver(resourceResolver);
                #endregion Snippet_3g
                Console.WriteLine(line);
            }
            {
                #region Snippet_3h
                ILine line = LineRoot.Global.ResourceResolver("Lexical.Localization.Resource.ResourceResolver");
                #endregion Snippet_3h
                Console.WriteLine(line);
            }

            // 4. Keys
            {
                #region Snippet_4a
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                #endregion Snippet_4b
            }
            {
                #region Snippet_4c
                #endregion Snippet_4c
            }
            {
                #region Snippet_4d
                #endregion Snippet_4d
            }
            {
                #region Snippet_4e
                #endregion Snippet_4e
            }
            {
                #region Snippet_4f
                #endregion Snippet_4f
            }
            {
                #region Snippet_4g
                #endregion Snippet_4g
            }
            {
                #region Snippet_4h
                #endregion Snippet_4h
            }

            // 5. Non-canonicals
            {
                #region Snippet_5a
                Assembly asm = typeof(ILine_Examples).Assembly;
                ILine line1 = LineRoot.Global.Assembly(asm);
                ILine line2 = LineRoot.Global.Assembly("docs");
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b
                CultureInfo culture = CultureInfo.GetCultureInfo("en");
                ILine line1 = LineRoot.Global.Culture(culture);
                ILine line2 = LineRoot.Global.Culture("en");
                #endregion Snippet_5b
            }
            {
                #region Snippet_5c
                ILine line1 = LineRoot.Global.Type<ILine_Examples>();
                ILine line2 = LineRoot.Global.Type(typeof(ILine_Examples));
                #endregion Snippet_5c
            }
            {
                #region Snippet_5d
                #endregion Snippet_5d
            }
            {
                #region Snippet_5e
                #endregion Snippet_5e
            }
            {
                #region Snippet_5f
                #endregion Snippet_5f
            }
            {
                #region Snippet_5g
                #endregion Snippet_5g
            }
            {
                #region Snippet_5h
                #endregion Snippet_5h
            }

            // 6. Canonicals
            {
                #region Snippet_6a
                ILine line = LineRoot.Global.Key("Ok");
                #endregion Snippet_6a
            }
            {
                #region Snippet_6b
                ILine line = LineRoot.Global.Section("Resources").Key("Ok");
                #endregion Snippet_6b
            }
            {
                #region Snippet_6c
                ILine line = LineRoot.Global.Location(@"c:\dir");
                #endregion Snippet_6c
            }
            {
                #region Snippet_6d
                ILine line = LineRoot.Global.BaseName("docs.Resources");
                #endregion Snippet_6d
            }
            {
                #region Snippet_6e
                #endregion Snippet_6e
            }
            {
                #region Snippet_6f
                #endregion Snippet_6f
            }
            {
                #region Snippet_6g
                #endregion Snippet_6g
            }
            {
                #region Snippet_6h
                #endregion Snippet_6h
            }

            // 7. Strings
            {
                #region Snippet_7a
                IString str = CSharpFormat.Default.Parse("ErrorCode = 0x{0:X8}");
                ILine line = LineRoot.Global.Key("Error").String(str);
                #endregion Snippet_7a
            }
            {
                #region Snippet_7b
                ILine line = LineRoot.Global.Key("Error").StringFormat(CSharpFormat.Default).String("ErrorCode = 0x{0:X8}");
                #endregion Snippet_7b
            }
            {
                #region Snippet_7c
                ILine line = LineRoot.Global.Key("Error").Format("ErrorCode = 0x{0:X8}");
                #endregion Snippet_7c
            }
            {
                #region Snippet_7c2
                int code = 0x100;
                ILine line = LineRoot.Global.Key("Error").Format($"ErrorCode = 0x{code:X8}");
                #endregion Snippet_7c2
                Console.WriteLine(line);
            }
            {
                #region Snippet_7d
                ILine line = LineRoot.Global.Key("Hello").Text("Hello World");
                #endregion Snippet_7d
            }
            {
                #region Snippet_7e
                ILine line = LineRoot.Global.Section("Section").Key("Success")
                    .Format("Success")                                  // Add inlining to the root culture ""
                    .Inline("Culture:en", "Success")                   // Add inlining to culture "en"
                    .Inline("Culture:fi", "Onnistui")                  // Add inlining to culture "fi"
                    .Inline("Culture:sv", "Det funkar");               // Add inlining to culture "sv"
                #endregion Snippet_7e
            }
            {
                #region Snippet_7f
                ILine line = LineRoot.Global.Section("Section").Key("Success")
                    .Format("Success")                                  // Add inlining to the root culture ""
                    .en("Success")                                      // Add inlining to culture "en"
                    .fi("Onnistui")                                     // Add inlining to culture "fi"
                    .sv("Det funkar");                                  // Add inlining to culture "sv"
                #endregion Snippet_7f
            }

            // Enumerations
            {
                #region Snippet_7l
                ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>();
                #endregion Snippet_7l
            }
            {
                #region Snippet_7l2
                ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>().InlineEnum<CarFeature>();
                #endregion Snippet_7l2
            }
            {
                #region Snippet_7m
                ILine carFeature = LineRoot.Global.Assembly("docs").Type<CarFeature>()
                    .InlineEnum<CarFeature>()
                    .InlineEnum(CarFeature.Electric, "fi", "Sähkö")
                    .InlineEnum(CarFeature.Petrol, "fi", "Bensiini")
                    .InlineEnum(CarFeature.NaturalGas, "fi", "Maakaasu")
                    .InlineEnum(CarFeature.TwoDoors, "fi", "Kaksiovinen")
                    .InlineEnum(CarFeature.FourDoors, "fi", "Neliovinen")
                    .InlineEnum(CarFeature.FiveDoors, "fi", "Viisiovinen")
                    .InlineEnum(CarFeature.Red, "fi", "Punainen")
                    .InlineEnum(CarFeature.Black, "fi", "Musta")
                    .InlineEnum(CarFeature.White, "fi", "Valkoinen");
                #endregion Snippet_7m
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("");
                #region Snippet_7m2
                Console.WriteLine( carFeature.Enum(CarFeature.Petrol) );
                Console.WriteLine( carFeature.Enum(CarFeature.Petrol).Culture("fi") );
                #endregion Snippet_7m2
                #region Snippet_7m3
                CarFeature features = CarFeature.Petrol | CarFeature.FiveDoors | CarFeature.Black;
                Console.WriteLine( carFeature.ResolveEnumFlags(features, " | ") );
                Console.WriteLine( carFeature.Culture("fi").ResolveEnumFlags(features, " | ") );
                #endregion Snippet_7m3
            }

            // 8. Values
            {
                #region Snippet_8a
                ILine line = LineRoot.Global.Key("Error").Format("ErrorCode={0}").Value(0x100);
                LineString result = line.ResolveString();
                #endregion Snippet_8a
            }
            {
                #region Snippet_8b
                #endregion Snippet_8b
            }
            {
                #region Snippet_8c
                #endregion Snippet_8c
            }
            {
                #region Snippet_8d
                #endregion Snippet_8d
            }
            {
                #region Snippet_8e
                #endregion Snippet_8e
            }
            {
                #region Snippet_8f
                #endregion Snippet_8f
            }
            {
                #region Snippet_8g
                #endregion Snippet_8g
            }
            {
                #region Snippet_8h
                #endregion Snippet_8h
            }

            // 10. Resources
            {
                #region Snippet_10a
                ILine line = LineRoot.Global.Key("Error").Resource(new byte[] { 1, 2, 3 });
                LineResourceBytes result = line.ResolveBytes();
                #endregion Snippet_10a
            }
            {
                #region Snippet_10b
                ILine line = LineRoot.Global.Key("Error").Resource(new byte[] { 1, 2, 3 });
                using (LineResourceStream result = line.ResolveStream())
                {

                }
                #endregion Snippet_10b
            }
            {
                #region Snippet_10c
                #endregion Snippet_10c
            }
            {
                #region Snippet_10d
                #endregion Snippet_10d
            }
            {
                #region Snippet_10e
                #endregion Snippet_10e
            }
            {
                #region Snippet_10f
                #endregion Snippet_10f
            }
            {
                #region Snippet_10g
                #endregion Snippet_10g
            }
            {
                #region Snippet_10h
                #endregion Snippet_10h
            }
        }

        // 9. Use in classes
        #region Snippet_9a
        class MyController
        {
            ILine localization;

            public MyController(ILine<MyController> localization)
            {
                this.localization = localization;
            }

            public MyController(ILineRoot localizationRoot)
            {
                this.localization = localizationRoot.Type<MyController>();
            }

            public void Do()
            {
                string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
            }
        }
        #endregion Snippet_9a

        #region Snippet_9b
        class MyControllerB
        {
            static ILine localization = LineRoot.Global.Type<MyControllerB>();

            public void Do()
            {
                string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
            }
        }
        #endregion Snippet_9b

        #region Snippet_7h
        class MyController__
        {
            static ILine localization = LineRoot.Global.Type<MyControllerB>();
            static ILine Success = localization.Key("Success").Format("Success").sv("Det funkar").fi("Onnistui");

            public string Do()
            {
                return Success.ToString();
            }
        }
        #endregion Snippet_7h
    }

    #region Snippet_3d
    public class CustomFormat : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == "DATE" && arg is DateTime time)
            {
                return time.Date.ToString();
            }
            return null;
        }

        public object GetFormat(Type formatType)
            => formatType == typeof(ICustomFormatter) ? this : default;
    }
    #endregion Snippet_3d

    #region Snippet_7i
    [Flags]
    enum CarFeature
    {
        // Fuel Type
        Electric = 0x0001,
        Petrol = 0x0002,
        NaturalGas = 0x0004,

        // Door count
        TwoDoors = 0x0010,
        FourDoors = 0x0020,
        FiveDoors = 0x0040,

        // Color
        Red = 0x0100,
        Black = 0x0200,
        White = 0x0400,
    }
    #endregion Snippet_7i


}
