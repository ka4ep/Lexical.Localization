// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Adapts <see cref="IConfigurationBuilder"/> to <see cref="IAssetSource"/>.
    /// </summary>
    public class ConfigurationBuilderLocalizationAssetSource : 
        IAssetSource
    {
        public readonly IConfigurationBuilder ConfigurationBuilder;
        public readonly IAssetKeyNamePolicy NamePolicy;

        public ConfigurationBuilderLocalizationAssetSource(IConfigurationBuilder configurationBuilder, IAssetKeyNamePolicy namePolicy = null)
        {
            ConfigurationBuilder = configurationBuilder ?? throw new ArgumentNullException(nameof(configurationBuilder));
            this.NamePolicy = namePolicy;
        }

        public void Build(IList<IAsset> list)
        {
            IConfigurationRoot root = ConfigurationBuilder.Build();
            IAsset asset = new ConfigurationLocalizationAsset(root, NamePolicy);
            list.Add(asset);
        }

        public IAsset PostBuild(IAsset asset)
            => asset;
    }

    public static partial class ConfigurationLocalizationExtensions
    {
        public static IAssetBuilder AddConfigurationBuilder(this IAssetBuilder localizationBuilder, IConfigurationBuilder configurationBuilder, IAssetKeyNamePolicy namePolicy = null)
        {
            localizationBuilder.Sources.Add(new ConfigurationBuilderLocalizationAssetSource(configurationBuilder, namePolicy));
            return localizationBuilder;
        }
    }
}
