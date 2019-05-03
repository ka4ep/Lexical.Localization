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
    /// Key has capability of inline assignment.
    /// </summary>
    [Obsolete]
    public interface ILineInlinesAssigned : ILinePart
    {
        /// <summary>
        /// Add <see cref="ILineInlines"/> part to the key. 
        /// </summary>
        /// <returns>key with inlines (non-null dictionary)</returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        ILineInlines AddInlines();
    }

    /// <summary>
    /// Key that has multiple value assignments.
    /// </summary>
    public interface ILineInlines : ILinePart, IDictionary<ILinePart, IFormulationString>
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Add an inlined language string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text">text to add, or null to remove</param>
        /// <returns>new key with inliens or <paramref name="key"/></returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        public static ILineInlines Inline(this ILinePart key, string text)
        {
            ILineInlines inlinesKey = key.GetOrCreateInlines();
            if (text == null) inlinesKey.Remove(key); else inlinesKey[key] = LexicalStringFormat.Instance.Parse(text);
            return inlinesKey;
        }

        /// <summary>
        /// Add inlined language string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKeyText">subkey in parametrized format, e.g. "Culture:en", or "Culture:en:N:One"</param>
        /// <param name="text"></param>
        /// <returns>new key with inliens or <paramref name="key"/></returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        public static ILinePart Inline(this ILinePart key, string subKeyText, string text)
        {
            ILineInlines inlinesKey = key.GetOrCreateInlines();
            ILinePart subKey = ParameterParser.Instance.Parse(subKeyText, key);
            if (text == null) inlinesKey.Remove(subKey); else inlinesKey[subKey] = LexicalStringFormat.Instance.Parse(text);
            return inlinesKey;
        }

        /// <summary>
        /// Finds in <see cref="ILineInlines"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ILineInlines FindInlines(this ILinePart key)
        {
            for (ILinePart k = key; k != null; k = k.PreviousPart)
                if (k is ILineInlines inlinesKey) return inlinesKey;
            return null;
        }

        /// <summary>
        /// Get or create <see cref="ILineInlines"/> section in the <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines key</returns>
        /// <exception cref="AssetKeyException">If <paramref name="key"/> doesn't implement <see cref="ILineInlinesAssigned"/></exception>
        public static ILineInlines GetOrCreateInlines(this ILinePart key)
            => key.FindInlines() ?? 
               (key is ILineInlinesAssigned assignable ? 
                assignable.AddInlines() : 
                throw new AssetKeyException(key, $"Doesn't implement {nameof(ILineInlinesAssigned)}"));

        /// <summary>
        /// Walks linked list and searches for all inlines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines</returns>
        public static IEnumerable<ILineInlines> FindAllInlines(this ILinePart key)
        {
            for (; key != null; key = key.PreviousPart)
                if (key is ILineInlines casted)
                    yield return casted;
        }

    }
}
