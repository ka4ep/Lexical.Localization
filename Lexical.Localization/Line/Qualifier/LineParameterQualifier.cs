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
    /// <summary>
    /// Filter rules.
    /// </summary>
    public abstract class LineParameterQualifier : ILineParameterQualifier
    {
        /// <summary>
        /// Parameter name to apply rule to.
        /// </summary>
        public string ParameterName { get; protected set; }

        /// <summary>
        /// The occurance of this parameter this rule applies to. 
        /// 
        /// Use -1 to have rule be applied against any occurance.
        /// 
        /// Occurance counter starts from the root at 0 and increments for every occurance of the <see cref="ParameterName"/>.
        /// For the effective non-canonical parameter, the index is always 0.
        /// </summary>
        public int OccuranceIndex { get; protected set; }

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
        public virtual bool NeedsOccuranceIndex => OccuranceIndex >= 0;

        /// <summary>
        /// Create rule for a specific <paramref name="parameterName"/> and <paramref name="occuranceIndex"/>.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="occuranceIndex">occurance of parameter name this rule applies to. Use 0 for non-canonical parameters.</param>
        public LineParameterQualifier(string parameterName, int occuranceIndex)
        {
            this.ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            this.OccuranceIndex = occuranceIndex;
        }

        /// <summary>
        /// Apply rule to <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">value that occured in the compared key (note "" for empty)</param>
        /// <param name="occuranceIndex"></param>
        /// <returns></returns>
        public abstract bool QualifyParameter(ILineParameter parameter, int occuranceIndex);

        /// <summary>
        /// Print rule
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";

        /// <summary>
        /// Filter rule that matches paramter value against <see cref="Pattern"/>.
        /// </summary>
        public class Regex : LineParameterQualifier
        {
            /// <summary>
            /// Regex pattern
            /// </summary>
            public readonly System.Text.RegularExpressions.Regex Pattern;

            /// <summary>
            /// Create rule that validates parameter value against <see cref="System.Text.RegularExpressions.Regex"/>.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="pattern"></param>
            public Regex(string parameterName, int occuranceIndex, System.Text.RegularExpressions.Regex pattern) : base(parameterName, occuranceIndex)
            {
                Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns></returns>
            public override bool QualifyParameter(ILineParameter parameter, int occuranceIndex)
            {
                // This filter doesn't apply
                if (OccuranceIndex >= 0 && OccuranceIndex != occuranceIndex) return true;
                // This filter doesn't apply
                if (parameter.ParameterName != ParameterName) return true;
                // This parameter doesn't apply
                if (parameter.ParameterValue == null) return true;
                // This filter applies
                return QualifyValue(parameter.ParameterValue);
            }

            /// <summary>
            /// Qualify value when other filter otherwise applies.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public bool QualifyValue(string parameterValue)
                => Pattern.Match(parameterValue).Success;

            /// <summary>
            /// Ensure that parameter does not exist (effectively) in the whole line.
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>
            public bool Qualify(ILine line)
            {
                if (OccuranceIndex < 0)
                {
                    bool matched = true;
                    int count = 0;
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(parameter.ParameterValue); }
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(lineParameter.ParameterValue); }
                        if (!matched) break;
                    }
                    return matched && count > 0;
                }
                else
                {
                    StructList8<string> list = new StructList8<string>();
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName && (parameter.ParameterValue != null && parameter.ParameterValue != "")) list.Add(parameter.ParameterValue);
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName && (lineParameter.ParameterValue != null && lineParameter.ParameterValue != "")) list.Add(lineParameter.ParameterValue);
                    }
                    // Did not exist
                    if (OccuranceIndex >= list.Count) return false;
                    return QualifyValue(list[list.Count - 1 - OccuranceIndex]);
                }
            }

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(Pattern)}={Pattern})";
        }

        /// <summary>
        /// Filter rule that matches paramter value against one value.
        /// </summary>
        public class IsEqualTo : LineParameterQualifier, ILineQualifierEvaluatable
        {
            /// <summary>
            /// Accepted value
            /// </summary>
            public readonly string Value;

            /// <summary>
            /// Create rule that validates parameter value against a string.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="parameterValue">expected value</param>
            public IsEqualTo(string parameterName, int occuranceIndex, string parameterValue) : base(parameterName, occuranceIndex)
            {
                Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue), $"Use {nameof(IsEmpty)} class instead");
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns>true if line is passed by the filter, false if rejected</returns>
            public override bool QualifyParameter(ILineParameter parameter, int occuranceIndex)
            {
                // This filter doesn't apply
                if (OccuranceIndex >= 0 && OccuranceIndex != occuranceIndex) return true;
                // This filter doesn't apply
                if (parameter.ParameterName != ParameterName) return true;
                // This parameter doesn't apply
                if (parameter.ParameterValue == null) return true;
                // This filter applies
                return QualifyValue(parameter.ParameterValue);
            }

            /// <summary>
            /// Qualify value when other filter otherwise applies.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public bool QualifyValue(string parameterValue)
                => parameterValue == Value;

            /// <summary>
            /// Ensure that parameter does not exist (effectively) in the whole line.
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>
            public bool Qualify(ILine line)
            {
                if (OccuranceIndex < 0)
                {
                    bool matched = true;
                    int count = 0;
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(parameter.ParameterValue); }
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(lineParameter.ParameterValue); }
                        if (!matched) break;
                    }
                    return matched && count > 0;
                }
                else
                {
                    StructList8<string> list = new StructList8<string>();
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName && (parameter.ParameterValue != null && parameter.ParameterValue != "")) list.Add(parameter.ParameterValue);
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName && (lineParameter.ParameterValue != null && lineParameter.ParameterValue != "")) list.Add(lineParameter.ParameterValue);
                    }
                    // Did not exist
                    if (OccuranceIndex >= list.Count) return false;
                    return QualifyValue(list[list.Count - 1 - OccuranceIndex]);
                }
            }

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(Value)}={Value})";
        }

        /// <summary>
        /// Filter rule that matches parameter value against a set of valid values.
        /// </summary>
        public class IsInGroup : LineParameterQualifier, ILineQualifier
        {
            /// <summary>
            /// List of accepted values.
            /// </summary>
            public IEnumerable<string> AcceptedValues => values;

            HashSet<string> values;

            bool acceptsEmpty;

            /// <summary>
            /// Creates a rule that validates parameter value against a set of values.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="acceptedParameterValues">expected values, one of these must be in the key</param>
            public IsInGroup(string parameterName, int occuranceIndex, IEnumerable<string> acceptedParameterValues) : base(parameterName, occuranceIndex)
            {
                if (acceptedParameterValues == null) throw new ArgumentNullException(nameof(acceptedParameterValues));
                values = new HashSet<string>();
                foreach (var acceptedValue in acceptedParameterValues)
                {
                    if (acceptedValue == null) throw new ArgumentNullException(nameof(acceptedParameterValues));
                    if (acceptedValue == "") acceptsEmpty = true;
                    else values.Add(acceptedValue);
                }
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns></returns>
            public override bool QualifyParameter(ILineParameter parameter, int occuranceIndex)
            {
                // This filter doesn't apply
                if (OccuranceIndex >= 0 && OccuranceIndex != occuranceIndex) return true;
                // This filter doesn't apply
                if (parameter.ParameterName != ParameterName) return true;
                // This filter applies
                return QualifyValue(parameter.ParameterValue);
            }

            /// <summary>
            /// Qualify value when other filter otherwise applies.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public bool QualifyValue(string parameterValue)
                => (acceptsEmpty && String.IsNullOrEmpty(parameterValue)) || (parameterValue != null && values.Contains(parameterValue));

            /// <summary>
            /// Ensure that parameter does not exist (effectively) in the whole line.
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>
            public bool Qualify(ILine line)
            {
                if (OccuranceIndex < 0)
                {
                    bool matched = true;
                    int count = 0;
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(parameter.ParameterValue); }
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName) { count++; matched &= QualifyValue(lineParameter.ParameterValue); }
                        if (!matched) break;
                    }
                    return matched && count > 0;
                }
                else
                {
                    StructList8<string> list = new StructList8<string>();
                    for (ILine l = line; l != null; l = l.GetPreviousPart())
                    {
                        if (l is ILineParameterEnumerable lineParameters)
                            foreach (ILineParameter parameter in lineParameters)
                                if (parameter.ParameterName == ParameterName && (parameter.ParameterValue != null && parameter.ParameterValue != "")) list.Add(parameter.ParameterValue);
                        if (l is ILineParameter lineParameter)
                            if (lineParameter.ParameterName == ParameterName && (lineParameter.ParameterValue != null && lineParameter.ParameterValue != "")) list.Add(lineParameter.ParameterValue);
                    }
                    // Did not exist
                    if (OccuranceIndex >= list.Count) return false;
                    return QualifyValue(list[list.Count - 1 - OccuranceIndex]);
                }
            }


            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex}, {nameof(AcceptedValues)}={String.Join(", ", AcceptedValues)})";
        }

        /// <summary>
        /// Filter rule that asserts that the parameter is null or "".
        /// </summary>
        public class IsEmpty : LineParameterQualifier
        {
            /// <summary>
            /// Create rule that approves emptry (null or "") parameter value.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">Occurance index to which this rule applies. If -1, then applies to every occurance</param>
            public IsEmpty(string parameterName, int occuranceIndex) : base(parameterName, occuranceIndex)
            {
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameter"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns></returns>
            public override bool QualifyParameter(ILineParameter parameter, int occuranceIndex)
            {
                // This filter doesn't apply
                if (OccuranceIndex >= 0 && OccuranceIndex != occuranceIndex) return true;
                // This filter doesn't apply
                if (parameter.ParameterName != ParameterName) return true;
                // This filter applies
                return parameter.ParameterValue != null && parameter.ParameterValue != "";
            }

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";
        }

    }

}
