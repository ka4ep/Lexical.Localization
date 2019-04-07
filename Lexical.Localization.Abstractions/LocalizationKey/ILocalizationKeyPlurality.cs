// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// A key where plurality can been assigned.
    /// 
    /// The parameter name is "N" if argumentIndex = 0, and "N1" for 1, "N2", etc.
    /// </summary>
    public interface ILocalizationKeyPluralityAssignable : ILocalizationKey
    {
        /// <summary>
        /// Assign plurality to the key on argument <paramref name="argumentIndex"/> as <paramref name="category"/>. 
        /// 
        /// See <see cref="Plurality"/> for constants.
        /// </summary>
        /// <param name="argumentIndex">argument index to configure plurality, e.g. 0 = "{0}" in the format string</param>
        /// <param name="category">"Zero", "One", "Plural", reflects to the Name of constructed the key.</param>
        /// <returns>assigned key</returns>
        ILocalizationKeyPluralityAssigned N(int argumentIndex, string category);
    }

    /// <summary>
    /// A key where plurality has been assigned.
    /// </summary>
    public interface ILocalizationKeyPluralityAssigned : ILocalizationKey
    {
        /// <summary>
        /// Argument index this plurality is a rule to.
        /// </summary>
        int ArgumentIndex { get; }
    }

    public static partial class LocalizationKeyExtensions
    {
        /// <summary>
        /// Assign plurality kind to argument <paramref name="argumentIndex"/>.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="argumentIndex">argument index to configure plurality, e.g. 0 = "{0}" in the format string</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N(this IAssetKey key, int argumentIndex, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(argumentIndex, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 0.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(0, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 1.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N1(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(1, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 2.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N2(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(2, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 3.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N3(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(3, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 4.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N4(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(4, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 5.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N5(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(5, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 6.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N6(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(6, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 7.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N7(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(7, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 8.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N8(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(8, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Assign plurality kind to argument 9.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="category"></param>
        /// <returns>key with assigned plurality</returns>
        /// <exception cref="AssetKeyException">if key cannot be assigned.</exception>
        public static ILocalizationKeyPluralityAssigned N9(this IAssetKey key, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(9, category) : throw new AssetKeyException(key, $"doesn't implement {nameof(ILocalizationKeyPluralityAssignable)}.");

        /// <summary>
        /// Try to set to a specific culture.
        /// </summary>
        /// <param name="key">key to append to</param>
        /// <param name="argumentIndex">argument index to configure plurality, e.g. 0 = "{0}" in the format string</param>
        /// <param name="category">e.g. "Zero", "One", "Plural", reflects to the Name of constructed the key.</param>
        /// <returns>new key or null</returns>
        public static ILocalizationKeyPluralityAssigned TrySetPlurality(this IAssetKey key, int argumentIndex, string category)
            => key is ILocalizationKeyPluralityAssignable assignable ? assignable.N(argumentIndex, category) : null;

    }


}
