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
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for IEnumerable{ILine}}.
    /// </summary>
    public static partial class LinesExtensions
    {
        static ILineFormat DefaultPolicy = LineParameterPrinter.Default;

        /// <summary>
        /// Convert <paramref name="lines"/> to asset key lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IString>> ToStringLines(this IEnumerable<ILine> lines, ILineFormat lineFormat)
            => lines.Select(line => new KeyValuePair<string, IString>((lineFormat ?? DefaultPolicy).Print(line), line.GetString()));

        /// <summary>
        /// Convert <paramref name="lines"/> to asset key lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IString>> ToStringLines(this IEnumerable<KeyValuePair<ILine, string>> lines, ILineFormat lineFormat, IStringFormatParser valueParser)
            => lines.Select(line => new KeyValuePair<string, IString>((lineFormat ?? DefaultPolicy).Print(line.Key), valueParser.Parse(line.Value)));

        /// <summary>
        /// Convert <paramref name="lines"/> to Key Tree of one level.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public static ILineTree ToLineTree(this IEnumerable<ILine> lines, ILineFormat lineFormat)
        {
            LineTree tree = new LineTree();
            if (lineFormat is ILinePattern pattern)
                tree.AddRange(lines, pattern, lineFormat.GetParameterInfos());
            else
                tree.AddRange(lines);
            return tree;
        }

        /// <summary>
        /// Add lines of key,value pairs to tree. Lines will be added as flat first level nodes
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static ILineTree AddRange(this ILineTree tree, IEnumerable<ILine> lines)
        {
            foreach (var line in lines)
            {
                ILineTree subtree = tree.GetOrCreate(line);

                ILine valueClone;
                if (line.TryCloneValue(out valueClone)) subtree.Values.Add(valueClone);
            }
            return tree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryCloneValue(this ILine line, out ILine value)
        {
            ILine valuePart;
            if (line == null || !line.TryGetStringPart(out valuePart)) { value = default; return false; }

            // Clone value
            ILine valueClone = null;
            if (valuePart is ILineString lineValue) { value = new LineStringPart(null, null, lineValue.String); return true; }
            if (valueClone == null && valuePart is ILineParameterEnumerable lineParameters)
            {
                foreach (ILineParameter lineParameter in lineParameters)
                    if (lineParameter.ParameterName == "String" && lineParameter.ParameterValue != null)
                    { value = new LineHint(null, null, "String", lineParameter.ParameterValue); return true; }
            }
            if (valueClone == null && valuePart is ILineParameter parameter && parameter.ParameterName == "String" && parameter.ParameterValue != null)
            { value = new LineHint(null, null, "String", parameter.ParameterValue); return true; }

            value = default;
            return false;
        }

        /// <summary>
        /// Default grouping rule.
        /// </summary>
        public static ILinePattern DefaultGroupingRule = new LinePattern("{Culture}/{Location_n}{Assembly}{BaseName_n}/{Type}{Section_n/}/{Key_n}");

        /// <summary>
        /// Add an enumeration of key,value pairs. Each key will constructed a new node.
        /// 
        /// If <paramref name="groupingRule"/> the nodes are laid out in the order occurance of name pattern parts.
        /// 
        /// For example grouping pattern "{Type}/{Culture}{anysection}{Key}" would order nodes as following:
        /// <code>
        ///  "type:MyController": {
        ///      "key:Success": "Success",
        ///      "culture:en:key:Success": "Success",
        ///  }
        /// </code>
        /// 
        /// Non-capture parts such as "/" in pattern "{Section}/{Culture}", specify separator of tree node levels.
        /// 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="lines"></param>
        /// <param name="groupingRule"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        public static ILineTree AddRange(this ILineTree tree, IEnumerable<ILine> lines, ILinePattern groupingRule, IParameterInfos parameterInfos) // Todo separate to sortRule + groupingRule
        {
            // Use another method
            //if (groupingRule == null) { node.AddRange(lines); return node; }
            StructList16<ILineParameter> parameters = new StructList16<ILineParameter>(null);
            foreach (var line in lines)
            {
                parameters.Clear();
                line.GetParameterParts(ref parameters);
                parameterListSorter.Reverse(ref parameters);

                // Key for the current level. 
                ILine levelKey = null;
                // Build levels with this collection
                List<ILine> key_levels = new List<ILine>();
                // Visit both lists concurrently
                ILineParameter next_parameter = default;
                if (groupingRule != null)
                {
                    foreach (var part in groupingRule.AllParts)
                    {
                        // Is not a capture part
                        if (part.CaptureIndex < 0)
                        {
                            // Non-capture part has "/", go to next level. eg. "{nn}/{nn}"
                            if (part.Text != null && part.Text.Contains("/")) { if (levelKey != null) key_levels.Add(levelKey); levelKey = null; }
                            // Next part
                            continue;
                        }
                        // Capture part has "/" in prefix, start next level
                        if (levelKey != null && part.PrefixSeparator.Contains("/")) { if (levelKey != null) key_levels.Add(levelKey); levelKey = null; }

                        // Look ahead to see if there is a parameter that matches this capture part
                        int next_parameter_ix = -1;
                        for (int ix = 0; ix < parameters.Count; ix++)
                        {
                            // Copy 
                            next_parameter = parameters[ix];
                            // Already added before
                            if (next_parameter.ParameterName == null) continue;
                            // Get name
                            string parameter_name = next_parameter.ParameterName;
                            // Parameter matches the name in the pattern's capture part
                            if (parameter_name == part.ParameterName) { next_parameter_ix = ix; break; }
                            // Matches with "anysection"
                            //IParameterInfo info;
                            //if (part.ParameterName == "anysection" && ParameterInfos.Default.TryGetValue(parameter_name, out info) && info.IsSection) { next_parameter_ix = ix; break; }
                        }
                        // No matching parameter for this capture part
                        if (next_parameter_ix < 0) continue;

                        // This part is canonical, hint, or parameter.
                        if (!next_parameter.IsNonCanonicalKey(parameterInfos))
                        {
                            // There (may be) are other canonical parts between part_ix and next_part_is. We have to add them here.
                            for (int ix = 0; ix < next_parameter_ix; ix++)
                            {
                                // Copy
                                ILineParameter parameter = parameters[ix];
                                // Has been added before
                                if ((parameter.ParameterName == null) || parameter.IsNonCanonicalKey(parameterInfos)) continue;
                                // Append to level's key
                                levelKey = LineAppender.NonResolving.Create(levelKey, LineArguments.ToArguments(parameter));
                                // Mark handled
                                parameters[ix] = unused;
                            }
                        }
                        // Append to level's key
                        levelKey = LineAppender.NonResolving.Create(levelKey, LineArguments.ToArguments(next_parameter));
                        // Mark handled
                        parameters[next_parameter_ix] = unused;
                        // Yield level
                        if (part.PostfixSeparator.Contains("/")) { key_levels.Add(levelKey); levelKey = null; }
                    }
                }

                // Append rest of the parameters
                for (int ix = 0; ix < parameters.Count; ix++)
                {
                    // Copy
                    ILineParameter parameter = parameters[ix];
                    if (parameter.ParameterName != null && parameter.ParameterName != "String") levelKey = LineAppender.NonResolving.Create(levelKey, LineArguments.ToArguments(parameter));
                }

                // yield levelKey
                if (levelKey != null) { key_levels.Add(levelKey); levelKey = null; }

                // Yield line
                tree.AddRecursive(key_levels, line.GetString());
                key_levels.Clear();
                parameters.Clear();
            }

            return tree;
        }

        // Reorder parts according to grouping rule
        static StructListSorter<StructList16<ILineParameter>, ILineParameter> parameterListSorter = new StructListSorter<StructList16<ILineParameter>, ILineParameter>(null);

        static ILineParameter unused = new LineParameter(null, null, null, null);

        /// <summary>
        /// Create an asset that uses <paramref name="lines"/>.
        /// 
        /// Lines are reloaded into the asset if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<ILine> lines)
            => new StringAsset().Add(lines).Load();

        /// <summary>
        /// Convert <paramref name="lines"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<ILine> lines)
            => new KeyLineSource(lines);

        /// <summary>
        /// Convert to dictionary.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="keyComparer">(optional) <see cref="ILine"/> comparer</param>
        /// <returns></returns>
        public static Dictionary<ILine, IString> ToDictionary(this IEnumerable<ILine> lines, IEqualityComparer<ILine> keyComparer = default)
        {
            Dictionary<ILine, IString> result = new Dictionary<ILine, IString>(keyComparer ?? LineComparer.Default);
            foreach (var line in lines)
                if (line != null) result[line] = line.GetString();
            return result;
        }
    }

}
