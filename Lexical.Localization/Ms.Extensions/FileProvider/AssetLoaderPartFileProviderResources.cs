// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Ms.Extensions
{
    /// <summary>
    /// Provides <see cref="IAssetResourceProvider"/> assets from <see cref="IFileProvider"/> files.
    /// Use as a member of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartFileProviderResources : IAssetLoaderPart
    {
        public IFileProvider fileProvider;
        public IAssetNamePattern Pattern { get; internal set; }
        public IAssetLoaderPartOptions Options { get; set; }

        public AssetLoaderPartFileProviderResources(IFileProvider fileProvider, string pattern) : this(fileProvider, new AssetNamePattern(pattern)) { }
        public AssetLoaderPartFileProviderResources(IFileProvider fileProvider, IAssetNamePattern pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.Options = new AssetLoaderPartOptions();
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        public IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> initialParameters)
        {
            Regex regex = null; string matchString = null;
            if (Pattern.TestSuccess(initialParameters, true))
            {
                // Match string
                matchString = Pattern.BuildName(initialParameters);
            } else
            {
                bool hasInitialParameters = false;
                if (initialParameters != null && initialParameters.Count > 0) foreach (var kp in initialParameters) if (kp.Value != null) { hasInitialParameters = true; break; }
                regex = hasInitialParameters ? Pattern.BuildRegex(initialParameters) : Pattern.Regex;
            }

            Queue<string> queue = new Queue<string>();

            // Add root or add paths
            IList<string> paths = Options.GetPaths();
            if (paths == null) queue.Enqueue("");
            else
            {
                foreach (var path in paths)
                    queue.Enqueue(path);
            }

            while (queue.Count > 0)
            {
                string path = queue.Dequeue();
                foreach (IFileInfo fileInfo in fileProvider.GetDirectoryContents(path))
                {
                    string filepath = path.Length == 0 ? fileInfo.Name : path + "/" + fileInfo.Name;
                    if (fileInfo.IsDirectory)
                    {
                        queue.Enqueue(filepath);
                    }
                    else
                    {
                        if (matchString != null)
                        {
                            if (filepath == matchString) yield return initialParameters;
                        }
                        else
                        {
                            Match match = regex.Match(filepath);
                            if (!match.Success) continue;

                            NamePatternMatch m = new NamePatternMatch(Pattern);
                            if (initialParameters != null) m.Add(initialParameters);
                            m.Add(match);
                            m._fixPartsWithOccurancesAndLastOccurance();
                            if (m.Success) yield return m;
                        }
                    }
                }
            }
        }

        public virtual IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            // Build name
            string resourceName = Pattern.BuildName(parameters);

            // Required part is missing.
            if (resourceName == null) return null;

            // Test if file exists
            var info = fileProvider.GetFileInfo(resourceName);
            if (info == null || !info.Exists) return null;

            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            // Create reader asset
            return new FileProviderResourceAsset(fileProvider, Pattern, newParameters);
        }

        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            HashSet<string> visited = null;
            foreach (var file_params in ListLoadables(null))
            {
                string culture;
                if (!file_params.TryGetValue("culture", out culture)) continue;
                if (visited != null && visited.Contains(culture)) continue;
                if (visited == null) visited = new HashSet<string>();
                visited.Add(culture);
                CultureInfo ci;
                try { ci = CultureInfo.GetCultureInfo(culture); } catch (Exception) { ci = null; }
                yield return ci;
            }
        }

        public override string ToString()
            => $"{GetType().Name}({Pattern.ToString()})";
    }

    class FileProviderResourceAsset : IAssetResourceProvider
    {
        IReadOnlyDictionary<string, string> parameters;
        IAssetNamePattern pattern;
        IFileProvider fileProvider;

        public FileProviderResourceAsset(IFileProvider fileProvider, IAssetNamePattern pattern, IReadOnlyDictionary<string, string> parameters)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }        

        public Stream OpenStream(IAssetKey key)
        {
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

            // Test if file exists
            var fileInfo = fileProvider.GetFileInfo(resourceName);
            if (fileInfo == null || !fileInfo.Exists) return null;

            // Try open stream
            return fileInfo.CreateReadStream();
        }

        public byte[] GetResource(IAssetKey key)
        {
            using (var s = OpenStream(key))
                return ReadFully(s);
        }

        protected static byte[] ReadFully(Stream s)
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
