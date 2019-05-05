using System.Collections.Generic;
using System.Globalization;
using Lexical.Localization;
#region Snippet_7b1
using Lexical.Localization.Inlines;
#endregion Snippet_7b1

namespace docs
{
    public class IAssetKey_Examples
    {
        public static void Main(string[] args)
        {
            {
                IAsset asset = new LocalizationAsset(new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } }, ParameterPolicy.Instance);
                // Assign the composition to root
                IAssetRoot root = new LocalizationRoot(asset, new CulturePolicy());
                #region Snippet_0
                #endregion Snippet_0
            }

            {
                #region Snippet_1
                ILinePart key = new LocalizationRoot().Section("Section").Section("Section").Key("Key");
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                IAssetRoot root = new LocalizationRoot();
                ILinePart section1 = root.Section("Section1");
                ILinePart section2 = section1.Section("Section2");
                ILinePart section1_1 = section1.Section("Section1.1");
                ILinePart key1_1_1 = section1_1.Key("Key1");
                ILinePart key1_1_2 = section1_1.Key("Key2");
                // ...
                #endregion Snippet_2
            }

            {
                IAsset asset = null;
                #region Snippet_3a
                // Create localization reference
                ILinePart key = new LocalizationRoot().Section("Section").Section("Section").Key("Key");

                // Retrieve string with a reference
                IFormulationString str = asset.GetString(key.Culture("en"));

                // Retrieve binary resource with a reference
                byte[] data = asset.GetResource(key.Culture("en"));
                #endregion Snippet_3a
            }
            {
                #region Snippet_3b
                // Language string source
                Dictionary<string, string> src = new Dictionary<string, string> { { "en:Section:Key", "Hello World!" } };
                // Create Asset
                IAsset asset = new LocalizationAsset(src, KeyPrinter.Colon_Colon_Colon);
                // Create culture policy
                ICulturePolicy culturePolicy = new CulturePolicy();
                // Create root
                IAssetRoot root = new LocalizationRoot(asset, culturePolicy);
                // Set Current Culture
                CultureInfo.CurrentCulture = new CultureInfo("en");
                // Create key specific provider
                ILinePart key = root.Section("Section").Key("Key");
                // Retieve string from provider
                string str = key.ToString();
                // Retrieve binary resoruce from provider
                byte[] data = key.GetResource();
                #endregion Snippet_3b
            }

            {
                #region Snippet_4a
                // Add canonical parts
                ILinePart key = new LocalizationRoot().Section("Section1").Section("Section2").Key("Key");

                // "Section1:Section2:Key"
                string id = KeyPrinter.Colon_Colon_Colon.Print(key);
                #endregion Snippet_4a
            }
            {
                #region Snippet_4b
                // Add canonical parts, and non-canonical culture
                ILinePart key1 = new LocalizationRoot().Section("Section").Key("Key").Culture("en");
                ILinePart key2 = new LocalizationRoot().Culture("en").Section("Section").Key("Key");

                // "en:Section1:Section2:Key"
                string id1 = KeyPrinter.Colon_Colon_Colon.Print(key1);
                // "en:Section1:Section2:Key"
                string id2 = KeyPrinter.Colon_Colon_Colon.Print(key2);
                #endregion Snippet_4b
            }


            {
                #region Snippet_6a
                // Create language strings
                Dictionary<string, string> strs = new Dictionary<string, string>();
                strs["ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
                // Create asset
                IAsset asset = new LocalizationAsset(strs, KeyPrinter.Default);
                // Create root
                IAssetRoot root = new LocalizationRoot(asset);
                #endregion Snippet_6a

                #region Snippet_6b
                // Create key "Error"
                ILinePart key = root.Type("ConsoleApp1.MyController").Key("Error");
                // Formulate key
                ILinePart key_formulated = key.Format(0xFeedF00d);
                #endregion Snippet_6b
                { 
                #region Snippet_6c
                    // Resolve to localized string "Error (Code=0x{0:X8})", but does not append arguments
                    string str = key_formulated.ResolveString();
                #endregion Snippet_6c
                }
                {
                    #region Snippet_6d
                    // Resolve to formulated string to "Error (Code=0xFEEDF00D)"
                    string str = key_formulated.ResolveFormulatedString();
                    #endregion Snippet_6d
                }
            }


            {
                #region Snippet_7a
                // Create root
                IAssetRoot root = new LocalizationRoot();
                // Create key and add default value
                ILinePart key = root.Section("Section").Key("Success").Inline("Success");
                // Resolve string from inlined key "Success"
                string str = key.ToString();
                #endregion Snippet_7a
            }

            {
                // Create root
                IAssetRoot root = new LocalizationRoot();
                #region Snippet_7b
                // Create key and add default strings
                ILinePart key = root.Section("Section").Key("Success")
                    .Inline("Success")                                 // Add inlining to the root culture ""
                    .Inline("Culture:en", "Success")                   // Add inlining to culture "en"
                    .Inline("Culture:fi", "Onnistui")                  // Add inlining to culture "fi"
                    .Inline("Culture:sv", "Det funkar");               // Add inlining to culture "sv"
                // Resolve string from inlined key "Success"
                string str = key.Culture("en").ToString();
                #endregion Snippet_7b
            }

            {
                // Create root
                IAssetRoot root = new LocalizationRoot();
                #region Snippet_7c
                // Create key and add default strings
                ILinePart key = root.Section("Section").Key("Success")
                    .Inline("Success")                                 // Add inlining to the root culture ""
                    .en("Success")                                     // Add inlining to culture "en"
                    .fi("Onnistui")                                    // Add inlining to culture "fi"
                    .sv("Det funkar");                                 // Add inlining to culture "sv"
                #endregion Snippet_7c
            }

            {
                #region Snippet_8a
                // Assign key to localization of type "MyController"
                ILinePart key = new LocalizationRoot().Type(typeof(MyController));
                // Search "MyController:Success"
                string str = key.Key("Success").ToString();
                #endregion Snippet_8a
            }

            {
                #region Snippet_8b
                // Assign key to localization of type "MyController"
                ILineKey<MyController> key = new LocalizationRoot().Type<MyController>();
                #endregion Snippet_8b
            }

            {
                #region Snippet_9a
                // Create root that matches only to english strings
                ILinePart root_en = new LocalizationRoot().Culture("en");
                // Create key
                ILinePart key = root_en.Section("Section").Key("Key");
                #endregion Snippet_9a
            }

            {
                #region Snippet_10
                // Dynamic assignment
                dynamic root = new LocalizationRoot();
                // Provides to string on typecast
                string str = root.Section("Section").Key("Hello");
                #endregion Snippet_10
            }
        }

        #region Snippet_11a
        class MyController
        {
            ILinePart localization;

            public MyController(ILineKey<MyController> localization)
            {
                this.localization = localization;
            }

            public MyController(IAssetRoot localizationRoot)
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
            static ILinePart localization = LocalizationRoot.Global.Type<MyControllerB>();

            public void Do()
            {
                string str = localization.Key("Success").en("Success").fi("Onnistui").ToString();
            }
        }
        #endregion Snippet_11b

        #region Snippet_7d
        class MyController__
        {
            static ILinePart localization = LocalizationRoot.Global.Type<MyControllerB>();
            static ILinePart Success = localization.Key("Success").Inline("Success").sv("Det funkar").fi("Onnistui");

            public string Do()
            {
                return Success.ToString();
            }
        }
        #endregion Snippet_7d

    }

}
