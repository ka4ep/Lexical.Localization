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

        /// <summary>
        /// Create new asset cache.
        /// </summary>
        /// <param name="source">The source asset whose requests are cached.</param>
        public AssetCache(IAsset source)
        {
            Options = new AssetCacheOptions();
            Source = source ?? throw new ArgumentNullException(nameof(source));
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
    /// Cache part that caches results of <see cref="IAssetKeyCollection"/>.
    /// </summary>
    public class AssetCachePartKeys : IAssetCachePart, IAssetKeyCollection, IAssetReloadable
    {
        static IEnumerable<IAssetKey> emptyStrings = new IAssetKey[0];
        static CultureInfo NO_CULTURE = CultureInfo.GetCultureInfo("");
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        public IAsset Source { get; internal set; }
        public AssetCacheOptions Options { get; internal set; }
        protected volatile int iteration;

        AssetKeyComparer comparer;
        AssetKeyCloner cloner;

        /// <summary>
        /// Cached queries where key is filter-criteria-key, and value is query result.
        /// </summary>
        Dictionary<IAssetKey, IAssetKey[]> allKeyQueries, keyQueries;

        public AssetCachePartKeys(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            // Create a cloner that reads values from IAssetKeys
            this.cloner = new AssetKeyCloner(Key.Root);

            // Create parametrizer, comparer and cache that reads IAssetKeys and AssetKeyProxies interchangeably. ParameterKey.Parametrizer must be on the left side, or it won't work. (because ParameterKey : IAssetKey).
            this.comparer = AssetKeyComparer.Default;

            this.keyQueries = new Dictionary<IAssetKey, IAssetKey[]>(comparer);
            this.allKeyQueries = new Dictionary<IAssetKey, IAssetKey[]>(comparer);
        }

        public IEnumerable<IAssetKey> GetKeys(IAssetKey key = null)
        {
            int iter = iteration;
            IAssetKey[] queryResult = null;
            m_lock.EnterReadLock();
            // Hash-Equals may throw exceptions, we need try-finally to release lock properly.
            try
            {
                if (keyQueries.TryGetValue(key ?? cloner.Root, out queryResult)) return queryResult;
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            queryResult = Source.GetKeys(key)?.ToArray();

            // Write to cache, be that null or not
            IAssetKey cacheKey = (Options.GetCloneKeys() ? cloner.Copy(key) : key) ?? cloner.Root;
            m_lock.EnterWriteLock();
            try
            {
                // The caller has flushed the cache, so let's not cache the data.
                if (iter != iteration) return queryResult;
                keyQueries[cacheKey] = queryResult;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return queryResult;
        }

        public IEnumerable<IAssetKey> GetAllKeys(IAssetKey key = null)
        {
            int iter = iteration;
            IAssetKey[] queryResult = null;
            m_lock.EnterReadLock();
            // Hash-Equals may throw exceptions, we need try-finally to release lock properly.
            try
            {
                if (allKeyQueries.TryGetValue(key ?? cloner.Root, out queryResult)) return queryResult;
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            queryResult = Source.GetAllKeys(key)?.ToArray();

            // Write to cache, be that null or not
            IAssetKey cacheKey = (Options.GetCloneKeys() ? cloner.Copy(key) : key) ?? cloner.Root;
            m_lock.EnterWriteLock();
            try
            {
                // The caller has flushed the cache, so let's not cache the data.
                if (iter != iteration) return queryResult;
                allKeyQueries[cacheKey] = queryResult;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return queryResult;
        }

        IAsset IAssetReloadable.Reload()
        {
            Source.Reload();
            iteration++;

            m_lock.EnterWriteLock();
            try
            {
                keyQueries.Clear();
                allKeyQueries.Clear();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return this;
        }

        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary>
    /// Cache part that caches results of <see cref="ILocalizationAssetCultureCapabilities" />.
    /// </summary>
    public class AssetCachePartCultures : IAssetCachePart, ILocalizationAssetCultureCapabilities, IAssetReloadable
    {
        static CultureInfo[] empty_cultures = new CultureInfo[0];
        static CultureInfo NO_CULTURE = CultureInfo.GetCultureInfo("");
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        public IAsset Source { get; internal set; }
        public AssetCacheOptions Options { get; internal set; }
        protected volatile int iteration;

        AssetKeyComparer comparer;
        AssetKeyCloner cloner;
        CultureInfo[] cultures;
        bool culturesCached;

        public AssetCachePartCultures(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.cloner = new AssetKeyCloner(Key.Root);
            this.comparer = AssetKeyComparer.Default;
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

    }

    /// <summary>
    /// Cache part that caches results of <see cref="ILocalizationStringCollection" /> and <see cref="ILocalizationStringProvider"/>.
    /// </summary>
    public class AssetCachePartStrings : IAssetCachePart, ILocalizationStringCollection, ILocalizationStringProvider, IAssetReloadable
    {
        static IEnumerable<KeyValuePair<string, string>> emptyStrings = new KeyValuePair<string, string>[0];
        static CultureInfo NO_CULTURE = CultureInfo.GetCultureInfo("");
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        public IAsset Source { get; internal set; }
        public AssetCacheOptions Options { get; internal set; }
        protected volatile int iteration;

        AssetKeyComparer comparer;
        AssetKeyCloner cloner;

        Dictionary<IAssetKey, string> stringCache;
        Dictionary<IAssetKey, KeyValuePair<string, string>[]> allStrings;

        public AssetCachePartStrings(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.cloner = new AssetKeyCloner(Key.Root);
            this.comparer = AssetKeyComparer.Default;
            this.stringCache = new Dictionary<IAssetKey, string>(comparer);
            this.allStrings = new Dictionary<IAssetKey, KeyValuePair<string, string>[]>(comparer);
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllStrings(IAssetKey key = null)
        {
            int iter = iteration;
            KeyValuePair<string, string>[] value = null;
            m_lock.EnterReadLock();
            // Hash-Equals may throw exceptions, we need try-finally to release lock properly.
            try
            {
                if (allStrings.TryGetValue(key ?? cloner.Root, out value)) return value;
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            value = Source.GetAllStrings(key)?.ToArray();

            // Write to cache, be that null or not
            IAssetKey cacheKey = (Options.GetCloneKeys() ? cloner.Copy(key) : key) ?? cloner.Root;
            m_lock.EnterWriteLock();
            try
            {
                // The caller has flushed the cache, so let's not cache the data.
                if (iter != iteration) return value;
                allStrings[cacheKey] = value;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return value;
        }

        string ILocalizationStringProvider.GetString(IAssetKey key)
        {
            int iter = iteration;
            string value = null;
            m_lock.EnterReadLock();
            // Hash-Equals may throw exceptions, we need try-finally to capture that.
            try
            {
                if (stringCache.TryGetValue(key, out value)) return value;
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            value = Source.GetString(key);

            // Write to cache, be that null or not
            IAssetKey cacheKey = cloner.Copy(key);
            m_lock.EnterWriteLock();
            try
            {
                // The caller has flushed the cache, so let's not cache the data.
                if (iter != iteration) return value;
                stringCache[cacheKey] = value;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return value;
        }

        IAsset IAssetReloadable.Reload()
        {
            Source.Reload();
            iteration++;

            m_lock.EnterWriteLock();
            try
            {
                stringCache.Clear();
                allStrings.Clear();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return this;
        }

        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }

    /// <summary>
    /// Cache part that caches the results of <see cref="IAssetResourceCollection"/> and <see cref="IAssetResourceProvider"/>.
    /// </summary>
    public class AssetCachePartResources : IAssetCachePart, IAssetResourceCollection, IAssetResourceProvider, IAssetReloadable
    {
        public AssetCacheOptions Options { get; internal set; }
        public IAsset Source { get; internal set; }

        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        protected volatile int iteration;
        AssetKeyComparer comparer;
        AssetKeyCloner cloner;

        IDictionary<IAssetKey, byte[]> resourceCache;
        string[] resourceNames;
        bool resourceNamesCached;

        public AssetCachePartResources(IAsset source, AssetCacheOptions options)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            // Create a cloner that reads values from IAssetKeys
            this.cloner = new AssetKeyCloner(Key.Root);

            // Create parametrizer, comparer and cache that reads IAssetKeys and AssetKeyProxies interchangeably. ParameterKey.Parametrizer must be on the left side, or it won't work. (because ParameterKey : IAssetKey).
            this.comparer = AssetKeyComparer.Default;
            this.resourceCache = new Dictionary<IAssetKey, byte[]>(comparer);
        }

        public byte[] GetResource(IAssetKey key)
        {
            int iter = iteration;
            byte[] value = null;
            m_lock.EnterReadLock();
            // Hash-Equals may throw exceptions, we need try-finally to capture that.
            try
            {
                if (resourceCache.TryGetValue(key ?? cloner.Root, out value)) return value;
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Read from backend and write to cache
            value = Source.GetResource(key);

            // Write to cache, be that null or not.
            if (value == null || value.Length <= Options.GetMaxResourceSize())
            {
                IAssetKey cacheKey = (Options.GetCloneKeys() ? cloner.Copy(key) : key) ?? cloner.Root;
                m_lock.EnterWriteLock();
                try
                {
                    // The caller has flushed the cache, so let's not cache the data.
                    if (iter != iteration) return value;
                    resourceCache[cacheKey] = value;
                }
                finally
                {
                    m_lock.ExitWriteLock();
                }
                return value;
            }

            return null;
        }

        public Stream OpenStream(IAssetKey key)
        {
            int iter = iteration;
            byte[] value = null;

            m_lock.EnterReadLock();
            try
            {
                if (resourceCache.TryGetValue(key ?? cloner.Root, out value)) return new MemoryStream(value);
            }
            finally
            {
                m_lock.ExitReadLock();
            }

            // Open stream
            Stream stream = Source.OpenStream(key);

            // Store into cache?
            if (Options.GetCacheStreams())
            {
                // Cache null value
                if (stream == null)
                {
                    IAssetKey cacheKey = (Options.GetCloneKeys() ? cloner.Copy(key) : key) ?? cloner.Root;
                    m_lock.EnterWriteLock();
                    try
                    {
                        // The caller has not flushed the cache, so let's cache the data.
                        if (iter == iteration) resourceCache[cacheKey] = null;
                        return null;
                    }
                    finally
                    {
                        m_lock.ExitWriteLock();
                    }
                }

                // Read stream completely and then cache it
                long position = -1;
                int ix = 0;
                try
                {
                    // Try to read stream lenght, if fails, throws an exception
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
                            IAssetKey cacheKey = cloner.Copy(key);
                            m_lock.EnterWriteLock();
                            try
                            {
                                // The caller has not flushed the cache, so let's cache the data.
                                if (iter == iteration) resourceCache[cacheKey] = data;

                                // Wrap to new stream.
                                return new MemoryStream(data);
                            }
                            finally
                            {
                                m_lock.ExitWriteLock();
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

                    // Stream is not rewound. Let's open it again.
                    if (ix > 0)
                    {
                        stream.Dispose();
                        return Source.OpenStream(key);
                    }
                }
            }
            return null;
        }

        static string[] empty_string_array = new string[0];
        public IEnumerable<string> GetResourceNames(IAssetKey key)
        {
            // Request was for specific key
            if (key != null) return Source.GetResourceNames(key);

            // Do we have this in cache
            var _resourceNames = resourceNames;
            if (resourceNamesCached) return _resourceNames;

            // Retrieve
            int iter = iteration;
            IEnumerable<string> enumr = Source.GetResourceNames(null);
            m_lock.EnterWriteLock();
            try
            {
                // Got nothing
                if (enumr == null)
                {
                    // Reloaded
                    if (iter != iteration) return null;
                    // Cache string[0]
                    resourceNamesCached = true;
                    return resourceNames = null;
                }

                // Reloaded, don't cache
                if (iter != iteration) return enumr;

                // Cache
                resourceNames = enumr.ToArray();
                resourceNamesCached = true;
                return resourceNames;
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
        }

        public IAsset Reload()
        {
            Source.Reload();
            iteration++;

            m_lock.EnterWriteLock();
            try
            {
                resourceNames = null;
                resourceNamesCached = false;
                resourceCache.Clear();
            }
            finally
            {
                m_lock.ExitWriteLock();
            }

            return this;
        }

        public override string ToString()
            => $"{GetType().Name}({Source.ToString()})";
    }


    public static partial class LocalizationCacheExtensions
    {
        public static IAssetCache AddKeysCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartKeys(cache.Source, cache.Options));
            return cache;
        }
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
