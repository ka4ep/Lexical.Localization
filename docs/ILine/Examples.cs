using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
using Lexical.Localization.Asset;
#region Snippet_7b1
using Lexical.Localization.Inlines;
using Lexical.Localization.StringFormat;
#endregion Snippet_7b1

namespace docs
{
    public class ILine_Examples
    {
        public static void Main(string[] args)
        {
            {
                IAsset asset = new StringAsset(new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } }, LineFormat.Parameters);
                // Assign the composition to root
                ILineRoot root = new LineRoot(asset, new CulturePolicy());
                #region Snippet_0
                #endregion Snippet_0
            }

            {
                #region Snippet_1
                ILine key = new LineRoot().Section("Section").Section("Section").Key("Key");
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                ILineRoot root = new LineRoot();
                ILine section1 = root.Section("Section1");
                ILine section2 = section1.Section("Section2");
                ILine section1_1 = section1.Section("Section1.1");
                ILine key1_1_1 = section1_1.Key("Key1");
                ILine key1_1_2 = section1_1.Key("Key2");
                // ...
                #endregion Snippet_2
            }

            {
                IAsset asset = null;
                #region Snippet_3a
                // Create localization reference
                ILine key = new LineRoot().Section("Section").Section("Section").Key("Key");

                // Retrieve string with a reference
                IString value = asset.GetLine(key.Culture("en")).GetString();

                // Retrieve binary resource with a reference
                byte[] data = asset.GetResourceBytes(key.Culture("en")).Value;
                #endregion Snippet_3a
            }
            {
                #region Snippet_3b
                // Language string source
                Dictionary<string, string> src = new Dictionary<string, string> { { "en:Section:Key", "Hello World!" } };
                // Create Asset
                IAsset asset = new StringAsset(src, LineParameterPrinter.Colon_Colon_Colon);
                // Create culture policy
                ICulturePolicy culturePolicy = new CulturePolicy();
                // Create root
                ILineRoot root = new LineRoot(asset, culturePolicy);
                // Set Current Culture
                CultureInfo.CurrentCulture = new CultureInfo("en");
                // Create key specific provider
                ILine key = root.Section("Section").Key("Key");
                // Retieve string from provider
                string str = key.ToString();
                // Retrieve binary resoruce from provider
                byte[] data = key.GetResourceBytes().Value;
                #endregion Snippet_3b
            }

            {
                #region Snippet_4a
                // Add canonical parts
                ILine key = new LineRoot().Section("Section1").Section("Section2").Key("Key");

                // "Section1:Section2:Key"
                string id = LineParameterPrinter.Colon_Colon_Colon.Print(key);
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                // Add canonical parts, and non-canonical culture
                ILine key1 = new LineRoot().Section("Section").Key("Key").Culture("en");
                ILine key2 = new LineRoot().Culture("en").Section("Section").Key("Key");

                // "en:Section1:Section2:Key"
                string id1 = LineParameterPrinter.Colon_Colon_Colon.Print(key1);
                // "en:Section1:Section2:Key"
                string id2 = LineParameterPrinter.Colon_Colon_Colon.Print(key2);
                #endregion Snippet_4b
            }


            {
                #region Snippet_6a
                // Create language strings
                Dictionary<string, string> strs = new Dictionary<string, string>();
                strs["ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
                // Create asset
                IAsset asset = new StringAsset(strs, LineParameterPrinter.Default);
                // Create root
                ILineRoot root = new LineRoot(asset);
                #endregion Snippet_6a

                #region Snippet_6b
                // Create key "Error"
                ILine key = root.Type("ConsoleApp1.MyController").Key("Error");
                // Formulate key
                ILine key_formulated = key.Value(0xFeedF00d);
                #endregion Snippet_6b
                {
                    #region Snippet_6c
                    // Resolve to localized string "Error (Code=0x{0:X8})", but does not append arguments
                    IString str = key_formulated.ResolveFormatString();
                #endregion Snippet_6c
                }
                {
                    #region Snippet_6d
                    // Resolve to formulated string to "Error (Code=0xFEEDF00D)"
                    LineString str = key_formulated.ResolveString();
                    #endregion Snippet_6d
                }
            }


            {
                #region Snippet_7a
                // Create root
                ILineRoot root = new LineRoot();
                // Create key and add default value
                ILine key = root.Section("Section").Key("Success").Format("Success");
                // Resolve string from inlined key "Success"
                string str = key.ToString();
                #endregion Snippet_7a
            }

            {
                // Create root
                ILineRoot root = new LineRoot();
                #region Snippet_7b
                // Create key and add default strings
                ILine key = root.Section("Section").Key("Success")
                    .Format("Success")                                  // Add inlining to the root culture ""
                    .Inline("Culture:en", "Success")                   // Add inlining to culture "en"
                    .Inline("Culture:fi", "Onnistui")                  // Add inlining to culture "fi"
                    .Inline("Culture:sv", "Det funkar");               // Add inlining to culture "sv"
                // Resolve string from inlined key "Success"
                string str = key.Culture("en").ToString();
                #endregion Snippet_7b
            }

            {
                // Create root
                ILineRoot root = new LineRoot();
                #region Snippet_7c
                // Create key and add default strings
                ILine key = root.Section("Section").Key("Success")
                    .Format("Success")                                  // Add inlining to the root culture ""
                    .en("Success")                                     // Add inlining to culture "en"
                    .fi("Onnistui")                                    // Add inlining to culture "fi"
                    .sv("Det funkar");                                 // Add inlining to culture "sv"
                #endregion Snippet_7c
            }

            {
                #region Snippet_8a
                // Assign key to localization of type "MyController"
                ILine key = new LineRoot().Type(typeof(MyController));
                // Search "MyController:Success"
                string str = key.Key("Success").ToString();
                #endregion Snippet_8a
            }

            {
                #region Snippet_8b
                // Assign key to localization of type "MyController"
                ILine<MyController> key = new LineRoot().Type<MyController>();
                #endregion Snippet_8b
            }

            {
                #region Snippet_9a
                // Create root that matches only to english strings
                ILine root_en = new LineRoot().Culture("en");
                // Create key
                ILine key = root_en.Section("Section").Key("Key");
                #endregion Snippet_9a
            }

            {
                #region Snippet_10
                // Dynamic assignment
                dynamic root = new LineRoot();
                // Provides to string on typecast
                string str = root.Section("Section").Key("Hello");
                #endregion Snippet_10
            }
        }

        #region Snippet_11a
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
        #endregion Snippet_11a

        #region Snippet_11b
        class MyControllerB
        {
            static ILine localization = LineRoot.Global.Type<MyControllerB>();

            public void Do()
            {
                string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
            }
        }
        #endregion Snippet_11b

        #region Snippet_7d
        class MyController__
        {
            static ILine localization = LineRoot.Global.Type<MyControllerB>();
            static ILine Success = localization.Key("Success").Format("Success").sv("Det funkar").fi("Onnistui");

            public string Do()
            {
                return Success.ToString();
            }
        }
        #endregion Snippet_7d

    }

}
