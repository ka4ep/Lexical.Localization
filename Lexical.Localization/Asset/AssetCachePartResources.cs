// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// This <see cref="IAssetCachePart"/> is a component of <see cref="IAssetCache"/> that adds a cache
    /// the capability to cache byte[] resources.
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
            this.comparer = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();
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

    public static partial class AssetCacheExtensions_
    {
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
