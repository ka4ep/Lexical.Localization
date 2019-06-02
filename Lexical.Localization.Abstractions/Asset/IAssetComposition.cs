// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexical.Localization.Asset
{
    #region interfaces
    /// <summary>
    /// Composition of <see cref="IAsset"/> components.
    /// </summary>
    public interface IAssetComposition : IAsset, IList<IAsset>
    {
        /// <summary>
        /// Set to new content.
        /// </summary>
        /// <param name="newContent"></param>
        /// <exception cref="InvalidOperationException">If compostion is readonly</exception>
        void CopyFrom(IEnumerable<IAsset> newContent);

        /// <summary>
        /// Get component assets that implement T. 
        /// </summary>
        /// <param name="recursive">if true, visits children recursively</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>enumerable or null</returns>
        IEnumerable<T> GetComponents<T>(bool recursive) where T : IAsset;
    }
    #endregion interfaces
}
