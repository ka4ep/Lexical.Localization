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
    /// Measures qualification of <see cref="ILine"/>s according to configured rules.
    /// </summary>
    public class LineQualifier : ILineQualifier, ILineArgumentQualifier, ILineQualifierComposition, ILineQualifierEnumerable, ILineQualifierEvaluatable, ILineQualifierLinesEvaluatable
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
        /// List of all qualifier instances. 
        /// </summary>
        protected List<ILineQualifier> qualifiers = new List<ILineQualifier>();

        /// <summary>
        /// List of whole-line-qualifiers. Null if none are assigned.
        /// </summary>
        protected List<ILineQualifierEvaluatable> lineQualifiers; 


        /// <summary>
        /// List of argument qualifiers. Null if none are assigned.
        /// </summary>
        protected List<ILineArgumentQualifier> argumentQualifiers;

        /// <summary>
        /// If true, <see cref="QualifyArgument(ILineArgument, int)"/> caller must have occurance index.
        /// If false, caller can use -1 for unknown.
        /// </summary>
        public bool NeedsOccuranceIndex { get; protected set; }

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        public LineQualifier()
        {
        }

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        /// <param name="qualifiers"></param>
        public LineQualifier(params ILineQualifier[] qualifiers)
        {
            foreach (var f in qualifiers)
                Add(f);
        }

        /// <summary>
        /// Create line qualifier. 
        /// </summary>
        /// <param name="qualifiers"></param>
        public LineQualifier(IEnumerable<ILineQualifier> qualifiers)
        {
            foreach (var f in qualifiers)
                Add(f);
        }

        /// <summary>
        /// Add generic qualifier rule.
        /// </summary>
        /// <param name="qualifier"></param>
        public void Add(ILineQualifier qualifier)
        {
            if (qualifier == null) throw new ArgumentNullException(nameof(qualifier));
            if (qualifier is ILineArgumentQualifier argumentQualifier && qualifier is ILineQualifierEvaluatable == false /*Don't run twice*/)
            {
                if (argumentQualifiers == null) argumentQualifiers = new List<ILineArgumentQualifier>();
                argumentQualifiers.Add(argumentQualifier);
                NeedsOccuranceIndex |= argumentQualifier.NeedsOccuranceIndex;
            }
            if (qualifier is ILineQualifierEvaluatable lineQualifier)
            {
                if (lineQualifiers == null) lineQualifiers = new List<ILineQualifierEvaluatable>();
                lineQualifiers.Add(lineQualifier);
            }
            qualifiers.Add(qualifier);
        }

        /// <summary>
        /// Tests if there are any rules in the qualifier
        /// </summary>
        public bool HasRules => (qualifiers != null && qualifiers.Count > 0);

        /// <summary>
        /// Qualifier <paramref name="line"/> against the qualifier rules.
        /// 
        /// The whole <paramref name="line"/> is matched against every <see cref="qualifiers"/>. 
        /// If one of the mismatches then returns false.
        /// 
        /// The <paramref name="line"/> is broken into key parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the qualifier passes for that key part.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public virtual bool Qualify(ILine line)
        {
            // Apply line qualifiers
            if (lineQualifiers != null)
            {
                foreach (var qualifier in lineQualifiers)
                    if (!qualifier.Qualify(line)) return false;
            }

            // Apply argument qualifieres
            if (argumentQualifiers != null)
            {
                if (NeedsOccuranceIndex)
                {
                    // Break key into effective parameters with occurance index
                    StructList12<(ILineArgument, int)> list1 = new StructList12<(ILineArgument, int)>();
                    line.GetArgumentPartsWithOccurance(ref list1);
                    foreach (ILineArgumentQualifier argumentQualifier in argumentQualifiers)
                        for (int i = 0; i < list1.Count; i++)
                            if (!argumentQualifier.QualifyArgument(list1[i].Item1, list1[i].Item2)) return false;                    
                }
                else
                {
                    // Break key into parameters
                    StructList12<ILineArgument> list2 = new StructList12<ILineArgument>();
                    line.GetArgumentParts(ref list2, null, false, null);
                    foreach (ILineArgumentQualifier argumentQualifier in argumentQualifiers)
                        for (int i = 0; i < list2.Count; i++)
                            if (!argumentQualifier.QualifyArgument(list2[i], -1)) return false;
                }
            }

            // Everything qualified
            return true;
        }

        /// <summary>
        /// Qualifiers lines against qualifier rules.
        /// 
        /// Each key in <paramref name="lines"/> is matched against every <see cref="qualifiers"/>. 
        /// If every one of them passes the qualifier then key is yielded.
        /// 
        /// Each key in <paramref name="lines"/> is broken into parts.
        /// If any rule for (parameterName, occuranceIndex) passes, the qualifier passes for that key part.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public virtual IEnumerable<ILine> Qualify(IEnumerable<ILine> lines)
        {
            StructList12<(ILineArgument, int)> list1 = new StructList12<(ILineArgument, int)>();
            StructList12<ILineArgument> list2 = new StructList12<ILineArgument>();

            foreach (ILine line in lines)
            {
                bool ok = true;

                // Apply line qualifiers
                if (lineQualifiers != null)
                {
                    foreach (var qualifier in lineQualifiers)
                    {
                        ok &= qualifier.Qualify(line);
                        if (!ok) break;
                    }
                }

                // Apply parameter qualifieres
                if (argumentQualifiers != null)
                {
                    if (NeedsOccuranceIndex)
                    {
                        list1.Clear();
                        // Break key into effective parameters with occurance index
                        line.GetArgumentPartsWithOccurance(ref list1);
                        foreach (ILineArgumentQualifier argumentQualifier in argumentQualifiers)
                        {
                            for (int i = 0; i < list1.Count; i++)
                            {
                                ok &= argumentQualifier.QualifyArgument(list1[i].Item1, list1[i].Item2);
                                if (!ok) break;
                            }
                            if (!ok) break;
                        }
                    }
                    else
                    {
                        list2.Clear();
                        // Break key into parameters
                        line.GetArgumentParts(ref list2);
                        foreach (ILineArgumentQualifier argumentQualifier in argumentQualifiers)
                        {
                            for (int i = 0; i < list2.Count; i++)
                            {
                                ok &= argumentQualifier.QualifyArgument(list2[i], -1);
                                if (!ok) break;
                            }
                            if (!ok) break;
                        }
                    }
                }

                // Everything qualified
                if (ok) yield return line;
            }
        }

        /// <summary>
        /// Test rule with <paramref name="argument"/>.
        /// </summary>
        /// <param name="argument">parameter part of a compared key (note ParameterName="" for empty), or null if value did not occur</param>
        /// <param name="occuranceIndex">Occurance index of the parameterName. 0-first, 1-second, etc. Use -1 if occurance is unknown</param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public bool QualifyArgument(ILineArgument argument, int occuranceIndex)
        {
            if (argumentQualifiers != null)
            {
                foreach(ILineArgumentQualifier argumentQualifier in argumentQualifiers)
                {
                    if (!argumentQualifier.QualifyArgument(argument, occuranceIndex)) return false;
                }
            }
            return true;
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
        public IEnumerator<ILineQualifier> GetEnumerator()
            => qualifiers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => qualifiers.GetEnumerator();
    }
}
