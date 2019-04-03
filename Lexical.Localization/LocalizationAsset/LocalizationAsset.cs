// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// This class contains language strings. The key class is <see cref="IAssetKey"/>.
    /// 
    /// Content is loaded from <see cref="IEnumerable{T}"/> sources when <see cref="IAssetReloadable.Reload"/> is called.
    /// </summary>
    public class LocalizationAsset : ILocalizationStringProvider, IAssetReloadable, ILocalizationKeyLinesEnumerable, ILocalizationAssetCultureCapabilities, IDisposable
    {
        /// <summary>
        /// Loaded and active key-values. It is compiled union of all sources.
        /// </summary>
        protected IReadOnlyDictionary<IAssetKey, string> dictionary;

        /// <summary>
        /// List of source where values are read from when <see cref="Load"/> is called.
        /// </summary>
        protected List<IEnumerable<KeyValuePair<IAssetKey, string>>> sources;

        /// <summary>
        /// <see cref="IAssetKey"/> comparer for <see cref="dictionary"/>.
        /// </summary>
        IEqualityComparer<IAssetKey> comparer;

        /// <summary>
        /// Create string asset with no initial sources.
        /// </summary>
        public LocalizationAsset()
        {
            this.comparer = comparer ?? AssetKeyComparer.Default;
            this.sources = new List<IEnumerable<KeyValuePair<IAssetKey, string>>>();
            Load();
        }

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
        /// Load content all <see cref="sources"/> into a new internal (<see cref="dictionary"/>). Replaces previous content.
        /// </summary>
        /// <returns>this</returns>
        public virtual LocalizationAsset Load()
        {
            SetContent(sources.SelectMany(l=>l));
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
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable<KeyValuePair<Key, string>> linesSource)
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
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable<KeyValuePair<IAssetKey, string>> linesSource)
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
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable<KeyValuePair<string, string>> linesSource, string namePattern)
            => AddSource(linesSource, new AssetNamePattern(namePattern));

        /// <summary>
        /// Add language string key-value source. 
        /// Caller must call <see cref="Load"/> afterwards to make the changes effective.
        /// 
        /// If <paramref name="stringLines"/> implements <see cref="IDisposable"/>, then its disposed along with the class or when <see cref="ClearSources"/> is called.
        /// </summary>
        /// <param name="stringLines"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable<KeyValuePair<string, string>> stringLines, IAssetKeyNamePolicy namePolicy)
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
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable<IKeyTree> treeSource)
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
        /// <returns></returns>
        public LocalizationAsset AddSource(IKeyTree treeSource)
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
        /// Print name of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";

    }

    /// <summary>
    /// Line source information
    /// </summary>
    public class Source : IObserver<IAssetSourceEvent>, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        /// <summary>
        /// Reader, the original reference.
        /// </summary>
        protected IEnumerable source;

        /// <summary>
        /// Casted to key string reader.
        /// </summary>
        protected IEnumerable<KeyValuePair<IAssetKey, string>> keyLinesReader;

        /// <summary>
        /// Previously loaded snapshot.
        /// </summary>
        protected KeyValuePair<IAssetKey, string>[] snapshot;

        /// <summary>
        /// Previous line count.
        /// </summary>
        protected int lineCount;

        /// <summary>
        /// Handle that observes source
        /// </summary>
        protected IDisposable observerHandle;

        /// <summary>
        /// Name policy.
        /// 
        /// If source is string lines the parses into strings into <see cref="IAssetKey"/>.
        /// </summary>
        protected IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// Handler that processes file load errors, and file monitoring errors.
        /// 
        /// If <see cref="errorHandler"/> returns true, or there is no handler, then exception is thrown and asset loading fails.
        /// 
        /// If <see cref="errorHandler"/> returns false, then exception is caught and empty list is used.
        /// </summary>
        protected Func<Exception, bool> errorHandler;

        /// <summary>
        /// Create source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="namePolicy">used if source is string line enumerable</param>
        /// <param name="errorHandler">(optional) handles file load and observe errors for logging and capturing exceptions. If <paramref name="errorHandler"/> returns false then exception is caught and not thrown upwards</param>
        public Source(IEnumerable source, IAssetKeyNamePolicy namePolicy, Func<Exception, bool> errorHandler)
        {
            this.source = source;
            this.namePolicy = namePolicy;
            this.keyLinesReader = source is IEnumerable<KeyValuePair<IAssetKey, string>> keyLines ? keyLines :
                source is IEnumerable<KeyValuePair<string, string>> stringLines ? stringLines.ToKeyLines(namePolicy) :
                source is IEnumerable<IKeyTree> trees ? trees.SelectMany(tree => tree.ToKeyLines()) :
                throw new ArgumentException("source must be key-lines, string-lines or key tree", nameof(source));
            this.errorHandler = errorHandler;

            if (source is IObservable<IAssetSourceEvent> observable)
            {
                try
                {
                    observerHandle = observable.Subscribe(this);
                } catch (Exception e) when (errorHandler==null?true:!errorHandler(e))
                {
                    // Discard error.
                }
            }
        }

        /// <summary>
        /// Dispose source information
        /// </summary>
        public virtual void Dispose(ref StructList4<Exception> errors)
        {
            try
            {
                // Cancel observer
                Interlocked.CompareExchange(ref observerHandle, null, observerHandle)?.Dispose();
            }
            catch (Exception e)
            {
                errors.Add(e);
            }
        }

        KeyValuePair<IAssetKey, string>[] GetLines()
        {
            return null;
            /*            // Read lines
                        try
                        {
                            list.AddRange(keyLinesReader);
                            snapshot = list.ToArray();

                        }
                        catch (Exception e)
                        {
                            // Problem reading file
                        }
            */
        }

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asset source stopped sending events
        /// </summary>
        public virtual void OnCompleted()
        {
            // Cancel observer
            Interlocked.CompareExchange(ref observerHandle, null, observerHandle)?.Dispose();
        }

        /// <summary>
        /// Error while monitoring asset source
        /// </summary>
        /// <param name="error"></param>
        public virtual void OnError(Exception error)
        {
        }

        /// <summary>
        /// Source file changed.
        /// </summary>
        /// <param name="value"></param>
        public virtual void OnNext(IAssetSourceEvent value)
        {
            if (value is IAssetChangeEvent changeEvent)
            {
                // Discard snapshot
                snapshot = null;

                // Pass on event.
            }
        }

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => source.ToString();

    }


    public static partial class LocalizationAssetExtensions_
    {
        /// <summary>
        /// Add string dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetBuilder AddKeyLines(this IAssetBuilder builder, IEnumerable<KeyValuePair<IAssetKey, string>> lines)
        {
            builder.AddAsset(new LocalizationAsset().AddSource(lines).Load());
            return builder;
        }

        /// <summary>
        /// Add string dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetComposition AddKeyLines(this IAssetComposition composition, IEnumerable<KeyValuePair<IAssetKey, string>> lines)
        {
            composition.Add(new LocalizationAsset().AddSource(lines).Load());
            return composition;
        }
    }

}
