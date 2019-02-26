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
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains language strings.
    /// The key is in context free format format <see cref="Key"/>.
    /// </summary>
    public class LocalizationAsset :
        ILocalizationStringProvider, IAssetReloadable, IAssetKeyCollection,
        ILocalizationAssetCultureCapabilities, IDisposable
    {
        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<IAssetKey, string>>> sources;

        /// <summary>
        /// Active snapshot of key-values.
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
            this.sources = new List<IEnumerable<KeyValuePair<IAssetKey, string>>>();
            this.Comparer = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();
            Load();
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="keyValueSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeySource(IEnumerable<KeyValuePair<Key, string>> keyValueSource, string sourceHint = null)
        {
            if (keyValueSource == null) throw new ArgumentNullException(nameof(keyValueSource));
            lock (sources) sources.Add(keyValueSource.Select(kp=>new KeyValuePair<IAssetKey, string>((IAssetKey)kp.Key, kp.Value)));
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="keyValueSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="keyValueSource"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddKeySource(IEnumerable<KeyValuePair<IAssetKey, string>> keyValueSource, string sourceHint = null)
        {
            if (keyValueSource == null) throw new ArgumentNullException(nameof(keyValueSource));
            lock (sources) sources.Add(keyValueSource);
            return this;
        }

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="keyValueSource"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
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
        /// 
        /// If <paramref name="stringLines"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="stringLines"></param>
        /// <param name="namePolicy"></param>
        /// <param name="sourceHint">(optional) added to error message</param>
        /// <returns></returns>
        public LocalizationAsset AddStringSource(IEnumerable<KeyValuePair<string, string>> stringLines, IAssetKeyNamePolicy namePolicy, string sourceHint = null)
        {
            if (stringLines == null) throw new ArgumentNullException(nameof(stringLines));
            if (namePolicy == null) throw new ArgumentNullException(nameof(namePolicy));
            IEnumerable<KeyValuePair<IAssetKey, string>> adaptedSource = stringLines.ToKeyLines(namePolicy);
            lock (sources) sources.Add(adaptedSource);
            return this;
        }

        /// <summary>
        /// Clear all key-value sources.
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public LocalizationAsset ClearSources()
        {
            ClearCache();
            IDisposable[] disposables;
            lock (sources)
            {
                disposables = sources.Select(s => s as IDisposable).Where(s => s != null).ToArray();
                sources.Clear();
            }
            LazyList<Exception> errors = new LazyList<Exception>();
            foreach (IDisposable d in disposables)
            {
                try
                {
                    d.Dispose();
                } catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            if (errors.Count > 0) throw new AggregateException(errors);
            return this;
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            ClearSources();
        }

        protected virtual IEnumerable<KeyValuePair<IAssetKey, string>> ConcatenateSources()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> result = null;
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
            => map.Keys.FilterKeys(criteriaKey);

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
