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

namespace Lexical.Localization.Resource
{
    /// <summary>
    /// Resolves keys to resources by applying contextual information such as culture.
    /// </summary>
    public class ResourceResolver : IResourceResolver
    {
        private static ResourceResolver instance = new ResourceResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ResourceResolver Default => instance;

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
        public ResourceResolver()
        {
            this.Resolvers = Lexical.Localization.Resolver.Resolvers.Default;
            this.ResolveSequence = ResolverSequence.Default;
        }

        /// <summary>
        /// Create resource resolver 
        /// </summary>
        /// <param name="resolvers"></param>
        /// <param name="resolveSequence"></param>
        public ResourceResolver(IResolver resolvers, ResolveSource[] resolveSequence = default)
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
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceBytes.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result status</returns>
        public LineResourceBytes ResolveBytes(ILine key)
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
                return new LineResourceBytes(key, e, features.Status);
            }

            try
            {
                // Resolve key to line
                CultureInfo culture = features.Culture;

                // Resolve to bytes
                LineResourceBytes result = ResolveKeyToBytes(key, ref features, ref culture);

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
                LineResourceBytes lineString = new LineResourceBytes(key, e, features.Status);
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
        LineResourceBytes ResolveKeyToBytes(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                LineResourceBytes failedResult = new LineResourceBytes(key, LineStatus.NoResult);

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
                                        LineResourceBytes result = asset.GetResourceBytes(key);
                                        if (result.Value!=null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                            return result;
                                        } else
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
                                        LineResourceBytes result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetResourceBytes();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                                return result;
                                            } else
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
                                    if (culture == null) culture = RootCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                    return new LineResourceBytes(key, features.Resource, LineStatus.ResolveOkFromKey);
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
                    try
                    {
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
                                                LineResourceBytes result = asset.GetResourceBytes(key_with_culture);
                                                if (result.Value != null)
                                                {
                                                    culture = c;
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                                    features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                                    features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
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
                                                LineResourceBytes result;
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    result = line.GetResourceBytes();
                                                    if (result.Value != null)
                                                    {
                                                        culture = c;
                                                        features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                        features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
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
                                        if (features.Resource != null && c.Equals(features.Culture))
                                        {
                                            if (culture == null) culture = RootCulture;
                                            features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                            return new LineResourceBytes(key_with_culture, features.Resource, LineStatus.ResolveOkFromKey);
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
                                        LineResourceBytes result = asset.GetResourceBytes(key);
                                        if (result.Value != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
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
                                        LineResourceBytes result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetResourceBytes();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
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
                                    if (culture == null) culture = RootCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
                                    return new LineResourceBytes(key, features.Resource, LineStatus.ResolveOkFromKey);
                                }
                                break;
                        }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(LineStatus.CultureErrorCultureNoMatch);
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
                return new LineResourceBytes(key, e, features.Status);
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
        ///     <item><see cref="LineStatus.ResolveOkFromKey"/>Resource was acquired</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoValue"/>If resource could not be found</item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResult"/>Request was not processed</item>
        ///     <item><see cref="LineStatus.ResolveFailedException"/>Unexpected exception was thrown, <see cref="LineResourceStream.Exception"/></item>
        ///     <item><see cref="LineStatus.ResolveFailedNoResourceResolver"/>Resolver was not found.</item>
        /// </list>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>result status</returns>
        public LineResourceStream ResolveStream(ILine key)
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
                return new LineResourceStream(key, e, features.Status);
            }

            try
            {
                // Resolve key to line
                CultureInfo culture = features.Culture;

                // Resolve to bytes
                LineResourceStream result = ResolveKeyToStream(key, ref features, ref culture);

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
                LineResourceStream lineString = new LineResourceStream(key, e, features.Status);
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
        LineResourceStream ResolveKeyToStream(ILine key, ref LineFeatures features, ref CultureInfo culture)
        {
            try
            {
                // Tmp variable
                LineResourceStream failedResult = new LineResourceStream(key, LineStatus.NoResult);

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
                                        LineResourceStream result = asset.GetResourceStream(key);
                                        if (result.Value != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
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
                                        LineResourceStream result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetResourceStream();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
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
                                    if (culture == null) culture = RootCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                    return new LineResourceStream(key, new MemoryStream(features.Resource), LineStatus.ResolveOkFromKey);
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
                    try
                    {
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
                                                LineResourceStream result = asset.GetResourceStream(key_with_culture);
                                                if (result.Value != null)
                                                {
                                                    culture = c;
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                                    features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
                                                    features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                                    features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
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
                                                LineResourceStream result;
                                                if (features.Inlines[i].TryGetValue(key_with_culture, out line))
                                                {
                                                    result = line.GetResourceStream();
                                                    if (result.Value != null)
                                                    {
                                                        culture = c;
                                                        features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                        features.Status.UpCulture(c.Name == "" ? LineStatus.CultureWarningNoMatch : LineStatus.CultureOkMatchedCulturePolicy);
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
                                        if (features.Resource != null && c.Equals(features.Culture))
                                        {
                                            if (culture == null) culture = RootCulture;
                                            features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedKeyCulture);
                                            return new LineResourceStream(key_with_culture, new MemoryStream(features.Resource), LineStatus.ResolveOkFromKey);
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
                                        LineResourceStream result = asset.GetResourceStream(key);
                                        if (result.Value != null)
                                        {
                                            features.Status.UpResolve(LineStatus.ResolveOkFromAsset);
                                            features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
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
                                        LineResourceStream result;
                                        if (features.Inlines[i].TryGetValue(key, out line))
                                        {
                                            result = line.GetResourceStream();
                                            if (result.Value != null)
                                            {
                                                features.Status.UpResolve(LineStatus.ResolveOkFromInline);
                                                features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
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
                                    if (culture == null) culture = RootCulture;
                                    features.Status.UpResolve(LineStatus.ResolveOkFromKey);
                                    features.Status.UpCulture(LineStatus.CultureOkMatchedNoCulture);
                                    return new LineResourceStream(key, new MemoryStream(features.Resource), LineStatus.ResolveOkFromKey);
                                }
                                break;
                        }

                    // No matching value was found for the requested key and the explicit culture in the key.
                    features.Status.UpCulture(LineStatus.CultureErrorCultureNoMatch);
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
                return new LineResourceStream(key, e, features.Status);
            }
        }


        static CultureInfo RootCulture = CultureInfo.GetCultureInfo("");

    }
}
