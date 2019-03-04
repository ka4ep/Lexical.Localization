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
        /// Add <see cref="ILocalizationKeyInlined"/> part to the key. 
        /// </summary>
        /// <returns>key with inlines (non-null dictionary)</returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        ILocalizationKeyInlined AddInlines();
    }

    /// <summary>
    /// Key that (may have) has inlining dictionary assigned.
    /// </summary>
    public interface ILocalizationKeyInlined : IAssetKeyNonCanonicallyCompared, ILocalizationKey
    {
        /// <summary>
        /// The assigned inlining dictionary. Or null if there is none assigned.
        /// </summary>
        IDictionary<IAssetKey, string> Inlines { get; }
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
        public static ILocalizationKeyInlined Inline(this IAssetKey key, string text)
        {
            ILocalizationKeyInlined inlinesKey = key.GetOrCreateInlinesKey();
            IDictionary<IAssetKey, string> inlines = inlinesKey.Inlines;
            if (text == null) inlines.Remove(key); else inlines[key] = text;
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
            ILocalizationKeyInlined inlinesKey = key.GetOrCreateInlinesKey();
            IDictionary<IAssetKey, string> inlines = inlinesKey.Inlines;
            IAssetKey subKey = ParameterNamePolicy.Instance.Parse(subKeyText, key);
            if (text == null) inlines.Remove(subKey); else inlines[subKey] = text;
            return inlinesKey;
        }

        /// <summary>
        /// Finds in <see cref="ILocalizationKeyInlined"/> that has a non-null <see cref="ILocalizationKeyInlined.Inlines"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ILocalizationKeyInlined FindInlinesKey(this IAssetKey key)
        {
            for (IAssetKey k = key; k != null; k = k.GetPreviousKey())
                if (k is ILocalizationKeyInlined inlinesKey && inlinesKey.Inlines != null) return inlinesKey;
            return null;
        }

        /// <summary>
        /// Get or create <see cref="ILocalizationKeyInlined"/> section in the <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines key</returns>
        /// <exception cref="AssetKeyException">If <paramref name="key"/> doesn't implement <see cref="ILocalizationKeyInlineAssignable"/></exception>
        public static ILocalizationKeyInlined GetOrCreateInlinesKey(this IAssetKey key)
            => key.FindInlinesKey() ?? 
               (key is ILocalizationKeyInlineAssignable assignable ? 
                assignable.AddInlines() : 
                throw new AssetKeyException(key, $"Doesn't implement {nameof(ILocalizationKeyInlineAssignable)}"));

        /// <summary>
        /// Get or create inlines dictionary for <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines dictionary</returns>
        public static IDictionary<IAssetKey, string> GetOrCreateInlines(this IAssetKey key)
            => key.GetOrCreateInlinesKey().Inlines;

        /// <summary>
        /// Walks linked list and searches for previous inlines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines or null</returns>
        public static IDictionary<IAssetKey, string> FindInlines(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyInlined casted && casted.Inlines != null) return casted.Inlines;
            return null;
        }

    }
}
