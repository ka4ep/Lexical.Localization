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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ResolveString(this IAssetKey key)
        {
            ILocalizationFormatter formatter = LocalizationFormatter.Instance;
            LocalizationString str = formatter.ResolveString(key);
            return str.Value;
        }

        /// <summary>
        /// Resolve language string. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILocalizationKeyFormatArgs"/> part, then return the formulation string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static string ResolveFormulatedString(this IAssetKey key)
        {
            ILocalizationFormatter formatter = LocalizationFormatter.Instance;
            LocalizationString str = formatter.ResolveFormulatedString(key);
            return str.Value;
        }

    }
}
