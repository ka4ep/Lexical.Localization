using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace docs
{
    public class Ms_DependencyInjection_Example1
    {
        public static void Main(string[] args)
        {
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Create localization source
            var source = new Dictionary<string, string> { { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" } };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Service can provide the asset
                IAsset asset = serviceProvider.GetService<IAsset>();

                // Service can provide root
                IAssetRoot root = serviceProvider.GetService<IAssetRoot>();

                // Service can provide type key
                ILine typeKey = serviceProvider.GetService<ILineKey<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").Culture("en").ToString();
            }
        }
    }

}
