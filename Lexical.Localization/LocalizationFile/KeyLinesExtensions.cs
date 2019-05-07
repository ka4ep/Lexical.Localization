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
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{KeyValuePair{ILine, string}}"/>.
    /// </summary>
    public static partial class KeyLinesExtensions
    {
        static IParameterPolicy DefaultPolicy = KeyPrinter.Default;

        /// <summary>
        /// Convert <paramref name="lines"/> to asset key lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> ToStringLines(this IEnumerable<KeyValuePair<ILine, IFormulationString>> lines, IParameterPolicy policy)
            => lines.Select(line => new KeyValuePair<string, IFormulationString>((policy ?? DefaultPolicy).Print(line.Key), line.Value));

        /// <summary>
        /// Convert <paramref name="lines"/> to asset key lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="keyPolicy"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, IFormulationString>> ToStringLines(this IEnumerable<KeyValuePair<ILine, string>> lines, IParameterPolicy keyPolicy, ILocalizationStringFormatParser valueParser)
            => lines.Select(line => new KeyValuePair<string, IFormulationString>((keyPolicy ?? DefaultPolicy).Print(line.Key), valueParser.Parse(line.Value)));

        /// <summary>
        /// Convert <paramref name="lines"/> to Key Tree of one level.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public static IKeyTree ToKeyTree(this IEnumerable<KeyValuePair<ILine, IFormulationString>> lines, IParameterPolicy namePolicy)
        {
            KeyTree tree = new KeyTree(Key.Root, null);
            if (namePolicy is IParameterPattern pattern)
                tree.AddRange(lines, pattern);
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
        public static IKeyTree AddRange(this IKeyTree tree, IEnumerable<KeyValuePair<ILine, IFormulationString>> lines)
        {
            foreach (var line in lines)
            {
                tree.GetOrCreate(line.Key).Values.Add(line.Value);
            }
            return tree;
        }
        
        /// <summary>
        /// Default grouping rule.
        /// </summary>
        public static IParameterPattern DefaultGroupingRule = new ParameterPattern("{Culture}/{Location_n}{Assembly}{Resource_n}/{Type}{Section_n/}/{Key_n}");

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
        /// <param name="node"></param>
        /// <param name="lines"></param>
        /// <param name="groupingRule"></param>
        /// <returns></returns>
        public static IKeyTree AddRange(this IKeyTree node, IEnumerable<KeyValuePair<ILine, IFormulationString>> lines, IParameterPattern groupingRule) // Todo separate to sortRule + groupingRule
        {
            // Use another method
            //if (groupingRule == null) { node.AddRange(lines); return node; }

            StructList16<Parameter> parameters = new StructList16<Parameter>(null);
            foreach (var line in lines)
            {
                // Convert key into parts
                for (ILine k = line.Key; k != null; k = k.GetPreviousPart())
                {
                    string parameterName = k.GetParameterName(), parameterValue = k.GetParameterValue();
                    if (parameterName == null || parameterValue == null) continue;
                    bool isCanonical = k is ILineCanonicalKey, isNonCanonical = k is ILineNonCanonicalKey;
                    if (!isCanonical && !isNonCanonical) continue;

                    // Overwrite previously assigned non-canonical parameter
                    if (isNonCanonical)
                    {
                        // Find previous occurance
                        int ixx = -1;
                        for (int i = 0; i < parameters.Count; i++) if (parameters[i].parameterName == parameterName) { ixx = i; break; }
                        // Overwrite previous occurance, only left-most is effective
                        if (ixx >= 0) { parameters[ixx] = new Parameter(parameterName, parameterValue, isCanonical, isNonCanonical); continue; }
                    }
                    // Add parameter to list
                    parameters.Add(new Parameter(parameterName, parameterValue, isCanonical, isNonCanonical));
                }
                parameterListSorter.Reverse(ref parameters);

                // Key for the current level. 
                Key levelKey = null;
                // Build levels with this collection
                List<ILine> key_levels = new List<ILine>();
                // Visit both lists concurrently
                Parameter next_parameter = default;
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
                            if (next_parameter.isUnused) continue;
                            // Get name
                            string parameter_name = next_parameter.parameterName;
                            // Parameter matches the name in the pattern's capture part
                            if (parameter_name == part.ParameterName) { next_parameter_ix = ix; break; }
                            // Matches with "anysection"
                            IParameterInfo info;
                            if (part.ParameterName == "anysection" && ParameterInfos.Default.TryGetValue(parameter_name, out info) && info.IsSection) { next_parameter_ix = ix; break; }
                        }
                        // No matching parameter for this capture part
                        if (next_parameter_ix < 0) continue;

                        // This part is canonical.
                        if (next_parameter.isCanonical)
                        {
                            // There (may be) are other canonical parts between part_ix and next_part_is. We have to add them here.
                            for (int ix = 0; ix < next_parameter_ix; ix++)
                            {
                                // Copy
                                Parameter parameter = parameters[ix];
                                // Has been added before
                                if (parameter.isUnused || !parameter.isCanonical) continue;
                                // Append to level's key
                                levelKey = parameter.CreateKey(levelKey);
                                // Mark handled
                                parameters[ix] = Parameter.Unused;
                            }
                        }
                        // Append to level's key
                        levelKey = next_parameter.CreateKey(levelKey);
                        // Mark handled
                        parameters[next_parameter_ix] = Parameter.Unused;
                        // Yield level
                        if (part.PostfixSeparator.Contains("/")) { key_levels.Add(levelKey); levelKey = null; }
                    }
                }

                // Append rest of the parameters
                for (int ix = 0; ix < parameters.Count; ix++)
                {
                    // Copy
                    Parameter parameter = parameters[ix];
                    if (!parameter.isUnused) levelKey = parameter.CreateKey(levelKey);
                }

                // yield levelKey
                if (levelKey != null) { key_levels.Add(levelKey); levelKey = null; }

                // Yield line
                node.AddRecursive(key_levels, line.Value);
                key_levels.Clear();
                parameters.Clear();
            }

            return node;
        }

        // Reorder parts according to grouping rule
        static StructListSorter<StructList16<Parameter>, Parameter> parameterListSorter = new StructListSorter<StructList16<Parameter>, Parameter>(null);

        /// <summary>
        /// Create an asset that uses <paramref name="lines"/>.
        /// 
        /// Lines are reloaded into the asset if <see cref="IAssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAsset ToAsset(this IEnumerable<KeyValuePair<ILine, IFormulationString>> lines)
            => new LocalizationAsset().Add(lines).Load();

        /// <summary>
        /// Convert <paramref name="lines"/> to <see cref="IAssetSource"/>..
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IAssetSource ToAssetSource(this IEnumerable<KeyValuePair<ILine, IFormulationString>> lines)
            => new LocalizationKeyLinesSource(lines);

        /// <summary>
        /// Convert to dictionary.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="keyComparer">(optional) <see cref="ILine"/> comparer</param>
        /// <returns></returns>
        public static Dictionary<ILine, IFormulationString> ToDictionary(this IEnumerable<KeyValuePair<ILine, IFormulationString>> lines, IEqualityComparer<ILine> keyComparer = default)
        {
            Dictionary<ILine, IFormulationString> result = new Dictionary<ILine, IFormulationString>(keyComparer ?? LineComparer.Default);
            foreach (var line in lines)
                if (line.Key != null) result[line.Key] = line.Value;
            return result;
        }
    }

    internal struct Parameter
    {
        public static Parameter Unused = new Parameter { parameterName = null, parameterValue = null, flags = 0 };

        public string parameterName;
        public string parameterValue;

        public bool isUnused => (flags & 1) == 0;
        public bool isCanonical => (flags & 2) == 2;
        public bool isNonCanonical => (flags & 4) == 4;

        /// <summary>
        /// 1 - unallocated
        /// 2 - is canonical
        /// 4 - is non canonical
        /// </summary>
        public int flags;

        public Parameter(string parameterName, string parameterValue, bool isCanonical, bool isNonCanonical)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
            int _flags = 1;
            if (isCanonical) _flags |= 2;
            if (isNonCanonical) _flags |= 4;
            this.flags = _flags;
        }

        public Key CreateKey(Key prev = default)
            => ((flags & 2) == 2) ? new Key.Canonical(prev, parameterName, parameterValue) :
               ((flags & 4) == 4) ? new Key.NonCanonical(prev, parameterName, parameterValue) :
               new Key(prev, parameterName, parameterValue);

        public override string ToString()
            => parameterName == null || parameterValue == null ? null : parameterName + ":" + parameterValue;
    }

}
