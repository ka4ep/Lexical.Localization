// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using Lexical.Localization.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// The default localization formatter implementation.
    /// 
    /// There are three sources of strings: asset, inlines and default value.
    /// 
    /// 
    /// Culture Policy: 
    /// 
    /// The requesting <see cref="ILine"/> may contain a request for a specific culture.
    /// 
    /// 
    /// </summary>
    public class StringResolver : IStringResolver
    {
        private static StringResolver instance = new StringResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static StringResolver Default => instance;

        /// <summary>
        /// Resolvers
        /// </summary>
        public readonly IResolver Resolvers;

        /// <summary>
        /// The resolve sequence that determines in which priority
        /// lines are resolved.
        /// </summary>
        public readonly ResolveSource[] ResolveSequence;

        /// <summary>
        /// Maximum number of plurality arguments in formatted strings.
        /// For example "{ordinal:0} {1:cardinal}-{2:cardinal}"
        /// </summary>
        public readonly int MaxPluralArguments = 3;

        /// <summary>
        /// Enumerable info resolver
        /// </summary>
        public readonly IEnumInfoResolver EnumResolver;

        /// <summary>
        /// Create string resolver 
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolveSequence"></param>
        /// <param name="maxPluralArguments">maximum plural arguments. The higher the number, the more permutation overhead can occur</param>
        /// <param name="enumResolver">(optional) enumeration information resolver. Determines the search key.</param>
        public StringResolver(IResolver resolvers = default, ResolveSource[] resolveSequence = default, int maxPluralArguments = 3, IEnumInfoResolver enumResolver = default)
        {
            this.Resolvers = resolvers ?? Lexical.Localization.Resolver.Resolvers.Default;
            this.ResolveSequence = resolveSequence ?? ResolverSequence.Default;
            this.MaxPluralArguments = maxPluralArguments;
            this.EnumResolver = enumResolver ?? EnumInfoResolver.Default;
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        public IString ResolveFormatString(ILine key)
        {
            // Extract parameters from line
            LineFeatures features = new LineFeatures { Resolvers = Resolvers };

            // Scan features
            try
            {
                features.ScanFeatures(key);
            }
            catch (Exception e)
            {
                features.Log(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                return new StatusString(null, features.Status);
            }

            // Resolve key to line
            CultureInfo culture = features.Culture;
            ILine line = ResolveKeyToLine(key, ref features, ref culture);

            // No line or value
            if (line == null || !features.HasValue)
            {
                features.Status.UpResolve(LineStatus.ResolveFailedNoValue);
                LineString str = new LineString(key, (Exception)null, features.Status);
                features.Log(str);
                return new StatusString(null, features.Status);
            }

            // Parse value
            IString value = features.EffectiveString;
            features.Status.Up(value.Status);

            // Value has error
            if (value.Parts == null || value.Status.Failed())
            {
                LineString str = new LineString(key, (Exception)null, features.Status);
                features.Log(str);
                return new StatusString(null, features.Status);
            }

            // Plural Rules
            if (value.HasPluralRules())
            {
                if (features.PluralRules != null)
                {
                    // Evaluate expressions in placeholders into strings
                    StructList12<string> placeholder_values = new StructList12<string>();
                    CultureInfo culture_for_format = features.Culture;
                    if (culture_for_format == null && features.CulturePolicy != null) { CultureInfo[] cultures = features.CulturePolicy.Cultures; if (cultures != null && cultures.Length > 0) culture_for_format = cultures[0]; }
                    if (culture_for_format == null) culture_for_format = CultureInfo.InvariantCulture;
                    EvaluatePlaceholderValues(key, line, null, value.Placeholders, ref features, ref placeholder_values, culture_for_format);

                    // Create permutation configuration
                    PluralCasePermutations permutations = new PluralCasePermutations(line);
                    for (int i = 0; i < value.Placeholders.Length; i++)
                    {
                        // Get placeholder
                        IPlaceholder placeholder = value.Placeholders[i];
                        // No plural category in this placeholder
                        if (placeholder.PluralCategory == null) continue;
                        // Placeholder value after evaluation
                        string ph_value = placeholder_values[i];
                        // Placeholder evaluated value
                        IPluralNumber placeholderValue = ph_value == null ? DecimalNumber.Empty : new DecimalNumber.Text(ph_value?.ToString(), culture);
                        // Add placeholder to permutation configuration
                        permutations.AddPlaceholder(placeholder, placeholderValue, features.PluralRules, culture?.Name ?? "");
                    }

                    // Find first value that matches permutations
                    features.CulturePolicy = null;
                    features.String = null;
                    features.StringText = null;
                    for (int i = 0; i < permutations.Count - 1; i++)
                    {
                        // Create key with plurality cases
                        ILine key_with_plurality = permutations[i];
                        // Search line with the key
                        ILine line_for_plurality_arguments = ResolveKeyToLine(key_with_plurality, ref features, ref culture);
                        // Got no match
                        if (line_for_plurality_arguments == null) continue;
                        // Parse value
                        IString value_for_plurality = line_for_plurality_arguments.GetString(Resolvers);
                        // Add status from parsing the value
                        features.Status.Up(value_for_plurality.Status);
                        // Value has error
                        if (value_for_plurality.Parts == null || value_for_plurality.Status.Failed())
                        {
                            LineString str = new LineString(key, (Exception)null, features.Status);
                            features.Log(str);
                            return new StatusString(null, features.Status);
                        }
                        // Return with match
                        features.Status.UpPlurality(LineStatus.PluralityOkMatched);
                        // Update status codes
                        features.Status.Up(value_for_plurality.Status);
                        // Return values
                        value = value_for_plurality;
                        line = line_for_plurality_arguments;
                        break;
                    }
                }
                else
                {
                    // Plural rules were not found
                    features.Status.Up(LineStatus.PluralityErrorRulesNotFound);
                }
            }
            else
            {
                // Plurality feature was not used.
                features.Status.UpPlurality(LineStatus.PluralityOkNotUsed);
            }

            return value;
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="LineString"/> with format arguments applied.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineString ResolveString(ILine key)
        {
            // Extract parameters from line
            LineFeatures features = new LineFeatures { Resolvers = Resolvers };

            // Scan features
            try
            {
                features.ScanFeatures(key);
            }
            catch (Exception e)
            {
                features.Log(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                return new LineString(key, e, features.Status);
            }

            try
            {
                // Resolve key to line
                CultureInfo culture = features.Culture;
                ILine line = ResolveKeyToLine(key, ref features, ref culture);

                // No line or value
                if (line == null || !features.HasValue)
                {
                    features.Status.UpResolve(LineStatus.ResolveFailedNoValue);
                    LineString str = new LineString(key, (Exception)null, features.Status);
                    features.Log(str);
                    return str;
                }

                // Parse value
                IString value = features.EffectiveString;
                features.Status.Up(value.Status);

                // Value has error
                if (value.Parts == null || value.Status.Failed())
                {
                    LineString str = new LineString(key, (Exception)null, features.Status);
                    features.Log(str);
                    return str;
                }

                // Evaluate expressions in placeholders into strings
                StructList12<string> placeholder_values = new StructList12<string>();
                CultureInfo culture_for_format = features.Culture;
                if (culture_for_format == null && features.CulturePolicy != null) { CultureInfo[] cultures = features.CulturePolicy.Cultures; if (cultures != null && cultures.Length > 0) culture_for_format = cultures[0]; }
                if (culture_for_format == null) culture_for_format = CultureInfo.InvariantCulture;
                EvaluatePlaceholderValues(key, line, null, value.Placeholders, ref features, ref placeholder_values, culture_for_format);

                // Plural Rules
                if (value.HasPluralRules())
                {
                    if (features.PluralRules != null)
                    {
                        // Create permutation configuration
                        PluralCasePermutations permutations = new PluralCasePermutations(line);
                        for (int i = 0; i < value.Placeholders.Length; i++)
                        {
                            // Get placeholder
                            IPlaceholder placeholder = value.Placeholders[i];
                            // No plural category in this placeholder
                            if (placeholder.PluralCategory == null) continue;
                            // Placeholder value after evaluation
                            string ph_value = placeholder_values[i];
                            // Placeholder evaluated value
                            IPluralNumber placeholderValue = ph_value == null ? DecimalNumber.Empty : new DecimalNumber.Text(ph_value?.ToString(), culture);
                            // Add placeholder to permutation configuration
                            permutations.AddPlaceholder(placeholder, placeholderValue, features.PluralRules, culture?.Name ?? "");
                        }

                        if (permutations.ArgumentCount <= MaxPluralArguments)
                        {
                            // Find first value that matches permutations
                            features.CulturePolicy = null;
                            features.String = null;
                            features.StringText = null;
                            for (int i = 0; i < permutations.Count - 1; i++)
                            {
                                // Create key with plurality cases
                                ILine key_with_plurality = permutations[i];
                                // Search line with the key
                                ILine line_for_plurality_arguments = ResolveKeyToLine(key_with_plurality, ref features, ref culture);
                                // Got no match
                                if (line_for_plurality_arguments == null) continue;
                                // Scan value
                                try
                                {
                                    features.ScanValueFeature(line_for_plurality_arguments);
                                }
                                catch (Exception e)
                                {
                                    features.Log(e);
                                    features.Status.Up(LineStatus.FailedUnknownReason);
                                    return new LineString(key, e, features.Status);
                                }
                                // Parse value
                                IString value_for_plurality = features.EffectiveString;
                                // Add status from parsing the value
                                features.Status.Up(value_for_plurality.Status);
                                // Value has error
                                if (value_for_plurality.Parts == null || value_for_plurality.Status.Failed())
                                {
                                    LineString str = new LineString(key, (Exception)null, features.Status);
                                    features.Log(str);
                                    return str;
                                }
                                // Return with match
                                features.Status.UpPlurality(LineStatus.PluralityOkMatched);
                                // Evaluate placeholders again
                                if (!EqualPlaceholders(value, value_for_plurality)) { placeholder_values.Clear(); EvaluatePlaceholderValues(key, line, line_for_plurality_arguments, value_for_plurality.Placeholders, ref features, ref placeholder_values, culture); }
                                // Update status codes
                                features.Status.Up(value_for_plurality.Status);
                                // Return values
                                value = value_for_plurality;
                                line = line_for_plurality_arguments;
                                break;
                            }
                        }
                        else
                        { 
                            features.Status.UpPlaceholder(LineStatus.PluralityErrorMaxPluralArgumentsExceeded);
                        }
                    }
                    else
                    {
                        // Plural rules were not found
                        features.Status.Up(LineStatus.PluralityErrorRulesNotFound);
                    }
                }
                else
                {
                    // Plurality feature was not used.
                    features.Status.UpPlurality(LineStatus.PluralityOkNotUsed);
                }

                // Put string together
                string text = null;
                
                if (value == null || value.Parts == null)
                {
                    text = null;
                }

                // Only one part
                else if (value.Parts.Length == 1)
                {
                    if (value.Parts[0].Kind == StringPartKind.Text)
                    {
                        text = value.Parts[0].Text;
                        features.Status.UpStringFormat(LineStatus.StringFormatOkString);
                    }
                    else if (value.Parts[0].Kind == StringPartKind.Placeholder)
                    {
                        text = placeholder_values[0];
                        features.Status.UpStringFormat(LineStatus.StringFormatOkString);
                    }
                }
                // Compile multiple parts
                else {
                    // Calculate length
                    int length = 0;
                    for (int i = 0; i < value.Parts.Length; i++)
                    {
                        IStringPart part = value.Parts[i];
                        string partText = part.Kind switch { StringPartKind.Text => part.Text, StringPartKind.Placeholder => placeholder_values[((IPlaceholder)part).PlaceholderIndex], _ => null };
                        if (partText != null) length += partText.Length;
                    }

                    // Copy characters
                    char[] arr = new char[length];
                    int ix = 0;
                    for (int i = 0; i < value.Parts.Length; i++)
                    {
                        IStringPart part = value.Parts[i];
                        string str = part.Kind switch { StringPartKind.Text => part.Text, StringPartKind.Placeholder => placeholder_values[((IPlaceholder)part).PlaceholderIndex], _ => null };
                        if (str != null) { str.CopyTo(0, arr, ix, str.Length); ix += str.Length; }
                    }

                    // String
                    text = new string(arr);
                    features.Status.UpStringFormat(LineStatus.StringFormatOkString);
                }

                // Create result 
                LineString result = new LineString(key, text, features.Status);

                // Log
                features.Log(result);

                // Return
                return result;
            } catch (Exception e)
            {
                // Capture unexpected error
                features.Log(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                LineString lineString = new LineString(key, e, features.Status);
                features.Log(lineString);
                return lineString;
            }
        }        

        /// <summary>
        /// Resolve keys into a line. Searches from asset, inlines and key itself.
        /// 
        /// If resolving causes an exception, it is caught, logged and status in <paramref name="features"/> is updated.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="features"></param>
        /// <param name="culture">The culture that matched</param>
        /// <returns>matching line or null</returns>
        ILine ResolveKeyToLine(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                ILine line = null;

                // Explicit culture request
                if (culture != null)
                {
                    // 0 - main preference, 1+ - fallback preferences
                    int preferenceIndex = 0;
                    for (CultureInfo ci = culture, prev = null; ci != null && ci != prev; prev = ci, ci = ci.Parent)
                    {
                        ILine key_with_culture = preferenceIndex == 0 ? key : key.Prune(NoCultureQualifier.Default).Culture(ci);

                        // Should invariant culture be tested here??
                        foreach (var stage in ResolveSequence)
                            switch (stage)
                            {
                                // Try asset
                                case ResolveSource.Asset:
                                    for (int i = 0; i < features.Assets.Count; i++)
                                    {
                                        try
                                        {
                                            IAsset asset = features.Assets[i];
                                            if ((line = asset.GetLine(key_with_culture)) != null)
                                            {
                                                // Up status with source info
                                                features.Status.UpResolve(LineStatus.ResolveOkFromAsset);

                                                // Up status with culture info
                                                LineStatus cultureStatus = preferenceIndex == 0
                                                    // Request matched with first preference
                                                    ? (ci.Name == "" ? LineStatus.CultureOkRequestMatchedInvariantCulture :
                                                       ci.IsNeutralCulture ? LineStatus.CultureOkRequestMatchedLanguage : LineStatus.CultureOkRequestMatchedLanguageAndRegion)
                                                    // Request matched with a fallback preference
                                                    : (ci.Name == "" ? Localization.LineStatus.CultureErrorRequestMatchedInvariantCulture :
                                                       ci.IsNeutralCulture ? LineStatus.CultureWarningRequestMatchedLanguage : Localization.LineStatus.CultureWarningRequestMatchedLanguageAndRegion);
                                                features.Status.UpCulture(cultureStatus);

                                                features.ScanFeatures(line);
                                                return line;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                            features.Log(e);
                                        }
                                    }
                                    break;

                                // Try inlines
                                case ResolveSource.Inline:
                                    for (int i = 0; i < features.Inlines.Count; i++)
                                    {
                                        try
                                        {
                                            if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                            {
                                                // Up status with source info
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);

                                                // Up status with culture info
                                                LineStatus cultureStatus = preferenceIndex == 0
                                                    // Request matched with first preference
                                                    ? (ci.Name == "" ? LineStatus.CultureOkRequestMatchedInvariantCulture :
                                                       ci.IsNeutralCulture ? LineStatus.CultureOkRequestMatchedLanguage : LineStatus.CultureOkRequestMatchedLanguageAndRegion)
                                                    // Request matched with a fallback preference
                                                    : (ci.Name == "" ? Localization.LineStatus.CultureErrorRequestMatchedInvariantCulture :
                                                       ci.IsNeutralCulture ? LineStatus.CultureWarningRequestMatchedLanguage : Localization.LineStatus.CultureWarningRequestMatchedLanguageAndRegion);
                                                features.Status.UpCulture(cultureStatus);

                                                features.ScanFeatures(line);
                                                return line;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                            features.Log(e);
                                        }
                                    }
                                    break;

                                case ResolveSource.Line:
                                    // Key has explicit culture and value, use the value
                                    if (preferenceIndex == 0 && (features.String != null || features.StringText != null))
                                    {

                                        // Up status with source info
                                        features.Status.UpResolve(LineStatus.CultureWarningRequestMatchedInvariantCulture);

                                        // Up status with culture info
                                        LineStatus cultureStatus =
                                            culture == null || culture.Name == "" ? LineStatus.CultureOkRequestMatchedInvariantCulture :
                                            LineStatus.CultureWarningRequestMatchedInvariantCulture;
                                        features.Status.UpCulture(cultureStatus);

                                        if (culture == null) culture = CultureInfo.InvariantCulture;

                                        return key;
                                    }
                                    break;
                            }

                        preferenceIndex++;
                    }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(LineStatus.CultureFailedRequestNoMatch);
                }

                // Try with cultures from the culture policy
                CultureInfo[] cultures = features.CulturePolicy?.Cultures;
                if (culture == null && cultures != null)
                {
                    try { 
                        for (int preferenceIndex = 0; preferenceIndex<cultures.Length; preferenceIndex++)
                        {
                            CultureInfo ci = cultures[preferenceIndex];

                            // Has already been tested above? Skip
                            if (ci == culture) continue;

                            ILine key_with_culture = key.Culture(ci);
                            foreach (var stage in ResolveSequence)
                                switch (stage)
                                {
                                    // Try asset
                                    case ResolveSource.Asset:
                                        for (int i = 0; i < features.Assets.Count; i++)
                                        {
                                            try
                                            {
                                                IAsset asset = features.Assets[i];
                                                if ((line = asset.GetLine(key_with_culture)) != null)
                                                {
                                                    culture = ci;

                                                    // Up status with source info
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);

                                                    // Up status with culture info
                                                    LineStatus cultureStatus = preferenceIndex == 0
                                                        // Request matched with first preference
                                                        ? (ci.Name == "" ? LineStatus.CultureOkCulturePolicyMatchedInvariantCulture :
                                                           ci.IsNeutralCulture ? LineStatus.CultureOkCulturePolicyMatchedLanguage : LineStatus.CultureOkCulturePolicyMatchedLanguageAndRegion)
                                                        // Request matched with a fallback preference
                                                        : (ci.Name == "" ? Localization.LineStatus.CultureErrorCulturePolicyMatchedInvariantCulture :
                                                           ci.IsNeutralCulture ? LineStatus.CultureWarningCulturePolicyMatchedLanguage : Localization.LineStatus.CultureWarningCulturePolicyMatchedLanguageAndRegion);
                                                    features.Status.UpCulture(cultureStatus);

                                                    features.ScanFeatures(line);
                                                    return line;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                                features.Log(e);
                                            }
                                        }
                                        break;

                                    // Try inlines
                                    case ResolveSource.Inline:
                                        for (int i = 0; i < features.Inlines.Count; i++)
                                        {
                                            try
                                            {
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    culture = ci;
                                                    // Up status with source info
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromInline);

                                                    // Up status with culture info
                                                    LineStatus cultureStatus = preferenceIndex == 0
                                                        // Request matched with first preference
                                                        ? (ci.Name == "" ? LineStatus.CultureOkCulturePolicyMatchedInvariantCulture :
                                                           ci.IsNeutralCulture ? LineStatus.CultureOkCulturePolicyMatchedLanguage : LineStatus.CultureOkCulturePolicyMatchedLanguageAndRegion)
                                                        // Request matched with a fallback preference
                                                        : (ci.Name == "" ? Localization.LineStatus.CultureErrorCulturePolicyMatchedInvariantCulture :
                                                           ci.IsNeutralCulture ? LineStatus.CultureWarningCulturePolicyMatchedLanguage : Localization.LineStatus.CultureWarningCulturePolicyMatchedLanguageAndRegion);
                                                    features.Status.UpCulture(cultureStatus);

                                                    features.ScanFeatures(line);
                                                    return line;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                                features.Log(e);
                                            }
                                        }
                                        break;

                                    case ResolveSource.Line:
                                        if ((features.String != null || features.StringText != null) && ci.Equals(features.Culture))
                                        {
                                            culture = ci;
                                            // Up status with source info
                                            features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                            // Up status with culture info
                                            LineStatus cultureStatus = preferenceIndex == 0
                                                // Request matched with first preference
                                                ? (ci.Name == "" ? LineStatus.CultureOkCulturePolicyMatchedInvariantCulture :
                                                   ci.IsNeutralCulture ? LineStatus.CultureOkCulturePolicyMatchedLanguage : LineStatus.CultureOkCulturePolicyMatchedLanguageAndRegion)
                                                // Request matched with a fallback preference
                                                : (ci.Name == "" ? Localization.LineStatus.CultureErrorCulturePolicyMatchedInvariantCulture :
                                                   ci.IsNeutralCulture ? LineStatus.CultureWarningCulturePolicyMatchedLanguage : Localization.LineStatus.CultureWarningCulturePolicyMatchedLanguageAndRegion);
                                            features.Status.UpCulture(cultureStatus);

                                            return key;
                                        }
                                        break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        features.Status.UpResolve(LineStatus.CultureErrorCulturePolicyException);
                        features.Log(e);
                    }
                }

                // No request culture - try this _after_ culture policy
                if (culture == null)
                {
                    foreach (var stage in ResolveSequence)
                        switch (stage)
                        {
                            // Try asset
                            case ResolveSource.Asset:
                                for (int i = 0; i < features.Assets.Count; i++)
                                {
                                    try
                                    {
                                        IAsset asset = features.Assets[i];
                                        if ((line = asset.GetLine(key)) != null)
                                        {
                                            // Up status with source info
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);

                                            line.TryGetCultureInfo(out culture);
                                            if (culture == null) culture = CultureInfo.InvariantCulture;

                                            // Up status with culture info
                                            LineStatus cultureStatus = 
                                                culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                            features.Status.UpCulture(cultureStatus);

                                            features.ScanFeatures(line);

                                            return line;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                        features.Log(e);
                                    }
                                }
                                break;

                            // Try inlines
                            case ResolveSource.Inline:
                                for (int i = 0; i < features.Inlines.Count; i++)
                                {
                                    try
                                    {
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            // Up status with source info
                                            features.Status.UpResolve(LineStatus.ResolveOkFromInline);

                                            line.TryGetCultureInfo(out culture);
                                            if (culture == null) culture = CultureInfo.InvariantCulture;

                                            // Up status with culture info
                                            LineStatus cultureStatus =
                                                culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                            features.Status.UpCulture(cultureStatus);

                                            features.ScanFeatures(line);
                                            return line;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                        features.Log(e);
                                    }
                                }
                                break;

                            case ResolveSource.Line:
                                // Key has explicit culture and value, use the value
                                if (features.String != null || features.StringText != null)
                                {
                                    // Up status with source info
                                    features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                    if (culture == null) culture = CultureInfo.InvariantCulture;
                                    LineStatus cultureStatus = LineStatus.CultureOkMatchedInvariantCulture;
                                    features.Status.UpCulture(cultureStatus);

                                    return key;
                                }
                                break;
                        }
                }

                // No matching value was found for the requested key and the explicit culture in the key.
                features.Status.UpCulture(cultures != null ? LineStatus.CultureFailedCulturePolicyNoMatch : LineStatus.CultureFailedRequestNoMatch);
                // No match
                features.Status.UpResolve(LineStatus.ResolveErrorNoMatch);
                culture = null;
                return null;
            }
            catch (Exception e)
            {
                // Uncaptured error
                features.Status.UpResolve(LineStatus.ResolveError);
                features.Log(e);
                culture = null;
                return null;
            }
        }

        /// <summary>
        /// Compares whether two strings have equal placeholders.
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        static bool EqualPlaceholders(IString str1, IString str2)
        {
            if (str1 == null && str2 == null) return true;
            if (str1 == null || str2 == null) return false;
            if (str1.Placeholders.Length != str2.Placeholders.Length) return false;
            int c = str1.Placeholders.Length;
            for (int i=0; i<c; i++)
            {
                IPlaceholder ph1 = str1.Placeholders[i], ph2 = str2.Placeholders[i];
                if (!PlaceholderExpressionEquals.Equals(ph1.Expression, ph2.Expression)) return false;
            }
            return true;
        }

        /// <summary>
        /// Evaluate placeholders into string values.
        /// </summary>
        /// <param name="line">original requesting line</param>
        /// <param name="resolvedLine">(optional Line that was matched from IAsset or inlines</param>
        /// <param name="pluralLine">(optional) Line that was matched from IAsset or inlines for plural value</param>
        /// <param name="placeholders"></param>
        /// <param name="features">contextual data</param>
        /// <param name="placeholder_values">collection where strings are placed, one for each placeholder</param>
        /// <param name="culture">the culture in which to evaluate</param>
        void EvaluatePlaceholderValues(ILine line, ILine resolvedLine, ILine pluralLine, IPlaceholder[] placeholders, ref LineFeatures features, ref StructList12<string> placeholder_values, CultureInfo culture)
        {
            PlaceholderExpressionEvaluator placeholder_evaluator = new PlaceholderExpressionEvaluator();
            placeholder_evaluator.Args = features.ValueArgs;
            placeholder_evaluator.FunctionEvaluationCtx.Culture = culture;
            placeholder_evaluator.FunctionEvaluationCtx.Line = line;
            placeholder_evaluator.FunctionEvaluationCtx.ResolvedLine = resolvedLine;
            placeholder_evaluator.FunctionEvaluationCtx.PluralLine = pluralLine;
            placeholder_evaluator.FunctionEvaluationCtx.StringResolver = this;
            placeholder_evaluator.FunctionEvaluationCtx.EnumResolver = EnumResolver;
            if (features.FormatProviders.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = features.FormatProviders[0]; else if (features.FormatProviders.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = new FormatProviderComposition(features.FormatProviders.ToArray());
            if (features.Functions.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = features.Functions[0]; else if (features.Functions.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = new FunctionsMap(features.Functions);
            for (int i = 0; i < placeholders.Length; i++)
            {
                try
                {
                    // Get placeholder
                    IPlaceholder ph = placeholders[i];
                    // Evaluate value
                    string ph_value = placeholder_evaluator.toString( placeholder_evaluator.Evaluate(ph.Expression) );
                    // Add to array
                    placeholder_values.Add(ph_value);
                    // Update code
                    features.Status.UpPlaceholder(placeholder_evaluator.Status);
                }
                catch (Exception e)
                {
                    // Log exceptions
                    features.Log(e);
                    // Mark error
                    features.Status.UpPlaceholder(LineStatus.PlaceholderErrorExpressionEvaluation);
                    // Put empty value
                    placeholder_values.Add(null);
                }
            }
        }

    }

}
