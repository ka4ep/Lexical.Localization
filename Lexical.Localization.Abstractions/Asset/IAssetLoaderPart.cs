// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lexical.Localization
{
    #region interface
    /// <summary>
    /// Interface for objects that load assets from IAssetLoader depending on parameters of a <see cref="ILinePattern"/>.
    /// This interface is used with <see cref="IAssetLoader"/>.
    /// 
    /// For example, localization files are separated by culture, then file pattern could be "localization{-culture}.ini".
    /// Then this loader can load different files depending on culture value.
    /// </summary>
    public interface IAssetLoaderPart
    {
        /// <summary>
        /// Filename pattern of this loader. For example "Resources/localization{-culture}.ini".
        /// </summary>
        ILinePattern Pattern { get; }

        /// <summary>
        /// Options of this loader.
        /// </summary>
        IAssetLoaderPartOptions Options { get; set; }

        /// <summary>
        /// Load an asset file. 
        /// 
        /// <paramref name="parameters"/> is a list of arguments are used for constructing filename.
        /// Parameters match the capture parts of the associated <see cref="Pattern"/> property.
        /// 
        /// If Options.MatchParameters has parameters, this method does not try to match existing files. 
        /// Instead, the caller must find suitable matches with ListLoadables.
        /// 
        /// The callee musn't take ownership of <paramref name="parameters"/>, as the caller modify the contents.
        /// </summary>
        /// <param name="parameters">Parameters that are extracted from filename using the pattern</param>
        /// <returns>loaded asset, or null if file was not found</returns>
        /// <exception cref="Exception">on problem loading asset</exception>
        IAsset Load(IReadOnlyDictionary<string, string> parameters);

        /// <summary>
        /// Get a list loadable assets of in parametrized format.
        /// Parameters correspond to capture parts of the associated <see cref="Pattern"/> property.
        /// </summary>
        /// <returns>loadables</returns>
        /// <param name="parameters"></param>
        /// <exception cref="Exception">on problem enumerating files</exception>
        IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> parameters = null);
    }
    #endregion interface

    /// <summary></summary>
    public static partial class IAssetLoaderPartExtensions
    {
        /// <summary>
        /// Set options, for chain calls.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="options">new options</param>
        /// <returns>part</returns>
        public static IAssetLoaderPart SetOptions(this IAssetLoaderPart part, IAssetLoaderPartOptions options)
        {
            part.Options = options;
            return part;
        }

        /// <summary>
        /// Configure options, for chain calls.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="configurer">delegeate that modifies options</param>
        /// <returns>part</returns>
        public static IAssetLoaderPart ConfigureOptions(this IAssetLoaderPart part, Action<IAssetLoaderPartOptions> configurer)
        {
            configurer(part.Options);
            return part;
        }

        /// <summary>
        /// Append new parameters to match parameters property.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="parameters"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddMatchParameters(this IAssetLoaderPart part, params string[] parameters)
        {
            part.Options.AddMatchParameters(parameters);
            return part;
        }

        /// <summary>
        /// Append new assemblies to search embedded resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="assembly"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddAssembly(this IAssetLoaderPart part, Assembly assembly)
        {
            part.Options.AddAssemblies(assembly);
            return part;
        }

        /// <summary>
        /// Append new assemblies to search embedded resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="assemblies"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddAssemblies(this IAssetLoaderPart part, params Assembly[] assemblies)
        {
            part.Options.AddAssemblies(assemblies);
            return part;
        }

        /// <summary>
        /// Append new assemblies to search embedded resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="assemblies"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddAssemblies(this IAssetLoaderPart part, IEnumerable<Assembly> assemblies)
        {
            part.Options.AddAssemblies(assemblies);
            return part;
        }

        /// <summary>
        /// Append new base path(s) top search file resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="path"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddPath(this IAssetLoaderPart part, String path)
        {
            part.Options.AddPath(path);
            return part;
        }

        /// <summary>
        /// Append new base path(s) top search file resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="paths"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddPaths(this IAssetLoaderPart part, params String[] paths)
        {
            part.Options.AddPaths(paths);
            return part;
        }

        /// <summary>
        /// Append new base path(s) top search file resources from.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="paths"></param>
        /// <returns>part</returns>
        public static IAssetLoaderPart AddPaths(this IAssetLoaderPart part, IEnumerable<String> paths)
        {
            part.Options.AddPaths(paths);
            return part;
        }

        /// <summary>
        /// List filenames
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static IEnumerable<string> ListFiles(this IAssetLoaderPart part)
            => part.ListLoadables().Select(param => part.Pattern.Print(param));

        /// <summary>
        /// Load an asset file by filename.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="filename"></param>
        /// <returns>loaded asset, or null if file was not found</returns>
        /// <exception cref="Exception">on problem loading asset</exception>
        public static IAsset Load(this IAssetLoaderPart part, string filename)
        {
            var match = part.Pattern.Match(filename);
            if (!match.Success) return null;
            return part.Load(match);
        }

    }
}
