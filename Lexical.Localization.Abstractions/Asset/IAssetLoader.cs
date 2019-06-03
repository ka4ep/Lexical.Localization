// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    #region interface
    /// <summary>
    /// Loads assets based on parameters in keys, and is configurable with <see cref="IAssetLoaderPart"/>.
    /// </summary>
    public interface IAssetLoader : IAssetProvider
    {
        /// <summary>
        /// List of loader parts.
        /// </summary>
        IAssetLoaderPart[] LoaderParts { get; }

        /// <summary>
        /// Add new loader function.
        /// </summary>
        /// <param name="part">Object that loads assets based on the parameters, such as "Culture"</param>
        /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
        /// <returns>this</returns>
        IAssetLoader Add(IAssetLoaderPart part);

        /// <summary>
        /// Add loader functions.
        /// </summary>
        /// <param name="part">(optional)list of loaders</param>
        /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
        /// <returns>this</returns>
        IAssetLoader AddRange(IEnumerable<IAssetLoaderPart> part);
    }
    #endregion interface
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    /// <summary></summary>
    public static partial class IAssetLoaderExtensions
    { 
        /// <summary>
        /// Apply <paramref name="configurer"/> on every part of the asset loader.
        /// </summary>
        /// <param name="assetLoader"></param>
        /// <param name="configurer">delegate to apply on every part</param>
        /// <returns>assetLoader</returns>
        public static IAssetLoader ConfigureParts(this IAssetLoader assetLoader, Action<IAssetLoaderPartOptions> configurer)
        {
            foreach (var part in assetLoader.LoaderParts)
                configurer(part.Options);
            return assetLoader;
        }
    }


}
