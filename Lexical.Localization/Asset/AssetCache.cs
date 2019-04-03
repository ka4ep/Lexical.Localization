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
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Bases for cache. Individual cache features for different interfaces need to be added separately.
    /// </summary>
    public class AssetCache : AssetComposition, IAssetCache
    {
        public AssetCacheOptions Options { get; }
        public IAsset Source { get; internal set; }

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

        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary>
    /// Add <see cref="AssetCacheSource"/> to <see cref="IAssetBuilder"/> to have it add cache instances on new asset builds.
    /// </summary>
    public class AssetCacheSource : IAssetSource
    {
        Action<IAssetCache> configurer;

        public AssetCacheSource(Action<IAssetCache> configurer)
        {
            this.configurer = configurer;
        }

        public void Build(IList<IAsset> list)
        {
        }

        public IAsset PostBuild(IAsset asset)
        {
            AssetCache cache = new AssetCache(asset);
            configurer(cache);
            return cache;
        }
    }

    /// <summary>
    /// Cache part that caches results of <see cref="ILocalizationAssetCultureCapabilities" />.
    /// </summary>
    public class AssetCachePartCultures : IAssetCachePart, ILocalizationAssetCultureCapabilities, IAssetReloadable, IDisposable
    {
        static CultureInfo[] empty_cultures = new CultureInfo[0];
        static CultureInfo NO_CULTURE = CultureInfo.GetCultureInfo("");
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        public IAsset Source { get; protected set; }
        public AssetCacheOptions Options { get; internal set; }
        protected volatile int iteration;

        CultureInfo[] cultures;
        bool culturesCached;

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

        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";

        public void Dispose()
        {
            Source = null;
            cultures = null;
        }
    }

    /// <summary>
    /// Cache part that caches calls to <see cref="ILocalizationKeyLinesEnumerable" /> and <see cref="ILocalizationStringProvider"/>.
    /// </summary>
    public class AssetCachePartStrings : IAssetCachePart, ILocalizationKeyLinesEnumerable, ILocalizationStringLinesEnumerable, ILocalizationStringProvider, IAssetReloadable, IDisposable
    {
        public IAsset Source { get; internal set; }
        public AssetCacheOptions Options { get; internal set; }

        AssetKeyComparer comparer;
        AssetKeyCloner cloner;

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
            public List<KeyValuePair<IAssetKey, string>> keysLinesPartial;

            /// <summary>
            /// Cached result of GetAllKeyLines(null)
            /// </summary>
            public List<KeyValuePair<IAssetKey, string>> keysLinesAll;

            /// <summary>
            /// Cached result of individual GetString() fetches
            /// </summary>
            public Dictionary<IAssetKey, string> strings;

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
            public List<KeyValuePair<string, string>> stringLinesPartial;

            /// <summary>
            /// Cached result of GetAllKeyLines(null)
            /// </summary>
            public List<KeyValuePair<string, string>> stringLinesAll;

            /// <summary>
            /// GetKeyLines(null) was read and it was null.
            /// </summary>
            public bool stringLinesPartialIsNull;

            /// <summary>
            /// GetAllKeyLines(null) was read and it was null.
            /// </summary>
            public bool stringLinesAllIsNull;

            public Cache(AssetKeyComparer comparer)
            {
                this.strings = new Dictionary<IAssetKey, string>(comparer);
            }
        }

        public AssetCachePartStrings(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.cloner = new AssetKeyCloner(Key.Root);
            this.comparer = AssetKeyComparer.Default;
            this.cache = new Cache(comparer);
        }

        public IAsset Reload()
        {
            // Discard previous cache
            this.cache = new Cache(comparer);
            // Reload source
            Source.Reload();
            return this;
        }

        public string GetString(IAssetKey key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            string value = null;
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
            value = Source.GetString(key);

            // Write to cache, be that null or not
            IAssetKey cacheKey = cloner.Copy(key);
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
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetKeyLines(IAssetKey key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetKeyLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysLinesPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keyLinesPartialIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = Source.GetKeyLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.keyLinesPartialIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => new KeyValuePair<IAssetKey, string>(cloner.Copy(line.Key), line.Value));

            // Take snapshot
            lines = new List<KeyValuePair<IAssetKey, string>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysLinesPartial = (List<KeyValuePair<IAssetKey, string>>)lines;
                foreach (var line in lines)
                    _cache.strings[line.Key] = line.Value;
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
        public IEnumerable<KeyValuePair<IAssetKey, string>> GetAllKeyLines(IAssetKey key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllKeyLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.keysLinesAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.keyLinesAllIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = Source.GetAllKeyLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.keyLinesAllIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => new KeyValuePair<IAssetKey, string>(cloner.Copy(line.Key), line.Value));

            // Take snapshot
            lines = new List<KeyValuePair<IAssetKey, string>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysLinesAll = (List<KeyValuePair<IAssetKey, string>>)lines;
                foreach (var line in lines)
                    _cache.strings[line.Key] = line.Value;
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
        public IEnumerable<KeyValuePair<string, string>> GetStringLines(IAssetKey key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetStringLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.stringLinesPartial;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.stringLinesPartialIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<string, string>> lines = Source.GetStringLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.stringLinesPartialIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<KeyValuePair<string, string>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.stringLinesPartial = (List<KeyValuePair<string, string>>)lines;
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
        public IEnumerable<KeyValuePair<string, string>> GetAllStringLines(IAssetKey key = null)
        {
            // Filtered queries are not cached
            if (key != null) return Source.GetAllStringLines(key);

            // Get cache instance
            Cache _cache = this.cache;

            // Return previously cached list
            var _cachedList = _cache.stringLinesAll;
            if (_cachedList != null) return _cachedList;

            // Previous read returned null
            if (_cache.stringLinesAllIsNull) return null;

            // Read from source
            IEnumerable<KeyValuePair<string, string>> lines = Source.GetAllStringLines(null);

            // Got no results
            if (lines == null)
            {
                _cache.stringLinesAllIsNull = true;
                return null;
            }

            // Take snapshot
            lines = new List<KeyValuePair<string, string>>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.stringLinesAll = (List<KeyValuePair<string, string>>)lines;
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
    /// Cache part that caches the results of <see cref="IAssetResourceKeysEnumerable"/>, <see cref="IAssetResourceNamesEnumerable"/> and <see cref="IAssetResourceProvider"/>.
    /// </summary>
    public class AssetCachePartResources : IAssetCachePart, IAssetResourceKeysEnumerable, IAssetResourceNamesEnumerable, IAssetResourceProvider, IAssetReloadable, IDisposable
    {
        public IAsset Source { get; internal set; }
        public AssetCacheOptions Options { get; internal set; }

        AssetKeyComparer comparer;
        AssetKeyCloner cloner;

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
            public List<IAssetKey> keysPartial;

            /// <summary>
            /// Cached result of GetAllResourceKeys(null)
            /// </summary>
            public List<IAssetKey> keysAll;

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
            public Dictionary<IAssetKey, byte[]> data;

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

            public Cache(AssetKeyComparer comparer)
            {
                this.data = new Dictionary<IAssetKey, byte[]>(comparer);
            }
        }

        public AssetCachePartResources(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.cloner = new AssetKeyCloner(Key.Root);
            this.comparer = AssetKeyComparer.Default;
            this.cache = new Cache(comparer);
        }

        public IAsset Reload()
        {
            // Discard previous cache
            this.cache = new Cache(comparer);
            // Reload source
            Source.Reload();
            return this;
        }

        public byte[] GetResource(IAssetKey key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            byte[] value = null;
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
            value = Source.GetResource(key);

            // Write to cache, be that null or not
            if (value != null && value.Length <= Options.GetMaxResourceSize())
            {
                IAssetKey cacheKey = cloner.Copy(key);
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
        public Stream OpenStream(IAssetKey key)
        {
            Cache _cache = this.cache;

            // Try to read previously cached value
            byte[] value = null;
            _cache.m_lock.EnterReadLock();
            try
            {
                if (_cache.data.TryGetValue(key, out value))
                    return new MemoryStream(value);
            }
            finally
            {
                _cache.m_lock.ExitReadLock();
            }

            // Open stream
            Stream stream = Source.OpenStream(key);

            // Store into cache?
            if (Options.GetCacheStreams())
            {
                IAssetKey cacheKey = Options.GetCloneKeys() ? cloner.Copy(key) : key;

                // Cache null value
                if (stream == null)
                {
                    _cache.m_lock.EnterWriteLock();
                    try
                    {
                        _cache.data[cacheKey] = null;
                        return null;
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
                    long len = stream.Length;

                    if (len < Options.GetMaxResourceSize())
                    {
                        // Try to read position.
                        position = stream.Position;

                        // Try to read stream completely.
                        int len_ = (int)len;
                        byte[] data = new byte[len];

                        // Read chunks
                        while (ix < len_)
                        {
                            int count = stream.Read(data, ix, len_ - ix);

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
                                _cache.data[cacheKey] = data;
                                // Wrap to new stream.
                                return new MemoryStream(data);
                            }
                            finally
                            {
                                _cache.m_lock.ExitWriteLock();
                            }
                        }
                        else
                        {
                            // Reading completely failed, revert position
                            stream.Position = position;
                            ix = 0;
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        // Return position. 
                        if (position > 0L) { stream.Position = position; ix = 0; }
                    }
                    catch (Exception)
                    {
                        // Failed to rewind stream.
                    }

                    // Stream has not been rewound. Let's open it again.
                    if (ix > 0)
                    {
                        stream.Dispose();
                        return Source.OpenStream(key);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get partial key-lines and cache result, or return already cached lines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<IAssetKey> GetResourceKeys(IAssetKey key = null)
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
            IEnumerable<IAssetKey> lines = Source.GetResourceKeys(null);

            // Got no results
            if (lines == null)
            {
                _cache.keysPartialIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => cloner.Copy(line));

            // Take snapshot
            lines = new List<IAssetKey>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysPartial = (List<IAssetKey>)lines;
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
        public IEnumerable<IAssetKey> GetAllResourceKeys(IAssetKey key = null)
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
            IEnumerable<IAssetKey> lines = Source.GetAllResourceKeys(null);

            // Got no results
            if (lines == null)
            {
                _cache.keysAllIsNull = true;
                return null;
            }

            // Clone keys
            if (Options.GetCloneKeys()) lines = lines.Select(line => cloner.Copy(line));

            // Take snapshot
            lines = new List<IAssetKey>(lines);

            // Write to cache
            _cache.m_lock.EnterWriteLock();
            try
            {
                _cache.keysAll = (List<IAssetKey>)lines;
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
        public IEnumerable<string> GetResourceNames(IAssetKey key = null)
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
        public IEnumerable<string> GetAllResourceNames(IAssetKey key = null)
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

    public static partial class LocalizationCacheExtensions
    {
        public static IAssetCache AddCulturesCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartCultures(cache.Source, cache.Options));
            return cache;
        }
        public static IAssetCache AddStringsCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartStrings(cache.Source, cache.Options));
            return cache;
        }

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

        public static IAssetCache AddResourceCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartResources(cache.Source, cache.Options));
            return cache;
        }

        public const string Key_MaxResourceCount = "MaxResourceCount";
        public static AssetCacheOptions SetMaxResourceCount(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceCount, newValue);
            return options;
        }
        public static int GetMaxResourceCount(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceCount, out value) && value is int _value) return _value;
            return Int32.MaxValue;
        }

        public const string Key_MaxResourceSize = "MaxResourceSize";
        public static AssetCacheOptions SetMaxResourceSize(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceSize, newValue);
            return options;
        }
        public static int GetMaxResourceSize(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceSize, out value) && value is int _value) return _value;
            return 4096;
        }

        public const string Key_MaxResourceTotalSize = "MaxResourceTotalSize";
        public static AssetCacheOptions SetMaxResourceTotalSize(this AssetCacheOptions options, int newValue)
        {
            options.Set<int>(Key_MaxResourceTotalSize, newValue);
            return options;
        }
        public static int GetMaxResourceTotalSize(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_MaxResourceTotalSize, out value) && value is int _value) return _value;
            return 1024 * 1024;
        }

        public const string Key_CacheStreams = "CacheStreams";
        public static AssetCacheOptions SetCacheStreams(this AssetCacheOptions options, bool newValue)
        {
            options.Set<bool>(Key_CacheStreams, newValue);
            return options;
        }
        public static bool GetCacheStreams(this AssetCacheOptions options)
        {
            object value;
            if (options.TryGetValue(Key_CacheStreams, out value) && value is bool _value) return _value;
            return true;
        }

    }

}
