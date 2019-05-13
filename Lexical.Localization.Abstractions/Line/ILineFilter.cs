// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Filter that can qualify and disqualify <see cref="ILine"/> and <see cref="ILineParameter"/>s.
    /// </summary>
    public interface ILineFilter
    {
    }

    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineFilterEvaluatable : ILineFilter
    {
        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        bool Filter(ILine key);
    }

    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineFilterLinesEvaluatable : ILineFilter
    {
        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        IEnumerable<ILine> Filter(IEnumerable<ILine> lines);
    }

    /// <summary>
    /// <see cref="ILineParameter"/> specific rule.
    /// </summary>
    public interface ILineFilterParameterEvaluatable : ILineFilter
    {
        /// <summary>
        /// Test rule with <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">parameter part of a compared key (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        bool Filter(ILineParameter parameter, int occuranceIndex);
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
    public interface ILineFilterComposite : ILineFilter
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
    }

    /// <summary>
    /// Parameter filter that has occurance index constraint.
    /// </summary>
    public interface ILineFilterParameterOccuranceConstraint : ILineFilterParameterEvaluatable
    {
        /// <summary>
        /// Occurance index this rule applies to. 
        /// 
        /// Occurance counter starts from the root at 0 and increments for every occurance of the <see cref="ILineParameter.ParameterName"/>.
        /// For the effective non-canonical parameter, the index is always 0.
        /// 
        /// (-1 to not be constrained to one occurance index).
        /// </summary>
        int OccuranceIndex { get; }
    }

    /// <summary>
    /// Parameter filter that applies to only one ParameterName.
    /// </summary>
    public interface ILineFilterParameterNameConstraint : ILineFilterParameterEvaluatable
    {
        /// <summary>
        /// Parameter name this rule applies to. (null if isn't constrained to one parameter name)
        /// </summary>
        string ParameterName { get; }
    }

    /// <summary></summary>
    public static class ILineFilterExtensions
    {
        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="key"></param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        public static bool Filter(this ILineFilter filter, ILine key)
            => filter is ILineFilterEvaluatable eval ? eval.Filter(key) : false;

        /// <summary>
        /// Filters lines against filter rules.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        public static IEnumerable<ILine> Filter(this ILineFilter filter, IEnumerable<ILine> lines)
            => filter is ILineFilterLinesEvaluatable eval ? eval.Filter(lines) : _filter(filter, lines);

        static IEnumerable<ILine> _filter(ILineFilter filter, IEnumerable<ILine> lines)
        {
            if (filter is ILineFilterEvaluatable eval)
            {
                foreach (var line in lines)
                    if (eval.Filter(line)) yield return line;
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// Test rule with <paramref name="parameter"/>.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parameter">parameter part of a compared key (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        public static bool Filter(this ILineFilter filter, ILineParameter parameter, int occuranceIndex)
            => filter is ILineFilterParameterEvaluatable eval ? eval.Filter(parameter, occuranceIndex) : false;

        /// <summary>
        /// Set line filter as read-only.
        /// </summary>
        /// <param name="lineFilter"></param>
        /// <returns></returns>
        public static ILineFilter SetReadonly(this ILineFilter lineFilter)
        {
            if (lineFilter is ILineFilterComposite configurable)
            {
                configurable.ReadOnly = true;
                return lineFilter;
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineFilterComposite)}");
        }

        /// <summary>
        /// Add filter rule that validates parameters.
        /// </summary>
        /// <param name="lineFilter"></param>
        /// <param name="rule"></param>
        public static ILineFilter Add(this ILineFilter lineFilter, ILineFilter rule)
        {
            if (lineFilter is ILineFilterComposite configurable) configurable.Add(rule);
            else throw new InvalidOperationException($"Is not {nameof(ILineFilterComposite)}");
            return lineFilter;
        }

        /// <summary>
        /// Add filter rules.
        /// </summary>
        /// <param name="lineFilter"></param>
        /// <param name="rules"></param>
        public static ILineFilter AddRange(this ILineFilter lineFilter, IEnumerable<ILineFilter> rules)
        {
            if (lineFilter is ILineFilterComposite configurable)
            {
                foreach(var rule in rules)
                    configurable.Add(rule);
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineFilterComposite)}");
            return lineFilter;
        }

        /// <summary>
        /// Add filter rules.
        /// </summary>
        /// <param name="lineFilter"></param>
        /// <param name="rules"></param>
        public static ILineFilter AddRange(this ILineFilter lineFilter, params ILineFilter[] rules)
        {
            if (lineFilter is ILineFilterComposite configurable)
            {
                foreach (var rule in rules)
                    configurable.Add(rule);
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineFilterComposite)}");
            return lineFilter;
        }


    }

}
