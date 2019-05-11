// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Key that has multiple value assignments.
    /// </summary>
    public interface ILineInlines : ILine, IDictionary<ILine, IFormulationString>
    {
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="text"></param>
        /// <returns>new key with inliens or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, string subKeyText, string text)
        {
            ILineInlines inlinesKey = line.GetOrCreateInlines();
            ILine subKey = LineFormat.Instance.Parse(subKeyText, line);
            if (text == null)
            {
                inlinesKey.Remove(subKey);
            }
            else
            {
                IStringFormat stringFormat = line.FindStringFormat(/*StringFormatResolver.Default*/) ?? CSharpFormat.Instance;
                inlinesKey[subKey] = stringFormat.Parse(text);
            }
            return inlinesKey;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="text"></param>
        /// <returns>new key with inliens or <paramref name="line"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine line, string subKeyText, IFormulationString text)
        {
            ILineInlines inlinesKey = line.GetOrCreateInlines();
            ILine subKey = LineFormat.Instance.Parse(subKeyText, line);
            if (text == null) inlinesKey.Remove(subKey); else inlinesKey[subKey] = text;
            return inlinesKey;
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
        /// <returns>inlines key</returns>
        /// <exception cref="LineException">If <paramref name="line"/> doesn't implement <see cref="ILineInlines"/></exception>
        public static ILineInlines GetOrCreateInlines(this ILine line)
            => line.FindInlines() ?? line.Append<ILineInlines>();

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
