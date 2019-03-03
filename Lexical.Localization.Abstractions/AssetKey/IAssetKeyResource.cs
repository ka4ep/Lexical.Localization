// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "Resource" parameter assignment. "Resource" describes a part of a path to assembly's embedded resource.
    /// This corresponds to "baseName" argument of System.Resources.ResourceManager.
    /// 
    /// For example, instances of this interface match to "Resource" part in the following name pattern "[assembly.][resource.]{type.}{section.}{Key}".
    /// 
    /// This parameter is mostly used with IStringLocalization of Microsoft.Extensions.Localization.Abstractions.
    /// 
    /// Consumers of this interface should use the extension method <see cref="AssetKeyExtensions.Resource(IAssetKey, string)"/>.
    /// </summary>
    public interface IAssetKeyResourceAssignable : IAssetKey
    {
        /// <summary>
        /// Create a new key where "Resource" is appended to the key.
        /// 
        /// Resource location is a hint that describes the location embedded resources of .resx files ("basename").
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        IAssetKeyResourceAssigned Resource(string resource);
    }

    /// <summary>
    /// This key is assigned with a "Resource" parameter section.
    /// 
    /// Resource in this context means a part of a path to assembly's embedded resource.
    /// For instance, resource hint matches in name pattern "[assembly.][resource.]{type.}{section.}{Key}".
    /// 
    /// This parameter mainly used with IStringLocalization of Microsoft.Extensions.Localization.Abstractions to locate .resx files.
    /// </summary>
    public interface IAssetKeyResourceAssigned : IAssetKeySection
    {
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyResourceAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ITypeAssignableLocalizationKey</exception>
        public static IAssetKeyResourceAssigned Resource(this IAssetKey key, string resource)
        {
            if (key is IAssetKeyResourceAssignable casted) return casted.Resource(resource);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyResourceAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyResourceAssigned"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyResourceAssigned TryAddResource(this IAssetKey key, String resource)
        {
            if (key is IAssetKeyResourceAssignable casted) return casted.Resource(resource);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyResourceAssigned"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyResourceAssigned FindResourceKey(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyResourceAssigned asmKey && !string.IsNullOrEmpty(key.Name)) return asmKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyResourceAssigned"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static string FindResourceName(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyResourceAssigned resKey && resKey.Name != null) return resKey.Name;
                key = key.GetPreviousKey();
            }
            return null;
        }

    }
}
