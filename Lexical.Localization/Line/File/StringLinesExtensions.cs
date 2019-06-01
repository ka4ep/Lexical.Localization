//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for IEnumerable&lt;KeyValuePair&lt;string, IFormatString&gt;&gt;.
    /// </summary>
    public static class StringLinesExtensions
    {
        /// <summary>
        /// Parse string key of each line into <see cref="ILine"/> by using <paramref name="lineFormat"/>.
        /// 
        /// If parse fails, then skips the key, doesn't throw exception.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"><see cref="ILineFormatParser"/> parses string to line.</param>
        /// <returns>lines with <see cref="ILine"/> keys</returns>
        public static IEnumerable<ILine> ToLines(this IEnumerable<KeyValuePair<string, IString>> lines, ILineFormat lineFormat)
        {
            foreach (var line in lines)
            {
                ILine l;
                if (lineFormat.TryParse(line.Key, out l))
                    yield return line.Value == null ? l : l = l.String(line.Value);
            }
        }

        /// <summary>
        /// Parse string key of each line and put into <see cref="ILineTree"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"><see cref="ILineFormatParser"/> parses strings to lines.</param>
        /// <returns></returns>
        public static ILineTree ToLineTree(this IEnumerable<KeyValuePair<string, IString>> lines, ILineFormat lineFormat)
            => LineTree.Create(lines.ToLines(lineFormat), null, lineFormat.GetParameterInfos());

        /// <summary>
        /// Convert <paramref name="lines"/> to asset.
        /// 
        /// Lines are reloaded into the asset if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"><see cref="ILineFormatParser"/> parses strings to lines.</param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<KeyValuePair<string, IString>> lines, ILineFormat lineFormat)
            => new LocalizationAsset().Add(lines, lineFormat).Load();

        /// <summary>
        /// Convert <paramref name="lines"/> to <see cref="IAssetSource"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<KeyValuePair<string, IString>> lines, ILineFormat policy)
            => new StringLinesSource(lines, policy);

        /// <summary>
        /// Add prefix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IString>> AddKeyPrefix(this IEnumerable<KeyValuePair<string, IString>> lines, string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || lines == null) return lines;
            return lines.Select(line => new KeyValuePair<string, IString>(prefix + line.Key, line.Value));
        }

    }
}

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Extensions for IEnumerable&lt;KeyValuePair&lt;string, IFormatString&gt;&gt;.
    /// </summary>
    public static class StringLinesExtensions
    { 
        /// <summary>
        /// Parse string key of each line into <see cref="ILine"/> by using <paramref name="lineFormat"/>.
        /// 
        /// If parse fails, then skips the key, doesn't throw exception.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"><see cref="ILineFormatParser"/> parses string to line.</param>
        /// <param name="valueParser"></param>
        /// <returns>lines with <see cref="ILine"/> keys</returns>
        public static IEnumerable<ILine> ToLines(this IEnumerable<KeyValuePair<string, string>> lines, ILineFormat lineFormat, IStringFormatParser valueParser)
        {
            foreach (var line in lines)
            {
                ILine l;
                if (lineFormat.TryParse(line.Key, out l))
                    yield return line.Value == null ? l : l.String(valueParser.Parse(line.Value));
            }
        }
    }
}
