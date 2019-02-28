//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public static partial class KeyLinesExtensions
    {
        static IAssetKeyNamePolicy DefaultPolicy = AssetKeyNameProvider.Default;

        /// <summary>
        /// Convert <paramref name="lines"/> to asset key lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToStringLines(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, IAssetKeyNamePolicy policy)
            => lines.Select(line => new KeyValuePair<string, string>((policy ?? DefaultPolicy).BuildName(line.Key), line.Value));

        /// <summary>
        /// Convert <paramref name="lines"/> to Key Tree of one level.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, IAssetKeyNamePolicy namePolicy)
            => namePolicy is IAssetNamePattern pattern ?
                new KeyTree(new Key("root", ""), null).AddRange(lines, pattern) :
                new KeyTree(new Key("root", ""), null).AddRange(lines);

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> lines)
            => AddRange(node, lines, groupingRule: null);

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lines"></param>
        /// <param name="groupingPatternText"></param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> lines, string groupingPatternText)
            => AddRange(node, lines, groupingRule: new AssetNamePattern(groupingPatternText));

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// 
        /// If <paramref name="groupingRule"/> the nodes are laid out in the order occurance of name pattern parts.
        /// 
        /// For example grouping pattern "{type}/{culture}{anysection}{key}" would order nodes as following:
        /// <code>
        ///  "type:MyController": {
        ///      "key:Success": "Success",
        ///      "culture:en:key:Success": "Success",
        ///      "culture:fi:key:Success": "Onnistui"
        ///  }
        /// </code>
        /// 
        /// Non-capture parts such as "/" in pattern "{section}/{culture}", specify separator of tree node levels.
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="lines"></param>
        /// <param name="groupingRule">(optional)</param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> lines, IAssetNamePattern groupingRule) // TOdo separate to sortRule + groupingRule
        {
            // Create comparer that can compare TreeNode and argument's keys
            ParametrizedComparer comparer = new ParametrizedComparer();
            // Create orderer
            PartComparer partComparer = new PartComparer().AddParametersToSortOrder("root");
            if (groupingRule != null)
            {
                foreach (IAssetNamePatternPart part in groupingRule.CaptureParts)
                    partComparer.AddParametersToSortOrder(part.ParameterName);
            }
            else
            {
                partComparer.AddParametersToSortOrder("culture");
            }

            List<PartComparer.Part> partList = new List<PartComparer.Part>(10);
            List<IAssetKey> key_parts = new List<IAssetKey>();
            foreach (var kp in lines)
            {
                foreach (IAssetKey part in kp.Key.ArrayFromRoot())
                {
                    string parameterName = part.GetParameterName(), parameterValue = part.Name;
                    if (parameterName == null || parameterValue == null) continue;
                    bool isCanonical = part is IAssetKeyCanonicallyCompared, isNonCanonical = part is IAssetKeyNonCanonicallyCompared;
                    if (!isCanonical && !isNonCanonical) continue;
                    partList.Add(new PartComparer.Part(parameterName, parameterValue, isCanonical, isNonCanonical));
                }
                // Reorder parts according to grouping rule
                partList.Sort(partComparer);

                // Segment according to grouping rule
                if (groupingRule == null)
                {
                    key_parts.AddRange(partList.Select(p => p.CreateKey()));
                }
                else
                {
                    int part_ix = 0;
                    Key constructedKey = null;
                    foreach (var part in groupingRule.AllParts)
                    {
                        // yield constructedKey into the array due to separator
                        if (constructedKey != null && part.PrefixSeparator.Contains("/")) { key_parts.Add(constructedKey); constructedKey = null; }

                        // Is not a capture part
                        if (part.CaptureIndex < 0) { if (constructedKey != null) { key_parts.Add(constructedKey); constructedKey = null; } continue; }

                        // Look ahead to see if there is part for this parameter name
                        int ixx = -1;
                        for (int ix = part_ix; ix < partList.Count; ix++)
                        {
                            // Detected part
                            if (partList[ix].name == part.ParameterName) { ixx = ix; break; }
                        }

                        // Detected part for parameter name in the grouping rule
                        if (ixx >= 0) for (; part_ix <= ixx; part_ix++) constructedKey = new Key(constructedKey, partList[part_ix].name, partList[part_ix].value);

                        // yield constructedKey into the array due to separator
                        if (constructedKey != null && part.PrefixSeparator.Contains("/")) { key_parts.Add(constructedKey); constructedKey = null; }
                    }

                    // yield constructedKey into the array.
                    if (constructedKey != null) { key_parts.Add(constructedKey); constructedKey = null; }

                    // Add rest of the keys
                    for (; part_ix <= partList.Count; part_ix++)
                        key_parts.Add(partList[part_ix].CreateKey(constructedKey));
                }

                // Add recursively
                node.AddRecursive(key_parts, kp.Value);

                // Clear
                partList.Clear();
                key_parts.Clear();
            }

            return node;
        }

        /// <summary>
        /// Create an asset that uses <paramref name="trees"/>.
        /// 
        /// Lines are reloaded into the asset if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="hintSource"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, string hintSource = null)
            => new LoadableLocalizationAsset().AddKeyLinesSource(lines, hintSource).Load();

        /// <summary>
        /// Convert <paramref name="lines"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="sourceHint">(optional)</param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, string sourceHint = null)
            => new LocalizationKeyLinesSource(lines, sourceHint);

        /// <summary>
        /// Add prefix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="key">(optional)</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> AddKeyPrefix(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, IAssetKey key)
        {
            if (key == null || key.GetParameterCount() == 0) return lines;
            return lines.Select(line => new KeyValuePair<IAssetKey, string>(key.ConcatIfNew(line.Key), line.Value));
        }

        /// <summary>
        /// Add suffix parameters to each key.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="key">(optional)</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> AddKeySuffix(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, IAssetKey key)
        {
            if (key == null || key.GetParameterCount() == 0) return lines;
            return lines.Select(line => new KeyValuePair<IAssetKey, string>(line.Key.ConcatIfNew(key), line.Value));
        }

        /// <summary>
        /// Convert to dictionary.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="keyComparer">(optional) <see cref="IAssetKey"/> comparer</param>
        /// <returns></returns>
        public static Dictionary<IAssetKey, string> ToDictionary(this IEnumerable<KeyValuePair<IAssetKey, string>> lines, IEqualityComparer<IAssetKey> keyComparer = default)
        {
            Dictionary<IAssetKey, string> result = new Dictionary<IAssetKey, string>(keyComparer??AssetKeyComparer.Default);
            foreach (var line in lines)
                if (line.Key != null) result[line.Key] = line.Value;
            return result;
        }
    }

    /// <summary>
    /// TreeNode is an intermediate model for writing text files
    /// 
    /// Reorganize parts so that non-canonicals parts, so that "root" is first, then "culture", and then others by parameter name.
    /// </summary>
    internal class PartComparer : IComparer<PartComparer.Part>
    {
        private static readonly PartComparer instance = new PartComparer().AddParametersToSortOrder("root", "culture");
        public static PartComparer Default => instance;

        public readonly List<string> order = new List<string>();

        public PartComparer()
        {
        }

        public PartComparer AddParametersToSortOrder(IEnumerable<string> parameters)
        {
            this.order.AddRange(parameters);
            return this;
        }

        public PartComparer AddParametersToSortOrder(params string[] parameters)
        {
            this.order.AddRange(parameters);
            return this;
        }

        public int Compare(Part x, Part y)
        {
            // canonical parts cannot be reordered between themselves.
            if (x.isCanonical || y.isCanonical) return 0;
            int xix = order.IndexOf(x.name);
            int yix = order.IndexOf(y.name);
            if (xix == yix) return 0;
            if (xix < 0) xix = Int32.MaxValue;
            if (yix < 0) yix = Int32.MaxValue;
            return xix - yix;
        }

        public struct Part
        {
            public string name;
            public string value;
            public bool isCanonical;
            public bool isNonCanonical;

            public Part(string name, string value, bool isCanonical, bool isNonCanonical)
            {
                this.name = name;
                this.value = value;
                this.isCanonical = isCanonical;
                this.isNonCanonical = isNonCanonical;
            }

            public Key CreateKey(Key prev = default)
                => isCanonical ? new Key.Canonical(prev, name, value) :
                   isNonCanonical ? new Key.NonCanonical(prev, name, value) :
                   new Key(prev, name, value);
        }
    }
}
