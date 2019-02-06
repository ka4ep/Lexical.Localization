﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Lexical.Asset.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Knows how to build <see cref="AssetLoaderPartEmbeddedResources"/> and 
    /// <see cref="AssetLoaderPartFileResources"/>.
    /// </summary>
    internal class AssetLoaderLocalizationPartBuilderParticipant : IAssetLoaderPartBuilderPart
    {
        /// <summary>
        /// Static instance
        /// </summary>
        static readonly AssetLoaderLocalizationPartBuilderParticipant instance = new AssetLoaderLocalizationPartBuilderParticipant();

        /// <summary>
        /// Static instance getter
        /// </summary>
        public static IAssetLoaderPartBuilderPart Instance => instance;

        /// <summary>
        /// Parameters that are used for configuring the builder part
        /// </summary>
        static string[] builderParameters = new string[] { Lexical.Asset.Ms.Extensions.AssetLoaderPartBuilderExtensions.KEY_FILEPROVIDER };

        /// <summary>
        /// Parameters that are used for configuring the builder part
        /// </summary>
        public string[] BuilderParameters => builderParameters;

        /// <summary>
        /// Check the parameters, and if they match, build new part.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="errors"></param>
        /// <returns>parts</returns>
        public IEnumerable<IAssetLoaderPart> TryBuild(AssetLoaderPartBuilder builder, IList<string> errors)
        {
            // Assert if "Strings" was requested.
            if (!builder.GetPartTypes().Contains(Lexical.Localization.AssetLoaderPartBuilderExtensions.PartType_Strings)) yield break;

            IList<IAssetNamePattern> filePatterns = builder.GetFilePatterns();
            IList<IAssetKeyNamePolicy> keyPatterns = builder.GetKeyPolicies();
            IList<IFileProvider> fileProviders = builder.GetFileProviders();
            IList<AssetFileConstructor> keyConstructors = builder.GetAssetFileConstructors();

            // Create part that can read file resources
            if (fileProviders != null && fileProviders.Count > 0)
            {
                // Assert "FileProvider"s
                if (filePatterns == null || filePatterns.Count == 0) { errors.Add($"Please add file name pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.FilePattern)}()."); yield break; }

                foreach (IFileProvider fileProvider in fileProviders)
                {
                    foreach (IAssetNamePattern filePattern in filePatterns)
                    {

                        if (keyPatterns != null)
                        {
                            foreach (IAssetKeyNamePolicy keyNamePolicy in keyPatterns)
                            {
                                IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, filePattern, keyNamePolicy);
                                builder.PopulateOptions(part.Options);
                                yield return part;
                            }
                        }

                        if (keyConstructors != null)
                        {
                            foreach (AssetFileConstructor keyConstructor in keyConstructors)
                            {
                                IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, filePattern, keyConstructor);
                                builder.PopulateOptions(part.Options);
                                yield return part;
                            }
                        }
                    }
                }
            }
        }
    }
}
