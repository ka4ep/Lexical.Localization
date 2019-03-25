using Lexical.Localization;
using Lexical.Utils.Permutation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Lexical.Localization.Tests
{
    public class AssetData
    {
        public Dictionary<string, string> languageStrings = new Dictionary<string, string>();
        public Dictionary<string, string> languageStrings_en = new Dictionary<string, string>();
        public Dictionary<string, string> languageStrings_fi = new Dictionary<string, string>();
        public Dictionary<string, string> languageStrings_fi_savo = new Dictionary<string, string>();
        public Dictionary<string, string> languageStrings_sv = new Dictionary<string, string>();

        public Dictionary<string, byte[]> res = new Dictionary<string, byte[]>();
        public Dictionary<string, byte[]> res_en = new Dictionary<string, byte[]>();
        public Dictionary<string, byte[]> res_fi = new Dictionary<string, byte[]>();
        public Dictionary<string, byte[]> res_fi_savo = new Dictionary<string, byte[]>();
        public Dictionary<string, byte[]> res_sv = new Dictionary<string, byte[]>();

        public AssetData()
        {
            languageStrings["ConsoleApp1.MyController:Success"] = "Success";
            languageStrings["ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings_en["en:ConsoleApp1.MyController:Success"] = "Success";
            languageStrings_en["en:ConsoleApp1.MyController:Error"] = "Error (Code=0x{0:X8})";
            languageStrings_fi["fi:ConsoleApp1.MyController:Success"] = "Onnistui";
            languageStrings_fi["fi:ConsoleApp1.MyController:Error"] = "Virhe (Koodi=0x{0:X8})";
            languageStrings_fi_savo["fi-Savo:ConsoleApp1.MyController:Success"] = "Onnistuepie";
            languageStrings_fi_savo["fi-Savo:ConsoleApp1.MyController:Error"] = "Epäonnistuepa (Koodi=0x{0:X8})";
            languageStrings_sv["sv:ConsoleApp1.MyController:Success"] = "Det funkar";
            languageStrings_sv["sv:ConsoleApp1.MyController:Error"] = "Sönder (kod=0x{0:X8})";

            res["ConsoleApp1.MyController:Icon:Success"] = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            res["ConsoleApp1.MyController:Icon:Error"] = new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            res_en["en:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 4, 5, 6, 7, 8, 9, 10, 1, 2, 3 };
            res_en["en:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 7, 6, 5, 4, 3, 2, 1, 10, 9, 8, };
            res_fi["fi:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, };
            res_fi["fi:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 5, 4, 3, 2, 1, 10, 9, 8, 7, 6, };
            res_fi_savo["fi-Savo:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 9, 10, 1, 2, 3, 4, 5, 6, 7, 8 };
            res_fi_savo["fi-Savo:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 2, 1, 10, 9, 8, 7, 6, 5, 4, 3 };
            res_sv["sv:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 1 };
            res_sv["sv:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 10 };
        }

        public static IAssetKeyNamePolicy Policy = new AssetNamePattern("{Culture:}{Type:}{anysection_n:}[Key]{:Key_n}{:N}{:N1}");

    }

    [Case(nameof(IAsset), nameof(LocalizationStringAsset))]
    public class Asset_LocalizationStringAsset : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create dictionary
            var source = new Dictionary<string, string>(languageStrings.Concat(languageStrings_en).Concat(languageStrings_fi).Concat(languageStrings_fi_savo));

            // Create asset
            IAsset asset = new LocalizationStringAsset(source, AssetKeyNameProvider.Default); 

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(ResourceStringDictionary))]
    public class Asset_LocalizationResourceDictionary : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = false;
            init["resources"] = true;

            // Create dictionary
            var source = new Dictionary<string, byte[]>(res.Concat(res_en).Concat(res_fi).Concat(res_fi_savo));

            // Create asset
            IAsset asset = new ResourceStringDictionary(source, AssetKeyNameProvider.Default);

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(AssetComposition))]
    public class Asset_AssetComposition : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create composition
            IAssetComposition composition = new AssetComposition();

            // Add asset components
            composition.AddStrings(languageStrings, AssetKeyNameProvider.Default);
            composition.AddStrings(languageStrings_en, AssetKeyNameProvider.Default);
            composition.AddStrings(languageStrings_fi, AssetKeyNameProvider.Default);
            composition.AddStrings(languageStrings_fi_savo, AssetKeyNameProvider.Default);
            composition.AddDictionary(res, AssetKeyNameProvider.Default);
            composition.AddDictionary(res_en, AssetKeyNameProvider.Default);
            composition.AddDictionary(res_fi, AssetKeyNameProvider.Default);
            composition.AddDictionary(res_fi_savo, AssetKeyNameProvider.Default);

            // Asset composition
            IAsset asset = composition;

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetBuilder))]
    public class Asset_LocalizationAssetBuilder : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create builder
            IAssetBuilder builder = new AssetBuilder();

            // Add asset components
            builder.AddStrings(languageStrings, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_en, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_fi, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_fi_savo, AssetKeyNameProvider.Default);
            builder.AddDictionary(res, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_en, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_fi, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_fi_savo, AssetKeyNameProvider.Default);

            // Build asset composition
            IAsset asset = builder.Build();

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetBuilder.OneBuildInstance))]
    public class Asset_LocalizationAssetBuilder_OneBuildInstance : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create builder
            IAssetBuilder builder = new AssetBuilder.OneBuildInstance();

            // Add asset components
            builder.AddStrings(languageStrings, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_en, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_fi, AssetKeyNameProvider.Default);
            builder.AddStrings(languageStrings_fi_savo, AssetKeyNameProvider.Default);
            builder.AddDictionary(res, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_en, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_fi, AssetKeyNameProvider.Default);
            builder.AddDictionary(res_fi_savo, AssetKeyNameProvider.Default);

            // Build asset composition
            IAsset asset = builder.Build();

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(LocalizationRoot))]
    public class Asset_Localization : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create Builder
            IAssetBuilder builder = LocalizationRoot.Builder;

            // Add asset sources
            builder.AddAsset(new LocalizationStringAsset(languageStrings, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_fi_savo, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_fi_savo, AssetKeyNameProvider.Default));

            // Build asset composition
            IAsset asset = builder.Build();

            return asset;
        }
        public void Cleanup(Run cleanup)
        {
            LocalizationRoot.Builder.Sources.Clear();
        }
    }

}
