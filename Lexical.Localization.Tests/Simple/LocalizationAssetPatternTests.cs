using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Lexical.Localization;
using Lexical.Asset;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class LocalizationAssetPatternTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(new AssetNamePattern(@"{culture}{section<\<.*\>>}{key}").Match("en<mysection>mykey").Success);
            Assert.IsTrue(new AssetNamePattern(@"{culture}{section<\d+>}{key}").Match("en01234mykey").Success);

            AssetNamePattern pattern1 = new AssetNamePattern("Resources.localization{-culture}.json");
            AssetNamePattern pattern2 = new AssetNamePattern("Resources.localization[-culture].json");
            AssetNamePattern pattern3 = new AssetNamePattern("Assets/{type/}localization{-culture}.ini");
            AssetNamePattern pattern4 = new AssetNamePattern("Assets/{assembly/}{type/}{section.}localization{-culture}.ini");
            AssetNamePattern pattern5 = new AssetNamePattern("Assets/{anysection_0/}{anysection_1/}localization{-culture}.ini");

            IAssetKey key1 = LocalizationRoot.Global.Section("Section1").Section("Section2").Key("Key1").SetCulture("fi");
            IAssetKey key2 = LocalizationRoot.Global.Section("Section1").Section("Section2").Key("Key1");
            IAssetKey key3 = LocalizationRoot.Global.AssemblySection(GetType().Assembly).TypeSection(GetType()).Key("Key1").SetCulture("fi");

            Assert.AreEqual("Resources.localization-fi.json", pattern1.MatchToString(key1));
            Assert.AreEqual("Resources.localization.json", pattern1.MatchToString(key2));
            Assert.AreEqual("Resources.localization-fi.json", pattern1.MatchToString(key3));

            Assert.AreEqual("Resources.localization-fi.json", pattern2.MatchToString(key1));
            Assert.AreEqual(null, pattern2.MatchToString(key2));
            Assert.AreEqual("Resources.localization-fi.json", pattern2.MatchToString(key3));

            Assert.AreEqual("Assets/localization-fi.ini", pattern3.MatchToString(key1));
            Assert.AreEqual("Assets/localization.ini", pattern3.MatchToString(key2));
            Assert.AreEqual("Assets/Lexical.Localization.Tests.LocalizationAssetPatternTests/localization-fi.ini", pattern3.MatchToString(key3));

            Assert.AreEqual("Assets/Section1.localization-fi.ini", pattern4.MatchToString(key1));
            Assert.AreEqual("Assets/Section1.localization.ini", pattern4.MatchToString(key2));
            Assert.AreEqual("Assets/Lexical.Localization.Tests/Lexical.Localization.Tests.LocalizationAssetPatternTests/localization-fi.ini", pattern4.MatchToString(key3));

            Assert.AreEqual("Assets/Section1/Section2/localization-fi.ini", pattern5.MatchToString(key1));
            Assert.AreEqual("Assets/Section1/Section2/localization.ini", pattern5.MatchToString(key2));
            Assert.AreEqual("Assets/Lexical.Localization.Tests/Lexical.Localization.Tests.LocalizationAssetPatternTests/localization-fi.ini", pattern5.MatchToString(key3));
        }
    }
}
