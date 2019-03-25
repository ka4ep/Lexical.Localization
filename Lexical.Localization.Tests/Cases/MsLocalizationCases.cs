using ConsoleApp1;
using Lexical.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests
{
    /// <summary>
    /// This test case runs permutations with StringLocalizerRoot as ILocalizationRoot.
    /// </summary>
    [Case(nameof(IAssetRoot), nameof(StringLocalizerRoot), new[] { nameof(IAsset), nameof(ICulturePolicy) })]
    public class Root_StringLocalizerRoot
    {
        public object Initialize(Run init)
            => new StringLocalizerRoot(init.Get<IAsset>(), init.Get<ICulturePolicy>());
    }
    
    /// <summary>
    /// This test case runs permutations with StringLocalizerGlobal is ILocalizationRoot.
    /// </summary>
    [Case(nameof(IAssetRoot), nameof(StringLocalizerRoot), new[] { nameof(IAsset), nameof(ICulturePolicy) })]
    public class Root_StringLocalizerGlobal
    {
        public object Initialize(Run init)
        {
            StringLocalizerRoot.Global.CulturePolicy = init.Get<ICulturePolicy>();
            StringLocalizerRoot.Global.Asset = init.Get<IAsset>();
            return StringLocalizerRoot.Global;
        }
        public void Cleanup(Run cleanup)
        {
            StringLocalizerRoot.Global.Asset = StringLocalizerRoot.Builder.Build();
        }
    }
    
    [Case(nameof(IAsset), nameof(StringLocalizerRoot))]
    public class Asset_StringLocalizerGlobal : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = true;
            IAssetBuilder builder = StringLocalizerRoot.Builder;
            builder.AddAsset(new LocalizationStringAsset(languageStrings, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new LocalizationStringAsset(languageStrings_fi_savo, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_en, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_fi, AssetKeyNameProvider.Default));
            builder.AddAsset(new ResourceStringDictionary(res_fi_savo, AssetKeyNameProvider.Default));
            return builder.Build();
        }

        public void Cleanup(Run cleanup)
        {
            StringLocalizerRoot.Builder.Sources.Clear();
        }
    }
    
    [Case(nameof(IAsset), "ResourceManagerStringLocalizerAsset")]
    public class Asset_DependencyInjection_Configuration_Resx : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            ILoggerFactory loggerFactory = new LoggerFactory();
            IAsset asset = ResourceManagerStringLocalizerAsset.Create("Lexical.Localization.Tests", "Resources", "Localization", loggerFactory);
            return asset;
        }
    }

    [Case("Consume", "StringLocalizer")]
    public class ConsumeStringLocalizer : AssetData
    {
        public void Run(Run run)
        {
            IStringLocalizerFactory asset = run.Get<IAssetRoot>() as IStringLocalizerFactory;

            // Test strings
            if (asset != null && run.Get<bool>("strings"))
            {
                IStringLocalizer section = asset.Create(typeof(MyController));
                IStringLocalizer en = section.WithCulture(CultureInfo.GetCultureInfo("en"));
                IStringLocalizer fi = section.WithCulture(CultureInfo.GetCultureInfo("fi"));
                var strings = section.GetAllStrings(true);

                // Assert
                if (strings != null) Assert.IsTrue(strings.Where(kp => !kp.Value.Contains(";")).Count() == 8);
                Assert.AreEqual("Success", en["Success"].ToString());
                Assert.AreEqual("Virhe (Koodi=0xFEEDF00D)", fi["Error", 0xFeedF00d].ToString());

                // Again this time from cache
                strings = section.GetAllStrings(true);
                if (strings != null) Assert.IsTrue(section.GetAllStrings(true).Where(kp => !kp.Value.Contains(";")).Count() == 8);
                Assert.AreEqual("Success", en["Success"].ToString());
                Assert.AreEqual("Virhe (Koodi=0xFEEDF00D)", fi["Error", 0xFeedF00d].ToString());
            }
        }
    }



    /*
        [Case(nameof(ILocalizationAsset), "Resx+DependencyInjection")]
        public class Asset_DependencyInjection_Configuration_Resx : AssetData
        {
            public object Initialize(Run init)
            {
                init["strings"] = true;
                init["resources"] = false;

                // Initialize service collection
                IServiceCollection serviceCollection = new ServiceCollection();

                // Add lexical Localizations
                serviceCollection.AddLexicalLocalization(
                    false, false, false, false);

                // Resx support
                serviceCollection.AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
                serviceCollection.AddSingleton(s => s.GetService<ResourceManagerStringLocalizerFactory>().ToSource());

                // Build
                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                return serviceProvider.GetService<ILocalizationAsset>();
            }
        }
        */

}
