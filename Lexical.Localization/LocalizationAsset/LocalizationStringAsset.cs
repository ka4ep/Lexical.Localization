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
        ILocalizationStringProvider, ILocalizationStringLinesEnumerable, ILocalizationKeyLinesEnumerable, IAssetReloadable,
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
        /// <see cref="ILocalizationStringLinesEnumerable.GetAllStringLines(IAssetKey)"/> and 
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

        public IEnumerable<KeyValuePair<string, string>> GetStringLines(IAssetKey criteriaKey = null)
        {
            // Return all 
            if (criteriaKey == null) return dictionary;
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(criteriaKey);
            // There are no rules
            if (!filter.HasRules) return dictionary;
            // Filter with pattern
            if (namePolicy is IAssetNamePattern pattern_) return Filter1(pattern_);
            // Filter with parser
            if (namePolicy is IAssetKeyNameParser parser_) return Filter2(parser_);
            // Return nothing
            return null;

            IEnumerable<KeyValuePair<string, string>> Filter1(IAssetNamePattern pattern)
            {
                foreach (var line in dictionary)
                {
                    IAssetNamePatternMatch match = pattern.Match(line.Key);
                    if (!match.Success || !filter.Filter(match)) continue;
                    yield return line;
                }
            }
            IEnumerable<KeyValuePair<string, string>> Filter2(IAssetKeyNameParser parser)
            {
                foreach (var line in dictionary)
                {
                    IAssetKey key;
                    if (!parser.TryParse(line.Key, out key)) continue;
                    if (!filter.Filter(key)) continue;
                    yield return line;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllStringLines(IAssetKey criteriaKey)
            => GetStringLines(criteriaKey);

        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey criteriaKey = null)
        {
            // Create filter.
            AssetKeyFilter filter = criteriaKey == null ? null : new AssetKeyFilter().KeyRule(criteriaKey);
            // Filter with pattern
            if (namePolicy is IAssetNamePattern pattern_) return Filter1(pattern_);
            // Filter with parser
            if (namePolicy is IAssetKeyNameParser parser_) return Filter2(parser_);
            // Return nothing
            return null;

            IEnumerable<KeyValuePair<IAssetKey, string>> Filter1(IAssetNamePattern pattern)
            {
                foreach (var line in dictionary)
                {
                    IAssetNamePatternMatch match = pattern.Match(line.Key);
                    if (!match.Success) continue;
                    if (filter != null && !filter.Filter(match)) continue;
                    yield return new KeyValuePair<IAssetKey, string>(match.ToKey(), line.Value);
                }
            }
            IEnumerable<KeyValuePair<IAssetKey, string>> Filter2(IAssetKeyNameParser parser)
            {
                foreach (var line in dictionary)
                {
                    IAssetKey key;
                    if (!parser.TryParse(line.Key, out key)) continue;
                    if (filter != null && !filter.Filter(key)) continue;
                    yield return new KeyValuePair<IAssetKey, string>(key, line.Value);
                }
            }
        }

        public IEnumerable<KeyValuePair<IAssetKey, string>> GetAllKeyLines(IAssetKey criteriaKey)
            => GetKeyLines(criteriaKey);

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
