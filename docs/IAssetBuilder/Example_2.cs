using System;
using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.StringFormat;
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
            serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddCulturesCache()));
            // Add IAssetSource, that adds strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            serviceCollection.AddSingleton<IAssetSource>(new AssetInstanceSource(new LocalizationAsset(strings, LineParameterPrinter.Default)));

            // Add delegate to forward IAsset request to IAssetBuilder
            serviceCollection.AddSingleton<IAsset>(s => s.GetService<IAssetBuilder>().Build());

            // Create service scope
            using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
            {
                // Construct new asset
                IAsset asset = serviceScope.GetService<IAsset>();

                // Create string key
                ILine key = new LineRoot().Key("hello").Culture("en");
                // Request string
                IString value = asset.GetString(key).GetValue();
                // Print result
                Console.WriteLine(value);
            }
            #endregion Snippet
        }
    }

}
