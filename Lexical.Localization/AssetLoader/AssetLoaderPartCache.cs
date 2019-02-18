// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           23.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// Decorates <see cref="IAssetLoaderPart"/> by caching loaded parts.
    /// 
    /// AssetLoader adds cache automatically to every part that is added to it.
    /// </summary>
    public class AssetLoaderPartCache : IAssetLoaderPart, IDisposable
    {
        /// <summary>
        /// Loader object.
        /// </summary>
        public readonly IAssetLoaderPart loader;

        /// <summary>
        /// Map of loaded assets and load attempts.
        /// </summary>
        Dictionary<IReadOnlyDictionary<string, string>, IAsset> assetsCache;

        /// <summary>
        /// Cached filenames
        /// </summary>
        Dictionary<IReadOnlyDictionary<string, string>, IReadOnlyDictionary<string, string>[]> loadablesCache;

        /// <summary>
        /// Lock for accessing caches.
        /// </summary>
        ReaderWriterLockSlim mLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Associated file name pattern.
        /// </summary>
        public IAssetNamePattern Pattern => loader.Pattern;

        /// <summary>
        /// All options
        /// </summary>
        public IAssetLoaderPartOptions Options { get => loader.Options; set { loader.Options = value; ClearCache(); } }

        /// <summary>
        /// Array of Pattern.CaptureParts
        /// </summary>
        public string[] CapturePartNames { get; internal set; }

        /// <summary>
        /// Used for no-params key in dictionaries.
        /// </summary>
        IReadOnlyDictionary<string, string> key_no_params;

        public AssetLoaderPartCache(IAssetLoaderPart loader)
        {
            this.loader = loader;
            NamePatternMatchComparer comparer = new NamePatternMatchComparer(loader.Pattern);
            this.assetsCache = new Dictionary<IReadOnlyDictionary<string, string>, IAsset>( comparer );
            this.loadablesCache = new Dictionary<IReadOnlyDictionary<string, string>, IReadOnlyDictionary<string, string>[]>(comparer);
            this.CapturePartNames = loader.Pattern.CaptureParts.Select(part => part.Identifier).ToArray();
            this.key_no_params = new NamePatternMatch(loader.Pattern);
        }

        public void ClearCache()
        {
            LazyList<IAsset> copy = new LazyList<IAsset>();

            mLock.EnterWriteLock();
            try {
                // Gather a list of loaded assets, for disposing them
                foreach (var kp in assetsCache)
                    if (kp.Value != null) copy.Add(kp.Value);
                assetsCache.Clear();
                loadablesCache.Clear();
            }
            finally
            {
                mLock.ExitWriteLock();
            }


            // Dispose loaded assets
            LazyList<Exception> errors = new LazyList<Exception>();
            foreach (var loader in copy)
            {
                try
                {
                    if (loader is IDisposable disposable) disposable.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            if (errors.Count == 1) throw errors[0];
            if (errors.Count > 1) throw new AggregateException(errors);
        }

        public IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Try get from cache
            mLock.EnterReadLock();
            try {
                IAsset cachedAsset;
                if (assetsCache.TryGetValue(parameters, out cachedAsset)) return cachedAsset;
            } finally
            {
                mLock.ExitReadLock();
            }

            // Load
            IAsset asset = loader.Load(parameters);

            // Update cache
            mLock.EnterWriteLock();
            try
            {
                IAsset cachedAsset;
                if (assetsCache.TryGetValue(parameters, out cachedAsset))
                {
                    // Looks like another thread already loaded another instance of asset. Dispose ours.
                    if (cachedAsset != asset)
                    {
                        if (asset is IDisposable _disposable) _disposable.Dispose();
                        return cachedAsset;
                    }
                }

                // Create copy of parameters
                NamePatternMatch newParameters = new NamePatternMatch(loader.Pattern);
                newParameters.Add(parameters, true);

                // Write to cache, even if it is null, to know that key has been tried to load.
                assetsCache[newParameters] = asset;
            } finally
            {
                mLock.ExitWriteLock();
            }

            return asset;
        }

        static IReadOnlyDictionary<string, string>[] empty = new IReadOnlyDictionary<string, string>[0];

        public IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> parameters = null)
        {
            var cache_key = parameters ?? key_no_params;

            // Try cached results
            mLock.EnterReadLock();
            try {
                IReadOnlyDictionary<string, string>[] cached_result = null;
                if (loadablesCache.TryGetValue(cache_key, out cached_result)) return cached_result;
            } finally
            {
                mLock.ExitReadLock();
            }

            // Read from source loader
            IEnumerable<IReadOnlyDictionary<string, string>> result = loader.ListLoadables(parameters);

            // Update cache
            mLock.EnterWriteLock();
            try {
                IReadOnlyDictionary<string, string>[] cached_result = null;
                if (loadablesCache.TryGetValue(cache_key, out cached_result))
                {
                    // Looks like another thread already loaded another result. Return that.
                    if (cached_result != result) return cached_result;
                }

                // Create copy of parameters
                NamePatternMatch new_cache_key = new NamePatternMatch(loader.Pattern);
                new_cache_key.Add(cache_key, true);

                // Put result to array
                cached_result = result.ToArray();

                // Write to cache, even if it is null, to know that key has been tried to load.
                loadablesCache[new_cache_key] = cached_result;

                // Return result as array
                return cached_result;
            } finally
            {
                mLock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            LazyList<IAsset> copy = new LazyList<IAsset>();
            lock (mLock)
            {
                loadablesCache = null;
                copy.AddRange(assetsCache.Values);
                assetsCache.Clear();
            }

            foreach (var asset in copy)
                if (asset is IDisposable disposable) disposable.Dispose();
            assetsCache.Clear();
        }

        public override string ToString()
            => $"{GetType().Name}({loader.ToString()})";
    }

}
