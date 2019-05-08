// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using Lexical.Localization;

namespace docs
{
    public class DynamicUseExamples
    {
        public static void Main(string[] args)
        {
            // For testing
            {
                #region Snippet_0
                Dictionary<string, string> languageStrings = new Dictionary<string, string>();
                languageStrings["en:ConsoleApp1:MyController:Success"] = "Success";
                languageStrings["en:ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
                languageStrings["fi:ConsoleApp1:MyController:Success"] = "Onnistui";
                languageStrings["fi:ConsoleApp1:MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
                languageStrings["fi-Savo:ConsoleApp1:MyController:Success"] = "Onnistuepie";
                languageStrings["fi-Savo:ConsoleApp1:MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";

                dynamic myLocalization = new LineRoot(new LocalizationAsset(languageStrings, KeyPrinter.Default), new CulturePolicy());
                //dynamic x = myLocalization.Culture("fi");
                dynamic mySection1 = myLocalization.Section("ConsoleApp1");
                ILine k = mySection1.Section("MyController");
                dynamic mySection = myLocalization.Section("ConsoleApp1").Section("MyController");
                dynamic inlined = mySection.Success.fi_Savo("Onnistuepi").Culture("fi-Savo");
                object inlines = inlined.Inlines;
                byte[] icon = mySection.Success.Icon;
                
                Console.WriteLine(mySection.Success);
                Console.WriteLine(mySection.Error);
                Console.WriteLine(mySection.Error.Format(0xCafeBabe));
                Console.WriteLine(mySection.Error.Culture("sv").sv("Sönder (kod=0x{0:X8})").Format(0xCafeBabe));
                #endregion Snippet_0
            }

            {
                #region Snippet_1
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
