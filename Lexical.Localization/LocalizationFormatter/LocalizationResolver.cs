// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// The default localization formatter implementation.
    /// </summary>
    public class LocalizationResolver : ILocalizationResolver
    {
        private static LocalizationResolver instance = new LocalizationResolver();

        /// <summary>
        /// Default instance
        /// </summary>
        public static LocalizationResolver Instance => instance;

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
        public LocalizationString ResolveString(IAssetKey key)
        {
            // If there is no explicitly assigned culture in the key, try cultures from culture policy
            string explicitCulture = key.FindCultureName();
            IEnumerable<CultureInfo> cultures = null;
            bool rootCultureTried = false;
            if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                foreach (CultureInfo culture in cultures)
                {
                    bool rootCulture = culture.Name == "";
                    rootCultureTried |= rootCulture;
                    // 
                    IAssetKey culture_key = rootCulture ? key : key.Culture(culture);
                    // Try inlines
                    if (languageString == null && inlines != null) inlines.TryGetValue(culture_key, out languageString);
                    // Try key
                    if (languageString == null) languageString = culture_key.TryGetString();
                    // Return
                    if (languageString != null) return new LocalizationString(key, languageString, 0UL, this);
                }
            }

            if (!rootCultureTried)
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                // Try inlines with key
                if (languageString == null && inlines != null) inlines.TryGetValue(key, out languageString);
                // Try asset with key
                if (languageString == null) languageString = key.TryGetString();
                // Return
                if (languageString != null) return new LocalizationString(key, languageString, 0UL, this);
            }

            return new LocalizationString(key, null, LocalizationStatus.NoResult, this);
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
        public LocalizationString ResolveFormulatedString(IAssetKey key)
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
            bool rootCultureTried = false;
            string explicitCulture = key.FindCultureName();
            IEnumerable<CultureInfo> cultures = null;
            if (explicitCulture == null && (cultures = key.FindCulturePolicy()?.Cultures) != null)
            {
                string languageString = null;
                // Get inlines
                IDictionary<IAssetKey, string> inlines = key.FindInlines();
                foreach (CultureInfo culture in cultures)
                {
                    bool rootCulture = culture.Name == "";
                    rootCultureTried |= rootCulture;
                    // Append culture
                    IAssetKey pluralityKey_with_culture = rootCulture ? pluralityKey : pluralityKey?.Culture(culture);
                    // Try inlines with plurality key
                    if (languageString == null && inlines != null && pluralityKey_with_culture != null) inlines.TryGetValue(pluralityKey_with_culture, out languageString);
                    // Try inlines with plurality key permutations
                    if (languageString == null && inlines != null && pluralityKeys != null)
                    {
                        foreach (IAssetKey _pluralityKey in pluralityKeys)
                            if (inlines.TryGetValue(_pluralityKey.Culture(culture), out languageString) && languageString != null) break;
                    }
                    // Append culture
                    IAssetKey key_with_culture = rootCulture ? key : key.Culture(culture);
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
                    if (languageString != null && format_args != null) languageString = String.Format(culture, languageString, format_args);
                    // Return
                    if (languageString != null) return new LocalizationString(key, languageString, 0UL, this);
                }
            }

            // Try key as is
            if (!rootCultureTried)
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
                if (languageString != null && format_args != null) languageString = String.Format(rootCulture, languageString, format_args);
                // Return
                if (languageString != null) return new LocalizationString(key, languageString, 0UL, this);
            }

            return new LocalizationString(key, null, LocalizationStatus.NoResult, this);
        }

        static CultureInfo rootCulture = CultureInfo.GetCultureInfo("");

        /*
        public struct ArgumentLine
        {
            public object arg;
            public int argIndex;
            public IFormatProvider formatProvider;
            public string format;
            public string formattedValue;
        }*/

        /// <summary>
        /// Gets plurality for number in <paramref name="o"/>. 
        /// 
        /// See: <see cref="Plurality"/>.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>null, "Zero", "One", "Plural"</returns>
        protected static string GetPluralityKind(object o)
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

        static string Format(object o, string format, IFormatProvider culture)
        {
            if (o is IFormattable formattable) return formattable.ToString(format, culture);
            if (culture.GetFormat(typeof(ICustomFormatter)) is ICustomFormatter customFormatter_)
                return customFormatter_.Format(format, o, culture);
            return culture == null ? String.Format("{0:" + format + "}", o) : String.Format(culture, "{0:" + format + "}", o);
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
