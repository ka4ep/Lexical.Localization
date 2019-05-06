// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Culture" parameter assignment.
    /// </summary>
    [Obsolete]
    public interface ILocalizationKeyCultureAssignable : ILine
    {
        /// <summary>
        /// Select a specific culture. 
        /// 
        /// Adds <see cref="ILineKeyCulture"/> link.
        /// </summary>
        /// <param name="culture">Name for new sub key.</param>
        /// <returns>new key</returns>
        ILineKeyCulture Culture(CultureInfo culture);

        /// <summary>
        /// Set to a specific culture
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        /// <exception cref="CultureNotFoundException">if culture was not found</exception>
        ILineKeyCulture Culture(string cultureName);
    }

    /// <summary>
    /// Key (may have) has "Culture" parameter assigned.
    /// </summary>
    public interface ILineCulture : ILine
    {
        /// <summary>
        /// Selected culture, or null.
        /// </summary>
        CultureInfo Culture { get; set; }
    }

    /// <summary>
    /// Key (may have) has "Culture" parameter assigned.
    /// </summary>
    public interface ILineKeyCulture : ILineCulture, ILine
    {
    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine Culture(this ILine key, CultureInfo culture)
            => key.Append<ILineKeyCulture, CultureInfo>(culture);

        /// <summary>
        /// Append culture key "Culture:xx".
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILine Culture(this ILine key, string cultureName)
            => key.Append<ILineKeyNonCanonicallyCompared, string, string>("Culture", cultureName);

        /// <summary>
        /// Try append <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>new key or null</returns>
        public static ILine TryAppendCulture(this ILine key, CultureInfo culture)
            => key.TryAppend<ILineKeyCulture, CultureInfo>(culture);

        /// <summary>
        /// Try append culture key "Culture:xx"
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key or null</returns>
        public static ILine TryAppendCulture(this ILine key, string cultureName)
            => key.TryAppend<ILineKeyNonCanonicallyCompared, string, string>("Culture", cultureName);

        /// <summary>
        /// Search linked list and finds the selected (left-most) <see cref="ILineKeyCulture"/> key.
        /// 
        /// If implements <see cref="ILineCulture"/> returns the culture. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture info or null</returns>
        public static CultureInfo GetCultureInfo(this ILine line)
        {
            

            if (line is ILineParameterEnumerable lineParameters)
            {
                var keys = lineParameters.Parameters;
                if (keys != null)
                    foreach (var kv in keys)
                        if (kv.Key == "Culture" && kv.Value != null) try { return CultureInfo.GetCultureInfo(kv.Value); } catch (CultureNotFoundException) { }
            }

            if (line is ILine part)
            {
                string cultureName = null;
                CultureInfo culture = null;
                for (ILine l = line; l != null; l = l.GetPreviousPart())
                {
                    if (l is ILineCulture lineCulture && lineCulture.Culture != null) culture = lineCulture.Culture;
                    else if (l is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) cultureName = parameterKey.ParameterValue;
                }
                if (culture != null) return culture;
                if (cultureName != null) try { return CultureInfo.GetCultureInfo(cultureName); } catch (CultureNotFoundException) { }
            }

            return null;
        }

        /// <summary>
        /// Get effective (closest to root) culture value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture name or null</returns>
        public static string GetCulture(this ILine line)
        {
            if (line is ILineCulture lineCulture && lineCulture.Culture != null) return lineCulture.Culture.Name;

            if (line is ILineParameterEnumerable lineParameters)
            {
                var keys = lineParameters.Parameters;
                if (keys != null)
                    foreach (var kv in keys)
                        if (kv.Key == "Culture" && kv.Value != null) return kv.Value;
            }

            if (line is ILine part)
            {
                string result = null;
                for (ILine p = part; p != null; p = p.GetPreviousPart())
                {
                    if (p is ILineKeyCulture cultureKey && cultureKey.Culture != null) result = cultureKey.Culture.Name;
                    else if (p is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) result = parameterKey.ParameterValue;
                }
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        /// Search linked list and find the effective (closest to root) culture key either <see cref="ILineKeyCulture"/> or <see cref="ILineParameter"/>.
        /// </summary>
        /// <param name="tail"></param>
        /// <returns>Key with culture policy or null</returns>
        public static ILine GetCultureKey(this ILine tail)
        {
            ILine result = null;
            for (; tail != null; tail = tail.GetPreviousPart())
            {
                if (tail is ILineKeyCulture cultureKey && cultureKey.Culture != null) result = cultureKey;
                else if (tail is ILineParameter parameterKey && parameterKey.ParameterName == "Culture" && parameterKey.ParameterValue != null) result = parameterKey;
            }
            return result;
        }

    }

    /// <summary>
    /// Compares <see cref="ILineCulture"/> values of lines.
    /// 
    /// This comparer regards null and "" non-equivalent. 
    /// </summary>
    public class LineCultureComparer : IEqualityComparer<ILine>
    {
        static IEqualityComparer<string> string_comparer = StringComparer.InvariantCulture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
            => string_comparer.Equals(x?.GetCulture(), y?.GetCulture());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(ILine obj)
            => string_comparer.GetHashCode(obj?.GetCulture());
    }

    /// <summary>
    /// comparer that compares <see cref="ILineCulture"/> values of lines.
    /// 
    /// This comparer regards null and "" equivalent. 
    /// </summary>
    public class LineCultureComparer2 : IEqualityComparer<ILine>
    {
        static IEqualityComparer<string> string_comparer = StringComparer.InvariantCulture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ILine x, ILine y)
            => string_comparer.Equals(x?.GetCulture() ?? "", y?.GetCulture() ?? "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(ILine obj)
            => string_comparer.GetHashCode(obj?.GetCulture() ?? "");
    }

}
