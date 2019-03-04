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
        /// Resolve the formulation string. 
        /// 
        /// Uses the following algorithm:
        ///   1. Either explicitly assigned culture or <see cref="ICulturePolicy"/> from <see cref="LocalizationKeyExtensions.FindCulturePolicy(IAssetKey)"/>.
        ///   2. Try key as is.
        ///   
        ///      a. Search inlines with culture
        ///      b. Search asset with culture
        /// </summary>
        /// <param name="key"></param>
        /// <returns>formulation string (without formulating it) or null</returns>
        public static string ResolveString(this IAssetKey key)
        {
            // If there is no explicitly assigned culture in the key, try cultures from culture policy
            string explicitCulture = key.FindCultureByName();
            IEnumerable<CultureInfo> cultures = null;
            if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                foreach (CultureInfo culture in cultures)
                {
                    IAssetKey culture_key = key.Culture(culture);
                    // Try inlines
                    if (languageString == null && inlines != null) inlines.TryGetValue(culture_key, out languageString);
                    // Try key
                    if (languageString == null) languageString = culture_key.TryGetString();
                    // Return
                    if (languageString != null) return languageString;
                }
            }

            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                // Try inlines with key
                if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                // Try asset with key
                if (languageString == null) languageString = key.TryGetString();
                // Return
                if (languageString != null) return languageString;
            }

            return null;
        }

        /// <summary>
        /// Resolve language string. 
        /// 
        /// Uses the following algorithm:
        ///   1. Either explicitly assigned culture or <see cref="ICulturePolicy"/> from <see cref="LocalizationKeyExtensions.FindCulturePolicy(IAssetKey)"/>.
        ///   2. Try key as is.
        ///   
        ///      a. Search inlines with plurality and culture
        ///      b. Search inlines with culture
        ///      c. Search asset with plurality and culture
        ///      d. Search asset with culture
        ///   
        ///   3. Then try to formulate the string with assigned arguments, e.g. "Error (Code=0xFEEDF00D)"
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static string ResolveFormulatedString(this IAssetKey key)
        {
            // Get args
            object[] format_args = key.FindFormatArgs();

            // Plurality key when there is only one numeric argument. e.g. "Key:N:One"
            IAssetKey pluralityKey = null;
            // Pluarlity key for all argument permutations (whether argument is provided or not)
            IEnumerable<IAssetKey> pluralityKeys = null;
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
                        pluralityKeys = CreatePluralityKeyPermutations(key: key, maxArgumentCount: Plurality.MAX_NUMERIC_ARGUMENTS_TO_PERMUTATE, args: format_args); // 2^5 = 32 keys
                        break;
                    }
                }
            }

            // If there is no explicitly assigned culture in the key, try cultures from culture policy
            string explicitCulture = key.FindCultureByName();
            IEnumerable<CultureInfo> cultures = null;
            if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                foreach (CultureInfo culture in cultures)
                {
                    // Append culture
                    IAssetKey pluralityKey_with_culture = pluralityKey?.Culture(culture);
                    // Try inlines with plurality key
                    if (languageString == null && inlines != null && pluralityKey_with_culture != null) inlines.TryGetValue(pluralityKey_with_culture, out languageString);
                    // Try inlines with plurality key permutations
                    if (languageString == null && inlines != null && pluralityKeys != null)
                    {
                        foreach (IAssetKey _pluralityKey in pluralityKeys)
                            if (inlines.TryGetValue(_pluralityKey.Culture(culture), out languageString) && languageString != null) break;
                    }
                    // Append culture
                    IAssetKey key_with_culture = key.Culture(culture);
                    // Try inlines with fallback key
                    if (languageString == null && inlines != null) inlines.TryGetValue(key_with_culture, out languageString);
                    // Try asset with plurality key
                    if (languageString == null && pluralityKey_with_culture != null) languageString = pluralityKey_with_culture.TryGetString();
                    // Try asset with plurality key permutations
                    if (languageString == null && pluralityKeys != null)
                    {
                        foreach (IAssetKey _pluralityKey in pluralityKeys)
                        {
                            languageString = _pluralityKey.TryGetString();
                            if (languageString != null) break;
                        }
                    }
                    // Try asset with fallback key
                    if (languageString == null) languageString = key_with_culture.TryGetString();
                    // Formulate language string
                    if (languageString != null && format_args != null) languageString = string.Format(languageString, format_args);
                    // Return
                    if (languageString != null) return languageString;
                }
            }

            // Try key as is
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                // Try inlines with plurality key
                if (languageString == null && inlines != null && pluralityKey != null) inlines.TryGetValue(pluralityKey, out languageString);
                // Try inlines with plurality key permutations
                if (languageString == null && inlines != null && pluralityKeys != null)
                {
                    foreach (IAssetKey _pluralityKey in pluralityKeys)
                        if (inlines.TryGetValue(_pluralityKey, out languageString) && languageString != null) break;
                }
                // Try inlines with fallback key
                if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                // Try asset with plurality key
                if (languageString == null && pluralityKey != null) languageString = pluralityKey.TryGetString();
                // Try asset with plurality key permutations
                if (languageString == null && pluralityKeys != null)
                {
                    foreach (IAssetKey _pluralityKey in pluralityKeys)
                    {
                        languageString = _pluralityKey.TryGetString();
                        if (languageString != null) break;
                    }
                }
                // Try asset with fallback key
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
        private static IEnumerable<IAssetKey> CreatePluralityKeyPermutations(IAssetKey key, int maxArgumentCount, object[] args)
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

        private static IEnumerable<IAssetKey> _CreatePluralityKeyPermutations(IAssetKey key, (int, string)[] argInfos, int argumentCount)
        {            
            int allBits = (int)(1 << argumentCount) - 1;
            for (int bits = allBits; bits > 0; bits--)
            {
                IAssetKey pluralityKey = key;
                for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
                {
                    if ((bits & (1 << argumentIndex)) == 0) continue;
                    pluralityKey = pluralityKey.N(argumentIndex, argInfos[argumentIndex].Item2);
                }
                yield return pluralityKey;
            }
        }

        private static IEnumerable<IAssetKey> _CreatePluralityKeysSingleArgumentOnly(IAssetKey key, (int, string)[] argInfos, int argumentCount)
        {
            for (int argumentIndex = 0; argumentIndex < argumentCount; argumentIndex++)
            {
                yield return key.N(argumentIndex, argInfos[argumentIndex].Item2);
            }
        }

    }
}
