using Lexical.Asset;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Lexical.Utils.Permutation;

namespace Lexical.Localization.Tests.Cases
{
   
    /// <summary>
    /// Adds a case where <see cref="IniConfigurationProvider"/> is adapted to <see cref="IAsset"/> with <see cref="ConfigurationLocalizationAsset"/> adapter.
    /// </summary>
    [Case(nameof(IAsset), nameof(IConfiguration)+"+.ini")]
    public class Asset_ConfigurationLocalizationStringProvider_Ini : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create configuration builder
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Set file source
            IFileProvider embedded = new EmbeddedFileProvider(GetType().Assembly);
            configurationBuilder.SetFileProvider(embedded);

            // Add .ini file
            configurationBuilder.AddIniFile("localization.ini");

            // Build configuration
            IConfiguration configuration = configurationBuilder.Build();

            // Build asset
            IAsset asset = new ConfigurationLocalizationAsset(configuration, namePolicy: ConfigurationLocalizationAsset.CULTURE_ROOT);

            return asset;
        }
    }
    
    /// <summary>
    /// Adds a case where <see cref="XmlConfigurationProvider"/> is adapted to <see cref="IAsset"/> with <see cref="ConfigurationLocalizationAsset"/> adapter.
    /// </summary>
    [Case(nameof(IAsset), nameof(IConfiguration) + "+.xml")]
    public class Asset_ConfigurationLocalizationStringProvider_Xml : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create configuration builder
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Set file source
            IFileProvider embedded = new EmbeddedFileProvider(GetType().Assembly);
            configurationBuilder.SetFileProvider(embedded);

            // Add .xml file
            configurationBuilder.AddXmlFile("localization.xml");

            // Build configuration
            IConfiguration configuration = configurationBuilder.Build();

            // Build asset
            IAsset asset = new ConfigurationLocalizationAsset(configuration, namePolicy: ConfigurationLocalizationAsset.CULTURE_ROOT);

            return asset;
        }
    }

    /// <summary>
    /// Adds a case where <see cref="JsonConfigurationProvider"/> is adapted to <see cref="IAsset"/> with <see cref="ConfigurationLocalizationAsset"/> adapter.
    /// </summary>
    [Case(nameof(IAsset), nameof(IConfiguration) + "+.json")]
    public class Asset_ConfigurationLocalizationStringProvider_Json : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create configuration builder
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Set file source
            IFileProvider embedded = new EmbeddedFileProvider(GetType().Assembly);
            configurationBuilder.SetFileProvider(embedded);

            // Add .xml file
            configurationBuilder.AddJsonFile("localization.json");

            // Build configuration
            IConfiguration configuration = configurationBuilder.Build();

            // Build asset
            IAsset asset = new ConfigurationLocalizationAsset(configuration, namePolicy: ConfigurationLocalizationAsset.CULTURE_ROOT);

            return asset;
        }
    }

    /// <summary>
    /// Adds a case where <see cref="IniConfigurationProvider"/> is adapted to <see cref="IAsset"/> with <see cref="ConfigurationLocalizationAsset"/> adapter.
    /// </summary>
    [Case(nameof(IAsset), nameof(ConfigurationBuilderLocalizationAssetSource))]
    public class Asset_ConfigurationBuilderLocalizationAssetSource : AssetData
    {
        public object Initialize(Run init)
        {
            init["strings"] = true;
            init["resources"] = false;

            // Create file source
            IFileProvider embedded = new EmbeddedFileProvider(GetType().Assembly);

            // Create builder
            IAssetBuilder localizationBuilder = new AssetBuilder();
            localizationBuilder.AddConfigurationBuilder(new ConfigurationBuilder().SetFileProvider(embedded).AddIniFile("localization.ini"));

            // Build 
            IAsset asset = localizationBuilder.Build();

            return asset;
        }
    }

}
