// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Asset;
using Lexical.Localization.Resolver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lexical.Localization.Binary
{
    /// <summary>
    /// Resolves keys to resources by applying contextual information such as culture.
    /// </summary>
    public class BinaryResolver : IBinaryResolver
    {
        private static BinaryResolver instance = new BinaryResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static BinaryResolver Default => instance;

        /// <summary>
        /// Resolvers
        /// </summary>
        public readonly IResolver Resolvers;

        /// <summary>
        /// Resolve sequence
        /// </summary>
        public readonly ResolveSource[] ResolveSequence;

        /// <summary>
        /// Create resource resolver 
        /// </summary>
        public BinaryResolver()
        {
            this.Resolvers = Lexical.Localization.Resolver.Resolvers.Default;
            this.ResolveSequence = ResolverSequence.Default;
        }

        /// <summary>
        /// Create resource resolver 
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolveSequence"></param>
        public BinaryResolver(IResolver resolvers, ResolveSource[] resolveSequence = default)
        {
            this.Resolvers = resolvers ?? throw new ArgumentNullException(nameof(resolvers));
            this.ResolveSequence = resolveSequence ?? ResolverSequence.Default;
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into bytes.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result status</returns>
        public LineBinaryBytes ResolveBytes(ILine key)
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
                features.LogResolveBytes(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                return new LineBinaryBytes(key, e, features.Status);
            }

            try
            {
                // Resolve key to line
                CultureInfo culture = features.Culture;

                // Resolve to bytes
                LineBinaryBytes result = ResolveKeyToBytes(key, ref features, ref culture);

                // Synchronize status codes to result
                result.Status.Up(features.Status);

                // Log
                features.LogResolveBytes(result);

                // Return
                return result;
            }
            catch (Exception e)
            {
                // Capture unexpected error
                features.LogResolveBytes(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                LineBinaryBytes lineString = new LineBinaryBytes(key, e, features.Status);
                features.LogResolveBytes(lineString);
                return lineString;
            }
        }

        /// <summary>
        /// Resolves key to bytes. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="features"></param>
        /// <param name="culture">The culture that matched</param>
        /// <returns>result</returns>
        LineBinaryBytes ResolveKeyToBytes(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                LineBinaryBytes failedResult = new LineBinaryBytes(key, LineStatus.NoResult);

                // Key has explicit culture
                if (culture != null)
                {
                    // 0 - main preference, 1+ - fallback preferences
                    int preferenceIndex = 0;
                    for (CultureInfo ci = culture, prev = null; ci != null && ci != prev; prev = ci, ci = ci.Parent)
                    {
                        ILine key_with_culture = preferenceIndex == 0 ? key : key.Prune(NoCultureQualifier.Default).Culture(ci);

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
                                            LineBinaryBytes result = asset.GetBytes(key_with_culture);
                                            if (result.Value != null)
                                            {
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
                                                return result;
                                            }
                                            else
                                            {
                                                if (result.Severity <= failedResult.Severity) failedResult = result;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                            features.LogResolveBytes(e);
                                        }
                                    }
                                    break;

                                // Try inlines
                                case ResolveSource.Inline:
                                    for (int i = 0; i < features.Inlines.Count; i++)
                                    {
                                        try
                                        {
                                            ILine line;
                                            LineBinaryBytes result;
                                            if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                            {
                                                result = line.GetBytes();
                                                if (result.Value != null)
                                                {
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

                                                    return result;
                                                }
                                                else
                                                {
                                                    if (result.Severity <= failedResult.Severity) failedResult = result;
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                            features.LogResolveBytes(e);
                                        }
                                    }
                                    break;

                                case ResolveSource.Line:
                                    // Key has explicit culture and resource, use the resource
                                    if (features.Resource != null)
                                    {
                                        if (culture == null) culture = CultureInfo.InvariantCulture;
                                        features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                        // Up status with culture info
                                        LineStatus cultureStatus =
                                            culture == null || culture.Name == "" ? LineStatus.CultureOkRequestMatchedInvariantCulture :
                                            LineStatus.CultureWarningRequestMatchedInvariantCulture;
                                        features.Status.UpCulture(cultureStatus);

                                        return new LineBinaryBytes(key, features.Resource, LineStatus.ResolveOkFromLine);
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
                if (cultures != null)
                {
                    try
                    {
                        for (int preferenceIndex = 0; preferenceIndex < cultures.Length; preferenceIndex++)
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
                                                LineBinaryBytes result = asset.GetBytes(key_with_culture);
                                                if (result.Value != null)
                                                {
                                                    culture = ci;
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

                                                    return result;
                                                }
                                                else
                                                {
                                                    if (result.Severity <= failedResult.Severity) failedResult = result;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                                features.LogResolveBytes(e);
                                            }
                                        }
                                        break;

                                    // Try inlines
                                    case ResolveSource.Inline:
                                        for (int i = 0; i < features.Inlines.Count; i++)
                                        {
                                            try
                                            {
                                                ILine line;
                                                LineBinaryBytes result;
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    result = line.GetBytes();
                                                    if (result.Value != null)
                                                    {
                                                        culture = ci;
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

                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        if (result.Severity <= failedResult.Severity) failedResult = result;
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                                features.LogResolveBytes(e);
                                            }
                                        }
                                        break;

                                    case ResolveSource.Line:
                                        // Key has explicit culture and resource, use the resource
                                        if (features.Resource != null && ci.Equals(features.Culture))
                                        {
                                            culture = ci;
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

                                            return new LineBinaryBytes(key_with_culture, features.Resource, LineStatus.ResolveOkFromLine);
                                        }
                                        break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        features.Status.UpResolve(LineStatus.CultureErrorCulturePolicyException);
                        features.LogResolveBytes(e);
                    }
                }

                // Key has no explicit culture - try this _after_ culture policy
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
                                        LineBinaryBytes result = asset.GetBytes(key);
                                        if (result.Value != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);

                                            // Up status with culture info
                                            if (culture == null) culture = CultureInfo.InvariantCulture;
                                            LineStatus cultureStatus =
                                                culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                            features.Status.UpCulture(cultureStatus);

                                            return result;
                                        }
                                        else
                                        {
                                            if (result.Severity <= failedResult.Severity) failedResult = result;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                        features.LogResolveBytes(e);
                                    }
                                }
                                break;

                            // Try inlines
                            case ResolveSource.Inline:
                                for (int i = 0; i < features.Inlines.Count; i++)
                                {
                                    try
                                    {
                                        ILine line;
                                        LineBinaryBytes result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetBytes();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);

                                                line.TryGetCultureInfo(out culture);
                                                if (culture == null) culture = CultureInfo.InvariantCulture;

                                                // Up status with culture info
                                                LineStatus cultureStatus =
                                                    culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                    culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                                features.Status.UpCulture(cultureStatus);

                                                return result;
                                            }
                                            else
                                            {
                                                if (result.Severity <= failedResult.Severity) failedResult = result;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                        features.LogResolveBytes(e);
                                    }
                                }
                                break;

                            case ResolveSource.Line:
                                // Key has explicit culture and value, use the value
                                if (features.Resource != null)
                                {
                                    if (culture == null) culture = CultureInfo.InvariantCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                    if (culture == null) culture = CultureInfo.InvariantCulture;
                                    LineStatus cultureStatus = LineStatus.CultureOkMatchedInvariantCulture;
                                    features.Status.UpCulture(cultureStatus);

                                    return new LineBinaryBytes(key, features.Resource, LineStatus.ResolveOkFromLine);
                                }
                                break;
                        }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(cultures != null ? LineStatus.CultureFailedCulturePolicyNoMatch : LineStatus.CultureFailedRequestNoMatch);
                }

                // No match
                features.Status.UpResolve(LineStatus.ResolveErrorNoMatch);
                culture = null;
                return failedResult;
            }
            catch (Exception e)
            {
                // Uncaptured error
                features.Status.UpResolve(LineStatus.ResolveError);
                features.LogResolveBytes(e);
                return new LineBinaryBytes(key, e, features.Status);
            }
        }

        /// <summary>
        /// Resolve <paramref name="key"/> into bytes.
        /// 
        /// Applies contextual information, such as culture, from the executing context.
        /// executing context.
        /// 
        /// Status codes:
        /// <list type="bullet">
        ///     <item><see cref="LineStatus.ResolveOkFromAsset"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromInline"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveOkFromLine"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineBinaryStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoBinaryResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result status</returns>
        public LineBinaryStream ResolveStream(ILine key)
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
                features.LogResolveStream(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                return new LineBinaryStream(key, e, features.Status);
            }

            try
            {
                // Resolve key to line
                CultureInfo culture = features.Culture;

                // Resolve to bytes
                LineBinaryStream result = ResolveKeyToStream(key, ref features, ref culture);

                // Synchronize status codes to result
                result.Status.Up(features.Status);

                // Log
                features.LogResolveStream(result);

                // Return
                return result;
            }
            catch (Exception e)
            {
                // Capture unexpected error
                features.LogResolveStream(e);
                features.Status.UpResolve(LineStatus.ResolveFailedException);
                LineBinaryStream lineString = new LineBinaryStream(key, e, features.Status);
                features.LogResolveStream(lineString);
                return lineString;
            }
        }

        /// <summary>
        /// Resolves key to bytes. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="features"></param>
        /// <param name="culture">The culture that matched</param>
        /// <returns>result</returns>
        LineBinaryStream ResolveKeyToStream(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                LineBinaryStream failedResult = new LineBinaryStream(key, LineStatus.NoResult);

                // Key has explicit culture
                if (culture != null)
                {
                    // 0 - main preference, 1+ - fallback preferences
                    int preferenceIndex = 0;
                    for (CultureInfo ci = culture, prev = null; ci != null && ci != prev; prev = ci, ci = ci.Parent)
                    {
                        ILine key_with_culture = preferenceIndex == 0 ? key : key.Prune(NoCultureQualifier.Default).Culture(ci);

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
                                            LineBinaryStream result = asset.GetStream(key_with_culture);
                                            if (result.Value != null)
                                            {
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

                                                return result;
                                            }
                                            else
                                            {
                                                if (result.Severity <= failedResult.Severity) failedResult = result;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                            features.LogResolveStream(e);
                                        }
                                    }
                                    break;

                                // Try inlines
                                case ResolveSource.Inline:
                                    for (int i = 0; i < features.Inlines.Count; i++)
                                    {
                                        try
                                        {
                                            ILine line;
                                            LineBinaryStream result;
                                            if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                            {
                                                result = line.GetStream();
                                                if (result.Value != null)
                                                {
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

                                                    return result;
                                                }
                                                else
                                                {
                                                    if (result.Severity <= failedResult.Severity) failedResult = result;
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                            features.LogResolveStream(e);
                                        }
                                    }
                                    break;

                                case ResolveSource.Line:
                                    // Key has explicit culture and resource, use the resource
                                    if (features.Resource != null)
                                    {
                                        if (culture == null) culture = CultureInfo.InvariantCulture;
                                        features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                        // Up status with culture info
                                        LineStatus cultureStatus =
                                            culture == null || culture.Name == "" ? LineStatus.CultureOkRequestMatchedInvariantCulture :
                                            LineStatus.CultureWarningRequestMatchedInvariantCulture;
                                        features.Status.UpCulture(cultureStatus);

                                        return new LineBinaryStream(key, new MemoryStream(features.Resource), LineStatus.ResolveOkFromLine);
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
                if (cultures != null)
                {
                    try
                    {
                        for (int preferenceIndex = 0; preferenceIndex < cultures.Length; preferenceIndex++)
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
                                                LineBinaryStream result = asset.GetStream(key_with_culture);
                                                if (result.Value != null)
                                                {
                                                    culture = ci;
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

                                                    return result;
                                                }
                                                else
                                                {
                                                    if (result.Severity <= failedResult.Severity) failedResult = result;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                                features.LogResolveStream(e);
                                            }
                                        }
                                        break;

                                    // Try inlines
                                    case ResolveSource.Inline:
                                        for (int i = 0; i < features.Inlines.Count; i++)
                                        {
                                            try
                                            {
                                                ILine line;
                                                LineBinaryStream result;
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    result = line.GetStream();
                                                    if (result.Value != null)
                                                    {
                                                        culture = ci;
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

                                                        return result;
                                                    }
                                                    else
                                                    {
                                                        if (result.Severity <= failedResult.Severity) failedResult = result;
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                                features.LogResolveStream(e);
                                            }
                                        }
                                        break;

                                    case ResolveSource.Line:
                                        // Key has explicit culture and resource, use the resource
                                        if (features.Resource != null && ci.Equals(features.Culture))
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

                                            return new LineBinaryStream(key_with_culture, new MemoryStream(features.Resource), LineStatus.ResolveOkFromLine);
                                        }
                                        break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        features.Status.UpResolve(LineStatus.CultureErrorCulturePolicyException);
                        features.LogResolveStream(e);
                    }
                }

                // Key has no explicit culture - try this _after_ culture policy
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
                                        LineBinaryStream result = asset.GetStream(key);
                                        if (result.Value != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);

                                            if (culture == null) culture = CultureInfo.InvariantCulture;

                                            // Up status with culture info
                                            LineStatus cultureStatus =
                                                culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                            features.Status.UpCulture(cultureStatus);

                                            return result;
                                        }
                                        else
                                        {
                                            if (result.Severity <= failedResult.Severity) failedResult = result;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorAssetException);
                                        features.LogResolveStream(e);
                                    }
                                }
                                break;

                            // Try inlines
                            case ResolveSource.Inline:
                                for (int i = 0; i < features.Inlines.Count; i++)
                                {
                                    try
                                    {
                                        ILine line;
                                        LineBinaryStream result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetStream();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);

                                                // Up status with culture info
                                                LineStatus cultureStatus =
                                                    culture.Name == "" ? LineStatus.CultureOkMatchedInvariantCulture :
                                                    culture.IsNeutralCulture ? LineStatus.CultureOkMatchedLanguage : LineStatus.CultureOkMatchedLanguageAndRegion;
                                                features.Status.UpCulture(cultureStatus);

                                                return result;
                                            }
                                            else
                                            {
                                                if (result.Severity <= failedResult.Severity) failedResult = result;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        features.Status.UpResolve(LineStatus.ResolveErrorInlinesException);
                                        features.LogResolveStream(e);
                                    }
                                }
                                break;

                            case ResolveSource.Line:
                                // Key has explicit culture and value, use the value
                                if (features.Resource != null)
                                {
                                    if (culture == null) culture = CultureInfo.InvariantCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromLine);

                                    if (culture == null) culture = CultureInfo.InvariantCulture;
                                    LineStatus cultureStatus = LineStatus.CultureOkMatchedInvariantCulture;
                                    features.Status.UpCulture(cultureStatus);

                                    return new LineBinaryStream(key, new MemoryStream(features.Resource), LineStatus.ResolveOkFromLine);
                                }
                                break;
                        }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(cultures != null ? LineStatus.CultureFailedCulturePolicyNoMatch : LineStatus.CultureFailedRequestNoMatch);
                }

                // No match
                features.Status.UpResolve(LineStatus.ResolveErrorNoMatch);
                culture = null;
                return failedResult;
            }
            catch (Exception e)
            {
                // Uncaptured error
                features.Status.UpResolve(LineStatus.ResolveError);
                features.LogResolveStream(e);
                return new LineBinaryStream(key, e, features.Status);
            }
        }


    }
}
