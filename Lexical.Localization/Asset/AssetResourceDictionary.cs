// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class adapts IDictionary{byte[], byte[]} to <see cref="IAssetResourceProvider"/> and <see cref="ILocalizationStringProvider"/>.
    /// </summary>
    public class AssetResourceDictionary : IAssetResourceProvider, IAssetResourceCollection
    {
        protected IReadOnlyDictionary<string, byte[]> source;

        IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        /// <param name="parametrizer">(optional) parametr reader</param>
        public AssetResourceDictionary(IReadOnlyDictionary<string, byte[]> dictionary, IAssetKeyNamePolicy namePolicy = default)
        {
            this.source = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<string> GetResourceNames(IAssetKey key)
        {
            if (key == null) return source.Keys;
            if (namePolicy is IAssetNamePattern pattern)
            {
                return source.Keys.Where(name => pattern.Match(name).Success);
            } else
            {
                string key_name = namePolicy.BuildName(key);
                return source.Keys.Where(name => name.Contains(key_name));
            }
        }

        public byte[] GetResource(IAssetKey key)
        {
            byte[] result = null;
            string id = namePolicy.BuildName(key);

            // Search dictionary
            source.TryGetValue(id, out result);
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
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetBuilder AddDictionary(this IAssetBuilder builder, IReadOnlyDictionary<String, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            builder.AddAsset(new AssetResourceDictionary(dictionary, namePolicy));
            return builder;
        }

        /// <summary>
        /// Add byte[] dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="namePolicy">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetComposition AddDictionary(this IAssetComposition composition, IReadOnlyDictionary<String, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new AssetResourceDictionary(dictionary, namePolicy));
            return composition;
        }
    }
}
