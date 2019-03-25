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
    /// This class adapts IDictionary{IAssetKey, byte[]} to <see cref="IAssetResourceProvider"/> and <see cref="IAssetResourceKeysEnumerable"/>.
    /// </summary>
    public class ResourceDictionary : IAssetResourceProvider, IAssetResourceKeysEnumerable, ILocalizationAssetCultureCapabilities
    {
        protected IReadOnlyDictionary<IAssetKey, byte[]> source;

        IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        public ResourceDictionary(IReadOnlyDictionary<IAssetKey, byte[]> dictionary, IAssetKeyNamePolicy namePolicy = default)
        {
            this.source = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<IAssetKey> GetResourceKeys(IAssetKey filterKey)
            => GetAllResourceKeys(filterKey);
        public IEnumerable<IAssetKey> GetAllResourceKeys(IAssetKey filterKey)
        {
            if (filterKey == null) return source.Keys;
            return null; // TODO Implement filtering
        }

        CultureInfo[] cultures;
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (_cultures != null) return _cultures;

            HashSet<CultureInfo> set = new HashSet<CultureInfo>();
            foreach(var line in source)
            {
                CultureInfo ci = line.Key.FindCulture();
                if (ci != null) set.Add(ci);
            }
            return cultures = set.ToArray();
        }

        public byte[] GetResource(IAssetKey key)
        {
            byte[] result = null;
            // Search dictionary
            source.TryGetValue(key, out result);
            return result;
        }

        public Stream OpenStream(IAssetKey key)
        {
            byte[] data = GetResource(key);
            if (data == null) return null;
            return new MemoryStream(data);
        }

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
        /// <param name="namePolicy">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetBuilder AddDictionary(this IAssetBuilder builder, IReadOnlyDictionary<IAssetKey, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            builder.AddAsset(new ResourceDictionary(dictionary, namePolicy));
            return builder;
        }

        /// <summary>
        /// Add byte[] dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetComposition AddDictionary(this IAssetComposition composition, IReadOnlyDictionary<IAssetKey, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new ResourceDictionary(dictionary, namePolicy));
            return composition;
        }
    }
}
