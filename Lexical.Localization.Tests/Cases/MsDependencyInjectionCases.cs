using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration.Ini;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Lexical.Localization;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests.Cases
{
    
    /// <summary>
    /// Adds a case where <see cref="IniConfigurationProvider"/> is adapted to <see cref="IAsset"/> with <see cref="ConfigurationLocalizationAsset"/> adapter.
    /// </summary>
    [Case(nameof(IAsset), "DependencyInjection+"+nameof(IConfiguration)+"+Ini")]
    public class Asset_DependencyInjection_Configuration_Ini : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Initialize service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // Add lexical localizations
            serviceCollection.AddLexicalLocalization(false, false, false, false);

            // Add embedded .ini file(s) using IConfiguration
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            serviceCollection.AddSingleton<IConfiguration>(configurationBuilder.Build());
            IFileProvider embedded = new EmbeddedFileProvider(GetType().Assembly);
            configurationBuilder.SetFileProvider(embedded);
            configurationBuilder.AddIniFile("localization.ini", optional: true, reloadOnChange: false);
            serviceCollection.AddSingleton<IConfigurationBuilder>(configurationBuilder);
            serviceCollection.AddSingleton<IAssetSource, ConfigurationBuilderLocalizationAssetSource>();

            // Build
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IAsset>();
        }
    }

}
