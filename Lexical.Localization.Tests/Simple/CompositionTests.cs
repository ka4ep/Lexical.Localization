using Lexical.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class CompositionTests
    {
        [TestMethod]
        public void Test1()
        {
            // Arrange
            Dictionary<string, string> languageStrings1 = new Dictionary<string, string>();
            languageStrings1["ConsoleApp1:MyController:Success"] = "Success";
            languageStrings1["ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            Dictionary<string, string> languageStrings2 = new Dictionary<string, string>();
            languageStrings2["en:ConsoleApp1:MyController:Success"] = "Success";
            languageStrings2["en:ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            Dictionary<string, string> languageStrings3 = new Dictionary<string, string>();
            languageStrings3["fi.ConsoleApp1.MyController.Success"] = "Onnistui";
            languageStrings3["fi.ConsoleApp1.MyController.Error"] = "Virhe (Koodi=0x{0:X8})";
            Dictionary<string, string> languageStrings4 = new Dictionary<string, string>();
            languageStrings4["fi-Savo:ConsoleApp1:MyController:Success"] = "Onnistuepie";
            languageStrings4["fi-Savo:ConsoleApp1:MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAsset map1 = new LocalizationDictionary(languageStrings1, "{culture:}{section_0:}{section_1:}{section_n:}{key}");
            IAsset map2 = new LocalizationDictionary(languageStrings2, "{culture:}{section_0:}{section_1:}{section_n:}{key}");
            IAsset map3 = new LocalizationDictionary(languageStrings3, "{culture.}{section_0.}{section_1.}{section_n.}{key}");
            IAsset map4 = new LocalizationDictionary(languageStrings4, "{culture:}{section_0:}{section_1:}{section_n:}{key}");
            IAsset asset = new AssetComposition(map1, map2, map3, map4);
            IAssetKey root = LocalizationRoot.Global;
            IAssetKey section = root.Section("ConsoleApp1").Section("MyController");
            IAssetKey fi = section.SetCulture("fi"), en = section.SetCulture("en"), fi_savo = section.SetCulture("fi-Savo");
            IAssetKey success = section.Key("Success"), fi_success = fi.Key("Success"), en_success = en.Key("Success");

            // Assert
            Assert.IsTrue(asset.GetAllStrings(root).Count() == 8);
            Assert.IsTrue(asset.GetAllStrings(fi).Count() == 2);
            Assert.IsTrue(asset.GetAllStrings(en).Count() == 2);
            Assert.IsTrue(asset.GetAllStrings(fi_savo).Count() == 2);
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 4);
            Assert.AreEqual("Onnistui", asset.GetString(fi_success));
            Assert.AreEqual("Success", asset.GetString(en_success));
            Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));

            // Act
            languageStrings3["sv.ConsoleApp1.MyController.Success"] = "Det går bra";
            languageStrings3["sv.ConsoleApp1.MyController.Error"] = "Det funkar inte (Kod=0x{0:X8})";
            asset.Reload();
            // Assert
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 5);
        }
    }
}
