// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Lexical.Localization
{
    /// <summary>
    /// This class matches loads .resources (.resx) files from assemblies.
    /// 
    /// This component is used as part of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartEmbeddedResourceManager : AssetLoaderPartEmbeddedResources, IAssetLoaderPartResourceManager
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
        public AssetLoaderPartEmbeddedResourceManager(string pattern, IAssetKeyNamePolicy keyNamePolicy = default) : this(new AssetNamePattern(pattern), keyNamePolicy) { }

        /// <summary>
        /// Create <see cref="IAssetLoaderPart"/> that matches <see cref="IAssetKey"/>s to <see cref="IAsset"/> instances that source keys from <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="pattern">file name pattern that detects .resources embedded resources</param>
        /// <param name="keyNamePolicy">(optional) key name policy to use within .resx file</param>
        public AssetLoaderPartEmbeddedResourceManager(IAssetNamePattern pattern, IAssetKeyNamePolicy keyNamePolicy = default) : base(pattern)
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
            // Assembly was named
            string assemblyName;
            if (parameters.TryGetValue("assembly", out assemblyName) && assemblyName != null)
            {
                Assembly assembly;
                if (!AssemblyMap.TryGetValue(assemblyName, out assembly)) assembly = Assembly.Load(assemblyName);

                if (assembly == null) return null;

                // Make name
                string resourceName = Pattern.BuildName(parameters);

                // A required part is missing
                if (resourceName == null) return null;

                // Cut ".resources" from the name
                if (resourceName.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase)) resourceName = resourceName.Substring(0, resourceName.Length - ".resources".Length);

                // Create ResourceManager
                ResourceManager resourceManager = new ResourceManager(resourceName, assembly, ResourceSetType);

                // Create Asset that adapts ResourceManager
                return new ResourceManagerAsset(resourceManager, KeyNamePolicy);
            }

            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            // Assembly wasn't named and there are multiple
            foreach (var _assembly in Assemblies)
            {
                // Set assembly
                newParameters["assembly"] = _assembly.GetName().Name;

                // Make name
                string resourceName = Pattern.BuildName(newParameters);

                // A required part is missing
                if (resourceName == null) continue;

                // Cut ".resources" from the name
                if (resourceName.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase)) resourceName = resourceName.Substring(0, resourceName.Length - ".resources".Length);

                // Create ResourceManager
                ResourceManager resourceManager = new ResourceManager(resourceName, _assembly, ResourceSetType);

                // Create Asset that adapts ResourceManager
                return new ResourceManagerAsset(resourceManager, KeyNamePolicy);
            }
            return null;
        }

    }

}
