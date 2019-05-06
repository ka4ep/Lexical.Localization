// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;

namespace Lexical.Localization
{
    /// <summary>
    /// A key where <see cref="IAsset"/> can be assigned. (Typically a mutable root.)
    /// </summary>
    public interface IAssetKeyAssetAssignable : ILine
    {
        /// <summary>
        /// Set localization asset.
        /// </summary>
        /// <param name="asset">localization asset</param>
        /// <returns>key (most likely self)</returns>
        /// <exception cref="InvalidOperationException">If object is read-only</exception>
        IAssetKeyAssetAssigned SetAsset(IAsset asset);
    }

    /// <summary>
    /// Key has <see cref="IAsset"/> hint assigned.
    /// </summary>
    public interface IAssetKeyAssetAssigned : ILine
    {
        /// <summary>
        /// Object that contains localization assets.
        /// </summary>
        IAsset Asset { get; }
    }

    public static partial class ILineExtensions
    {
        /// <summary>
        /// Try to set localization asset. Doesn't throw expected exception.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="localizationResources"></param>
        /// <returns>key or null if failed.</returns>
        public static IAssetKeyAssetAssigned TrySetAsset(this IAssetKeyAssetAssignable key, IAsset localizationResources)
        {
            try
            {
                return key.SetAsset(localizationResources);
            }
            catch (InvalidOperationException) {
                return null;
            }
        }

        /// <summary>
        /// Find key where asset may be assigned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>key or null</returns>
        public static IAssetKeyAssetAssignable FindAssetAssignable(this ILine key)
            => key.Find<IAssetKeyAssetAssignable>();

        /// <summary>
        /// Search for localization asset instance. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns>language strings or null</returns>
        public static IAsset FindAsset(this ILine line)
        {
            for (; line != null; line = line.GetPreviousPart())
                if (line is IAssetKeyAssetAssigned casted && casted.Asset != null) return casted.Asset;
            return null;
        }
    }

}
