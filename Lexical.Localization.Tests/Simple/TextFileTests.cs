using Lexical.Localization;
using Lexical.Localization.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Lexical.Localization.Utils;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class TextFileTests
    {
        [TestMethod]
        public void ReadIniFile()
        {
            // Arrange
            string filename = "Lexical.Localization.Tests.localization.ini";
            IAssetKey key = LocalizationRoot.Global.Culture("en").Type("ConsoleApp1.MyController").Key("Success");

            // Act
            IAsset asset = LocalizationIniReader.Instance.EmbeddedAsset(GetType().Assembly, filename);

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadJsonFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = LocalizationJsonReader.Instance;
            string filename = "Lexical.Localization.Tests.localization.json";
            IAssetKey key = LocalizationRoot.Global.Culture("en").Type("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            IAsset asset = fileformat.StreamAsset(s, null);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadXmlFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = LocalizationXmlReader.Instance;
            string filename = "Lexical.Localization.Tests.localization.xml";
            IAssetKey key = LocalizationRoot.Global.Culture("en").Type("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = GetType().Assembly.GetManifestResourceStream(filename);
            IAsset asset = fileformat.StreamAsset(s, null);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

        [TestMethod]
        public void ReadResXFile()
        {
            // Arrange
            ILocalizationFileFormat fileformat = LocalizationResxReader.Instance;
            Dictionary<string, string> param = new Dictionary<string, string> { { "Culture", "en" } };  

            // .resx file property must be set to "Copy to output directory" = "Copy Always"
            string filename = "Resources/localization.en.resx";
            IAssetKey key = LocalizationRoot.Global/*.Culture("en")*/.Type("ConsoleApp1.MyController").Key("Success");

            // Act
            Stream s = new FileStream(filename, FileMode.Open);
            IAsset asset = fileformat.StreamAsset(s, new AssetNamePattern("{Culture.}[anysection.][Key]")/*, Key.CreateFromParameters(param)*/);
            s.Dispose();

            // Assert
            Assert.AreEqual("Success", asset.GetString(key));
        }

    }
}
