// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class adapts IDictionary{string, string} to ILanguageStringResolver and ILanguageStringCollection.
    /// </summary>
    public class LocalizationStringDictionary : 
        ILocalizationStringProvider, ILocalizationStringCollection, IAssetReloadable, 
        ILocalizationAssetCultureCapabilities
    {
        protected IReadOnlyDictionary<string, string> source;
        protected IAssetKeyNamePolicy namePolicy;
        IAssetKeyParametrizer parametrizer;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        /// <param name="parametrizer">(optional) object that extracts parameters</param>
        public LocalizationStringDictionary(IReadOnlyDictionary<string, string> source, IAssetKeyNamePolicy namePolicy = default, IAssetKeyParametrizer parametrizer = default)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
            this.parametrizer = parametrizer ?? AssetKeyParametrizer.Singleton;
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePattern">name patern</param>
        /// <param name="parametrizer">(optional) object that extracts parameters</param>
        public LocalizationStringDictionary(IReadOnlyDictionary<string, string> source, string namePattern, IAssetKeyParametrizer parametrizer = default)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.namePolicy = new AssetNamePattern(namePattern);
            this.parametrizer = parametrizer ?? AssetKeyParametrizer.Singleton;
        }

        public virtual IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key)
        {
            if (key == null) return source;
            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternMatch match = pattern.Match(key, parametrizer);
                return source.Where(kp => IsEqualOrSuperset(match, pattern.Match(kp.Key)));
            }
            else
            {
                string key_name = namePolicy.BuildName(key);
                return source.Where(kp => kp.Key.Contains(key_name));
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() 
            => source.GetEnumerator();

        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            string id = namePolicy.BuildName(key);

            // Search dictionary
            source.TryGetValue(id, out result);
            return result;
        }

        public virtual IAsset Reload()
        {
            ClearCache();
            // If cultures is buing built, the cache becomes wrong, but 
            // Reload() isn't intended for multi-thread use anyway. Only for initialization.
            return this;
        }

        protected virtual void ClearCache()
        {
            cultures = null;
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
                foreach (var kp in source)
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


        /// <summary>
        /// Comapres two matches for equality or being superset.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        static bool IsEqualOrSuperset(IAssetNamePatternMatch match, IAssetNamePatternMatch other)
        {
            if (match.Pattern != other.Pattern) return false;
            for (int ix = 0; ix < match.Pattern.CaptureParts.Length; ix++)
            {
                IAssetNamePatternPart part = match.Pattern.CaptureParts[ix];

                if (match.PartValues[ix] == null) continue;
                if (match.PartValues[ix] != other.PartValues[ix]) return false;
            }
            return true;
        }

        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class LocalizationAssetExtensions_
    {
        /// <summary>
        /// Add string dictionary to builder.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to string</param>
        /// <returns></returns>
        public static IAssetBuilder AddDictionary(this IAssetBuilder builder, IReadOnlyDictionary<string, string> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            builder.AddAsset(new LocalizationStringDictionary(dictionary, namePolicy));
            return builder;
        }

        /// <summary>
        /// Add string dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to string</param>
        /// <returns></returns>
        public static IAssetComposition AddDictionary(this IAssetComposition composition, IReadOnlyDictionary<string, string> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new LocalizationStringDictionary(dictionary, namePolicy));
            return composition;
        }


        /// <summary>
        /// Adapts <see cref="Delegate"/> to <see cref="IAssetSource"/> and adds to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resolver"></param>
        /// <returns>builder</returns>
        public static IAssetBuilder AddSourceFunc(this IAssetBuilder builder, Func<IAssetKey, string> resolver)
        {
            builder.Sources.Add(new AssetSource(new LocalizationStringsFunc(resolver)));
            return builder;
        }

    }
}
