﻿using Lexical.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class LocalizationStringAssetTests
    {
        [TestMethod]
        public void Test1()
        {
            // Arrange
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["ConsoleApp1:MyController:Success"] = "Success";
            languageStrings["ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["en:ConsoleApp1:MyController:Success"] = "Success";
            languageStrings["en:ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1:MyController:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1:MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Success"] = "Onnistuepie";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAssetKeyNamePolicy pattern = new AssetNamePattern("{Culture:}{anysection_0:}{anysection_1:}{anysection_2:}{anysection_3:}{anysection_4:}{anysection_n:}{Key}{:Key_n}");
            IAsset asset = new LocalizationStringAsset(languageStrings, pattern);
            IAssetKey root = LocalizationRoot.Global;
            IAssetKey section = root.Section("ConsoleApp1").Section("MyController");
            IAssetKey fi = section.Culture("fi"), en = section.Culture("en"), fi_savo = section.Culture("fi-Savo");
            IAssetKey success = section.Key("Success"), fi_success = fi.Key("Success"), en_success = en.Key("Success");

            // Assert
            Assert.IsTrue(asset.GetAllStringLines(root).Count() == 8);
            Assert.IsTrue(asset.GetAllStringLines(fi).Count() == 2);
            Assert.IsTrue(asset.GetAllStringLines(en).Count() == 2);
            Assert.IsTrue(asset.GetAllStringLines(fi_savo).Count() == 2);
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 4);
            Assert.AreEqual("Onnistui", asset.GetString(fi_success));
            Assert.AreEqual("Success", asset.GetString(en_success));
            Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));

            languageStrings["sv:ConsoleApp1:MyController.Success"] = "Det går bra";
            languageStrings["sv:ConsoleApp1:MyController.Error"] = "Det funkar inte (Kod=0x{0:X8})";
            asset.Reload();
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 5);
        }

        [TestMethod]
        public void Test2()
        {
            // Arrange
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["ConsoleApp1:MyController:Success"] = "Success";
            languageStrings["ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["en:ConsoleApp1:MyController:Success"] = "Success";
            languageStrings["en:ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1:MyController:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1:MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Success"] = "Onnistuepie";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAsset asset = new LocalizationStringAsset(languageStrings, AssetKeyNameProvider.Default);
            IAssetKey root = LocalizationRoot.Global;
            IAssetKey section = root.Section("ConsoleApp1").Section("MyController");
            IAssetKey fi = section.Culture("fi"), en = section.Culture("en"), fi_savo = section.Culture("fi-Savo");
            IAssetKey success = section.Key("Success"), fi_success = fi.Key("Success"), en_success = en.Key("Success");

            // Assert
            Assert.IsTrue(asset.GetAllStringLines(root).Count() == 8);
            //Assert.IsTrue(asset.GetAllStringLines(fi).Count() == 2);
            //Assert.IsTrue(asset.GetAllStringLines(en).Count() == 2);
            //Assert.IsTrue(asset.GetAllStringLines(fi_savo).Count() == 2);
            //Assert.IsTrue(asset.GetSupportedCultures().Count() == 4);
            Assert.AreEqual("Onnistui", asset.GetString(fi_success));
            Assert.AreEqual("Success", asset.GetString(en_success));
            Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));

            languageStrings["sv:ConsoleApp1:MyController.Success"] = "Det går bra";
            languageStrings["sv:ConsoleApp1:MyController.Error"] = "Det funkar inte (Kod=0x{0:X8})";
            asset.Reload();
            //Assert.IsTrue(asset.GetSupportedCultures().Count() == 5);
        }
    }
}