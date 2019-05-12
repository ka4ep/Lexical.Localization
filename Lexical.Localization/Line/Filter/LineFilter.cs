// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Filters lines against filter rules.
    /// </summary>
    public class LineFilter : ILineFilter
    {
        /// <summary>
        /// List of generic filters. Null if none is assigned.
        /// </summary>
        protected List<ILineFilter> genericFilters;

        /// <summary>
        /// List of parameter name specific rules. Null if none is assigned.
        /// </summary>
        protected MapList<KeyValuePair<string, int>, LineFilterParameterRule> parameterRules;

        /// <summary>
        /// (optional) Parameter infos for determining if parameter is key.
        /// </summary>
        protected IReadOnlyDictionary<string, IParameterInfo> parameterInfos;

        /// <summary>
        /// Create line filter. 
        /// </summary>
        /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key. <see cref="ParameterInfos.Default"/> for default infos.</param>
        public LineFilter(IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
        {
            this.parameterInfos = parameterInfos;
        }

        /// <summary>
        /// Add generic filter rule.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>this</returns>
        public LineFilter Rule(ILineFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (genericFilters == null) genericFilters = new List<ILineFilter>();
            genericFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Add parameter rule.
        /// 
        /// </summary>
        /// <param name="parameterRule"></param>
        /// <returns>this</returns>
        public LineFilter Rule(LineFilterParameterRule parameterRule)
        {
            if (parameterRule == null) throw new ArgumentNullException(nameof(parameterRule));
            if (parameterRules == null) parameterRules = new MapList<KeyValuePair<string, int>, LineFilterParameterRule>(KeyValuePairEqualityComparer<string, int>.Default);
            parameterRules.Add(new KeyValuePair<string, int>(parameterRule.ParameterName, parameterRule.OccuranceIndex), parameterRule);
            return this;
        }

        /// <summary>
        /// Add the parameters of <paramref name="filterKey"/> as required criteria.
        /// 
        /// Every parameter of <paramref name="filterKey"/> will be required to match in the lines that will be compared with this filter. 
        /// If a parameterValue in the <paramref name="filterKey"/> is "", then the compared key must not have that parameter, or it must be "".
        /// </summary>
        /// <param name="filterKey">parameters</param>
        /// <returns></returns>
        public LineFilter Rule(ILine filterKey)
        {
            // Break filterKey into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();
            filterKey.GetEffectiveParameters(ref list, parameterInfos);

            // Add rules
            foreach ((string parameterName, int occuranceIndex, string parameterValue) in list)
            {
                if (String.IsNullOrEmpty(parameterValue)) Rule(new LineFilterParameterRule.IsEmpty(parameterName, occuranceIndex));
                else Rule(new LineFilterParameterRule.IsEqualTo(parameterName, occuranceIndex, parameterValue));
            }

            return this;
        }

        /// <summary>
        /// Tests if there are any rules in the filter
        /// </summary>
        public bool HasRules => (genericFilters != null && genericFilters.Count > 0) || (parameterRules != null && parameterRules.Count > 0);

        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// 
        /// The whole <paramref name="key"/> is matched against every <see cref="genericFilters"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// The <paramref name="key"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if <paramref name="key"/> passes the filter, containing all required parameters</returns>
        public bool Filter(ILine key)
        {
            // Apply generic filters
            if (genericFilters != null)
            {
                foreach (var filter in genericFilters)
                    if (!filter.Filter(key)) return false;
            }

            // Apply parameter filters

            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();
            key.GetEffectiveParameters(ref list, parameterInfos);

            // Evaluate rules
            if (this.parameterRules != null)
                foreach (var parameterRules in this.parameterRules)
                {
                    List<LineFilterParameterRule> rules = parameterRules.Value;
                    if (rules.Count == 0) continue;
                    KeyValuePair<string, int> parameterKey = parameterRules.Key;

                    // Find parameter value
                    string parameterValue = null;
                    for (int i = 0; i < list.Count; i++)
                    {
                        (string pn, int oc, string pv) = list[i];
                        if (pn == parameterKey.Key && oc == parameterKey.Value) { parameterValue = pv; break; }
                    }

                    // Apply rules
                    bool ok = false;
                    foreach (var rule in rules)
                    {
                        ok |= rule.Filter(parameterValue);
                        if (ok) break;
                    }
                    if (!ok) return false;
                }

            // Nothing failed.
            return true;
        }

        /// <summary>
        /// Filters lines against filter rules.
        /// 
        /// Each key in <paramref name="keys"/> is matched against every <see cref="genericFilters"/>. 
        /// If every one of them passes the filter then key is yielded.
        /// 
        /// Each key in <paramref name="keys"/> is broken into parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<ILine> Filter(IEnumerable<ILine> keys)
        {
            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();

            foreach (ILine key in keys)
            {
                // Apply generic filters
                if (genericFilters != null)
                {
                    foreach (var filter in genericFilters)
                        if (!filter.Filter(key)) continue;
                }

                // Apply parameter filters
                list.Clear();
                key.GetEffectiveParameters(ref list, parameterInfos);

                // Evaluate rules
                bool keyOk = true;
                if (this.parameterRules != null)
                    foreach (var parameterRules in this.parameterRules)
                    {
                        List<LineFilterParameterRule> rules = parameterRules.Value;
                        if (rules.Count == 0) continue;
                        KeyValuePair<string, int> parameterKey = parameterRules.Key;

                        // Find parameter value
                        string parameterValue = null;
                        for (int i = 0; i < list.Count; i++)
                        {
                            (string pn, int oc, string pv) = list[i];
                            if (pn == parameterKey.Key && oc == parameterKey.Value) { parameterValue = pv; break; }
                        }

                        // Apply rules
                        bool ok = false;
                        foreach (var rule in rules)
                        {
                            ok |= rule.Filter(parameterValue);
                            if (ok) break;
                        }
                        keyOk &= ok;
                        if (!keyOk) break;
                    }
                yield return key;
            }
        }

        /// <summary>
        /// Filter by <paramref name="keyMatch"/>.
        /// 
        /// If <see cref="ILinePatternMatch.Success"/> is false then return false.
        /// 
        /// The whole <paramref name="keyMatch"/> is matched against every <see cref="genericFilters"/>. 
        /// If one mismatches then returns false.
        /// 
        /// The <paramref name="keyMatch"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="keyMatch"></param>
        /// <returns>true if <paramref name="keyMatch"/> passes the filter</returns>
        public bool Filter(ILinePatternMatch keyMatch)
        {
            // 
            if (!keyMatch.Success) return false;

            // Apply generic filters
            if (genericFilters != null)
            {
                ILine filterKey = keyMatch.ToKey();
                foreach (var filter in genericFilters)
                    if (!filter.Filter(filterKey)) return false;
            }

            // Apply parameter filters

            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();
            for (int i = 0; i < keyMatch.Pattern.CaptureParts.Length; i++)
            {
                ILinePatternPart part = keyMatch.Pattern.CaptureParts[i];
                string partValue = keyMatch.PartValues[i];
                if (partValue == null) continue;
                list.Add((part.ParameterName, part.OccuranceIndex, partValue));
            }

            // Evaluate rules
            if (this.parameterRules != null)
                foreach (var parameterRules in this.parameterRules)
                {
                    List<LineFilterParameterRule> rules = parameterRules.Value;
                    if (rules.Count == 0) continue;
                    KeyValuePair<string, int> parameterKey = parameterRules.Key;

                    // Find parameter value
                    string parameterValue = null;
                    for (int i = 0; i < list.Count; i++)
                    {
                        (string pn, int oc, string pv) = list[i];
                        if (oc != parameterKey.Value) continue;
                        if (pn == "anysection")
                        {
                            IParameterInfo pi;
                            if (!ParameterInfos.Default.TryGetValue(parameterKey.Key, out pi)) continue;
                            //if (/*pi.IsSection*/) { parameterValue = pv; break; }
                        }
                        else
                        {
                            if (pn == parameterKey.Key) { parameterValue = pv; break; }
                        }
                    }

                    // Apply rules
                    bool ok = false;
                    foreach (var rule in rules)
                    {
                        ok |= rule.Filter(parameterValue);
                        if (ok) break;
                    }
                    if (!ok) return false;
                }

            // Nothing failed.
            return true;
        }

        /// <summary>
        /// Print filter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(LineFilter));
            sb.Append("(");
            if (genericFilters != null)
            {
                sb.Append("GenericFilters=[");
                sb.Append(String.Join(", ", genericFilters));
                sb.Append("]");
            }
            if (parameterRules != null)
            {
                if (genericFilters != null) sb.Append(", ");
                sb.Append("ParameterRules=[");
                sb.Append(String.Join(", ", parameterRules.Values.SelectMany(_ => _)));
                sb.Append("]");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
