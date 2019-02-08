// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Key has capability of "resource" parameter assignment. "resource" describes a part of a path to assembly's embedded resource.
    /// This corresponds to "baseName" argument of System.Resources.ResourceManager.
    /// 
    /// For example, instances of this interface match to "resource" part in the following name pattern "[assembly.][resource.]{type.}{section.}{key}".
    /// 
    /// This parameter is mostly used with IStringLocalization of Microsoft.Extensions.Localization.Abstractions.
    /// 
    /// Consumers of this interface should use the extension method <see cref="AssetKeyExtensions.ResourceSection(IAssetKey, string)"/>.
    /// </summary>
    public interface IAssetKeyResourceSectionAssignable : IAssetKey
    {
        /// <summary>
        /// Add resource location to the key.
        /// 
        /// Resource location is a hint that describes the location embedded resources of .resx files ("basename").
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        [AssetKeyConstructor(parameterName: "resource")]
        IAssetKeyResourceSection ResourceSection(string resource);
    }

    /// <summary>
    /// This key is assigned with a "resource" parameter section.
    /// 
    /// Resource in this context means a part of a path to assembly's embedded resource.
    /// For instance, resource hint matches in name pattern "[assembly.][resource.]{type.}{section.}{key}".
    /// 
    /// This parameter mainly used with IStringLocalization of Microsoft.Extensions.Localization.Abstractions to locate .resx files.
    /// </summary>
    [AssetKeyParameter(parameterName: "resource")]
    public interface IAssetKeyResourceSection : IAssetKeySection
    {
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Add <see cref="IAssetKeyResourceSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns>new key</returns>
        /// <exception cref="AssetKeyException">If key doesn't implement ITypeAssignableLocalizationKey</exception>
        public static IAssetKeyResourceSection ResourceSection(this IAssetKey key, string resource)
        {
            if (key is IAssetKeyResourceSectionAssignable casted) return casted.ResourceSection(resource);
            throw new AssetKeyException(key, $"doesn't implement {nameof(IAssetKeyResourceSectionAssignable)}.");
        }

        /// <summary>
        /// Try to add <see cref="IAssetKeyResourceSection"/> section.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns>new key or null</returns>
        public static IAssetKeyResourceSection TryAddResourceSection(this IAssetKey key, String resource)
        {
            if (key is IAssetKeyResourceSectionAssignable casted) return casted.ResourceSection(resource);
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyResourceSection"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static IAssetKeyResourceSection FindResourceSection(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyResourceSection asmKey && !string.IsNullOrEmpty(key.Name)) return asmKey;
                key = key.GetPreviousKey();
            }
            return null;
        }

        /// <summary>
        /// Get previous <see cref="IAssetKeyResourceSection"/> that has a resolved Assembky.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type">type value to search</param>
        /// <returns>type key with type or null</returns>
        public static string FindResourceSectionName(this IAssetKey key)
        {
            while (key != null)
            {
                if (key is IAssetKeyResourceSection resKey && resKey.Name != null) return resKey.Name;
                key = key.GetPreviousKey();
            }
            return null;
        }

    }
}
