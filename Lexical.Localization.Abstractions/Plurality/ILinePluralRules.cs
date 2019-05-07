// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Globalization;
using Lexical.Localization.Plurality;

namespace Lexical.Localization
{
    /// <summary>
    /// Key that has specific <see cref="IPluralRules"/> instance assigned.
    /// </summary>
    public interface ILinePluralRules : ILine
    {
        /// <summary>
        /// (optional) Assigned instance of plural rules.
        /// </summary>
        IPluralRules PluralRules { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class IPluralRulesExtensions
    {
        /// <summary>
        /// Assign a specific instance of rules. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rules"></param>
        /// <returns>new key with rules</returns>
        public static ILinePluralRules PluralRules(this ILine key, IPluralRules rules)
            => key.Append<ILinePluralRules, IPluralRules>(rules);

        /// <summary>
        /// Assign plurality rules as 
        /// <list type="bullet">
        /// <item>Assign assembly qualified type name of <see cref="IPluralRules"/>, e.g. "Unicode.CLDR35"</item>
        /// <item>Plural rules expression (starts with '['), e.g. "[Category=cardinal,Case=One,Optional=1]n=0[Category=cardinal,Case=One]n=1[Category=cardinal,Case=Other]true"</item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ruleSet"></param>
        /// <returns>new key with rules</returns>
        public static ILine PluralRules(this ILine key, string ruleSet)
            => key.Parameter("PluralRules", ruleSet);

        /// <summary>
        /// Assign plurality case to argument <paramref name="argumentIndex"/>.
        /// 
        /// Appends a parameter "Nx:case", e.g. "N1:many".
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="argumentIndex">argument index to configure plurality, e.g. 0 = "{0}" in the format string</param>
        /// <param name="case">Plurality case, e.g. "zero", "one", "other"</param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="LineException">if key cannot be assigned.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="argumentIndex"/> is smaller than 0</exception>
        public static ILine N(this ILine key, int argumentIndex, string @case)
            => key.Parameter(PrintPluralityParameter(argumentIndex), @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 0, "N:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N(this ILine key, string @case)
            => key.Parameter("N", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 1, "N1:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N1(this ILine key, string @case)
            => key.Parameter("N1", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 2, "N2:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N2(this ILine key, string @case)
            => key.Parameter("N2", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 3, "N3:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N3(this ILine key, string @case)
            => key.Parameter("N3", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 4, "N4:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N4(this ILine key, string @case)
            => key.Parameter("N4", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 5, "N5:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N5(this ILine key, string @case)
            => key.Parameter("N5", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 6, "N6:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N6(this ILine key, string @case)
            => key.Parameter("N6", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 7, "N7:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N7(this ILine key, string @case)
            => key.Parameter("N7", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 8, "N8:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N8(this ILine key, string @case)
            => key.Parameter("N8", @case);

        /// <summary>
        /// Assign <paramref name="case"/> for argument 9, "N9:case".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="case"></param>
        /// <returns></returns>
        public static ILine N9(this ILine key, string @case)
            => key.Parameter("N9", @case);

        /// <summary>
        /// Static parameter names for plurality keys.
        /// </summary>
        public static string[] ParameterNames = new string[] { "N", "N1", "N2", "N3", "N4", "N5", "N6", "N7", "N8", "N9", "N10", "N11", "N12", "N13", "N14", "N15" };

        /// <summary>
        /// Tries to parse parametername "Nx" into argument index.
        /// </summary>
        /// <param name="parameterName">Parameter name, such as "N", or "N1"</param>
        /// <param name="argumentIndex">argument index</param>
        /// <returns>true if parameter was argument index, false if not</returns>
        public static bool TryParsePluralityParameter(string parameterName, out int argumentIndex)
        {
            // Assert arguments
            if (String.IsNullOrEmpty(parameterName) || parameterName[0] != 'N') { argumentIndex = -1; return false; }
            // 'N'
            if (parameterName.Length == 1) { argumentIndex = 0; return true; }
            // "Nxxx"
            int result = 0;
            for (int i=1; i<parameterName.Length; i++)
            {
                // Get char
                char ch = parameterName[i];
                // Non-digit
                if (ch<'0'||ch>'9') { argumentIndex = -1; return false; }
                // Calculate
                int digit = ch - '0';
                result = i > 1 ? 10 * result + digit : digit;
            }
            argumentIndex = result;
            return true;
        }

        /// <summary>
        /// Print argument index into string.
        /// </summary>
        /// <param name="argumentIndex"></param>
        /// <returns>string</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="argumentIndex"/> is smaller than 0</exception>
        public static string PrintPluralityParameter(int argumentIndex)
        {
            if (argumentIndex < 0) throw new ArgumentOutOfRangeException(nameof(argumentIndex));
            if (argumentIndex < ParameterNames.Length) return ParameterNames[argumentIndex];
            return "N" + argumentIndex.ToString(CultureInfo.InvariantCulture);
        }

    }
}
