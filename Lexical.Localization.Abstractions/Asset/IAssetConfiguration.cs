// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Configuration that <see cref="IAssetBuilder"/> uses for constructing a composed <see cref="IAsset"/>.
    /// </summary>
    public interface IAssetConfiguration
    {
        /// <summary>
        /// List of assets.
        /// </summary>
        IList<IAsset> Assets { get; }

        /// <summary>
        /// List of asset factories.
        /// </summary>
        IList<IAssetFactory> AssetFactories { get; }

        /// <summary>
        /// Asset file observe policy for asset files that don't have explicitly configured <see cref="IAssetSourceObservePolicy"/>.
        /// </summary>
        IAssetObservePolicy AssetObservePolicy { get; set; }

        /// <summary>
        /// Post build actions
        /// </summary>
        IList<IAssetPostBuild> AssetPostBuilds { get; }

        /// <summary>
        /// List of asset sources.
        /// </summary>
        IList<IAssetSource> AssetSources { get; }

        /// <summary>
        /// List of <see cref="IFileSystem"/>s that are used as file sources for <see cref="IAssetSource"/>s.
        /// that don't have explicitly have a configured <see cref="IAssetSourceFileSystem"/>.
        /// </summary>
        IList<IFileSystem> FileSystems { get; }
    }

    /// <summary>
    /// Policy whether asset files should be observed and reloaded when modified.
    /// </summary>
    public interface IAssetObservePolicy : IObservable<bool>
    {
        /// <summary>
        /// Policy whether assets should be observed and reloaded if they are modified.
        /// </summary>
        bool Observe { get; set; }
    }

    /// <summary>
    /// Post build action for <see cref="IAssetBuilder"/>.
    /// </summary>
    public interface IAssetPostBuild
    {
        /// <summary>
        /// Allows source to do post build action and to decorate already built asset.
        /// 
        /// This allows a source to provide decoration such as cache.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>asset or component</returns>
        IAsset PostBuild(IAsset asset);
    }

    /// <summary>
    /// Factory that constructs <see cref="IAsset"/>(s). 
    /// </summary>
    public interface IAssetFactory
    {
        /// <summary>
        /// Source adds its <see cref="IAsset"/>s to list.
        /// </summary>
        /// <returns>new assets</returns>
        IEnumerable<IAsset> Create();
    }
    // </doc>

    // <doc2>
    /// <summary>
    /// This interface signals that the class provides asset configuration for a class library.
    /// </summary>
    public interface ILibraryAssetConfiguration : IAssetConfiguration { }
    // </doc2>
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static partial class IAssetConfigurationExtensions
    {
        /// <summary>
        /// Add <paramref name="asset"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="asset"></param>
        public static IAssetConfiguration AddAsset(this IAssetConfiguration assetConfiguration, IAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.Assets.Add(asset);
            return assetConfiguration;
        }

        /// <summary>
        /// Add <paramref name="assetFactory"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="assetFactory"></param>
        public static IAssetConfiguration AddAsset(this IAssetConfiguration assetConfiguration, IAssetFactory assetFactory)
        {
            if (assetFactory == null) throw new ArgumentNullException(nameof(assetFactory));
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.AssetFactories.Add(assetFactory);
            return assetConfiguration;
        }

        /// <summary>
        /// Add <paramref name="assetSource"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="assetSource"></param>
        public static IAssetConfiguration AddAssetSource(this IAssetConfiguration assetConfiguration, IAssetSource assetSource)
        {
            if (assetSource == null) throw new ArgumentNullException(nameof(assetSource));
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.AssetSources.Add(assetSource);
            return assetConfiguration;
        }

        /// <summary>
        /// Add <paramref name="assetSources"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="assetSources"></param>
        public static IAssetConfiguration AddSources(this IAssetConfiguration assetConfiguration, IEnumerable<IAssetSource> assetSources)
        {
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            if (assetSources == null) return assetConfiguration;
            foreach (IAssetSource src in assetSources)
                assetConfiguration.AssetSources.Add(src);
            return assetConfiguration;
        }

        /// <summary>
        /// Add <paramref name="assetPostBuild"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="assetPostBuild"></param>
        public static IAssetConfiguration AddAssetPostBuild(this IAssetConfiguration assetConfiguration, IAssetPostBuild assetPostBuild)
        {
            if (assetPostBuild == null) throw new ArgumentNullException(nameof(assetPostBuild));
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.AssetPostBuilds.Add(assetPostBuild);
            return assetConfiguration;
        }

        /// <summary>
        /// Add <paramref name="fileSystem"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="fileSystem"></param>
        public static IAssetConfiguration AddFileSystem(this IAssetConfiguration assetConfiguration, IFileSystem fileSystem)
        {
            if (fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.FileSystems.Add(fileSystem);
            return assetConfiguration;
        }

        /// <summary>
        /// Add all configurations from <paramref name="assetConfigurationFrom"/> to <paramref name="assetConfigurationTo"/>.
        /// </summary>
        /// <param name="assetConfigurationTo"></param>
        /// <param name="assetConfigurationFrom"></param>
        public static IAssetConfiguration AddAssetConfiguration(this IAssetConfiguration assetConfigurationTo, IAssetConfiguration assetConfigurationFrom)
        {
            if (assetConfigurationFrom == null) throw new ArgumentNullException(nameof(assetConfigurationFrom));
            if (assetConfigurationTo == null) throw new ArgumentNullException(nameof(assetConfigurationTo));

            // Add assets
            if (assetConfigurationFrom.Assets != null)
                foreach (IAsset asset in assetConfigurationFrom.Assets)
                    assetConfigurationTo.Assets.Add(asset);

            // Add asset factories
            if (assetConfigurationFrom.AssetFactories != null)
                foreach (IAssetFactory assetFactory in assetConfigurationFrom.AssetFactories)
                    assetConfigurationTo.AssetFactories.Add(assetFactory);

            // Add asset sources
            if (assetConfigurationFrom.AssetSources != null)
                foreach (IAssetSource assetSource in assetConfigurationFrom.AssetSources)
                    assetConfigurationTo.AssetSources.Add(assetSource);

            // Add asset filesystems
            if (assetConfigurationFrom.FileSystems != null)
                foreach (IFileSystem fileSystem in assetConfigurationFrom.FileSystems)
                    assetConfigurationTo.FileSystems.Add(fileSystem);

            // Add observe policy
            if (assetConfigurationFrom.AssetObservePolicy != null)
                assetConfigurationTo.AssetObservePolicy = assetConfigurationFrom.AssetObservePolicy;

            return assetConfigurationTo;
        }

        /// <summary>
        /// Set <paramref name="observePolicy"/> to configuration.
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="observePolicy">observe policy, or null to remove previous policy</param>
        public static IAssetConfiguration SetObservePolicy(this IAssetConfiguration assetConfiguration, IAssetObservePolicy observePolicy)
        {
            if (assetConfiguration == null) throw new ArgumentNullException(nameof(assetConfiguration));
            assetConfiguration.AssetObservePolicy = observePolicy;
            return assetConfiguration;
        }

        /// <summary>
        /// Search for classes that implement <see cref="ILibraryAssetConfiguration"/> in <paramref name="library"/>.
        /// Instantiates them and adds as <see cref="IAssetSource"/>.
        /// 
        /// </summary>
        /// <param name="assetConfiguration"></param>
        /// <param name="library"></param>
        public static IAssetConfiguration AddLibraryAssetConfiguration(this IAssetConfiguration assetConfiguration, Assembly library)
        {
            if (library == null) return assetConfiguration;

            IEnumerable<IAssetConfiguration> librarysAssetSources =
                    library.GetExportedTypes()
                    .Where(t => typeof(ILibraryAssetConfiguration).IsAssignableFrom(t))
                    .Select(t => (IAssetConfiguration)Activator.CreateInstance(t));

            foreach (IAssetConfiguration src in librarysAssetSources)
                assetConfiguration.AddAssetConfiguration(src);

            return assetConfiguration;
        }

    }
}
