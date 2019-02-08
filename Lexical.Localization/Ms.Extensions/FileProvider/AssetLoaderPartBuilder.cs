// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace Lexical.Localization.Ms.Extensions
{
    public static partial class AssetLoaderPartBuilderExtensions
    {
        /// <summary>
        /// List of <see cref="IFileProvider"/>s.
        /// Value type is <see cref="List{IFileProvider}"/>. 
        /// </summary>
        public const string KEY_FILEPROVIDER = "FileProvider";

        /// <summary>
        /// Add <see cref="IFileProvider"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="fileProvider"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder FileProvider(this AssetLoaderPartBuilder partBuilder, IFileProvider fileProvider)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderResourcesParticipant.Instance);
            partBuilder.Add<IFileProvider>(KEY_FILEPROVIDER, fileProvider);
            return partBuilder;
        }

        /// <summary>
        /// Add range of <see cref="IFileProvider"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="fileProviders"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder FileProvider(this AssetLoaderPartBuilder partBuilder, IEnumerable<IFileProvider> fileProviders)
        { 
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderResourcesParticipant.Instance);
            partBuilder.AddRange<IFileProvider>(KEY_FILEPROVIDER, fileProviders);
            return partBuilder;
        }

        /// <summary>
        /// Get file <see cref="IFileProvider"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>file providers or null</returns>
        public static IList<IFileProvider> GetFileProviders(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<IFileProvider>>(KEY_FILEPROVIDER);
    }
}
