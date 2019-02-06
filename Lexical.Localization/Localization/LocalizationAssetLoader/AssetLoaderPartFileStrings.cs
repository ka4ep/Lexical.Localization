// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           6.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using Lexical.Localization.LocalizationFile;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// This class loads file files as strings.
    /// 
    /// Use with <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartFileStrings : AssetLoaderPartFileResources, IAssetLoaderPart
    {
        public readonly AssetFileConstructor assetConstructor;

        public AssetLoaderPartFileStrings(string filenamePattern) : this(filenamePattern, (IAssetKeyNamePolicy) null) { }
        public AssetLoaderPartFileStrings(IAssetNamePattern filenamePattern) : this(filenamePattern, (IAssetKeyNamePolicy) null) { }
        public AssetLoaderPartFileStrings(string filenamePattern, AssetFileConstructor assetConstructor) : base(filenamePattern) { this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor)); }
        public AssetLoaderPartFileStrings(IAssetNamePattern filenamePattern, AssetFileConstructor assetConstructor) : base(filenamePattern) { this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor)); }

        /// <summary>
        /// Create part that matches <paramref name="filenamePattern"/> to existing files 
        /// and <paramref name="keyNamePattern"/> to to lines found in those files.
        /// 
        /// Derives file extension from the <paramref name="filenamePattern"/> and searches for matching
        /// <see cref="AssetFileConstructor"/> from LocalizationTextReaderBuilder.
        /// </summary>
        /// <param name="filenamePattern"></param>
        /// <param name="keyNamePattern"></param>
        public AssetLoaderPartFileStrings(IAssetNamePattern filenamePattern, IAssetKeyNamePolicy keyNamePattern) : base(filenamePattern)
        {
            this.assetConstructor = AssetFileConstructors.FileFormat(filenamePattern.Pattern, keyNamePattern);
        }
        public AssetLoaderPartFileStrings(IAssetNamePattern filenamePattern, string keyNamePattern) : this(filenamePattern, new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartFileStrings(string filenamePattern, string keyNamePattern) : this(new AssetNamePattern(filenamePattern), new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartFileStrings(string filenamePattern, IAssetKeyNamePolicy keyNamePattern) : this(new AssetNamePattern(filenamePattern), keyNamePattern) { }

        public override IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            // Make name
            string filename = Pattern.BuildName(newParameters);

            // A required part is missing
            if (filename == null) return null;

            // Get paths
            IList<string> paths = Options.GetPaths();

            // No paths
            if (paths == null || paths.Count == 0) return null;

            foreach (string path in paths)
            {
                // Full path
                string fullpath = Path.Combine(path, filename);

                // File doesn't exist
                if (!File.Exists(fullpath)) continue;

                // Read the strings file
                using (FileStream fs = new FileStream(fullpath, FileMode.Open))
                    return assetConstructor(fs, newParameters);
            }

            return null;
        }
    }


}
