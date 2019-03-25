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
    /// This class adapts IDictionary{string, byte[]} to <see cref="IAssetResourceProvider"/> and <see cref="IAssetResourceNamesEnumerable"/>.
    /// </summary>
    public class ResourceStringDictionary : IAssetResourceProvider, IAssetResourceNamesEnumerable, ILocalizationAssetCultureCapabilities
    {
        protected IReadOnlyDictionary<string, byte[]> source;

        IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="namePolicy">(optional) policy that describes how to convert localization key to dictionary key</param>
        public ResourceStringDictionary(IReadOnlyDictionary<string, byte[]> dictionary, IAssetKeyNamePolicy namePolicy = default)
        {
            this.source = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.namePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<string> GetResourceNames(IAssetKey filterKey)
            => GetAllResourceNames(filterKey);
        public IEnumerable<string> GetAllResourceNames(IAssetKey filterKey)
        {
            if (filterKey == null) return source.Keys;
            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternMatch match = pattern.Match(filterKey);
                return source.Where(kp => IsEqualOrSuperset(match, pattern.Match(kp.Key))).Select(kp=>kp.Key);
            }
            else
            {
                string key_name = namePolicy.BuildName(filterKey);
                return source.Where(kp => kp.Key.Contains(key_name)).Select(kp => kp.Key);
            }
        }

        CultureInfo[] cultures;
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var _cultures = cultures;
            if (_cultures != null) return _cultures;

            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternPart culturePart;
                if (!pattern.PartMap.TryGetValue("Culture", out culturePart)) return null;

                Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
                foreach (var kp in source)
                {
                    IAssetNamePatternMatch match = pattern.Match(kp.Key);
                    if (!match.Success) continue;
                    string culture = match[culturePart.CaptureIndex];
                    if (culture == null) culture = "";
                    if (result.ContainsKey(culture)) continue;
                    try { result[culture] = CultureInfo.GetCultureInfo(culture); } catch (CultureNotFoundException) { }
                }
                return cultures = result.Values.ToArray();
            }
            else
            {
                // Can't extract culture
                return null;
            }
        }

        /// <summary>
        /// Comapres two matches for equality or being superset.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        static bool IsEqualOrSuperset(IAssetNamePatternMatch match, IAssetNamePatternMatch other)
        {
            if (match.Pattern != other.Pattern) return false;
            for (int ix = 0; ix < match.Pattern.CaptureParts.Length; ix++)
            {
                IAssetNamePatternPart part = match.Pattern.CaptureParts[ix];

                if (match.PartValues[ix] == null) continue;
                if (match.PartValues[ix] != other.PartValues[ix]) return false;
            }
            return true;
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
            builder.AddAsset(new ResourceStringDictionary(dictionary, namePolicy));
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
            composition.Add(new ResourceStringDictionary(dictionary, namePolicy));
            return composition;
        }
    }
}
