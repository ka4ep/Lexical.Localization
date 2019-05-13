// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Filters lines against filter rules.
    /// </summary>
    public class LineFilter : LineParameterFilterComposition, ILineFilter, ILineFilterComposite, ILineFilterEnumerable, ILineFilterEvaluatable, ILineFilterLinesEvaluatable
    {
        /// <summary>
        /// List of generic filters. Null if none is assigned.
        /// </summary>
        protected List<ILineFilter> filters;

        /// <summary>
        /// Add generic filter rule.
        /// </summary>
        /// <param name="filter"></param>
        public override void Add(ILineFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            if (filter is ILineFilterParameterEvaluatable eval) { base.Add(filter); return; }
            if (filters == null) filters = new List<ILineFilter>();
            filters.Add(filter);
        }

        /// <summary>
        /// Tests if there are any rules in the filter
        /// </summary>
        public bool HasRules => (filters != null && filters.Count > 0) || HasParameterRules;

        /// <summary>
        /// Filter <paramref name="key"/> against the filter rules.
        /// 
        /// The whole <paramref name="key"/> is matched against every <see cref="filters"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// The <paramref name="key"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        public virtual bool Filter(ILine key)
        {
            // Apply generic filters
            if (filters != null)
            {
                foreach (var filter in filters)
                    if (!filter.Filter(key)) return false;
            }

            if (HasParameterRules)
            {
                // Break key into effective parameters with occurance index
                StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
                key.GetParameterPartsWithOccurance(ref list);
                foreach ((ILineParameter parameter, int occuranceIndexx) in list)
                    if (!base.Filter(parameter, occuranceIndexx)) return false;
            }

            // Nothing disqualified.
            return true;
        }

        /// <summary>
        /// Filters lines against filter rules.
        /// 
        /// Each key in <paramref name="keys"/> is matched against every <see cref="filters"/>. 
        /// If every one of them passes the filter then key is yielded.
        /// 
        /// Each key in <paramref name="keys"/> is broken into parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the filter passes for that key part.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual IEnumerable<ILine> Filter(IEnumerable<ILine> keys)
        {
            StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();

            foreach (ILine key in keys)
            {
                // Apply generic filters
                if (filters != null)
                    foreach (var filter in filters)
                        if (!filter.Filter(key)) continue;

                // Apply parameter filters
                if (HasParameterRules)
                {
                    // Break key into effective parameters with occurance index
                    list.Clear();
                    key.GetParameterPartsWithOccurance(ref list);
                    foreach ((ILineParameter parameter, int occuranceIndexx) in list)
                        if (!base.Filter(parameter, occuranceIndexx)) continue;
                }
                yield return key;
            }
        }

        /// <summary>
        /// Print filter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append("(");
            int c = 0;
            if (filters != null)
                foreach (var f in filters)
                {
                    if (c++ > 0) sb.Append(", ");
                    sb.Append(f);
                }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Get component filters
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<ILineFilter> GetEnumerator()
        {
            if (filters != null) foreach (ILineFilter r in filters) yield return r;
            if (parameterRules != null) foreach (ILineFilterParameterEvaluatable r in parameterRules) yield return r;
            if (nameParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)nameParameterRules) yield return r;
            if (nameOccuranceParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)nameOccuranceParameterRules) yield return r;
            if (occuranceParameterRules != null) foreach (ILineFilterParameterEvaluatable r in (IEnumerable<ILineFilterParameterEvaluatable>)occuranceParameterRules) yield return r;
        }
    }
}
