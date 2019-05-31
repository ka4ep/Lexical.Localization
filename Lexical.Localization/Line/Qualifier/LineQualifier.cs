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
    /// Measures qualification of <see cref="ILine"/>s according to configured rules.
    /// </summary>
    public class LineQualifier : LineParameterQualifierComposition, ILineQualifier, ILineQualifierComposition, ILineQualifierEnumerable, ILineQualifierEvaluatable, ILineQualifierLinesEvaluatable
    {
        /// <summary>
        /// List of generic qualifiers. Null if none is assigned.
        /// </summary>
        protected List<ILineQualifier> qualifiers;

        /// <summary>
        /// Add generic qualifier rule.
        /// </summary>
        /// <param name="qualifier"></param>
        public override void Add(ILineQualifier qualifier)
        {
            if (qualifier == null) throw new ArgumentNullException(nameof(qualifier));
            if (qualifier is ILineParameterQualifier parameterQualifier) { base.Add(parameterQualifier); return; }
            if (qualifiers == null) qualifiers = new List<ILineQualifier>();
            qualifiers.Add(qualifier);
        }

        /// <summary>
        /// Tests if there are any rules in the qualifier
        /// </summary>
        public bool HasRules => (qualifiers != null && qualifiers.Count > 0) || HasParameterRules;

        /// <summary>
        /// Qualifier <paramref name="key"/> against the qualifier rules.
        /// 
        /// The whole <paramref name="key"/> is matched against every <see cref="qualifiers"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// The <paramref name="key"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the qualifier passes for that key part.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public virtual bool Qualify(ILine key)
        {
            // Apply generic qualifiers
            if (qualifiers != null)
            {
                foreach (var qualifier in qualifiers)
                    if (!qualifier.Qualify(key)) return false;
            }

            if (HasParameterRules)
            {
                // Break key into effective parameters with occurance index
                StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
                key.GetParameterPartsWithOccurance(ref list);
                foreach ((ILineParameter parameter, int occuranceIndexx) in list)
                    if (!base.QualifyParameter(parameter, occuranceIndexx)) return false;
            }

            // Everything qualified
            return true;
        }

        /// <summary>
        /// Qualifiers lines against qualifier rules.
        /// 
        /// Each key in <paramref name="keys"/> is matched against every <see cref="qualifiers"/>. 
        /// If every one of them passes the qualifier then key is yielded.
        /// 
        /// Each key in <paramref name="keys"/> is broken into parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the qualifier passes for that key part.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual IEnumerable<ILine> Qualify(IEnumerable<ILine> keys)
        {
            StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();

            foreach (ILine key in keys)
            {
                // Apply generic qualifiers
                if (qualifiers != null)
                    foreach (var qualifier in qualifiers)
                        if (!qualifier.Qualify(key)) continue;

                // Apply parameter qualifiers
                if (HasParameterRules)
                {
                    // Break key into effective parameters with occurance index
                    list.Clear();
                    key.GetParameterPartsWithOccurance(ref list);
                    foreach ((ILineParameter parameter, int occuranceIndexx) in list)
                        if (!base.QualifyParameter(parameter, occuranceIndexx)) continue;

                }
                yield return key;
            }
        }

        /// <summary>
        /// Print qualifier
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append("(");
            int c = 0;
            if (qualifiers != null)
                foreach (var f in qualifiers)
                {
                    if (c++ > 0) sb.Append(", ");
                    sb.Append(f);
                }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Get component qualifiers
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<ILineQualifier> GetEnumerator()
        {
            if (qualifiers != null) foreach (ILineQualifier r in qualifiers) yield return r;
            if (parameterRules != null) foreach (ILineParameterQualifierEvaluatable r in parameterRules) yield return r;
            if (nameParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)nameParameterRules) yield return r;
            if (nameOccuranceParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)nameOccuranceParameterRules) yield return r;
            if (occuranceParameterRules != null) foreach (ILineParameterQualifierEvaluatable r in (IEnumerable<ILineParameterQualifierEvaluatable>)occuranceParameterRules) yield return r;
        }
    }
}
