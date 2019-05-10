using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace docs
{
    public class Ms_DependencyInjection_Example0
    {
        public static void Main(string[] args)
        {
            #region Snippet_1
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: false,
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);
            #endregion Snippet_1

            #region Snippet_2
            // Create localization source
            var source = new Dictionary<string, string> { { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" } };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(source, ParameterParser.Instance).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);
            #endregion Snippet_2

            #region Snippet_3
            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Service can provide the asset
                IAsset asset = serviceProvider.GetService<IAsset>();

                // Service can provide root
                ILineRoot root = serviceProvider.GetService<ILineRoot>();

                // Service can provide type key
                ILine typeKey = serviceProvider.GetService<ILineKey<ConsoleApp1.MyController>>();

                // Get "Hello World!"
                string str = typeKey.Key("Hello").Culture("en").ToString();
            }
            #endregion Snippet_3
        }
    }

}
