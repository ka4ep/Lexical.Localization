using System;
using System.Collections.Generic;
using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace docs
{
    public class IAssetBuilder_Example_2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Initialize service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add IAssetBuilder, an instruction to construct one later
            serviceCollection.AddSingleton<IAssetBuilder, AssetBuilder>();
            // Add IAssetSource, that will construct cache cache
            serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddKeysCache().AddCulturesCache()));
            // Add IAssetSource, that adds strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            serviceCollection.AddSingleton<IAssetSource>(new AssetSource(new LocalizationStringAsset(strings, AssetKeyNameProvider.Default)));

            // Add delegate to forward IAsset request to IAssetBuilder
            serviceCollection.AddSingleton<IAsset>(s => s.GetService<IAssetBuilder>().Build());

            // Create service scope
            using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
            {
                // Construct new asset
                IAsset asset = serviceScope.GetService<IAsset>();

                // Create string key
                IAssetKey key = new LocalizationRoot().Key("hello").Culture("en");
                // Request string
                string str = asset.GetString(key);
                // Print result
                Console.WriteLine(str);
            }
            #endregion Snippet
        }
    }

}
