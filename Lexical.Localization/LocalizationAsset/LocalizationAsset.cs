// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class contains language strings. The key class is <see cref="IAssetKey"/>.
    /// 
    /// Content is loaded from <see cref="IEnumerable{T}"/> sources when <see cref="IAssetReloadable.Reload"/> is called.
    /// </summary>
    public class LocalizationAsset :
        ILocalizationStringProvider, IAssetReloadable, 
        ILocalizationKeyLinesEnumerable,
        ILocalizationAssetCultureCapabilities, IDisposable
    {
        /// <summary>
        /// Active dictionary of key-values.
        /// </summary>
        protected IDictionary<IAssetKey, string> dictionary;

        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<IAssetKey, string>>> sources;

        /// <summary>
        /// Comparer that can compare instances of <see cref="IAssetKey"/>.
        /// </summary>
        IEqualityComparer<IAssetKey> comparer;

        /// <summary>
        /// 
        /// </summary>
        protected LocalizationAsset() { }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="comparer">(optional) comparer to use</param>
        public LocalizationAsset(IEqualityComparer<IAssetKey> comparer = default) : base()
        {
            this.comparer = comparer ?? AssetKeyComparer.Default;
            this.sources = new List<IEnumerable<KeyValuePair<IAssetKey, string>>>();
            Load();
        }

        /// <summary>
        /// Create localization asset that uses <paramref name="lines"/> as source of key-values.
        /// </summary>
        /// <param name="lines">reference to use as key-value source</param>
        public LocalizationAsset(IDictionary<IAssetKey, string> lines) : this()
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            this.comparer = comparer ?? AssetKeyComparer.Default;
            this.sources = new List<IEnumerable<KeyValuePair<IAssetKey, string>>>();
            this.sources.Add(lines);
            Load();
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            ClearSources(disposeSources: true);
            this.cultures = null;
        }

        IAsset IAssetReloadable.Reload() => Load();

        /// <summary>
        /// Load content all <see cref="sources"/> into the asset. Replaces previous content.
        /// </summary>
        /// <returns></returns>
        public virtual LocalizationAsset Load()
        {
            SetContent(ConcatenateSources());
            return this;
        }

        /// <summary>
        /// Replaces <see cref="dictionary"/> with a new dictionary that is filled with lines from <paramref name="src"/>.
        /// </summary>
        /// <param name="src"></param>
        protected virtual void SetContent(IEnumerable<KeyValuePair<IAssetKey, string>> src)
        {
            var newMap = new Dictionary<IAssetKey, string>(comparer);
            foreach (var line in src)
            {
                if (line.Key == null) continue;
                newMap[line.Key] = line.Value;
            }
            // Replace reference
            this.cultures = null;
            this.dictionary = newMap;
        }

        /// <summary>
        /// Get language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            this.dictionary.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Iterate content and get supported cultures.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (_cultures != null) return _cultures;

            return cultures = dictionary.Keys.Select(k => k.FindCulture()).Where(ci => ci != null).Distinct().ToArray();
        }
        CultureInfo[] cultures;

        /// <summary>
        /// Get a snapshot of key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey filterKey = null)
        {
            var map = dictionary;
            if (map == null) return null;
            if (filterKey == null) return map;
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            return map.Where(line => filter.Filter(line.Key));
        }

        /// <summary>
        /// Get a snapshot of all the key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetAllKeyLines(IAssetKey filterKey = null)
            => GetKeyLines(filterKey);

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="linesSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="linesSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeyLinesSource(IEnumerable<KeyValuePair<Key, string>> linesSource, string sourceHint = null)
        {
            if (linesSource == null) throw new ArgumentNullException(nameof(linesSource));
            lock (sources) sources.Add(linesSource.Select(kp => new KeyValuePair<IAssetKey, string>((IAssetKey)kp.Key, kp.Value)));
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="linesSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="linesSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeyLinesSource(IEnumerable<KeyValuePair<IAssetKey, string>> linesSource, string sourceHint = null)
        {
            if (linesSource == null) throw new ArgumentNullException(nameof(linesSource));
            lock (sources) sources.Add(linesSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="linesSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="linesSource"></param>
        /// <param name="namePattern"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddStringLinesSource(IEnumerable<KeyValuePair<string, string>> linesSource, string namePattern, string sourceHint = null)
            => AddStringLinesSource(linesSource, new AssetNamePattern(namePattern), sourceHint);

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="stringLines"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="stringLines"></param>
        /// <param name="namePolicy"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddStringLinesSource(IEnumerable<KeyValuePair<string, string>> stringLines, IAssetKeyNamePolicy namePolicy, string sourceHint = null)
        {
            if (stringLines == null) throw new ArgumentNullException(nameof(stringLines));
            if (namePolicy is IAssetKeyNameParser == false) throw new ArgumentException($"{nameof(namePolicy)} must implement {nameof(IAssetKeyNameParser)}.");
            IEnumerable<KeyValuePair<IAssetKey, string>> adaptedSource = stringLines.ToKeyLines(namePolicy);
            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="treeSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="treeSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeyTreeSource(IEnumerable<IKeyTree> treeSource, string sourceHint = null)
        {
            if (treeSource == null) throw new ArgumentNullException(nameof(treeSource));
            IEnumerable<KeyValuePair<IAssetKey, string>> adaptedSource = treeSource.SelectMany(tree => tree.ToKeyLines());
            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="treeSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="treeSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeyTreeSource(IKeyTree treeSource, string sourceHint = null)
        {
            if (treeSource == null) throw new ArgumentNullException(nameof(treeSource));
            IEnumerable<KeyValuePair<IAssetKey, string>> adaptedSource = treeSource.ToKeyLines();
            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Clear all key-value sources.
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <returns></returns>
        /// <param name="disposeSources">if true, sources are disposed</param>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public LocalizationAsset ClearSources(bool disposeSources)
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
        /// Concatenate lines from each <see cref="sources"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<IAssetKey, string>> ConcatenateSources()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> result = null;
            foreach (var source in sources)
            {
                result = result == null ? source : result.Concat(source);
            }
            return result ?? no_lines;
        }
        static KeyValuePair<IAssetKey, string>[] no_lines = new KeyValuePair<IAssetKey, string>[0];

        /// <summary>
        /// Print name of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    public static partial class LocalizationAssetExtensions_
    {
        /// <summary>
        /// Add string dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetBuilder AddKeyLinesSource(this IAssetBuilder builder, IEnumerable<KeyValuePair<IAssetKey, string>> lines)
        {
            builder.AddAsset(new LocalizationAsset().AddKeyLinesSource(lines).Load());
            return builder;
        }

        /// <summary>
        /// Add string dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetComposition AddKeyLinesSource(this IAssetComposition composition, IEnumerable<KeyValuePair<IAssetKey, string>> lines)
        {
            composition.Add(new LocalizationAsset().AddKeyLinesSource(lines).Load());
            return composition;
        }
    }

}
