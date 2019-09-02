// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
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
    /// Asset builder constructs <see cref="IAsset"/>s from parts.
    /// 
    /// It's main purpose address how to inject assets, lazily constructed assets, files and file patterns
    /// in dependency injection use case.
    /// 
    /// <see cref="Build"/> method assembles different parts into one <see cref="IAsset"/>.
    /// The implementation determines how parts are assembled together.
    /// 
    /// <see cref="Sources"/> are constructed on <see cref="Build"/>.
    /// For example, and asset file pattern can refere to "{Culture/}localizaiton.xml".
    /// 
    /// <see cref="IAsset"/>, <see cref="IAssetSource"/>, <see cref="IAssetFile"/> and <see cref="IAssetFilePattern"/> must signal whether the 
    /// asset produces strings or resources.
    /// </summary>
    public interface IAssetBuilder
    {
        /// <summary>
        /// List of asset sources that are constructed into <see cref="IAsset"/> on <see cref="Build"/> call.
        /// </summary>
        IList<IAssetSource> Sources { get; }

        /// <summary>
        /// List of <see cref="IFileSystem"/>s that are used as file sources for asset files.
        /// </summary>
        IList<IFileSystem> FileSystems { get; }

        /// <summary>
        /// Asset file observe policy to be attached to the built asset.
        /// </summary>
        IAssetFileObservePolicy ObservePolicy { get; set; }

        /// <summary>
        /// Build asset. 
        /// </summary>
        /// <returns>asset</returns>
        /// <exception cref="AssetException">If the asset builder implementation could not build from the configuration.</exception>
        IAsset Build();
    }

    /// <summary>
    /// Policy whether asset files should be observed and reloaded when modified.
    /// </summary>
    public interface IAssetFileObservePolicy : IObservable<bool>
    {
        /// <summary>
        /// Policy whether assets should be observed and reloaded if they are modified.
        /// </summary>
        bool Observe { get; set; }
    }
    // </doc>
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static partial class IAssetBuilderExtensions
    {
        /// <summary>
        /// Add <paramref name="assetSource"/> to sources.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="assetSource"></param>
        public static IAssetBuilder AddSource(this IAssetBuilder assetBuilder, IAssetSource assetSource)
        {
            assetBuilder.Sources.Add(assetSource);
            return assetBuilder;
        }

        /// <summary>
        /// Add <paramref name="fileSystem"/> to builder.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="fileSystem"></param>
        public static IAssetBuilder AddFileSystem(this IAssetBuilder assetBuilder, IFileSystem fileSystem)
        {
            assetBuilder.FileSystems.Add(fileSystem);
            return assetBuilder;
        }

        /// <summary>
        /// Set <paramref name="observePolicy"/> to the builder.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="observePolicy">observe policy, or null to remove</param>
        public static IAssetBuilder SetObservePolicy(this IAssetBuilder assetBuilder, IAssetFileObservePolicy observePolicy)
        {
            assetBuilder.ObservePolicy = observePolicy;
            return assetBuilder;
        }

        /// <summary>
        /// Add <paramref name="assetSources"/> to sources.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="assetSources"></param>
        public static IAssetBuilder AddSources(this IAssetBuilder assetBuilder, IEnumerable<IAssetSource> assetSources)
        {
            if (assetSources == null) return assetBuilder;
            foreach(IAssetSource src in assetSources)
                assetBuilder.Sources.Add(src);
            return assetBuilder;
        }


        /// <summary>
        /// Search for classes that implement <see cref="ILibraryAssetSources"/> in <paramref name="library"/>.
        /// Instantiates them and adds as <see cref="IAssetSource"/>.
        /// 
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="library"></param>
        public static IAssetBuilder AddLibraryAssetSources(this IAssetBuilder assetBuilder, Assembly library)
        {
            if (library == null) return assetBuilder;

            IEnumerable<IAssetSource> librarysAssetSources =
                    library.GetExportedTypes()
                    .Where(t => typeof(ILibraryAssetSources).IsAssignableFrom(t))
                    .SelectMany(t => (IEnumerable<IAssetSource>)Activator.CreateInstance(t));

            foreach (IAssetSource src in librarysAssetSources)
                assetBuilder.Sources.Add(src);

            return assetBuilder;
        }

    }
}
