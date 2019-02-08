// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization
{
    #region interface
    /// <summary>
    /// Source of assets. Adds resources to builder's list.
    /// </summary>
    public interface IAssetSource
    {
        /// <summary>
        /// Source adds its <see cref="IAsset"/>s to list.
        /// </summary>
        /// <param name="list">list to add provider(s) to</param>
        /// <returns>self</returns>
        void Build(IList<IAsset> list);

        /// <summary>
        /// Allows source to do post build action and to decorate already built asset.
        /// 
        /// This allows a source to provide decoration such as cache.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>asset or component</returns>
        IAsset PostBuild(IAsset asset);
    }
    #endregion interface
}
