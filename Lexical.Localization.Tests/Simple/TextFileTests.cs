using Lexical.Localization;
using Lexical.Localization.Internal;
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
            ILocalizationFileFormat fileformat = IniFileFormat.Instance;
            string filename = "Lexical.Localization.Tests.localization.ini";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").TypeSection("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            IAsset asset = fileformat.CreateAsset(s, null);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadJsonFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = JsonFileFormat.Instance;
            string filename = "Lexical.Localization.Tests.localization.json";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").TypeSection("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            IAsset asset = fileformat.CreateAsset(s, null);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadXmlFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = XmlFileFormat.Instance;
            string filename = "Lexical.Localization.Tests.localization.xml";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").TypeSection("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            IAsset asset = fileformat.CreateAsset(s, null);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadResXFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = ResXFileFormat.Instance;
            Dictionary<string, string> param = new Dictionary<string, string> { { "culture", "en" } };  

            // .resx file property must be set to "Copy to output directory" = "Copy Always"
            string filename = "Resources/localization.en.resx";
            IAssetKey key = LocalizationRoot.Global.SetCulture("en").TypeSection("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = new FileStream(filename, FileMode.Open);
            IAsset asset = fileformat.CreateAsset(s, new AssetNamePattern("{culture.}[anysection.][key]"), Key.CreateFromParameters(param));
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

    }
}
