// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumEvaluation
    {
        /// <summary>
        /// Evaluate enumeration value.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="enum"></param>
        /// <param name="separator">(optional) separator to use between strings</param>
        /// <returns>
        /// Result codes:
        /// <list type="bullet">
        /// <item><see cref="LineStatus.PlaceholderOkEnum">Ok</see></item>
        /// <item><see cref="LineStatus.PlaceholderWarningEnum">Got warning while evaluating enum</see></item>
        /// <item><see cref="LineStatus.PlaceholderWarningEnumCaseNotMatched">A case was not matched, returned a number instead</see></item>
        /// <item><see cref="LineStatus.PlaceholderErrorEnum">Got unspecified error</see></item>
        /// <item><see cref="LineStatus.PlaceholderFailedEnum">Failed</see></item>
        /// </list>
        /// </returns>
        public static LineString EvaluateEnum(this ref FunctionEvaluationContext ctx, Enum @enum, string separator = ", ")
        {
            // Status
            LineStatus status = 0UL;
            try
            {
                // Get enum info
                IEnumInfo enumInfo = ctx.EnumResolver != null ? ctx.EnumResolver.GetEnumInfo(@enum.GetType()) : new EnumInfo(@enum.GetType());
                // Get value
                ulong value = EnumCase.ToUInt64(@enum);
                // Remove hash-equals comparable key parts.
                ILine keyBase = ctx.Line.Prune(LineEnumBaseKeyPruner.Default, LineAppender.NonResolving);
                // Result cases
                StructList8<(IEnumCase, string)> resultCases = new StructList8<(IEnumCase, string)>();
                // Split into cases
                while (value != 0UL)
                {
                    string caseStr = null;

                    // First run, find matching case
                    foreach (IEnumCase @case in enumInfo.EvalCases)
                    {
                        // Is applicable
                        if ((value & @case.Value) != @case.Value) continue;
                        // Append other key parts
                        ILine key = keyBase.Concat(@case.Key);
                        // Resolve
                        LineString case_resolve = ctx.StringResolver.ResolveString(key);
                        // Was localization string found?
                        if (case_resolve.Value != null)
                        {
                            if (case_resolve.Warning) status.UpPlaceholder(LineStatus.PlaceholderWarningEnum);
                            else if (case_resolve.Error) status.UpPlaceholder(LineStatus.PlaceholderErrorEnum);
                            caseStr = case_resolve.Value;
                            // Remove flag
                            value &= ~@case.Value;
                            // Add to results
                            resultCases.Add((@case, caseStr));
                            break;
                        }
                    }

                    // Second run, name or description from @case
                    if (caseStr == null)
                    {
                        // First run, find matching case
                        foreach (IEnumCase @case in enumInfo.EvalCases)
                        {
                            // Is applicable
                            if ((value & @case.Value) != @case.Value) continue;
                            //
                            caseStr = @case.Description ?? @case.Name;
                            // 
                            status.UpPlaceholder(LineStatus.PlaceholderErrorEnumNoMatch);
                            // Remove flag
                            value &= ~@case.Value;
                            // Add to results
                            resultCases.Add((@case, caseStr));
                            break;
                        }
                    }

                    // Fallback as number
                    if (caseStr == null)
                    {
                        status.UpPlaceholder(LineStatus.PlaceholderWarningEnumCaseNotMatched);
                        caseStr = value.ToString(ctx.Culture);
                        // Add to results
                        resultCases.Add((null, caseStr));
                        value = 0UL;
                    }
                }

                // Sort by to occurance order
                if (resultCases.Count > 1) CaseSorter.Sorter.Sort(ref resultCases);

                // Put together a result string
                String text = null;
                if (resultCases.Count == 0) text = "";
                else if (resultCases.Count == 1) text = resultCases[0].Item2;
                else
                {
                    int length = (resultCases.Count - 1) * separator.Length;
                    for (int i = 0; i < resultCases.Count; i++) length += resultCases[i].Item2.Length;
                    char[] chars = new char[length];
                    int ix = 0;
                    for (int i = 0; i < resultCases.Count; i++)
                    {
                        // Append separator
                        if (i>0)
                        {
                            separator.CopyTo(0, chars, ix, separator.Length);
                            ix += separator.Length;
                        }
                        // Append text
                        String caseText = resultCases[i].Item2;
                        caseText.CopyTo(0, chars, ix, caseText.Length);
                        ix += caseText.Length;
                    }
                    text = new String(chars);
                }

                // Return result
                status.UpPlaceholder(LineStatus.PlaceholderOkEnum);
                return new LineString(ctx.ResolvedLine, text, status);
            } catch (Exception e)
            {
                status.UpPlaceholder(LineStatus.PlaceholderFailedEnum);
                return new LineString(ctx.ResolvedLine, e, status);
            }
        }


        /// <summary>
        /// Qualifier that approves all but "Key", "Assembly", "Type" and "Section" parameters.
        /// 
        /// Used for converting request key to enum key.
        /// </summary>
        public static readonly ILineQualifier enum_key_prune_qualifier = new LineQualifier().Rule("Key", -1, "").Rule("Assembly", -1, "").Rule("Type", -1, "").Rule("Section", -1, "");

        /// <summary>
        /// Used for pruning unwanted parts from base key, so that key can be used for base keys for enum localization.
        /// 
        /// Rules:
        ///   1. Disqualify "Assembly", "Key", "Type" parameters.
        ///   2. Disqualify <see cref="ILineString"/>, <see cref="ILineValue"/>.
        ///  
        /// </summary>
        public class LineEnumBaseKeyPruner : ILineArgumentQualifier
        {
            private static readonly LineEnumBaseKeyPruner instance = new LineEnumBaseKeyPruner();

            /// <summary>
            /// Default singleton instance.
            /// </summary>
            public static LineEnumBaseKeyPruner Default => instance;

            /// <summary>
            /// Doesn't need occurance
            /// </summary>
            public bool NeedsOccuranceIndex => false;

            /// <summary>
            /// Evaluate argument
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns></returns>
            public bool QualifyArgument(ILineArgument argument, int occuranceIndex = -1)
            {
                if (argument is ILineArgument<ILineString, IString>) return false;
                if (argument is ILineArgument<ILineValue, object[]>) return false;
                if (argument is ILineArgument<ILineType, Type>) return false;
                if (argument is ILineArgument<ILineAssembly, Assembly>) return false;
                //if (argument is ILineArgument<ILineInlines, IDictionary<ILine, ILine>>) return false;
                string parameterName, parameterValue;
                if (argument.TryGetParameter(out parameterName, out parameterValue))
                {
                    if (parameterName == "Assembly" || parameterName == "Type" || parameterName == "Key") return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Used for pruning unwanted parts from base key, so that key can be used for base keys for enum localization.
        /// 
        /// Rules:
        ///   1. Disqualifies known key parameters.
        ///   2. Disqualify <see cref="ILineString"/>, <see cref="ILineValue"/>.
        ///  
        /// </summary>
        public class LineEnumBaseKeyPruner2 : ILineArgumentQualifier
        {
            private static readonly LineEnumBaseKeyPruner2 instance = new LineEnumBaseKeyPruner2(Lexical.Localization.Utils.ParameterInfos.Default);

            /// <summary>
            /// Default singleton instance.
            /// </summary>
            public static LineEnumBaseKeyPruner2 Default => instance;

            /// <summary>
            /// Doesn't need occurance
            /// </summary>
            public bool NeedsOccuranceIndex => false;

            /// <summary>
            /// Parameter infos for recognizing key parameters
            /// </summary>
            public readonly IParameterInfos ParameterInfos;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parameterInfos"></param>
            public LineEnumBaseKeyPruner2(IParameterInfos parameterInfos)
            {
                ParameterInfos = parameterInfos;
            }

            /// <summary>
            /// Evaluate argument
            /// </summary>
            /// <param name="argument"></param>
            /// <param name="occuranceIndex"></param>
            /// <returns></returns>
            public bool QualifyArgument(ILineArgument argument, int occuranceIndex = -1)
            {
                if (argument is ILineArgument<ILineString, IString>) return false;
                if (argument is ILineArgument<ILineValue, object[]>) return false;
                if (argument is ILineArgument<ILineType, Type>) return false;
                if (argument is ILineArgument<ILineAssembly, Assembly>) return false;
                //if (argument is ILineArgument<ILineInlines, IDictionary<ILine, ILine>>) return false;
                string parameterName, parameterValue;
                if (argument.TryGetParameter(out parameterName, out parameterValue))
                {
                    IParameterInfo pi;
                    if (ParameterInfos.TryGetValue(parameterName, out pi))
                        return pi.InterfaceType == typeof(ILineNonCanonicalKey) || pi.InterfaceType == typeof(ILineCanonicalKey);
                }

                return true;
            }
        }

        /// <summary>
        /// Sorts cases based their occurance
        /// </summary>
        public class CaseSorter : IComparer<(IEnumCase, string)>
        {
            private static StructListSorter<StructList8<(IEnumCase, string)>, (IEnumCase, string)> sorter = new StructListSorter<StructList8<(IEnumCase, string)>, (IEnumCase, string)>(new CaseSorter());

            /// <summary>
            /// List sorter
            /// </summary>
            public static StructListSorter<StructList8<(IEnumCase, string)>, (IEnumCase, string)> Sorter => sorter;

            /// <summary>
            /// Compare lines
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare((IEnumCase, string) x, (IEnumCase, string) y)
            {
                if (x.Item1 == null && y.Item1 == null) return 0;
                if (x.Item1 == null) return -1;
                if (y.Item1 == null) return 1;
                return x.Item1.CaseIndex - y.Item1.CaseIndex;
            }
        }
    }

}
