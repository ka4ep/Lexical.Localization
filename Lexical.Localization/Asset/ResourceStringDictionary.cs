﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// This class adapts IDictionary&lt;string, byte[]&gt; to <see cref="IAssetResourceProvider"/> and <see cref="IAssetResourceNamesEnumerable"/>.
    /// </summary>
    public class ResourceStringDictionary : IAssetResourceProvider, IAssetResourceNamesEnumerable, ILocalizationAssetCultureCapabilities
    {
        /// <summary>
        /// Source dictionary
        /// </summary>
        protected IReadOnlyDictionary<string, byte[]> dictionary;

        /// <summary>
        /// Name policy that converts <see cref="IAssetKey"/> to string, and back to <see cref="IAssetKey"/>.
        /// </summary>
        IAssetKeyNamePolicy namePolicy;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="namePolicy">policy that describes how to convert localization key to dictionary key</param>
        public ResourceStringDictionary(IReadOnlyDictionary<string, byte[]> dictionary, IAssetKeyNamePolicy namePolicy = default)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.namePolicy = namePolicy ?? throw new ArgumentNullException(nameof(namePolicy));
        }

        /// <summary>
        /// Get resource names
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<string> GetResourceNames(IAssetKey filterKey)
        {
            // Return all 
            if (filterKey == null) return dictionary.Keys.ToList();
            // Create filter.
            AssetKeyFilter filter = new AssetKeyFilter().KeyRule(filterKey);
            // There are no rules
            if (!filter.HasRules) return dictionary.Keys.ToList();
            // Filter with pattern
            if (namePolicy is IAssetNamePattern pattern_) return Filter1(pattern_).ToList();
            // Filter with parser
            if (namePolicy is IAssetKeyNameParser parser_) return Filter2(parser_).ToList();
            // Return nothing
            return null;

            IEnumerable<string> Filter1(IAssetNamePattern pattern)
            {
                foreach (var line in dictionary)
                {
                    IAssetNamePatternMatch match = pattern.Match(line.Key);
                    if (!match.Success || !filter.Filter(match)) continue;
                    yield return line.Key;
                }
            }
            IEnumerable<string> Filter2(IAssetKeyNameParser parser)
            {
                foreach (var line in dictionary)
                {
                    IAssetKey key;
                    if (!parser.TryParse(line.Key, out key)) continue;
                    if (!filter.Filter(key)) continue;
                    yield return line.Key;
                }
            }
        }

        /// <summary>
        /// Get string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<string> GetAllResourceNames(IAssetKey filterKey)
            => GetResourceNames(filterKey);

        /// <summary>
        /// Get cultures
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            if (namePolicy is IAssetNamePattern pattern)
            {
                IAssetNamePatternPart culturePart;
                if (!pattern.PartMap.TryGetValue("Culture", out culturePart)) return null;

                Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
                foreach (var kp in dictionary)
                {
                    IAssetNamePatternMatch match = pattern.Match(kp.Key);
                    if (!match.Success) continue;
                    string culture = match[culturePart.CaptureIndex];
                    if (culture == null) culture = "";
                    if (result.ContainsKey(culture)) continue;
                    try { result[culture] = CultureInfo.GetCultureInfo(culture); } catch (CultureNotFoundException) { }
                }
                return result.Values.ToArray();
            }
            else if (namePolicy is IAssetKeyNameParser parser)
            {
                return dictionary.Keys.Select(k => parser.TryParse(k, Key.Root)?.FindCulture()).Where(ci => ci != null).Distinct().ToArray();
            }
            else
            {
                // Can't extract culture
                return null;
            }
        }

        /// <summary>
        /// Get resource from <see cref="dictionary"/> by converting <paramref name="key"/> to <see cref="string"/> with <see cref="namePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or null</returns>
        public byte[] GetResource(IAssetKey key)
        {
            byte[] result = null;
            string id = namePolicy.BuildName(key);

            // Search dictionary
            dictionary.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Open stream to resource from <see cref="dictionary"/> by converting <paramref name="key"/> to <see cref="string"/> with <see cref="namePolicy"/>.
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
        /// <param name="namePolicy">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetBuilder AddResources(this IAssetBuilder builder, IReadOnlyDictionary<String, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
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
        public static IAssetComposition AddResources(this IAssetComposition composition, IReadOnlyDictionary<String, byte[]> dictionary, IAssetKeyNamePolicy namePolicy)
        {
            composition.Add(new ResourceStringDictionary(dictionary, namePolicy));
            return composition;
        }
    }
}
