// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           23.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lexical.Localization
{
    /// <summary>
    /// This class loads embedded files as strings.
    /// 
    /// Use with <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartEmbeddedStrings : AssetLoaderPartEmbeddedResources, IAssetLoaderPart
    {
        public readonly AssetFileConstructor assetConstructor;

        public AssetLoaderPartEmbeddedStrings(string resourceNamePattern) : this(resourceNamePattern, (IAssetKeyNamePolicy)default) { }
        public AssetLoaderPartEmbeddedStrings(IAssetNamePattern resourceNamePattern) : this(resourceNamePattern, (IAssetKeyNamePolicy)default) { }
        
        public AssetLoaderPartEmbeddedStrings(string resourceNamePattern, AssetFileConstructor assetConstructor) : base(resourceNamePattern)
        {
            this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor));
        }

        public AssetLoaderPartEmbeddedStrings(IAssetNamePattern resourceNamePattern, AssetFileConstructor assetConstructor) : base(resourceNamePattern)
        {
            this.assetConstructor = assetConstructor ?? throw new ArgumentNullException(nameof(assetConstructor));
        }

        /// <summary>
        /// Create part that matches <paramref name="resourceNamePattern"/> to existing files 
        /// and <paramref name="keyNamePattern"/> to to lines found in those files.
        /// 
        /// Derives file extension from the <paramref name="resourceNamePattern"/> and searches for matching
        /// <see cref="AssetFileConstructor"/> from LocalizationTextReaderBuilder.
        /// </summary>
        /// <param name="resourceNamePattern"></param>
        /// <param name="keyNamePattern"></param>
        public AssetLoaderPartEmbeddedStrings(IAssetNamePattern resourceNamePattern, IAssetKeyNamePolicy keyNamePattern) : base(resourceNamePattern)
        {
            this.assetConstructor = AssetFileConstructors.FileFormat(resourceNamePattern.Pattern, keyNamePattern);
        }
        public AssetLoaderPartEmbeddedStrings(IAssetNamePattern resourceNamePattern, string keyNamePattern) : this(resourceNamePattern, new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartEmbeddedStrings(string resourceNamePattern, string keyNamePattern) : this(new AssetNamePattern(resourceNamePattern), new AssetNamePattern(keyNamePattern)) { }
        public AssetLoaderPartEmbeddedStrings(string resourceNamePattern, IAssetKeyNamePolicy keyNamePattern) : this(new AssetNamePattern(resourceNamePattern), keyNamePattern) { }

        public override IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            // Assembly was named
            string assemblyName;
            if (parameters.TryGetValue("assembly", out assemblyName) && assemblyName != null)
            {
                Assembly assembly;
                if (!AssemblyMap.TryGetValue(assemblyName, out assembly)) assembly = Assembly.Load(assemblyName);

                if (assembly == null) return null;

                // Make name
                string resourceName = Pattern.BuildName(newParameters);

                // A required part is missing
                if (resourceName == null) return null;

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    if (stream != null) return assetConstructor(stream, newParameters);
            }

            // Assembly wasn't named and there are multiple
            foreach (var _assembly in Assemblies)
            {
                // Set assembly
                newParameters["assembly"] = _assembly.GetName().Name;

                // Make name
                string resourceName = Pattern.BuildName(newParameters);

                // A required part is missing
                if (resourceName == null) continue;

                using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
                    if (stream != null) return assetConstructor(stream, newParameters);
            }
            return null;
        }
    }

}
