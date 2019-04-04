// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lexical.Localization
{
    /// <summary>
    /// This class contains language strings. The key class is <see cref="IAssetKey"/>.
    /// 
    /// Content is loaded from <see cref="IEnumerable{T}"/> sources when <see cref="IAssetReloadable.Reload"/> is called.
    /// </summary>
    public class LocalizationAsset : ILocalizationStringProvider, IAssetReloadable, ILocalizationKeyLinesEnumerable, ILocalizationAssetCultureCapabilities, IDisposable, IAssetObservable
    {
        /// <summary>
        /// Loaded and active key-values. It is compiled union of all sources.
        /// </summary>
        protected IReadOnlyDictionary<IAssetKey, string> lines;

        /// <summary>
        /// Collections of lines and source readers. They are read when <see cref="Load"/> is called.
        /// </summary>
        protected ConcurrentDictionary<IEnumerable, Collection> collections = new ConcurrentDictionary<IEnumerable, Collection>();

        /// <summary>
        /// Timer task that reloads content.
        /// </summary>
        protected Task reloadTask;

        /// <summary>
        /// <see cref="IAssetKey"/> comparer for <see cref="lines"/>.
        /// </summary>
        IEqualityComparer<IAssetKey> comparer;

        /// <summary>
        /// Handler that processes file load errors, and file monitoring errors.
        /// 
        /// If <see cref="errorHandler"/> returns false, or there is no handler, then exception is thrown and asset loading fails.
        /// If <see cref="errorHandler"/> returns true, then exception is caught and empty list is used.
        /// </summary>
        Func<Exception, bool> errorHandler;

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEqualityComparer<IAssetKey> comparer = default, Func<Exception, bool> errorHandler = null) : base()
        {
            this.comparer = comparer ?? AssetKeyComparer.Default;
            this.errorHandler = errorHandler;
            // Load to create snapshop
            Load();
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            this.cultures = null;
            ClearSources();
        }

        /// <summary>
        /// Load new and changed files.
        /// </summary>
        /// <returns>this</returns>
        /// <exception cref="Exception">On any non-captured problem</exception>
        public virtual LocalizationAsset Load()
        {
            SetContent(collections.ToArray().SelectMany(l=>l.Value));
            return this;
        }

        /// <summary>
        /// Reload all files.
        /// </summary>
        /// <returns>this</returns>
        /// <exception cref="Exception">On any non-captured problem</exception>
        public virtual IAsset Reload()
        {
            // Clear snapshots
            var collectionLines = collections.ToArray();
            foreach (var line in collectionLines)
                line.Value.snapshot = null;

            // Set content
            SetContent(collectionLines.SelectMany(l => l.Value));
            return this;
        }

        /// <summary>
        /// Replaces <see cref="lines"/> with a new dictionary that is filled with lines from <paramref name="src"/>.
        /// </summary>
        /// <param name="src"></param>
        protected virtual void SetContent(IEnumerable<KeyValuePair<IAssetKey, string>> src)
        {
            Dictionary<IAssetKey, string> newLines = new Dictionary<IAssetKey, string>(comparer);
            foreach (var line in src)
            {
                if (line.Key == null) continue;
                newLines[line.Key] = line.Value;
            }
            // Replace reference
            // TODO notify observers, such as cache, that content is changed.
            this.cultures = null;
            this.lines = newLines;
        }

        /// <summary>
        /// Get language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public virtual string GetString(IAssetKey key)
        {
            string result = null;
            this.lines.TryGetValue(key, out result);
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

            return cultures = lines.Keys.Select(k => k.FindCulture()).Where(ci => ci != null).Distinct().ToArray();
        }
        CultureInfo[] cultures;

        /// <summary>
        /// Get a snapshot of key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey filterKey = null)
        {
            var map = lines;
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
        /// Add reader of lines.
        /// 
        /// Reader must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;IAssetKey, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;IKeyTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="namePolicy">(optional) name policy that reads the content. Required if reader implements string reader</param>
        /// <param name="errorHandler">(optional) overrides default handler.</param>
        /// <param name="disposeReader">Dispose <paramref name="reader"/> along with <see cref="LocalizationAsset"/></param>
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable reader, IAssetKeyNamePolicy namePolicy = null, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
        {
            // Reader argument not null
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            // IEnumerable<KeyValuePair<string,string>> must be added with namePolicy
            if (reader is IEnumerable<KeyValuePair<IAssetKey, string>> == false && reader is IEnumerable<IKeyTree> == false && reader is IEnumerable<KeyValuePair<string, string>> && namePolicy == null)
                throw new ArgumentNullException(nameof(namePolicy), $"{nameof(namePolicy)} name policy must be provided for reader of type {reader.GetType().FullName}");

            // Create collection
            var _errorHandler = errorHandler ?? this.errorHandler;
            Collection collection = new Collection(reader, namePolicy, _errorHandler, this, disposeReader);
            // Start observing file changes
            collection.SubscribeObserving();
            // Add to collection
            bool addedOk = collections.TryAdd(reader, collection);

            // Adding failed, dispose the collection
            if (!addedOk)
            {
                StructList4<Exception> errors = new StructList4<Exception>();
                collection.Dispose(ref errors);
                if (errors.Count > 0)
                {
                    Exception e = errors.Count == 1 ? errors[0] : new AggregateException(errors);
                    if (_errorHandler == null || !_errorHandler(e)) throw errors.Count == 1 ? new AggregateException(errors) : e;
                }
            }
            return this;
        }

        /// Add reader of lines.
        /// 
        /// Reader must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;IAssetKey, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;IKeyTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="namePolicy">name policy that reads the content. Required if reader implements string reader</param>
        /// <param name="errorHandler">(optional) overrides default handler.</param>
        /// <param name="disposeReader">Dispose <paramref name="reader"/> along with <see cref="LocalizationAsset"/></param>
        /// <returns></returns>
        public LocalizationAsset AddSource(IEnumerable reader, string namePattern, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
            => AddSource(reader, new AssetNamePattern(namePattern), errorHandler, disposeReader);

        /// <summary>
        /// Remove <paramref name="reader"/>. If reader was added with disposeReader, it will be disposed here.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LocalizationAsset RemoveSource(IEnumerable reader)
        {
            Collection c;
            if (collections.TryRemove(reader, out c))
            {
                StructList4<Exception> errors = new StructList4<Exception>();
                c.Dispose(ref errors);
                if (errors.Count > 0)
                {
                    Exception e = errors.Count == 1 ? errors[0] : new AggregateException(errors);
                    if (c.errorHandler == null || !c.errorHandler(e)) throw errors.Count == 1 ? new AggregateException(errors) : e;
                }
            }
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
            StructList4<Exception> errors = new StructList4<Exception>();
            foreach (var collectionLine in collections.ToArray())
            {
                Collection c;
                collections.TryRemove(collectionLine.Key, out c);
                collectionLine.Value.Dispose(ref errors);
            }
            if (errors.Count > 0) throw new AggregateException(errors);
            return this;
        }

        /// <summary>
        /// Starts a task that reloads the content. 
        /// Task sleeps for a while (500ms) before loading content.
        /// Task is not started if one is already sleeping.
        /// </summary>
        public virtual void StartReloadTimer()
        {
            // Reload task already running
            if (reloadTask != null) return;

            Task[] tasks = new Task[1];
            tasks[0] = new Task(ReloadTask, tasks);
            Interlocked.CompareExchange(ref reloadTask, tasks[0], null);
            tasks[0].Start();
        }        

        /// <summary>
        /// Task that is called by <see cref="StartReloadTimer"/>.
        /// </summary>
        /// <param name="tasks"></param>
        protected void ReloadTask(object tasks)
        {
            // Wait for a while
            Thread.Sleep(500);

            // Remove task reference
            Interlocked.CompareExchange(ref reloadTask, null, (tasks as Task[])[0]);

            // Reload changed content
            Load();
        }

        /// <summary>
        /// Subscribe for content change events
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<IAssetEvent> observer)
        {
            // TODO
            return null;
        }

        /// <summary>
        /// Print name of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{GetType().Name}()";
    }

    /// <summary>
    /// Collection of lines
    /// </summary>
    public class Collection : IObserver<IAssetSourceEvent>, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        /// <summary>
        /// Reader, the original reference.
        /// </summary>
        protected IEnumerable reader;

        /// <summary>
        /// Casted to key string reader.
        /// </summary>
        protected IEnumerable<KeyValuePair<IAssetKey, string>> keyLinesReader;

        /// <summary>
        /// Previously loaded snapshot.
        /// </summary>
        internal protected KeyValuePair<IAssetKey, string>[] snapshot;

        /// <summary>
        /// Previous line count.
        /// </summary>
        protected int lineCount = -1;

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
        /// If <see cref="errorHandler"/> returns false, or there is no handler, then exception is thrown and asset loading fails.
        /// If <see cref="errorHandler"/> returns true, then exception is caught and empty list is used.
        /// </summary>
        internal protected Func<Exception, bool> errorHandler;

        /// <summary>
        /// Parent object
        /// </summary>
        protected LocalizationAsset parent;

        /// <summary>
        /// Should reader be disposed along with this class.
        /// </summary>
        protected bool disposeReader;

        /// <summary>
        /// Create source
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="namePolicy">used if source is string line enumerable</param>
        /// <param name="errorHandler">(optional) handles file load and observe errors for logging and capturing exceptions. If <paramref name="errorHandler"/> returns true then exception is caught and not thrown</param>
        /// <param name="parent"></param>
        /// <param name="disposeReader"></param>
        public Collection(IEnumerable reader, IAssetKeyNamePolicy namePolicy, Func<Exception, bool> errorHandler, LocalizationAsset parent, bool disposeReader)
        {
            this.parent = parent;
            this.reader = reader;
            this.namePolicy = namePolicy;
            this.keyLinesReader = reader is IEnumerable<KeyValuePair<IAssetKey, string>> keyLines ? keyLines :
                reader is IEnumerable<KeyValuePair<string, string>> stringLines ? stringLines.ToKeyLines(namePolicy) :
                reader is IEnumerable<IKeyTree> trees ? trees.SelectMany(tree => tree.ToKeyLines()) :
                throw new ArgumentException("source must be key-lines, string-lines or key tree", nameof(reader));
            this.errorHandler = errorHandler;
            this.disposeReader = disposeReader;
        }

        /// <summary>
        /// Start observing file changes
        /// </summary>
        public void SubscribeObserving()
        {
            if (reader is IObservable<IAssetSourceEvent> observable)
            {
                try
                {
                    observerHandle = observable.Subscribe(this);
                }
                catch (Exception e) when (errorHandler != null && errorHandler(e))
                {
                    // Observing failed, but discard the problem as per error handler.
                }
            }
        }

        /// <summary>
        /// Dispose source information
        /// </summary>
        public virtual void Dispose(ref StructList4<Exception> errors)
        {
            var _reader = reader;
            keyLinesReader = null;
            reader = null;
            errorHandler = null;
            snapshot = null;
            parent = null;

            // Cancel observer
            try
            {
                Interlocked.CompareExchange(ref observerHandle, null, observerHandle)?.Dispose();
            }
            catch (Exception e)
            {
                errors.Add(e);
            }

            // Dispose reader
            if (disposeReader && _reader is IDisposable disposable)
                try 
                {
                    disposable.Dispose();
                } catch(Exception e)
                {
                    errors.Add(e);
                }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            StructList4<Exception> errors = new StructList4<Exception>();
            Dispose(ref errors);
            if (errors.Count > 0) throw new AggregateException(errors);
        }

        /// <summary>
        /// Get snaphost or read lines
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">On read problem that is not handled by <see cref="errorHandler"/>.</exception>
        public KeyValuePair<IAssetKey, string>[] GetLines()
        {
            // Return snapshot
            var _snapshot = snapshot;
            if (_snapshot != null) return _snapshot;

            // Read lines
            try
            {
                List<KeyValuePair<IAssetKey, string>> lines = new List<KeyValuePair<IAssetKey, string>>(lineCount < 0 ? 25 : lineCount);
                lines.AddRange(keyLinesReader);
                lineCount = lines.Count;
                return snapshot = lines.ToArray();
            } catch (Exception e) when(errorHandler!=null && errorHandler(e))
            {
                // Reading failed, but discard the problem as per error handler.
                return snapshot = new KeyValuePair<IAssetKey, string>[0];
            }
        }

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<IAssetKey, string>>) GetLines()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetLines().GetEnumerator();

        /// <summary>
        /// Asset source stopped sending events
        /// </summary>
        void IObserver<IAssetSourceEvent>.OnCompleted()
        {
            // Cancel observer
            Interlocked.CompareExchange(ref observerHandle, null, observerHandle)?.Dispose();
        }

        /// <summary>
        /// Error while monitoring asset source
        /// </summary>
        /// <param name="error"></param>
        void IObserver<IAssetSourceEvent>.OnError(Exception error)
        {
        }

        /// <summary>
        /// Source file changed.
        /// </summary>
        /// <param name="value"></param>
        void IObserver<IAssetSourceEvent>.OnNext(IAssetSourceEvent value)
        {
            if (value is IAssetChangeEvent changeEvent)
            {
                // Discard snapshot
                snapshot = null;
                // Start timer that reloads collections
                parent.StartReloadTimer();
            }
        }

        /// <summary>
        /// Print info from source reference.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => reader.ToString();

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
