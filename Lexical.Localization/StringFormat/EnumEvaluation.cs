// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
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
        public static LineString EvaluateEnum(this ref FunctionEvaluationContext ctx, Enum @enum)
        {
            // Status
            LineStatus status = 0UL;
            try
            {
                // Get enum info
                IEnumInfo enumInfo = ctx.EnumResolver != null ? ctx.EnumResolver.GetEnumInfo(@enum.GetType()) : new EnumInfo(@enum.GetType());
                // Get value
                ulong value = EnumCase.ToUInt64(@enum);
                // Create string
                StringBuilder sb = new StringBuilder();
                // Separator between cases
                String separator = ", ";
                // Remove hash-equals comparable key parts.
                ILine keyBase = ctx.Line.Prune(enum_key_prune_qualifier, LineAppender.NonResolving);
                // Append culture
                if (ctx.Culture != null) keyBase = keyBase.Culture(ctx.Culture);
                // Split into cases
                while (value != 0UL)
                {
                    string caseStr = null;
                    foreach (IEnumCase @case in enumInfo.EvalCases)
                    {
                        // Is applicable
                        if ((value & @case.Value) != @case.Value) continue;
                        // Remove flag
                        value &= ~@case.Value;
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
                            break;
                        }
                    }

                    // Fallback as number
                    if (caseStr == null)
                    {
                        status.UpPlaceholder(LineStatus.PlaceholderWarningEnumCaseNotMatched);
                        caseStr = value.ToString(ctx.Culture);
                        value = 0UL;
                    }

                    // Append to sb
                    if (sb.Length > 0) sb.Append(separator);
                    sb.Append(caseStr);
                }

                // Return result
                status.UpPlaceholder(LineStatus.PlaceholderOkEnum);
                return new LineString(ctx.Line, sb.ToString(), status);
            } catch (Exception e)
            {
                status.UpPlaceholder(LineStatus.PlaceholderFailedEnum);
                return new LineString(ctx.Line, e, status);
            }
        }


        /// <summary>
        /// Qualifier that approves all but "Key", "Assembly", "Type" and "Section" parameters.
        /// 
        /// Used for converting request key to enum key.
        /// </summary>
        public static readonly ILineQualifier enum_key_prune_qualifier = new LineQualifier().Rule("Key", -1, "").Rule("Assembly", -1, "").Rule("Type", -1, "").Rule("Section", -1, "");

    }
}
