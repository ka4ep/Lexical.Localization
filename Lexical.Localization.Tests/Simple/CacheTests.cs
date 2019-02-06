using Lexical.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void Test1()
        {
            // Arrange
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["ConsoleApp1.MyController:Success"] = "Success";
            languageStrings["ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["en:ConsoleApp1.MyController:Success"] = "Success";
            languageStrings["en:ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1.MyController:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1.MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-Savo:ConsoleApp1.MyController:Success"] = "Onnistuepie";
            languageStrings["fi-Savo:ConsoleApp1.MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAsset map = new LocalizationStringDictionary(languageStrings, "{culture:}[type:]{section_0:}{section_1:}{section_2:}{section_n:}{key}");
            IAssetCache asset = new AssetCache(map).AddResourceCache().AddStringsCache().AddCulturesCache();
            LocalizationRoot.Global.SetAsset(asset);
            IAssetKey root = LocalizationRoot.Global;
            IAssetKey section = root.TypeSection("ConsoleApp1.MyController");
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
            Assert.AreEqual("Onnistui", fi_success.ToString());
            Assert.AreEqual("Success", en_success.ToString());
            Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));

            // Again this time from cache
            Assert.IsTrue(asset.GetAllStrings(root).Count() == 8);
            Assert.IsTrue(asset.GetAllStrings(fi).Count() == 2);
            Assert.IsTrue(asset.GetAllStrings(en).Count() == 2);
            Assert.IsTrue(asset.GetAllStrings(fi_savo).Count() == 2);
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 4);
            Assert.AreEqual("Onnistui", asset.GetString(fi_success));
            Assert.AreEqual("Success", asset.GetString(en_success));
            Assert.AreEqual("Onnistui", fi_success.ToString());
            Assert.AreEqual("Success", en_success.ToString());
            Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));

            languageStrings["sv:ConsoleApp1.MyController:Success"] = "Det går bra";
            languageStrings["sv:ConsoleApp1.MyController:Error"] = "Det funkar inte (Kod=0x{0:X8})";
            asset.Reload();
            Assert.IsTrue(asset.GetSupportedCultures().Count() == 5);
        }

        /// <summary>
        /// Tests cache when active culture changes.
        /// </summary>
        [TestMethod]
        public void Test2()
        {
            // Arrange
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["ConsoleApp1.MyController:Icon:Success"] = "Success";
            languageStrings["ConsoleApp1.MyController:Icon:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["en:ConsoleApp1.MyController:Icon:Success"] = "Success";
            languageStrings["en:ConsoleApp1.MyController:Icon:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1.MyController:Icon:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1.MyController:Icon:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-Savo:ConsoleApp1.MyController:Icon:Success"] = "Onnistuepie";
            languageStrings["fi-Savo:ConsoleApp1.MyController:Icon:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAsset map = new LocalizationStringDictionary(languageStrings, AssetKeyNameProvider.Default);
            IAsset asset = new AssetCache(map).AddResourceCache().AddStringsCache().AddCulturesCache();
            ICulturePolicy culturePolicy = new CulturePolicy().SetToCurrentCulture();
            IAssetKey root = new LocalizationRoot(asset, culturePolicy);
            IAssetKey section = root.Section("ConsoleApp1.MyController").Section("Icon");
            IAssetKey success = section.Key("Success"), error = section.Key("Error");

            // Assert
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            Assert.AreEqual("Onnistui", success.ToString());
            Assert.AreEqual("Onnistui", success.ToString());
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            Assert.AreEqual("Success", success.ToString());
            Assert.AreEqual("Success", success.ToString());
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi-SE");
            Assert.AreEqual("Onnistui", success.ToString());
            Assert.AreEqual("Onnistui", success.ToString());
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr");
            Assert.AreEqual("Success", success.ToString());
            Assert.AreEqual("Success", success.ToString());
        }


    }
}
