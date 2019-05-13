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
    /// Composition of <see cref="ILineFilterParameterEvaluatable"/> rules.
    /// </summary>
    public class LineParameterFilterComposition : ILineFilterParameterEvaluatable, ILineFilterComposite, ILineFilterEnumerable
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
        /// List of generic filters. Null if none are assigned.
        /// </summary>
        protected List<ILineFilterParameterEvaluatable> parameterRules;

        /// <summary>
        /// List of parameter name and occurance index specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<KeyValuePair<string, int>, ILineFilterParameterEvaluatable> nameOccuranceParameterRules;

        /// <summary>
        /// List of parameter name specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<string, ILineFilterParameterNameConstraint> nameParameterRules;

        /// <summary>
        /// List of parameter occurance index specific rules. Null if none are assigned.
        /// </summary>
        protected MapList<int, ILineFilterParameterOccuranceConstraint> occuranceParameterRules;

        /// <summary>
        /// Create line filter. 
        /// </summary>
        public LineParameterFilterComposition()
        {
        }

        /// <summary>
        /// Create line filter. 
        /// </summary>
        /// <param name="filters"></param>
        public LineParameterFilterComposition(params ILineFilterParameterEvaluatable[] filters)
        {
            foreach (var f in filters)
                Add(f);
        }

        /// <summary>
        /// Create line filter. 
        /// </summary>
        /// <param name="filters"></param>
        public LineParameterFilterComposition(IEnumerable<ILineFilterParameterEvaluatable> filters)
        {
            foreach (var f in filters)
                Add(f);
        }

        /// <summary>
        /// Tests if there are any rules in the filter
        /// </summary>
        public bool HasParameterRules => parameterRules != null || nameOccuranceParameterRules != null || nameParameterRules != null || occuranceParameterRules != null;

        /// <summary>
        /// Print filter
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
        /// Add parameter filter
        /// </summary>
        /// <param name="filterRule"></param>
        public virtual void Add(ILineFilter filterRule)
        {
            if (filterRule == null) throw new ArgumentNullException(nameof(filterRule));
            if (isReadonly) throw new InvalidOperationException("read-only");
            if (filterRule is ILineFilterParameterNameConstraint nameConstraint && nameConstraint.ParameterName != null)
            {
                if (filterRule is ILineFilterParameterOccuranceConstraint occuranceConstraint && occuranceConstraint.OccuranceIndex >= 0)
                {
                    if (nameOccuranceParameterRules == null) nameOccuranceParameterRules = new MapList<KeyValuePair<string, int>, ILineFilterParameterEvaluatable>(KeyValuePairEqualityComparer<string, int>.Default);
                    nameOccuranceParameterRules.Add(new KeyValuePair<string, int>(nameConstraint.ParameterName, occuranceConstraint.OccuranceIndex), occuranceConstraint);
                }
                else
                {
                    if (nameParameterRules == null) nameParameterRules = new MapList<string, ILineFilterParameterNameConstraint>();
                    nameParameterRules.Add(nameConstraint.ParameterName, nameConstraint);
                }
            }
            else if (filterRule is ILineFilterParameterOccuranceConstraint occuranceConstraint && occuranceConstraint.OccuranceIndex >= 0)
            {
                if (occuranceParameterRules == null) occuranceParameterRules = new MapList<int, ILineFilterParameterOccuranceConstraint>();
                occuranceParameterRules.Add(occuranceConstraint.OccuranceIndex, occuranceConstraint);
            }
            else if (filterRule is ILineFilterParameterEvaluatable eval)
            {
                parameterRules.Add(eval);
            }
            else throw new InvalidOperationException($"Cannot add {filterRule} to {GetType().Name}.");
        }

        /// <summary>
        /// Tests whether <paramref name="parameter"/> is 'approved' by the component filters.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc</param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        public virtual bool Filter(ILineParameter parameter, int occuranceIndex)
        {
            if (parameterRules != null)
            {
                foreach (var r in parameterRules)
                    if (!r.Filter(parameter, occuranceIndex)) return false;
            }

            if (nameParameterRules != null)
            {
                List<ILineFilterParameterNameConstraint> list = nameParameterRules.TryGetList(parameter.ParameterName);
                if (list != null)
                    foreach (var r in list)
                        if (!r.Filter(parameter, occuranceIndex)) return false;
            }

            if (occuranceParameterRules != null)
            {
                List<ILineFilterParameterOccuranceConstraint> list = occuranceParameterRules.TryGetList(occuranceIndex);
                if (list != null)
                    foreach (var r in list)
                        if (!r.Filter(parameter, occuranceIndex)) return false;
            }

            if (nameParameterRules != null && occuranceParameterRules != null)
            {
                List<ILineFilterParameterEvaluatable> list = nameOccuranceParameterRules.TryGetList(new KeyValuePair<string, int>(parameter.ParameterName, occuranceIndex));
                if (list != null)
                    foreach (var r in list)
                        if (!r.Filter(parameter, occuranceIndex)) return false;
            }

            return true;
        }

        /// <summary>
        /// Get component filters
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<ILineFilter> GetEnumerator()
        {
            if (parameterRules != null) foreach (ILineFilterParameterEvaluatable r in parameterRules) yield return r;
            if (nameParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)nameParameterRules) yield return r;
            if (nameOccuranceParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)nameOccuranceParameterRules) yield return r;
            if (occuranceParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)occuranceParameterRules) yield return r;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<ILineFilter>)this).GetEnumerator();

    }

}
