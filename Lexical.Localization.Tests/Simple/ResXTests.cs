using ConsoleApp1;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class ResXTests
    {
        [TestMethod]
        public void Test1()
        {
            // Method 1: Add root resource manager ResourceManagerStringLocalizerFactory
            //           This way every key needs to have assemblysection + typesection, or just typesection
            ILoggerFactory loggerFactory = new LoggerFactory();
            IAsset asset = ResourceManagerStringLocalizerAsset.CreateFactory("Resources", loggerFactory);
            asset = asset.CreateCache(_ => _.AddResourceCache().AddStringsCache().AddCulturesCache());
            ICulturePolicy culturePolicy = new CulturePolicy();
            IAssetRoot root = new StringLocalizerRoot(asset, culturePolicy);
            IAssetKey key_resx = root.AssemblySection(typeof(ResXTests).Assembly).ResourceSection("localization");
            IAssetKey key = key_resx.TypeSection<MyController>().Key("Success");
            Assert.AreEqual("Onnistui", key.SetCulture("fi").ToString());
            Assert.AreEqual("Success", key.SetCulture("en").ToString());
        }

        [TestMethod]
        public void Test2()
        {
            // Method 1: Add resx assets from specific locations
            ILoggerFactory loggerFactory = new LoggerFactory();
            IAsset asset = ResourceManagerStringLocalizerAsset.Create("Lexical.Localization.Tests", "Resources", "localization", loggerFactory);
            ICulturePolicy culturePolicy = new CulturePolicy();
            IAssetRoot root = new StringLocalizerRoot(asset.CreateCache(_ => _.AddResourceCache().AddStringsCache().AddCulturesCache()), culturePolicy);

            Assert.AreEqual("Onnistui", root.TypeSection<MyController>().Key("Success").SetCulture("fi").ToString());
            Assert.AreEqual("Success", root.TypeSection<MyController>().Key("Success").SetCulture("en").ToString());
            Assert.AreEqual("Virhe (Koodi=0x{0:X8})", root.TypeSection<MyController>().Key("Error").SetCulture("fi").ToString());
            Assert.AreEqual("Virhe (Koodi=0xCAFEBABE)", root.TypeSection<MyController>().Key("Error").SetCulture("fi").Format(0xCAFEBABE).ToString());
        }
    }
}
