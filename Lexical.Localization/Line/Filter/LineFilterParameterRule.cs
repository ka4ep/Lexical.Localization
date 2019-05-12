// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Filter rule for a specific <see cref="ParameterName"/>
    /// </summary>
    public abstract class LineFilterParameterRule
    {
        /// <summary>
        /// Parameter name to apply rule to.
        /// </summary>
        public readonly string ParameterName;

        /// <summary>
        /// The occurance of this parameter this rule applies to. 
        /// 
        /// Use -1 to have rule be applied against any occurance.
        /// 
        /// Occurance counter starts from the root at 0 and increments for every occurance of the <see cref="ParameterName"/>.
        /// For the effective non-canonical parameter, the index is always 0.
        /// </summary>
        public readonly int OccuranceIndex;

        /// <summary>
        /// Create rule for a specific <paramref name="parameterName"/> and <paramref name="occuranceIndex"/>.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="occuranceIndex">occurance of parameter name this rule applies to. Use 0 for non-canonical parameters.</param>
        public LineFilterParameterRule(string parameterName, int occuranceIndex)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        /// <summary>
        /// Apply rule to <paramref name="parameterValue"/>.
        /// </summary>
        /// <param name="parameterValue">value that occured in the compared key (note "" for empty), or null if value did not occur</param>
        /// <returns></returns>
        public abstract bool Filter(string parameterValue);

        /// <summary>
        /// Print rule
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";

        /// <summary>
        /// Filter rule that matches paramter value against <see cref="Pattern"/>.
        /// </summary>
        public class Regex : LineFilterParameterRule
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
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => parameterValue == null ? false : Pattern.Match(parameterValue).Success;

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
        public class IsEqualTo : LineFilterParameterRule
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
                Value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => Value == parameterValue;

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
        public class IsIn : LineFilterParameterRule
        {
            /// <summary>
            /// List of accepted values.
            /// </summary>
            public IEnumerable<string> AcceptedValues => values;

            HashSet<string> values;

            /// <summary>
            /// Creates a rule that validates parameter value against a set of values.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex">occurance of parameter this rule applies to. Use 0 for non-canonical parameters.</param>
            /// <param name="parameterValues">expected values, one of these must be in the key</param>
            public IsIn(string parameterName, int occuranceIndex, IEnumerable<string> parameterValues) : base(parameterName, occuranceIndex)
            {
                if (parameterValues == null) throw new ArgumentNullException(nameof(parameterValues));
                values = new HashSet<string>(parameterValues);
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => parameterValue == null ? false : values.Contains(parameterValue);

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
        public class IsEmpty : LineFilterParameterRule
        {
            /// <summary>
            /// Create rule that approves emptry (null or "") parameter value.
            /// </summary>
            /// <param name="parameterName"></param>
            /// <param name="occuranceIndex"></param>
            public IsEmpty(string parameterName, int occuranceIndex) : base(parameterName, occuranceIndex)
            {
            }

            /// <summary>
            /// Filter parameter from compared key.
            /// </summary>
            /// <param name="parameterValue"></param>
            /// <returns></returns>
            public override bool Filter(string parameterValue)
                => String.IsNullOrEmpty(parameterValue);

            /// <summary>
            /// Print rule
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => $"{GetType().Name}({nameof(ParameterName)}={ParameterName}, {nameof(OccuranceIndex)}={OccuranceIndex})";
        }

    }

    namespace Internal
    {
        /// <summary>
        /// <see cref="ILine"/> extension methods with internal class dependencies.
        /// </summary>
        public static partial class ILineParameterExtensions
        {
            /// <summary>
            /// Break <paramref name="line"/> into effective parameters and write to <paramref name="list"/>.
            /// The <paramref name="list"/> is allocated from stack by caller.
            /// 
            /// For non-canonical parameters, only the left-most is added, with occurance index 0.
            /// For canonical parameters, the left most occurance starts with index 0, and increments for every new occurance.
            /// </summary>
            /// <param name="line"></param>
            /// <param name="list">(ParameterName, occuranceIndex, ParameterValue)</param>
            /// <param name="parameterInfos">(optional) Parameter infos for determining if parameter is key</param>
            public static void GetEffectiveParameters(this ILine line, ref StructList12<(string, int, string)> list, IReadOnlyDictionary<string, IParameterInfo> parameterInfos)
            {
                for (ILine part = line; part != null; part = part.GetPreviousPart())
                {
                    if (part is ILineParameterEnumerable lineParameters)
                    {
                        StructList4<(string, int, string)> tmp = new StructList4<(string, int, string)>();
                        foreach (ILineParameter parameter in lineParameters)
                        {
                            if (parameter.IsNonCanonicalKey(parameterInfos))
                            {
                                // Test if parameter is already in list
                                int ix = -1;
                                for (int i = 0; i < list.Count; i++) if (list[i].Item1 == parameter.ParameterName) { ix = i; break; }
                                // Overwrite
                                if (ix >= 0) list[ix] = (parameter.ParameterName, 0, parameter.ParameterValue);
                                // Add new
                                else list.Add((parameter.ParameterName, 0, parameter.ParameterValue));
                            }

                            // Add to list, fix occurance index later
                            else if (parameter.IsCanonicalKey(parameterInfos)) tmp.Add((parameter.ParameterName, -1, parameter.ParameterValue));
                        }
                        for (int i = tmp.Count - 1; i >= 0; i--) list.Add(tmp[i]);
                    }
                    {
                        if (part is ILineParameter parameter && parameter.ParameterName != null)
                        {
                            if (parameter.IsNonCanonicalKey(parameterInfos))
                            {
                                // Test if parameter is already in list
                                int ix = -1;
                                for (int i = 0; i < list.Count; i++) if (list[i].Item1 == parameter.ParameterName) { ix = i; break; }
                                // Overwrite
                                if (ix >= 0) list[ix] = (parameter.ParameterName, 0, parameter.ParameterValue);
                                // Add new
                                else list.Add((parameter.ParameterName, 0, parameter.ParameterValue));
                            }

                            // Add to list, fix occurance index later
                            else if (parameter.IsCanonicalKey(parameterInfos)) list.Add((parameter.ParameterName, -1, parameter.ParameterValue));
                        }
                    }
                }

                // Fix occurance indices
                for (int i = 0; i < list.Count; i++)
                {
                    (string parameterName, int occurance, string parameterValue) = list[i];
                    if (occurance >= 0) continue;
                    int oix = 0;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        (string parameterName_, int occurance_, string _) = list[j];
                        if (parameterName_ == parameterName) { oix = occurance_ + 1; break; }
                    }
                    list[i] = (parameterName, oix, parameterValue);
                }

            }

        }
    }
}
}
