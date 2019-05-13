﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Line that has multiple subline assignments.
    /// 
    /// This interface intherits dictionary that contains subline assignments.
    /// The Key of the dictionary is used for comparing, and should use default LineComparer.
    /// The Value of the dictionary contains the full subline with value.
    /// </summary>
    public interface ILineInlines : ILine, IDictionary<ILine, ILine>
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKey">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="valueText">(optional) value to add, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, ILine subKey, string valueText)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Concat(subKey);
            if (valueText == null)
            {
                inlines.Remove(subline);
            }
            else
            {
                IStringFormat stringFormat = subline.FindStringFormat(StringFormatResolver.Default) ?? CSharpFormat.Instance;
                IFormulationString formulation = stringFormat.Parse(valueText);
                ILine value = subline.Value(formulation);
                inlines[subline] = value;
            }
            return line;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKey">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="value">(optional) value to append, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, ILine subKey, IFormulationString value)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Concat(subKey);
            if (value == null)
            {
                inlines.Remove(subline);
            }
            else
            {
                inlines[subline] = subline.Value(value);
            }
            return line;
        }

        /// <summary>
        /// Inline <paramref name="valueText"/> to <paramref name="culture"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="culture">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="valueText">value to add</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine InlineCulture(this ILine line, string culture, string valueText)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Culture(culture);
            IStringFormat stringFormat = subline.FindStringFormat(StringFormatResolver.Default) ?? CSharpFormat.Instance;
            IFormulationString formulation = stringFormat.Parse(valueText);
            ILine value = subline.Value(formulation);
            inlines[subline] = value;
            return line;
        }

        /// <summary>
        /// Inline <paramref name="value"/> to <paramref name="culture"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="culture">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="value">(optional) value to append, if null removes previously existing the inline</param>
        /// <returns>new line with inlines or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine InlineCulture(this ILine line, string culture, IFormulationString value)
        {
            ILineInlines inlines;
            line = line.GetOrCreateInlines(out inlines);
            ILine subline = line.Culture(culture);
            inlines[subline] = subline.Value(value);
            return line;
        }

        /// <summary>
        /// Finds in <see cref="ILineInlines"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static ILineInlines FindInlines(this ILine line)
        {
            for (ILine k = line; k != null; k = k.GetPreviousPart())
                if (k is ILineInlines inlinesKey) return inlinesKey;
            return null;
        }

        /// <summary>
        /// Get or create <see cref="ILineInlines"/> section in the <paramref name="line"/>.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inlines">inlines with dictionary</param>
        /// <returns><paramref name="line"/> or inlines key</returns>
        /// <exception cref="LineException">If <paramref name="line"/> doesn't implement <see cref="ILineInlines"/></exception>
        public static ILine GetOrCreateInlines(this ILine line, out ILineInlines inlines)
        {
            ILineInlines _inlines = line.FindInlines();
            if (_inlines != null) { inlines = _inlines; return line; }
            inlines = _inlines = line.Append<ILineInlines>();
            return _inlines;
        }

        /// <summary>
        /// Walks linked list and searches for all inlines.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>inlines</returns>
        public static IEnumerable<ILineInlines> FindAllInlines(this ILine line)
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is ILineInlines casted)
                    yield return casted;
        }

    }
}