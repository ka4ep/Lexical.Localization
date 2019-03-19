// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Language string container that reads key-value pairs from <see cref="IReadOnlyDictionary{string, string}"/> source.
    /// 
    /// Requesting <see cref="IAssetKey"/>s are converted to strings for key matching. 
    /// <see cref="IAssetKeyNamePolicy"/> is used for converting <see cref="IAssetKey"/> to <see cref="string"/>.
    /// This way the source file can have key notation where sections are not entirely distinguisable from each other.
    /// </summary>
    public class LocalizationStringAsset :
        ILocalizationStringProvider, ILocalizationStringCollection, IAssetReloadable, IAssetKeyCollection,
        ILocalizationAssetCultureCapabilities
    {
        /// <summary>
        /// The default policy this asset uses.
        /// </summary>
        public static readonly IAssetKeyNamePolicy DefaultPolicy = AssetKeyNameProvider.Default;

        protected IReadOnlyDictionary<string, string> dictionary;
        protected IAssetKeyNamePolicy namePolicy;

        protected LocalizationStringAsset() { }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// <see cref="ILocalizationStringCollection.GetAllStrings(IAssetKey)"/> and 
        /// <see cref="IAssetKeyCollection.GetAllKeys(IAssetKey)"/> requests.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        public LocalizationStringAsset(IReadOnlyDictionary<string, string> source, IAssetKeyNamePolicy namePolicy = default)
        {
            this.dictionary = source ?? throw new ArgumentNullException(nameof(source));
            this.namePolicy = namePolicy ?? DefaultPolicy;
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePattern">name patern</param>
        public LocalizationStringAsset(IReadOnlyDictionary<string, string> source, string namePattern) : this(source, new AssetNamePattern(namePattern)) { }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        public LocalizationStringAsset(IEnumerable<KeyValuePair<string, string>> source, IAssetKeyNamePolicy namePolicy = default)
        {
            this.dictionary = source is IReadOnlyDictionary<string, string> map ? map : 
                source?.ToDictionary(line=>line.Key, line=>line.Value)
                ?? throw new ArgumentNullException(nameof(source));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePattern">name patern</param>
        public LocalizationStringAsset(IEnumerable<KeyValuePair<string, string>> source, string namePattern) : this(source, new AssetNamePattern(namePattern)) { }

        public virtual IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key)
        {
            if (key == null) return dictionary;
            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternMatch match = pattern.Match(key);
                return dictionary.Where(kp => IsEqualOrSuperset(match, pattern.Match(kp.Key)));
            }
            else
            {
                string key_name = namePolicy.BuildName(key);
                return dictionary.Where(kp => kp.Key.Contains(key_name));
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => dictionary.GetEnumerator();

        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            string id = namePolicy.BuildName(key);

            // Search dictionary
            dictionary.TryGetValue(id, out result);
            return result;
        }

        public IEnumerable<IAssetKey> GetKeys(IAssetKey criteriaKey = null)
        {
            if (namePolicy is IAssetNamePattern pattern) return GetAllKeysWithPattern(pattern, criteriaKey);

            // Cannot provide keys
            return null;
        }

        public IEnumerable<IAssetKey> GetAllKeys(IAssetKey criteriaKey = null)
        {
            if (namePolicy is IAssetNamePattern pattern)
            {
                List<IAssetKey> list = new List<IAssetKey>(dictionary.Count *2);
                foreach(IAssetKey key in GetAllKeysWithPattern(pattern, criteriaKey))
                {
                    if (key == null) return null;
                    list.Add(key);
                }
                return list;
            }

            // Cannot provide keys
            return null;
        }

        IEnumerable<IAssetKey> GetAllKeysWithPattern(IAssetNamePattern pattern, IAssetKey criteriaKey)
        {
            KeyValuePair<string, string>[] criteriaParams = criteriaKey.GetParameters();
            
            foreach (var line in dictionary)
            {
                IAssetNamePatternMatch match = pattern.Match(line.Key);
                if (!match.Success)
                {
                    yield return null;
                    continue;
                }

                // Filter by criteria key
                bool ok = true;
                if (criteriaParams != null)
                {
                    // Iterate all criteria parameters (key,value)
                    foreach (var criteriaParameter in criteriaParams)
                    {
                        if (criteriaParameter.Key == "Root") continue;
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

                Key _key = Key.Root;
                foreach (var part in pattern.CaptureParts)
                {
                    string partValue = match[part.CaptureIndex];
                    if (partValue == null) continue;

                    _key = _key.Append(part.ParameterName, partValue);
                }
                if (_key != null) yield return _key;
            }
        }

        public virtual IAsset Reload()
        {
            ClearCache();
            // If cultures is buing built, the cache becomes wrong, but 
            // Reload() isn't intended for initialization not concurrency.
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
                if (!pattern.PartMap.TryGetValue("Culture", out culturePart)) return null;

                Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
                foreach (var kp in dictionary)
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

    /// <summary>
    /// This class contains key-value lines in format where the key is <see cref="string"/>.
    /// 
    /// The content is built from one or multiple configurable sources.    
    /// </summary>
    public class LoadableLocalizationStringAsset : LocalizationStringAsset, IDisposable
    {
        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<string, string>>> sources;

        /// <summary>
        /// Comparer that can compare instances of <see cref="string"/>.
        /// </summary>
        IEqualityComparer<string> comparer;

        /// <summary>
        /// Create new localization string asset.
        /// </summary>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        /// <param name="comparer">(optional) string key comparer</param>
        public LoadableLocalizationStringAsset(IAssetKeyNamePolicy namePolicy = default, IEqualityComparer<string> comparer = default) : base()
        {
            this.namePolicy = namePolicy ?? DefaultPolicy;
            this.comparer = comparer ?? StringComparer.InvariantCulture;
            this.sources = new List<IEnumerable<KeyValuePair<string, string>>>();
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="lines"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LoadableLocalizationStringAsset AddLineStringSource(IEnumerable<KeyValuePair<string, string>> lines, string sourceHint = null)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            lock (sources) sources.Add(lines);
            return this;
        }

        /// <summary>
        /// Clear all key-value sources.
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <returns></returns>
        /// <param name="disposeSources">if true, sources are disposed</param>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public LoadableLocalizationStringAsset ClearSources(bool disposeSources)
        {
            ClearCache();
            IDisposable[] disposables = null;
            lock (sources)
            {
                if (disposeSources) disposables = sources.Select(s => s as IDisposable).Where(s => s != null).ToArray();
                sources.Clear();
            }
            StructList4<Exception> errors = new StructList4<Exception>();
            if (disposeSources)
                foreach (IDisposable d in disposables)
                {
                    try
                    {
                        d.Dispose();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }
            if (errors.Count > 0) throw new AggregateException(errors);
            return this;
        }

        static KeyValuePair<string, string>[] no_lines = new KeyValuePair<string, string>[0];
        protected virtual IEnumerable<KeyValuePair<string, string>> ConcatenateSources()
        {
            IEnumerable<KeyValuePair<string, string>> result = null;
            foreach (var source in sources)
            {
                result = result == null ? source : result.Concat(source);
            }
            return result ?? no_lines;
        }

        protected virtual void SetContent(IEnumerable<KeyValuePair<string, string>> src)
        {
            var newMap = new Dictionary<string, string>(comparer);
            foreach (var line in src)
            {
                if (line.Key == null) continue;
                newMap[line.Key] = line.Value;
            }
            this.dictionary = newMap;
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            ClearSources(disposeSources: true);
        }

        public virtual LoadableLocalizationStringAsset Load()
        {
            base.Reload();
            SetContent(ConcatenateSources());
            return this;
        }
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
            builder.AddAsset(new LocalizationStringAsset(dictionary, namePolicy));
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
            composition.Add(new LocalizationStringAsset(dictionary, namePolicy));
            return composition;
        }
    }
}
