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
    /// This class contains language strings. The key is in <see cref="IAssetKey"/> format.
    /// 
    /// See: <see cref="LocalizationStringAsset"/> for container where key is <see cref="string"/>.
    /// See: <see cref="LoadableLocalizationAsset"/> for version where asset can be reloaded from its configured sources.
    /// </summary>
    public class LocalizationAsset :
        ILocalizationStringProvider, IAssetReloadable, IAssetKeyCollection,
        ILocalizationAssetCultureCapabilities, IDisposable
    {
        /// <summary>
        /// Active dictionary of key-values.
        /// </summary>
        protected IReadOnlyDictionary<IAssetKey, string> dictionary;

        protected LocalizationAsset() { }

        /// <summary>
        /// Create localization asset that uses <paramref name="lines"/> as source of key-values.
        /// </summary>
        /// <param name="lines">reference to use as key-value source</param>
        public LocalizationAsset(IReadOnlyDictionary<IAssetKey, string> lines) : this()
        {
            this.dictionary = lines ?? throw new ArgumentNullException(nameof(lines));
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            ClearCache();
        }

        public virtual IAsset Reload()
        {
            ClearCache();
            return this;
        }

        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            this.dictionary.TryGetValue(key, out result);
            return result;
        }

        public IEnumerable<IAssetKey> GetAllKeys(IAssetKey criteriaKey = null)
            => dictionary.Keys.FilterKeys(criteriaKey);

        protected virtual void ClearCache()
        {
            cultures = null;
        }

        /// <summary>
        /// Iterate content and get supported cultures.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (_cultures != null) return _cultures;

            Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
            foreach (Key key in dictionary.Keys)
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
        CultureInfo[] cultures;

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

        /// <summary>
        /// Print name of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    /// <summary>
    /// This class contains language strings. The key is in <see cref="IAssetKey"/> format.
    /// 
    /// Asset can be reloaded from its configured sources with <see cref="IAssetReloadable.Reload"/>.
    /// </summary>
    public class LoadableLocalizationAsset : LocalizationAsset
    {
        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<IAssetKey, string>>> sources;

        /// <summary>
        /// Comparer that can compare instances of <see cref="IAssetKey"/>.
        /// </summary>
        IEqualityComparer<IAssetKey> comparer;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="comparer">(optional) comparer to use</param>
        public LoadableLocalizationAsset(IEqualityComparer<IAssetKey> comparer = default) : base()
        {
            this.comparer = comparer ?? AssetKeyComparer.Default;
            this.sources = new List<IEnumerable<KeyValuePair<IAssetKey, string>>>();
            Load();
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="linesSourec"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="linesSourec"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LoadableLocalizationAsset AddKeyLinesSource(IEnumerable<KeyValuePair<Key, string>> linesSourec, string sourceHint = null)
        {
            if (linesSourec == null) throw new ArgumentNullException(nameof(linesSourec));
            lock (sources) sources.Add(linesSourec.Select(kp => new KeyValuePair<IAssetKey, string>((IAssetKey)kp.Key, kp.Value)));
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
        public LoadableLocalizationAsset AddKeyLinesSource(IEnumerable<KeyValuePair<IAssetKey, string>> linesSource, string sourceHint = null)
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
        public LoadableLocalizationAsset AddKeyStringSource(IEnumerable<KeyValuePair<string, string>> linesSource, string namePattern, string sourceHint = null)
            => AddKeyStringSource(linesSource, new AssetNamePattern(namePattern), sourceHint);

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
        public LoadableLocalizationAsset AddKeyStringSource(IEnumerable<KeyValuePair<string, string>> stringLines, IAssetKeyNamePolicy namePolicy, string sourceHint = null)
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
        public LoadableLocalizationAsset AddKeyTreeSource(IEnumerable<IKeyTree> treeSource, string sourceHint = null)
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
        public LoadableLocalizationAsset AddKeyTreeSource(IKeyTree treeSource, string sourceHint = null)
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
        public LoadableLocalizationAsset ClearSources(bool disposeSources)
        {
            ClearCache();
            IDisposable[] disposables = null;
            lock (sources)
            {
                if (disposeSources) disposables = sources.Select(s => s as IDisposable).Where(s => s != null).ToArray();
                sources.Clear();
            }
            LazyList<Exception> errors = new LazyList<Exception>();
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

        static KeyValuePair<IAssetKey, string>[] no_lines = new KeyValuePair<IAssetKey, string>[0];
        protected virtual IEnumerable<KeyValuePair<IAssetKey, string>> ConcatenateSources()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> result = null;
            foreach (var source in sources)
            {
                result = result == null ? source : result.Concat(source);
            }
            return result ?? no_lines;
        }

        protected virtual void SetContent(IEnumerable<KeyValuePair<IAssetKey, string>> src)
        {
            var newMap = new Dictionary<IAssetKey, string>(comparer);
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
        public override void Dispose()
        {
            base.Dispose();
            ClearSources(disposeSources: true);
        }

        public virtual LoadableLocalizationAsset Load()
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
        /// <param name="builder"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IAssetBuilder AddKeySource(this IAssetBuilder builder, IEnumerable<KeyValuePair<Key, string>> dictionary)
        {
            builder.AddAsset(new LoadableLocalizationAsset().AddKeyLinesSource(dictionary).Load());
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
            composition.Add(new LoadableLocalizationAsset().AddKeyLinesSource(dictionary).Load());
            return composition;
        }
    }

}
