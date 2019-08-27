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
    /// Key with "Culture" parameter.
    /// </summary>
    public interface ILineCulture : ILine
    {
        /// <summary>
        /// Selected culture, or null.
        /// </summary>
        CultureInfo Culture { get; set; }
    }

    /// <summary></summary>
    public static partial class ILineExtensions
    {
        /// <summary>
        /// Append <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="culture"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine Culture(this ILine line, CultureInfo culture)
            => line.Append<ILineCulture, CultureInfo>(culture);

        /// <summary>
        /// Append culture key "Culture:xx".
        /// </summary>
        /// <param name="line"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILine Culture(this ILine line, string cultureName)
            => line.Append<ILineNonCanonicalKey, string, string>("Culture", cultureName);

        /// <summary>
        /// Create <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="culture"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key cannot be appended</exception>
        public static ILine Culture(this ILineFactory lineFactory, CultureInfo culture)
            => lineFactory.Create<ILineCulture, CultureInfo>(null, culture);

        /// <summary>
        /// Create culture key "Culture:xx".
        /// </summary>
        /// <param name="lineFactory"></param>
        /// <param name="cultureName"></param>
        /// <returns>new key</returns>
        /// <exception cref="LineException">If key doesn't implement ICultureAssignableLocalizationKey</exception>
        public static ILine Culture(this ILineFactory lineFactory, string cultureName)
            => lineFactory.Create<ILineNonCanonicalKey, string, string>(null, "Culture", cultureName);

        /// <summary>
        /// Try append <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <param name="result"></param>
        /// <returns>true if append was successful</returns>
        public static bool TryAppendCulture(this ILine key, CultureInfo culture, out ILine result)
        {
            ILineCulture _result;
            if (key.TryAppend<ILineCulture, CultureInfo>(culture, out _result)) { result = _result; return true; }
            result = default;
            return false;
        }

        /// <summary>
        /// Try append culture key "Culture:xx"
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cultureName"></param>
        /// <param name="result"></param>
        /// <returns>new key or null</returns>
        public static bool TryAppendCulture(this ILine key, string cultureName, out ILine result)
        {
            ILineNonCanonicalKey _result;
            if (key.TryAppend<ILineNonCanonicalKey, string, string>(cultureName, "Culture", out _result)) { result = _result; return true; }
            result = default;
            return false;
        }

        /// <summary>
        /// Search linked list and finds the effective (left-most) <see cref="ILineCulture"/> or <see cref="ILineParameter"/> of "Culture".
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture info</returns>
        /// <exception cref="LineException">If culture info was not found.</exception>
        public static CultureInfo GetCultureInfo(this ILine line)
        {
            CultureInfo result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = culture.Culture;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null)
                    try { result = CultureInfo.GetCultureInfo(parameter_.ParameterValue); } catch (CultureNotFoundException) { }
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null)
                            try { result = CultureInfo.GetCultureInfo(parameter.ParameterValue); } catch (CultureNotFoundException) { }
                }
            }
            return result ?? throw new LineException(line, "Could not find CultureInfo");
        }

        /// <summary>
        /// Search linked list and finds the effective (left-most) <see cref="ILineCulture"/> or <see cref="ILineParameter"/> of "Culture".
        /// </summary>
        /// <param name="line"></param>
        /// <param name="cultureInfo"></param>
        /// <returns>true if culture info was retrieved</returns>
        public static bool TryGetCultureInfo(this ILine line, out CultureInfo cultureInfo)
        {
            CultureInfo result = null;
            for (ILine l = line; l!=null; l=l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = culture.Culture;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null)
                    try { result = CultureInfo.GetCultureInfo(parameter_.ParameterValue); } catch (CultureNotFoundException) { }
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null)
                            try { result = CultureInfo.GetCultureInfo(parameter.ParameterValue); } catch (CultureNotFoundException) { }
                }
            }
            cultureInfo = result;
            return result != null;
        }

        /// <summary>
        /// Get effective (closest to root) culture value.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>culture name</returns>
        /// <exception cref="LineException">If culture info was not found.</exception>
        public static string GetCultureName(this ILine line)
        {
            String result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = culture.Culture.Name;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null) result = parameter_.ParameterValue;
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                }
            }
            return result ?? throw new LineException(line, "Could not find CultureName");
        }

        /// <summary>
        /// Get effective (closest to root) culture value.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="cultureName"></param>
        /// <returns>true if name was found</returns>
        /// <exception cref="LineException">If culture info was not found.</exception>
        public static bool TryGetCultureName(this ILine line, out string cultureName)
        {
            String result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = culture.Culture.Name;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null) result = parameter_.ParameterValue;
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null) result = parameter.ParameterValue;
                }
            }
            cultureName = result;
            return result != null;
        }

        /// <summary>
        /// Search linked list and find the effective (closest to root) culture key either <see cref="ILineCulture"/> or <see cref="ILineParameter"/> "Culture".
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Key with culture policy or null</returns>
        /// <exception cref="LineException">If culture info was not found.</exception>
        public static ILine GetCultureKey(this ILine line)
        {
            ILine result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = l;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null) result = l;
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null) { result = l; break; }
                }
            }
            return result ?? throw new LineException(line, "Could not find CultureKey");
        }

        /// <summary>
        /// Search linked list and find the effective (closest to root) culture key either <see cref="ILineCulture"/> or <see cref="ILineParameter"/> "Culture".
        /// </summary>
        /// <param name="line"></param>
        /// <param name="key"></param>
        /// <returns>true if key was found</returns>
        public static bool TryGetCultureKey(this ILine line, out ILine key)
        {
            ILine result = null;
            for (ILine l = line; l != null; l = l.GetPreviousPart())
            {
                if (l is ILineCulture culture && culture.Culture != null) result = l;
                else if (l is ILineParameter parameter_ && parameter_.ParameterName == "Culture" && parameter_.ParameterValue != null) result = l;
                else if (l is ILineParameterEnumerable lineParameters)
                {
                    foreach (var parameter in lineParameters)
                        if (parameter.ParameterName == "Culture" && parameter.ParameterValue != null) { result = l; break; }
                }
            }
            key = result;
            return result != null;
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
            => string_comparer.Equals(x?.GetCultureName(), y?.GetCultureName());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(ILine obj)
            => string_comparer.GetHashCode(obj?.GetCultureName());
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
            => string_comparer.Equals(x?.GetCultureName() ?? "", y?.GetCultureName() ?? "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(ILine obj)
            => string_comparer.GetHashCode(obj?.GetCultureName() ?? "");
    }

}
