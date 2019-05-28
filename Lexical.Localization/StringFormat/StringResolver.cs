// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Plurality;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// The default localization formatter implementation.
    /// </summary>
    public class StringResolver : IStringResolver
    {
        private static StringResolver instance = new StringResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static StringResolver Default => instance;

        /// <summary>
        /// Group of resolvers
        /// </summary>
        public readonly ResolverSet Resolvers;

        /// <summary>
        /// Create string resolver 
        /// </summary>
        public StringResolver()
        {
            this.Resolvers = new ResolverSet(null,
                Lexical.Localization.StringFormat.StringFormatResolver.Default,
                Lexical.Localization.StringFormat.FunctionsResolver.Default,
                Lexical.Localization.StringFormat.FormatProviderResolver.Default,
                Lexical.Localization.Plurality.PluralRulesResolver.Default);
            this.ResolveSequence = new ResolveSource[] { ResolveSource.Asset, ResolveSource.Inlines, ResolveSource.Key };
        }

        /// <summary>
        /// Create string resolver 
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolveSequence"></param>
        public StringResolver(ResolverSet resolvers, ResolveSource[] resolveSequence = default)
        {
            this.Resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
            this.ResolveSequence = resolveSequence ?? new ResolveSource[] { ResolveSource.Asset, ResolveSource.Inlines, ResolveSource.Key };
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IFormatString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        public IFormatString ResolveFormatString(ILine key)
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
                features.Status.Up(LineStatus.FailedUnknownReason);
                return new StatusFormatString(null, features.Status);
            }

            // Resolve key to line
            CultureInfo culture = features.Culture;
            ILine line = ResolveKeyToLine(key, ref features, ref culture);

            // No line or value
            if (line == null || !features.HasValue)
            {
                features.Status.UpResolve(LineStatus.ResolveFailedNoValue);
                LineString str = new LineString(key, null, features.Status);
                features.Log(str);
                return new StatusFormatString(null, features.Status);
            }

            // Parse value
            IFormatString value = features.EffectiveValue;
            features.Status.UpFormat(value.Status);

            // Value has error
            if (value.Parts == null || value.Status.Failed())
            {
                LineString str = new LineString(key, null, features.Status);
                features.Log(str);
                return new StatusFormatString(null, features.Status);
            }

            // Plural Rules
            if (value.HasPluralRules())
            {
                if (features.PluralRules != null)
                {
                    // Evaluate expressions in placeholders into strings
                    StructList12<string> placeholder_values = new StructList12<string>();
                    EvaluatePlaceholderValues(value.Placeholders, ref features, ref placeholder_values, culture);

                    // Create permutation configuration
                    PluralCasePermutations permutations = new PluralCasePermutations(line);
                    for (int i = 0; i < value.Placeholders.Length; i++)
                    {
                        // Get placeholder
                        IPlaceholder ph = value.Placeholders[i];
                        // No plural category in this placeholder
                        if (ph.PluralCategory == null) continue;
                        // Placeholder value after evaluation
                        string ph_value = placeholder_values[i];
                        // Placeholder as number
                        IPluralNumber number = ph_value == null ? DecimalNumber.Empty : new DecimalNumber.Text(ph_value?.ToString(), culture);
                        // Query possible cases for the plural rules
                        PluralRuleInfo query = new PluralRuleInfo(null, ph.PluralCategory, culture?.Name, null, -1);
                        IPluralRule[] cases = features.PluralRules.Evaluate(query, number);
                        if (cases == null) continue;
                        permutations.AddPlaceholder(ph, cases);
                    }

                    // Find first value that matches permutations
                    features.CulturePolicy = null;
                    features.Value = null;
                    features.ValueText = null;
                    for (int i = 0; i < permutations.Count - 1; i++)
                    {
                        // Create key with plurality cases
                        ILine key_with_plurality = permutations[i];
                        // Search line with the key
                        ILine line_for_plurality_arguments = ResolveKeyToLine(key_with_plurality, ref features, ref culture);
                        // Got no match
                        if (line_for_plurality_arguments == null) continue;
                        // Parse value
                        IFormatString value_for_plurality = line_for_plurality_arguments.GetValue(Resolvers.StringFormatResolver);
                        // Add status from parsing the value
                        features.Status.UpFormat(value_for_plurality.Status);
                        // Value has error
                        if (value_for_plurality.Parts == null || value_for_plurality.Status.Failed())
                        {
                            LineString str = new LineString(key, null, features.Status);
                            features.Log(str);
                            return new StatusFormatString(null, features.Status);
                        }
                        // Return with match
                        features.Status.UpPlurality(LineStatus.PluralityOkMatched);
                        // Update status codes
                        features.Status.UpFormat(value_for_plurality.Status);
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
                features.Status.Up(LineStatus.FailedUnknownReason);
                return new LineString(key, null, features.Status);
            }

            // Resolve key to line
            CultureInfo culture = features.Culture;
            ILine line = ResolveKeyToLine(key, ref features, ref culture);

            // No line or value
            if (line == null || !features.HasValue)
            {
                features.Status.UpResolve(LineStatus.ResolveFailedNoValue);
                LineString str = new LineString(key, null, features.Status);
                features.Log(str);
                return str;
            }

            // Parse value
            IFormatString value = features.EffectiveValue;
            features.Status.UpFormat(value.Status);

            // Value has error
            if (value.Parts==null||value.Status.Failed())
            {
                LineString str = new LineString(key, null, features.Status);
                features.Log(str);
                return str;
            }

            // Evaluate expressions in placeholders into strings
            StructList12<string> placeholder_values = new StructList12<string>();
            EvaluatePlaceholderValues(value.Placeholders, ref features, ref placeholder_values, culture);

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
                        IPlaceholder ph = value.Placeholders[i];
                        // No plural category in this placeholder
                        if (ph.PluralCategory == null) continue;
                        // Placeholder value after evaluation
                        string ph_value = placeholder_values[i];
                        // Placeholder as number
                        IPluralNumber number = ph_value == null ? DecimalNumber.Empty : new DecimalNumber.Text(ph_value?.ToString(), culture);
                        // Query possible cases for the plural rules
                        PluralRuleInfo query = new PluralRuleInfo(null, ph.PluralCategory, culture?.Name, null, -1);                        
                        IPluralRule[] cases = features.PluralRules.Evaluate(query, number);
                        if (cases == null) continue;
                        permutations.AddPlaceholder(ph, cases);
                    }

                    // Find first value that matches permutations
                    features.CulturePolicy = null;
                    features.Value = null;
                    features.ValueText = null;
                    for (int i = 0; i < permutations.Count - 1; i++)
                    {
                        // Create key with plurality cases
                        ILine key_with_plurality = permutations[i];
                        // Search line with the key
                        ILine line_for_plurality_arguments = ResolveKeyToLine(key_with_plurality, ref features, ref culture);
                        // Got no match
                        if (line_for_plurality_arguments == null) continue;
                        // Parse value
                        IFormatString value_for_plurality = line_for_plurality_arguments.GetValue(Resolvers.StringFormatResolver);
                        // Add status from parsing the value
                        features.Status.UpFormat(value_for_plurality.Status);
                        // Value has error
                        if (value_for_plurality.Parts == null || value_for_plurality.Status.Failed())
                        {
                            LineString str = new LineString(key, null, features.Status);
                            features.Log(str);
                            return str;
                        }
                        // Return with match
                        features.Status.UpPlurality(LineStatus.PluralityOkMatched);
                        // Evaluate placeholders again
                        if (!EqualPlaceholders(value, value_for_plurality)) { placeholder_values.Clear(); EvaluatePlaceholderValues(value_for_plurality.Placeholders, ref features, ref placeholder_values, culture); }
                        // Update status codes
                        features.Status.UpFormat(value_for_plurality.Status);
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
            } else {
                // Plurality feature was not used.
                features.Status.UpPlurality(LineStatus.PluralityOkNotUsed);
            }

            // Put string together
            string text = null;
            if (value != null && value.Parts != null)
            {
                // Only one part
                if (value.Parts.Length == 1)
                {
                    if (value.Parts[0].Kind == FormatStringPartKind.Text) text = value.Parts[0].Text;
                    else if (value.Parts[0].Kind == FormatStringPartKind.Placeholder) text = placeholder_values[0];
                } else
                // Compile parts
                {
                    // Calculate length
                    int length = 0;
                    for (int i = 0; i < value.Parts.Length; i++)
                    {
                        IFormatStringPart part = value.Parts[i];
                        length += part.Kind switch { FormatStringPartKind.Text => part.Length, FormatStringPartKind.Placeholder => placeholder_values[((IPlaceholder)part).PlaceholderIndex].Length, _ => 0 };
                    }

                    // Copy characters
                    char[] arr = new char[length];
                    int ix = 0;
                    for (int i = 0; i < value.Parts.Length; i++)
                    {
                        IFormatStringPart part = value.Parts[i];
                        string str = part.Kind switch { FormatStringPartKind.Text => part.Text, FormatStringPartKind.Placeholder => placeholder_values[((IPlaceholder)part).PlaceholderIndex], _ => null };
                        if (str != null) { str.CopyTo(0, arr, ix, str.Length); ix += str.Length; }
                    }

                    // String
                    text = new string(arr);
                }
                features.Status.UpFormat(LineStatus.FormatOkString);
            }

            // Create result 
            LineString result = new LineString(key, text, features.Status);

            // Log
            features.Log(result);

            // Return
            return result;
        }

        /// <summary>
        /// Resolve source
        /// </summary>
        public enum ResolveSource {
            /// <summary>Resolve from <see cref="ILineAsset"/></summary>
            Asset,
            /// <summary>Resolve from <see cref="ILineInlines"/></summary>
            Inlines,
            /// <summary>Resolve from the Value part of Key itself</summary>
            Key
        };

        /// <summary>
        /// Resolve sequence
        /// </summary>
        public readonly ResolveSource[] ResolveSequence;


        /// <summary>
        /// Resolve keys into a line. Searches from asset, inlines and key itself.
        /// 
        /// If resolving causes an exception, it is caught, logged and status in <paramref name="features"/> is updated.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="features"></param>
        /// <param name="culture">as input the culture to match, and as output the culture that matched</param>
        /// <returns>matching line or null</returns>
        ILine ResolveKeyToLine(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                ILine line = null;

                // Key has explicit culture
                if (culture != null)
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
                                        if ((line = asset.GetString(key)) != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
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
                            case ResolveSource.Inlines:
                                for (int i = 0; i < features.Inlines.Count; i++)
                                {
                                    try
                                    {
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
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

                            case ResolveSource.Key:
                                // Key has explicit culture and value, use the value
                                if (features.Value != null || features.ValueText != null)
                                {
                                    if (culture == null) culture = RootCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
                                    return key;
                                }
                                break;
                        }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(LineStatus.CultureErrorCultureNoMatch);
                }

                // Try with cultures from the culture policy
                CultureInfo[] cultures = features.CulturePolicy?.Cultures;
                if (cultures != null)
                {
                    try { 
                        foreach (CultureInfo c in cultures)
                        {
                            // Has already been tested above? Skip
                            if (c == culture) continue;
                            ILine key_with_culture = key.Culture(c);
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
                                                if ((line = asset.GetString(key_with_culture)) != null)
                                                {
                                                    culture = c;
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                                    features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
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
                                    case ResolveSource.Inlines:
                                        for (int i = 0; i < features.Inlines.Count; i++)
                                        {
                                            try
                                            {
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    culture = c;
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                    features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
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

                                    case ResolveSource.Key:
                                        if (features.Value != null || features.ValueText != null)
                                        {
                                            if (c.Name == "" && features.Culture == null)
                                            {
                                                if (culture == null) culture = c;
                                                features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                                features.Status.UpCulture(LineStatus.CultureWarningNoMatch);
                                                return key;
                                            }
                                            else if (c.Equals(features.Culture))
                                            {
                                                if (culture == null) culture = c;
                                                features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                                features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
                                                return key;
                                            }
                                        }
                                        break;
                                }
                        }

                        // No matching value was found for the requested key and the cultures that culture policy provided.
                        features.Status.UpCulture(LineStatus.CultureErrorCulturePolicyNoMatch);
                    }
                    catch (Exception e)
                    {
                        features.Status.UpResolve(LineStatus.CultureErrorCulturePolicyException);
                        features.Log(e);
                    }
                }

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

        static CultureInfo RootCulture = CultureInfo.GetCultureInfo("");

        /// <summary>
        /// Compares whether two strings have equal placeholders.
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        static bool EqualPlaceholders(IFormatString str1, IFormatString str2)
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
        /// <param name="placeholders"></param>
        /// <param name="features">contextual data</param>
        /// <param name="placeholder_values">collection where strings are placed, one for each placeholder</param>
        /// <param name="culture">the culture in which to evaluate</param>
        void EvaluatePlaceholderValues(IPlaceholder[] placeholders, ref LineFeatures features, ref StructList12<string> placeholder_values, CultureInfo culture)
        {
            PlaceholderExpressionEvaluator placeholder_evaluator = new PlaceholderExpressionEvaluator();
            placeholder_evaluator.Args = features.FormatArgs;
            placeholder_evaluator.FunctionEvaluationCtx.Culture = culture;
            if (features.FormatProviders.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = features.FormatProviders[0]; else if (features.FormatProviders.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = new FormatProviderComposition(features.FormatProviders.ToArray());
            if (features.Functions.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = features.Functions[0]; else if (features.Functions.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = new Functions(features.Functions);
            for (int i = 0; i < placeholders.Length; i++)
            {
                try
                {
                    // Get placeholder
                    IPlaceholder ph = placeholders[i];
                    // Evaluate value
                    object ph_value = placeholder_evaluator.Evaluate(ph.Expression);
                    // Add to array
                    placeholder_values.Add(ph_value?.ToString());
                }
                catch (Exception e)
                {
                    // Log exceptions
                    features.Log(e);
                    // Mark error
                    features.Status.UpPlaceholder(LineStatus.PlaceholderErrorExpressionEvaluationException);
                    // Put empty value
                    placeholder_values.Add(null);
                }
            }
        }

    }

}
