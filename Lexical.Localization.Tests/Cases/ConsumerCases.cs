using Lexical.Localization.Inlines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApp1;
using System.Globalization;
using Lexical.Localization;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    [Case("Consume", "Strings")]
    public class ConsumeStrings : AssetData
    {
        public void Run(Run run)
        {
            IAssetRoot root = run.Get<IAssetRoot>();
            IAssetKey controller = root.Type<MyController>();

            // Test strings
            if (run.Get<bool>("strings"))
            {
                IAssetKey section = controller;
                IAssetKey fi = section.Culture("fi"), en = section.Culture("en"), fi_savo = section.Culture("fi-Savo");
                IAssetKey success = section.Key("Success"), fi_success = fi.Key("Success"), en_success = en.Key("Success");
                IAsset asset = run.Get<IAsset>();
                dynamic d_section = root.Type<MyController>();

                var string_enumeration_is_supported = asset.GetAllStrings() != null;
                var culture_enumeration_is_supported = asset.GetSupportedCultures() != null;

                // Assert
                if (string_enumeration_is_supported)
                {
                    Assert.IsTrue(asset.GetAllStrings().Where(kp=>!kp.Value.Contains(";")).Count() == 8);
                    Assert.IsTrue(asset.GetAllStrings(section).Where(kp => !kp.Value.Contains(";")).Count() == 8);
                    Assert.IsTrue(asset.GetAllStrings(fi).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                    Assert.IsTrue(asset.GetAllStrings(en).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                    Assert.IsTrue(asset.GetAllStrings(fi_savo).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                }
                if (culture_enumeration_is_supported)
                {
                    IEnumerable<CultureInfo> cultures = asset.GetSupportedCultures()?.Distinct().Where(ci => ci.Name != "");
                    //  todo, change all implementations so that root culture "" is returned too ^^  
                    int count = cultures.Count();
                    Assert.IsTrue(count == 3 || count == 4);
                }
                Assert.AreEqual("Success", asset.GetString(en_success));
                Assert.AreEqual("Onnistui", asset.GetString(fi_success));
                Assert.AreEqual("Onnistui", fi_success.ToString());
                Assert.AreEqual("Success", en_success.ToString());
                Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));
                Assert.AreEqual("Onnistui", fi_success.ToString());
                Assert.AreEqual("Success", en_success.ToString());
                Assert.AreEqual("Virhe (Koodi=0xFEEDF00D)", fi.Key("Error").Format(0xFeedF00d).ToString());
                Assert.AreEqual("Erfolg", section.Culture("de").de("Erfolg").ToString());
                Assert.AreEqual("Onnistui", (string)d_section.Success.Culture("fi"));
                Assert.AreEqual(null, d_section.Inlines);
                Assert.AreEqual(null, d_section.Args);
                Assert.IsTrue((int)((object[])d_section.Error.Format(0xBADF00D).Args)[0] == 0xBADF00D);
                IAssetKey _key = d_section.Error.Culture("sv-SE").sv_SE("Sönder (kod=0x{0:X8})").Format(0xCafeBabe);
                Assert.AreEqual("Sönder (kod=0xCAFEBABE)", _key.ToString());

                // Again this time from cache
                if (string_enumeration_is_supported)
                {
                    Assert.IsTrue(asset.GetAllStrings().Where(kp => !kp.Value.Contains(";")).Count() == 8);
                    Assert.IsTrue(asset.GetAllStrings(section).Where(kp => !kp.Value.Contains(";")).Count() == 8);
                    Assert.IsTrue(asset.GetAllStrings(fi).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                    Assert.IsTrue(asset.GetAllStrings(en).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                    Assert.IsTrue(asset.GetAllStrings(fi_savo).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                }
                if (culture_enumeration_is_supported)
                {
                    IEnumerable<CultureInfo> cultures = asset.GetSupportedCultures()?.Distinct().Where(ci=>ci.Name!="");
                    int count = cultures.Count();
                    Assert.IsTrue(count == 4);
                }
                Assert.AreEqual("Onnistui", asset.GetString(fi_success));
                Assert.AreEqual("Success", asset.GetString(en_success));
                Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));
                Assert.AreEqual("Onnistui", fi_success.ToString());
                Assert.AreEqual("Success", en_success.ToString());
                Assert.AreEqual("Virhe (Koodi=0xFEEDF00D)", fi.Key("Error").Format(0xFeedF00d).ToString());
                Assert.AreEqual("Erfolg", section.Culture("de").de("Erfolg").ToString());

                // Reload and try again.
                if (asset is IAssetReloadable reloadable)
                {
                    reloadable.Reload();
                    // Again this time from cache
                    if (string_enumeration_is_supported)
                    {
                        Assert.IsTrue(asset.GetAllStrings().Where(kp => !kp.Value.Contains(";")).Count() == 8);
                        Assert.IsTrue(asset.GetAllStrings(section).Where(kp => !kp.Value.Contains(";")).Count() == 8);
                        Assert.IsTrue(asset.GetAllStrings(fi).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                        Assert.IsTrue(asset.GetAllStrings(en).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                        Assert.IsTrue(asset.GetAllStrings(fi_savo).Where(kp => !kp.Value.Contains(";")).Count() == 2);
                    }
                    if (culture_enumeration_is_supported)
                    {
                        IEnumerable<CultureInfo> cultures = asset.GetSupportedCultures()?.Distinct().Where(ci => ci.Name != "");
                        int count = cultures.Count();
                        Assert.IsTrue(count == 4);
                    }
                    Assert.AreEqual("Onnistui", asset.GetString(fi_success));
                    Assert.AreEqual("Success", asset.GetString(en_success));
                    Assert.AreEqual(null, asset.GetString(section.Key("uncertain")));
                    Assert.AreEqual("Onnistui", fi_success.ToString());
                    Assert.AreEqual("Success", en_success.ToString());
                    Assert.AreEqual("Virhe (Koodi=0xFEEDF00D)", fi.Key("Error").Format(0xFeedF00d).ToString());
                    Assert.AreEqual("Erfolg", section.Culture("de").de("Erfolg").ToString());
                }

                // Modify, Reload, Test
                if (asset is IAssetComposition composition && !composition.IsReadOnly)
                {
                    IAsset sv_asset = new LocalizationStringAsset(languageStrings_sv, AssetKeyNameProvider.Colon_Colon_Dot);
                    composition.Add(sv_asset);
                    try
                    {
                        composition.Reload();
                        if (culture_enumeration_is_supported)
                        {
                            Assert.IsTrue(asset.GetSupportedCultures().Distinct().Count() == 5);
                            Assert.IsTrue(asset.GetSupportedCultures().Distinct().Count() == 5);
                        }
                    }
                    finally
                    {
                        composition.Remove(sv_asset);
                        composition.Reload();
                    }
                }
            }
        }
    }
    
    [Case("Consume", "Resources")]
    public class ConsumeResources : AssetData
    {
        public void Run(Run run)
        {
            IAssetRoot root = run.Get<IAssetRoot>();
            IAssetKey controller = root.Type<MyController>();

            // Test resources
            if (run.Get<bool>("resources"))
            {
                IAssetKey section = controller.Section("Icon");
                IAssetKey fi = section.Culture("fi");
                IAssetKey en = section.Culture("en");
                IAssetKey fi_savo = section.Culture("fi-Savo");

                IAssetKey success = controller.Section("Icon").Key("Success");
                IAssetKey fi_success = controller.Section("Icon").Key("Success").Culture("fi");
                IAssetKey en_success = controller.Section("Icon").Key("Success").Culture("en");

                IAsset asset = run.Get<IAsset>();
                dynamic d_section = root.Type<MyController>().Section("Icon");
                var resource_enumeration_is_supported = asset.GetResourceNames() != null;
                var culture_enumeration_is_supported = asset.GetSupportedCultures() != null;

                // Assert
                if (resource_enumeration_is_supported)
                {                    
                    Assert.IsTrue(asset.GetResourceNames().Count() == 8);
                    Assert.IsTrue(asset.GetResourceNames(section).Count() == 8);
                    Assert.IsTrue(asset.GetResourceNames(fi).Count() == 2);
                    Assert.IsTrue(asset.GetResourceNames(en).Count() == 2);
                    Assert.IsTrue(asset.GetResourceNames(fi_savo).Count() == 2);
                }
                if (culture_enumeration_is_supported)
                {
                    int count = asset.GetSupportedCultures().Distinct().Count();
                    Assert.IsTrue(count == 4);
                }
                Assert.AreEqual(4, asset.GetResource(en_success)[0]);
                Assert.AreEqual(6, asset.GetResource(fi_success)[0]);
                Assert.AreEqual(4, asset.OpenStream(en_success).ReadByte());
                Assert.AreEqual(6, asset.OpenStream(fi_success).ReadByte());
                Assert.AreEqual(null, asset.GetResource(section.Key("uncertain")));
                Assert.IsTrue(((byte[])d_section.Success.Culture("en"))[0] == 4);

                // Again this time from cache
                if (resource_enumeration_is_supported)
                {
                    Assert.IsTrue(asset.GetResourceNames().Distinct().Count() == 8);
                    Assert.IsTrue(asset.GetResourceNames(section).Distinct().Count() == 8);
                    Assert.IsTrue(asset.GetResourceNames(fi).Distinct().Count() == 2);
                    Assert.IsTrue(asset.GetResourceNames(en).Distinct().Count() == 2);
                    Assert.IsTrue(asset.GetResourceNames(fi_savo).Distinct().Count() == 2);
                }
                if (culture_enumeration_is_supported)
                {
                    int count = asset.GetSupportedCultures().Distinct().Count();
                    Assert.IsTrue(count == 4);
                }
                Assert.AreEqual(4, asset.GetResource(en_success)[0]);
                Assert.AreEqual(6, asset.GetResource(fi_success)[0]);
                Assert.AreEqual(4, asset.OpenStream(en_success).ReadByte());
                Assert.AreEqual(6, asset.OpenStream(fi_success).ReadByte());
                Assert.AreEqual(null, asset.GetResource(section.Key("uncertain")));

                // Modify, Reload, Test
                if (asset is IAssetComposition composition && !composition.IsReadOnly)
                {
                    IAsset sv_asset = new AssetResourceDictionary(res_sv, AssetKeyNameProvider.Colon_Colon_Dot);
                    composition.Add(sv_asset);
                    try
                    {
                        composition.Reload();
                        if (culture_enumeration_is_supported)
                        {
                            Assert.IsTrue(asset.GetSupportedCultures().Distinct().Count() == 5);
                            Assert.IsTrue(asset.GetSupportedCultures().Distinct().Count() == 5);
                        }
                    }
                    finally
                    {
                        composition.Remove(sv_asset);
                        composition.Reload();
                    }
                }
            }
        }
    }
        
}
