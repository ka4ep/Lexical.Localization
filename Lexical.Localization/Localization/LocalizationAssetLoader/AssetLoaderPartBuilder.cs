// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lexical.Localization
{
    public static partial class AssetLoaderPartBuilderExtensions
    {
        /// <summary>
        /// List of <see cref="IAssetKeyNamePolicy"/>s.
        /// Value type is <see cref="List{IAssetKeyNamePolicy}"/>. 
        /// </summary>
        public const string KEY_KEYPOLICY = "KeyPattern";

        /// <summary>
        /// Add <see cref="IAssetKeyNamePolicy"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder KeyPolicy(this AssetLoaderPartBuilder partBuilder, IAssetKeyNamePolicy keyPattern)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.Add<IAssetKeyNamePolicy>(KEY_KEYPOLICY, keyPattern);
            return partBuilder;
        }

        /// <summary>
        /// Add range of <see cref="IAssetKeyNamePolicy"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPatterns"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder KeyPolicies(this AssetLoaderPartBuilder partBuilder, IEnumerable<IAssetKeyNamePolicy> keyPatterns)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.AddRange<IAssetKeyNamePolicy>(KEY_KEYPOLICY, keyPatterns);
            return partBuilder;
        }

        /// <summary>
        /// Add <see cref="IAssetKeyNamePolicy"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder KeyPattern(this AssetLoaderPartBuilder partBuilder, string keyPattern)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.Add<IAssetKeyNamePolicy>(KEY_KEYPOLICY, new AssetNamePattern(keyPattern));
            return partBuilder;
        }

        /// <summary>
        /// Add range of <see cref="IAssetNamePattern"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPatterns"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder KeyPatterns(this AssetLoaderPartBuilder partBuilder, IEnumerable<string> keyPatterns)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.AddRange<IAssetKeyNamePolicy>(KEY_KEYPOLICY, keyPatterns.Select(str => new AssetNamePattern(str)));
            return partBuilder;
        }

        /// <summary>
        /// Get <see cref="IAssetNamePattern"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>file providers or null</returns>
        public static IList<IAssetKeyNamePolicy> GetKeyPolicies(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<IAssetKeyNamePolicy>>(KEY_KEYPOLICY);

        /// <summary>
        /// List of <see cref="AssetFileConstructor"/>s.
        /// Value type is <see cref="List{AssetFileConstructor}"/>. 
        /// </summary>
        public const string KEY_ASSETFILECONSTRUCTOR = "AssetFileConstructor";

        /// <summary>
        /// Add <see cref="IFileProvider"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPatterns"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder AssetFileConstructor(this AssetLoaderPartBuilder partBuilder, AssetFileConstructor keyPatterns)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.Add<AssetFileConstructor>(KEY_ASSETFILECONSTRUCTOR, keyPatterns);
            return partBuilder;
        }

        /// <summary>
        /// Add range of <see cref="AssetFileConstructor"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="keyPatterns"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder AssetFileConstructor(this AssetLoaderPartBuilder partBuilder, IEnumerable<AssetFileConstructor> keyPatterns)
        {
            // Add build participant
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddBuilderPart(Lexical.Localization.Ms.Extensions.AssetLoaderLocalizationPartBuilderParticipant.Instance);
            partBuilder.AddRange<AssetFileConstructor>(KEY_ASSETFILECONSTRUCTOR, keyPatterns);
            return partBuilder;
        }

        /// <summary>
        /// Get <see cref="AssetFileConstructor"/>s.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>file providers or null</returns>
        public static IList<AssetFileConstructor> GetAssetFileConstructors(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<AssetFileConstructor>>(KEY_ASSETFILECONSTRUCTOR);


        /// <summary>
        /// Part type that implements <see cref="ILocalizationStringProvider"/>.
        /// </summary>
        public const string PartType_Strings = "Strings";

        /// <summary>
        /// Add signal that <paramref name="partBuilder"/> is to produce a part that implements <see cref="ILocalizationStringProvider"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>part builder</returns>
        public static AssetLoaderPartBuilder Strings(this AssetLoaderPartBuilder partBuilder)
        {
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddUnique<string>(Lexical.Asset.AssetLoaderPartBuilderExtensions.Key_PartTypes, PartType_Strings);
            return partBuilder;
        }

        /// <summary>
        /// Part type that implements <see cref="IAssetResourceProvider"/>.
        /// </summary>
        public const string PartType_ResourceManager = "ResourceManager";

        /// <summary>
        /// Add signal that <paramref name="partBuilder"/> is to produce a part that implements <see cref="ILocalizationStringProvider"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>part builder</returns>
        public static AssetLoaderPartBuilder ResourceManager(this AssetLoaderPartBuilder partBuilder)
        {
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddUnique<string>(Lexical.Asset.AssetLoaderPartBuilderExtensions.Key_PartTypes, PartType_ResourceManager);
            return partBuilder;
        }

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
        static string[] builderParameters = new string[] { AssetLoaderPartBuilderExtensions.KEY_KEYPOLICY, AssetLoaderPartBuilderExtensions.KEY_ASSETFILECONSTRUCTOR };

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
            if (builder.GetPartTypes().Contains(AssetLoaderPartBuilderExtensions.PartType_Strings))
            {
                IList<IAssetKeyNamePolicy> keyPolicies = builder.GetKeyPolicies();
                IList<AssetFileConstructor> keyConstructors = builder.GetAssetFileConstructors();
                IList<string> paths = builder.GetPaths();
                IList<Assembly> asms = builder.GetAssemblies();

                // Create part that can read file resources
                if (paths != null && paths.Count > 0)
                {
                    IList<IAssetNamePattern> filePatterns = builder.GetFilePatterns();
                    // Asset "FilePattern"
                    if (filePatterns == null || filePatterns.Count == 0) { errors.Add($"Please add file name pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.FilePattern)}()."); }

                    else
                    {
                        foreach (IAssetNamePattern filePattern in filePatterns)
                        {

                            if (keyPolicies != null)
                            {
                                foreach (IAssetKeyNamePolicy keyNamePolicy in keyPolicies)
                                {
                                    IAssetLoaderPart part = new AssetLoaderPartFileStrings(filePattern, keyNamePolicy);
                                    builder.PopulateOptions(part.Options);
                                    yield return part;
                                }
                            }

                            if (keyConstructors != null)
                            {
                                foreach (AssetFileConstructor keyConstructor in keyConstructors)
                                {
                                    IAssetLoaderPart part = new AssetLoaderPartFileStrings(filePattern, keyConstructor);
                                    builder.PopulateOptions(part.Options);
                                    yield return part;
                                }
                            }
                        }
                    }
                }

                // Create part that can read embedded resources
                if (asms != null && asms.Count > 0)
                {
                    IList<IAssetNamePattern> embeddedPatterns = builder.GetEmbeddedPatterns();
                    // Asset "EmbeddedPattern"
                    if (embeddedPatterns == null || embeddedPatterns.Count == 0) { errors.Add($"Please add embedded resource name pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.EmbeddedPattern)}()."); }

                    else
                    {
                        foreach (IAssetNamePattern embeddedPatern in embeddedPatterns)
                        {

                            if (keyPolicies != null)
                            {
                                foreach (IAssetKeyNamePolicy keyNamePolicy in keyPolicies)
                                {
                                    IAssetLoaderPart part = new AssetLoaderPartEmbeddedStrings(embeddedPatern, keyNamePolicy);
                                    builder.PopulateOptions(part.Options);
                                    yield return part;
                                }
                            }

                            if (keyConstructors != null)
                            {
                                foreach (AssetFileConstructor keyConstructor in keyConstructors)
                                {
                                    IAssetLoaderPart part = new AssetLoaderPartEmbeddedStrings(embeddedPatern, keyConstructor);
                                    builder.PopulateOptions(part.Options);
                                    yield return part;
                                }
                            }
                        }
                    }
                }
            }


            // Assert if "ResourceManager" was requested.
            if (builder.GetPartTypes().Contains(AssetLoaderPartBuilderExtensions.PartType_ResourceManager))
            {
                IList<IAssetKeyNamePolicy> keyPolicies = builder.GetKeyPolicies();
                IList<string> paths = builder.GetPaths();
                IList<Assembly> asms = builder.GetAssemblies();

                // Create part that can read file resources
                if (paths != null && paths.Count > 0)
                {
                    IList<IAssetNamePattern> filePatterns = builder.GetFilePatterns();

                    // Asset "FilePattern"
                    if (filePatterns == null || filePatterns.Count == 0) { errors.Add($"Please add file name pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.FilePattern)}()."); }

                    else
                    {
                        foreach (IAssetNamePattern filePattern in filePatterns)
                        {
                            if (keyPolicies != null)
                            {
                                foreach (IAssetKeyNamePolicy keyNamePolicy in keyPolicies)
                                {
                                    IAssetLoaderPart part = new AssetLoaderPartFileStrings(filePattern, keyNamePolicy);
                                    builder.PopulateOptions(part.Options);
                                    yield return part;
                                }
                            }
                        }
                    }
                }

                // Create part that can read embedded resources
                if (asms != null && asms.Count > 0)
                {
                    IList<IAssetNamePattern> embeddedPatterns = builder.GetEmbeddedPatterns();

                    // Asset "EmbeddedPattern"
                    if (embeddedPatterns == null || embeddedPatterns.Count == 0) { errors.Add($"Please add embedded resource pattern with .{nameof(Lexical.Asset.AssetLoaderPartBuilderExtensions.EmbeddedPattern)}()."); }
                    else
                    {
                        foreach (IAssetNamePattern embeddedPattern in embeddedPatterns)
                        {
                            foreach (IAssetKeyNamePolicy keyNamePolicy in keyPolicies)
                            {
                                IAssetLoaderPart part = new AssetLoaderPartEmbeddedStrings(embeddedPattern, keyNamePolicy);
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
