// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Lexical.Localization.Ms.Extensions
{
    public static partial class MsLocalizationExtensions
    {
        /// <summary>
        /// Adds the following Lexical.Localization services:
        ///    <see cref="IAssetRoot"/>
        ///    <see cref="IAssetKey{T}"/>
        ///    <see cref="IAssetBuilder"/>
        ///    
        /// If <paramref name="addCulturePolicyService"/> is true a <see cref="CultureResolver"/> is added,
        /// otherwise <see cref="ICulturePolicy"/> must be added to the service collection.
        /// 
        /// Further services are needed:
        ///    <see cref="IAssetSource"/> one or more.
        ///    
        /// If <paramref name="addStringLocalizerService"/> is true, the following services are added:
        ///    <see cref="IStringLocalizerFactory"/>
        ///    <see cref="IStringLocalizer{T}"/>
        ///    
        /// If <paramref name="useGlobalInstance"/> is true, then uses global <see cref="LocalizationRoot"/>.
        /// 
        /// 
        /// After this call, the <paramref name="serviceCollection"/> still needs to be populated with 
        /// instances of <see cref="IAssetSource"/>, such as:
        ///     <see cref="ConfigurationAssetSource"/>
        ///     <see cref="AssetResourceDictionary"/>
        ///     <see cref="LocalizationStringAsset"/>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="addStringLocalizerService"></param>
        /// <param name="addCulturePolicyService">Add instance of <see cref="CultureResolver"/></param>
        /// <param name="useGlobalInstance"></param>
        /// <param name="addCache"></param>
        /// <returns></returns>
        public static IServiceCollection AddLexicalLocalization(
            this IServiceCollection serviceCollection, 
            bool addStringLocalizerService = true,
            bool addCulturePolicyService = true,
            bool useGlobalInstance = false,
            bool addCache = false)
        {
            if (useGlobalInstance)
            {
                // Use StringLocalizerRoot as ILocalizationRoot
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetRoot>(StringLocalizerRoot.Global));
            }
            else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetRoot, StringLocalizerRoot>());
            }

            // ICulturePolicy
            if (addCulturePolicyService) serviceCollection.TryAdd(ServiceDescriptor.Singleton<ICulturePolicy, CulturePolicy>());

            // ILocalizationAssetBuilder
            if (useGlobalInstance)
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder>(StringLocalizerRoot.Builder));
            } else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder, AssetBuilder.OneBuildInstance>());
            }

            // ILocalizationAsset
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAsset>(s => s.GetService<IAssetBuilder>().Build()));
            // ILocalizationKey<>
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IAssetKey<>), typeof(StringLocalizerKey._Type<>)));

            // IStringLocalizer<>
            // IStringLocalizerFactory
            if (addStringLocalizerService)
            {
                // Service request for IStringLocalizer<> instances
                serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(IStringLocalizer<>), typeof(StringLocalizerKey._Type<>)));
                // Service reqeust for IStringLocalizerFactory
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IStringLocalizerFactory>(s =>
                {
                    IAssetRoot localizationRoot = s.GetService<IAssetRoot>();
                    // Use the StringLocalizerKey or StringLocalizerRoot implementation from th service.
                    if (localizationRoot is StringLocalizerKey casted) return casted;
                    // Create new root that implements IStringLocalizerFactory and acquires asset and policy with delegate
                    return new StringLocalizerRoot.Mutable(new LocalizationAssetFunc(() => localizationRoot.FindAsset()), new CulturePolicy().SetSourceFunc(() => localizationRoot.FindCulturePolicy()));
                }));
            }

            // Add cache
            if (addCache)
            {
                // Add cache
                serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddKeysCache().AddCulturesCache()));
            }

            return serviceCollection;
        }
    }
}
