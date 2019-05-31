// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Composition of <see cref="ILineParameterQualifierEvaluatable"/> rules.
    /// </summary>
    public class LineParameterQualifierComposition : ILineParameterQualifierEvaluatable, ILineQualifierComposition, ILineQualifierEnumerable
    {
        /// <summary>
        /// Is in read-only state.
        /// </summary>
        protected bool isReadonly;

        /// <summary>
        /// Read-only state.
        /// </summary>
        public bool ReadOnly
        {
            get => isReadonly;
            set { if (value == isReadonly) return; if (!value) throw new InvalidOperationException("Cannot disable read-only state."); isReadonly = value; }
        }

        /// <summary>
        /// List of generic qualifiers. Null if none are assigned.
        /// </summary>
        protected List<ILineParameterQualifierEvaluatable> parameterRules;

        /// <summary>
        /// List of parameter name and occurance index specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<KeyValuePair<string, int>, ILineParameterQualifierEvaluatable> nameOccuranceParameterRules;

        /// <summary>
        /// List of parameter name specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<string, ILineParameterQualifierNameConstraint> nameParameterRules;

        /// <summary>
        /// List of parameter occurance index specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<int, ILineParameterQualifierOccuranceConstraint> occuranceParameterRules;

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        public LineParameterQualifierComposition()
        {
        }

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        /// <param name="qualifiers"></param>
        public LineParameterQualifierComposition(params ILineParameterQualifierEvaluatable[] qualifiers)
        {
            foreach (var f in qualifiers)
                Add(f);
        }

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        /// <param name="qualifiers"></param>
        public LineParameterQualifierComposition(IEnumerable<ILineParameterQualifierEvaluatable> qualifiers)
        {
            foreach (var f in qualifiers)
                Add(f);
        }

        /// <summary>
        /// Tests if there are any rules in the qualifier
        /// </summary>
        public bool HasParameterRules => parameterRules != null || nameOccuranceParameterRules != null || nameParameterRules != null || occuranceParameterRules != null;

        /// <summary>
        /// Print qualifier
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(String.Join(", ", this));
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Add parameter qualifier
        /// </summary>
        /// <param name="qualifierRule"></param>
        public virtual void Add(ILineQualifier qualifierRule)
        {
            if (qualifierRule == null) throw new ArgumentNullException(nameof(qualifierRule));
            if (isReadonly) throw new InvalidOperationException("read-only");
            if (qualifierRule is ILineParameterQualifierNameConstraint nameConstraint && nameConstraint.ParameterName != null)
            {
                if (qualifierRule is ILineParameterQualifierOccuranceConstraint occuranceConstraint && occuranceConstraint.OccuranceIndex >= 0)
                {
                    if (nameOccuranceParameterRules == null) nameOccuranceParameterRules = new MapList<KeyValuePair<string, int>, ILineParameterQualifierEvaluatable>(KeyValuePairEqualityComparer<string, int>.Default);
                    nameOccuranceParameterRules.Add(new KeyValuePair<string, int>(nameConstraint.ParameterName, occuranceConstraint.OccuranceIndex), occuranceConstraint);
                }
                else
                {
                    if (nameParameterRules == null) nameParameterRules = new MapList<string, ILineParameterQualifierNameConstraint>();
                    nameParameterRules.Add(nameConstraint.ParameterName, nameConstraint);
                }
            }
            else if (qualifierRule is ILineParameterQualifierOccuranceConstraint occuranceConstraint && occuranceConstraint.OccuranceIndex >= 0)
            {
                if (occuranceParameterRules == null) occuranceParameterRules = new MapList<int, ILineParameterQualifierOccuranceConstraint>();
                occuranceParameterRules.Add(occuranceConstraint.OccuranceIndex, occuranceConstraint);
            }
            else if (qualifierRule is ILineParameterQualifierEvaluatable eval)
            {
                parameterRules.Add(eval);
            }
            else throw new InvalidOperationException($"Cannot add {qualifierRule} to {GetType().Name}.");
        }

        /// <summary>
        /// Tests whether <paramref name="parameter"/> is qualified by the component qualifiers.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public virtual bool QualifyParameter(ILineParameter parameter, int occuranceIndex)
        {
            if (parameterRules != null)
            {
                foreach (var r in parameterRules)
                    if (!r.QualifyParameter(parameter, occuranceIndex)) return false;
            }

            if (nameParameterRules != null)
            {
                List<ILineParameterQualifierNameConstraint> list = nameParameterRules.TryGetList(parameter.ParameterName);
                if (list != null)
                    foreach (var r in list)
                        if (!r.QualifyParameter(parameter, occuranceIndex)) return false;
            }

            if (occuranceParameterRules != null)
            {
                List<ILineParameterQualifierOccuranceConstraint> list = occuranceParameterRules.TryGetList(occuranceIndex);
                if (list != null)
                    foreach (var r in list)
                        if (!r.QualifyParameter(parameter, occuranceIndex)) return false;
            }

            if (nameParameterRules != null && occuranceParameterRules != null)
            {
                List<ILineParameterQualifierEvaluatable> list = nameOccuranceParameterRules.TryGetList(new KeyValuePair<string, int>(parameter.ParameterName, occuranceIndex));
                if (list != null)
                    foreach (var r in list)
                        if (!r.QualifyParameter(parameter, occuranceIndex)) return false;
            }

            return true;
        }

        /// <summary>
        /// Get component qualifiers
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<ILineQualifier> GetEnumerator()
        {
            if (parameterRules != null) foreach (ILineParameterQualifierEvaluatable r in parameterRules) yield return r;
            if (nameParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)nameParameterRules) yield return r;
            if (nameOccuranceParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)nameOccuranceParameterRules) yield return r;
            if (occuranceParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)occuranceParameterRules) yield return r;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<ILineQualifier>)this).GetEnumerator();

    }

}
