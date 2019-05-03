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
    /// This class contains language strings. The key class is <see cref="ILinePart"/>.
    /// 
    /// Content is loaded from <see cref="IEnumerable{T}"/> sources when <see cref="IAssetReloadable.Reload"/> is called.
    /// </summary>
    public class LocalizationAsset :
        ILocalizationStringProvider,
        ILocalizationStringLinesEnumerable,
        ILocalizationKeyLinesEnumerable,
        ILocalizationAssetCultureCapabilities,
        IAssetReloadable,
        IDisposable,
        IAssetObservable
    {
        /// <summary>
        /// Get or load key-lines
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<ILinePart, IFormulationString> KeyLines => keyLines ?? LoadKeyLines();

        /// <summary>
        /// Get or load key-lines
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, IFormulationString> StringLines => stringLines ?? LoadStringLines();

        /// <summary>
        /// String lines sorted by name policy.
        /// </summary>
        protected virtual Dictionary<IAssetKeyNameProvider, Dictionary<string, IFormulationString>> StringLinesByProvider => stringLinesByProvider ?? LoadStringLinesByProvider();

        /// <summary>
        /// Loaded and active key lines. It is compiled union of all sources.
        /// </summary>
        protected Dictionary<ILinePart, IFormulationString> keyLines;

        /// <summary>
        /// Loaded and active string lines. It is compiled union of all sources.
        /// </summary>
        protected Dictionary<string, IFormulationString> stringLines;

        /// <summary>
        /// String lines sorted by name policy.
        /// </summary>
        protected Dictionary<IAssetKeyNameProvider, Dictionary<string, IFormulationString>> stringLinesByProvider;

        /// <summary>
        /// Collections of lines and source readers. They are read when <see cref="Load"/> is called.
        /// </summary>
        protected ConcurrentDictionary<IEnumerable, Collection> collections = new ConcurrentDictionary<IEnumerable, Collection>();

        /// <summary>
        /// Timer task that reloads content.
        /// </summary>
        protected Task reloadTask;

        /// <summary>
        /// <see cref="ILinePart"/> comparer for <see cref="keyLines"/>.
        /// </summary>
        IEqualityComparer<ILinePart> comparer;

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
        public LocalizationAsset(IEqualityComparer<ILinePart> comparer = default, Func<Exception, bool> errorHandler = null) : base()
        {
            this.comparer = comparer ?? LineComparer.Default;
            this.errorHandler = errorHandler;
            Load();
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// 
        /// <paramref name="reader"/> must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;IAssetKey, IFormulationString&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, IFormulationString&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;IAssetKey, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;IKeyTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader">initial reader</param>
        /// <param name="keyPolicy"></param>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEnumerable reader, IAssetKeyNamePolicy keyPolicy, IEqualityComparer<ILinePart> comparer = default, Func<Exception, bool> errorHandler = null) : base()
        {
            this.comparer = comparer ?? LineComparer.Default;
            this.errorHandler = errorHandler;
            Add(reader ?? throw new ArgumentNullException(nameof(reader)), keyPolicy);
            Load();
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// 
        /// <paramref name="reader"/> must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;IAssetKey, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;IKeyTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader">initial reader</param>
        /// <param name="keyPattern"></param>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEnumerable reader, string keyPattern, IEqualityComparer<ILinePart> comparer = default, Func<Exception, bool> errorHandler = null)
            : this(reader, new AssetNamePattern(keyPattern), comparer, errorHandler)
        {
        }

        /// <summary>
        /// Dispose asset.
        /// </summary>
        /// <exception cref="AggregateException">If disposing of one of the sources failed</exception>
        public virtual void Dispose()
        {
            this.culturesFetched = false;
            this.cultures = null;
            Clear();
        }

        /// <summary>
        /// Load new and changed files.
        /// </summary>
        /// <returns>this</returns>
        /// <exception cref="Exception">On any non-captured problem</exception>
        public virtual LocalizationAsset Load()
        {
            foreach (var line in collections.ToArray())
                line.Value.Load();
            this.culturesFetched = false;
            cultures = null;
            keyLines = null;
            stringLines = null;
            return this;
        }

        /// <summary>
        /// Reload all files.
        /// </summary>
        /// <returns>this</returns>
        /// <exception cref="Exception">On any non-captured problem</exception>
        public virtual IAsset Reload()
        {
            // Clear caches
            var collectionLines = collections.ToArray();
            foreach (var line in collectionLines)
                line.Value.Clear();

            cultures = null;
            culturesFetched = false;
            keyLines = null;
            stringLines = null;

            // Load content
            return Load();
        }

        /// <summary>
        /// Replaces <see cref="keyLines"/> with a new dictionary that is filled with lines from <see cref="collections"/>.
        /// </summary>
        /// <returns>new key lines</returns>
        /// <exception cref="Exception">If load fails</exception>
        protected virtual Dictionary<ILinePart, IFormulationString> LoadKeyLines()
        {
            Dictionary<ILinePart, IFormulationString> newLines = new Dictionary<ILinePart, IFormulationString>(comparer);
            foreach (var collectionsLine in collections.ToArray())
            {
                foreach (var line in collectionsLine.Value.KeyLines)
                    newLines[line.Key] = line.Value;
            }
            return this.keyLines = newLines;
        }

        /// <summary>
        /// Replaces <see cref="stringLines"/> with a new dictionary that is filled with lines from <see cref="collections"/>.
        /// </summary>
        /// <returns>new key lines</returns>
        /// <exception cref="Exception">If load fails</exception>
        protected virtual Dictionary<string, IFormulationString> LoadStringLines()
        {
            Dictionary<string, IFormulationString> newLines = new Dictionary<string, IFormulationString>();
            foreach (var collectionsLine in collections.ToArray())
            {
                foreach (var line in collectionsLine.Value.StringLines)
                    newLines[line.Key] = line.Value;
            }
            return this.stringLines = newLines;
        }

        /// <summary>
        /// Replaces <see cref="stringLines"/> with a new dictionary that is filled with lines from <see cref="collections"/>.
        /// </summary>
        /// <returns>new key lines</returns>
        /// <exception cref="Exception">If load fails</exception>
        protected virtual Dictionary<IAssetKeyNameProvider, Dictionary<string, IFormulationString>> LoadStringLinesByProvider()
        {
            Dictionary<IAssetKeyNameProvider, Dictionary<string, IFormulationString>> byProvider = new Dictionary<IAssetKeyNameProvider, Dictionary<string, IFormulationString>>();

            foreach (var collectionsLine in collections.ToArray())
            {
                Collection c = collectionsLine.Value;
                if (c.Type == CollectionType.StringLines)
                {
                    Dictionary<string, IFormulationString> newLines;
                    IAssetKeyNameProvider provider = c.namePolicy as IAssetKeyNameProvider;
                    if (provider == null) continue;
                    if (!byProvider.TryGetValue(provider, out newLines))
                        byProvider[provider] = newLines = new Dictionary<string, IFormulationString>();

                    foreach (var line in c.StringLines)
                        newLines[line.Key] = line.Value;
                } 
            }

            return this.stringLinesByProvider = byProvider;
        }

        /// <summary>
        /// Get language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public virtual IFormulationString GetString(ILinePart key)
        {
            IFormulationString result = null;
            if (KeyLines.TryGetValue(key, out result)) return result;
            foreach(var line in StringLinesByProvider)
            {
                string id = line.Key.BuildName(key);
                if (line.Value.TryGetValue(id, out result)) return result;
            }

            return null;
        }

        /// <summary>
        /// Iterate content and get supported cultures.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            if (culturesFetched) return this.cultures;

            HashSet<CultureInfo> cultures = null;
            foreach (var collectionLine in collections.ToArray())
            {
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is IAssetKeyNameParser == false && collectionLine.Value.StringLines.Length>0) return null;
                foreach (var line in collectionLine.Value.KeyLines)
                {
                    if (cultures == null) cultures = new HashSet<CultureInfo>();
                    CultureInfo ci = line.Key.GetCultureInfo() ?? rootCulture;
                    cultures.Add(ci);
                }
            }

            this.cultures = cultures?.ToArray();
            culturesFetched = true;
            return this.cultures;
        }
        bool culturesFetched;
        CultureInfo[] cultures;
        static CultureInfo rootCulture = CultureInfo.GetCultureInfo("");

        /// <summary>
        /// Get a snapshot of key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<KeyValuePair<ILinePart, IFormulationString>> GetKeyLines(ILinePart filterKey = null)
        {
            // Get snapshot
            var _lines = KeyLines;
            // Return all
            if (filterKey == null) return _lines;
            // Create filter
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            // Apply filter
            return _lines.Where(line => filter.Filter(line.Key));
        }

        /// <summary>
        /// Get a snapshot of all the key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<KeyValuePair<ILinePart, IFormulationString>> GetAllKeyLines(ILinePart filterKey = null)
            => GetKeyLines(filterKey);

        /// <summary>
        /// Get string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<string, IFormulationString>> GetStringLines(ILinePart filterKey = null)
        {
            // Return all 
            if (filterKey == null) return StringLines;
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            // Apply filter
            List<KeyValuePair<string, IFormulationString>> result = null;
            foreach (var collectionLine in collections.ToArray())
            {
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is IAssetKeyNameProvider nameProvider_ && collectionLine.Value.namePolicy is IAssetKeyNameParser nameParser_)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Filter(line.Key)).Select(line => new KeyValuePair<string, IFormulationString>(nameProvider_.BuildName(line.Key), line.Value));
                    if (result == null) result = new List<KeyValuePair<string, IFormulationString>>();
                    result.AddRange(__stringLines);
                }
                else
                if ((collectionLine.Value.Type == CollectionType.KeyLines || collectionLine.Value.Type == CollectionType.KeyTree) && collectionLine.Value.namePolicy is IAssetKeyNameProvider nameProvider)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Filter(line.Key)).Select(line => new KeyValuePair<string, IFormulationString>(nameProvider.BuildName(line.Key), line.Value));
                    if (result == null) result = new List<KeyValuePair<string, IFormulationString>>();
                    result.AddRange(__stringLines);
                }
            }
            return result;
        }

        /// <summary>
        /// Get all string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<string, IFormulationString>> GetAllStringLines(ILinePart filterKey = null)
        {
            // Return all 
            if (filterKey == null) return StringLines;
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            // Apply filter
            List<KeyValuePair<string, IFormulationString>> result = null;
            foreach (var collectionLine in collections.ToArray())
            {
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is IAssetKeyNameProvider nameProvider_ && collectionLine.Value.namePolicy is IAssetKeyNameParser nameParser_)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Filter(line.Key)).Select(line => new KeyValuePair<string, IFormulationString>(nameProvider_.BuildName(line.Key), line.Value));
                    if (result == null) result = new List<KeyValuePair<string, IFormulationString>>();
                    result.AddRange(__stringLines);
                } else 
                if ((collectionLine.Value.Type == CollectionType.KeyLines || collectionLine.Value.Type == CollectionType.KeyTree) && collectionLine.Value.namePolicy is IAssetKeyNameProvider nameProvider)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Filter(line.Key)).Select(line => new KeyValuePair<string, IFormulationString>(nameProvider.BuildName(line.Key), line.Value));
                    if (result == null) result = new List<KeyValuePair<string, IFormulationString>>();
                    result.AddRange(__stringLines);
                }
                else return null;
            }
            return result;
        }

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
        public LocalizationAsset Add(IEnumerable reader, IAssetKeyNamePolicy namePolicy = null, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
        {
            // Reader argument not null
            if (reader == null) throw new ArgumentNullException(nameof(reader));

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
        /// <param name="namePattern">name pattern that reads the content</param>
        /// <param name="errorHandler">(optional) overrides default handler.</param>
        /// <param name="disposeReader">Dispose <paramref name="reader"/> along with <see cref="LocalizationAsset"/></param>
        /// <returns></returns>
        public LocalizationAsset Add(IEnumerable reader, string namePattern, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
            => Add(reader, new AssetNamePattern(namePattern), errorHandler, disposeReader);

        /// <summary>
        /// Remove <paramref name="reader"/>. If reader was added with disposeReader, it will be disposed here.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LocalizationAsset Remove(IEnumerable reader)
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
        public LocalizationAsset Clear()
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
    /// Collection reader type
    /// </summary>
    public enum CollectionType
    {
        /// <summary>
        /// Key is string
        /// </summary>
        StringLines,

        /// <summary>
        /// Key is IAssetKey
        /// </summary>
        KeyLines,

        /// <summary>
        /// Key is IKeyTree
        /// </summary>
        KeyTree
    }

    /// <summary>
    /// Collection of lines
    /// </summary>
    public class Collection : IObserver<IAssetSourceEvent>, IEnumerable<KeyValuePair<ILinePart, IFormulationString>>, IEnumerable<KeyValuePair<string, IFormulationString>>
    {
        /// <summary>
        /// Reader, the original reference.
        /// </summary>
        internal protected IEnumerable reader;

        /// <summary>
        /// Previously loaded snapshot of key lines
        /// </summary>
        internal protected KeyValuePair<ILinePart, IFormulationString>[] keyLines;

        /// <summary>
        /// Previously loaded snapshot of key lines
        /// </summary>
        internal protected KeyValuePair<string, IFormulationString>[] stringLines;

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
        /// If source is string lines the parses into strings into <see cref="ILinePart"/>.
        /// </summary>
        protected internal IAssetKeyNamePolicy namePolicy;

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
        /// Collection type.
        /// </summary>
        public CollectionType Type;

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
            this.namePolicy = namePolicy ?? ParameterNamePolicy.Instance;
            this.errorHandler = errorHandler;
            this.disposeReader = disposeReader;

            if (reader is IEnumerable<KeyValuePair<ILinePart, IFormulationString>> keyLinesReader) this.Type = CollectionType.KeyLines;
            else if (reader is IEnumerable<IKeyTree> treesReader) this.Type = CollectionType.KeyTree;
            else if (reader is IEnumerable<KeyValuePair<string, IFormulationString>> stringLinesReader) this.Type = CollectionType.StringLines;
            else if (reader is IEnumerable<KeyValuePair<ILinePart, string>> keyLinesReader_) this.Type = CollectionType.KeyLines;
            else if (reader is IEnumerable<KeyValuePair<string, string>> stringLinesReader_) this.Type = CollectionType.StringLines;
            else throw new ArgumentException($"Cannot read from {reader.GetType().FullName}: {reader}");
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
        /// Clear cached lines
        /// </summary>
        public void Clear()
        {
            keyLines = null;
            stringLines = null;
        }

        /// <summary>
        /// Dispose source information
        /// </summary>
        public virtual void Dispose(ref StructList4<Exception> errors)
        {
            var _reader = reader;
            reader = null;
            errorHandler = null;
            keyLines = null;
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
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
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
        /// Load reader into memory, if has not already been loaded.
        /// </summary>
        public void Load()
        {
            if (reader is IEnumerable<KeyValuePair<ILinePart, IFormulationString>> || reader is IEnumerable<IKeyTree>)
            {
                var _lines = KeyLines;
            }
            else if (reader is IEnumerable<KeyValuePair<string, IFormulationString>>)
            {
                var _lines = StringLines;
            }

        }

        /// <summary>
        /// Get snapshot or read lines
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">On read problem that is not handled by <see cref="errorHandler"/>.</exception>
        public KeyValuePair<ILinePart, IFormulationString>[] KeyLines
        {
            get
            {
                // Return snapshot
                var _lines = keyLines;
                if (_lines != null) return _lines;

                // Read lines
                try
                {
                    List<KeyValuePair<ILinePart, IFormulationString>> lines = new List<KeyValuePair<ILinePart, IFormulationString>>(lineCount < 0 ? 25 : lineCount);

                    // Read as key-lines
                    if (reader is IEnumerable<KeyValuePair<ILinePart, IFormulationString>> keyLinesReader)
                    {
                        lines.AddRange(keyLinesReader);
                    }

                    // Read as tree lines
                    else if (reader is IEnumerable<IKeyTree> treesReader)
                    {
                        lines.AddRange(treesReader.SelectMany(tree => tree.ToKeyLines()));
                    }

                    // Read as string lines
                    else if (reader is IEnumerable<KeyValuePair<string, IFormulationString>> stringLinesReader)
                    {
                        // Convert from string lines
                        var _stringLines = stringLines;
                        if (_stringLines != null && namePolicy is IAssetKeyNameParser parser)
                            lines.AddRange(_stringLines.ToKeyLines(parser));
                        else
                            lines.AddRange(stringLinesReader.ToKeyLines(namePolicy));
                    }
                    // Read as key-lines
                    else if (reader is IEnumerable<KeyValuePair<ILinePart, string>> keyLinesReader_)
                    {
                        lines.AddRange(keyLinesReader_.Select(line=>new KeyValuePair<ILinePart, IFormulationString>(line.Key, LexicalStringFormat.Instance.Parse(line.Value))));
                    }
                    else if (reader is IEnumerable<KeyValuePair<string, string>> stringLinesReader_)
                    {
                        // Convert from string lines
                        var _stringLines = stringLines;
                        if (_stringLines != null && namePolicy is IAssetKeyNameParser parser)
                            lines.AddRange(_stringLines.ToKeyLines(parser));
                        else
                            lines.AddRange(stringLinesReader_.ToKeyLines(namePolicy, LexicalStringFormat.Instance));
                    }
                    else throw new ArgumentException($"Cannot read {reader.GetType().FullName}: {reader}");

                    lineCount = lines.Count;
                    return keyLines = lines.ToArray();
                }
                catch (Exception e) when (errorHandler != null && errorHandler(e))
                {
                    // Reading failed, but discard the problem as per error handler.
                    return keyLines = new KeyValuePair<ILinePart, IFormulationString>[0];
                }
            }
        }

        /// <summary>
        /// Get previously read, or read lines now
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">On read problem that is not handled by <see cref="errorHandler"/>.</exception>
        public KeyValuePair<string, IFormulationString>[] StringLines
        {
            get
            {
                // Return snapshot
                var _lines = stringLines;
                if (_lines != null) return _lines;

                // Read lines
                try
                {
                    List<KeyValuePair<string, IFormulationString>> lines = new List<KeyValuePair<string, IFormulationString>>(lineCount < 0 ? 25 : lineCount);

                    // Read as key-lines
                    if (reader is IEnumerable<KeyValuePair<string, IFormulationString>> stringLinesReader)
                    {
                        lines.AddRange(stringLinesReader);
                    }

                    // Read as tree lines
                    else if (reader is IEnumerable<IKeyTree> treesReader)
                    {
                        lines.AddRange(treesReader.SelectMany(tree => tree.ToStringLines(namePolicy)));
                    }

                    // Read as string lines
                    else if (reader is IEnumerable<KeyValuePair<ILinePart, IFormulationString>> keyLinesReader)
                    {
                        // Convert from string lines
                        var _keyLines = keyLines;
                        if (_keyLines != null && namePolicy is IAssetKeyNameProvider provider)
                            lines.AddRange(_keyLines.ToStringLines(provider));
                        else
                            lines.AddRange(keyLinesReader.ToStringLines(namePolicy));
                    }
                    else if (reader is IEnumerable<KeyValuePair<string, string>> stringLinesReader_)
                    {
                        lines.AddRange(stringLinesReader_.Select(line => new KeyValuePair<string, IFormulationString>(line.Key, LexicalStringFormat.Instance.Parse(line.Value))));
                    }
                    else if (reader is IEnumerable<KeyValuePair<ILinePart, string>> keyLinesReader_)
                    {
                        // Convert from string lines
                        var _keyLines = keyLines;
                        if (_keyLines != null && namePolicy is IAssetKeyNameProvider provider)
                            lines.AddRange(_keyLines.ToStringLines(provider));
                        else
                            lines.AddRange(keyLinesReader_.ToStringLines(namePolicy, LexicalStringFormat.Instance));
                    }
                    else throw new ArgumentException($"Cannot read {reader.GetType().FullName}: {reader}");

                    lineCount = lines.Count;
                    return stringLines = lines.ToArray();
                }
                catch (Exception e) when (errorHandler != null && errorHandler(e))
                {
                    // Reading failed, but discard the problem as per error handler.
                    return stringLines = new KeyValuePair<string, IFormulationString>[0];
                }
            }
        }

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<ILinePart, IFormulationString>> IEnumerable<KeyValuePair<ILinePart, IFormulationString>>.GetEnumerator()
            => ((IEnumerable<KeyValuePair<ILinePart, IFormulationString>>)KeyLines).GetEnumerator();

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, IFormulationString>> IEnumerable<KeyValuePair<string, IFormulationString>>.GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, IFormulationString>>)StringLines).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => KeyLines.GetEnumerator();

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
                keyLines = null;
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
        public static IAssetBuilder AddKeyLines(this IAssetBuilder builder, IEnumerable<KeyValuePair<ILinePart, IFormulationString>> lines)
        {
            builder.AddAsset(new LocalizationAsset().Add(lines).Load());
            return builder;
        }

        /// <summary>
        /// Add string dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetComposition AddKeyLines(this IAssetComposition composition, IEnumerable<KeyValuePair<ILinePart, IFormulationString>> lines)
        {
            composition.Add(new LocalizationAsset().Add(lines).Load());
            return composition;
        }
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
            builder.AddAsset(new LocalizationAsset(dictionary, namePolicy));
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
            composition.Add(new LocalizationAsset(dictionary, namePolicy));
            return composition;
        }
    }

}
