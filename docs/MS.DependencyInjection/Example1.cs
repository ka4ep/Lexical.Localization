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

            // Add localization services: ILineRoot, ILine<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: false);

            // Create localization source
            var lines = new List<ILine> { LineAppender.Default.Culture("en").Type("ConsoleApp1.MyController").Key("Hello").Value("Hello World!") };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(lines).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Service can provide the asset
                IAsset asset = serviceProvider.GetService<IAsset>();

                // Service can provide root
                ILineRoot root = serviceProvider.GetService<ILineRoot>();

                // Service can provide type key
                ILine typeKey = serviceProvider.GetService<ILine<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").Culture("en").ToString();
            }
        }
    }

}
