// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Lexical.Localization
{
    #region interface
    /// <summary>
    /// Builder that can create <see cref="IAsset"/> instance(s).
    /// 
    /// For dependency injection.
    /// </summary>
    public interface IAssetBuilder
    {
        /// <summary>
        /// List of asset sources that can construct assets.
        /// </summary>
        IList<IAssetSource> Sources { get; }

        /// <summary>
        /// Build language strings.
        /// </summary>
        /// <returns></returns>
        IAsset Build();
    }
    #endregion interface

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
