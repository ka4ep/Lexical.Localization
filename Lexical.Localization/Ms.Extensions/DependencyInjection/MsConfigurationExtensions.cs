// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Lexical.Localization.Ms.Extensions
{
    public static partial class MsConfigurationExtensions
    {
        /// <summary>
        /// Adds an adapter that adapts a required service <see cref="IConfigurationBuilder"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationAsset_From_ConfigurationBuilder(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAssetSource, ConfigurationBuilderLocalizationAssetSource>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds an adapter that adapts a specific instnace <see cref="IConfigurationBuilder"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationAsset(this IServiceCollection serviceCollection, IConfigurationBuilder configurationBuilder)
        {
            serviceCollection.AddSingleton<IAssetSource>(new ConfigurationBuilderLocalizationAssetSource(configurationBuilder));
            return serviceCollection;
        }

        /// <summary>
        /// Adds an instance of <see cref="IConfiguration"/> as <see cref="IAsset"/>.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="namePolicy"></param>
        /// <param name="parametrizer"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationAsset(this IServiceCollection serviceCollection, IConfiguration configuration, IAssetKeyNamePolicy namePolicy = default)
        {
            serviceCollection.AddSingleton<IAssetSource>(
                new ConfigurationLocalizationAsset(configuration, namePolicy).ToSource());
            return serviceCollection;
        }

    }
}
