// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the class gives localizations specific parameters, hints or capabilities.
    /// 
    /// Parameters:
    ///     <see cref="ILocalizationKeyCultureAssigned"/>
    ///     <see cref="ILocalizationKeyInlined"/>
    ///     <see cref="ILocalizationKeyFormatArgs"/>
    ///     
    /// Hints:
    ///     <see cref="ILocalizationKeyCulturePolicyAssigned"/>
    ///     
    /// Capabilities:
    ///     <see cref="ILocalizationKeyCultureAssignable"/>
    ///     <see cref="ILocalizationKeyCulturePolicyAssignable"/>
    ///     <see cref="ILocalizationKeyFormattable"/>
    ///     <see cref="ILocalizationKeyInlineAssignable"/>
    ///     
    /// The ILocalizationKey ToString() must try to resolve the key.
    /// If resolve fails ToString returns the built name of the key.
    /// </summary>
    public interface ILocalizationKey : IAssetKey
    {
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(IAssetKey)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>language string</returns>
        /// <exception cref="AssetKeyException">If resolving failed or resolver was not found</exception>
        public static string GetString(this IAssetKey key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new AssetKeyException(key, "String resolver was not found.");
            string str = asset.GetString(key);
            if (str == null) throw new AssetKeyException(key, $"String {key} was not found.");
            return str;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(IAssetKey)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>language string, or null if language string was not found, or if resolver was not found</returns>
        public static string TryGetString(this IAssetKey key)
            => key.FindAsset()?.GetString(key);

        /// <summary>
        /// Resolve language string using the active culture. Uses the following algorithm:
        ///   1. If key has a selected culture, try that
        ///      a) from inlines
        ///      b) from Asset
        ///   2. If key has <see cref="ICulturePolicy"/>, iterate the cultures.
        ///      a) Try inlined strings.
        ///      b) Try asset
        ///   3. Try to read value for key from asset as is
        ///   4. Return null
        ///   
        /// Does not formulate string, returns the formulation string, e.g. "Error (Code=0x{0:X8})".
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string (without formulating it) or null</returns>
        public static string ResolveString(this IAssetKey key)
        {
            // Arrange
            IAsset asset = key.FindAsset();
            IDictionary<string, string> inlines = key.FindInlines();
            string result = null;

            // 1. Try selected culture
            CultureInfo selectedCulture = key.FindCulture();
            if (selectedCulture != null)
            {
                // 1a. Try inlines
                if (inlines != null && inlines.TryGetValue(selectedCulture?.Name ?? "", out result) && result != null) return result;

                // 1b. Try from asset
                result = asset.GetString(key);
                if (result != null) return result;
            }

            // 2. Try culture policy
            IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
            if (cultures != null)
            {
                foreach(var culture in cultures)
                {
                    // This was already tried above
                    if (culture == selectedCulture) continue;

                    // 2a. Try from inlines
                    if (inlines != null && inlines.TryGetValue(culture.Name, out result) && result != null) return result;

                    // 2b. Try from asset
                    if (asset != null)
                    {
                        ILocalizationKey cultured = key.TrySetCulture(culture);
                        if (cultured != null)
                        {
                            result = asset.GetString(cultured);
                            if (result != null) return result;
                        }
                    }
                }
            }

            // 3. Try key as is
            if (asset != null)
            {
                result = asset.GetString(key);
                if (result != null) return result;
            }

            // Not found
            return null;
        }

        /// <summary>
        /// Resolve language string, by using the following algorithm:
        ///   1. Try key as is.
        ///   2. Try <see cref="ICulturePolicy"/> from <see cref="LocalizationKeyExtensions.FindCulturePolicy(IAssetKey)"/>.
        ///   3. Try inlined strings.
        ///   4. Return null
        ///   
        /// Then try to formulate the string, e.g. "Error (Code=0xFEEDF00D)".
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static string ResolveFormulatedString(this IAssetKey key)
        {
            // Resolve to language string
            string languageString = key.ResolveString();
            // Nothing was resolved
            if (languageString == null) return null;

            //// Apply formulation            
            // Resolve culture
            CultureInfo culture = key.FindCulture() ?? key.FindCulturePolicy()?.Cultures?.First();
            // Get args
            object[] format_args = key.FindFormatArgs();
            // Formulate
            return format_args == null ? languageString : 
                culture == null ?
                string.Format(languageString, format_args) :
                string.Format(culture, languageString, format_args);
        }

    }
}
