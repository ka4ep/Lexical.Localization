// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Adapts <see cref="IConfiguration"/> to <see cref="IAsset"/>.
    /// </summary>
    public class ConfigurationLocalizationAsset : ILocalizationStringProvider, ILocalizationStringCollection, IAssetReloadable, ILocalizationAssetCultureCapabilities, IDisposable
    {
        /// <summary>
        /// Use this name policy when culture is at the root of the configuration.
        /// For examples: .ini
        /// [en]
        /// Section:Section:key = value
        /// </summary>
        public static IAssetKeyNamePolicy CULTURE_ROOT = new AssetKeyNameProvider().SetDefault(true, ":", "");

        public readonly IConfiguration configuration;
        IDisposable observerHandle;
        IAssetKeyNamePolicy namePolicy;
        IAssetKeyParametrizer parametrizer;

        public ConfigurationLocalizationAsset(IConfiguration configuration, IAssetKeyNamePolicy namePolicy = default, IAssetKeyParametrizer parametrizer = default)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
            this.parametrizer = parametrizer ?? AssetKeyParametrizer.Singleton;
            this.observerHandle = this.configuration.GetReloadToken()?.RegisterChangeCallback(ConfigurationChanged, this);
        }

        void ConfigurationChanged(object state)
        {
            ClearCache();
        }

        public void Dispose()
        {
            // Swap and dispose
            IDisposable handle = null;
            Interlocked.Exchange(ref observerHandle, handle);
            handle?.Dispose();
        }

        IEnumerable<KeyValuePair<string, string>> ListStrings(IConfiguration c)
        {
            LinkedList<IConfiguration> queue = new LinkedList<IConfiguration>();
            queue.AddLast(c);
            while (queue.Count > 0)
            {
                IConfiguration item = queue.First.Value;
                queue.RemoveFirst();

                // Add children
                foreach (IConfigurationSection child in item.GetChildren()) queue.AddLast(child);

                if (item is IConfigurationSection section && section.Value != null)
                {
                    // "en:ConsoleApp1:MyController.Success"
                    string path = section.Path;

                    yield return new KeyValuePair<string, string>(section.Path, section.Value);
                }
            }
        }

        KeyValuePair<string, string>[] empty = new KeyValuePair<string, string>[0];
        public IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key = null)
        {
            IEnumerable<KeyValuePair<string, string>> strings = ListStrings(configuration);

            // List all strings
            if (key == null) return strings;

            if (namePolicy is IAssetNamePattern pattern)
            {
                return strings.Where(kp => pattern.Match(kp.Key).Success);
            }
            else
            {
                string key_name = namePolicy.BuildName(key);
                return strings.Where(kp => kp.Key.Contains(key_name));
            }
        }

        /// <summary>
        /// Try to read a localization string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resolved string or null</returns>
        public string GetString(IAssetKey key)
        {
            if (key == null) return null;
            string id = key.BuildName(namePolicy, parametrizer);
            return configuration[id];
        }

        CultureInfo[] cultures;
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (_cultures != null) return _cultures;

            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternPart culturePart;
                if (!pattern.PartMap.TryGetValue("culture", out culturePart)) return null;

                Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
                foreach (var kp in ListStrings(configuration))
                {
                    IAssetNamePatternMatch match = pattern.Match(kp.Key);
                    if (!match.Success) continue;
                    string culture = match[culturePart.CaptureIndex];
                    if (culture == null) culture = "";
                    if (result.ContainsKey(culture)) continue;
                    try { result[culture] = CultureInfo.GetCultureInfo(culture); } catch (CultureNotFoundException) { }
                }
                return cultures = result.Values.ToArray();
            }
            else
            {
                // Can't extract culture
                return null;
            }
        }

        public IAsset Reload()
        {
            ClearCache();
            if (configuration is IConfigurationRoot root) root.Reload();
            return this;
        }

        protected virtual void ClearCache()
        {
            cultures = null;
        }
    }

    public static partial class ConfigurationLocalizationExtensions
    {
        public static IAssetComposition AddConfiguration(this IAssetComposition localizationComposition, IConfiguration configuration, IAssetKeyNamePolicy namePolicy = default, IAssetKeyParametrizer parametrizer = default)
        {
            localizationComposition.Add(new ConfigurationLocalizationAsset(configuration, namePolicy, parametrizer));
            return localizationComposition;
        }
    }
}
