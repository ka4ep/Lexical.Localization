using Lexical.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.Tests
{
    [TestClass]
    public class AssetKeyNameParameterPolicyTests : TestUtils
    {
        [TestMethod]
        public void Test1()
        {
            // Arrange
            IAssetKey root = LocalizationRoot.Global;
            IAssetKey section = root.Section("ConsoleApp1").Section("MyController");
            IAssetKey key = section.Key("Success");
            IAssetKey key_en = key.SetCulture("en");
            CultureInfo en = CultureInfo.GetCultureInfo("en");

            // Act
            AssetKeyNameProvider policy_colon_colon_colon = AssetKeyNameProvider.Default;
            AssetKeyNameProvider policy_colon_colon_dot = AssetKeyNameProvider.Colon_Colon_Dot;
            AssetKeyNameProvider policy_colon_dot_dot = AssetKeyNameProvider.Colon_Dot_Dot;
            AssetKeyNameProvider policy_dot_dot_dot = AssetKeyNameProvider.Dot_Dot_Dot;

            // Assert
            Assert.AreEqual("en:ConsoleApp1:MyController:Success", policy_colon_colon_colon.BuildName(key_en));
            Assert.AreEqual("en:ConsoleApp1:MyController.Success", policy_colon_colon_dot.BuildName(key_en));
            Assert.AreEqual("en:ConsoleApp1.MyController.Success", policy_colon_dot_dot.BuildName(key_en));
            Assert.AreEqual("en.ConsoleApp1.MyController.Success", policy_dot_dot_dot.BuildName(key_en));
            Assert.AreEqual("en:ConsoleApp1:MyController:Success", policy_colon_colon_colon.BuildName(key_en));
            Assert.AreEqual("en:ConsoleApp1:MyController.Success", policy_colon_colon_dot.BuildName(key_en));
            Assert.AreEqual("en:ConsoleApp1.MyController.Success", policy_colon_dot_dot.BuildName(key_en));
            Assert.AreEqual("en.ConsoleApp1.MyController.Success", policy_dot_dot_dot.BuildName(key_en));


        }
    }
}
