// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           18.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Lexical.Localization
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Localization;

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
                // Use StringLocalizerRoot as IAssetRoot
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetRoot>(
                    s=>
                    {
                        IAsset asset = s.GetService<IAsset>(); // DO NOT REMOVE
                        ICulturePolicy culturePolicy = s.GetService<ICulturePolicy>();
                        if (culturePolicy != null) StringLocalizerRoot.Global.CulturePolicy = culturePolicy;
                        return StringLocalizerRoot.Global;
                    }
                    ));
            }
            else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetRoot, StringLocalizerRoot>());
            }

            // IAssetKeyAssetAssigned
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetKeyAssetAssigned>(s => s.GetService<IAssetRoot>() as IAssetKeyAssetAssigned));

            // ICulturePolicy
            if (addCulturePolicyService)
            {
                if (useGlobalInstance)
                {
                    serviceCollection.TryAdd(ServiceDescriptor.Singleton<ICulturePolicy>( StringLocalizerRoot.Global.CulturePolicy ));
                }
                else
                {
                    serviceCollection.TryAdd(ServiceDescriptor.Singleton<ICulturePolicy>(
                        s => new CulturePolicy().SetToCurrentCulture()
                        ));
                }
            }

            // IAssetBuilder
            if (useGlobalInstance)
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder>(s=>
                {
                    IEnumerable<IAssetSource> assetSources = s.GetServices<IAssetSource>();
                    IAssetBuilder builder = StringLocalizerRoot.Builder;
                    builder.AddSources( assetSources );
                    return builder;
                }));
            } else
            {
                serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAssetBuilder>(s=>
                {
                    // Get IAssetSource services
                    IEnumerable<IAssetSource> assetSources = s.GetServices<IAssetSource>();
                    // Get IEnumerable<IAssetSource> services
                    IEnumerable<IEnumerable<IAssetSource>> assetSourcesLists = s.GetServices<IEnumerable<IAssetSource>>();
                    // Get IEnumerable<ILibraryAssetSources> services
                    IEnumerable<ILibraryAssetSources> libraryAssetSourcesLists = s.GetServices<ILibraryAssetSources>();
                    // Concatenate
                    if (assetSourcesLists != null)
                    {
                        foreach(IEnumerable<IAssetSource> assetSources_ in assetSourcesLists)
                            assetSources = assetSources == null ? assetSources_ : assetSources.Concat(assetSources_);
                    }
                    if (libraryAssetSourcesLists != null)
                    {
                        foreach (IEnumerable<IAssetSource> assetSources_ in libraryAssetSourcesLists)
                            assetSources = assetSources == null ? assetSources_ : assetSources.Concat(assetSources_);
                    }
                    // Take distinct
                    if (assetSources != null) assetSources = assetSources.Distinct();
                    // Is it still empty
                    if (assetSources == null) assetSources = new IAssetSource[0];
                    // Create builder
                    AssetBuilder.OneBuildInstance builder = new AssetBuilder.OneBuildInstance(assetSources);
                    return builder;
                }));
            }

            // IAsset
            serviceCollection.TryAdd(ServiceDescriptor.Singleton<IAsset>(s => s.GetService<IAssetBuilder>().Build()));

            // IAssetKey<>
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
                    return new StringLocalizerRoot.LinkedTo(localizationRoot);
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

        /// <summary>
        /// Search for classes that implement <see cref="ILibraryAssetSources"/> in <paramref name="library"/>.
        /// Instantiates them and adds as services of <see cref="ILibraryAssetSources"/>, which will be picked up
        /// by <see cref="AddLexicalLocalization"/>.
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="library">(optional) library to scan</param>
        public static IServiceCollection AddAssetLibrarySources(this IServiceCollection services, Assembly library)
        {
            if (library == null) return services;

            IEnumerable<ServiceDescriptor> librarysAssetSourceServices =
                    library
                    .GetExportedTypes()
                    .Where(t => typeof(ILibraryAssetSources).IsAssignableFrom(t))
                    .Select(t => new ServiceDescriptor(typeof(ILibraryAssetSources), t, ServiceLifetime.Singleton));

            foreach (ServiceDescriptor serviceDescriptor in librarysAssetSourceServices)
                services.Add(serviceDescriptor);

            return services;
        }
    }
}
