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
    ///     <see cref="ILineKeyAssembly"/>
    ///     <see cref="IAssetKeyLocationAssigned"/>
    ///     <see cref="IAssetKeyResourceAssigned"/>
    ///     <see cref="ILineKeyType"/>
    ///     <see cref="IAssetKeySectionAssigned"/>
    ///     <see cref="IAssetKeyAssigned"/>
    ///     
    /// Hints:
    ///     <see cref="IAssetKeyAssetAssigned"/>
    ///     
    /// Capabilities:
    ///     <see cref="IAssetKeyAssemblyAssignable"/>
    ///     <see cref="IAssetKeyAssetAssignable"/>
    ///     <see cref="IAssetKeyAssignable"/>
    ///     <see cref="IAssetKeyLocationAssignable"/>
    ///     <see cref="IAssetKeyResourceAssignable"/>
    ///     <see cref="IAssetKeySectionAssignable"/>
    ///     <see cref="IAssetKeyTypeAssignable"/>
    ///  
    /// Others:
    ///     <see cref="IAssetRoot"/>
    ///     <see cref="ILinePart"/>
    /// 
    /// </summary>
    public partial interface ILinePart
    {
    }

    public static partial class ILinePartExtensions
    {
        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILinePart)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        /// <exception cref="LineException">If resolving failed or resolver was not found</exception>
        public static byte[] GetResource(this ILinePart key)
        {
            IAsset asset = key.FindAsset();
            if (asset == null) throw new LineException(key, "String resolver was not found.");
            byte[] data = asset.GetResource(key);
            if (data == null) throw new LineException(key, $"String {key} was not found.");
            return data;
        }

        /// <summary>
        /// Find <see cref="IAsset"/> and get language string.
        /// Ignores culture policy, ignores inlining, ignores formatting.
        /// 
        /// <see cref="ResolveString(ILinePart)"/> to resolve string with active culture from <see cref="ICulturePolicy"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>byte[] or null</returns>
        public static byte[] TryGetResource(this ILinePart key)
            => key.FindAsset()?.GetResource(key);

    }

}
