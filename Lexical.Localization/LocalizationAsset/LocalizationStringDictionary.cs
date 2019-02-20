// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains localization key-value pairs. 
    /// The key is in string format, and is matched by converting the requesting IAssetKey to identity string with
    /// help of a <see cref="IAssetKeyNamePolicy"/>.
    /// 
    /// This class adapts IDictionary{string, string} to ILanguageStringResolver and ILanguageStringCollection.
    /// </summary>
    public class LocalizationStringDictionary :
        ILocalizationStringProvider, ILocalizationStringCollection, IAssetReloadable, IAssetKeyCollection,
        ILocalizationAssetCultureCapabilities
    {
        protected IReadOnlyDictionary<string, string> source;
        protected IAssetKeyNamePolicy namePolicy;
        IAssetKeyParametrizer parametrizer;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// 
        /// 
        /// If <paramref name="parametrizer"/> is provided this implementation can provide values for
        /// <see cref="ILocalizationStringCollection.GetAllStrings(IAssetKey)"/> and 
        /// <see cref="IAssetKeyCollection.GetAllKeys(IAssetKey)"/> requests.         
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

        public IEnumerable<Key> GetAllKeys(IAssetKey criteriaKey = null)
        {
            if (namePolicy is IAssetNamePattern pattern) return GetAllKeysWithPattern(pattern, criteriaKey);

            // Cannot provide keys
            return null;
        }

        IEnumerable<Key> GetAllKeysWithPattern(IAssetNamePattern pattern, IAssetKey criteriaKey)
        {
            KeyValuePair<string, string>[] criteriaParams = 
                criteriaKey == null ? null : (
                    criteriaKey is Key _criteria_key ? _criteria_key.ToKeyValueArray() :
                    AssetKeyParametrizer.Singleton.GetAllParameters(criteriaKey).ToArray()
                );

            foreach (var line in source)
            {
                IAssetNamePatternMatch match = pattern.Match(line.Key);
                if (!match.Success) continue;

                // Filter by criteria key
                bool ok = true;
                if (criteriaParams != null)
                {
                    // Iterate all criteria parameters (key,value)
                    foreach (var criteriaParameter in criteriaParams)
                    {
                        if (criteriaParameter.Key == "root") continue;
                        // Search key in our pattern.
                        IAssetNamePatternPart[] parts;
                        // If criteria has a parameter that is not in the pattern, then exit, no values can be provided.
                        if (!pattern.ParameterMap.TryGetValue(criteriaParameter.Key, out parts)) yield break;

                        // Test if one of the parts match criteria's value
                        bool okk = false;
                        foreach (var part in parts)
                        {
                            if (part.CaptureIndex < 0) continue;
                            string matchValue = match[part.CaptureIndex];
                            if (matchValue == criteriaParameter.Value) { okk = true; break; }
                        }
                        // criteria did not match, go to next line
                        ok &= okk;
                        if (!ok) break;
                    }

                }
                if (!ok) continue;

                Key _key = null;
                foreach (var part in pattern.CaptureParts)
                {
                    string partValue = match[part.CaptureIndex];
                    if (partValue == null) continue;

                    _key = new Key(_key, part.ParameterName, partValue);
                }
                if (_key != null) yield return _key;
            }
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
        public static IAssetBuilder AddStrings(this IAssetBuilder builder, IReadOnlyDictionary<string, string> dictionary, IAssetKeyNamePolicy namePolicy)
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
        public static IAssetComposition AddStrings(this IAssetComposition composition, IReadOnlyDictionary<string, string> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new LocalizationStringDictionary(dictionary, namePolicy));
            return composition;
        }
    }
}
