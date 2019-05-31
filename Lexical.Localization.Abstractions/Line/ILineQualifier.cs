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
    /// Measures qualities of <see cref="ILine"/>s and <see cref="ILineParameter"/>.
    /// </summary>
    public interface ILineQualifier
    {
    }

    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a qualification criteria.
    /// </summary>
    public interface ILineQualifierEvaluatable : ILineQualifier
    {
        /// <summary>
        /// Qualify <paramref name="key"/> against the criteria.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        bool Qualify(ILine key);
    }

    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineQualifierLinesEvaluatable : ILineQualifier
    {
        /// <summary>
        /// Qualifies lines against rules.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        IEnumerable<ILine> Qualify(IEnumerable<ILine> lines);
    }

    /// <summary>
    /// Can evaluate <see cref="ILineParameter"/> whether it qualifies or not.
    /// </summary>
    public interface ILineParameterQualifierEvaluatable : ILineParameterQualifier
    {
        /// <summary>
        /// Test rule with <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">parameter part of a compared key (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        bool QualifyParameter(ILineParameter parameter, int occuranceIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ILineQualifierEnumerable : ILineQualifier, IEnumerable<ILineQualifier>
    {
    }

    /// <summary>
    /// Line qualifier for a <see cref="ILineParameter"/>.
    /// </summary>
    public interface ILineParameterQualifier : ILineQualifier
    {
    }

    /// <summary>
    /// Composition of line qualifiers.
    /// </summary>
    public interface ILineQualifierComposition : ILineQualifier
    {
        /// <summary>
        /// Is collection in read-only state.
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Add rule that is validated against complete <see cref="ILine"/>.
        /// </summary>
        /// <param name="qualifierRule"></param>
        void Add(ILineQualifier qualifierRule);
    }

    /// <summary>
    /// Parameter qualifier that has occurance index constraint.
    /// </summary>
    public interface ILineParameterQualifierOccuranceConstraint : ILineParameterQualifierEvaluatable
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
    /// Parameter qualifier that applies to only one ParameterName.
    /// </summary>
    public interface ILineParameterQualifierNameConstraint : ILineParameterQualifierEvaluatable
    {
        /// <summary>
        /// Parameter name this rule applies to. (null if isn't constrained to one parameter name)
        /// </summary>
        string ParameterName { get; }
    }

    /// <summary></summary>
    public static class ILineQualifierExtensions
    {
        /// <summary>
        /// Qualify <paramref name="key"/> against the qualifier rules.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="key"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        /// <exception cref="InvalidOperationException">If qualifier is not applicable</exception>
        public static bool Qualify(this ILineQualifier qualifier, ILine key)
            => qualifier is ILineQualifierEvaluatable eval ? eval.Qualify(key) : throw new InvalidOperationException($"{qualifier} doesn't implement {nameof(ILineQualifierEvaluatable)}");

        /// <summary>
        /// Qualifies lines against qualifier rules.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        /// <exception cref="InvalidOperationException">If qualifier is not applicable</exception>
        public static IEnumerable<ILine> Qualify(this ILineQualifier qualifier, IEnumerable<ILine> lines)
            => qualifier is ILineQualifierLinesEvaluatable eval ? eval.Qualify(lines) : _qualifier(qualifier, lines);

        static IEnumerable<ILine> _qualifier(ILineQualifier qualifier, IEnumerable<ILine> lines)
        {
            if (qualifier is ILineQualifierEvaluatable eval)
            {
                foreach (var line in lines)
                    if (eval.Qualify(line)) yield return line;
            }
            else
            {
                throw new InvalidOperationException($"{qualifier} doesn't implement {nameof(ILineQualifierEvaluatable)}");
            }
        }

        /// <summary>
        /// Qualify parameter <paramref name="parameter"/>.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="parameter">parameter part of a compared key (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        /// <exception cref="InvalidOperationException">If qualifier is not applicable</exception>
        public static bool QualifyParameter(this ILineQualifier qualifier, ILineParameter parameter, int occuranceIndex)
            => qualifier is ILineParameterQualifierEvaluatable eval ? eval.QualifyParameter(parameter, occuranceIndex) : throw new InvalidOperationException($"{qualifier} doesn't implement {nameof(ILineParameterQualifierEvaluatable)}");

        /// <summary>
        /// Set line qualifier as read-only.
        /// </summary>
        /// <param name="LineQualifier"></param>
        /// <returns></returns>
        public static ILineQualifier SetReadonly(this ILineQualifier LineQualifier)
        {
            if (LineQualifier is ILineQualifierComposition configurable)
            {
                configurable.ReadOnly = true;
                return LineQualifier;
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineQualifierComposition)}");
        }

        /// <summary>
        /// Add qualifier rule that validates parameters.
        /// </summary>
        /// <param name="lineQualifier"></param>
        /// <param name="rule"></param>
        public static ILineQualifier Add(this ILineQualifier lineQualifier, ILineQualifier rule)
        {
            if (lineQualifier is ILineQualifierComposition configurable) configurable.Add(rule);
            else throw new InvalidOperationException($"Is not {nameof(ILineQualifierComposition)}");
            return lineQualifier;
        }

        /// <summary>
        /// Add qualifier rules.
        /// </summary>
        /// <param name="lineQualifier"></param>
        /// <param name="rules"></param>
        public static ILineQualifier AddRange(this ILineQualifier lineQualifier, IEnumerable<ILineQualifier> rules)
        {
            if (lineQualifier is ILineQualifierComposition configurable)
            {
                foreach(var rule in rules)
                    configurable.Add(rule);
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineQualifierComposition)}");
            return lineQualifier;
        }

        /// <summary>
        /// Add qualifier rules.
        /// </summary>
        /// <param name="lineQualifier"></param>
        /// <param name="rules"></param>
        public static ILineQualifier AddRange(this ILineQualifier lineQualifier, params ILineQualifier[] rules)
        {
            if (lineQualifier is ILineQualifierComposition configurable)
            {
                foreach (var rule in rules)
                    configurable.Add(rule);
            }
            else throw new InvalidOperationException($"Is not {nameof(ILineQualifierComposition)}");
            return lineQualifier;
        }


    }

}
