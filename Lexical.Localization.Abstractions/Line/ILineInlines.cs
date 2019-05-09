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
        /// Add an inlined language string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text">text to add, or null to remove</param>
        /// <returns>new key with inliens or <paramref name="key"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILineInlines Inline(this ILine key, string text)
        {
            ILineInlines inlinesKey = key.GetOrCreateInlines();
            if (text == null) inlinesKey.Remove(key); else inlinesKey[key] = CSharpFormat.Instance.Parse(text);
            return inlinesKey;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="text"></param>
        /// <returns>new key with inliens or <paramref name="key"/></returns>
        /// <exception cref="LineException">If key can't be inlined.</exception>
        public static ILine Inline(this ILine key, string subKeyText, string text)
        {
            ILineInlines inlinesKey = key.GetOrCreateInlines();
            ILine subKey = ParameterParser.Instance.Parse(subKeyText, key);
            if (text == null) inlinesKey.Remove(subKey); else inlinesKey[subKey] = CSharpFormat.Instance.Parse(text);
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
            => line.FindInlines() ?? 
               (line is ILineInlines assignable ? 
                assignable.AddInlines() : 
                throw new LineException(line, $"Doesn't implement {nameof(ILineInlines)}"));

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
