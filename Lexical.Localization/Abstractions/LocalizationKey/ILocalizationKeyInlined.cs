// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of inline assignment.
    /// </summary>
    public interface ILocalizationKeyInlineAssignable : IAssetKey
    {
        /// <summary>
        /// Add inlined culture specific string.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="text">(optional) text, if null, removes previous text</param>
        /// <returns>key with inline assignment (new or same key)</returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        ILocalizationKeyInlined Inline(string culture, string text);
    }

    /// <summary>
    /// Key (may have) has inlining assigned.
    /// </summary>
    public interface ILocalizationKeyInlined : IAssetKeyNonCanonicallyCompared, ILocalizationKey
    {
        /// <summary>
        /// Get inlined strings.
        /// Returns null if there are none. 
        /// </summary>
        IDictionary<string, string> Inlines { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Add inlined language strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        public static ILocalizationKeyInlined Inline(this IAssetKey key, string culture, string text)
            => key is ILocalizationKeyInlineAssignable inlined ? inlined.Inline(culture, text) : throw new AssetKeyException(key, $"Key doesn't implement {nameof(ILocalizationKeyInlineAssignable)}");

        /// <summary>
        /// Add inlined language strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="AssetKeyException">If key can't be inlined.</exception>
        public static ILocalizationKeyInlined Inline(this IAssetKey key, string text)
            => key is ILocalizationKeyInlineAssignable inlined ? inlined.Inline("", text) : throw new AssetKeyException(key, $"Key doesn't implement {nameof(ILocalizationKeyInlineAssignable)}");

        /// <summary>
        /// Walks linked list and searches for previous inlines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines or null</returns>
        public static IDictionary<string, string> FindInlines(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyInlined casted && casted.Inlines != null) return casted.Inlines;
            return null;
        }

        /// <summary>
        /// Walks linked list and searches for all inlines.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>inlines</returns>
        public static IEnumerable<IDictionary<string, string>> FindAllInlines(this IAssetKey key)
        {
            for (; key != null; key = key.GetPreviousKey())
                if (key is ILocalizationKeyInlined casted && casted.Inlines != null)
                    yield return casted.Inlines;
        }

    }
}
