// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// Signals that the class gives asset specific parameters, hints or capabilities.
    /// 
    /// Parameters:
    ///     <see cref="IAssetKeyAssemblySection"/>
    ///     <see cref="IAssetKeyLocationSection"/>
    ///     <see cref="IAssetKeyResourceSection"/>
    ///     <see cref="IAssetKeyTypeSection"/>
    ///     <see cref="IAssetKeySectionAssigned"/>
    ///     <see cref="IAssetKeyAssigned"/>
    ///     
    /// Hints:
    ///     <see cref="IAssetKeyAssetAssigned"/>
    ///     
    /// Capabilities:
    ///     <see cref="IAssetKeyAssemblySectionAssignable"/>
    ///     <see cref="IAssetKeyAssetAssignable"/>
    ///     <see cref="IAssetKeyAssignable"/>
    ///     <see cref="IAssetKeyLocationSectionAssignable"/>
    ///     <see cref="IAssetKeyResourceSectionAssignable"/>
    ///     <see cref="IAssetKeySectionAssignable"/>
    ///     <see cref="IAssetKeyTypeSectionAssignable"/>
    ///  
    /// Others:
    ///     <see cref="IAssetRoot"/>
    ///     <see cref="IAssetKeyLinked"/>
    /// 
    /// </summary>
    public interface IAssetKey
    {
        /// <summary>
        /// Local identity of the key. For example "ConsoleApp1.MyController" or "Error1".
        /// 
        /// Name property is used by other decending interfaces for the string representation of their essential value, 
        /// such as type name, or assembly name.
        /// </summary>
        String Name { get; }
    }

    public static partial class AssetKeyExtensions
    {
        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(IAssetKey)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        /// <exception cref="AssetKeyException">If resolving failed or resolver was not found</exception>
        public static byte[] GetResource(this IAssetKey key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new AssetKeyException(key, "String resolver was not found.");
            byte[] data = asset.GetResource(key);
            if (data == null) throw new AssetKeyException(key, $"String {key} was not found.");
            return data;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(IAssetKey)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        public static byte[] TryGetResource(this IAssetKey key)
            => key.FindAsset()?.GetResource(key);

    }

}
