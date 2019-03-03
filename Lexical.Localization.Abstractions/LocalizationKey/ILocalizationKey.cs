// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           28.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
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
            if (str == null) throw new AssetKeyException(key, "String was not found.");
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
            // Try with the culture that is explicitly assigned to the key
            CultureInfo explicitCulture = key.FindCulture();
            if (explicitCulture != null)
            {
                // XXX: Inlines must be redesigned due to the plurality model
                string languageString = null;
                // Get inlines
                IDictionary<string, string> inlines = key.FindInlines();
                // Try inlines
                if (languageString == null && inlines != null) inlines.TryGetValue(explicitCulture?.Name ?? "", out languageString);
                // Try key
                if (languageString == null) languageString = key.TryGetString();
                // Return
                if (languageString != null) return languageString;
            }
            else
            {
                // Use culture policy
                IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
                if (cultures != null)
                {
                    string languageString = null;
                    // Get inlines
                    IDictionary<string, string> inlines = key.FindInlines();
                    foreach (CultureInfo culture in cultures)
                    {
                        // Try inlines
                        if (languageString == null && inlines != null) inlines.TryGetValue(culture?.Name ?? "", out languageString);
                        // Try key
                        if (languageString == null) languageString = key.Culture(culture).TryGetString();
                        // Return
                        if (languageString != null) return languageString;
                    }
                }
            }

            {
                // No explicit culture and culture policy didn't help. 
                string languageString = null;
                // Get inlines
                IDictionary<string, string> inlines = key.FindInlines();
                // Try inlines
                if (languageString == null && inlines != null) inlines.TryGetValue("", out languageString);
                // Try key
                if (languageString == null) languageString = key.TryGetString();
                // Return
                if (languageString != null) return languageString;
            }

            return null;
        }

        /// <summary>
        /// Resolve language string, by using the following algorithm:
        ///   1. Use explicit culture
        ///   2. Use cultures from use <see cref="ICulturePolicy"/> from <see cref="LocalizationKeyExtensions.FindCulturePolicy(IAssetKey)"/>.
        ///   3. Use key as is
        ///   
        /// Then try to formulate the string, e.g. "Error (Code=0xFEEDF00D)".
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static string ResolveFormulatedString(this IAssetKey key)
        {
            // Get args
            object[] format_args = key.FindFormatArgs();

            // Plurality key, e.g. "Key:N:One"
            IAssetKey pluralityKey = null;
            if (format_args != null)
            {
                for (int argumentIndex = 0; argumentIndex < format_args.Length; argumentIndex++)
                {
                    object o = format_args[argumentIndex];
                    if (o == null) continue;
                    string pluralityKind = GetPluralityKind(o);
                    if (pluralityKind != null) pluralityKey = (pluralityKey ?? key).N(argumentIndex, pluralityKind);
                }
            }

            // Try with the culture that is explicitly assigned to the key
            CultureInfo explicitCulture = key.FindCulture();
            if (explicitCulture != null)
            {
                // XXX: Inlines must be redesigned due to the plurality model
                string languageString = null;
                // Get inlines
                IDictionary<string, string> inlines = key.FindInlines();
                // Try inlines
                if (languageString == null && inlines != null) inlines.TryGetValue(explicitCulture?.Name ?? "", out languageString);
                // Try cardinality key
                if (languageString == null && pluralityKey != null) languageString = pluralityKey.TryGetString();
                // Try key
                if (languageString == null) languageString = key.TryGetString();
                // Formulate language string
                if (languageString != null && format_args != null) languageString = string.Format(languageString, format_args);
                // Return
                if (languageString != null) return languageString;
            }
            else
            {
                // Use culture policy
                IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
                if (cultures != null)
                {
                    string languageString = null;
                    // Get inlines
                    IDictionary<string, string> inlines = key.FindInlines();
                    foreach (CultureInfo culture in cultures)
                    {
                        // Try inlines
                        if (languageString == null && inlines != null) inlines.TryGetValue(culture?.Name ?? "", out languageString);
                        // Try cardinality key
                        if (languageString == null && pluralityKey != null) languageString = pluralityKey.Culture(culture).TryGetString();
                        // Try key
                        if (languageString == null) languageString = key.Culture(culture).TryGetString();
                        // Resolve to language string
                        if (languageString == null && pluralityKey != null) languageString = pluralityKey.Culture(culture).TryGetString();
                        // Formulate language string
                        if (languageString != null && format_args != null) languageString = string.Format(languageString, format_args);
                        // Return
                        if (languageString != null) return languageString;
                    }
                }
            }

            {
                // No explicit culture and culture policy didn't help. 
                string languageString = null;
                // Get inlines
                IDictionary<string, string> inlines = key.FindInlines();
                // Try inlines
                if (languageString == null && inlines != null) inlines.TryGetValue("", out languageString);
                // Try cardinality key
                if (languageString == null && pluralityKey != null) languageString = pluralityKey.TryGetString();
                // Try key
                if (languageString == null) languageString = key.TryGetString();
                // Formulate language string
                if (languageString != null && format_args != null) languageString = string.Format(languageString, format_args);
                // Return
                if (languageString != null) return languageString;
            }

            return null;
        }

        /// <summary>
        /// Gets plurality for number in <paramref name="o"/>. 
        /// 
        /// See: <see cref="Plurality"/>.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>null, "Zero", "One", "Plural"</returns>
        public static string GetPluralityKind(object o)
        {
            // null
            if (o == null) return null;

            Type type = o.GetType();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    byte _byte = (byte)o;
                    if (_byte == 0) return Plurality.Zero;
                    if (_byte == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.SByte:
                    sbyte _sbyte = (sbyte)o;
                    if (_sbyte == 0) return Plurality.Zero;
                    if (_sbyte == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Decimal:
                    decimal _decimal = (decimal)o;
                    if (_decimal == 0) return Plurality.Zero;
                    if (_decimal == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Int16:
                    Int16 _int16 = (Int16)o;
                    if (_int16 == 0) return Plurality.Zero;
                    if (_int16 == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Int32:
                    Int32 _int32 = (Int32)o;
                    if (_int32 == 0) return Plurality.Zero;
                    if (_int32 == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Int64:
                    Int64 _int64 = (Int64)o;
                    if (_int64 == 0L) return Plurality.Zero;
                    if (_int64 == 1L) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Single:
                    Single _single = (Single)o;
                    if (_single == 0.0f) return Plurality.Zero;
                    if (_single == 1.0f) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Double:
                    double _double = (double)o;
                    if (_double == 0.0) return Plurality.Zero;
                    if (_double == 1.0) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.UInt16:
                    UInt16 _uint16 = (UInt16)o;
                    if (_uint16 == 0) return Plurality.Zero;
                    if (_uint16 == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.UInt32:
                    UInt32 _uint32 = (UInt32)o;
                    if (_uint32 == 0) return Plurality.Zero;
                    if (_uint32 == 1) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.UInt64:
                    UInt64 _uint64 = (UInt32)o;
                    if (_uint64 == 0UL) return Plurality.Zero;
                    if (_uint64 == 1UL) return Plurality.One;
                    return Plurality.Plural;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return GetPluralityKind(Nullable.GetUnderlyingType(type));
                    return null;
            }
            return null;
        }

        private static string GetPluralityKind<T>()
            => GetPluralityKind(typeof(T));

    }
}
