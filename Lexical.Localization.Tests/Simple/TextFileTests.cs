using Lexical.Asset;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class TextFileTests
    {
        [TestMethod]
        public void ReadIniFile()
        {
            // Arrange
            AssetFileConstructor fileformat = AssetFileConstructors.Ini;
            string filename = "Lexical.Localization.Tests.localization.ini";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").Section("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            ILocalizationStringProvider asset = fileformat(s, null) as ILocalizationStringProvider;
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadJsonFile()
        {
            // Arrange
            AssetFileConstructor fileformat = AssetFileConstructors.Json;
            string filename = "Lexical.Localization.Tests.localization.json";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").Section("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            ILocalizationStringProvider asset = fileformat(s, null) as ILocalizationStringProvider;
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadXmlFile()
        {
            // Arrange
            AssetFileConstructor fileformat = AssetFileConstructors.Xml;
            string filename = "Lexical.Localization.Tests.localization.xml";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").Section("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            ILocalizationStringProvider asset = fileformat(s, null) as ILocalizationStringProvider;
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadResXFile()
        {
            // Arrange
            AssetFileConstructor fileformat = AssetFileConstructors.ResX;
            Dictionary<string, string> param = new Dictionary<string, string> { { "culture", "en" } };  

            // .resx file property must be set to "Copy to output directory" = "Copy Always"
            string filename = "Resources/localization.en.resx";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").Section("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = new FileStream(filename, FileMode.Open);
            ILocalizationStringProvider asset = fileformat(s, param) as ILocalizationStringProvider;
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

    }
}
