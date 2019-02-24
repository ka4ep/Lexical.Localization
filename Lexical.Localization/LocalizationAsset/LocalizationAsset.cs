// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains language strings.
    /// The key is in context free format format <see cref="Key"/>.
    /// </summary>
    public class LocalizationAsset :
        ILocalizationStringProvider, IAssetReloadable, IAssetKeyCollection,
        ILocalizationAssetCultureCapabilities
    {
        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<Key, string>>> sources;

        /// <summary>
        /// Values are copied here. Keys are context free instances of <see cref="Key"/>.
        /// </summary>
        protected Dictionary<IAssetKey, string> map;

        /// <summary>
        /// Comparer that can compare instances of <see cref="Key"/> and <see cref="IAssetKey"/>.
        /// </summary>
        public readonly IEqualityComparer<IAssetKey> Comparer;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">source dictionary of key-values.</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        /// <param name="parametrizer">(optional) object that extracts parameters from source and the string requests</param>
        public LocalizationAsset()
        {
            this.sources = new List<IEnumerable<KeyValuePair<Key, string>>>();
            this.Comparer = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();
            Load();
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeySource(IEnumerable<KeyValuePair<Key, string>> keyValueSource, string sourceHint = null)
        {
            if (keyValueSource == null) throw new ArgumentNullException(nameof(keyValueSource));
            lock (sources) sources.Add(keyValueSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddAssetKeySource(IEnumerable<KeyValuePair<IAssetKey, string>> keyValueSource, string sourceHint = null)
        {
            if (keyValueSource == null) throw new ArgumentNullException(nameof(keyValueSource));

            AssetKeyCloner cloner = new AssetKeyCloner(Key.Root).AddParameterToExclude("root"); 
            IEnumerable<KeyValuePair<Key, string>> adaptedSource = keyValueSource.Select( kp=>new KeyValuePair<Key, string>((Key)cloner.Copy(kp.Key), kp.Value) ).Where(kp=>kp.Key!=null);
            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="namePattern"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddStringSource(IEnumerable<KeyValuePair<string, string>> keyValueSource, string namePattern, string sourceHint = null)
            => AddStringSource(keyValueSource, new AssetNamePattern(namePattern), sourceHint);

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="namePolicy"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddStringSource(IEnumerable<KeyValuePair<string, string>> keyValueSource, IAssetKeyNamePolicy namePolicy, string sourceHint = null)
        {
            if (keyValueSource == null) throw new ArgumentNullException(nameof(keyValueSource));
            if (namePolicy == null) throw new ArgumentNullException(nameof(namePolicy));
            IEnumerable<KeyValuePair<Key, string>> adaptedSource = null;
            if (namePolicy is IAssetNamePattern patternPolicy)
            {
                adaptedSource = keyValueSource.Select(kp => new KeyValuePair<Key, string>(ConvertKey(kp.Key, patternPolicy), kp.Value)).Where(kp => kp.Key != null);
            }
            else if (namePolicy is ParameterNamePolicy parameterPolicy)
            {
                adaptedSource = keyValueSource.Select(kp => new KeyValuePair<Key, string>((Key)parameterPolicy.ParseKey(kp.Key, Key.Root), kp.Value)).Where(kp => kp.Key != null);
            }
            else {
                throw new ArgumentException($"Cannot add strings to {nameof(LocalizationAsset)} with {nameof(namePolicy)} {namePolicy.GetType().FullName}. Please use either {nameof(LocalizationStringAsset)}, or another policy such as {nameof(AssetNamePattern)} or {nameof(ParameterNamePolicy)}.");
            }

            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Convert string to <see cref="Key"/>.
        /// The parameter for "root" is skipped.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parametrizer"></param>
        /// <returns>key or null if there were no parameters</returns>
        Key ConvertKey(string key, IAssetNamePattern namePolicy)
        {
            IAssetNamePatternMatch match = namePolicy.Match(text: key, filledParameters: null);
            if (!match.Success) return null;
            Key result = null;
            foreach (var kp in match)
            {
                if (kp.Key == null || kp.Value == null) continue;
                result = new Key(result, kp.Key, kp.Value);
            }
            return result;
        }

        /// <summary>
        /// Clear all key-value sources.
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <returns></returns>
        public LocalizationAsset ClearSources()
        {
            lock (sources) sources.Clear();
            return this;
        }

        protected virtual IEnumerable<KeyValuePair<IAssetKey, string>> ConcatenateSources()
        {
            IEnumerable<KeyValuePair<Key, string>> result = null;
            foreach(var source in sources)
            {
                result = result == null ? source : result.Concat(source);
            }
            return result?.Select(kp=>new KeyValuePair<IAssetKey, string>(kp.Key, kp.Value)) ?? new KeyValuePair<IAssetKey, string>[0];
        }

        protected virtual void SetContent(IEnumerable<KeyValuePair<IAssetKey, string>> src)
        {
            var newMap = new Dictionary<IAssetKey, string>(Comparer);
            foreach (var line in src)
            {
                if (line.Key == null) continue;
                newMap[line.Key] = line.Value;
            }
            this.map = newMap;
        }

        IAsset IAssetReloadable.Reload()
        {
            return this.Load();
        }

        public virtual LocalizationAsset Load()
        {
            ClearCache();
            SetContent(ConcatenateSources());
            return this;
        }

        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            this.map.TryGetValue(key, out result);
            return result;
        }

        public IEnumerable<IAssetKey> GetAllKeys(IAssetKey criteriaKey = null)
        {
            IEnumerable<IAssetKey> keyEnumr = map.Keys;
            // Filter
            if (criteriaKey != null)
            {
                KeyValuePair<string, string>[] filterParameters = criteriaKey.GetParameters();
                if (filterParameters.Length > 0) keyEnumr = FilterKeys(keyEnumr, filterParameters);
            }
            return keyEnumr.ToArray();
        }

        IEnumerable<IAssetKey> FilterKeys(IEnumerable<IAssetKey> keys, KeyValuePair<string, string>[] filterParameters)
        {
            foreach (IAssetKey key in keys)
            {
                // Filter by criteria key
                bool ok = true;
                // Iterate all criteria parameters (key,value)
                foreach (var filterParameter in filterParameters)
                {
                    bool okk = false;
                    for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                    {
                        if (k.GetParameterName() == filterParameter.Key)
                        {
                            okk = k.Name == filterParameter.Value;
                            break;
                        }
                    }

                    // criteria did not match, go to next line
                    ok &= okk;
                    if (!ok) break;
                }

                if (ok) yield return key;
            }
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

            Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
            foreach (Key key in map.Keys)
            {
                for(Key k=key; k!=null; k=k.Previous)
                    if (k.Name=="culture")
                    {
                        string culture = k.Value ?? "";
                        if (result.ContainsKey(culture)) continue;
                        try { result[culture] = CultureInfo.GetCultureInfo(culture); } catch (CultureNotFoundException) { }
                    }
            }
            return cultures = result.Values.ToArray();
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
        /// <param name="builder"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IAssetBuilder AddKeySource(this IAssetBuilder builder, IEnumerable<KeyValuePair<Key, string>> dictionary)
        {
            builder.AddAsset(new LocalizationAsset().AddKeySource(dictionary).Load());
            return builder;
        }

        /// <summary>
        /// Add string dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to string</param>
        /// <returns></returns>
        public static IAssetComposition AddStrings(this IAssetComposition composition, IEnumerable<KeyValuePair<Key, string>> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new LocalizationAsset().AddKeySource(dictionary).Load());
            return composition;
        }
    }

}
