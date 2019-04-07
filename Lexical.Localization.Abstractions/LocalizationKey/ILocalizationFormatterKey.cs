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
    /// A key that can be assigned with a <see cref="ILocalizationFormatter"/>.
    /// </summary>
    public interface ILocalizationFormatterAssignable : IAssetKey
    {
        /// <summary>
        /// Create key where a new <paramref name="formatter"/> has been assigned to the key chain.
        /// 
        /// If key has multiple formatters, they are evaluated in order from tail towards root.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns>key that is assigned with <paramref name="formatter"/></returns>
        ILocalizationFormatterAssigned Formatter(ILocalizationFormatter formatter);
    }

    /// <summary>
    /// A key that has been assigned with formatter
    /// </summary>
    public interface ILocalizationFormatterAssigned : IAssetKey
    {
        /// <summary>
        /// (Optional) The assigned formatter.
        /// </summary>
        ILocalizationFormatter Formatter { get; }
    }

    public static partial class LocalizationKeyExtensions
    {

        /// <summary>
        /// Get formulation string, but does not apply arguments.
        /// 
        /// Tries to resolve string with each <see cref="ILocalizationFormatter"/> until result other than <see cref="LocalizationStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="ILocalizationFormatter"/> is found return a value with state <see cref="LocalizationStatus.NoResult"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ResolveString(this IAssetKey key)
        {
            for(IAssetKey k = key; k!=null; k=k.GetPreviousKey())
            {
                ILocalizationFormatter _formatter;
                if (k is ILocalizationFormatterAssigned formatterAssigned && ((_formatter=formatterAssigned.Formatter)!=null))
                {
                    LocalizationString str = _formatter.ResolveString(key);
                    if (str.HasResult) return str.Value;
                }
            }

            // TODO Remove this
            return LocalizationFormatter.Instance.ResolveString(key).Value;
            // TODO change return type to LocalizationString -- in another method
            //return new LocalizationString(key, null, LocalizationStatus.NoResult, null);
        }

        /// <summary>
        /// Resolve and formulate string (applies arguments).
        /// 
        /// Tries to resolve string with each <see cref="ILocalizationFormatter"/> until result other than <see cref="LocalizationStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="ILocalizationFormatter"/> is found return a value with state <see cref="LocalizationStatus.NoResult"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static string ResolveFormulatedString(this IAssetKey key)
        {
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
            {
                ILocalizationFormatter _formatter;
                if (k is ILocalizationFormatterAssigned formatterAssigned && ((_formatter = formatterAssigned.Formatter) != null))
                {
                    LocalizationString str = _formatter.ResolveFormulatedString(key);
                    if (str.HasResult) return str.Value;
                }
            }

            // TODO Remove this
            return LocalizationFormatter.Instance.ResolveFormulatedString(key).Value;
            // TODO change return type to LocalizationString -- in another method
            //return new LocalizationString(key, null, LocalizationStatus.NoResult, null);
        }

    }
}
