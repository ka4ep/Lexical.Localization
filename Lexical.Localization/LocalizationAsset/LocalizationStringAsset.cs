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
    /// This class contains key-value lines in format where the key is <see cref="string"/>.
    /// 
    /// The content is built from one or multiple configurable sources.
    /// 
    /// When language string is requested, the argument <see cref="IAssetKey"/> is converted to string for key matching. 
    /// 
    /// <see cref="IAssetKeyNamePolicy"/> is used for converting <see cref="IAssetKey"/> to <see cref="string"/>.
    /// This way the source file can have key notation where sections are not entirely distinguisable from each other.
    /// </summary>
    public class LocalizationStringAsset : ILocalizationStringProvider, ILocalizationStringLinesEnumerable, ILocalizationKeyLinesEnumerable, IAssetReloadable, ILocalizationAssetCultureCapabilities
    {
        /// <summary>
        /// Active dictionary of string lines.
        /// </summary>
        protected IReadOnlyDictionary<string, string> dictionary;

        /// <summary>
        /// Name policy that converts <see cref="IAssetKey"/> to string, and back to <see cref="IAssetKey"/>.
        /// </summary>
        protected IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<string, string>>> sources = new List<IEnumerable<KeyValuePair<string, string>>>();

        /// <summary>
        /// Comparer that can compare instances of <see cref="string"/>.
        /// </summary>
        IEqualityComparer<string> comparer;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePolicy">policy that describes how to convert localization key to dictionary key</param>
        public LocalizationStringAsset(IEnumerable<KeyValuePair<string, string>> source, IAssetKeyNamePolicy namePolicy)
        {
            this.sources.Add(source);
            this.namePolicy = namePolicy;
            Load();
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="namePattern">name patern</param>
        public LocalizationStringAsset(IEnumerable<KeyValuePair<string, string>> source, string namePattern) : this(source, new AssetNamePattern(namePattern))
        {
        }

        /// <summary>
        /// Create new localization string asset.
        /// </summary>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        /// <param name="comparer">(optional) string key comparer</param>
        public LocalizationStringAsset(IAssetKeyNamePolicy namePolicy, IEqualityComparer<string> comparer = default) : base()
        {
            this.namePolicy = namePolicy ?? throw new ArgumentNullException(nameof(namePolicy));
            this.comparer = comparer;
            Load();
        }

        /// <summary>
        /// Get language string. Converts <paramref name="key"/> into string by using <see cref="namePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            string id = namePolicy.BuildName(key);

            // Search dictionary
            dictionary.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Get string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<string, string>> GetStringLines(IAssetKey filterKey = null)
        {
            // Return all 
            if (filterKey == null) return dictionary;
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
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

        /// <summary>
        /// Get all string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<string, string>> GetAllStringLines(IAssetKey filterKey = null)
            => GetStringLines(filterKey);

        /// <summary>
        /// Get key lines.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey filterKey = null)
        {
            // Create filter.
            AssetKeyFilter filter = filterKey == null ? null : new AssetKeyFilter().KeyRule(filterKey);
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

        /// <summary>
        /// Get key lines.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetAllKeyLines(IAssetKey filterKey = null)
            => GetKeyLines(filterKey);

        IAsset IAssetReloadable.Reload() => Load();

        public virtual LocalizationStringAsset Load()
        {
            SetContent(sources.SelectMany(_ => _));
            return this;
        }

        /// <summary>
        /// Get available cultures
        /// </summary>
        /// <returns></returns>
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
            else if (namePolicy is IAssetKeyNameParser parser)
            {
                return cultures = dictionary.Keys.Select(k=>parser.TryParse(k, Key.Root)?.FindCulture()).Where(ci => ci != null).Distinct().ToArray();
            } else 
            {
                // Can't extract culture
                return null;
            }
        }
        CultureInfo[] cultures;

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="lines"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public LocalizationStringAsset AddSource(IEnumerable<KeyValuePair<string, string>> lines)
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
        public LocalizationStringAsset ClearSources(bool disposeSources)
        {
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

        /// <summary>
        /// Load content all <see cref="sources"/> into a new internal (<see cref="dictionary"/>). Replaces previous content.
        /// </summary>
        /// <returns>this</returns>
        protected virtual void SetContent(IEnumerable<KeyValuePair<string, string>> src)
        {
            var newMap = comparer == null ? new Dictionary<string, string>() : new Dictionary<string, string>(comparer);
            foreach (var line in src)
            {
                if (line.Key == null) continue;
                newMap[line.Key] = line.Value;
            }
            this.dictionary = newMap;
            this.cultures = null;
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            cultures = null;
            ClearSources(disposeSources: true);
        }

        /// <summary>
        /// Print class name
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    /// <summary>
    /// </summary>
    public static partial class LocalizationAssetExtensions_
    {
        /// <summary>
        /// Add string dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
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
