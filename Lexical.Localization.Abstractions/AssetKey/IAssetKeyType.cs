// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// 
    /// Consumers of this interface should use the extension method <see cref="ILinePartExtensions.Type(ILinePart, string)"/> and others.
    /// </summary>
    public interface IAssetKeyTypeAssignable : ILinePart
    {
        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="name">type</param>
        /// <returns>new key</returns>
        IAssetKeyTypeAssigned Type(string name);

        /// <summary>
        /// Create type section key for specific type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>new key</returns>
        IAssetKeyTypeAssigned Type(Type type);

        /// <summary>
        /// Create type section for specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>new key</returns>
        IAssetKey<T> Type<T>();
    }

    /// <summary>
    /// Key (may have) has "Type" parameter assignment.
    /// 
    /// Type parameters are used with physical files and embedded resources.
    /// </summary>
    public interface IAssetKeyTypeAssigned : IAssetKeySection
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
    public interface IAssetKey<T> : IAssetKeyTypeAssigned
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyTypeAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeAssignable"/></exception>
        public static IAssetKeyTypeAssigned Type(this ILinePart key, string name)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Type(name);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyTypeAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeAssignable"/></exception>
        public static IAssetKeyTypeAssigned Type(this ILinePart key, Type type)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Type(type);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeAssignable)}.");
        }

        /// <summary>
        /// Add <see cref="IAssetKeyTypeAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement <see cref="IAssetKeyTypeAssignable"/></exception>
        public static IAssetKey<T> Type<T>(this ILinePart key)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Type<T>();
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyTypeAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyTypeAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyTypeAssigned TrySetType(this ILinePart key, Type type)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Type(type);
            return null;
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyTypeAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyTypeAssigned TrySetType(this ILinePart key, string type)
        {
            if (key is IAssetKeyTypeAssignable casted) return casted.Type(type);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyTypeAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyTypeAssigned FindTypeKey(this ILinePart key)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeAssigned typeKey && typeKey.Type != null) return typeKey;
                key = key.PreviousPart;
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyTypeAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyTypeAssigned FindTypeKey(this ILinePart key, Type type)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeAssigned typeKey && typeKey.Type == type) return typeKey;
                key = key.PreviousPart;
            }
            return null;
        }

        /// <summary>
        /// Get first <see cref="IAssetKeyTypeAssigned"/> key that has a Type.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>type or null</returns>
        public static Type FindType(this ILinePart key)
        {
            while (key != null)
            {
                if (key is IAssetKeyTypeAssigned typeKey && typeKey.Type != null) return typeKey.Type;
                key = key.PreviousPart;
            }
            return null;
        }

    }
}
