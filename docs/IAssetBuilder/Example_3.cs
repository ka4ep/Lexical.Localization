using Lexical.Localization;
using Lexical.Localization.StringFormat;
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

            // Add IAssetBuilder, ILineRoot and ICulturePolicy to service collection
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false, 
                addCulturePolicyService: false, 
                useGlobalInstance: false,
                addCache: true);

            // Add dictionary of strings
            Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
            serviceCollection.AddSingleton<IAssetSource>(new AssetInstanceSource(new StringAsset(strings, LineParameterPrinter.Default)));

            // Create service scope
            using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
            {
                // Construct new asset
                IAsset asset = serviceScope.GetService<IAsset>();

                // Create string key
                ILine key = new LineRoot().Key("hello").Culture("en");
                // Request string
                IString value = asset.GetString(key).GetString();
                // Print result
                Console.WriteLine(value);
            }
            #endregion Snippet
        }
    }

}
