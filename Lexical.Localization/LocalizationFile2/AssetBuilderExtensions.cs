//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using Lexical.Localization.LocalizationFile2;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="IAssetBuilder"/> that crosscut with <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static partial class AssetBuilderExtensions__
    {
        /// <summary>
        /// Add localization file source.
        /// </summary>
        /// <param name="assetBuilder"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional)</param>
        /// <param name="fileFormat">(optional) overriding file format to use in case format cannot be infered from file extension</param>
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

    }

}
