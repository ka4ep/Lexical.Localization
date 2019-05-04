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
    /// Filters keys and key-lines against filter rules.
    /// </summary>
    public class AssetKeyFilter : IAssetKeyFilter
    {
        /// <summary>
        /// List of generic filters. Null if none is assigned.
        /// </summary>
        protected List<IAssetKeyFilter> genericFilters;

        /// <summary>
        /// List of parameter name specific rules. Null if none is assigned.
        /// </summary>
        protected MapList<KeyValuePair<string, int>, ParameterRule> parameterRules;

        /// <summary>
        /// Add generic filter rule.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>this</returns>
        public AssetKeyFilter Rule(IAssetKeyFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (genericFilters == null) genericFilters = new List<IAssetKeyFilter>();
            genericFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Add parameter rule.
        /// 
        /// </summary>
        /// <param name="parameterRule"></param>
        /// <returns>this</returns>
        public AssetKeyFilter Rule(ParameterRule parameterRule)
        {
            if (parameterRule == null) throw new ArgumentNullException(nameof(parameterRule));
            if (parameterRules==null) parameterRules = new MapList<KeyValuePair<string, int>, ParameterRule>(KeyValuePairEqualityComparer<string, int>.Default);
            parameterRules.Add(new KeyValuePair<string, int>(parameterRule.ParameterName, parameterRule.OccuranceIndex), parameterRule);
            return this;
        }

        /// <summary>
        /// Add key rule.
        /// 
        /// If <paramref name="filterKey"/> has a parameter, then the parameter is required from the compared key. 
        /// Unless the value in the parameter of the <paramref name="filterKey"/> is "", then the compared key must not have that parameter, or it must be "".
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public AssetKeyFilter KeyRule(ILinePart filterKey)
        {
            // Break filterKey into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();
            filterKey.GetEffectiveParameters(ref list);

            // Add rules
            foreach ((string parameterName, int occuranceIndex, string parameterValue) in list)
            {
                if (String.IsNullOrEmpty(parameterValue)) Rule(new EmptyRule(parameterName, occuranceIndex));
                else Rule(new ValueRule(parameterName, occuranceIndex, parameterValue));
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
        public bool Filter(ILinePart key)
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
            key.GetEffectiveParameters(ref list);

            // Evaluate rules
            if (this.parameterRules != null)
                foreach (var parameterRules in this.parameterRules)
                {
                    List<ParameterRule> rules = parameterRules.Value;
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
        public IEnumerable<ILinePart> Filter(IEnumerable<ILinePart> keys)
        {
            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();

            foreach (ILinePart key in keys)
            {
                // Apply generic filters
                if (genericFilters != null)
                {
                    foreach (var filter in genericFilters)
                        if (!filter.Filter(key)) continue;
                }

                // Apply parameter filters
                list.Clear();
                key.GetEffectiveParameters(ref list);

                // Evaluate rules
                bool keyOk = true;
                if (this.parameterRules != null)
                    foreach (var parameterRules in this.parameterRules)
                    {
                        List<ParameterRule> rules = parameterRules.Value;
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
        /// Filters lines against filter rules.
        /// 
        /// Each key in <paramref name="lines"/> is matched against every <see cref="genericFilters"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// Each key in <paramref name="lines"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<ILinePart, IFormulationString>> Filter(IEnumerable<KeyValuePair<ILinePart, IFormulationString>> lines)
        {
            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();

            foreach (var line in lines)
            {
                // Apply generic filters
                if (genericFilters != null)
                {
                    foreach (var filter in genericFilters)
                        if (!filter.Filter(line.Key)) continue;
                }

                // Apply parameter filters
                list.Clear();
                line.Key.GetEffectiveParameters(ref list);

                // Evaluate rules
                bool keyOk = true;
                if (this.parameterRules != null)
                    foreach (var parameterRules in this.parameterRules)
                    {
                        List<ParameterRule> rules = parameterRules.Value;
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
                yield return line;
            }
        }

        /// <summary>
        /// Filter by <paramref name="keyMatch"/>.
        /// 
        /// If <see cref="IParameterPatternMatch.Success"/> is false then return false.
        /// 
        /// The whole <paramref name="key"/> is matched against every <see cref="genericFilters"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// The <paramref name="key"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="keyMatch"></param>
        /// <returns>true if <paramref name="keyMatch"/> passes the filter</returns>
        public bool Filter(IParameterPatternMatch keyMatch)
        {
            // 
            if (!keyMatch.Success) return false;

            // Apply generic filters
            if (genericFilters != null)
            {
                ILinePart filterKey = keyMatch.ToKey();
                foreach (var filter in genericFilters)
                    if (!filter.Filter(filterKey)) return false;
            }

            // Apply parameter filters

            // Break key into effective non-canonical parameters, and to canonical parameters and occurance index
            // (ParameterName, occuranceIndex, ParameterValue)
            StructList12<(string, int, string)> list = new StructList12<(string, int, string)>();
            for (int i = 0; i < keyMatch.Pattern.CaptureParts.Length; i++)
            {
                IParameterPatternPart part = keyMatch.Pattern.CaptureParts[i];
                string partValue = keyMatch.PartValues[i];
                if (partValue == null) continue;
                list.Add((part.ParameterName, part.OccuranceIndex, partValue));
            }

            // Evaluate rules
            if (this.parameterRules != null)
                foreach (var parameterRules in this.parameterRules)
                {
                    List<ParameterRule> rules = parameterRules.Value;
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
                            if (pi.IsSection) { parameterValue = pv; break; }
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
            sb.Append(nameof(AssetKeyFilter));
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

        /// <summary>
        /// Filter rule for a specific <see cref="ParameterName"/>
        /// </summary>
        public abstract class ParameterRule
        {
            /// <summary>
            /// Parameter name to apply rule to.
            /// </summary>
            public readonly string ParameterName;

            /// <summary>
            /// The occurance of this parameter this rule applies to. 
            /// 
            /// Occurance counter starts from the root at 0 and increments for every occurance of the <see cref="ParameterName"/>.
            /// For non-canonical parameters the index is always 0.
            /// </summary>
            public readonly int OccuranceIndex;

            /// <summary>
            /// Create rule for a specific <paramref name="parameterName"/> and <paramref name="occuranceIndex"/>.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            public ParameterRule(string parameterName, int occuranceIndex)
            {
                ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            }

            /// <summary>
            /// Apply rule to a parameter value.
            /// </summary>
            /// <param name="parameterValue">value that occured in the compared key (note "" for empty), or null if value did not occur</param>
            /// <returns></returns>
            public abstract bool Filter(string parameterValue);

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";
        }

        /// <summary>
        /// Filter rule that matches paramter value against <see cref="Pattern"/>.
        /// </summary>
        public class RegexRule : ParameterRule
        {
            /// <summary>
            /// Regex pattern
            /// </summary>
            public readonly Regex Pattern;

            /// <summary>
            /// Create rule that validates parameter value against <see cref="Regex"/>.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="pattern"></param>
            public RegexRule(string parameterName, int occuranceIndex, Regex pattern) : base(parameterName, occuranceIndex)
            {
                Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => parameterValue == null ? false : Pattern.Match(parameterValue).Success;

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(Pattern)}={Pattern})";
        }

        /// <summary>
        /// Filter rule that matches paramter value against one value.
        /// </summary>
        public class ValueRule : ParameterRule
        {
            /// <summary>
            /// Accepted value
            /// </summary>
            public readonly string Value;

            /// <summary>
            /// Create rule that validates parameter value against a string.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="parameterValue">expected value</param>
            public ValueRule(string parameterName, int occuranceIndex, string parameterValue) : base(parameterName, occuranceIndex)
            {
                Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => Value == parameterValue;

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(Value)}={Value})";
        }

        /// <summary>
        /// Filter rule that matches parameter value against a set of valid values.
        /// </summary>
        public class ValuesRule : ParameterRule
        {
            /// <summary>
            /// List of accepted values.
            /// </summary>
            public IEnumerable<string> AcceptedValues => values;

            HashSet<string> values;

            /// <summary>
            /// Creates a rule that validates parameter value against a set of values.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="parameterValues">expected values, one of these must be in the key</param>
            public ValuesRule(string parameterName, int occuranceIndex, IEnumerable<string> parameterValues) : base(parameterName, occuranceIndex)
            {
                if (parameterValues == null) throw new ArgumentNullException(nameof(parameterValues));
                values = new HashSet<string>(parameterValues);
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => parameterValue == null ? false : values.Contains(parameterValue);

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(AcceptedValues)}={String.Join(", ", AcceptedValues)})";
        }

        /// <summary>
        /// Filter rule that asserts that the parameter is null or "".
        /// </summary>
        public class EmptyRule : ParameterRule
        {
            /// <summary>
            /// Create rule that approves emptry (null or "") parameter value.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex"></param>
            public EmptyRule(string parameterName, int occuranceIndex) : base(parameterName, occuranceIndex)
            {
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => String.IsNullOrEmpty(parameterValue);

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";
        }

    }
}
