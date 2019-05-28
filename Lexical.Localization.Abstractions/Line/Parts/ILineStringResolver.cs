// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// A key that has been assigned with resolver.
    /// </summary>
    public interface ILineStringResolver : ILine
    {
        /// <summary>
        /// (Optional) The assigned resolver.
        /// </summary>
        IStringResolver Resolver { get; set; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append localization resolver.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver Resolver(this ILine line, IStringResolver resolver)
            => line.Append<ILineStringResolver, IStringResolver>(resolver);

        /// <summary>
        /// Create localization resolver.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="resolver"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If part append fails</exception>
        public static ILineStringResolver Resolver(this ILineFactory lineFactory, IStringResolver resolver)
            => lineFactory.Create<ILineStringResolver, IStringResolver>(null, resolver);

        /// <summary>
        /// Resolve <paramref name="key"/> into <see cref="IFormatString"/>, but without applying format arguments.
        /// 
        /// If the <see cref="IFormatString"/> contains plural categories, then matches into the applicable plurality case.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>format string</returns>
        public static IFormatString ResolveFormatString(this ILine key)
        {
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                IStringResolver _formatter;
                if (k is ILineStringResolver formatterAssigned && ((_formatter = formatterAssigned.Resolver) != null))
                {
                    IFormatString str = _formatter.ResolveFormatString(key);
                    if (str != null) return str;
                }
            }
            return StatusFormatString.Null;
        }

        /// <summary>
        /// Resolve and formulate string (applies arguments).
        /// 
        /// Tries to resolve string with each <see cref="IStringResolver"/> until result other than <see cref="LineStatus.NoResult"/> is found.
        /// 
        /// If no applicable <see cref="IStringResolver"/> is found return a value with state <see cref="LineStatus.NoResult"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If key has <see cref="ILineFormatArgs"/> part, then return the formulated string "Error (Code=0xFEEDF00D)".
        /// If key didn't have <see cref="ILineFormatArgs"/> part, then return the format string "Error (Code=0x{0:X8})".
        /// otherwise return null</returns>
        public static LineString ResolveString(this ILine key)
        {

            LineString result = new LineString(key, null, LineStatus.ResolveFailedNoStringResolver | LineStatus.CultureFailedNoResult | LineStatus.PluralityFailedNoResult | LineStatus.PlaceholderFailedNoResult | LineStatus.FormatFailedNoResult);
            for (ILine k = key; k != null; k = k.GetPreviousPart())
            {
                IStringResolver _formatter;
                if (k is ILineStringResolver formatterAssigned && ((_formatter = formatterAssigned.Resolver) != null))
                {
                    LineString str = _formatter.ResolveString(key);
                    if (str.Severity <= result.Severity) result = str;
                }
            }
            return result;
        }

        /// <summary>
        /// Resolve localization resource using the active culture. Uses the following algorithm:
        ///   1. If key has a selected culture, try that
        ///      a) from Asset
        ///   2. If key has <see cref="ICulturePolicy"/>, iterate the cultures.
        ///      a) Try asset
        ///   3. Try to read value for key from asset as is
        ///   4. Return null
        /// </summary>
        /// <param name="key"></param>
        /// <returns>resource or null</returns>
        public static byte[] ResolveResource(this ILine key)
        {
            // Arrange
            IAsset asset = key.FindAsset();
            byte[] result = null;

            // 1. Try selected culture
            CultureInfo selectedCulture = key.GetCultureInfo();
            if (selectedCulture != null)
            {
                // 1a. Try from asset
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // 2. Try culture policy
            IEnumerable<CultureInfo> cultures = key.FindCulturePolicy()?.Cultures;
            if (cultures != null)
            {
                foreach (var culture in cultures)
                {
                    // This was already tried above
                    if (culture == selectedCulture) continue;

                    // 2a. Try from asset
                    if (asset != null)
                    {
                        ILine cultured = key.Culture(culture);
                        if (cultured != null)
                        {
                            result = asset.GetResource(cultured);
                            if (result != null) return result;
                        }
                    }
                }
            }

            // 3. Try key as is
            if (asset != null)
            {
                result = asset.GetResource(key);
                if (result != null) return result;
            }

            // Not found
            return null;
        }
    }
}
