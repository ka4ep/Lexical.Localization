// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    #region Interface
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
        /// Qualify <paramref name="line"/> against the criteria.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        bool Qualify(ILine line);
    }

    /// <summary>
    /// Tests whether a <see cref="ILine"/> matches a criteria.
    /// </summary>
    public interface ILineQualifierLinesEvaluatable : ILineQualifier
    {
        /// <summary>
        /// Qualifies lines. 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        IEnumerable<ILine> Qualify(IEnumerable<ILine> lines);
    }

    /// <summary>
    /// Measures qualifications of <see cref="ILineParameter"/>s.
    /// </summary>
    public interface ILineParameterQualifier : ILineQualifier
    {
        /// <summary>
        /// Policy whether occuranceIndex is needed for qualifying parameter.
        /// 
        /// If true, <see cref="QualifyParameter(ILineParameter, int)"/> caller must have occurance index.
        /// If false, caller can use -1 for unknown.
        /// 
        /// Occurance describes the position of parameter of same parameter name.
        /// For example, "Section:A:Section:B:Section:C" has parameter "Section" with three 
        /// occurance indices: 0, 1, 2.
        /// </summary>
        bool NeedsOccuranceIndex { get; }

        /// <summary>
        /// Qualify <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">parameter part of a compared line (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc. Use -1 if occurance is unknown</param>
        /// <returns>true if parameter is qualified, false if disqualified</returns>
        bool QualifyParameter(ILineParameter parameter, int occuranceIndex);
    }

    /// <summary>
    /// Measures qualifications of <see cref="ILineArguments"/>s.
    /// </summary>
    public interface ILineArgumentsQualifier : ILineQualifier
    {
        /// <summary>
        /// Qualify <paramref name="arguments"/>.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>true if argument is qualified, false if disqualified</returns>
        bool QualifyArguments(ILineArguments arguments);
    }

    /// <summary>
    /// Qualifier that can enumerate component qualifiers.
    /// </summary>
    public interface ILineQualifierEnumerable : ILineQualifier, IEnumerable<ILineQualifier>
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
    #endregion Interface

    /// <summary></summary>
    public static class ILineQualifierExtensions
    {
        /// <summary>
        /// Qualify <paramref name="line"/> against the qualifier rules.
        /// </summary>
        /// <param name="qualifier">(optional) qualifier</param>
        /// <param name="line"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public static bool Qualify(this ILineQualifier qualifier, ILine line)
        {
            // Evaluate whole line
            if (qualifier is ILineQualifierEvaluatable eval) return eval.Qualify(line);

            // Evaluate parameter
            if (qualifier is ILineParameterQualifier parameterQualifier)
            {
                if (parameterQualifier.NeedsOccuranceIndex)
                {
                    // Break key into effective parameters with occurance index
                    StructList12<(ILineParameter, int)> list1 = new StructList12<(ILineParameter, int)>();
                    line.GetParameterPartsWithOccurance(ref list1);
                    for (int i = 0; i < list1.Count; i++)
                        if (!parameterQualifier.QualifyParameter(list1[i].Item1, list1[i].Item2)) return false;
                }
                else
                {
                    // Break key into parameters
                    StructList12<ILineParameter> list2 = new StructList12<ILineParameter>();
                    line.GetParameterParts(ref list2);
                    for (int i = 0; i < list2.Count; i++)
                        if (!parameterQualifier.QualifyParameter(list2[i], -1)) return false;
                }
            }

            // no criteria, accept all
            return true;
        }

        /// <summary>
        /// Qualifies lines against qualifier rules.
        /// </summary>
        /// <param name="qualifier">(optional) qualifier</param>
        /// <param name="lines"></param>
        /// <returns>all lines that were qualified</returns>
        public static IEnumerable<ILine> Qualify(this ILineQualifier qualifier, IEnumerable<ILine> lines)
            => qualifier is ILineQualifierLinesEvaluatable linesEval ? linesEval.Qualify(lines) :
               qualifier is ILineQualifierEvaluatable eval ? _qualifier(eval, lines) :
               lines /*no criteria, accept all*/;

        static IEnumerable<ILine> _qualifier(ILineQualifierEvaluatable qualifier, IEnumerable<ILine> lines)
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
        /// <param name="qualifier">(optional) qualifier</param>
        /// <param name="parameter">parameter part of a compared line (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public static bool QualifyParameter(this ILineQualifier qualifier, ILineParameter parameter, int occuranceIndex)
            => qualifier is ILineParameterQualifier parameterQualifier ? parameterQualifier.QualifyParameter(parameter, occuranceIndex) : /*no parameter criteria, accept all*/ true;

        /// <summary>
        /// Does evaluation need parameter occurances.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <returns>true if needs parameter occurance, if false can use -1</returns>
        public static bool NeedsOccuranceIndex(this ILineQualifier qualifier)
            => qualifier is ILineParameterQualifier parameterQualifier ? parameterQualifier.NeedsOccuranceIndex : false;

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
