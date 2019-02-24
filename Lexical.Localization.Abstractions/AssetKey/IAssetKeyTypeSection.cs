// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// 
    /// Consumers of this interface should use the extension method <see cref="AssetKeyExtensions.TypeSection(IAssetKey, string)"/> and others.
    /// </summary>
    public interface IAssetKeyTypeSectionAssignable : IAssetKey
    {
        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="name">type</param>
        /// <returns>new key</returns>
        IAssetKeyTypeSection TypeSection(string name);

        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>new key</returns>
        IAssetKeyTypeSection TypeSection(Type type);

        /// <summary>
        /// Create type section for specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>new key</returns>
        IAssetKey<T> TypeSection<T>();
    }

    /// <summary>
    /// Key (may have) has "type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// </summary>
    public interface IAssetKeyTypeSection : IAssetKeySection
    {
        /// <summary>
        /// Type, or null.
        /// </summary>
        Type Type { get; }
    }

    /// <summary>
    /// Type section for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAssetKey<T> : IAssetKeyTypeSection
    {
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyTypeSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeSectionAssignable"/></exception>
        public static IAssetKeyTypeSection TypeSection(this IAssetKey key, string name)
        {
            if (key is IAssetKeyTypeSectionAssignable casted) return casted.TypeSection(name);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeSectionAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyTypeSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeSectionAssignable"/></exception>
        public static IAssetKeyTypeSection TypeSection(this IAssetKey key, Type type)
        {
            if (key is IAssetKeyTypeSectionAssignable casted) return casted.TypeSection(type);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeSectionAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyTypeSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeSectionAssignable"/></exception>
        public static IAssetKey<T> TypeSection<T>(this IAssetKey key)
        {
            if (key is IAssetKeyTypeSectionAssignable casted) return casted.TypeSection<T>();
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeSectionAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyTypeSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyTypeSection TryCreateTypeSection(this IAssetKey key, Type type)
        {
            if (key is IAssetKeyTypeSectionAssignable casted) return casted.TypeSection(type);
            return null;
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyTypeSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyTypeSection TryCreateTypeSection(this IAssetKey key, string type)
        {
            if (key is IAssetKeyTypeSectionAssignable casted) return casted.TypeSection(type);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyTypeSection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyTypeSection FindTypeSection(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeSection typeKey && typeKey.Type != null) return typeKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyTypeSection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyTypeSection FindTypeSection(this IAssetKey key, Type type)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeSection typeKey && typeKey.Type == type) return typeKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get first <see cref="IAssetKeyTypeSection"/> key that has a Type.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>type or null</returns>
        public static Type FindType(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeSection typeKey && typeKey.Type != null) return typeKey.Type;
                key = key.GetPreviousKey();
            }
            return null;
        }

    }
}
