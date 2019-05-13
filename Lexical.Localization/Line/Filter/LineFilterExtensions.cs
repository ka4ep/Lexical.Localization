// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Text.RegularExpressions;

namespace Lexical.Localization
{
    /// <summary>
    /// Extension methods that add rules to filter composites.
    /// </summary>
    public static partial class LineFilterExtensions
    {
        /// <summary>
        /// Add the parameters of <paramref name="filterKey"/> as criteria for filter.
        /// 
        /// Every parameter of <paramref name="filterKey"/> will be required to match in the lines that will be compared with this filter. 
        /// If a parameterValue in the <paramref name="filterKey"/> is "", then the compared key must not have that parameter, or it must be "".
        /// </summary>
        /// <param name="filter">configurable filter composition</param>
        /// <param name="filterKey">parameters</param>
        /// <returns><paramref name="filter"/></returns>
        public static ILineFilter Rule(this ILineFilter filter, ILine filterKey)
        {
            // Break filterKey into effective non-canonical parameters, and to canonical parameters and occurance index
            StructList12<(ILineParameter, int)> list = new StructList12<(ILineParameter, int)>();
            filterKey.GetParameterPartsWithOccurance(ref list);

            // Add rules
            foreach ((ILineParameter parameter, int occuranceIndex) in list)
            {
                // Parameter with null is to be ignored
                if (parameter.ParameterName == null || parameter.ParameterValue == null) continue;
                // parameter with "" is expectation of empty or non-existant value.
                if (parameter.ParameterValue == "") filter.Add(new LineFilterRule.IsEmpty(parameter.ParameterName, occuranceIndex));
                else filter.Add(new LineFilterRule.IsEqualTo(parameter.ParameterName, occuranceIndex, parameter.ParameterValue));
            }

            return filter;
        }

        /// <summary>
        /// Add regular expression matching rule.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurnace index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="pattern">pattern</param>
        /// <returns><paramref name="filter"/></returns>
        public static ILineFilter Rule(this ILineFilter filter, string parameterName, int occuranceIndex, Regex pattern)
        {
            filter.Add(new LineFilterRule.Regex(parameterName, occuranceIndex, pattern));
            return filter;
        }

        /// <summary>
        /// Add expected value rule.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurance index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="expectedParameterValue">expected value, or if "", then expects value to not occur</param>
        /// <returns><paramref name="filter"/></returns>
        public static ILineFilter Rule(this ILineFilter filter, string parameterName, int occuranceIndex, string expectedParameterValue)
        {
            if (expectedParameterValue == null) throw new ArgumentNullException(nameof(expectedParameterValue));
            if (expectedParameterValue == "") filter.Add(new LineFilterRule.IsEmpty(parameterName, occuranceIndex));
            else filter.Add(new LineFilterRule.IsEqualTo(parameterName, occuranceIndex, expectedParameterValue));
            return filter;
        }

        /// <summary>
        /// Add expected value rule.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parameterName">parameter name this rule applies to</param>
        /// <param name="occuranceIndex">occurnace index this rule applies to, or -1 to apply to every occurance of the parameter name</param>
        /// <param name="acceptedParameterValues">group of accepted parameter values</param>
        /// <returns><paramref name="filter"/></returns>
        public static ILineFilter Rule(this ILineFilter filter, string parameterName, int occuranceIndex, params string[] acceptedParameterValues)
        {
            if (acceptedParameterValues == null || acceptedParameterValues.Length == 0) return filter;
            if (acceptedParameterValues.Length == 1) filter.Rule(parameterName, occuranceIndex, acceptedParameterValues[0]);
            else filter.Add(new LineFilterRule.IsInGroup(parameterName, occuranceIndex, acceptedParameterValues));
            return filter;
        }

        /// <summary>
        /// Filter by <paramref name="keyMatch"/>.
        /// 
        /// If <see cref="ILinePatternMatch.Success"/> is false then return false.
        /// 
        /// The whole <paramref name="keyMatch"/> is matched against every filter, if one disqualifies then returns false.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="keyMatch">(optional) </param>
        /// <returns>true if line is qualified by the filter, false if disqualified</returns>
        public static bool Filter(this ILineFilter filter, ILinePatternMatch keyMatch)
            => keyMatch != null && keyMatch.Success && filter.Filter(keyMatch.ToLine());

        /// <summary>
        /// Convert <paramref name="match"/> to line where occurance indices are correct.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static ILine ToLine(this ILinePatternMatch match)
        {
            // Apply parameter filters
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
