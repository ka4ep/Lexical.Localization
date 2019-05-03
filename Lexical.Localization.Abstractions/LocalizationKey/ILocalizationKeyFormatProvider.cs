// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that can be assigned with a <see cref="IFormatProvider"/>.
    /// </summary>
    public interface ILocalizationKeyFormatProviderAssignable : ILinePart
    {
        /// <summary>
        /// Append a <paramref name="formatProvider"/> key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityRuleMap"/></item>
        /// <item><see cref="IPluralityCategory"/></item>
        /// </list>
        /// 
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <returns>key that is assigned with <paramref name="formatProvider"/></returns>
        ILocalizationKeyFormatProviderAssigned FormatProvider(IFormatProvider formatProvider);
    }

    /// <summary>
    /// A key that has been assigned with format provider.
    /// </summary>
    public interface ILocalizationKeyFormatProviderAssigned : ILinePart
    {
        /// <summary>
        /// (Optional) The assigned format provider.
        /// </summary>
        IFormatProvider FormatProvider { get; }
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Append format provider key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="ILocalizationArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityRuleMap"/></item>
        /// <item><see cref="IPluralityCategory"/></item>
        /// </list>        
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formatProvider"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="ILocalizationKeyFormatProviderAssignable"/></exception>
        public static ILocalizationKeyFormatProviderAssigned FormatProvider(this ILinePart key, IFormatProvider formatProvider)
        {
            if (key is ILocalizationKeyFormatProviderAssignable casted) return casted.FormatProvider(formatProvider);
            throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyFormatProviderAssignable)}.");
        }
    }
}
