// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Lexical.Localization
{
    /// <summary>
    /// This <see cref="IAssetCachePart"/> handles caching of <see cref="ILocalizationAssetCultureCapabilities" /> requests as a part of <see cref="IAssetCache"/>.
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
            this.comparer = new AssetKeyComparer().AddCanonicalParametrizedComparer().AddNonCanonicalParametrizedComparer();
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

    public static partial class LocalizationCacheExtensions
    {
        public static IAssetCache AddCulturesCache(this IAssetCache cache)
        {
            cache.Add(new AssetCachePartCultures(cache.Source, cache.Options));
            return cache;
        }
    }

}
