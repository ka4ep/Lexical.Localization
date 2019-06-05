// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           12.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Resource;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// This class adapts IDictionary&lt;string, byte[]&gt; to <see cref="IResourceAsset"/> and <see cref="IResourceAssetNamesEnumerable"/>.
    /// </summary>
    public class ResourceStringDictionary : IResourceAsset, IResourceAssetNamesEnumerable, IAssetCultureEnumerable
    {
        /// <summary>
        /// Source dictionary
        /// </summary>
        protected IReadOnlyDictionary<string, byte[]> dictionary;

        /// <summary>
        /// Line format that converts <see cref="ILine"/> to string, and back to <see cref="ILine"/>.
        /// </summary>
        ILineFormat lineFormat;

        /// <summary>
        /// Create language byte[] resolver that uses a dictionary as a backend.
        /// </summary>
        /// <param name="dictionary">dictionary</param>
        /// <param name="lineFormat">line format for converting lines to strings</param>
        public ResourceStringDictionary(IReadOnlyDictionary<string, byte[]> dictionary, ILineFormat lineFormat = default)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.lineFormat = lineFormat ?? throw new ArgumentNullException(nameof(lineFormat));
        }

        /// <summary>
        /// Get resource names
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public IEnumerable<string> GetResourceNames(ILine filterKey)
        {
            // Return all 
            if (filterKey == null) return dictionary.Keys.ToList();
            // Create filter.
            LineQualifier filter = new LineQualifier().Rule(filterKey) as LineQualifier;
            // There are no rules
            if (!filter.HasRules) return dictionary.Keys.ToList();
            // Filter with pattern
            if (lineFormat is ILinePattern pattern_) return Filter1(pattern_).ToList();
            // Filter with parser
            if (lineFormat is ILineFormatParser parser_) return Filter2(parser_).ToList();
            // Return nothing
            return null;

            IEnumerable<string> Filter1(ILinePattern pattern)
            {
                foreach (var line in dictionary)
                {
                    ILinePatternMatch match = pattern.Match(line.Key);
                    if (!match.Success || !filter.Qualify(match)) continue;
                    yield return line.Key;
                }
            }
            IEnumerable<string> Filter2(ILineFormatParser parser)
            {
                foreach (var line in dictionary)
                {
                    ILine key;
                    if (!parser.TryParse(line.Key, out key)) continue;
                    if (!filter.Qualify(key)) continue;
                    yield return line.Key;
                }
            }
        }

        /// <summary>
        /// Get string lines
        /// </summary>
        /// <param name="filterKey">(optional) filter key</param>
        /// <returns>lines or null</returns>
        public IEnumerable<string> GetAllResourceNames(ILine filterKey)
            => GetResourceNames(filterKey);

        /// <summary>
        /// Get cultures
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            if (lineFormat is ILinePattern pattern)
            {
                ILinePatternPart culturePart;
                if (!pattern.PartMap.TryGetValue("Culture", out culturePart)) return null;

                Dictionary<string, CultureInfo> result = new Dictionary<string, CultureInfo>();
                foreach (var kp in dictionary)
                {
                    ILinePatternMatch match = pattern.Match(kp.Key);
                    if (!match.Success) continue;
                    string culture = match[culturePart.CaptureIndex];
                    if (culture == null) culture = "";
                    if (result.ContainsKey(culture)) continue;
                    try { result[culture] = CultureInfo.GetCultureInfo(culture); } catch (CultureNotFoundException) { }
                }
                return result.Values.ToArray();
            }
            else if (lineFormat is ILineFormatParser parser)
            {
                //lineFormat.Parse()
                return dictionary.Keys.Select(k => {
                    ILine l;
                    CultureInfo c;
                    return parser.TryParse(k, out l) && l.TryGetCultureInfo(out c) ? c : null;
                }).Where(ci => ci != null).Distinct().ToArray();            
            }
            else
            {
                // Can't extract culture
                return null;
            }
        }

        /// <summary>
        /// Get resource from <see cref="dictionary"/> by converting <paramref name="key"/> to <see cref="string"/> with <see cref="lineFormat"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or null</returns>
        public LineResourceBytes GetResourceBytes(ILine key)
        {
            byte[] result = null;
            string id = lineFormat.Print(key);

            // Search dictionary
            if (dictionary.TryGetValue(id, out result))
                return new LineResourceBytes(key, result, LineStatus.ResolveOkFromAsset);
            else
                return new LineResourceBytes(key, LineStatus.ResolveFailedNoValue);
        }

        /// <summary>
        /// Open stream to resource from <see cref="dictionary"/> by converting <paramref name="key"/> to <see cref="string"/> with <see cref="lineFormat"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineResourceStream GetResourceStream(ILine key)
        {
            byte[] result = null;
            string id = lineFormat.Print(key);

            // Search dictionary
            if (dictionary.TryGetValue(id, out result))
                return new LineResourceStream(key, new MemoryStream(result), LineStatus.ResolveOkFromAsset);
            else
                return new LineResourceStream(key, LineStatus.ResolveFailedNoValue);
        }

        /// <summary>
        /// Print class name
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
            => $"{GetType().Name}()";
    }

    public static partial class AssetExtensions
    {
        /// <summary>
        /// Add byte[] dictionary to builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="dictionary"></param>
        /// <param name="lineFormat">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetBuilder AddResources(this IAssetBuilder builder, IReadOnlyDictionary<String, byte[]> dictionary, ILineFormat lineFormat)
        {
            builder.AddAsset(new ResourceStringDictionary(dictionary, lineFormat));
            return builder;
        }

        /// <summary>
        /// Add byte[] dictionary to composition.
        /// </summary>
        /// <param name="composition"></param>
        /// <param name="dictionary"></param>
        /// <param name="lineFormat">instructions how to convert key to byte[]</param>
        /// <returns></returns>
        public static IAssetComposition AddResources(this IAssetComposition composition, IReadOnlyDictionary<String, byte[]> dictionary, ILineFormat lineFormat)
        {
            composition.Add(new ResourceStringDictionary(dictionary, lineFormat));
            return composition;
        }
    }
}
