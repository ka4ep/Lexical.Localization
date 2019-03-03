// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Culture" parameter assignment.
    /// </summary>
    public interface ILocalizationKeyCultureAssignable : IAssetKey
    {
        /// <summary>
        /// Select a specific culture. 
        /// 
        /// Adds <see cref="ILocalizationKeyCultureAssigned"/> link.
        /// </summary>
        /// <param name="subkey">Name for new sub key.</param>
        /// <returns>new key</returns>
        ILocalizationKeyCultureAssigned SetCulture(CultureInfo culture);

        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        /// <exception cref="CultureNotFoundException">if culture was not found</exception>
        ILocalizationKeyCultureAssigned SetCulture(string cultureName);
    }

    /// <summary>
    /// Key (may have) has "Culture" parameter assigned.
    /// </summary>
    public interface ILocalizationKeyCultureAssigned : IAssetKeyNonCanonicallyCompared, ILocalizationKey
    {
        /// <summary>
        /// Selected culture, or null.
        /// </summary>
        CultureInfo Culture { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILocalizationKeyCultureAssigned Culture(this IAssetKey key, CultureInfo culture)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.SetCulture(culture);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyCultureAssignable)}.");
        }

        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILocalizationKeyCultureAssigned Culture(this IAssetKey key, string cultureName)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.SetCulture(cultureName);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyCultureAssignable)}.");
        }

        /// <summary>
        /// Try to set to a specific culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key or null</returns>
        public static ILocalizationKeyCultureAssigned TrySetCulture(this IAssetKey key, string cultureName)
        {
            try
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
                if (key is ILocalizationKeyCultureAssignable casted) return casted.SetCulture(culture);
            }
            catch (CultureNotFoundException) { }
            return null;
        }

        /// <summary>
        /// Try to set to a specific culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key or null</returns>
        public static ILocalizationKeyCultureAssigned TrySetCulture(this IAssetKey key, CultureInfo culture)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.SetCulture(culture);
            return null;
        }

        /// <summary>
        /// Search linked list and find selected culture.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture policy or null</returns>
        public static CultureInfo FindCulture(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyCultureAssigned cultureKey && cultureKey.Culture != null) return cultureKey.Culture;
            return null;
        }

        /// <summary>
        /// Search linked list and find selected culture.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture policy or null</returns>
        public static ILocalizationKeyCultureAssigned FindCultureKey(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyCultureAssigned cultureKey && cultureKey.Name != null) return cultureKey;
            return null;
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
        public static byte[] ResolveResource(this IAssetKey key)
        {
            // Arrange
            IAsset asset = key.FindAsset();
            byte[] result = null;

            // 1. Try selected culture
            CultureInfo selectedCulture = key.FindCulture();
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
                        ILocalizationKey cultured = key.TrySetCulture(culture);
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

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILocalizationKeyCultureAssigned"/> values of keys.
    /// </summary>
    public class LocalizationKeyCultureComparer : IEqualityComparer<IAssetKey>
    {
        static IEqualityComparer<CultureInfo> comparer = new EqualsComparer<CultureInfo>();
        public bool Equals(IAssetKey x, IAssetKey y)
            => comparer.Equals(x?.FindCulture(), y?.FindCulture());
        public int GetHashCode(IAssetKey obj)
            => comparer.GetHashCode(obj?.FindCulture());
    }

}
