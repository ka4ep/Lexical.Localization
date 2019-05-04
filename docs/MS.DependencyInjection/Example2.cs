using Lexical.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace docs
{
    public class Ms_DependencyInjection_Example2
    {
        public static void Main(string[] args)
        {
            #region Snippet
            // Create service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add localization services: IAssetRoot, IAssetKey<T>, IAssetBuilder, ICulturePolicy
            //                            IStringLocalizer<T>, IStringLocalizerFactory
            serviceCollection.AddLexicalLocalization(
                addStringLocalizerService: true,     // <- string localizer
                addCulturePolicyService: true,
                useGlobalInstance: false,
                addCache: true);

            // Create localization source
            var source = new Dictionary<string, string> {
                { "Culture:en:Type:ConsoleApp1.MyController:Key:Hello", "Hello World!" }
            };
            // Create asset source
            IAssetSource assetSource = new LocalizationAsset(source, ParameterPolicy.Instance).ToSource();
            // Add asset source
            serviceCollection.AddSingleton<IAssetSource>(assetSource);

            // Build service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Get string localizer for class "ConsoleApp1.MyController".
                IStringLocalizer stringLocalizer 
                    = serviceProvider.GetService<IStringLocalizer<ConsoleApp1.MyController>>();

                // Narrow scope down to "en" culture
                IStringLocalizer stringLocalizerScoped = stringLocalizer.WithCulture(CultureInfo.GetCultureInfo("en"));

                // Get "Hello World!"
                string str = stringLocalizerScoped.GetString("Hello");
            }
            #endregion Snippet
        }
    }

}
