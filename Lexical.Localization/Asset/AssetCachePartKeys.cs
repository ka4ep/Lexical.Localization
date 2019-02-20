// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
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
        public readonly IAssetKeyParametrizer parametrizer;

        Dictionary<IAssetKey, string> stringCache;
        Dictionary<IAssetKey, IAssetKey[]> allStrings;

        public AssetCachePartKeys(IAsset source, AssetCacheOptions options, IAssetKeyParametrizer parametrizer = default)
        {
            if (parametrizer == null) parametrizer = AssetKeyParametrizer.Singleton;
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            // Create a cloner that reads values from IAssetKeys
            this.cloner = new AssetKeyCloner(parametrizer, Key.Parametrizer.Default);

            // Create parametrizer, comparer and cache that reads IAssetKeys and AssetKeyProxies interchangeably. ParameterKey.Parametrizer must be on the left side, or it won't work. (because ParameterKey : IAssetKey).
            IAssetKeyParametrizer compositeParametrizer = Key.Parametrizer.Default.Concat(parametrizer);
            this.comparer = new AssetKeyComparer().AddCanonicalParametrizerComparer(compositeParametrizer).AddNonCanonicalParametrizerComparer(compositeParametrizer);

            this.stringCache = new Dictionary<IAssetKey, string>(comparer);
            this.allStrings = new Dictionary<IAssetKey, IAssetKey[]>(comparer);
        }

        public IEnumerable<IAssetKey> GetAllKeys(IAssetKey key = null)
        {
            int iter = iteration;
            IAssetKey[] value = null;
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
            value = Source.GetAllKeys(key)?.ToArray();

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

    public static partial class LocalizationCacheExtensions
    {
        public static IAssetCache AddKeysCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartKeys(cache.Source, cache.Options));
            return cache;
        }
    }
}
