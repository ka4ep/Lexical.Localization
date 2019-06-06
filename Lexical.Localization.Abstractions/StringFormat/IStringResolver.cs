// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;

namespace Lexical.Localization.StringFormat
{
    /// <summary>
    /// Resolver that resolves <see cref="ILine"/> into strings. 
    /// 
    /// Applies executing context dependent information, such as culture.
    /// </summary>
    public interface IStringResolver
    {
        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="IString"/>, but without applying arguments.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// 
        /// If the <see cref="IString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>format string</returns>
        IString ResolveFormatString(ILine line);

        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="LineString"/> with arguments applied.
        /// 
        /// If <paramref name="line"/> has <see cref="ICulturePolicy"/>, the applies the culture that is active in the
        /// executing context.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        LineString ResolveString(ILine line);
    }

}

namespace Lexical.Localization
{
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Resolve <paramref name="line"/> into <see cref="IString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>format string</returns>
        public static IString ResolveFormatString(this ILine line)
        {
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IStringResolver stringResolver;
                if (k is ILineStringResolver lineStringResolver && ((stringResolver = lineStringResolver.StringResolver) != null))
                {
                    IString str = stringResolver.ResolveFormatString(line);
                    if (str != null) return str;
                }
            }
            return StatusString.Null;
        }

        /// <summary>
        /// Resolve and formulate string (applies arguments).
        /// 
        /// Tries to resolve string with each <see cref="IStringResolver"/> until result other than <see cref="LineStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="IStringResolver"/> is found return a value with state <see cref="LineStatus.NoResult"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>If key has <see cref="ILineValue"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILineValue"/> part, then return the format string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static LineString ResolveString(this ILine line)
        {
            LineString result = new LineString(line, (Exception)null, LineStatus.ResolveFailedNoStringResolver);
            for (ILine k = line; k != null; k = k.GetPreviousPart())
            {
                IStringResolver _formatter;
                if (k is ILineStringResolver formatterAssigned && ((_formatter = formatterAssigned.StringResolver) != null))
                {
                    LineString str = _formatter.ResolveString(line);
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }
    }
}