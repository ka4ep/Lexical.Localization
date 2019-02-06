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
            languageStrings_sv["sv:ConsoleApp1.MyController.Success"] = "Det funkar";
            languageStrings_sv["sv:ConsoleApp1.MyController.Error"] = "Sönder (kod=0x{0:X8})";

            res["ConsoleApp1.MyController:Icon:Success"] = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            res["ConsoleApp1.MyController:Icon:Error"] = new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            res_en["en:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 4, 5, 6, 7, 8, 9, 10, 1, 2, 3 };
            res_en["en:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 7, 6, 5, 4, 3, 2, 1, 10, 9, 8, };
            res_fi["fi:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, };
            res_fi["fi:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 5, 4, 3, 2, 1, 10, 9, 8, 7, 6, };
            res_fi_savo["fi-Savo:ConsoleApp1.MyController:Icon:Success"] = new byte[] { 9, 10, 1, 2, 3, 4, 5, 6, 7, 8 };
            res_fi_savo["fi-Savo:ConsoleApp1.MyController:Icon:Error"] = new byte[] { 2, 1, 10, 9, 8, 7, 6, 5, 4, 3 };
            res_sv["sv:ConsoleApp1.MyController:Icon.Success"] = new byte[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 1 };
            res_sv["sv:ConsoleApp1.MyController:Icon.Error"] = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 10 };
        }
    }

    [Case(nameof(IAsset), nameof(LocalizationStringDictionary))]
    public class Asset_LocalizationStringDictionary : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create dictionary
            var source = new Dictionary<string, string>(languageStrings.Concat(languageStrings_en).Concat(languageStrings_fi).Concat(languageStrings_fi_savo));

            // Create asset
            IAsset asset = new LocalizationStringDictionary(source, AssetKeyNameProvider.Default);

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetResourceDictionary))]
    public class Asset_LocalizationResourceDictionary : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = false;
            init["resources"] = true;

            // Create dictionary
            var source = new Dictionary<string, byte[]>(res.Concat(res_en).Concat(res_fi).Concat(res_fi_savo));

            // Create asset
            IAsset asset = new AssetResourceDictionary(source, AssetKeyNameProvider.Default);

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
            composition.AddDictionary(languageStrings, AssetKeyNameProvider.Default);
            composition.AddDictionary(languageStrings_en, AssetKeyNameProvider.Default);
            composition.AddDictionary(languageStrings_fi, AssetKeyNameProvider.Default);
            composition.AddDictionary(languageStrings_fi_savo, AssetKeyNameProvider.Default);
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
            builder.AddDictionary(languageStrings, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_en, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_fi, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_fi_savo, AssetKeyNameProvider.Default);
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
            builder.AddDictionary(languageStrings, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_en, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_fi, AssetKeyNameProvider.Default);
            builder.AddDictionary(languageStrings_fi_savo, AssetKeyNameProvider.Default);
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
            builder.AddAsset(new LocalizationStringDictionary(languageStrings, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringDictionary(languageStrings_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringDictionary(languageStrings_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringDictionary(languageStrings_fi_savo, AssetKeyNameProvider.Default));
            builder.AddAsset(new AssetResourceDictionary(res, AssetKeyNameProvider.Default));
            builder.AddAsset(new AssetResourceDictionary(res_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new AssetResourceDictionary(res_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new AssetResourceDictionary(res_fi_savo, AssetKeyNameProvider.Default));

            // Build asset composition
            IAsset asset = builder.Build();

            return asset;
        }
        public void Cleanup(Run cleanup)
        {
            LocalizationRoot.Builder.Sources.Clear();
        }
    }

    [Case(nameof(IAsset), nameof(AssetLoader) + "/.ini")]
    public class Asset_LocalizationAssetLoader_Ini : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create loader
            Assembly assembly = GetType().Assembly;
            IAsset asset =
                new AssetLoader()
                .Add( 
                    new AssetLoaderPartEmbeddedStrings("[assembly.]Localization.localization{-culture}.ini", AssetKeyNameProvider.Default)
                        .AddMatchParameters("assembly")
                        .AddAssembly(assembly) )
                .Add( 
                    new AssetLoaderPartEmbeddedResources("[assembly.]Resources.res{.culture}[.type<.*>][.section][.key]")
                        .AddMatchParameters("assembly", "type", "section")
                        .AddAssembly(assembly));

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(ResourceManagerAsset)+"/Location")]
    public class Asset_ResourceManagerAsset_Location : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create asset that reads .resx
            Assembly asm = GetType().Assembly;
            IAsset asset = ResourceManagerAsset.CreateLocation($"{asm.GetName().Name}.Resources.localization", GetType().Assembly);

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(ResourceManagerAsset) + "/Type")]
    public class Asset_ResourceManagerAsset_Type : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create asset that reads .resx
            Assembly asm = GetType().Assembly;
            IAsset asset = ResourceManagerAsset.CreateType(new ResourceManager($"{asm.GetName().Name}.Resources.{typeof(ConsoleApp1.MyController).FullName}", asm, null), typeof(ConsoleApp1.MyController));

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetLoaderPartEmbeddedResourceManager)+"/BaseName")]
    public class Asset_AssetLoaderEmbeddedPartResourceManager_BaseName : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create loader that can read ".resources" embedded ResourceManager files. Add matchParameters to make it match against all detected embedded files.
            IAssetLoaderPart part_basenames = new AssetLoaderPartEmbeddedResourceManager("{assembly}{.resource}.localization.resources").AddMatchParameters("assembly", "resource").AddAssembly(GetType().Assembly);

            // Create asset that reads ".resources" files. 
            IAsset asset = new AssetLoader(part_basenames);

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(AssetLoaderPartEmbeddedResourceManager) + "/Type")]
    public class Asset_AssetLoaderEmbeddedPartResourceManager_Type : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // Create loader that can read ".resources" embedded ResourceManager files for types. Add matchParameters to make it match against all detected embedded files. Keys will still need "type" section.
            IAssetLoaderPart part_types = new AssetLoaderPartEmbeddedResourceManager("{assembly}{.resource}[.type].resources").AddMatchParameters("assembly", "resource").AddAssembly(GetType().Assembly);

            // Create asset that reads ".resources" files. Add assemblies to every loader part.
            IAsset asset = new AssetLoader(part_types);

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetLoaderPartFileResourceManager) + "/Type")]
    public class Asset_AssetLoaderPartFileResourceManager_Location : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false; // <- the .fi.resources, .en.resources and .sv.resources files don't have references to the byte files in them, so the test fails. 

            // Copy .resources files to temp
            /*
            Assembly asm = GetType().Assembly;
            foreach(string name in asm.GetManifestResourceNames())
            {
                if (!name.EndsWith(".resources")) continue;
                using (var s = asm.GetManifestResourceStream(name))
                {
                    byte[] data = new byte[s.Length];
                    s.Read(data, 0, data.Length);
                    using (var fs = new FileStream(Path.Combine(Path.GetTempPath(), name), FileMode.CreateNew))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            // From tmp we can copy them to Assets/ folder of this cs project.
            */

            /*
            using (ResourceWriter rw = new ResourceWriter(@"C:\Users\tonik\Desktop\Lexical\Localization\Lexical.Localization.Tests\Resources\localization.fi.resources"))
            {
                rw.AddResource("ConsoleApp1.MyController.Error", "Virhe (Koodi=0x{0:X8})");
                rw.AddResource("ConsoleApp1.MyController.Success", "Onnistui");
                rw.Generate();
            }
            using (ResourceWriter rw = new ResourceWriter(@"C:\Users\tonik\Desktop\Lexical\Localization\Lexical.Localization.Tests\Resources\localization.sv.resources"))
            {
                rw.AddResource("ConsoleApp1.MyController.Error", "Sönder (kod=0x{0:X8})");
                rw.AddResource("ConsoleApp1.MyController.Success", "Det funkar");
                rw.Generate();
            }*/

            // Create loader that can read ".resources" ResourceManager files. Add matchParameters to make it match against all "location" folders.
            IAssetLoaderPart part_folder = new AssetLoaderPartFileResourceManager(@"Assets\{location\}localization.resources").AddMatchParameters("location").AddPaths(".");

            // Create asset that reads ".resources" files. Add assemblies to every loader part.
            IAsset asset = new AssetLoader(part_folder);

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(AssetLoaderPartFileStrings))]
    public class Asset_AssetLoaderPartFileStrings : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create loader that can read ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartFileStrings(@"Assets/localization.ini", AssetFileConstructors.Ini).AddPaths(".");

            // Create asset that reads ".resources" files. Add assemblies to every loader part.
            IAsset asset = new AssetLoader(part);

            return asset;
        }
    }

}
