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
using Lexical.Localization.LocalizationFile2;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    public static partial class AssetKeyExtensions__
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
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> keyValues)
            => AddRange(node, keyValues, groupingRule: null);

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="keyValues"></param>
        /// <param name="groupingPatternText"></param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> keyValues, string groupingPatternText)
            => AddRange(node, keyValues, groupingRule: new AssetNamePattern(groupingPatternText));

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
        /// <param name="parametrizer"></param>
        /// <param name="keyValues"></param>
        /// <param name="groupingRule">(optional)</param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<IAssetKey, string>> keyValues, IAssetNamePattern groupingRule) // TOdo separate to sortRule + groupingRule
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
            foreach (var kp in keyValues)
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


    }
}
