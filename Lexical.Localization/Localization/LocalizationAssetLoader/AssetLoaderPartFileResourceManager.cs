// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization;
using System;
using System.Collections.Generic;
using System.Resources;

namespace Lexical.Localization
{
    /// <summary>
    /// This class matches loads .resources (.resx) files from files.
    /// 
    /// This component is used as part of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartFileResourceManager : AssetLoaderPartFileResources, IAssetLoaderPartResourceManager
    {
        /// <summary>
        /// This option determines which <see cref="ResourceSet"/> to use when loading <see cref="ResourceManager"/>.
        /// The type <see cref="ResourceSet"/> assignable type, or null.
        /// </summary>
        public Type ResourceSetType { get => Options.Get<Type>(AssetLoaderPartResourceManagerExtensions.Key_ResourceSetType); set => Options.Set<Type>(AssetLoaderPartResourceManagerExtensions.Key_ResourceSetType, value); }

        /// <summary>
        /// Key name policy that is used within .resource/.resx file.
        /// </summary>
        public IAssetKeyNamePolicy KeyNamePolicy { get; internal set; }

        /// <summary>
        /// Create <see cref="IAssetLoaderPart"/> that matches <see cref="IAssetKey"/>s to <see cref="IAsset"/> instances that source keys from <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="pattern">file name pattern that detects .resources embedded resources</param>
        /// <param name="keyNamePolicy">(optional) key name policy to use within .resx file</param>
        public AssetLoaderPartFileResourceManager(string pattern, IAssetKeyNamePolicy keyNamePolicy = default) : this(new AssetNamePattern(pattern), keyNamePolicy) { }

        /// <summary>
        /// Create <see cref="IAssetLoaderPart"/> that matches <see cref="IAssetKey"/>s to <see cref="IAsset"/> instances that source keys from <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="pattern">file name pattern that detects .resources embedded resources</param>
        /// <param name="keyNamePolicy">(optional) key name policy to use within .resx file</param>
        public AssetLoaderPartFileResourceManager(IAssetNamePattern pattern, IAssetKeyNamePolicy keyNamePolicy = default) : base(pattern)
        {
            // Create keyname policy for this part
            if (keyNamePolicy == null)
            {
                // Create key name policy where only "section" and "key" parameters are appended. Separator is ".".
                AssetKeyNameProvider keyNameBuilder = new AssetKeyNameProvider().SetParameter("section", true, ".").SetParameter("key", true, ".").SetDefault(false);

                // If the filename pattern doesn't contain "type" section, then add "type" section to the name builder.
                if (!pattern.PartMap.ContainsKey("type")) keyNameBuilder.SetParameter("type", true, ".");

                // If the filename pattern doesn't contain "resource" section, then add "resource" section to the name builder.
                if (!pattern.PartMap.ContainsKey("resource")) keyNameBuilder.SetParameter("resource", true, ".");

                keyNamePolicy = keyNameBuilder;
            }

            this.KeyNamePolicy = keyNamePolicy;
        }

        public override IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Make name
            string filename = Pattern.BuildName(parameters);

            // Required part of pattern is missing
            if (filename == null) return null;

            // Cut ".resources" from the filename
            if (filename.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase)) filename = filename.Substring(0, filename.Length - ".resources".Length);

            // Paths
            IList<string> paths = Options.GetPaths();

            // Return null
            if (paths == null || paths.Count == 0) return null;

            // Return one asset
            if (paths.Count == 1)
            {
                // Create ResourceManager
                ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(filename, paths[0], ResourceSetType);

                // Create Asset that adapts ResourceManager
                return new ResourceManagerAsset(resourceManager, KeyNamePolicy);
            }

            // Create composition
            List<IAsset> assets = new List<IAsset>(paths.Count);
            foreach(string path in paths)
            {
                // Create ResourceManager
                ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(filename, path, ResourceSetType);

                // Create Asset that adapts ResourceManager
                assets.Add(new ResourceManagerAsset(resourceManager, KeyNamePolicy));
            }
            return new AssetComposition(assets);

        }

    }

}
