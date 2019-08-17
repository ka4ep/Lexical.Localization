// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.7.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Helper class for converting <see cref="ILine"/> against <see cref="ILinePattern"/>s to file path <see cref="String"/> quickly.
    /// </summary>
    public class FilePatternHelper
    {
        /// <summary>
        /// Dictionary of features and pattern entries
        /// </summary>
        Dictionary<Key, PatternEntry> patterns = new Dictionary<Key, PatternEntry>();

        public FilePatternHelper(IEnumerable<ILinePattern> filePatterns)
        {
            foreach(ILinePattern filePattern in filePatterns)
            {
                // Extract features and convert into key
                // Create PatternEntry.
            }
        }

        /// <summary>
        /// Convert key into filenames.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PatternEntry[] GetFiles(ILine key)
        {
            return null;
        }

        Key ExtractFeatures(ILinePattern pattern)
        {
            // Required parts
            foreach (ILinePatternPart part in pattern.CaptureParts)
            {
                if (!part.Required) continue;
            }

            // Non-required parts
            foreach (ILinePatternPart part in pattern.CaptureParts)
            {
                if (part.Required) continue;
            }

            return default;
        }

        /// <summary>
        /// Features and a key.
        /// 
        /// Most typical Key contains just one part { "Culture", required, non-canonical }.
        /// </summary>
        public struct Key
        {

        }

        /// <summary>
        /// Information about a pattern, and files
        /// </summary>
        public class PatternEntry
        {
            public ILinePattern FilePattern;
            public string[] Files;
        }
    }
}
