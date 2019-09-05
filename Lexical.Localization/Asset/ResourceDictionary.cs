// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Binary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// This class adapts IDictionary&lt;ILine, byte[]&gt; to <see cref="IBinaryAsset"/> and <see cref="IBinaryAssetKeysEnumerable"/>.
    /// </summary>
    public class ResourceDictionary : IBinaryAsset, IBinaryAssetKeysEnumerable, IAssetCultureEnumerable
    {
        /// <summary>
        /// Source dictionary
        /// </summary>
        protected IReadOnlyDictionary<ILine, byte[]> dictionary;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        public ResourceDictionary(IReadOnlyDictionary<ILine, byte[]> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetBinaryKeys(ILine filterKey)
        {
            // Return all 
            if (filterKey == null) return dictionary.Keys.ToList();
            // Create filter.
            LineQualifier filter = new LineQualifier().Rule(filterKey) as LineQualifier;
            // Return keys as list
            return filter.Qualify(dictionary.Keys).ToList();
        }

        /// <summary>
        /// Get keys
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<ILine> GetAllBinaryKeys(ILine filterKey)
            => GetBinaryKeys(filterKey);

        /// <summary>
        /// Get cultures
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
            => dictionary.Keys.Select(l => l.GetCultureInfo()).Where(ci => ci != null).Distinct().ToArray();

        /// <summary>
        /// Read resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineBinaryBytes GetBytes(ILine key)
        {
            byte[] result = null;
            // Search dictionary
            if (dictionary.TryGetValue(key, out result))
                return new LineBinaryBytes(key, result, LineStatus.ResolveOkFromAsset);
            else
                return new LineBinaryBytes(key, LineStatus.ResolveFailedNoValue);
        }

        /// <summary>
        /// Open stream to resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineBinaryStream GetStream(ILine key)
        {
            byte[] data = null;
            // Search dictionary
            if (dictionary.TryGetValue(key, out data))
                return new LineBinaryStream(key, new MemoryStream(data), LineStatus.ResolveOkFromAsset);
            else
                return new LineBinaryStream(key, LineStatus.ResolveFailedNoValue);
        }

        /// <summary>
        /// Print class name
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
            => $"{GetType().Name}()";
    }

    /// <summary></summary>
    public static partial class AssetExtensions
    {
        /// <summary>
        /// Add byte[] dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IAssetBuilder AddResources(this IAssetBuilder builder, IReadOnlyDictionary<ILine, byte[]> dictionary)
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
        public static IAssetComposition AddResources(this IAssetComposition composition, IReadOnlyDictionary<ILine, byte[]> dictionary)
        {
            composition.Add(new ResourceDictionary(dictionary));
            return composition;
        }
    }
}
