// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineLocalizationResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        ILocalizationResolver Resolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append format provider key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILocalizationKeyResolverAssignable"/></exception>
        public static ILineLocalizationResolver Resolver(this ILine key, ILocalizationResolver resolver)
        {
            if (key is ILocalizationKeyResolverAssignable casted) return casted.Resolver(resolver);
            throw new LineException(key, $"doesn't implement {nameof(ILocalizationKeyResolverAssignable)}.");
        }

        /// <summary>
        /// Get formulation string, but does not apply arguments.
        /// 
        /// Tries to resolve string with each <see cref="ILocalizationResolver"/> until result other than <see cref="LocalizationStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="ILocalizationResolver"/> is found return a value with state <see cref="LocalizationStatus.NoResult"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static LocalizationString ResolveString(this ILine key)
        {
            LocalizationString result = new LocalizationString(key, null, LocalizationStatus.NoResult);
            for (ILine k = key; k!=null; k=k.GetPreviousPart())
            {
                ILocalizationResolver _formatter;
                if (k is ILineLocalizationResolver formatterAssigned && ((_formatter=formatterAssigned.Resolver)!=null))
                {
                    LocalizationString str = _formatter.ResolveString(key);
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

        /// <summary>
        /// Resolve and formulate string (applies arguments).
        /// 
        /// Tries to resolve string with each <see cref="ILocalizationResolver"/> until result other than <see cref="LocalizationStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="ILocalizationResolver"/> is found return a value with state <see cref="LocalizationStatus.NoResult"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILineFormatArgsPart"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILineFormatArgsPart"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static LocalizationString ResolveFormulatedString(this ILine key)
        {
            LocalizationString result = new LocalizationString(key, null, LocalizationStatus.NoResult);
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                ILocalizationResolver _formatter;
                if (k is ILineLocalizationResolver formatterAssigned && ((_formatter = formatterAssigned.Resolver) != null))
                {
                    LocalizationString str = _formatter.ResolveFormulatedString(key);
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

        /// <summary>
        /// Resolve localization resource using the active culture. Uses the following algorithm:
        ///   1. If key has a selected culture, try that
        ///      a) from Asset
        ///   2. If key has <see cref="ICulturePolicy"/>, iterate the cultures.
        ///      a) Try asset
        ///   3. Try to read value for key from asset as is
        ///   4. Return null
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or null</returns>
        public static byte[] ResolveResource(this ILine key)
        {
            // Arrange
            IAsset asset = key.FindAsset();
            byte[] result = null;

            // 1. Try selected culture
            CultureInfo selectedCulture = key.GetCultureInfo();
            if (selectedCulture != null)
            {
                // 1a. Try from asset
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // 2. Try culture policy
            IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
            if (cultures != null)
            {
                foreach (var culture in cultures)
                {
                    // This was already tried above
                    if (culture == selectedCulture) continue;

                    // 2a. Try from asset
                    if (asset != null)
                    {
                        ILine cultured = key.TryAppendCulture(culture);
                        if (cultured != null)
                        {
                            result = asset.GetResource(cultured);
                            if (result != null) return result;
                        }
                    }
                }
            }

            // 3. Try key as is
            if (asset != null)
            {
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // Not found
            return null;
        }
    }
}
