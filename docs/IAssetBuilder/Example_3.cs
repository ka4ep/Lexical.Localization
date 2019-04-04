using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace docs
{
    public class IAssetBuilder_Example_3
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Initialize service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add IAssetBuilder, IAssetRoot and ICulturePolicy to service collection
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false, 
                addCulturePolicyService: false, 
                useGlobalInstance: false,
                addCache: true);

            // Add dictionary of strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            serviceCollection.AddSingleton<IAssetSource>(new AssetInstanceSource(new LocalizationStringAsset(strings, AssetKeyNameProvider.Default)));

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
