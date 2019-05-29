// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
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
    /// This class contains language strings. The key class is <see cref="ILine"/>.
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
        protected virtual Dictionary<ILine, ILine> KeyLines => keyLines ?? LoadKeyLines();

        /// <summary>
        /// Get or load key-lines
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, ILine> StringLines => stringLines ?? LoadStringLines();

        /// <summary>
        /// String lines sorted by name policy.
        /// </summary>
        protected virtual Dictionary<ILinePrinter, Dictionary<string, ILine>> StringLinesByLineFormat => stringLinesByLineFormat ?? LoadStringLinesByLineFormat();

        /// <summary>
        /// Loaded and active key lines. It is compiled union of all sources.
        /// </summary>
        protected Dictionary<ILine, ILine> keyLines;

        /// <summary>
        /// Loaded and active string lines. It is compiled union of all sources.
        /// </summary>
        protected Dictionary<string, ILine> stringLines;

        /// <summary>
        /// String lines sorted by name policy.
        /// </summary>
        protected Dictionary<ILinePrinter, Dictionary<string, ILine>> stringLinesByLineFormat;

        /// <summary>
        /// Collections of lines and source readers. They are read when <see cref="Load"/> is called.
        /// </summary>
        protected ConcurrentDictionary<IEnumerable, Collection> collections = new ConcurrentDictionary<IEnumerable, Collection>();

        /// <summary>
        /// Timer task that reloads content.
        /// </summary>
        protected Task reloadTask;

        /// <summary>
        /// <see cref="ILine"/> comparer for <see cref="keyLines"/>.
        /// </summary>
        protected IEqualityComparer<ILine> comparer;

        /// <summary>
        /// Handler that processes file load errors, and file monitoring errors.
        /// 
        /// If <see cref="errorHandler"/> returns false, or there is no handler, then exception is thrown and asset loading fails.
        /// If <see cref="errorHandler"/> returns true, then exception is caught and empty list is used.
        /// </summary>
        protected Func<Exception, bool> errorHandler;

        /// <summary>
        /// Set of resolvers that are used for resolving string based parameter into instances.
        /// </summary>
        internal protected Resolvers resolvers;

        /// <summary>
        /// Create localization asset with default properties.
        /// </summary>
        public LocalizationAsset() : base()
        {
            this.resolvers = Resolvers.Instance;
            this.comparer = LineComparer.Default;
            this.errorHandler = null;
            Load();
        }

        /// <summary>
        /// Create language string resolver that uses a dictionary as a source.
        /// </summary>
        /// <param name="resolvers">(optional) resolvers, that are used for converting parameters and keys into resolved line parts</param>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEqualityComparer<ILine> comparer, Func<Exception, bool> errorHandler = null, Resolvers resolvers = null) : base()
        {
            this.resolvers = resolvers;
            this.comparer = comparer ?? LineComparer.Default;
            this.errorHandler = errorHandler;
            Load();
        }

        /// <summary>
        /// Convenience constructor that adds one lines source <paramref name="reader"/>. 
        /// 
        /// <paramref name="reader"/> must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;ILine, IFormatString&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, IFormatString&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;ILine, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;ILineTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader">initial reader</param>
        /// <param name="lineFormat"></param>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEnumerable reader, ILineFormat lineFormat = default, IEqualityComparer<ILine> comparer = default, Func<Exception, bool> errorHandler = null) : base()
        {
            this.resolvers = Resolvers.Instance;
            this.comparer = comparer ?? LineComparer.Default;
            this.errorHandler = errorHandler;
            Add(reader ?? throw new ArgumentNullException(nameof(reader)), lineFormat);
            Load();
        }

        /// <summary>
        /// Convenience constructor that adds one lines source <paramref name="reader"/>. 
        /// 
        /// <paramref name="reader"/> must implement one of:
        /// <list type="bullet">
        /// <item>IEnumerable&gt;KeyValuePair&gt;ILine, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;ILineTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader">initial reader</param>
        /// <param name="keyPattern"></param>
        /// <param name="comparer">(optional) comparer to use</param>
        /// <param name="errorHandler">(optional) handler, if null or returns false, then exception is let to be thrown</param>
        public LocalizationAsset(IEnumerable reader, string keyPattern, IEqualityComparer<ILine> comparer = default, Func<Exception, bool> errorHandler = null)
            : this(reader, new LinePattern(keyPattern), comparer, errorHandler)
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
        protected virtual Dictionary<ILine, ILine> LoadKeyLines()
        {
            Dictionary<ILine, ILine> newLines = new Dictionary<ILine, ILine>(comparer);
            foreach (var collectionsLine in collections.ToArray())
            {
                foreach (var line in collectionsLine.Value.KeyLines)
                    newLines[line] = line;
            }
            return this.keyLines = newLines;
        }

        /// <summary>
        /// Replaces <see cref="stringLines"/> with a new dictionary that is filled with lines from <see cref="collections"/>.
        /// </summary>
        /// <returns>new key lines</returns>
        /// <exception cref="Exception">If load fails</exception>
        protected virtual Dictionary<string, ILine> LoadStringLines()
        {
            Dictionary<string, ILine> newLines = new Dictionary<string, ILine>();
            foreach (var collectionsLine in collections.ToArray())
            {
                foreach (var line in collectionsLine.Value.StringLines)
                {
                    IFormatString format = line.Value;
                    ILine value = format == null ? nullLine: new LineValue(null, null, format);
                    newLines[line.Key] = value;
                }
            }
            return this.stringLines = newLines;
        }

        static ILine nullLine = new LineValue(null, null, StatusFormatString.Null);

        /// <summary>
        /// Replaces <see cref="stringLines"/> with a new dictionary that is filled with lines from <see cref="collections"/>.
        /// </summary>
        /// <returns>new key lines</returns>
        /// <exception cref="Exception">If load fails</exception>
        protected virtual Dictionary<ILinePrinter, Dictionary<string, ILine>> LoadStringLinesByLineFormat()
        {
            Dictionary<ILinePrinter, Dictionary<string, ILine>> byLineFormat = new Dictionary<ILinePrinter, Dictionary<string, ILine>>();

            foreach (var collectionsLine in collections.ToArray())
            {
                Collection c = collectionsLine.Value;
                if (c.Type == CollectionType.StringLines)
                {
                    ILinePrinter provider = c.namePolicy as ILinePrinter;
                    if (provider == null) continue;

                    // Get-or-create dictionary
                    Dictionary<string, ILine> newLines;
                    if (!byLineFormat.TryGetValue(provider, out newLines)) byLineFormat[provider] = newLines = new Dictionary<string, ILine>();
                    
                    foreach (var line in c.StringLines)
                    {
                        IFormatString format = line.Value;
                        ILine value = format == null ? nullLine : new LineValue(null, null, format);
                        newLines[line.Key] = value;
                    }
                } 
            }

            return this.stringLinesByLineFormat = byLineFormat;
        }

        /// <summary>
        /// Get language string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        public virtual ILine GetString(ILine key)
        {
            ILine result = null;
            if (KeyLines.TryGetValue(key, out result)) return result;
            foreach(var stringLines in StringLinesByLineFormat)
            {
                // Convert line's key to string
                string id = stringLines.Key.Print(key);
                // Search with string
                ILine value;
                if (stringLines.Value.TryGetValue(id, out value)) return value;
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
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is ILineParser == false && collectionLine.Value.StringLines.Length>0) return null;
                foreach (var line in collectionLine.Value.KeyLines)
                {
                    if (cultures == null) cultures = new HashSet<CultureInfo>();
                    CultureInfo ci;
                    if (!line.TryGetCultureInfo(out ci)) ci = rootCulture;
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
        public IEnumerable<ILine> GetLines(ILine filterKey = null)
        {
            // Get snapshot
            var _lines = KeyLines;
            // Return all
            if (filterKey == null) return _lines.Values;
            // Create filter
            LineQualifier filter = new LineQualifier().Rule(filterKey) as LineQualifier;
            // Apply filter
            return _lines.Where(line => filter.Qualify(line.Key)).Select(kv=>kv.Value);
        }

        /// <summary>
        /// Get a snapshot of all the key-lines in this asset.
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>list of key-lines, or null if could not be provided</returns>
        public IEnumerable<ILine> GetAllLines(ILine filterKey = null)
            => GetLines(filterKey);

        /// <summary>
        /// Get string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<KeyValuePair<string, IFormatString>> GetStringLines(ILine filterKey = null)
        {
            // Return all 
            if (filterKey == null) return StringLines.Select(kv => new KeyValuePair<string, IFormatString>(kv.Key, kv.Value.GetValue()));
            // Create filter.
            LineQualifier filter = new LineQualifier().Rule(filterKey) as LineQualifier;
            // Apply filter
            List<KeyValuePair<string, IFormatString>> result = null;
            foreach (var collectionLine in collections.ToArray())
            {
                // Source is of string lines
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is ILinePrinter nameProvider_ && collectionLine.Value.namePolicy is ILineParser nameParser_)
                {
                    // Parse to keys and then qualify
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Qualify(line)).Select(line => new KeyValuePair<string, IFormatString>(nameProvider_.Print(line), line.GetValue()));
                    if (result == null) result = new List<KeyValuePair<string, IFormatString>>();
                    result.AddRange(__stringLines);
                }
                else
                if ((collectionLine.Value.Type == CollectionType.KeyLines || collectionLine.Value.Type == CollectionType.LineTree) && collectionLine.Value.namePolicy is ILinePrinter nameProvider)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Qualify(line)).Select(line => new KeyValuePair<string, IFormatString>(nameProvider.Print(line), line.GetValue()));
                    if (result == null) result = new List<KeyValuePair<string, IFormatString>>();
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
        public IEnumerable<KeyValuePair<string, IFormatString>> GetAllStringLines(ILine filterKey = null)
        {
            // Return all 
            if (filterKey == null) return StringLines.Select(kv => new KeyValuePair<string, IFormatString>(kv.Key, kv.Value.GetValue()));
            // Create filter.
            LineQualifier filter = new LineQualifier().Rule(filterKey) as LineQualifier;
            // Apply filter
            List<KeyValuePair<string, IFormatString>> result = null;
            foreach (var collectionLine in collections.ToArray())
            {
                if (collectionLine.Value.Type == CollectionType.StringLines && collectionLine.Value.namePolicy is ILinePrinter nameProvider_ && collectionLine.Value.namePolicy is ILineParser nameParser_)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Qualify(line)).Select(line => new KeyValuePair<string, IFormatString>(nameProvider_.Print(line), line.GetValue()));
                    if (result == null) result = new List<KeyValuePair<string, IFormatString>>();
                    result.AddRange(__stringLines);
                } else 
                if ((collectionLine.Value.Type == CollectionType.KeyLines || collectionLine.Value.Type == CollectionType.LineTree) && collectionLine.Value.namePolicy is ILinePrinter nameProvider)
                {
                    var __stringLines = collectionLine.Value.KeyLines.Where(line => filter.Qualify(line)).Select(line => new KeyValuePair<string, IFormatString>(nameProvider.Print(line), line.GetValue()));
                    if (result == null) result = new List<KeyValuePair<string, IFormatString>>();
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
        /// <item>IEnumerable&gt;KeyValuePair&gt;ILine, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;ILineTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="lineFormat">(optional) line format that converts lines to strings. Required if reader implements string lines</param>
        /// <param name="errorHandler">(optional) overrides default handler.</param>
        /// <param name="disposeReader">Dispose <paramref name="reader"/> along with <see cref="LocalizationAsset"/></param>
        /// <returns></returns>
        public LocalizationAsset Add(IEnumerable reader, ILineFormat lineFormat = null, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
        {
            // Reader argument not null
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            // Create collection
            var _errorHandler = errorHandler ?? this.errorHandler;
            Collection collection = new Collection(reader, lineFormat, _errorHandler, this, disposeReader);
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
        /// <item>IEnumerable&gt;KeyValuePair&gt;ILine, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;KeyValuePair&gt;string, string&lt;&lt;</item>
        /// <item>IEnumerable&gt;ILineTree&lt;</item>
        /// </list>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="namePattern">name pattern that reads the content</param>
        /// <param name="errorHandler">(optional) overrides default handler.</param>
        /// <param name="disposeReader">Dispose <paramref name="reader"/> along with <see cref="LocalizationAsset"/></param>
        /// <returns></returns>
        public LocalizationAsset Add(IEnumerable reader, string namePattern, Func<Exception, bool> errorHandler = null, bool disposeReader = false)
            => Add(reader, new LinePattern(namePattern), errorHandler, disposeReader);

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
        /// Key is ILine
        /// </summary>
        KeyLines,

        /// <summary>
        /// Key is ILineTree
        /// </summary>
        LineTree
    }

    /// <summary>
    /// Collection of lines
    /// </summary>
    public class Collection : IObserver<IAssetSourceEvent>, IEnumerable<ILine>, IEnumerable<KeyValuePair<string, IFormatString>>
    {
        /// <summary>
        /// Reader, the original reference.
        /// </summary>
        internal protected IEnumerable reader;

        /// <summary>
        /// Previously loaded snapshot of key lines
        /// </summary>
        internal protected ILine[] keyLines;

        /// <summary>
        /// Previously loaded snapshot of key lines
        /// </summary>
        internal protected KeyValuePair<string, IFormatString>[] stringLines;

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
        /// If source is string lines the parses into strings into <see cref="ILine"/>.
        /// </summary>
        protected internal ILineFormat namePolicy;

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
        public Collection(IEnumerable reader, ILineFormat namePolicy, Func<Exception, bool> errorHandler, LocalizationAsset parent, bool disposeReader)
        {
            this.parent = parent;
            this.reader = reader;
            this.namePolicy = namePolicy ?? LineFormat.Parameters;
            this.errorHandler = errorHandler;
            this.disposeReader = disposeReader;

            if (reader is IEnumerable<ILine> keyLinesReader) this.Type = CollectionType.KeyLines;
            else if (reader is IEnumerable<ILineTree> treesReader) this.Type = CollectionType.LineTree;
            else if (reader is IEnumerable<KeyValuePair<string, IFormatString>> stringLinesReader) this.Type = CollectionType.StringLines;
            else if (reader is IEnumerable<KeyValuePair<ILine, string>> keyLinesReader_) this.Type = CollectionType.KeyLines;
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
            if (reader is IEnumerable<ILine> || reader is IEnumerable<ILineTree>)
            {
                var _lines = KeyLines;
            }
            else if (reader is IEnumerable<KeyValuePair<string, IFormatString>>)
            {
                var _lines = StringLines;
            }

        }

        /// <summary>
        /// Get snapshot or read lines
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">On read problem that is not handled by <see cref="errorHandler"/>.</exception>
        public ILine[] KeyLines
        {
            get
            {
                // Return snapshot
                var _lines = keyLines;
                if (_lines != null) return _lines;

                // Read lines
                try
                {
                    List<ILine> lines = new List<ILine>(lineCount < 0 ? 25 : lineCount);

                    // Read as key-lines
                    if (reader is IEnumerable<ILine> keyLinesReader)
                    {
                        lines.AddRange(keyLinesReader);
                    }

                    // Read as tree lines
                    else if (reader is IEnumerable<ILineTree> treesReader)
                    {
                        lines.AddRange(treesReader.SelectMany(tree => tree.ToLines()));
                    }

                    // Read as string lines
                    else if (reader is IEnumerable<KeyValuePair<string, IFormatString>> stringLinesReader)
                    {
                        // Convert from string lines
                        var _stringLines = stringLines;
                        if (_stringLines != null && namePolicy is ILineParser parser)
                            lines.AddRange(_stringLines.ToLines(parser));
                        else
                            lines.AddRange(stringLinesReader.ToLines(namePolicy));
                    }
                    // Read as key-lines
                    else if (reader is IEnumerable<KeyValuePair<ILine, string>> keyLinesReader_)
                    {
                        lines.AddRange(keyLinesReader_.Select(line=>line.Key.Value(CSharpFormat.Instance.Parse(line.Value))));
                    }
                    else if (reader is IEnumerable<KeyValuePair<string, string>> stringLinesReader_)
                    {
                        // Convert from string lines
                        var _stringLines = stringLines;
                        if (_stringLines != null && namePolicy is ILineParser parser)
                            lines.AddRange(_stringLines.ToLines(parser));
                        else
                            lines.AddRange(stringLinesReader_.ToLines(namePolicy, CSharpFormat.Instance));
                    }
                    else throw new ArgumentException($"Cannot read {reader.GetType().FullName}: {reader}");

                    lineCount = lines.Count;
                    return keyLines = lines.ToArray();
                }
                catch (Exception e) when (errorHandler != null && errorHandler(e))
                {
                    // Reading failed, but discard the problem as per error handler.
                    return keyLines = new ILine[0];
                }
            }
        }

        /// <summary>
        /// Get previously read, or read lines now
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">On read problem that is not handled by <see cref="errorHandler"/>.</exception>
        public KeyValuePair<string, IFormatString>[] StringLines
        {
            get
            {
                // Return snapshot
                var _lines = stringLines;
                if (_lines != null) return _lines;

                // Read lines
                try
                {
                    List<KeyValuePair<string, IFormatString>> lines = new List<KeyValuePair<string, IFormatString>>(lineCount < 0 ? 25 : lineCount);

                    // Read as key-lines
                    if (reader is IEnumerable<KeyValuePair<string, IFormatString>> stringLinesReader)
                    {
                        lines.AddRange(stringLinesReader);
                    }

                    // Read as tree lines
                    else if (reader is IEnumerable<ILineTree> treesReader)
                    {
                        lines.AddRange(treesReader.SelectMany(tree => tree.ToStringLines(namePolicy)));
                    }

                    // Read as string lines
                    else if (reader is IEnumerable<ILine> keyLinesReader)
                    {
                        // Convert from string lines
                        var _keyLines = keyLines;
                        if (_keyLines != null && namePolicy is ILinePrinter provider)
                            lines.AddRange(_keyLines.ToStringLines(provider));
                        else
                            lines.AddRange(keyLinesReader.ToStringLines(namePolicy));
                    }
                    else if (reader is IEnumerable<KeyValuePair<string, string>> stringLinesReader_)
                    {
                        lines.AddRange(stringLinesReader_.Select(line => new KeyValuePair<string, IFormatString>(line.Key, CSharpFormat.Instance.Parse(line.Value))));
                    }
                    else if (reader is IEnumerable<KeyValuePair<ILine, string>> keyLinesReader_)
                    {
                        // Convert from string lines
                        var _keyLines = keyLines;
                        if (_keyLines != null && namePolicy is ILinePrinter provider)
                            lines.AddRange(_keyLines.ToStringLines(provider));
                        else
                            lines.AddRange(keyLinesReader_.ToStringLines(namePolicy, CSharpFormat.Instance));
                    }
                    else throw new ArgumentException($"Cannot read {reader.GetType().FullName}: {reader}");

                    lineCount = lines.Count;
                    return stringLines = lines.ToArray();
                }
                catch (Exception e) when (errorHandler != null && errorHandler(e))
                {
                    // Reading failed, but discard the problem as per error handler.
                    return stringLines = new KeyValuePair<string, IFormatString>[0];
                }
            }
        }

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        IEnumerator<ILine> IEnumerable<ILine>.GetEnumerator()
            => ((IEnumerable<ILine>)KeyLines).GetEnumerator();

        /// <summary>
        /// Read source, or return already read snapshot.
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, IFormatString>> IEnumerable<KeyValuePair<string, IFormatString>>.GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, IFormatString>>)StringLines).GetEnumerator();

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
        public static IAssetBuilder AddKeyLines(this IAssetBuilder builder, IEnumerable<ILine> lines)
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
        public static IAssetComposition AddKeyLines(this IAssetComposition composition, IEnumerable<ILine> lines)
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
        public static IAssetBuilder AddStrings(this IAssetBuilder builder, IReadOnlyDictionary<string, string> dictionary, ILineFormat namePolicy)
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
        public static IAssetComposition AddStrings(this IAssetComposition composition, IReadOnlyDictionary<string, string> dictionary, ILineFormat namePolicy)
        {
            composition.Add(new LocalizationAsset(dictionary, namePolicy));
            return composition;
        }
    }

}
