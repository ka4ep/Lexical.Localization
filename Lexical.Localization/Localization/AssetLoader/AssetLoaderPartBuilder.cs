// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lexical.Asset
{
    /// <summary>
    /// <see cref="AssetLoaderPartBuilder"/> is a helper that constructs <see cref="IAssetLoaderPart"/>s.
    /// Has consistent way to add parameters, and lets the builder to choose the correct class for parts.
    /// 
    /// Example:
    /// <code>
    /// IAsset assetLoader = new AssetLoader()
    ///     .NewPart().AddFiles("{culture}/{location}/{key}.png").AddPath(".").AsResource().EndPart()
    /// </code>
    /// 
    /// </summary>
    public class AssetLoaderPartBuilder : AssetLoaderPartOptions
    {
        /// <summary>
        /// Builder parts.
        /// </summary>
        List<IAssetLoaderPartBuilderPart> builderParts = new List<IAssetLoaderPartBuilderPart>();

        /// <summary>
        /// Add builder part.
        /// Extension methods add their library specific parts to the builder. 
        /// When the builder is issued to build, these parts participate in the build process.
        /// </summary>
        /// <param name="builderPart"></param>
        /// <returns>builder</returns>
        public AssetLoaderPartBuilder AddBuilderPart(IAssetLoaderPartBuilderPart builderPart)
        {
            if (!builderParts.Contains(builderPart))
                builderParts.Add(builderPart);
            return this;
        }

        /// <summary>
        /// Create new <see cref="AssetLoaderPartOptions"/> that can be added to a new <see cref="IAssetLoaderPart"/>.
        /// 
        /// The returned options will be added all the key-values that this builder has, except those that 
        /// are used for configuring the <see cref="AssetLoaderPartBuilder" />. 
        /// </summary>
        /// <returns>configured options</returns>
        public AssetLoaderPartOptions PopulateOptions(IDictionary<string, object> options)
        {
            // Add options, some.
            foreach (var kp in options)
            {
                // Don't add "IAssetLoader" key
                if (kp.Key == nameof(IAssetLoader)) continue;

                // Don't add parameters that part builder use for configuring them.
                bool ok = true;
                foreach (IAssetLoaderPartBuilderPart builderPart in builderParts)
                    if (!(ok = builderPart.BuilderParameters.Contains(kp.Key))) break;
                if (!ok) continue;

                // Add option
                options[kp.Key] = kp.Value;
            }
            return this;
        }

        /// <summary>
        /// Build configuration into one or more parts.
        /// 
        /// After construction the builder must be cleared to start new parts.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AssetException">If part could not be built from the builder configuration</exception>
        public IEnumerable<IAssetLoaderPart> Build()
        {
            IEnumerable<IAssetLoaderPart> result = null;
            List<string> errors = new List<string>();
            foreach (IAssetLoaderPartBuilderPart builderPart in builderParts)
            {
                IEnumerable<IAssetLoaderPart> builtParts = builderPart.TryBuild(this, errors);
                if (builtParts != null) result = result == null ? builtParts : result.Concat(builtParts);
            }

            if (result == null) throw new AssetException($"Could not build {nameof(IAssetLoaderPart)}. "+string.Join(" ", errors));

            return result;
        }

    }

    /// <summary>
    /// Interface for classes that participate in building part(s).
    /// 
    /// </summary>
    public interface IAssetLoaderPartBuilderPart
    {
        /// <summary>
        /// Parameter names that this part utilizes.
        /// </summary>
        String[] BuilderParameters { get; }

        /// <summary>
        /// Triers to build parts based on parameters of the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="errors">location to place configuration errors</param>
        /// <returns>built parts, or null</returns>
        IEnumerable<IAssetLoaderPart> TryBuild(AssetLoaderPartBuilder builder, IList<string> errors);
    }

    public static partial class AssetLoaderPartBuilderExtensions
    {
        /// <summary>
        /// Start building new part to the <paramref name="assetLoader"/>.
        /// </summary>
        /// <param name="assetLoader"></param>
        /// <returns>part builder</returns>
        public static AssetLoaderPartBuilder NewPart(this IAssetLoader assetLoader)
        {
            if (assetLoader == null) throw new ArgumentNullException(nameof(assetLoader));
            AssetLoaderPartBuilder builder = new AssetLoaderPartBuilder();
            builder[nameof(IAssetLoader)] = assetLoader;
            return builder;
        }

        /// <summary>
        /// End building part(s), adds to <see cref="IAssetLoader"/>, and then returns the <see cref="IAssetLoader"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="errors">list or errors</param>
        /// <returns>asset loader with new part(s)</returns>
        /// <exception cref="AssetException">If part could not be built from the builder configuration</exception>
        public static IAssetLoader End(this AssetLoaderPartBuilder builder)
        {
            // Assert arg
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            // Read IAssetLoader
            IAssetLoader assetLoader;
            Object o;
            if (builder.TryGetValue(nameof(IAssetLoader), out o) && o is IAssetLoader loader) assetLoader = loader;
            else throw new InvalidOperationException($"To {nameof(End)}(), please start it with {nameof(IAssetLoader)}.{nameof(NewPart)}().");

            // Add parts
            assetLoader.AddRange(builder.Build());

            // Return
            return assetLoader;
        }

        /// <summary>
        /// File name pattern, for example "{culture}/{location}/{key}.png".
        /// 
        /// Added with <see cref="FilePattern(AssetLoaderPartBuilder, string)"/>.
        /// 
        /// Value type is <see cref="List{IAssetNamePattern}"/>. 
        /// </summary>
        public const string KEY_FILEPATTERN = "FilePattern";

        /// <summary>
        /// Add filename pattern, for example "{culture}/{location}/{key}.png".
        /// 
        /// Part will be built to search files from local files and matching files to the <paramref name="namePattern"/>.
        /// 
        /// Path must also be added with <see cref="AssetLoaderPartExtensions.AddPaths(AssetLoaderPartOptions, IEnumerable{string})"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="namePattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder FilePattern(this AssetLoaderPartBuilder partBuilder, string namePattern)
            => partBuilder.Add<IAssetNamePattern>(KEY_FILEPATTERN, new AssetNamePattern(namePattern)) as AssetLoaderPartBuilder;

        /// <summary>
        /// Set filename pattern, for example "{culture}/{location}/{key}.png".
        /// 
        /// Part will be built to search files from local files and matching files to the <paramref name="namePattern"/>.
        /// 
        /// Path must also be added with <see cref="AssetLoaderPartExtensions.AddPaths(AssetLoaderPartOptions, IEnumerable{string})"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="namePattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder FilePattern(this AssetLoaderPartBuilder partBuilder, IAssetNamePattern namePattern)
            => partBuilder.Add<IAssetNamePattern>(KEY_FILEPATTERN, namePattern) as AssetLoaderPartBuilder;

        /// <summary>
        /// Get file patterns.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>file patterns or null</returns>
        public static IList<IAssetNamePattern> GetFilePatterns(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<IAssetNamePattern>>(KEY_FILEPATTERN);

        /// <summary>
        /// Embedded resource pattern, for example "{assembly.}{resource.}{section.}{key}{-culture}.json".
        /// 
        /// Added with <see cref="EmbeddedPattern(AssetLoaderPartBuilder, string)"/>.
        /// 
        /// Value type is <see cref="List{IAssetNamePattern}"/>. 
        /// </summary>
        public const string KEY_EMBEDDEDPATTERN = "EmbeddedPattern";

        /// <summary>
        /// Add embedded resources pattern, for example "{assembly.}{resource.}{section.}{key}{-culture}.json".
        /// 
        /// Part will be built to search for embedded resources.
        /// 
        /// Source assemblies must also be added with <see cref="AssetLoaderPartExtensions.Assembly(AssetLoaderPartOptions, System.Reflection.Assembly)"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="namePattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder EmbeddedPattern(this AssetLoaderPartBuilder partBuilder, string namePattern)
            => partBuilder.Add<IAssetNamePattern>(KEY_EMBEDDEDPATTERN, new AssetNamePattern(namePattern)) as AssetLoaderPartBuilder;

        /// <summary>
        /// Add embedded resources pattern, for example "{assembly.}{resource.}{section.}{key}{-culture}.json".
        /// 
        /// Part will be built to search embedded resources.
        /// 
        /// Source assemblies must also be added with <see cref="AssetLoaderPartExtensions.Assembly(AssetLoaderPartOptions, System.Reflection.Assembly)"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <param name="namePattern"></param>
        /// <returns>partBuilder</returns>
        public static AssetLoaderPartBuilder EmbeddedPattern(this AssetLoaderPartBuilder partBuilder, IAssetNamePattern namePattern)
            => partBuilder.Add<IAssetNamePattern>(KEY_EMBEDDEDPATTERN, namePattern) as AssetLoaderPartBuilder;

        /// <summary>
        /// Get embedded patterns.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>embedded patterns or null</returns>
        public static IList<IAssetNamePattern> GetEmbeddedPatterns(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<IAssetNamePattern>>(KEY_EMBEDDEDPATTERN);

        /// <summary>
        /// This options determines part types: "Strings", "Resource", "ResourceManager"
        /// </summary>
        public const string Key_PartTypes = "PartTypes";

        /// <summary>
        /// Part type that implements <see cref="IAssetResourceProvider"/>.
        /// </summary>
        public const string PartType_Resource = "Resource";

        /// <summary>
        /// Add signal that <paramref name="partBuilder"/> is to produce a part that implements <see cref="IAssetResourceProvider"/>.
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns>part builder</returns>
        public static AssetLoaderPartBuilder Resource(this AssetLoaderPartBuilder partBuilder)
        {
            partBuilder.AddBuilderPart(AssetLoaderPartBuilderParticipant.Instance);
            partBuilder.AddUnique<string>(Key_PartTypes, PartType_Resource);
            return partBuilder;
        }

        /// <summary>
        /// Get part types that are expected to be built, such as "Resource"
        /// </summary>
        /// <param name="partBuilder"></param>
        /// <returns></returns>
        public static IList<string> GetPartTypes(this AssetLoaderPartBuilder partBuilder)
            => partBuilder.Get<List<string>>(Key_PartTypes);

        /// <summary>
        /// Append match parameters to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="parameter"></param>
        public static AssetLoaderPartBuilder MatchParameter(this AssetLoaderPartBuilder options, string parameter)
            => options.Add<string>(AssetLoaderPartOptionsExtensions.Key_MatchParameters, parameter) as AssetLoaderPartBuilder;

        /// <summary>
        /// Append match parameters to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="parameters"></param>
        public static AssetLoaderPartBuilder MatchParameters(this AssetLoaderPartBuilder options, params string[] parameters)
            => options.AddRange<string>(AssetLoaderPartOptionsExtensions.Key_MatchParameters, parameters) as AssetLoaderPartBuilder;

        /// <summary>
        /// Append new path to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="path"></param>
        public static AssetLoaderPartBuilder Path(this AssetLoaderPartBuilder options, string path)
            => options.Add<string>(AssetLoaderPartOptionsExtensions.Key_Paths, path) as AssetLoaderPartBuilder;

        /// <summary>
        /// Append new paths to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="paths"></param>
        public static AssetLoaderPartBuilder Paths(this AssetLoaderPartBuilder options, params string[] paths)
            => options.AddRange<string>(AssetLoaderPartOptionsExtensions.Key_Paths, paths) as AssetLoaderPartBuilder;

        /// <summary>
        /// Append new paths to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="paths"></param>
        public static AssetLoaderPartBuilder Paths(this AssetLoaderPartBuilder options, IEnumerable<string> paths)
            => options.AddRange<string>(AssetLoaderPartOptionsExtensions.Key_Paths, paths) as AssetLoaderPartBuilder;

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembly"></param>
        public static AssetLoaderPartBuilder Assembly(this AssetLoaderPartBuilder options, Assembly assembly)
            => options.Add<Assembly>(AssetLoaderPartOptionsExtensions.Key_Assemblies, assembly) as AssetLoaderPartBuilder;

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblys"></param>
        public static AssetLoaderPartBuilder Assemblies(this AssetLoaderPartBuilder options, params Assembly[] assemblys)
            => options.AddRange<Assembly>(AssetLoaderPartOptionsExtensions.Key_Assemblies, assemblys) as AssetLoaderPartBuilder;

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblys"></param>
        public static AssetLoaderPartBuilder Assemblies(this AssetLoaderPartBuilder options, IEnumerable<Assembly> assemblys)
            => options.AddRange<Assembly>(AssetLoaderPartOptionsExtensions.Key_Assemblies, assemblys) as AssetLoaderPartBuilder;

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
        static string[] builderParameters = new string[] { AssetLoaderPartBuilderExtensions.KEY_FILEPATTERN };

        /// <summary>
        /// Parameters that are used for configuring the builder part
        /// </summary>
        public string[] BuilderParameters => builderParameters;

        /// <summary>
        /// Check the parameters, and if they match, build new part.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>instances of <see cref="AssetLoaderPartFileResources"/> or <see cref="AssetLoaderPartEmbeddedResources"/></returns>
        public IEnumerable<IAssetLoaderPart> TryBuild(AssetLoaderPartBuilder builder, IList<string> errors)
        {
            // Assert if "Resource" was requested.
            if (!builder.GetPartTypes().Contains(AssetLoaderPartBuilderExtensions.PartType_Resource)) yield break;

            IList<string> paths = builder.GetPaths();
            IList<Assembly> asms = builder.GetAssemblies();

            // Create part that can read file resources
            if (paths != null && paths.Count>0)
            {
                IList<IAssetNamePattern> filePatterns = builder.GetFilePatterns();
                // Asset "FilePattern"
                if (filePatterns == null || filePatterns.Count == 0) { errors.Add($"Please add file name pattern with .{nameof(AssetLoaderPartBuilderExtensions.FilePattern)}()."); yield break; }

                foreach(IAssetNamePattern filePattern in filePatterns)
                {
                    IAssetLoaderPart part = new AssetLoaderPartFileResources(filePattern);
                    builder.PopulateOptions(part.Options);
                    yield return part;
                }
            }

            // Create part that can read embedded resources
            if (asms != null && asms.Count > 0)
            {
                IList<IAssetNamePattern> embeddedPatterns = builder.GetEmbeddedPatterns();
                // Asset "EmbeddedPattern"
                if (embeddedPatterns == null || embeddedPatterns.Count == 0) { errors.Add($"Please add embedded resource pattern with .{nameof(AssetLoaderPartBuilderExtensions.EmbeddedPattern)}()."); yield break; }

                foreach (IAssetNamePattern embeddedPattern in embeddedPatterns)
                {
                    IAssetLoaderPart part = new AssetLoaderPartEmbeddedResources(embeddedPattern);
                    builder.PopulateOptions(part.Options);
                    yield return part;
                }
            }
        }
    }
}
