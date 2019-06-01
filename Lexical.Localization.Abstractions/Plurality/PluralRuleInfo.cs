// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Record about metainfo of a <see cref="IPluralRule"/>.
    /// </summary>
    public struct PluralRuleInfo
    {
        /// <summary>
        /// No values
        /// </summary>
        public static readonly PluralRuleInfo Empty = new PluralRuleInfo(null, null, null, null, -1);

        /// <summary>
        /// Name of ruleset as assembly qualified type name.
        ///
        /// e.g. "Lexical.Localization.CLDR35".
        /// 
        /// If null, then info is used for querying for a set of rules, and RuleSet is left open.
        /// </summary>
        public readonly string RuleSet;

        /// <summary>
        /// Name of plurality category. 
        /// 
        /// E.g. "cardinal", "ordinal", "optional".
        /// 
        /// If null, then info is used for querying for a set of rules, and Category is left open.
        /// </summary>
        public readonly string Category;

        /// <summary>
        /// Name of culture.
        /// 
        /// E.g. "fi"
        /// 
        /// If null, then info is used for querying for a set of rules, and Culture is left open.
        /// </summary>
        public readonly string Culture;

        /// <summary>
        /// Name of plurality case.
        /// 
        /// E.g. "zero", "one", "few", "many", "other"
        /// 
        /// If null, then info is used for querying with case constraint.
        /// </summary>
        public readonly string Case;

        /// <summary>
        /// Info whether rule is optional.
        /// 
        /// <list type="bullet">
        /// <item>0  = no</item>
        /// <item>1  = yes</item>
        /// <item>-1 = unknown, used for querying</item>
        /// </list>
        /// </summary>
        public readonly int Optional;

        /// <summary>
        /// Create plural rule info
        /// </summary>
        /// <param name="ruleSet">(optional) rule set</param>
        /// <param name="category">(optional) category</param>
        /// <param name="culture">(optional) culture</param>
        /// <param name="case">(optional) case</param>
        /// <param name="optional">-1 (unknown), 0 (no), 1 (yes)</param>
        public PluralRuleInfo(string ruleSet, string category, string culture, string @case, int optional)
        {
            RuleSet = ruleSet;
            Category = category;
            Culture = culture;
            Case = @case;
            Optional = optional;
            if (optional < -1 || optional > 1) throw new ArgumentException("Must be -1, 0, 1", nameof(optional));
            int hash = 0;
            if (ruleSet != null) hash ^= 3 * ruleSet.GetHashCode();
            if (category != null) hash ^= 5 * category.GetHashCode();
            if (culture != null) hash ^= 7 * culture.GetHashCode();
            if (@case != null) hash ^= 11 * @case.GetHashCode();
            if (optional == 0) hash ^= 0x4234923; else if (optional == 1) hash ^= 0xbbabab;
            this.hashcode = hash;
        }

        private readonly int hashcode;

        /// <summary>
        /// If this info is a filter criteria, then tests whether <paramref name="info"/> matches the criteria.
        /// If any of the local fields is null, then that value is not compared.
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool FilterMatch(PluralRuleInfo info)
        {
            if (RuleSet != null && RuleSet != info.RuleSet) return false;
            if (Category != null && Category != info.Category) return false;
            if (Culture != null && Culture != info.Culture) return false;
            if (Case != null && Case != info.Case) return false;
            if (Optional != -1 && Optional != info.Optional) return false;
            return true;
        }

        /// <summary>
        /// Change ruleset value.
        /// </summary>
        /// <param name="newRuleSet"></param>
        /// <returns></returns>
        public PluralRuleInfo ChangeRuleSet(string newRuleSet)
            => new PluralRuleInfo(newRuleSet, Category, Culture, Case, Optional);

        /// <summary>
        /// Print debug info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (RuleSet != null) { sb.Append(nameof(RuleSet)); sb.Append('='); sb.Append(RuleSet); }
            if (Category != null) { if (sb.Length > 0) sb.Append(", "); sb.Append(nameof(Category)); sb.Append('='); sb.Append(Category); }
            if (Culture != null) { if (sb.Length > 0) sb.Append(", "); sb.Append(nameof(Culture)); sb.Append('='); sb.Append(Culture); }
            if (Case != null) { if (sb.Length > 0) sb.Append(", "); sb.Append(nameof(Case)); sb.Append('='); sb.Append(Case); }
            if (Optional != -1) { if (sb.Length > 0) sb.Append(", "); sb.Append(nameof(Optional)); sb.Append('='); sb.Append(Optional); }
            return sb.ToString();
        }

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PluralRuleInfo other)
            {
                if (other.hashcode != hashcode) return false;
                return ((RuleSet == null && other.RuleSet == null) || (RuleSet == other.RuleSet)) &&
                    ((Category == null && other.Category == null) || (Category == other.Category)) &&
                    ((Culture == null && other.Culture == null) || (Culture == other.Culture)) &&
                    ((Case == null && other.Case == null) || (Case == other.Case)) &&
                    (Optional == other.Optional);
            }
            return false;
        }

        /// <summary>
        /// Get cached hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => hashcode;

        /// <summary>
        /// Hash-equals comparer that can compare without boxing.
        /// </summary>
        public class Comparer : IEqualityComparer<PluralRuleInfo>
        {
            private static readonly Comparer instance = new Comparer();

            /// <summary>
            /// Default instance.
            /// </summary>
            public static Comparer Default => instance;

            /// <summary>
            /// Compare for equality
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(PluralRuleInfo x, PluralRuleInfo y)
            {
                if (y.hashcode != x.hashcode) return false;
                return ((x.RuleSet == null && y.RuleSet == null) || (x.RuleSet == y.RuleSet)) &&
                    ((x.Category == null && y.Category == null) || (x.Category == y.Category)) &&
                    ((x.Culture == null && y.Culture == null) || (x.Culture == y.Culture)) &&
                    ((x.Case == null && y.Case == null) || (x.Case == y.Case)) &&
                    (x.Optional == y.Optional);
            }

            /// <summary>
            /// Get cached hashcode.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(PluralRuleInfo obj)
                => obj.hashcode;
        }
    }

}
