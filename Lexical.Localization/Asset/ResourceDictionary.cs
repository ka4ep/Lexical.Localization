// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class adapts IDictionary&lt;IAssetKey, byte[]&gt; to <see cref="IAssetResourceProvider"/> and <see cref="IAssetResourceKeysEnumerable"/>.
    /// </summary>
    public class ResourceDictionary : IAssetResourceProvider, IAssetResourceKeysEnumerable, ILocalizationAssetCultureCapabilities
    {
        /// <summary>
        /// Source dictionary
        /// </summary>
        protected IReadOnlyDictionary<IAssetKey, byte[]> dictionary;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        public ResourceDictionary(IReadOnlyDictionary<IAssetKey, byte[]> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<IAssetKey> GetResourceKeys(IAssetKey filterKey)
        {
            // Return all 
            if (filterKey == null) return dictionary.Keys.ToList();
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            // Return keys as list
            return filter.Filter(dictionary.Keys).ToList();
        }

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<IAssetKey> GetAllResourceKeys(IAssetKey filterKey)
            => GetResourceKeys(filterKey);

        /// <summary>
        /// Get cultures
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
            => dictionary.Keys.Select(l => l.FindCulture()).Where(ci => ci != null).Distinct().ToArray();

        /// <summary>
        /// Read resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] GetResource(IAssetKey key)
        {
            byte[] result = null;
            // Search dictionary
            dictionary.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Open stream to resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Stream OpenStream(IAssetKey key)
        {
            byte[] data = GetResource(key);
            if (data == null) return null;
            return new MemoryStream(data);
        }

        /// <summary>
        /// Print class name
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
            => $"{GetType().Name}()";
    }

    public static partial class AssetExtensions_
    {
        /// <summary>
        /// Add byte[] dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IAssetBuilder AddResources(this IAssetBuilder builder, IReadOnlyDictionary<IAssetKey, byte[]> dictionary)
        {
            builder.AddAsset(new ResourceDictionary(dictionary));
            return builder;
        }

        /// <summary>
        /// Add byte[] dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IAssetComposition AddResources(this IAssetComposition composition, IReadOnlyDictionary<IAssetKey, byte[]> dictionary)
        {
            composition.Add(new ResourceDictionary(dictionary));
            return composition;
        }
    }
}
