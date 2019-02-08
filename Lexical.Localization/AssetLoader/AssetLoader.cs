// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           23.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class load assets based on properties of keys.
    /// 
    ///  <see cref="AssetNamePattern"/> for pattern format.
    /// </summary>
    public class AssetLoader : IAssetLoader, IAssetReloadable
    {
        protected List<AssetLoaderPartCache> loaders = new List<AssetLoaderPartCache>();

        public IAssetLoaderPart[] LoaderParts => loaders.Select(l => l.loader).ToArray();

        public AssetLoader() { }

        public AssetLoader(IEnumerable<IAssetLoaderPart> initialLoaders)
        {
            AddRange(initialLoaders);
        }

        public AssetLoader(params IAssetLoaderPart[] initialLoaders)
        {
            AddRange(initialLoaders);
        }

        /// <summary>
        /// Add new loader function.
        /// </summary>
        /// <param name="assetLoader">Object that loads a assets based on the parameters, such as "culture"</param>
        /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
        /// <returns>this</returns>
        public IAssetLoader Add(IAssetLoaderPart assetLoader)
        {
            if (assetLoader == null) throw new ArgumentNullException(nameof(assetLoader));
            AssetLoaderPartCache cached = assetLoader is AssetLoaderPartCache cachedLoader ? cachedLoader : new AssetLoaderPartCache(assetLoader);
            loaders.Add(cached);
            return this;
        }

        /// <summary>
        /// Add loader functions.
        /// </summary>
        /// <param name="assetLoaders">(optional)list of loaders</param>
        /// <exception cref="ArgumentException">If there was a problem parsing the filename pattern</exception>
        /// <returns>this</returns>
        public IAssetLoader AddRange(IEnumerable<IAssetLoaderPart> assetLoaders)
        {
            if (assetLoaders == null) return this;
            foreach (var loader in assetLoaders) Add(loader);
            return this;
        }

        /// <summary>
        /// Get and load assets that matches the key criteria.
        /// 
        /// Matches, those missing parameters that are in loader part's options, against detected filenames.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="AssetException"></exception>
        public IEnumerable<IAsset> LoadAssets(IAssetKey key)
            => _loadAssets(key, true);


        /// <summary>
        /// Get and load assets that matches the key criteria.
        /// 
        /// Matches all missing parameters against detected filenames.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="AssetException"></exception>
        public IEnumerable<IAsset> LoadAllAssets(IAssetKey key)
            => _loadAssets(key, true);

        IEnumerable<IAsset> _loadAssets(IAssetKey key, bool matchAllMissingParameters)
        {
            foreach (var loaderPart in loaders)
            {
                // Match key to pattern
                IAssetNamePatternMatch match = loaderPart.Pattern.Match(key);

                // Get "MatchParametrs" option.
                IList<string> options_matchParameters = matchAllMissingParameters ? loaderPart.CapturePartNames : loaderPart.Options?.GetMatchParameters();

                // Count the number of parameters that need matching
                int matchable_parameters_count = 0;
                if (matchAllMissingParameters)
                {
                    matchable_parameters_count = loaderPart.CapturePartNames.Length;
                } else 
                {
                    foreach (var part in loaderPart.Pattern.CaptureParts)
                        if (match[part.CaptureIndex] == null && options_matchParameters.Contains(part.Identifier))
                            matchable_parameters_count++; 
                }

                // Key matched pattern
                if (match.Success & matchable_parameters_count == 0)
                {
                    IAsset asset = null;
                    try
                    {
                        asset = loaderPart.Load(match);
                    }
                    catch (Exception e)
                    {
                        throw new AssetException($"Failed to load {loaderPart.Pattern.BuildName(match)}, {e.GetType().Name}, {e.Message}", e);
                    }
                    if (asset != null)
                    {
                        yield return asset;
                        continue;
                    }
                }

                // New set of parameters
                Dictionary<string, string> new_params = new Dictionary<string, string>();
                // Iterate files
                foreach (IReadOnlyDictionary<string, string> file_params in loaderPart.ListLoadables(_hasValues(match) ? match : null)) // With match slower but better match, with null faster but more mistakes, especially with embedded resource names and '.' separators.
                {
                    // Start from new dictionary.
                    new_params.Clear();

                    // Iterate missing parts
                    bool ok = true;
                    foreach (IAssetNamePatternPart part in loaderPart.Pattern.CaptureParts)
                    {
                        // Get parameter from file
                        string file_matched_value = null;
                        bool file_matched = file_params.TryGetValue(part.Identifier, out file_matched_value);

                        // Get parameter from key
                        string key_matched_value = match[part.Identifier];

                        // Mismatch? Go to next file
                        if (key_matched_value != null && file_matched && key_matched_value != file_matched_value) { ok = false; break; }

                        // Write value from key
                        if (key_matched_value != null) { new_params[part.Identifier] = match[part.Identifier]; continue; }

                        // This parameter is not intended to be matched
                        if (!options_matchParameters.Contains(part.Identifier)) continue;

                        // Write parameter from filename
                        if (file_matched) new_params[part.Identifier] = file_matched_value;

                        // Could not find the parameter from file and it is required
                        else if (part.Required) { ok = false; break; }
                    }

                    // Go to next one
                    if (!ok) continue;

                    // Try loading asset and yield it. (cannot yield in try-catch)
                    IAsset asset = null;
                    try
                    {
                        asset = loaderPart.Load(new_params);
                    }
                    catch (Exception e)
                    {
                        throw new AssetException($"Failed to load {loaderPart.Pattern.BuildName(match)}, {e.GetType().Name}, {e.Message}", e);
                    }
                    if (asset != null) yield return asset;
                }
            }
        }

        /// <summary>
        /// Tests if dictionary has values.
        /// </summary>
        /// <param name="keyvalues"></param>
        /// <returns>true if there are no value instances</returns>
        static private bool _hasValues(IReadOnlyDictionary<string, string> keyvalues)
        {
            if (keyvalues == null || keyvalues.Count == 0) return false;
            foreach (var kp in keyvalues)
                if (kp.Value != null) return true;
            return false;
        }

        /// <summary>
        /// Find assets that match key,culture criteria. The less there is criteria, more assets are located.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>found assets</returns>
        /// <exception cref="AssetException"></exception>
        protected IEnumerable<IAsset> FindAssets(IAssetKey key)
        {
            // Load all files
            Dictionary<string, string> param = null;
            foreach (var loaderPart in loaders)
            {
                IReadOnlyDictionary<string, string> initialMatch = loaderPart.Pattern.Match(key);
                IEnumerable<IReadOnlyDictionary<string, string>> files = null;
                try
                {
                    files = loaderPart.ListLoadables(initialMatch);
                }
                catch (Exception e)
                {
                    throw new AssetException($"Failed to list filenames, {e.GetType().Name}, {e.Message}", e);
                }
                if (files == null) continue;

                var options = loaderPart.Options;
                IList<string> options_matchParameters = options.GetMatchParameters();

                foreach (var file_params in files)
                {
                    if (param == null) param = new Dictionary<string, string>(); else param.Clear();
                    bool mismatch = false;
                    foreach (var part in loaderPart.Pattern.CaptureParts)
                    {
                        // Test if part is found in filename
                        string part_from_filename = null;
                        file_params.TryGetValue(part.Identifier, out part_from_filename);

                        // Part is required component.
                        if (part_from_filename == null && part.Required) { mismatch = true; break; }

                        // Part from filename
                        param[part.Identifier] = part_from_filename;

                        // Read same value from key
                        string part_from_key = AssetKeyParametrizer.Singleton.ReadParameter(key, part.ParameterName);

                        // Test if key matches filename
                        if (part.Required && part_from_key != part_from_filename) {

                            // Try matching parameter from existing files
                            if (part_from_key == null && part_from_filename != null && options_matchParameters != null && options_matchParameters.Contains(part.Identifier))
                            {
                            }
                            else
                            {
                                mismatch = true;
                                break;
                            }
                        }

                        // Test if they mismatch even if not required
                        if (!part.Required && part_from_key != null && part_from_filename != null && part_from_key != part_from_filename) { mismatch = true; break; }
                    }
                    if (mismatch) continue;

                    IAsset asset = null;
                    try
                    {
                        asset = loaderPart.Load(param);
                    }
                    catch (Exception e)
                    {
                        throw new AssetException($"Failed to load {file_params}, {e.GetType().Name}, {e.Message}", e);
                    }

                    if (asset == null) continue;
                    yield return asset;
                }
            }
        }
        
        public IAsset Reload()
        {
            foreach (var loader in loaders)
                loader.ClearCache();
            return this;
        }

        public override string ToString()
            => $"{GetType().Name}({string.Join(", ", loaders)})";
    }

    /// <summary>
    /// Source that creates new <see cref="LocalizationAssetLoader"/>s to <see cref="IAssetBuilder"/>.
    /// </summary>
    public class AssetLoaderSource : IAssetSource
    {
        Action<IAssetLoader> configurer;

        public AssetLoaderSource(Action<IAssetLoader> configurer)
        {
            this.configurer = configurer;
        }

        public void Build(IList<IAsset> list)
        {
            IAssetLoader loader = new AssetLoader();
            if (configurer != null) configurer(loader);
            list.Add(loader);
        }

        public IAsset PostBuild(IAsset asset)
            => asset;
    }

    public static partial class AssetExtensions_
    {
        /// <summary>
        /// Add <see cref="IAssetLoader"/> to builder, resoponds to <see cref="IAssetResourceProvider"/> and <see cref="IAssetResourceCollection"></see>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurer"></param>
        /// <returns>builder</returns>
        public static IAssetBuilder AddAssetLoader(this IAssetBuilder builder, Action<IAssetLoader> configurer = null)
        {
            builder.Sources.Add(new AssetLoaderSource(configurer));
            return builder;
        }
    }
}
