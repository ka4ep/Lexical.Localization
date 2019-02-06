// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexical.Asset
{
    /// <summary>
    /// This class matches loads file binary resources (byte[]) from file system.
    /// 
    /// This component is used as part of <see cref="IAssetLoader"/>.
    /// </summary>
    public class AssetLoaderPartFileResources : IAssetLoaderPart
    {
        /// <summary>
        /// Filename pattern to match file resources against.
        /// </summary>
        public IAssetNamePattern Pattern { get; internal set; }

        /// <summary>
        /// Options. Add Paths here.
        /// </summary>
        public IAssetLoaderPartOptions Options { get; set; }

        public AssetLoaderPartFileResources(string pattern) : this(new AssetNamePattern(pattern)) { }

        public AssetLoaderPartFileResources(IAssetNamePattern pattern)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.Options = new AssetLoaderPartOptions();
        }

        public virtual IEnumerable<IReadOnlyDictionary<string, string>> ListLoadables(IReadOnlyDictionary<string, string> parameters)
        {
            var paths = Options.GetPaths();
            if (paths == null) yield break;
            foreach (string path in paths)
            {
                int startIx = path.Length + 1;
                Regex regex = Pattern.BuildRegex(parameters);
                foreach (string filename in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    Match match = regex.Match(filename, startIx, filename.Length - startIx); // Match(_, index) is bugged, but Match(_, index, length) works.
                    if (!match.Success) continue;
                    NamePatternMatch m = new NamePatternMatch(Pattern);
                    if (parameters != null) m.Add(parameters);
                    m.Add(match);
                    m._fixPartsWithOccurancesAndLastOccurance();
                    if (m.Success) yield return m;
                }
            }
        }

        public virtual IAsset Load(IReadOnlyDictionary<string, string> parameters)
        {
            var paths = Options.GetPaths();
            if (paths == null) return null;

            // Create copy of parameters
            Dictionary<string, string> newParameters = new Dictionary<string, string>();
            foreach (var kp in parameters) newParameters[kp.Key] = kp.Value;

            return new FileResourcesAsset(paths?.ToArray(), Pattern, newParameters);
        }

        public override string ToString()
        {
            var paths = Options.GetPaths();
            string pathsStr = paths == null ? "" : String.Join(", ", paths);
            return $"{GetType().Name}({pathsStr}/{Pattern.ToString()})";
        }
    }

    class FileResourcesAsset : IAssetResourceProvider
    {
        string[] paths;
        IAssetNamePattern pattern;
        IReadOnlyDictionary<string, string> parameters;

        public FileResourcesAsset(string[] paths, IAssetNamePattern pattern, IReadOnlyDictionary<string, string> parameters)
        {
            this.paths = paths ?? throw new ArgumentNullException(nameof(pattern));
            this.pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
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
            string fileName = pattern.BuildName(match);

            // Required part of pattern is missing
            if (fileName == null) return null;

            // Try loading
            foreach (String path in paths)
            {
                // Append path
                string fullpath = Path.Combine(path, fileName);

                // Test if file exists
                if (!File.Exists(fullpath)) continue;

                // Open to read
                return new FileStream(fullpath, FileMode.Open);
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
