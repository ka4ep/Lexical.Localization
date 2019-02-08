// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           27.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// This class matches loads embedded binary resources (byte[]) from assemblies.
    /// 
    /// This component is used as part of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartEmbeddedResources : IAssetLoaderPart
    {
        /// <summary>
        /// Filename pattern to match embedded resources against.
        /// </summary>
        public IAssetNamePattern Pattern { get; internal set; }

        /// <summary>
        /// Options.
        /// </summary>
        public IAssetLoaderPartOptions Options { get; set; }

        /// <summary>
        /// The array instance that was used for building <see cref="assemblyMap"/>.
        /// </summary>
        IList<Assembly> assemblyMapSource;

        /// <summary>
        /// Quick lookup map. Keys are both fullname and name of assemblies.
        /// </summary>
        IReadOnlyDictionary<string, Assembly> assemblyMap;

        /// <summary>
        /// Property of assembly map, lookup table.
        /// </summary>
        protected internal IReadOnlyDictionary<string, Assembly> AssemblyMap => assemblyMapSource == Options.GetAssemblies() ? assemblyMap : BuildAssemblyMap(ref assemblyMap, ref assemblyMapSource);

        /// <summary>
        /// Property of associated assemblies.
        /// </summary>
        protected internal IList<Assembly> Assemblies => Options.GetAssemblies() ?? no_assemblies;
        static Assembly[] no_assemblies = new Assembly[0];

        /// <summary>
        /// Is there "assembly" in Pattern.
        /// </summary>
        bool patternHasAssemblyParameter;

        public AssetLoaderPartEmbeddedResources(string pattern) : this(new AssetNamePattern(pattern)) { }
        public AssetLoaderPartEmbeddedResources(IAssetNamePattern pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.Options = new AssetLoaderPartOptions();
            this.patternHasAssemblyParameter = Pattern.PartMap.ContainsKey("assembly");
        }

        IReadOnlyDictionary<string, Assembly> BuildAssemblyMap(ref IReadOnlyDictionary<string, Assembly> assemblyMap, ref IList<Assembly> assemblyMapSource)
        {
            Dictionary<string, Assembly> result = new Dictionary<string, Assembly>();
            IList<Assembly> assemblies = Options.GetAssemblies();
            if (assemblies != null)
            {
                foreach(var assembly in assemblies)
                {
                    result[assembly.GetName().Name] = assembly;
                    result[assembly.GetName().FullName] = assembly;
                }
            }
            assemblyMapSource = assemblies;
            assemblyMap = result;
            return result;
        }

        public virtual IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> parameters)
        {
            // Read assembly
            string assemblyName;
            Assembly assembly;

            // Assembly is assigned
            if (parameters != null && parameters.TryGetValue("assembly", out assemblyName) && assemblyName != null)
            {
                // Find assembly from map or load
                if (!AssemblyMap.TryGetValue(assemblyName, out assembly))
                {
                    // Load assembly
                    assembly = Assembly.Load(assemblyName);
                    // Failed to load
                    if (assembly == null) throw new AssetException($"Failed to load assembly {assemblyName}, please add the assembly using .Options.AddAssemblies(assembly).");
                }

                if (Pattern.TestSuccess(parameters, true))
                {
                    // Match string
                    string fullname = Pattern.BuildName(parameters);
                    var info = assembly.GetManifestResourceInfo(fullname);
                    if (info != null) yield return parameters;
                }
                else
                {
                    // Match with regex
                    Regex regex = Pattern.BuildRegex(parameters);
                    foreach (string resourceName in assembly.GetManifestResourceNames())
                    {
                        Match match = regex.Match(resourceName);
                        if (!match.Success) continue;
                        NamePatternMatch m = new NamePatternMatch(Pattern);
                        m.Add(parameters);
                        m.Add(match);
                        m._fixPartsWithOccurancesAndLastOccurance();
                        if (m.Success) yield return m;
                    }
                }
                yield break;
            }

            // Assembly is not assigned

            // Copy parameters
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (parameters != null) foreach (var kp in parameters) if (kp.Value != null) param[kp.Key] = kp.Value;

            // Search assembly from Options.Assemblies
            foreach (Assembly _assembly in Assemblies)
            {
                param["assembly"] = _assembly.GetName().Name;
                if (Pattern.TestSuccess(parameters, true))
                {
                    // Match string
                    string fullname = Pattern.BuildName(param);
                    var info = _assembly.GetManifestResourceInfo(fullname);
                    if (info != null) yield return param;
                }
                else
                {
                    // Match regex
                    Regex regex = Pattern.BuildRegex(param);
                    foreach (string resourceName in _assembly.GetManifestResourceNames())
                    {
                        Match match = regex.Match(resourceName);
                        if (!match.Success) continue;
                        NamePatternMatch m = new NamePatternMatch(Pattern);
                        m.Add(param);
                        m.Add(match);
                        m._fixPartsWithOccurancesAndLastOccurance();
                        if (m.Success) yield return m;
                    }
                }
            }
        }

        public virtual IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            string assemblyName;
            IList<Assembly> assemblies = Assemblies;

            // Assembly is assigned
            if (parameters.TryGetValue("assembly", out assemblyName))
            {
                Assembly assembly = null;
                // Find assembly from map 
                if (!AssemblyMap.TryGetValue(assemblyName, out assembly)) assembly = Assembly.Load(assemblyName);
                if (assembly != null) assemblies = new Assembly[] { assembly };
            }

            // Failed to load
            if (assemblies == null || assemblies.Count == 0) return null;

            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            return new EmbeddedResourcesAsset(assemblies?.ToArray(), Pattern, newParameters);
        }

        public override string ToString()
            => $"{GetType().Name}({Pattern.ToString()}, {string.Join(", ", Assemblies.Select(a => a.GetName().Name))})";

    }

    class EmbeddedResourcesAsset : IAssetResourceProvider
    {
        IAssetNamePattern pattern;
        Assembly[] assemblies;
        IReadOnlyDictionary<string, string> parameters;

        public EmbeddedResourcesAsset(Assembly[] assemblies, IAssetNamePattern pattern, IReadOnlyDictionary<string, string> parameters)
        {
            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            this.pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public Stream OpenStream(IAssetKey key)
        {
            // Check key
            IAssetNamePatternMatch match = pattern.Match(key);
            foreach (var kp in parameters)
            {
                // Test has value
                if (kp.Value == null) continue;

                // Get part
                IAssetNamePatternPart part;
                if (!match.Pattern.PartMap.TryGetValue(kp.Key, out part)) continue;

                // Read values
                string match_value_from_key = match.PartValues[part.CaptureIndex];
                string asset_value = kp.Value;

                if (match_value_from_key == null) match.PartValues[part.CaptureIndex] = kp.Value;

                // mismatch
                else if (match_value_from_key != asset_value)
                    return null;
            }
            match._fixPartsWithOccurancesAndLastOccurance();
            if (!match.Success) return null;
            
            // Make string
            string resourceName = pattern.BuildName(match);

            // Required part of pattern is missing
            if (resourceName == null) return null;

            foreach (Assembly assembly in assemblies)
            {
                Stream s = assembly.GetManifestResourceStream(resourceName);
                if (s != null) return s;
            }
            return null;
        }

        public byte[] GetResource(IAssetKey key)
        {
            using (var s = OpenStream(key))
                return ReadFully(s);
        }

        static byte[] ReadFully(Stream s)
        {
            if (s == null) return null;

            // Try to read stream completely.
            int len_ = (int)s.Length;
            if (len_ > 2147483647) throw new IOException("File size over 2GB");
            byte[] data = new byte[len_];

            // Read chunks
            int ix = 0;
            while (ix < len_)
            {
                int count = s.Read(data, ix, len_ - ix);

                // "returns zero (0) if the end of the stream has been reached."
                if (count == 0) break;

                ix += count;
            }
            if (ix == len_) return data;
            throw new AssetException("Failed to read stream fully");
        }
    }


}
 