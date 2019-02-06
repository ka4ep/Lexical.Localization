// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Asset.Ms.Extensions
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
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
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
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
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

    /// <summary>
    /// Knows how to build <see cref="AssetLoaderPartEmbeddedResources"/> and 
    /// <see cref="AssetLoaderPartFileResources"/>.
    /// </summary>
    internal class AssetLoaderPartBuilderParticipant : IAssetLoaderPartBuilderPart
    {
        /// <summary>
        /// Static instance
        /// </summary>
        static readonly AssetLoaderPartBuilderParticipant instance = new AssetLoaderPartBuilderParticipant();

        /// <summary>
        /// Static instance getter
        /// </summary>
        public static IAssetLoaderPartBuilderPart Instance => instance;

        /// <summary>
        /// Parameters that are used for configuring the builder part
        /// </summary>
        static string[] builderParameters = new string[] { AssetLoaderPartBuilderExtensions.KEY_FILEPROVIDER };

        /// <summary>
        /// Parameters that are used for configuring the builder part
        /// </summary>
        public string[] BuilderParameters => builderParameters;

        /// <summary>
        /// Check the parameters, and if they match, build new part.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>parts</returns>
        public IEnumerable<IAssetLoaderPart> TryBuild(AssetLoaderPartBuilder builder, IList<string> errors)
        {
            // Assert if "Resource" was requested.
            if (!builder.GetPartTypes().Contains(Lexical.Asset.AssetLoaderPartBuilderExtensions.PartType_Resource)) yield break;

            IList<IAssetNamePattern> filePatterns = builder.GetFilePatterns();
            IList<IFileProvider> fileProviders = builder.GetFileProviders();

            // Create part that can read file resources
            if (fileProviders != null && fileProviders.Count > 0)
            {
                // Assert "FileProvider"s
                if (filePatterns == null || filePatterns.Count == 0) { errors.Add($"Please add file name pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.FilePattern)}()."); yield break; }

                foreach (IAssetNamePattern pattern in filePatterns)
                {
                    IAssetLoaderPart part = new AssetLoaderPartFileResources(pattern);
                    builder.PopulateOptions(part.Options);
                    yield return part;
                }
            }
        }
    }
}
