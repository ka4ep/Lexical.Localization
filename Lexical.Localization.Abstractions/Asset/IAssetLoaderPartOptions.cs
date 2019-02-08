// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           29.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lexical.Localization
{
    #region interface
    /// <summary>
    /// Options of <see cref="IAssetLoaderPart"/> instance.
    /// </summary>
    public interface IAssetLoaderPartOptions : IDictionary<string, object>
    {
    }
    #endregion interface

    public static partial class AssetLoaderPartOptionsExtensions
    {
        /// <summary>
        /// This option determines whether to fill missing parameters of a key from filenames of existing files.
        /// 
        /// For example, if pattern is "[assembly.][resource.]localization{-culture}.ini", and
        /// the requested <see cref="IAssetKey"/> does not have the required parameters "assembly" and "resource",
        /// then if this option is configured with those parameters, all the files that match the criteria are searched.
        /// 
        /// For example, if there is a file "MyAssembly.Resources.localization-en.ini", and <see cref="AddMatchParameters"/>
        /// contains "assembly" and "resource" then "assembly"="MyAssembly." and "resource"="Resources." are matched for 
        /// the key.
        /// 
        /// Use this option, only for files that contain generic values.
        /// </summary>
        public const string Key_MatchParameters = "MatchParameters";

        /// <summary>
        /// This options determines which assemblies to use when searching for embedded resources.
        /// Value type for this option is <see cref="List{T}"/> of <see cref="System.Reflection.Assembly"/>s.
        /// </summary>
        public const string Key_Assemblies = "Assemblies";

        /// <summary>
        /// This options determines which base paths to use when searching for file resources.
        /// </summary>
        public const string Key_Paths = "Paths";

        /// <summary>
        /// Get a parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IAssetLoaderPartOptions options, string key)
        {
            object value;
            return options.TryGetValue(key, out value) ? (T)value : default;
        }

        /// <summary>
        /// Add element to a List of <paramref name="element"/>s.
        /// </summary>
        /// <typeparam name="Element"></typeparam>
        /// <param name="options"></param>
        /// <param name="key"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IAssetLoaderPartOptions Add<Element>(this IAssetLoaderPartOptions options, string key, Element element)
        {
            List<Element> list;
            object o;
            if (options.TryGetValue(key, out o) && o is List<Element> l) list = l;
            else options[key] = list = new List<Element>();
            list.Add(element);
            return options;
        }

        /// <summary>
        /// Add element to a List of <paramref name="element"/>s, if it has not already added.
        /// </summary>
        /// <typeparam name="Element"></typeparam>
        /// <param name="options"></param>
        /// <param name="key"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IAssetLoaderPartOptions AddUnique<Element>(this IAssetLoaderPartOptions options, string key, Element element)
        {
            List<Element> list;
            object o;
            if (options.TryGetValue(key, out o) && o is List<Element> l) list = l;
            else options[key] = list = new List<Element>();
            if (!list.Contains(element)) list.Add(element);
            return options;
        }

        /// <summary>
        /// Add range of elements to a List of <paramref name="elements"/>.
        /// </summary>
        /// <typeparam name="Element"></typeparam>
        /// <param name="options"></param>
        /// <param name="key"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IAssetLoaderPartOptions AddRange<Element>(this IAssetLoaderPartOptions options, string key, IEnumerable<Element> elements)
        {
            List<Element> list;
            object o;
            if (options.TryGetValue(key, out o) && o is List<Element> l) list = l;
            else options[key] = list = new List<Element>();
            list.AddRange(elements);
            return options;
        }

        /// <summary>
        /// Set a parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IAssetLoaderPartOptions Set<T>(this IAssetLoaderPartOptions options, string key, T value)
        {
            options[key] = value;
            return options;
        }

        /// <summary>
        /// Append match parameters to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="parameter"></param>
        public static IAssetLoaderPartOptions AddMatchParameter(this IAssetLoaderPartOptions options, string parameter)
            => options.Add<string>(Key_MatchParameters, parameter);

        /// <summary>
        /// Append match parameters to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="parameters"></param>
        public static IAssetLoaderPartOptions AddMatchParameters(this IAssetLoaderPartOptions options, params string[] parameters)
            => options.AddRange<string>(Key_MatchParameters, parameters);

        /// <summary>
        /// Get match parameters.
        /// </summary>
        /// <param name="options"></param>
        public static IList<String> GetMatchParameters(this IAssetLoaderPartOptions options)
            => options.Get<List<string>>(Key_MatchParameters);

        /// <summary>
        /// Append new path to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="path"></param>
        public static IAssetLoaderPartOptions AddPath(this IAssetLoaderPartOptions options, string path)
            => options.Add<string>(Key_Paths, path);

        /// <summary>
        /// Append new paths to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="paths"></param>
        public static IAssetLoaderPartOptions AddPaths(this IAssetLoaderPartOptions options, params string[] paths)
            => options.AddRange<string>(Key_Paths, paths);

        /// <summary>
        /// Append new paths to options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="paths"></param>
        public static IAssetLoaderPartOptions AddPaths(this IAssetLoaderPartOptions options, IEnumerable<string> paths)
            => options.AddRange<string>(Key_Paths, paths);

        /// <summary>
        /// Get paths.
        /// </summary>
        /// <param name="options"></param>
        public static IList<String> GetPaths(this IAssetLoaderPartOptions options)
            => options.Get<List<string>>(Key_Paths);

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assembly"></param>
        public static IAssetLoaderPartOptions AddAssembly(this IAssetLoaderPartOptions options, Assembly assembly)
            => options.Add<Assembly>(Key_Assemblies, assembly);

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblys"></param>
        public static IAssetLoaderPartOptions AddAssemblies(this IAssetLoaderPartOptions options, params Assembly[] assemblys)
            => options.AddRange<Assembly>(Key_Assemblies, assemblys);

        /// <summary>
        /// Add assembly to search for embedded resources.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblys"></param>
        public static IAssetLoaderPartOptions AddAssemblies(this IAssetLoaderPartOptions options, IEnumerable<Assembly> assemblys)
            => options.AddRange<Assembly>(Key_Assemblies, assemblys);

        /// <summary>
        /// Get Assemblies
        /// </summary>
        /// <param name="options"></param>
        public static IList<Assembly> GetAssemblies(this IAssetLoaderPartOptions options)
            => options.Get<List<Assembly>>(Key_Assemblies);

    }


}
