using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class LocalizationAssetPatternTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(new AssetNamePattern(@"{Culture}{Section<\<.*\>>}{Key}").Match("en<mysection>mykey").Success);
            Assert.IsTrue(new AssetNamePattern(@"{Culture}{Section<\d+>}{Key}").Match("en01234mykey").Success);

            AssetNamePattern pattern1 = new AssetNamePattern("Resources.localization{-Culture}.json");
            AssetNamePattern pattern2 = new AssetNamePattern("Resources.localization[-Culture].json");
            AssetNamePattern pattern3 = new AssetNamePattern("Assets/{Type/}localization{-Culture}.ini");
            AssetNamePattern pattern4 = new AssetNamePattern("Assets/{Assembly/}{Type/}{Section.}localization{-Culture}.ini");
            AssetNamePattern pattern5 = new AssetNamePattern("Assets/{anysection_0/}{anysection_1/}localization{-Culture}.ini");

            IAssetKey key1 = LocalizationRoot.Global.Section("Section1").Section("Section2").Key("Key1").Culture("fi");
            IAssetKey key2 = LocalizationRoot.Global.Section("Section1").Section("Section2").Key("Key1");
            IAssetKey key3 = LocalizationRoot.Global.Assembly(GetType().Assembly).Type(GetType()).Key("Key1").Culture("fi");

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
