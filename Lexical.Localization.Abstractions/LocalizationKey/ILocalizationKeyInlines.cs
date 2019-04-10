// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of inline assignment.
    /// </summary>
    public interface ILocalizationKeyInlineAssignable : IAssetKey
    {
        /// <summary>
        /// Add <see cref="ILocalizationKeyInlines"/> part to the key. 
        /// </summary>
        /// <returns>key with inlines (non-null dictionary)</returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        ILocalizationKeyInlines AddInlines();
    }

    /// <summary>
    /// Key that has inline value assignments.
    /// </summary>
    public interface ILocalizationKeyInlines : ILocalizationKey, IDictionary<IAssetKey, IFormulationString>
    {
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Add an inlined language string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text">text to add, or null to remove</param>
        /// <returns>new key with inliens or <paramref name="key"/></returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        public static ILocalizationKeyInlines Inline(this IAssetKey key, string text)
        {
            ILocalizationKeyInlines inlinesKey = key.GetOrCreateInlines();
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
        public static IAssetKey Inline(this IAssetKey key, string subKeyText, string text)
        {
            ILocalizationKeyInlines inlinesKey = key.GetOrCreateInlines();
            IAssetKey subKey = ParameterParser.Instance.Parse(subKeyText, key);
            if (text == null) inlinesKey.Remove(subKey); else inlinesKey[subKey] = LexicalStringFormat.Instance.Parse(text);
            return inlinesKey;
        }

        /// <summary>
        /// Finds in <see cref="ILocalizationKeyInlines"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ILocalizationKeyInlines FindInlines(this IAssetKey key)
        {
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                if (k is ILocalizationKeyInlines inlinesKey) return inlinesKey;
            return null;
        }

        /// <summary>
        /// Get or create <see cref="ILocalizationKeyInlines"/> section in the <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines key</returns>
        /// <exception cref="AssetKeyException">If <paramref name="key"/> doesn't implement <see cref="ILocalizationKeyInlineAssignable"/></exception>
        public static ILocalizationKeyInlines GetOrCreateInlines(this IAssetKey key)
            => key.FindInlines() ?? 
               (key is ILocalizationKeyInlineAssignable assignable ? 
                assignable.AddInlines() : 
                throw new AssetKeyException(key, $"Doesn't implement {nameof(ILocalizationKeyInlineAssignable)}"));

        /// <summary>
        /// Walks linked list and searches for all inlines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines</returns>
        public static IEnumerable<ILocalizationKeyInlines> FindAllInlines(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyInlines casted)
                    yield return casted;
        }

    }
}
