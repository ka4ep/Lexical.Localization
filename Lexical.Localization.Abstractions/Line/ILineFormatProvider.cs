// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with format provider.
    /// </summary>
    public interface ILineFormatProvider : ILine
    {
        /// <summary>
        /// (Optional) The assigned format provider.
        /// </summary>
        IFormatProvider FormatProvider { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append format provider key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="IArgumentFormatter"/></item>
        /// <item><see cref="ICustomFormatter"/></item>
        /// <item><see cref="IPluralityRuleMap"/></item>
        /// <item><see cref="IPluralityCategory"/></item>
        /// </list>        
        /// </summary>
        /// <param name="key"></param>
        /// <param name="formatProvider"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement <see cref="ILocalizationKeyFormatProviderAssignable"/></exception>
        public static ILineFormatProvider FormatProvider(this ILine key, IFormatProvider formatProvider)
            => key.Append<ILineFormatProvider, IFormatProvider>(formatProvider);
    }
}
