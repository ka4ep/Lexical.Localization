// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           4.7.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Status code for <see cref="LocalizationString"/>.
    /// </summary>
    [Flags]
    public enum LocalizationStatus : UInt64
    {
        /// <summary>Request has not been resolved</summary>
        NoResult = 0xFFFFFFFFFFFFFFFFUL,

        //// Resolve - Step that searches formulation string for IAssetKey from IAsset or Inlines.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveOk = 0x00UL << Shift.Resolve,
        /// <summary>Resolved string from asset</summary>
        ResolveOkFromAsset = 0x01UL << Shift.Resolve,
        /// <summary>Resolved string from inlines</summary>
        ResolveOkFromInline = 0x02UL << Shift.Resolve,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveWarning = 0x40UL << Shift.Resolve,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveError = 0x80UL << Shift.Resolve,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ResolveFailed = 0xC0UL << Shift.Resolve,
        /// <summary>Asset was found, but could not resolve key from it or inline</summary>
        ResolveFailedNotFound = 0xC1UL << Shift.Resolve,
        /// <summary>Asset was not found and could not resolve key inline</summary>
        ResolveFailedNoAsset = 0xC2UL << Shift.Resolve,
        /// <summary>Mask for severity</summary>
        ResolveSeverityMask = 0xC0UL << Shift.Resolve,
        /// <summary>Mask for resolve status</summary>
        ResolveMask = 0xffUL << Shift.Resolve,

        //// Culture - Step that matches active culture, culture policy to strings available in asset
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureOk = 0x00UL << Shift.Culture,
        /// <summary>Key contained no culture, and culture policy contained no culture</summary>
        CultureOkNotUsed = 0x01UL << Shift.Culture,
        /// <summary>Key requested a culture, and it was found</summary>
        CultureOkMatched = 0x02UL << Shift.Culture,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureWarning = 0x40UL << Shift.Culture,
        /// <summary>Key requested a specific culture, but a fallback culture was used</summary>
        CultureWarningFallback = 0x41UL << Shift.Culture,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureError = 0x80UL << Shift.Culture,
        /// <summary>Key requested a specific culture, but it was not found in asset, nor fallback culture</summary>
        CultureErrorNotMatched = 0x81UL << Shift.Culture,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        CultureFailed = 0xC0UL << Shift.Culture,
        /// <summary>Mask for severity</summary>
        CultureSeverityMask = 0xC00UL << Shift.Culture,
        /// <summary>Mask for culture status</summary>
        CultureMask = 0xffUL << Shift.Culture,

        //// Plurality - Step that matches plurality cases
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityOk = 0x00UL << Shift.Plurality,
        /// <summary>No plurality categories was used</summary>
        PluralityOkNotUsed = 0x01UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument formulation(s), and the plurality cases were found in the asset/inlines and used</summary>
        PluralityOkMatched = 0x02UL << Shift.Plurality,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityWarning = 0x40UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument formulation(s), and plurality cases were found for some arguments, but not all</summary>
        PluralityWarningPartiallyMatched = 0x41UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument formulation(s), but the plurality cases were not found in the asset/inlines, fallbacked to default string</summary>
        PluralityWarningNotMatched = 0x42UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument formulation, but the provided value was not a number</summary>
        PluralityWarningNotNumber = 0x43UL << Shift.Plurality,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityError = 0x80UL << Shift.Plurality,
        /// <summary>String contained "plurality/ordinal/range" argument formulation(s), but the plurality rules were not found in the key or in the asset</summary>
        PluralityErrorRulesNotFound = 0x81UL << Shift.Plurality,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        PluralityFailed = 0xC0UL << Shift.Plurality,
        /// <summary>Mask for severity</summary>
        PluralitySeverityMask = 0xC00UL << Shift.Plurality,
        /// <summary>Mask for plurality status</summary>
        PluralityMask = 0xffUL << Shift.Plurality,

        //// Argument - Step that converts argument objects into strings
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ArgumentOk = 0x00UL << Shift.Argument,
        /// <summary>One of the argument was null, or custom formatter resulted into null.</summary>
        ArgumentOkNull = 0x02UL << Shift.Argument,
        /// <summary>Request asked for the formulation string, without applying arguments to it.</summary>
        ArgumentOkNotApplied = 0x03L << Shift.Formulation,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ArgumentWarning = 0x40UL << Shift.Argument,
        /// <summary>No IFormattable implementation or ICustomFormatter was found, or they returned null, ToString was applied</summary>
        ArgumentWarningToStringUsed = 0x41UL << Shift.Argument,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ArgumentError = 0x80UL << Shift.Argument,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        ArgumentFailed = 0xC0UL << Shift.Argument,
        /// <summary>Mask for severity</summary>
        ArgumentSeverityMask = 0xC0UL << Shift.Argument,
        /// <summary>Mask for argument status</summary>
        ArgumentMask = 0xffUL << Shift.Argument,

        //// Formulation - Step that parses formulation string, inserts arguments, and builds into formulated string.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormulationOk = 0x00UL << Shift.Formulation,
        /// <summary>There were no arguments to formulate</summary>
        FormulationOkNoArguments = 0x02UL << Shift.Formulation,
        /// <summary>Request asked for the formulation string, without applying arguments to it.</summary>
        FormulationOkNotApplied = 0x03UL << Shift.Formulation,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormulationWarning = 0x40UL << Shift.Formulation,
        /// <summary>Formulation string contained arguments, but too many arguments were provided</summary>
        FormulationWarningTooManyArguments = 0x41UL << Shift.Formulation,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormulationError = 0x80UL << Shift.Formulation,
        /// <summary>Formulation string contained arguments, but arguments were not provided</summary>
        FormulationErrorNoArguments = 0x82UL << Shift.Formulation,
        /// <summary>Formulation string contained arguments, but too few arguments were provided. Using null values for missing arguments.</summary>
        FormulationErrorTooFewArguments = 0x82UL << Shift.Formulation,
        /// <summary>Formulation string was malformed. Returning the malformed string as value.</summary>
        FormulationErrorMalformed = 0x8fUL << Shift.Formulation,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        FormulationFailed = 0xC0UL << Shift.Formulation,
        /// <summary>Mask for severity</summary>
        FormulationSeverityMask = 0xC0UL << Shift.Formulation,
        /// <summary>Mask for argument status</summary>
        FormulationMask = 0xffUL << Shift.Formulation,

        //// Custom0 - ILocalizationFormatter implementation specific status flags. Can be used for any purpose.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Ok = 0x00UL << Shift.Custom0,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Warning = 0x40UL << Shift.Custom0,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Error = 0x80UL << Shift.Custom0,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom0Failed = 0xC0UL << Shift.Custom0,
        /// <summary>Mask for severity</summary>
        Custom0SeverityMask = 0xC0UL << Shift.Custom0,
        /// <summary>Mask for argument status</summary>
        Custom0Mask = 0xffUL << Shift.Custom0,

        //// Custom0 - ILocalizationFormatter implementation specific status flags. Can be used for any purpose.
        /// <summary>Ok for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Ok = 0x00UL << Shift.Custom1,
        /// <summary>Warning for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Warning = 0x40UL << Shift.Custom1,
        /// <summary>Error for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Error = 0x80UL << Shift.Custom1,
        /// <summary>Failed for unspecified reason. This flag used when comparing against SeverityMask</summary>
        Custom1Failed = 0xC0UL << Shift.Custom1,
        /// <summary>Mask for severity</summary>
        Custom1SeverityMask = 0xC0UL << Shift.Custom1,
        /// <summary>Mask for argument status</summary>
        Custom1Mask = 0xffUL << Shift.Custom1,
    }

    /// <summary>
    /// Localization string.
    /// </summary>
    public struct LocalizationString
    {
        /// <summary>
        /// Status code
        /// </summary>
        public LocalizationStatus Status;

        /// <summary>
        /// Key that was requested to be formatted.
        /// </summary>
        public IAssetKey Key;

        /// <summary>
        /// Resolved string.
        /// 
        /// Depending on what was requested, either formulation string as is, or formatted string with arguments applied to the formulation.
        /// 
        /// Null, if value was not available.
        /// </summary>
        public String Value;

        /// <summary>
        /// The object that formatted the result, or null if unavailable.
        /// </summary>
        public ILocalizationFormatter Formatter;

        /// <summary>
        /// Severity for the step that resolves <see cref="IAssetKey"/> into formulation string.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public int ResolveSeverity => (int)((ulong)Status >> Shift.ResolveSeverity) & 3;

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
        public int CultureSeverity => (int)((ulong)Status >> Shift.CultureSeverity) & 3;

        /// <summary>
        /// Severity for the step applies plurality.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public int PluralitySeverity => (int)((ulong)Status >> Shift.PluralitySeverity) & 3;

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
        public int ArgumentSeverity => (int)((ulong)Status >> Shift.ArgumentSeverity) & 3;

        /// <summary>
        /// Severity for the step that parses formulation string and applies arguments.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public int FormulationSeverity => (int)((ulong)Status >> Shift.FormulationSeverity) & 3;

        /// <summary>
        /// Severity for <see cref="ILocalizationFormatter"/> implementation specific "Custom0" status.
        /// 
        /// "Custom0" is a status code that is specific to the <see cref="ILocalizationFormatter"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public int Custom0Severity => (int)((ulong)Status >> Shift.Custom0Severity) & 3;

        /// <summary>
        /// Severity for <see cref="ILocalizationFormatter"/> implementation specific "Custom1" status.
        /// 
        /// "Custom1" is a status code that is specific to the <see cref="ILocalizationFormatter"/> implementation.
        /// 
        /// <list type="table">
        /// <item>0 OK, value</item>
        /// <item>1 Warning, but produced a value</item>
        /// <item>2 Error, but produced some kind of fallback value</item>
        /// <item>3 Failed, no value</item>
        /// </list>
        /// </summary>
        public int Custom1Severity => (int)((ulong)Status >> Shift.Custom1Severity) & 3;

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
        public int Severity
        {
            get
            {
                int a = ResolveSeverity, b = CultureSeverity, c = PluralitySeverity, d = ArgumentSeverity, e = FormulationSeverity, g = Custom0Severity, h = Custom1Severity;
                int result = a;
                if (b > result) result = b;
                if (c > result) result = c;
                if (d > result) result = d;
                if (e > result) result = e;
                // if (f > result) result = f;
                if (g > result) result = g;
                if (h > result) result = h;
                return result;
            }
        }

        /// <summary>
        /// Tests if there is no result.
        /// </summary>
        public bool NoResult => Status == LocalizationStatus.NoResult;

        /// <summary>
        /// Result has ok state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Produced ok value.
        /// </summary>
        public bool Ok => Severity == 0;

        /// <summary>
        /// Result has warning state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Warning state has a value, but there was something occured during the resolve that may need attention.
        /// </summary>
        public bool Warning => Severity == 1;

        /// <summary>
        /// Result has error state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Error state has some kind of fallback value, but it is bad quality.
        /// </summary>
        public bool Error => Severity == 2;

        /// <summary>
        /// Result has failed state out of four severity states (Ok, Warning, Error, Failed).
        /// 
        /// Failed state has no value.
        /// </summary>
        public bool Failed => Severity == 3;

        /// <summary>
        /// Create new localization string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="status"></param>
        /// <param name="formatter"></param>
        public LocalizationString(IAssetKey key, string value, LocalizationStatus status, ILocalizationFormatter formatter)
        {
            Key = key;
            Value = value;
            Status = status;
            Formatter = formatter;
        }

        /// <summary>
        /// Write flags as string into <paramref name="sb"/>.
        /// 
        /// Appends always "Resolve", "Culture", "Plurality", "Argument" and "Formulate" flags.
        /// Appends "Custom0", "Custom1" flags if they are other than NoResult.
        /// 
        /// If <paramref name="status"/> has "Custom0" or "Custom1" values, then uses enum names from <paramref name="flagsType"/> if is provided.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sb"></param>
        /// <param name="flagsType">(optional)</param>
        public static void AppendFlags(LocalizationStatus status, StringBuilder sb, Type flagsType = default)
        {
            // No Result
            if (status == LocalizationStatus.NoResult) { sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status)); return; }

            sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.ResolveMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.CultureMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.PluralityMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.ArgumentMask));
            sb.Append("|");
            sb.Append(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.FormulationMask));

            LocalizationStatus custom0 = status & LocalizationStatus.Custom0Mask;
            if (custom0 != LocalizationStatus.NoResult)
            {
                sb.Append("|");
                if (flagsType == null)
                {
                    LocalizationStatus custom0Severity = status & LocalizationStatus.Custom0SeverityMask;
                    sb.Append(Enum.GetName(typeof(LocalizationStatus), custom0Severity));
                    ulong code = ((ulong)custom0 >> Shift.Custom0) & 0x3f;
                    sb.Append(code.ToString("X2"));
                }
                else
                {
                    sb.Append(Enum.GetName(flagsType, custom0));
                }
            }

            LocalizationStatus custom1 = status & LocalizationStatus.Custom0Mask;
            if (custom1 != LocalizationStatus.NoResult)
            {
                sb.Append("|");
                if (flagsType == null)
                {
                    LocalizationStatus custom1Severity = status & LocalizationStatus.Custom1SeverityMask;
                    sb.Append(Enum.GetName(typeof(LocalizationStatus), custom1Severity));
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
        /// Appends always "Resolve", "Culture", "Plurality", "Argument" and "Formulate" flags.
        /// Appends "Custom0", "Custom1" flags if they are other than NoResult.
        /// 
        /// If <paramref name="status"/> has "Custom0" or "Custom1" values, then uses enum names from <paramref name="flagsType"/> if is provided.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="tw"></param>
        /// <param name="flagsType">(optional)</param>
        public static void WriteFlags(LocalizationStatus status, TextWriter tw, Type flagsType = default)
        {
            // No Result
            if (status == LocalizationStatus.NoResult) { tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status)); return; }

            tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.ResolveMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.CultureMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.PluralityMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.ArgumentMask));
            tw.Write("|");
            tw.Write(Enum.GetName(flagsType ?? typeof(LocalizationStatus), status & LocalizationStatus.FormulationMask));

            LocalizationStatus custom0 = status & LocalizationStatus.Custom0Mask;
            if (custom0 != LocalizationStatus.NoResult)
            {
                tw.Write("|");
                if (flagsType == null)
                {
                    LocalizationStatus custom0Severity = status & LocalizationStatus.Custom0SeverityMask;
                    tw.Write(Enum.GetName(typeof(LocalizationStatus), custom0Severity));
                    ulong code = ((ulong)custom0 >> Shift.Custom0) & 0x3f;
                    tw.Write(code.ToString("X2"));
                }
                else
                {
                    tw.Write(Enum.GetName(flagsType, custom0));
                }
            }

            LocalizationStatus custom1 = status & LocalizationStatus.Custom0Mask;
            if (custom1 != LocalizationStatus.NoResult)
            {
                tw.Write("|");
                if (flagsType == null)
                {
                    LocalizationStatus custom1Severity = status & LocalizationStatus.Custom1SeverityMask;
                    tw.Write(Enum.GetName(typeof(LocalizationStatus), custom1Severity));
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
        /// Print information about the formatting result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Append key
            ParameterParser.Instance.PrintKey(Key, sb);

            // Append result
            if (Value != null)
            {
                sb.Append(" \"");
                sb.Append(Value);
                sb.Append("\"");
            }

            // Append status
            sb.Append(" (");
            AppendFlags(Status, sb);
            sb.Append(")");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Contains bit-shifts for each status category.
    /// </summary>
    internal static class Shift
    {
        // bit-shifts for categories
        internal const int Resolve = 0;
        internal const int Culture = 8;
        internal const int Plurality = 16;
        internal const int Argument = 24;
        internal const int Formulation = 32;
        internal const int Reserved = 40;  // Reserved for future use
        internal const int Custom0 = 48;    // ILocalizationFormatter implemtation can use for any custom purpose.
        internal const int Custom1 = 56;    // ILocalizationFormatter implemtation can use for any custom purpose.

        // bit shifts for severity bits (2bits) of each category.
        internal const int ResolveSeverity = Resolve + 6;
        internal const int CultureSeverity = Culture + 6;
        internal const int PluralitySeverity = Plurality + 6;
        internal const int ArgumentSeverity = Argument + 6;
        internal const int FormulationSeverity = Formulation + 6;
        internal const int ReservedSeverity = Reserved + 6;  // Reserved for future use
        internal const int Custom0Severity = Custom0 + 6;
        internal const int Custom1Severity = Custom1 + 6;

    }

    /// <summary>
    /// Extension methods for <see cref="LocalizationString"/>
    /// </summary>
    public static partial class LocalizationFormatResultExtensions
    {
    }
}
