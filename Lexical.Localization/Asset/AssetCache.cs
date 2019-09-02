// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Lexical.Localization.Internal;
using Lexical.Localization.Resource;
using Lexical.Localization.StringFormat;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Bases for cache. Individual cache features for different interfaces need to be added separately.
    /// </summary>
    public class AssetCache : AssetComposition, IAssetCache
    {
        /// <summary>
        /// Cache options.
        /// </summary>
        public AssetCacheOptions Options { get; }

        /// <summary>
        /// Source asset that is cached.
        /// </summary>
        public IAsset Source { get; internal set; }

        /// <summary>
        /// If true, then <see cref="Source"/> is disposed along with this object.
        /// </summary>
        protected bool disposeSource;

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose requests are cached.</param>
        /// <param name="disposeSource">Disposes <paramref name="source"/> along with cache</param>
        public AssetCache(IAsset source, bool disposeSource = true)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            this.disposeSource = disposeSource;
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        public AssetCache(IAsset source, params IAssetCachePart[] cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            this.disposeSource = true;
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        /// <param name="disposeSource">Disposes <paramref name="source"/> along with cache</param>
        public AssetCache(IAsset source, bool disposeSource = true, params IAssetCachePart[] cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            this.disposeSource = disposeSource;
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="disposeSource">Disposes <paramref name="source"/> along with cache</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        public AssetCache(IAsset source, bool disposeSource, IEnumerable<IAssetCachePart> cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            this.disposeSource = disposeSource;
        }

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose reuqests are cached.</param>
        /// <param name="cacheParts">parts that handle interface specific properties, such as <see cref="AssetCachePartResources"/>, AssetStringsCachePart and AssetCulturesCachePart</param>
        public AssetCache(IAsset source, IEnumerable<IAssetCachePart> cacheParts) : base(cacheParts)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
            this.disposeSource = true;
        }

        /// <summary>
        /// Dispose cache parts and possibly <see cref="Source"/>.
        /// </summary>
        /// <param name="errors"></param>
        protected override void Dispose(ref StructList4<Exception> errors)
        {
            // Dispose cache parts
            base.Dispose(ref errors);
            try
            {
                // Dispose source
                if (disposeSource) Source.Dispose();
            }
            catch(Exception e)
            {
                // Add error
                errors.Add(e);
            }
        }

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary>
    /// Add <see cref="AssetCacheSource"/> to <see cref="IAssetBuilder"/> to have it add cache instances on new asset builds.
    /// </summary>
    public class AssetCacheSource : IAssetSource
    {
        Action<IAssetCache> configurer;

        /// <summary>
        /// Create source that adds cache
        /// </summary>
        /// <param name="configurer"></param>
        public AssetCacheSource(Action<IAssetCache> configurer)
        {
            this.configurer = configurer;
        }

        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        public void Build(IList<IAsset> list)
        {
        }

        /// <summary>
        /// Wrap asset into cache.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IAsset PostBuild(IAsset asset)
        {
            AssetCache cache = new AssetCache(asset);
            configurer(cache);
            return cache;
        }
    }

    /// <summary>
    /// Cache part that caches results of <see cref="IAssetCultureEnumerable" />.
    /// </summary>
    public class AssetCachePartCultures : IAssetCachePart, IAssetCultureEnumerable, IAssetReloadable, IDisposable
    {
        static CultureInfo[] empty_cultures = new CultureInfo[0];
        static CultureInfo NO_CULTURE = CultureInfo.GetCultureInfo("");
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Source asset that this is cache of.
        /// </summary>
        public IAsset Source { get; protected set; }

        /// <summary>
        /// Cache options.
        /// </summary>
        public AssetCacheOptions Options { get; internal set; }

        /// <summary>
        /// Version.
        /// </summary>
        protected volatile int iteration;

        CultureInfo[] cultures;
        bool culturesCached;

        /// <summary>
        /// Create part that caches cultures.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        public AssetCachePartCultures(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        IAsset IAssetReloadable.Reload()
        {
            Source.Reload();
            iteration++;

            m_lock.EnterWriteLock();
            try
            {
                cultures = null;
                culturesCached = false;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return this;
        }

        /// <summary>
        /// Get cultures from source and cache result until invalidated.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (culturesCached) return _cultures;
            int iter = iteration;
            _cultures = Source.GetSupportedCultures()?.ToArray();

            m_lock.EnterWriteLock();
            try
            {
                if (iter == iteration)
                {
                    cultures = _cultures;
                    culturesCached = true;
                }
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
            return _cultures;
        }

        /// <summary>
        /// Print into
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";

        /// <summary>
        /// Dispose cache
        /// </summary>
        public void Dispose()
        {
            Source = null;
            cultures = null;
        }
    }

    /// <summary>
    /// Cache part that caches calls to <see cref="IStringAssetLinesEnumerable" /> and <see cref="IStringAsset"/>.
    /// </summary>
    public class AssetCachePartStrings : IAssetCachePart, IStringAssetLinesEnumerable, IStringAssetUnformedLinesEnumerable, IStringAsset, IAssetReloadable, IDisposable
    {
        /// <summary>
        /// Source asset that this is cache of.
        /// </summary>
        public IAsset Source { get; internal set; }

        /// <summary>
        /// Cache options.
        /// </summary>
        public AssetCacheOptions Options { get; internal set; }

        LineComparer comparer;

        /// <summary>
        /// Cache that is discarded when <see cref="IAssetReloadable.Reload"/> is called.
        /// </summary>
        Cache cache;

        class Cache
        {
            /// <summary>
            /// Lock 
            /// </summary>
            public ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

            /// <summary>
            /// Cached result of GetKeyLines(null)
            /// </summary>
            public List<ILine> keysLinesPartial;

            /// <summary>
            /// Cached result of GetAllKeyLines(null)
            /// </summary>
            public List<ILine> keysLinesAll;

            /// <summary>
            /// Cached result of individual GetString() fetches
            /// </summary>
            public Dictionary<ILine, ILine> strings;

            /// <summary>
            /// GetKeyLines(null) was read and it was null.
            /// </summary>
            public bool keyLinesPartialIsNull;

            /// <summary>
            /// GetAllKeyLines(null) was read and it was null.
            /// </summary>
            public bool keyLinesAllIsNull;

            /// <summary>
            /// Cached result of GetKeyLines(null)
            /// </summary>
            public List<KeyValuePair<string, IString>> stringLinesPartial;

            /// <summary>
            /// Cached result of GetAllKeyLines(null)
            /// </summary>
            public List<KeyValuePair<string, IString>> stringLinesAll;

            /// <summary>
            /// GetKeyLines(null) was read and it was null.
            /// </summary>
            public bool stringLinesPartialIsNull;

            /// <summary>
            /// GetAllKeyLines(null) was read and it was null.
            /// </summary>
            public bool stringLinesAllIsNull;

            public Cache(LineComparer comparer)
            {
                this.strings = new Dictionary<ILine, ILine>(comparer);
            }
        }

        /// <summary>
        /// Create strings cache
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        public AssetCachePartStrings(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.comparer = LineComparer.Default;
            this.cache = new Cache(comparer);
        }

        /// <summary>
        /// Flush cache
        /// </summary>
        /// <returns></returns>
        public IAsset Reload()
        {
            // Discard previous cache
            this.cache = new Cache(comparer);
            // Reload source
            Source.Reload();
            return this;
        }

        /// <summary>
        /// Get-and-cache string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ILine GetLine(ILine key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            ILine value = null;
            _cache.m_lock.EnterReadLock();
            try
            {
                if (_cache.strings.TryGetValue(key, out value)) return value;
            }
            finally
            {
                _cache.m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            value = Source.GetLine(key);

            // Write to cache, be that null or not
            ILine cacheKey = key.CloneKey(LineAppender.NonResolving);
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.strings[cacheKey] = value;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            return value;
        }

        /// <summary>
        /// Get partial key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetLines(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysLinesPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keyLinesPartialIsNull) return null;

            // Read from source
            IEnumerable<ILine> lines = Source.GetLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.keyLinesPartialIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => line.CloneKey(LineAppender.NonResolving));

            // Take snapshot
            lines = new List<ILine>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysLinesPartial = (List<ILine>)lines;
                foreach (var line in lines)
                    _cache.strings[line] = line;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Get all key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllLines(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysLinesAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keyLinesAllIsNull) return null;

            // Read from source
            IEnumerable<ILine> lines = Source.GetAllLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.keyLinesAllIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => line.CloneKey(LineAppender.NonResolving));

            // Take snapshot
            lines = new List<ILine>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysLinesAll = (List<ILine>)lines;
                foreach (var line in lines)
                    _cache.strings[line] = line;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }


        /// <summary>
        /// Get partial key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> GetUnformedLines(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetUnformedLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.stringLinesPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.stringLinesPartialIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<string, IString>> lines = Source.GetUnformedLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.stringLinesPartialIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<KeyValuePair<string, IString>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.stringLinesPartial = (List<KeyValuePair<string, IString>>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Get all key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> GetAllUnformedLines(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllUnformedLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.stringLinesAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.stringLinesAllIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<string, IString>> lines = Source.GetAllUnformedLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.stringLinesAllIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<KeyValuePair<string, IString>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.stringLinesAll = (List<KeyValuePair<string, IString>>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Discard cached content.
        /// </summary>
        public void Dispose()
        {
            Source = null;
            this.cache = null;
        }

        /// <summary>
        /// Print cache part name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary>
    /// Cache part that caches the results of <see cref="IResourceAssetKeysEnumerable"/>, <see cref="IResourceAssetNamesEnumerable"/> and <see cref="IResourceAsset"/>.
    /// </summary>
    public class AssetCachePartResources : IAssetCachePart, IResourceAssetKeysEnumerable, IResourceAssetNamesEnumerable, IResourceAsset, IAssetReloadable, IDisposable
    {
        /// <summary>
        /// Source this is cache of
        /// </summary>
        public IAsset Source { get; internal set; }

        /// <summary>
        /// Options
        /// </summary>
        public AssetCacheOptions Options { get; internal set; }

        LineComparer comparer;

        /// <summary>
        /// Cache that is discarded when <see cref="IAssetReloadable.Reload"/> is called.
        /// </summary>
        Cache cache;

        class Cache
        {
            /// <summary>
            /// Lock 
            /// </summary>
            public ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

            /// <summary>
            /// Cached result of GetResourceKeys(null)
            /// </summary>
            public List<ILine> keysPartial;

            /// <summary>
            /// Cached result of GetAllResourceKeys(null)
            /// </summary>
            public List<ILine> keysAll;

            /// <summary>
            /// Cached result of GetResourceNames(null)
            /// </summary>
            public List<string> namesPartial;

            /// <summary>
            /// Cached result of GetAllResourceNames(null)
            /// </summary>
            public List<string> namesAll;

            /// <summary>
            /// Cached result of individual GetString() fetches
            /// </summary>
            public Dictionary<ILine, LineResourceBytes> data;

            /// <summary>
            /// GetResourceKeys(null) was read and it was null.
            /// </summary>
            public bool keysPartialIsNull;

            /// <summary>
            /// GetAllResourceKeys(null) was read and it was null.
            /// </summary>
            public bool keysAllIsNull;

            /// <summary>
            /// GetResourceNames(null) was read and it was null.
            /// </summary>
            public bool namesPartialIsNull;

            /// <summary>
            /// GetAllResourceNames(null) was read and it was null.
            /// </summary>
            public bool namesAllIsNull;

            public Cache(LineComparer comparer)
            {
                this.data = new Dictionary<ILine, LineResourceBytes>(comparer);
            }
        }

        /// <summary>
        /// Resources cache
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        public AssetCachePartResources(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.comparer = LineComparer.Default;
            this.cache = new Cache(comparer);
        }

        /// <summary>
        /// Flush cache
        /// </summary>
        /// <returns></returns>
        public IAsset Reload()
        {
            // Discard previous cache
            this.cache = new Cache(comparer);
            // Reload source
            Source.Reload();
            return this;
        }

        /// <summary>
        /// Get resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineResourceBytes GetResourceBytes(ILine key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            LineResourceBytes value = default;
            _cache.m_lock.EnterReadLock();
            try
            {
                if (_cache.data.TryGetValue(key, out value)) return value;
            }
            finally
            {
                _cache.m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            value = Source.GetResourceBytes(key);

            // Write to cache, be that null or not
            if (value.Value != null && value.Value.Length <= Options.GetMaxResourceSize())
            {
                ILine cacheKey = key.CloneKey(LineAppender.NonResolving);
                _cache.m_lock.EnterWriteLock();
                try
                {
                    _cache.data[cacheKey] = value;
                }
                finally
                {
                    _cache.m_lock.ExitWriteLock();
                }
            }

            return value;
        }

        /// <summary>
        /// Open stream
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineResourceStream GetResourceStream(ILine key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            LineResourceBytes value = default;
            _cache.m_lock.EnterReadLock();
            try
            {
                if (_cache.data.TryGetValue(key, out value))
                {
                    if (value.Value!=null)
                        return new LineResourceStream(value.Line, value.Value == null ? null : new MemoryStream(value.Value), value.Exception, value.Status);
                }
            }
            finally
            {
                _cache.m_lock.ExitReadLock();
            }

            // Open stream
            LineResourceStream stream = Source.GetResourceStream(key);

            // Store into cache?
            if (Options.GetCacheStreams())
            {
                ILine cacheKey = Options.GetCloneKeys() ? key.CloneKey(LineAppender.NonResolving) : key;

                // Cache null value
                if (stream.Value == null)
                {
                    _cache.m_lock.EnterWriteLock();
                    try
                    {
                        _cache.data[cacheKey] = new LineResourceBytes(value.Line, value.Exception, value.Status);
                        return stream;
                    }
                    finally
                    {
                        _cache.m_lock.ExitWriteLock();
                    }
                }

                // Read stream completely and then cache it
                long position = -1;
                int ix = 0;
                try
                {
                    // Try to read stream length, if fails, throws an exception
                    long len = stream.Value.Length;

                    if (len < Options.GetMaxResourceSize())
                    {
                        // Try to read position.
                        position = stream.Value.Position;

                        // Try to read stream completely.
                        int len_ = (int)len;
                        byte[] data = new byte[len];

                        // Read chunks
                        while (ix < len_)
                        {
                            int count = stream.Value.Read(data, ix, len_ - ix);

                            // 
                            // "returns zero (0) if the end of the stream has been reached."
                            //     
                            if (count == 0) break;

                            ix += count;
                        }

                        // Write data to cache
                        if (ix == len_)
                        {
                            _cache.m_lock.EnterWriteLock();
                            try
                            {
                                _cache.data[cacheKey] = new LineResourceBytes(value.Line, data, value.Status);
                                // Wrap to new stream.
                                return new LineResourceStream(value.Line, new MemoryStream(data), value.Status);
                            }
                            finally
                            {
                                _cache.m_lock.ExitWriteLock();
                            }
                        }
                        else
                        {
                            // Reading completely failed, revert position
                            stream.Value.Position = position;
                            ix = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        // Return position. 
                        if (position > 0L) { stream.Value.Position = position; ix = 0; }
                    }
                    catch (Exception)
                    {
                        // Failed to rewind stream.
                    }

                    // Stream has not been rewound. Let's open it again.
                    if (ix > 0)
                    {
                        stream.Value.Dispose();
                        return Source.GetResourceStream(key);
                    }
                }
            }
            return new LineResourceStream(key, (Exception)null, LineStatus.ResolveFailedNoResult);
        }

        /// <summary>
        /// Get partial key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetResourceKeys(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetResourceKeys(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keysPartialIsNull) return null;

            // Read from source
            IEnumerable<ILine> lines = Source.GetResourceKeys(null);

            // Got no results
            if (lines == null)
            {
                _cache.keysPartialIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => line.CloneKey(LineAppender.NonResolving));

            // Take snapshot
            lines = new List<ILine>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysPartial = (List<ILine>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Get all key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllResourceKeys(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllResourceKeys(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keysAllIsNull) return null;

            // Read from source
            IEnumerable<ILine> lines = Source.GetAllResourceKeys(null);

            // Got no results
            if (lines == null)
            {
                _cache.keysAllIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => line.CloneKey(LineAppender.NonResolving));

            // Take snapshot
            lines = new List<ILine>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysAll = (List<ILine>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Get partial key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<string> GetResourceNames(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetResourceNames(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.namesPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.namesPartialIsNull) return null;

            // Read from source
            IEnumerable<string> lines = Source.GetResourceNames(null);

            // Got no results
            if (lines == null)
            {
                _cache.namesPartialIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<string>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.namesPartial = (List<string>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Get all key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllResourceNames(ILine key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllResourceNames(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.namesAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.namesAllIsNull) return null;

            // Read from source
            IEnumerable<string> lines = Source.GetAllResourceNames(null);

            // Got no results
            if (lines == null)
            {
                _cache.namesAllIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<string>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.namesAll = (List<string>)lines;
            }
            finally
            {
                _cache.m_lock.ExitWriteLock();
            }

            // Return the snapshot
            return lines;
        }

        /// <summary>
        /// Discard cached content.
        /// </summary>
        public void Dispose()
        {
            Source = null;
            cache = null;
        }

        /// <summary>
        /// Print cache part name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary></summary>
    public static partial class LocalizationCacheExtensions
    {
        /// <summary>
        /// Add part that caches GetCultures
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static IAssetCache AddCulturesCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartCultures(cache.Source, cache.Options));
            return cache;
        }

        /// <summary>
        /// Add part that caches strings
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static IAssetCache AddStringsCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartStrings(cache.Source, cache.Options));
            return cache;
        }

        /// <summary>
        /// Key to option to clone keys
        /// </summary>
        public const string Key_CloneKeys = "CloneKeys";

        /// <summary>
        /// Set whether cache should create clones of keys, or whether to use the keys that come from requests in its cache structures.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cloneKeys"></param>
        /// <returns></returns>
        public static AssetCacheOptions SetCloneKeys(this AssetCacheOptions options, bool cloneKeys)
        {
            options.Set<bool>(Key_CloneKeys, cloneKeys);
            return options;
        }

        /// <summary>
        /// Get policy whether cache should create clones of keys, or whether it should use the keys that come from requests.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool GetCloneKeys(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_CloneKeys, out value) && value is bool _value) return _value;
            return true;
        }

        /// <summary>
        /// Add cache part that caches binary resources
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static IAssetCache AddResourceCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartResources(cache.Source, cache.Options));
            return cache;
        }

        /// <summary>
        /// Key to option for maximum resource count
        /// </summary>
        public const string Key_MaxResourceCount = "MaxResourceCount";

        /// <summary>
        /// Set maximum resource count
        /// </summary>
        /// <param name="options"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static AssetCacheOptions SetMaxResourceCount(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceCount, newValue);
            return options;
        }

        /// <summary>
        /// Get maximum resource option
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int GetMaxResourceCount(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceCount, out value) && value is int _value) return _value;
            return Int32.MaxValue;
        }

        /// <summary>
        /// Key to option for maximum resource size
        /// </summary>
        public const string Key_MaxResourceSize = "MaxResourceSize";

        /// <summary>
        /// Set maximum resource size
        /// </summary>
        /// <param name="options"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static AssetCacheOptions SetMaxResourceSize(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceSize, newValue);
            return options;
        }

        /// <summary>
        /// Get maximum resource size
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int GetMaxResourceSize(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceSize, out value) && value is int _value) return _value;
            return 4096;
        }

        /// <summary>
        /// Key to option for maximum resource total size
        /// </summary>
        public const string Key_MaxResourceTotalSize = "MaxResourceTotalSize";

        /// <summary>
        /// Set maximum resource total size
        /// </summary>
        /// <param name="options"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static AssetCacheOptions SetMaxResourceTotalSize(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceTotalSize, newValue);
            return options;
        }

        /// <summary>
        /// Get maximum resource total size option
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int GetMaxResourceTotalSize(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceTotalSize, out value) && value is int _value) return _value;
            return 1024 * 1024;
        }

        /// <summary>
        /// Key to option to cache streams as bytes
        /// </summary>
        public const string Key_CacheStreams = "CacheStreams";

        /// <summary>
        /// Set option for cache streams
        /// </summary>
        /// <param name="options"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static AssetCacheOptions SetCacheStreams(this AssetCacheOptions options, bool newValue)
        {
            options.Set<bool>(Key_CacheStreams, newValue);
            return options;
        }

        /// <summary>
        /// Get options for cache streams
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool GetCacheStreams(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_CacheStreams, out value) && value is bool _value) return _value;
            return true;
        }

    }

}
