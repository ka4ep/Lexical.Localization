// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization.Plurality
{
    /// <summary>
    /// Description of plurality rule.
    /// 
    /// <list type="bullet">
    ///   <item><see cref="IPluralRuleEvaluatable"/></item>
    /// </list>
    /// </summary>
    public interface IPluralRule
    {
        /// <summary>
        /// Metadata info record.
        /// </summary>
        PluralRuleInfo Info { get; }
    }

    /// <summary>
    /// Interface for classes that evaluate whether argument value and text matches the plurality case.
    /// </summary>
    public interface IPluralRuleEvaluatable : IPluralRule
    {
        /// <summary>
        /// Evaluate whether the rule applies to <paramref name="number"/>.
        /// </summary>
        /// <param name="number">numeric and text representation of numberic value</param>
        /// <returns>true or false</returns>
        bool Evaluate(IPluralNumber number);
    }

    /// <summary>
    /// Hash-equals comparer that can compare without boxing.
    /// </summary>
    public class IPluralRuleComparer : IEqualityComparer<IPluralRule>
    {
        private static readonly IEqualityComparer<IPluralRule> instance = new IPluralRuleComparer(PluralRuleInfo.Comparer.Default);

        /// <summary>
        /// Default instance.
        /// </summary>
        public static IEqualityComparer<IPluralRule> Default => instance;

        /// <summary>
        /// Plural rule comparer
        /// </summary>
        public readonly IEqualityComparer<PluralRuleInfo> InfoComparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public IPluralRuleComparer(IEqualityComparer<PluralRuleInfo> comparer)
        {
            InfoComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// Compare for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(IPluralRule x, IPluralRule y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return InfoComparer.Equals(x.Info, y.Info);
        }

        /// <summary>
        /// Get hashcode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(IPluralRule obj)
            => obj == null ? 0 : InfoComparer.GetHashCode(obj.Info);
    }

}
