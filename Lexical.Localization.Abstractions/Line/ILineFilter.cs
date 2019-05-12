// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineFilter
    {
        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if <paramref name="key"/> passes the filter, containing all required parameters</returns>
        bool Filter(ILine key);

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        IEnumerable<ILine> Filter(IEnumerable<ILine> lines);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ILineFilterEnumerable : ILineFilter, IEnumerable<ILineFilter>
    {
    }

    /// <summary>
    /// Configurable line filter.
    /// </summary>
    public interface ILineFilterConfigurable : ILineFilter
    {
        /// <summary>
        /// Is collection in read-only state.
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Add rule that is validated against complete <see cref="ILine"/>.
        /// </summary>
        /// <param name="filterRule"></param>
        void Add(ILineFilter filterRule);

        /// <summary>
        /// Add parameter specific rule.
        /// </summary>
        /// <param name="parameterRule"></param>
        void Add(ILineFilterParameterRule parameterRule);
    }

    /// <summary>
    /// <see cref="ILineParameter"/> specific rule.
    /// </summary>
    public interface ILineFilterParameterRule
    {
        /// <summary>
        /// Parameter name this rule applies to
        /// </summary>
        string ParameterName { get; }

        /// <summary>
        /// Occurance index this rule applies to. 
        /// 
        /// Use -1 to have rule be applied against any occurance.
        /// 
        /// Occurance counter starts from the root at 0 and increments for every occurance of the <see cref="ParameterName"/>.
        /// For the effective non-canonical parameter, the index is always 0.
        /// </summary>
        int OccuranceIndex { get; }

        /// <summary>
        /// Apply rule to <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="parameterValue">value that occured in the compared key (note "" for empty), or null if value did not occur</param>
        /// <returns></returns>
        bool Filter(string parameterValue);
    }
}
