// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Lexical.Asset.Ms.Extensions;
using Lexical.Localization.LocalizationFile;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Provides <see cref="ILocalizationStringProvider"/> assets from <see cref="IFileProvider"/> text files.
    /// Use as a member of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartFileProviderStrings : AssetLoaderPartFileProviderResources, IAssetLoaderPart
    {
        /// <summary>
        /// Function that loads assets. Has to load asset completely as caller will close the stream.
        /// Return null if file is not found.
        /// Throw <see cref="Exception"/> if there was a problem loading the file.
        /// </summary>
        private readonly AssetFileConstructor assetConstructor;

        /// <summary>
        /// Create asset loader that loads assets from <see cref="IFileProvider"/> source.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="filePattern"></param>
        /// <param name="assetConstructor">asset constructor, callee closes stream, caller can close it too, return null if load failed</param>
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, string filePattern, AssetFileConstructor assetConstructor) : base(fileProvider, filePattern)
        {
            this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor));
        }

        /// <summary>
        /// Create asset loader that loads assets from <see cref="IFileProvider"/> source.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="filePattern"></param>
        /// <param name="assetConstructor">asset constructor, callee closes stream, caller can close it too</param>
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, IAssetNamePattern filePattern, AssetFileConstructor assetConstructor) : base(fileProvider, filePattern)
        {
            this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor));
        }

        /// <summary>
        /// Create part that matches <paramref name="filenamePattern"/> to existing files 
        /// and <paramref name="keyNamePattern"/> to to lines found in those files.
        /// 
        /// Derives file extension from the <paramref name="filenamePattern"/> and searches for matching
        /// <see cref="AssetFileConstructor"/> from LocalizationTextReaderBuilder.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="filenamePattern"></param>
        /// <param name="keyNamePattern"></param>
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, IAssetNamePattern filenamePattern, IAssetKeyNamePolicy keyNamePattern) : base(fileProvider,filenamePattern)
        {
            this.assetConstructor = AssetFileConstructors.FileFormat(filenamePattern.Pattern, keyNamePattern);
        }
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, IAssetNamePattern filenamePattern, string keyNamePattern) : this(fileProvider, filenamePattern, new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, string filenamePattern, string keyNamePattern) : this(fileProvider, new AssetNamePattern(filenamePattern), new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartFileProviderStrings(IFileProvider fileProvider, string filenamePattern, IAssetKeyNamePolicy keyNamePattern) : this(fileProvider, new AssetNamePattern(filenamePattern), keyNamePattern) { }


        public override IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Make name
            string filename = Pattern.BuildName(parameters);

            // A required part is missing
            if (filename == null) return null;

            // Test if file exists
            IFileInfo fileinfo = fileProvider.GetFileInfo(filename);
            if (fileinfo == null || !fileinfo.Exists) return null;

            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            // Try open stream
            using (Stream stream = fileinfo.CreateReadStream())
                if (stream != null) return assetConstructor(stream, newParameters);

            return null;
        }
    }

}
