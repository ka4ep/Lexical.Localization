// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           9.4.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Part that carries additional <see cref="IFormatProvider"/> for <see cref="IStringFormat"/>.
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
        /// <item><see cref="ICustomFormatter"/></item>
        /// </list>        
        /// </summary>
        /// <param name="line"></param>
        /// <param name="formatProvider"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If <see cref="ILineFormatProvider"/> could not be appended</exception>
        public static ILineFormatProvider FormatProvider(this ILine line, IFormatProvider formatProvider)
            => line.Append<ILineFormatProvider, IFormatProvider>(formatProvider);

        /// <summary>
        /// Append format provider key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="ICustomFormatter"/></item>
        /// </list>        
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="formatProvider"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If <see cref="ILineFormatProvider"/> could not be appended</exception>
        public static ILineFormatProvider FormatProvider(this ILineFactory lineFactory, IFormatProvider formatProvider)
            => lineFactory.Create<ILineFormatProvider, IFormatProvider>(null, formatProvider);

        /// <summary>
        /// Append format provider key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="ICustomFormatter"/></item>
        /// </list>        
        /// </summary>
        /// <param name="line"></param>
        /// <param name="formatProvider">assembly qualified class name to <see cref="IFormatProvider"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If <see cref="ILineFormatProvider"/> could not be appended</exception>
        public static ILineHint FormatProvider(this ILine line, string formatProvider)
            => line.Append<ILineHint, string, string>("FormatProvider", formatProvider);

        /// <summary>
        /// Append format provider key.
        /// 
        /// Format provider is requested for following interfaces:
        /// <list type="bullet">
        /// <item><see cref="ICustomFormatter"/></item>
        /// </list>        
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="formatProvider">assembly qualified class name to <see cref="IFormatProvider"/></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If <see cref="ILineFormatProvider"/> could not be appended</exception>
        public static ILineHint FormatProvider(this ILineFactory lineFactory, string formatProvider)
            => lineFactory.Create<ILineHint, string, string>(null, "FormatProvider", formatProvider);

    }
}
