using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class AssetLoaderTests
    {
        [TestMethod]
        public void Test_LoadEmbedded_Resources()
        {
            // Arrange
            IAssetLoader loader = new AssetLoader()
                .Add( new AssetLoaderPartEmbeddedResources(@"[assembly<.*>.][resource.]res.{culture.}{type<.*>.}[section<[^\.]*>.][key<[^\.]*>]")
                        .AddAssembly(GetType().Assembly)
                        .AddMatchParameters("assembly", "resource", "type", "section")
                    );
            IAssetKey root = new LocalizationRoot(loader);
            IAssetKey key1 = root.SetCulture("fi").Section("Icon").Key("Success");
            IAssetKey key2 = root.SetCulture("en").Section("Icon").Key("Success");

            // Act
            byte[] data1 = key1.GetResource();
            byte[] data2 = key2.GetResource();

            // Assert
            Assert.IsTrue(data1 != null && data1.Length == 10 && data1[0] == 6);
            Assert.IsTrue(data2 != null && data2.Length == 10 && data2[0] == 4);
        }

        [TestMethod]
        public void Test_LoadFile_Resources()
        {
            // Arrange
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            IAssetLoader loader = new AssetLoader()
                .Add(new AssetLoaderPartFileProviderResources(fileProvider, "[location/]Localization/[key]{-culture}.ini").AddMatchParameters("location"));
            IAssetKey root = new LocalizationRoot(loader);
            IAssetKey key1 = root.SetCulture("fi").Key("localization");
            IAssetKey key2 = root.SetCulture("en").Key("localization");

            // Act
            byte[] data1 = key1.GetResource();
            byte[] data2 = key2.GetResource();

            // Assert
            Assert.IsTrue(data1 != null && data1.Length > 50);
            Assert.IsTrue(data2 != null && data2.Length > 50);
        }

    }
}
