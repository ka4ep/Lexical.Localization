using System;
using System.Collections.Generic;
using System.Reflection;
using Lexical.Asset;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace docs
{
    public class IAssetBuilder_Example_4
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Initialize service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add lexical localizations
            serviceCollection.AddLexicalLocalization(false, false, false, false);

            // Add IAssetSource that reads "localizatin.ini" from embedded IFileProvider.
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());
            IFileProvider embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            configurationBuilder.SetFileProvider(embedded);
            configurationBuilder.AddIniFile("localization.ini", optional: true, reloadOnChange: false);
            serviceCollection.AddSingleton<IConfigurationBuilder>(configurationBuilder);
            serviceCollection.AddSingleton<IAssetSource, ConfigurationBuilderLocalizationAssetSource>();

            // Create service scope
            using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
            {
                // Construct new asset
                IAsset asset = serviceScope.GetService<IAsset>();
            }
            #endregion Snippet
        }
    }

}
