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
    /// A key that can be assigned with a <see cref="ILocalizationResolver"/>.
    /// </summary>
    public interface ILocalizationKeyResolverAssignable : IAssetKey
    {
        /// <summary>
        /// Append a <paramref name="resolver"/> key.
        /// 
        /// If key has multiple formatters, they are evaluated in order of - from tail towards root -.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns>key that is assigned with <paramref name="resolver"/></returns>
        ILocalizationKeyResolverAssigned Resolver(ILocalizationResolver resolver);
    }

    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILocalizationKeyResolverAssigned : IAssetKey
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        ILocalizationResolver Resolver { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Append format provider key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="ILocalizationKeyResolverAssignable"/></exception>
        public static ILocalizationKeyResolverAssigned Resolver(this IAssetKey key, ILocalizationResolver resolver)
        {
            if (key is ILocalizationKeyResolverAssignable casted) return casted.Resolver(resolver);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyResolverAssignable)}.");
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
        public static LocalizationString ResolveString(this IAssetKey key)
        {
            LocalizationString result = new LocalizationString(key, null, LocalizationStatus.NoResult, null);
            for (IAssetKey k = key; k!=null; k=k.GetPreviousKey())
            {
                ILocalizationResolver _formatter;
                if (k is ILocalizationKeyResolverAssigned formatterAssigned && ((_formatter=formatterAssigned.Resolver)!=null))
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
        /// <returns>If key has <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static LocalizationString ResolveFormulatedString(this IAssetKey key)
        {
            LocalizationString result = new LocalizationString(key, null, LocalizationStatus.NoResult, null);
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                ILocalizationResolver _formatter;
                if (k is ILocalizationKeyResolverAssigned formatterAssigned && ((_formatter = formatterAssigned.Resolver) != null))
                {
                    LocalizationString str = _formatter.ResolveFormulatedString(key);
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

    }
}
