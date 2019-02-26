//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization
{
    public static partial class LocalizationFileFormatExtensions___
    {
        /// <summary>
        /// Add localization file source.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <param name="fileFormat"></param>
        /// <returns></returns>
        public static IAssetBuilder AddLocalizationFile(this IAssetBuilder assetBuilder, string filename, IAssetKeyNamePolicy namePolicy = default, ILocalizationFileFormat fileFormat = null)
        {
            if (fileFormat == null) {
                string ext = LocalizationFileFormatMap.GetExtension(filename);
                fileFormat = LocalizationFileFormatMap.Singleton.TryGet(ext);
            }
            if (fileFormat == null) throw new ArgumentException($"Could not resolve localization file reader for file {filename}.");
            assetBuilder.AddSource(new AssetFileSource(fileFormat, filename, namePolicy));
            return assetBuilder;
        }

        static IAssetKeyNamePolicy DefaultPolicy = AssetKeyNameProvider.Default;

        public static IEnumerable<KeyValuePair<string, string>> ToStringLines(this IEnumerable<KeyValuePair<IAssetKey, string>> keyLines, IAssetKeyNamePolicy policy)
            => keyLines.Select(line => new KeyValuePair<string, string>((policy?? DefaultPolicy).BuildName(line.Key), line.Value));
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<IAssetKey, string>> lines)
            => KeyTree.Create(lines);
    }

}
