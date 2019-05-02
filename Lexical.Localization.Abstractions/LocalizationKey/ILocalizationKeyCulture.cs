// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Culture" parameter assignment.
    /// </summary>
    public interface ILocalizationKeyCultureAssignable : ILinePart
    {
        /// <summary>
        /// Select a specific culture. 
        /// 
        /// Adds <see cref="ILocalizationKeyCultureAssigned"/> link.
        /// </summary>
        /// <param name="culture">Name for new sub key.</param>
        /// <returns>new key</returns>
        ILocalizationKeyCultureAssigned Culture(CultureInfo culture);

        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        /// <exception cref="CultureNotFoundException">if culture was not found</exception>
        ILocalizationKeyCultureAssigned Culture(string cultureName);
    }

    /// <summary>
    /// Key (may have) has "Culture" parameter assigned.
    /// </summary>
    public interface ILocalizationKeyCultureAssigned : ILineKeyNonCanonicallyCompared, ILocalizationKey
    {
        /// <summary>
        /// Selected culture, or null.
        /// </summary>
        CultureInfo Culture { get; }
    }

    /// <summary>
    /// </summary>
    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILocalizationKeyCultureAssigned Culture(this ILinePart key, CultureInfo culture)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.Culture(culture);
            //if (key is IAssetKeyParameterAssignable parametrizable) return parametrizable.AppendParameter("Culture", culture.Name);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyCultureAssignable)}.");
        }

        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILocalizationKeyCultureAssigned Culture(this ILinePart key, string cultureName)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.Culture(cultureName);
            //if (key is IAssetKeyParameterAssignable parametrizable) return parametrizable.AppendParameter("Culture", cultureName);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyCultureAssignable)}.");
        }

        /// <summary>
        /// Try to set to a specific culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key or null</returns>
        public static ILocalizationKeyCultureAssigned TrySetCulture(this ILinePart key, string cultureName)
        {
            try
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
                if (key is ILocalizationKeyCultureAssignable casted) return casted.Culture(culture);
            }
            catch (CultureNotFoundException) {
                if (key is ILocalizationKeyCultureAssignable casted) return casted.Culture(cultureName);
            }
            //if (key is IAssetKeyParameterAssignable parametrizable) return parametrizable.AppendParameter("Culture", cultureName);
            return null;
        }

        /// <summary>
        /// Try to set to a specific culture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key or null</returns>
        public static ILocalizationKeyCultureAssigned TrySetCulture(this ILinePart key, CultureInfo culture)
        {
            if (key is ILocalizationKeyCultureAssignable casted) return casted.Culture(culture);
            //if (key is IAssetKeyParameterAssignable parametrizable) return parametrizable.AppendParameter("Culture", culture.Name);
            return null;
        }

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILocalizationKeyCultureAssigned"/> key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture info or null</returns>
        public static CultureInfo FindCulture(this ILinePart key)
        {
            string cultureName = null;
            CultureInfo culture = null;
            for (; key != null; key = key.PreviousPart)
            {
                if (key is ILocalizationKeyCultureAssigned cultureKey) {
                    if (cultureKey.Culture != null) culture = cultureKey.Culture;
                    else if (cultureKey.GetParameterValue() != null) cultureName = cultureKey.GetParameterValue();
                }
                else if (key is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) cultureName = parameterKey.ParameterValue;
            }
            if (culture != null) return culture;
            if (cultureName != null) try { return CultureInfo.GetCultureInfo(cultureName); } catch (CultureNotFoundException) { }
            return null;
        }

        /// <summary>
        /// Search linked list and find selected (left-most) culture name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture name or null</returns>
        public static string FindCultureName(this ILinePart key)
        {
            string result = null;
            for (; key != null; key = key.PreviousPart)
            {
                if (key is ILocalizationKeyCultureAssigned cultureKey && cultureKey.GetParameterValue() != null) result = cultureKey.GetParameterValue();
                else if (key is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) result = parameterKey.ParameterValue;
            }
            return result;
        }

        /// <summary>
        /// Search linked list and find the effective culture key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>culture policy or null</returns>
        public static ILinePart FindCultureKey(this ILinePart key)
        {
            ILinePart result = null;
            for (; key != null; key = key.PreviousPart)
            {
                if (key is ILocalizationKeyCultureAssigned cultureKey && cultureKey.GetParameterValue() != null) result = cultureKey;
                else if (key is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) result = parameterKey;
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
        public static byte[] ResolveResource(this ILinePart key)
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
    /// 
    /// This comparer regards null and "" non-equivalent. 
    /// </summary>
    public class LocalizationKeyCultureComparer : IEqualityComparer<ILinePart>
    {
        //static IEqualityComparer<CultureInfo> culture_comparer = new EqualsComparer<CultureInfo>();
        static IEqualityComparer<string> string_comparer = StringComparer.InvariantCulture;
        public bool Equals(ILinePart x, ILinePart y)
            => string_comparer.Equals(x?.FindCultureName(), y?.FindCultureName());
        public int GetHashCode(ILinePart obj)
            => string_comparer.GetHashCode(obj?.FindCultureName());
    }

    /// <summary>
    /// Non-canonical comparer that compares <see cref="ILocalizationKeyCultureAssigned"/> values of keys.
    /// 
    /// This comparer regards null and "" equivalent. 
    /// </summary>
    public class LocalizationKeyCultureComparer2 : IEqualityComparer<ILinePart>
    {
        //static IEqualityComparer<CultureInfo> culture_comparer = new EqualsComparer<CultureInfo>();
        static IEqualityComparer<string> string_comparer = StringComparer.InvariantCulture;
        public bool Equals(ILinePart x, ILinePart y)
            => string_comparer.Equals(x?.FindCultureName() ?? "", y?.FindCultureName() ?? "");
        public int GetHashCode(ILinePart obj)
            => string_comparer.GetHashCode(obj?.FindCultureName() ?? "");
    }

}
