using ConsoleApp1;
using Lexical.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class DynamicTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Dictionary<string, string> languageStrings = new Dictionary<string, string>();
            languageStrings["en:ConsoleApp1:MyController:Success"] = "Success";
            languageStrings["en:ConsoleApp1:MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings["fi:ConsoleApp1:MyController:Success"] = "Onnistui";
            languageStrings["fi:ConsoleApp1:MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Success"] = "Onnistuepie";
            languageStrings["fi-Savo:ConsoleApp1:MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            IAsset asset = new LocalizationStringAsset(languageStrings, AssetKeyNameProvider.Default);

            // The root must be LocalizationRoot. AssetRoot does not have any of the localization features, such as culture, inlining or formulating.
            dynamic root = new LocalizationRoot(asset, new CulturePolicy());
            dynamic section = root.Section("ConsoleApp1").Section("MyController");
            IAssetKey success = section.Culture("en").Success;

            // Try to call dynamic with generic method. 
            // This may not work in Mono, as the DynamicMetaObject implementation has runtime specific code. The problem is in the API.
            IAssetKey controller = root.Type<MyController>();

            Assert.AreEqual("Success", (string) section.Culture("en").Success);
            Assert.AreEqual(null, (byte[])section.Success.Icon);

            string inline_to_swedish_and_formulate = section.Error.Culture("sv").sv("Sönder (kod=0x{0:X8})").Format(0xCafeBabe);
            Assert.AreEqual("Sönder (kod=0xCAFEBABE)", inline_to_swedish_and_formulate);

            string inline_to_swedish_and_formulate_2 = section.Error.sv("Sönder (kod=0x{0:X8}, meta={1})").Culture("sv").Format(0xCafeBabe, "sur fisk");
            Assert.AreEqual("Sönder (kod=0xCAFEBABE, meta=sur fisk)", inline_to_swedish_and_formulate_2);

            // Fetch property "Args" using dynamic object
            Assert.IsTrue(section.Error.fi_Savo("Onnistuepi").Culture("fi-Savo").Format(0xFeedF00d).Args is object[]);
        }
    }
}
