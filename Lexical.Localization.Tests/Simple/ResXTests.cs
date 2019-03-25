using ConsoleApp1;
using Lexical.Localization;
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
            IAssetKey key_resx = root.Assembly(typeof(ResXTests).Assembly).Resource("localization");
            IAssetKey key = key_resx.Type<MyController>().Key("Success");
            Assert.AreEqual("Onnistui", key.Culture("fi").ToString());
            Assert.AreEqual("Success", key.Culture("en").ToString());
        }

        [TestMethod]
        public void Test2()
        {
            // Method 1: Add resx assets from specific locations
            ILoggerFactory loggerFactory = new LoggerFactory();
            IAsset asset = ResourceManagerStringLocalizerAsset.Create("Lexical.Localization.Tests", "Resources", "localization", loggerFactory);
            ICulturePolicy culturePolicy = new CulturePolicy();
            IAssetRoot root = new StringLocalizerRoot(asset.CreateCache(_ => _.AddResourceCache().AddStringsCache().AddCulturesCache()), culturePolicy);

            Assert.AreEqual("Onnistui", root.Type<MyController>().Key("Success").Culture("fi").ToString());
            Assert.AreEqual("Success", root.Type<MyController>().Key("Success").Culture("en").ToString());
            Assert.AreEqual("Virhe (Koodi=0x{0:X8})", root.Type<MyController>().Key("Error").Culture("fi").ToString());
            Assert.AreEqual("Virhe (Koodi=0xCAFEBABE)", root.Type<MyController>().Key("Error").Culture("fi").Format(0xCAFEBABE).ToString());
        }
    }
}
