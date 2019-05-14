// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods that add rules to qualifier compositions.
    /// </summary>
    public static partial class LineQualifierExtensions
    {
        /// <summary>
        /// Add the parameters of <paramref name="qualifierKey"/> as criteria for qualifier.
        /// 
        /// Every parameter of <paramref name="qualifierKey"/> will be required to match in the lines that will be compared with this qualifier. 
        /// If a parameterValue in the <paramref name="qualifierKey"/> is "", then the compared key must not have that parameter, or it must be "".
        /// 
        /// (Does not expect canoncials to be in same order in relation to different ParameterNames.)
        /// </summary>
        /// <param name="qualifier">configurable qualifier composition</param>
        /// <param name="qualifierKey">parameters</param>
        /// <param name="parameterInfos">(optional) infos for determining which keys are non-canonical</param>
        /// <returns><paramref name="qualifier"/></returns>
        public static ILineQualifier Rule(this ILineQualifier qualifier, ILine qualifierKey, IParameterInfos parameterInfos = null)
        {
            // Break qualifierKey into effective non-canonical parameters, and to canonical parameters and occurance index
            StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
            qualifierKey.GetParameterPartsWithOccurance(ref list);

            // Add rules
            foreach ((ILineParameter parameter, int occuranceIndex) in list)
            {
                // Parameter with null is to be ignored
                if (parameter.ParameterName == null || parameter.ParameterValue == null) continue;
                // Ineffective key
                if (occuranceIndex > 0 && parameter.IsNonCanonicalKey(parameterInfos)) continue;
                // parameter with "" is expectation of empty or non-existant value.
                if (parameter.ParameterValue == "") qualifier.Add(new LineQualifierRule.IsEmpty(parameter.ParameterName, occuranceIndex));
                else qualifier.Add(new LineQualifierRule.IsEqualTo(parameter.ParameterName, occuranceIndex, parameter.ParameterValue));
            }

            return qualifier;
        }

        /// <summary>
        /// Add regular expression matching rule.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurnace index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="pattern">pattern</param>
        /// <returns><paramref name="qualifier"/></returns>
        public static ILineQualifier Rule(this ILineQualifier qualifier, string parameterName, int occuranceIndex, Regex pattern)
        {
            qualifier.Add(new LineQualifierRule.Regex(parameterName, occuranceIndex, pattern));
            return qualifier;
        }

        /// <summary>
        /// Add expected value rule.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurance index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="expectedParameterValue">expected value, or if "", then expects value to not occur</param>
        /// <returns><paramref name="qualifier"/></returns>
        public static ILineQualifier Rule(this ILineQualifier qualifier, string parameterName, int occuranceIndex, string expectedParameterValue)
        {
            if (expectedParameterValue == null) throw new ArgumentNullException(nameof(expectedParameterValue));
            if (expectedParameterValue == "") qualifier.Add(new LineQualifierRule.IsEmpty(parameterName, occuranceIndex));
            else qualifier.Add(new LineQualifierRule.IsEqualTo(parameterName, occuranceIndex, expectedParameterValue));
            return qualifier;
        }

        /// <summary>
        /// Add expected value rule.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurnace index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="acceptedParameterValues">group of accepted parameter values</param>
        /// <returns><paramref name="qualifier"/></returns>
        public static ILineQualifier Rule(this ILineQualifier qualifier, string parameterName, int occuranceIndex, params string[] acceptedParameterValues)
        {
            if (acceptedParameterValues == null || acceptedParameterValues.Length == 0) return qualifier;
            if (acceptedParameterValues.Length == 1) qualifier.Rule(parameterName, occuranceIndex, acceptedParameterValues[0]);
            else qualifier.Add(new LineQualifierRule.IsInGroup(parameterName, occuranceIndex, acceptedParameterValues));
            return qualifier;
        }

        /// <summary>
        /// Qualifier by <paramref name="keyMatch"/>.
        /// 
        /// If <see cref="ILinePatternMatch.Success"/> is false then return false.
        /// 
        /// The whole <paramref name="keyMatch"/> is matched against every qualifier, if one disqualifies then returns false.
        /// </summary>
        /// <param name="qualifier"></param>
        /// <param name="keyMatch">(optional) </param>
        /// <returns>true if line is qualified, false if disqualified</returns>
        public static bool Qualify(this ILineQualifier qualifier, ILinePatternMatch keyMatch)
            => keyMatch != null && keyMatch.Success && qualifier.Qualify(keyMatch.ToLine());

        /// <summary>
        /// Convert <paramref name="match"/> to line where occurance indices are correct.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static ILine ToLine(this ILinePatternMatch match)
        {
            // Apply parameter qualifiers
            ILine line = null;
            for (int i = 0; i < match.PartValues.Length; i++)
            {
                string parameterValue = match.PartValues[i];
                if (parameterValue == null) continue;
                string parameterName = match.Pattern.CaptureParts[i].ParameterName;
                line = new LineParameter(null, line, parameterName, parameterValue);

                // TODO Put line parts in occurance order //
            }
            return line;
        }

    }

}
