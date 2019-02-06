using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Options of <see cref="IAssetLoaderPart"/> instance.
    /// </summary>
    public class AssetLoaderPartOptions : Dictionary<string, object>, IAssetLoaderPartOptions
    {
        /// <summary>
        /// Print options
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({String.Join(", ", this.Select(kp => $"{kp.Key}={kp.Value}"))})";
    }

    public static partial class AssetLoaderPartOptionsExtensions_
    {
        /* // Feature is not yet implemented
        /// <summary>
        /// This options determines which assemblies to use when searching for embedded resources.
        /// Value type for this option is <see cref="List{T}"/> of <see cref="IAssetNamePattern"/>s.
        /// </summary>
        public const string Key_Assembly_FilePatterns = "AssemblyFilePatterns";

        /// <summary>
        /// Append new assembly to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembyFilePattern"></param>
        public static IAssetLoaderPartOptions AssemblyFilePattern(this IAssetLoaderPartOptions options, string assembyFilePattern)
            => options.Add<IAssetNamePattern>(Key_Assembly_FilePatterns, new AssetNamePattern(assembyFilePattern));

        /// <summary>
        /// Append new assembly to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembyFilePattern"></param>
        public static IAssetLoaderPartOptions AssemblyFilePattern(this IAssetLoaderPartOptions options, IAssetNamePattern assembyFilePattern)
            => options.Add<IAssetNamePattern>(Key_Assembly_FilePatterns, assembyFilePattern);

        /// <summary>
        /// Append new assemblys to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembyFilePatterns"></param>
        public static IAssetLoaderPartOptions AssemblyFilePatterns(this IAssetLoaderPartOptions options, params string[] assembyFilePatterns)
            => options.AddRange<IAssetNamePattern>(Key_Assembly_FilePatterns, assembyFilePatterns.Select(str=>new AssetNamePattern(str)));

        /// <summary>
        /// Append new assemblys to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembyFilePatterns"></param>
        public static IAssetLoaderPartOptions AddAssemblyFilePatterns(this IAssetLoaderPartOptions options, params IAssetNamePattern[] assembyFilePatterns)
            => options.AddRange<IAssetNamePattern>(Key_Assembly_FilePatterns, assembyFilePatterns);

        /// <summary>
        /// Get Assemblies
        /// </summary>
        /// <param name="options"></param>
        public static IList<IAssetNamePattern> GetAssemblyFilePatterns(this IAssetLoaderPartOptions options)
            => options.Get<List<IAssetNamePattern>>(Key_Assembly_FilePatterns);
         */
    }
}
