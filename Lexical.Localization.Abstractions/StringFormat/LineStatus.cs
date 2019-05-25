// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Status code for <see cref="LineString"/>.
    /// </summary>
    [Flags]
    public enum LineStatus : UInt64
    {
        /// <summary>Request has not been resolved</summary>
        NoResult = 0xFFFFFFFFFFFFFFFFUL,
        /// <summary>Unknown error</summary>
        FailedUnknownReason = ResolveFailed|CultureFailed|PluralityFailed|PlaceholderFailed|FormatFailed,

        //// Resolve - Step that searches format string for ILine from IAsset or Inlines.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveOk = 0x00UL << Shift.Resolve,
        /// <summary>Resolved value from key</summary>
        ResolveOkFromKey = 0x04UL << Shift.Resolve,
        /// <summary>Resolved value from asset</summary>
        ResolveOkFromAsset = 0x06UL << Shift.Resolve,
        /// <summary>Resolved value from inlines</summary>
        ResolveOkFromInline = 0x08UL << Shift.Resolve,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveWarning = 0x20UL << Shift.Resolve,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveError = 0x40UL << Shift.Resolve,
        /// <summary>No match</summary>
        ResolveErrorNoMatch = 0x42UL << Shift.Resolve,
        /// <summary>IAsset.GetString throw exception.</summary>
        ResolveErrorAssetException = 0x44UL << Shift.Resolve,
        /// <summary>ILineInlines throw exception.</summary>
        ResolveErrorInlinesException = 0x46UL << Shift.Resolve,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveFailed = 0x60UL << Shift.Resolve,
        /// <summary>Could not find a line with a value</summary>
        ResolveFailedNoValue = 0x62UL << Shift.Resolve,
        /// <summary>Result has not been processed</summary>
        ResolveFailedNoResult = 0x7FUL << Shift.Resolve,
        /// <summary>Mask for severity</summary>
        ResolveSeverityMask = 0x60UL << Shift.Resolve,
        /// <summary>Mask for resolve status</summary>
        ResolveMask = 0x7FUL << Shift.Resolve,

        //// Culture - Step that matches active culture, culture policy to strings available in asset
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureOk = 0x00UL << Shift.Culture,
        /// <summary>Key contained no culture, and culture policy contained no culture</summary>
        CultureOkNotUsed = 0x01UL << Shift.Culture,
        /// <summary>Key contained an explicit culture, and it matched to a line</summary>
        CultureOkMatchedCulture = 0x04UL << Shift.Culture,
        /// <summary>Key contained culture policy which provided a culture that matched to a line</summary>
        CultureOkMatchedCulturePolicy = 0x08UL << Shift.Culture,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureWarning = 0x20UL << Shift.Culture,
        /// <summary>Key requested a specific culture, but a fallback culture was used</summary>
        CultureWarningFallback = 0x21UL << Shift.Culture,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureError = 0x40UL << Shift.Culture,
        /// <summary>No matching value was found for the requested key and the explicit culture in the key.</summary>
        CultureErrorCultureNoMatch = 0x43UL << Shift.Culture,
        /// <summary>No matching value was found for the requested key and the cultures that culture policy provided.</summary>
        CultureErrorCulturePolicyNoMatch = 0x44UL << Shift.Culture,
        /// <summary>CulturePolicy throw exception</summary>
        CultureErrorCulturePolicyException = 0x45UL << Shift.Culture,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureFailed = 0x60UL << Shift.Culture,
        /// <summary>Result has not been processed</summary>
        CultureFailedNoResult = 0x7FUL << Shift.Culture,
        /// <summary>Mask for severity</summary>
        CultureSeverityMask = 0x60UL << Shift.Culture,
        /// <summary>Mask for culture status</summary>
        CultureMask = 0x7FUL << Shift.Culture,

        //// Plurality - Step that matches plurality cases
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityOk = 0x00UL << Shift.Plurality,
        /// <summary>No plurality categories was used</summary>
        PluralityOkNotUsed = 0x01UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument format(s), and the plurality cases were found in the asset/inlines and used</summary>
        PluralityOkMatched = 0x02UL << Shift.Plurality,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityWarning = 0x20UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument format(s), and plurality cases were found for some arguments, but not all</summary>
        PluralityWarningPartiallyMatched = 0x21UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument format(s), but the plurality cases were not found in the asset/inlines, fallbacked to default string</summary>
        PluralityWarningNotMatched = 0x22UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument format, but the provided value was not a number</summary>
        PluralityWarningNotNumber = 0x23UL << Shift.Plurality,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityError = 0x40UL << Shift.Plurality,
        /// <summary>Value contained plurality category, but the plurality rules were not found</summary>
        PluralityErrorRulesNotFound = 0x46UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument format(s), but the plurality rules were not found in the key or in the asset</summary>
        PluralityErrorRulesResolveError = 0x47UL << Shift.Plurality,
        /// <summary>Placeholder is null, but expected a number</summary>
        PluralityErrorPlaceholderNull = 0x48UL << Shift.Plurality,
        /// <summary>Placeholder is not a number</summary>
        PluralityErrorPlaceholderNotNumber = 0x4BUL << Shift.Plurality,
        /// <summary>Placeholder text is null, but expected a string</summary>
        PluralityErrorPlaceholderTextNull = 0x4DUL << Shift.Plurality,
        /// <summary>Could not parse plurality information without CultureInfo or NumberFormatInfo</summary>
        PluralityErrorPlaceholderNumberFormatNull = 0x50UL << Shift.Plurality,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityFailed = 0x60UL << Shift.Plurality,
        /// <summary>Result has not been processed</summary>
        PluralityFailedNoResult = 0x7FUL << Shift.Plurality,
        /// <summary>Mask for severity</summary>
        PluralitySeverityMask = 0x60UL << Shift.Plurality,
        /// <summary>Mask for plurality status</summary>
        PluralityMask = 0x7FUL << Shift.Plurality,

        //// Placeholder - Step that evaluates placeholder expression
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PlaceholderOk = 0x00UL << Shift.Placeholder,
        /// <summary>One of the argument was null, or custom formatter resulted into null.</summary>
        PlaceholderOkNull = 0x02UL << Shift.Placeholder,
        /// <summary>Request asked for the format string, without applying arguments to it.</summary>
        PlaceholderOkNotApplied = 0x03L << Shift.Format,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PlaceholderWarning = 0x20UL << Shift.Placeholder,
        /// <summary>No IFormattable implementation or ICustomFormatter was found, or they returned null, ToString was applied</summary>
        PlaceholderWarningToStringUsed = 0x21UL << Shift.Placeholder,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PlaceholderError = 0x40UL << Shift.Placeholder,
        /// <summary>Got exception while evaluating placeholder exception</summary>
        PlaceholderErrorExpressionEvaluationException = 0x45UL << Shift.Placeholder,
        /// <summary>The argument object did not match the type in format string</summary>
        PlaceholderErrorTypeMismatch = 0x48UL << Shift.Placeholder,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PlaceholderFailed = 0x60UL << Shift.Placeholder,
        /// <summary>Result has not been processed</summary>
        PlaceholderFailedNoResult = 0x7FUL << Shift.Placeholder,
        /// <summary>Mask for severity</summary>
        PlaceholderSeverityMask = 0x60UL << Shift.Placeholder,
        /// <summary>Mask for argument status</summary>
        PlaceholderMask = 0x7FUL << Shift.Placeholder,

        //// Format - Step that parses format string, inserts arguments, and builds into formulated string.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormatOk = 0x00UL << Shift.Format,
        /// <summary>There were no arguments to formulate</summary>
        FormatOkNoPlaceholders = 0x02UL << Shift.Format,
        /// <summary>Request asked for the format string, without applying arguments to it.</summary>
        FormatOkNotApplied = 0x03UL << Shift.Format,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormatWarning = 0x20UL << Shift.Format,
        /// <summary>Format string contained arguments, but too many arguments were provided</summary>
        FormatWarningTooManyPlaceholders = 0x21UL << Shift.Format,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormatError = 0x40UL << Shift.Format,
        /// <summary>Failed to resolve class name to <see cref="IFormatProvider"/></summary>
        FormatErrorFormatProviderResolveFailed = 0x43UL << Shift.Format,
        /// <summary>Failed to resolve class name to <see cref="IFunctions"/></summary>
        FormatErrorFunctionsResolveFailed = 0x44UL << Shift.Format,
        /// <summary>Failed to resolve class name to <see cref="IStringFormat"/></summary>
        FormatErrorStringFormatResolveFailed = 0x45UL << Shift.Format,
        /// <summary>Format string contained arguments, but arguments were not provided</summary>
        FormatErrorNoPlaceholders = 0x46UL << Shift.Format,
        /// <summary>Format string contained arguments, but too few arguments were provided. Using null values for missing arguments.</summary>
        FormatErrorTooFewPlaceholders = 0x48UL << Shift.Format,
        /// <summary>Format string was malformed. Returning the malformed string as value.</summary>
        FormatErrorMalformed = 0x4fUL << Shift.Format,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormatFailed = 0x60UL << Shift.Format,
        /// <summary>Format string is null</summary>
        FormatFailedNull = 0x601L << Shift.Format,
        /// <summary>No <see cref="IStringFormatParser"/></summary>
        FormatFailedNoParser = 0x607L << Shift.Format,
        /// <summary>Format parse failed</summary>
        FormatFailedParse = 0x609L << Shift.Format,
        /// <summary>Result has not been processed</summary>
        FormatFailedNoResult = 0x7FUL << Shift.Format,
        /// <summary>Mask for severity</summary>
        FormatSeverityMask = 0x60UL << Shift.Format,
        /// <summary>Mask for argument status</summary>
        FormatMask = 0x7FUL << Shift.Format,

        //// Custom0 - ILineResolver implementation specific status flags. Can be used for any purpose by the resolver.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Ok = 0x00UL << Shift.Custom0,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Warning = 0x20UL << Shift.Custom0,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Error = 0x40UL << Shift.Custom0,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Failed = 0x60UL << Shift.Custom0,
        /// <summary>Result has not been processed</summary>
        Custom0FailedNoResult = 0x7FUL << Shift.Custom0,
        /// <summary>Mask for severity</summary>
        Custom0SeverityMask = 0x60UL << Shift.Custom0,
        /// <summary>Mask for argument status</summary>
        Custom0Mask = 0x7FUL << Shift.Custom0,

        //// Custom0 - ILineResolver implementation specific status flags. Can be used for any purpose by the resolver.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Ok = 0x00UL << Shift.Custom1,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Warning = 0x20UL << Shift.Custom1,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Error = 0x40UL << Shift.Custom1,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Failed = 0x60UL << Shift.Custom1,
        /// <summary>Result has not been processed</summary>
        Custom1FailedNoResult = 0x7FUL << Shift.Custom1,
        /// <summary>Mask for severity</summary>
        Custom1SeverityMask = 0x60UL << Shift.Custom1,
        /// <summary>Mask for argument status</summary>
        Custom1Mask = 0x7FUL << Shift.Custom1,

        /// <summary>Last bit is reserverd</summary>
        Reserved3 = 0x8000000000000000UL
    }

    /// <summary>
    /// Contains bit-shifts for each status category.
    /// </summary>
    internal static class Shift
    {
        // bit-shifts for categories
        internal const int Resolve = 0;
        internal const int Culture = 7;
        internal const int Plurality = 14;
        internal const int Placeholder = 21;
        internal const int Format = 28;
        internal const int Custom0 = 35;    // ILineResolver implemtation can use for any custom purpose.
        internal const int Custom1 = 42;    // ILineResolver implemtation can use for any custom purpose.
        internal const int Reserved0 = 49;  // Reserved for future use
        internal const int Reserved1 = 56;  // Reserved for future use

        // bit shifts for severity bits (2bits) of each category.
        internal const int ResolveSeverity = Resolve + 5;
        internal const int CultureSeverity = Culture + 5;
        internal const int PluralitySeverity = Plurality + 5;
        internal const int PlaceholderSeverity = Placeholder + 5;
        internal const int FormatSeverity = Format + 5;
        internal const int Custom0Severity = Custom0 + 5;
        internal const int Custom1Severity = Custom1 + 5;
        internal const int Reserved0Severity = Reserved0 + 5;  // Reserved for future use
        internal const int Reserved1Severity = Reserved1 + 5;  // Reserved for future use

    }

    /// <summary></summary>
    public static class LineStatusExtensions
    {
        /// <summary>
        /// Severity for the step that resolves <see cref="ILine"/> into format string.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int ResolveSeverity(this LineStatus status) 
            => (int)((ulong)status >> Shift.ResolveSeverity) & 3;

        /// <summary>
        /// Get resolve step status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Resolve(this LineStatus status)
            => status & LineStatus.ResolveMask;

        /// <summary>
        /// Severity for the step that matches culture.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int CultureSeverity(this LineStatus status) 
            => (int)((ulong)status >> Shift.CultureSeverity) & 3;

        /// <summary>
        /// Get culture step status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Culture(this LineStatus status)
            => status & LineStatus.CultureMask;

        /// <summary>
        /// Severity for the step applies Plurality_.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public static int PluralitySeverity(this LineStatus status) 
            => (int)((ulong)status >> Shift.PluralitySeverity) & 3;

        /// <summary>
        /// Get plurality step status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Plurality(this LineStatus status)
            => status & LineStatus.PluralityMask;

        /// <summary>
        /// Severity for the step that converts arguments into strings
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int PlaceholderSeverity(this LineStatus status) 
            => (int)((ulong)status >> Shift.PlaceholderSeverity) & 3;

        /// <summary>
        /// Get argument status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Placeholder(this LineStatus status)
            => status & LineStatus.PlaceholderMask;

        /// <summary>
        /// Severity for the step that parses format string and applies arguments.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int FormatSeverity(this LineStatus status) 
            => (int)((ulong)status >> Shift.FormatSeverity) & 3;

        /// <summary>
        /// Get format step status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Format(this LineStatus status)
            => status & LineStatus.FormatMask;

        /// <summary>
        /// Severity for <see cref="IStringResolver"/> implementation specific "Custom0" status.
        /// 
        /// "Custom0" is a status code that is specific to the <see cref="IStringResolver"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int Custom0Severity(this LineStatus status)
            => (int)((ulong)status >> Shift.Custom0Severity) & 3;

        /// <summary>
        /// Get custom0 status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Custom0(this LineStatus status)
            => status & LineStatus.Custom0Mask;

        /// <summary>
        /// Severity for <see cref="IStringResolver"/> implementation specific "Custom1" status.
        /// 
        /// "Custom1" is a status code that is specific to the <see cref="IStringResolver"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int Custom1Severity(this LineStatus status) 
            => (int)((ulong)status >> Shift.Custom1Severity) & 3;

        /// <summary>
        /// Get custom1 status code.
        /// </summary>
        /// <param name="status"></param>
        public static LineStatus Custom1(this LineStatus status)
            => status & LineStatus.Custom1Mask;

        /// <summary>
        /// Highest severity value out of each category.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        public static int Severity(this LineStatus status)
        {
            int a = status.ResolveSeverity(), b = status.CultureSeverity(), c = status.PluralitySeverity(), d = status.PlaceholderSeverity(), e = status.FormatSeverity(), h = status.Custom0Severity(), i = status.Custom1Severity();
            int result = a;
            if (b > result) result = b;
            if (c > result) result = c;
            if (d > result) result = d;
            if (e > result) result = e;
            // if (f > result) result = f;
            //if (g > result) result = g;
            if (h > result) result = h;
            if (i > result) result = i;
            return result;
        }

        /// <summary>
        /// Tests if there is no result.
        /// </summary>
        /// <param name="status"></param>
        public static bool HasResult(this LineStatus status) 
            => status != LineStatus.NoResult;

        /// <summary>
        /// Result has ok state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Produced ok value.
        /// </summary>
        /// <param name="status"></param>
        public static bool Ok(this LineStatus status) 
            => status.Severity() == 0;

        /// <summary>
        /// Result has warning state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Warning state has a value, but there was something occured during the resolve that may need attention.
        /// </summary>
        /// <param name="status"></param>
        public static bool Warning(this LineStatus status) 
            => status.Severity() == 1;

        /// <summary>
        /// Result has error state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Error state has some kind of fallback value, but it is bad quality.
        /// </summary>
        /// <param name="status"></param>
        public static bool Error(this LineStatus status) 
            => status.Severity() == 2;

        /// <summary>
        /// Result has failed state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Failed state has no value.
        /// </summary>
        /// <param name="status"></param>
        public static bool Failed(this LineStatus status) 
            => status.Severity() == 3;

        /// <summary>
        /// Write flags as string into <paramref name="sb"/>.
        /// 
        /// Appends always "Resolve", "Culture", "Plurality", "Placeholder" and "Formulate" flags.
        /// Appends "Custom0", "Custom1" flags if they are other than NoResult.
        /// 
        /// If <paramref name="status"/> has "Custom0" or "Custom1" values, then uses enum names from <paramref name="flagsType"/> if is provided.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sb"></param>
        /// <param name="flagsType">(optional)</param>
        public static void AppendFlags(this LineStatus status, StringBuilder sb, Type flagsType = default)
        {
            // No Result
            if (status == LineStatus.NoResult) { sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status)); return; }

            sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.ResolveMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.CultureMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.PluralityMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.PlaceholderMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.FormatMask));

            LineStatus custom0 = status & LineStatus.Custom0Mask;
            if (custom0 != LineStatus.NoResult)
            {
                sb.Append("|");
                if (flagsType == null)
                {
                    LineStatus custom0Severity = status & LineStatus.Custom0SeverityMask;
                    sb.Append(Enum.GetName(typeof(LineStatus), custom0Severity));
                    ulong code = ((ulong)custom0 >> Shift.Custom0) & 0x3f;
                    sb.Append(code.ToString("X2"));
                }
                else
                {
                    sb.Append(Enum.GetName(flagsType, custom0));
                }
            }

            LineStatus custom1 = status & LineStatus.Custom0Mask;
            if (custom1 != LineStatus.NoResult)
            {
                sb.Append("|");
                if (flagsType == null)
                {
                    LineStatus custom1Severity = status & LineStatus.Custom1SeverityMask;
                    sb.Append(Enum.GetName(typeof(LineStatus), custom1Severity));
                    ulong code = ((ulong)custom1 >> Shift.Custom1) & 0x3f;
                    sb.Append(code.ToString("X2"));
                }
                else
                {
                    sb.Append(Enum.GetName(flagsType, custom1));
                }
            }
        }

        /// <summary>
        /// Write flags as string into <paramref name="tw"/>.
        /// 
        /// Appends always "Resolve", "Culture", "Plurality", "Placeholder" and "Formulate" flags.
        /// Appends "Custom0", "Custom1" flags if they are other than NoResult.
        /// 
        /// If <paramref name="status"/> has "Custom0" or "Custom1" values, then uses enum names from <paramref name="flagsType"/> if is provided.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="tw"></param>
        /// <param name="flagsType">(optional)</param>
        public static void WriteFlags(this LineStatus status, TextWriter tw, Type flagsType = default)
        {
            // No Result
            if (status == LineStatus.NoResult) { tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status)); return; }

            tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.ResolveMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.CultureMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.PluralityMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.PlaceholderMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LineStatus), status & LineStatus.FormatMask));

            LineStatus custom0 = status & LineStatus.Custom0Mask;
            if (custom0 != LineStatus.NoResult)
            {
                tw.Write("|");
                if (flagsType == null)
                {
                    LineStatus custom0Severity = status & LineStatus.Custom0SeverityMask;
                    tw.Write(Enum.GetName(typeof(LineStatus), custom0Severity));
                    ulong code = ((ulong)custom0 >> Shift.Custom0) & 0x3f;
                    tw.Write(code.ToString("X2"));
                }
                else
                {
                    tw.Write(Enum.GetName(flagsType, custom0));
                }
            }

            LineStatus custom1 = status & LineStatus.Custom0Mask;
            if (custom1 != LineStatus.NoResult)
            {
                tw.Write("|");
                if (flagsType == null)
                {
                    LineStatus custom1Severity = status & LineStatus.Custom1SeverityMask;
                    tw.Write(Enum.GetName(typeof(LineStatus), custom1Severity));
                    ulong code = ((ulong)custom1 >> Shift.Custom1) & 0x3f;
                    tw.Write(code.ToString("X2"));
                }
                else
                {
                    tw.Write(Enum.GetName(flagsType, custom1));
                }
            }
        }

        /// <summary>
        /// Print info as string.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="flagsType">(optional) <see cref="Type"/> with names of custom flags</param>
        /// <returns></returns>
        public static String Print(this LineStatus status, Type flagsType = default)
        {
            StringBuilder sb = new StringBuilder();
            status.AppendFlags(sb, flagsType);
            return sb.ToString();
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s culture status code, if <paramref name="cultureStatus"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="cultureStatus"></param>
        public static void UpCulture(this ref LineStatus status, LineStatus cultureStatus)
        {
            if ((status & LineStatus.CultureMask) < (cultureStatus & LineStatus.CultureMask)) status = (status & ~LineStatus.CultureMask) | cultureStatus;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="resolveStatus"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="resolveStatus"></param>
        public static void UpResolve(this ref LineStatus status, LineStatus resolveStatus)
        {
            if ((status & LineStatus.ResolveMask) < (resolveStatus & LineStatus.ResolveMask)) status = (status & ~LineStatus.ResolveMask) | resolveStatus;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="pluralityStatus"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pluralityStatus"></param>
        public static void UpPlurality(this ref LineStatus status, LineStatus pluralityStatus)
        {
            if ((status & LineStatus.PluralityMask) < (pluralityStatus & LineStatus.PluralityMask)) status = (status & ~LineStatus.PluralityMask) | pluralityStatus;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="argumentStatus"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="argumentStatus"></param>
        public static void UpPlaceholder(this ref LineStatus status, LineStatus argumentStatus)
        {
            if ((status & LineStatus.PluralityMask) < (argumentStatus & LineStatus.PluralityMask)) status = (status & ~LineStatus.PluralityMask) | argumentStatus;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="formatStatus"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="formatStatus"></param>
        public static void UpFormat(this ref LineStatus status, LineStatus formatStatus)
        {
            if ((status & LineStatus.FormatMask) < (formatStatus & LineStatus.FormatMask)) status = (status & ~LineStatus.FormatMask) | formatStatus;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="custom0Status"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="custom0Status"></param>
        public static void UpCustom0(this ref LineStatus status, LineStatus custom0Status)
        {
            if ((status & LineStatus.Custom0Mask) < (custom0Status & LineStatus.Custom0Mask)) status = (status & ~LineStatus.Custom0Mask) | custom0Status;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code, if <paramref name="custom1Status"/> is higher than the one in <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="custom1Status"></param>
        public static void UpCustom1(this ref LineStatus status, LineStatus custom1Status)
        {
            if ((status & LineStatus.Custom1Mask) < (custom1Status & LineStatus.Custom1Mask)) status = (status & ~LineStatus.Custom1Mask) | custom1Status;
        }

        /// <summary>
        /// Upgrade <paramref name="status"/>'s status code of each category that has higher status value.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="other"></param>
        public static void Up(this ref LineStatus status, LineStatus other)
        {
            UpCulture(ref status, other);
            UpResolve(ref status, other);
            UpPlurality(ref status, other);
            UpPlaceholder(ref status, other);
            UpFormat(ref status, other);
            UpCustom0(ref status, other);
            UpCustom1(ref status, other);
        }

        /// <summary>
        /// Create upgraded version of <paramref name="status"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="other"></param>
        /// <return></return>
        public static LineStatus Up_(this LineStatus status, LineStatus other)
        {
            UpCulture(ref status, other);
            UpResolve(ref status, other);
            UpPlurality(ref status, other);
            UpPlaceholder(ref status, other);
            UpFormat(ref status, other);
            UpCustom0(ref status, other);
            UpCustom1(ref status, other);
            return status;
        }

    }

}
