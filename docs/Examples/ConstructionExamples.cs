// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization;
using Lexical.Localization.Inlines;
using Lexical.Localization.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace docs
{
    public class ConstructionExamples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet_0
            // Create language strings
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["en:ConsoleApp1.MyController:Success"] = "Success";
            languageStrings["en:ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1.MyController:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1.MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-SAVO:ConsoleApp1.MyController:Success"] = "Onnistuepie";
            languageStrings["fi-SAVO:ConsoleApp1.MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            #endregion Snippet_0

            // 
            {
                #region Snippet_Plain_1
                AssetBuilder builder = new AssetBuilder();
                builder.AddDictionary(languageStrings, AssetKeyNameProvider.Default);

                CulturePolicy culturePolicy = new CulturePolicy();
                culturePolicy.SetCultures("en", "fi", "");
                
                IAssetRoot myLocalization = new LocalizationRoot(builder.Build(), culturePolicy);
                #endregion Snippet_Plain_1

                // Try it out
                //Console.WriteLine(myLocalization["ConsoleApp1.MyController"].Key("Success").BuildName());
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Success").BuildName());
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Success").SetCulture("fi").BuildName());
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Error").BuildName());
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Success"));
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Error"));
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Error").Format(0xBAADF00D));
                Console.WriteLine(myLocalization.Section("ConsoleApp1.MyController").Key("Error").sv("Sönder (kod=0x{0:X8})").SetCulture("sv").Format(0xBAADF00D));
                Console.WriteLine(myLocalization.TypeSection(typeof(ConstructionExamples)).Key("Success"));
                Console.WriteLine(myLocalization.TypeSection<ConstructionExamples>().Key("Success"));

                var key1 = myLocalization.TypeSection<ConstructionExamples>().SetCulture("en").Key("Success");
                var key2 = myLocalization.Section(typeof(ConstructionExamples).CanonicalName()).Key("Success").SetCulture("en");
                Console.WriteLine($"{key2.Equals(key1)} {key1.GetHashCode()}=={key2.GetHashCode()}");
                Console.WriteLine($"{key1.Equals(key2)} {key1.GetHashCode()}=={key2.GetHashCode()}");
            }

            {
                #region Snippet_Plain_2
                #endregion Snippet_Plain_2
            }

            {
                #region Snippet_Singleton
                // How to setup singleton instance
                (LocalizationRoot.Global.CulturePolicy as ICulturePolicyAssignable).SetToCurrentCulture();
                LocalizationRoot.Builder.AddDictionary(languageStrings, AssetKeyNameProvider.Default);
                LocalizationRoot.Builder.Build();

                // Try it out
                var myControllerLocalization = LocalizationRoot.Global.Section("ConsoleApp1.MyController");
                Console.WriteLine(myControllerLocalization.Key("Error"));
                Console.WriteLine(myControllerLocalization.Key("Error").Format(0xBAADF00D));
                Console.WriteLine(myControllerLocalization.Key("Error").Format(0xBAADF00D).SetCulture("fi"));
                Console.WriteLine(myControllerLocalization.Key("Error").sv("Sönder (kod=0x{0:X8})").SetCulture("sv").Format(0xBAADF00D));
                #endregion Snippet_Singleton
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

