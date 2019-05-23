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
        public static StringResolver Instance => instance;

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IFormatString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        public IFormatString ResolveFormatString(ILine key)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="LineString"/> with format arguments applied.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LineString ResolveString(ILine key)
        {
            // Extract parameters from line
            LineFeatures features = new LineFeatures();

            try
            {

                features.ScanFeatures(key);

                //// Resolve
                // Get default line
                // If there is no explicitly assigned culture in the key, try cultures from culture policy
                bool rootCultureTried = false;
                IEnumerable<CultureInfo> cultures = null;
                CultureInfo selected_culture = null;
                ILine line = null;
                // No explicit culture, use culture policy
                if (features.Culture == null && features.CulturePolicy != null && (cultures = features.CulturePolicy?.Cultures) != null)
                {
                    foreach (CultureInfo culture in cultures) // <- TODO FIX causes heap allocation because enumerator is acquired from abstraction (IEnumerable)
                    {
                        rootCultureTried |= culture.Name == "";
                        // Append culture
                        ILine key_with_culture = key.Culture(culture);

                        // Try asset
                        for (int i = 0; i < features.Assets.Count; i++)
                        {
                            IAsset asset = features.Assets[i];
                            if ((line = asset.GetString(key_with_culture)) != null)
                            {
                                selected_culture = culture;
                                features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                features.Status.UpCulture(LineStatus.CultureOkMatchedCulturePolicy);
                                break;
                            }
                        }

                        // Try inlines
                        if (line == null && features.Inlines.Count > 0)
                            for (int i = 0; i < features.Inlines.Count; i++)
                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                {
                                    selected_culture = culture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedCulturePolicy);
                                    break;
                                }

                        if (line != null) break;
                    }
                }
                // Use root culture
                // Try key as is
                if (!rootCultureTried && line == null && features.Culture != null)
                {
                    // Try asset
                    for (int i = 0; i < features.Assets.Count; i++)
                    {
                        IAsset asset = features.Assets[i];
                        if ((line = asset.GetString(key)) != null)
                        {
                            selected_culture = features.Culture ?? RootCulture;
                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset); 
                            features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
                            break;
                        }
                    }

                    // Try inlines
                    if (line == null && features.Inlines.Count > 0)
                        for (int i = 0; i < features.Inlines.Count; i++)
                            if (features.Inlines[i].TryGetValue(key, out line))
                            {
                                selected_culture = features.Culture ?? RootCulture;
                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                features.Status.UpCulture(LineStatus.CultureOkMatchedCulture);
                                break;
                            }
                }
                // Use explicit culture and use the key as line
                if (line == null && (features.Value != null || features.ValueText != null))
                {
                    selected_culture = features.Culture ?? RootCulture;
                    line = key;
                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                    features.Status.UpCulture(LineStatus.CultureOkNotUsed);
                }

                if (line == null)
                {
                    features.Status.UpResolve(LineStatus.ResolveFailedNoResult);
                }

                // New line was detected, scan for more features
                if (line != null)
                {
                    features.ScanFeatures(line);
                }

                // Get value
                IFormatString value = features.EffectiveValue;

                // Evaluate expressions in placeholders into strings
                StructList12<IPluralNumber> placeholder_values = new StructList12<IPluralNumber>();
                PlaceholderExpressionEvaluator placeholder_evaluator = new PlaceholderExpressionEvaluator();
                placeholder_evaluator.Args = features.FormatArgs;
                placeholder_evaluator.FunctionEvaluationCtx.Culture = selected_culture;
                if (features.FormatProviders.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = features.FormatProviders[0]; else if (features.FormatProviders.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.FormatProvider = new FormatProviderComposition(features.FormatProviders.ToArray());
                if (features.Functions.Count == 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = features.Functions[0]; else if (features.Functions.Count > 1) placeholder_evaluator.FunctionEvaluationCtx.Functions = new Functions(features.Functions);
                for (int i = 0; i < value.Placeholders.Length; i++)
                {
                    IPlaceholder ph = value.Placeholders[i];
                    object ph_value = placeholder_evaluator.Evaluate(ph.Expression);
                    placeholder_values.Add(ph_value == null ? DecimalNumber.Empty : new DecimalNumber.Text(ph_value?.ToString(), selected_culture));
                }

                // Plural Rules
                if (features.PluralRules != null)
                {
                    // Create permutations
                    PluralCasePermutations permutations = new PluralCasePermutations(line);
                    for (int i = 0; i < value.Placeholders.Length; i++)
                    {
                        // Get placeholder
                        IPlaceholder ph = value.Placeholders[i];
                        // No plural category in this placeholder
                        if (ph.PluralCategory == null) continue;
                        // Get evaluated numner
                        IPluralNumber number = placeholder_values[i];
                        // Query possible cases for the plural rules
                        PluralRuleInfo query = new PluralRuleInfo(null, ph.PluralCategory, selected_culture.Name, null, -1);
                        IPluralRule[] cases = features.PluralRules.Evaluate(query, number);
                        if (cases == null) continue;
                        permutations.AddPlaceholder(ph, cases);
                    }

                    // Find first value that matches permutations
                    for (int i = 0; i < permutations.Count; i++)
                    {
                        ILine key_with_plurality = permutations[i];

                        // Search from asset
                        // Search from inlines
                        // Search from key
                    }
                }

                return new LineString(key, null, features.Status);
            } catch (Exception e)
            {
                // Unexpected error 
                LineStatus status = LineStatus.FailedUnknownReason;
                status.Up(features.Status);
                LineString result = new LineString(key, null, status);
                if (features.Loggers.Count>0)
                {
                    for (int i=0; i<features.Loggers.Count; i++)
                    {
                        features.Loggers[i].OnError(e);
                        features.Loggers[i].OnNext(result);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Log errors, if loggers are available
        /// </summary>
        /// <param name="e"></param>
        /// <param name="features"></param>
        /// <returns>true</returns>
        static bool LogError(Exception e, ref LineFeatures features)
        {
            if (features.Loggers.Count > 0)
            {
                for (int i = 0; i < features.Loggers.Count; i++)
                {
                    features.Loggers[i].OnError(e);
                }
            }
            return true;
        }

        /*
                public LineString ResolveString(ILine key)
                {
                    // Plurality key when there is only one numeric argument. e.g. "Key:N:One"
                    ILine pluralityKey = null;
                    // Pluarlity key for all argument permutations (whether argument is provided or not)
                    IEnumerable<ILine> pluralityKeys = null;
                    if (format_args != null)
                    {
                        for (int argumentIndex = 0; argumentIndex < format_args.Length; argumentIndex++)
                        {
                            object o = format_args[argumentIndex];
                            if (o == null) continue;
                            string pluralityKind = GetPluralityKind(o);
                            if (pluralityKind == null) continue;
                            if (pluralityKey == null)
                            {
                                pluralityKey = key.N(argumentIndex, pluralityKind);
                            }
                            else
                            {
                                pluralityKey = null;
                                pluralityKeys = CreatePluralityKeyPermutations(key: key, maxArgumentCount: Plurality_.MAX_NUMERIC_ARGUMENTS_TO_PERMUTATE, args: format_args); // 2^5 = 32 keys
                                break;
                            }
                        }
                    }

                    // If there is no explicitly assigned culture in the key, try cultures from culture policy
                    bool rootCultureTried = false;
                    string explicitCulture = key.GetCultureName();
                    IEnumerable<CultureInfo> cultures = null;
                    if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        foreach (CultureInfo culture in cultures)
                        {
                            bool rootCulture = culture.Name == "";
                            rootCultureTried |= rootCulture;
                            // Append culture
                            ILine pluralityKey_with_culture = rootCulture ? pluralityKey : pluralityKey?.Culture(culture);
                            // Try inlines with plurality key
                            if (languageString == null && inlines != null && pluralityKey_with_culture != null) inlines.TryGetValue(pluralityKey_with_culture, out languageString);
                            // Try inlines with plurality key permutations
                            if (languageString == null && inlines != null && pluralityKeys != null)
                            {
                                foreach (ILine _pluralityKey in pluralityKeys)
                                    if (inlines.TryGetValue(_pluralityKey.Culture(culture), out languageString) && languageString != null) break;
                            }
                            // Append culture
                            ILine key_with_culture = rootCulture ? key : key.Culture(culture);
                            // Try inlines with fallback key
                            if (languageString == null && inlines != null) inlines.TryGetValue(key_with_culture, out languageString);
                            // Try asset with plurality key
                            if (languageString == null && pluralityKey_with_culture != null) languageString = pluralityKey_with_culture.TryGetString();
                            // Try asset with plurality key permutations
                            if (languageString == null && pluralityKeys != null)
                            {
                                foreach (ILine _pluralityKey in pluralityKeys)
                                {
                                    languageString = _pluralityKey.TryGetString();
                                    if (languageString != null) break;
                                }
                            }
                            // Try asset with fallback key
                            if (languageString == null) languageString = key_with_culture.TryGetString();
                            // Formulate language string
                            if (languageString != null && format_args != null) return Format(key, culture, languageString, format_args);
                            // Return format without arguments applied
                            if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                        }
                    }

                    // Try key as is
                    if (!rootCultureTried)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        // Try inlines with plurality key
                        if (languageString == null && inlines != null && pluralityKey != null) inlines.TryGetValue(pluralityKey, out languageString);
                        // Try inlines with plurality key permutations
                        if (languageString == null && inlines != null && pluralityKeys != null)
                        {
                            foreach (ILine _pluralityKey in pluralityKeys)
                                if (inlines.TryGetValue(_pluralityKey, out languageString) && languageString != null) break;
                        }
                        // Try inlines with fallback key
                        if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                        // Try asset with plurality key
                        if (languageString == null && pluralityKey != null) languageString = pluralityKey.TryGetString();
                        // Try asset with plurality key permutations
                        if (languageString == null && pluralityKeys != null)
                        {
                            foreach (ILine _pluralityKey in pluralityKeys)
                            {
                                languageString = _pluralityKey.TryGetString();
                                if (languageString != null) break;
                            }
                        }
                        // Try asset with fallback key
                        if (languageString == null) languageString = key.TryGetString();
                        // Formulate language string
                        if (languageString != null && format_args != null) return Format(key, rootCulture, languageString, format_args);
                        // Return format without applying arguments
                        if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                    }

                    return new LineString(key, null, LineStatus.NoResult);
                }

                static CultureInfo rootCulture = CultureInfo.GetCultureInfo("");

                /// <summary>
                /// Apply <paramref name="args"/> into <paramref name="formatString"/>.
                /// </summary>
                /// <param name="key"></param>
                /// <param name="culture"></param>
                /// <param name="formatString"></param>
                /// <param name="args"></param>
                /// <returns></returns>
                static LineString Format(ILine key, CultureInfo culture, IFormatString formatString, object[] args)
                {
                    // Convert to strings
                    string[] arg_strings = new string[formatString.Placeholders.Length];
                    for(int i=0; i< formatString.Placeholders.Length; i++)
                    {
                        IPlaceholder argumentFormat = formatString.Placeholders[i];
                        int argIndex = argumentFormat.ArgumentIndex;
                        if (args!=null && argIndex >= 0 && argIndex < args.Length)
                        {
                            arg_strings[i] = Format(args[argIndex], argumentFormat.Format, culture);
                        }
                        else
                        {
                            arg_strings[i] = "";
                        }
                    }

                    // Count characters
                    int c = 0;
                    foreach(var part in formatString.Parts)
                    {
                        if (part.Kind == FormatStringPartKind.Text) c += part.Length;
                        else if (part.Kind == FormatStringPartKind.Placeholder && part is IPlaceholder arg) c += arg_strings[arg.ArgumentsIndex].Length;
                    }

                    // Put together string
                    char[] chars = new char[c];
                    int ix = 0;
                    foreach (var part in formatString.Parts)
                    {
                        if (part.Kind == FormatStringPartKind.Text)
                        {
                            formatString.Text.CopyTo(part.Index, chars, ix, part.Length);
                            ix += part.Length;
                        }
                        else if (part.Kind == FormatStringPartKind.Placeholder && part is IPlaceholder arg)
                        {
                            string str = arg_strings[arg.ArgumentsIndex];
                            if (str != null)
                            {
                                str.CopyTo(0, chars, ix, str.Length);
                                ix += str.Length;
                            }
                        }
                    }


                    return new LineString(key, new String(chars), 0);
                }

                static string Format(object o, string format, IFormatProvider culture)
                {
                    if (o == null) return "";
                    if (o is IFormattable formattable) return formattable.ToString(format, culture);
                    if (culture.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter_)
                        return customFormatter_.Format(format, o, culture);
                    return culture == null ? String.Format("{0:" + format + "}", o) : String.Format(culture, "{0:" + format + "}", o);
                }

                /// <summary>
                /// Create plurality key permutations. 
                /// 
                /// If argument count exceeds <paramref name="maxArgumentCount"/> then returns single arguments only:
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   :N2:(Zero/One/Plural)
                ///   :N3:(Zero/One/Plural)
                /// 
                /// Result for two arguments:
                /// 
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   
                /// Result for three arguments:
                /// 
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural):N1:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural):N2:(Zero/One/Plural)
                ///   :N1:(Zero/One/Plural)
                ///   :N2:(Zero/One/Plural)
                /// 
                /// Number of elements: n = numbericArguments ^ 2 - 1
                /// </summary>
                /// <param name="key"></param>
                /// <param name="maxArgumentCount"></param>
                /// <param name="args">arguments, only numberic arguments are used</param>
                /// <returns>all permutation keys or null</returns>
                private static IEnumerable<ILine> CreatePluralityKeyPermutations(ILine key, int maxArgumentCount, object[] args)
                {
                    if (maxArgumentCount <= 0) return null;
                    // Gather info: (argumentIndex, pluralityKind)
                    (int, string)[] argInfos = new (int, string)[32];
                    int argumentCount = 0;
                    for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
                    {
                        object o = args[argumentIndex];
                        if (o == null) continue;
                        string pluralityKind = GetPluralityKind(o);
                        if (pluralityKind == null) continue;
                        argInfos[argumentCount] = (argumentIndex, pluralityKind);
                        argumentCount++;
                        if (argumentCount >= 32) return null;
                    }
                    if (argumentCount == 0) return null;

                    // Return single numeric argument keys only
                    if (argumentCount > maxArgumentCount) return _CreatePluralityKeysSingleArgumentOnly(key, argInfos, argumentCount);

                    // Return all permutations for numeric arguments
                    return _CreatePluralityKeyPermutations(key, argInfos, argumentCount);
                }

                private static IEnumerable<ILine> _CreatePluralityKeyPermutations(ILine key, (int, string)[] argInfos, int argumentCount)
                {
                    int allBits = (int)(1 << argumentCount) - 1;
                    for (int bits = allBits; bits > 0; bits--)
                    {
                        ILine pluralityKey = key;
                        for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                        {
                            if ((bits & (1 << argumentIndex)) == 0) continue;
                            pluralityKey = pluralityKey.N(argumentIndex, argInfos[argumentIndex].Item2);
                        }
                        yield return pluralityKey;
                    }
                }

                private static IEnumerable<ILine> _CreatePluralityKeysSingleArgumentOnly(ILine key, (int, string)[] argInfos, int argumentCount)
                {
                    for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                    {
                        yield return key.N(argumentIndex, argInfos[argumentIndex].Item2);
                    }
                }

                /// <summary>
                /// Resolve the format string. 
                /// 
                /// Uses the following algorithm:
                ///   1. Either explicitly assigned culture or <see cref="ICulturePolicy"/> from <see cref="ILineExtensions.FindCulturePolicy(ILine)"/>.
                ///   2. Try key as is.
                ///   
                ///      a. Search inlines with culture
                ///      b. Search asset with culture
                /// </summary>
                /// <param name="key"></param>
                /// <returns>format string (without formulating it) or null</returns>
                public LineString ResolveString(ILine key)
                {
                    // If there is no explicitly assigned culture in the key, try cultures from culture policy
                    string explicitCulture = key.GetCultureName();
                    IEnumerable<CultureInfo> cultures = null;
                    bool rootCultureTried = false;
                    if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        foreach (CultureInfo culture in cultures)
                        {
                            bool rootCulture = culture.Name == "";
                            rootCultureTried |= rootCulture;
                            // 
                            ILine culture_key = rootCulture ? key : key.Culture(culture);
                            // Try inlines
                            if (languageString == null && inlines != null) inlines.TryGetValue(culture_key, out languageString);
                            // Try key
                            if (languageString == null) languageString = culture_key.TryGetString();
                            // Return
                            if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                        }
                    }

                    if (!rootCultureTried)
                    {
                        IFormatString languageString = null;
                        // Get inlines
                        IDictionary<ILine, IFormatString> inlines = key.FindInlines();
                        // Try inlines with key
                        if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                        // Try asset with key
                        if (languageString == null) languageString = key.TryGetString();
                        // Return
                        if (languageString != null) return new LineString(key, languageString.Text, 0UL);
                    }

                    return new LineString(key, null, LineStatus.NoResult);
                }

                */

        static CultureInfo RootCulture = CultureInfo.GetCultureInfo("");
    }

}
