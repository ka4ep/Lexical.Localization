using Lexical.Localization.Ms.Extensions;
using Lexical.Utils.Permutation;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Lexical.Localization.Tests.Cases
{
    [Case(nameof(IAsset), nameof(AssetLoaderPartFileProviderStrings)+"+"+nameof(EmbeddedFileProvider) + "+.ini")]
    public class Asset_FileProviderAssetLoader_EmbeddedFileProvider_LocalizationStringsIni : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // File provider
            IFileProvider fileProvider = new EmbeddedFileProvider(GetType().Assembly);

            // Build
            IAsset asset =
                new AssetLoader()
                .Add(new AssetLoaderPartFileProviderStrings(fileProvider, "Localization.localization{-culture}.ini", AssetKeyNameProvider.Default))
                .Add(new AssetLoaderPartFileProviderResources(fileProvider, "Resources.res{.culture}[.type<.*>][.section][.key]").AddMatchParameters("type", "section"));

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetLoaderPartFileProviderStrings) + "+" + nameof(PhysicalFileProvider) + "+.ini")]
    public class Asset_FileProviderAssetLoader_PhysicalFileProvider_LocalizationStringsIni : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;

            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Build
            IAsset asset = 
                new AssetLoader()
                .Add(new AssetLoaderPartFileProviderStrings(fileProvider, "{location/}Localization/localization{-culture}.ini", AssetKeyNameProvider.Default).AddMatchParameters("location"))
                .Add(new AssetLoaderPartFileProviderResources(fileProvider, "Resources/res{.culture}[.type<.*>][.section][.key]").AddMatchParameters("type", "section"));

            return asset;
        }
    }
    
    [Case(nameof(IAsset), nameof(AssetLoader) + "/.resx")]
    public class Asset_LocalizationAssetLoader_Resx : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Create loader
            IAsset asset =
                new AssetLoader()
                .Add(new AssetLoaderPartFileProviderStrings(fileProvider, "Resources/localization{.culture}.resx", AssetKeyNameProvider.Colon_Dot_Dot));

            return asset;
        }
    }

    [Case(nameof(IAsset), nameof(AssetLoader) + "/.resources")]
    public class Asset_LocalizationAssetLoader_Resources : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Create loader
            IAsset asset =
                new AssetLoader()
                .Add(new AssetLoaderPartFileProviderStrings(fileProvider, "Assets/localization{.culture}.resources", AssetKeyNameProvider.Colon_Dot_Dot));
                
            return asset;
        }
    }

}
