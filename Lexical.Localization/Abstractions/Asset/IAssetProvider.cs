// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           3.11.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Asset
{
    #region interface
    /// <summary>
    /// Loads assets based on parameters in keys.
    /// </summary>
    public interface IAssetProvider : IAsset
    {
        /// <summary>
        /// Load assets that match the criteria of the parameters in <paramref name="key"/>.
        /// 
        /// If key doesn't have some of the required parameters, the <see cref="IAssetProvider"/> 
        /// may match against all detected filenames.
        /// 
        /// The parameters that are matched is specific to the implementation. 
        /// For example, <see cref="IAssetLoader"/> implementation matches based 
        /// on the options of it's parts (<see cref="IAssetLoaderPart.Options"/>).
        /// 
        /// <see cref="IAssetLoaderPartOptions.MatchParameters"/> is a list of parameters to 
        /// match against detected filenames.
        /// </summary>
        /// <param name="key">key as criteria, or null for no criteria</param>
        /// <returns>assets or null</returns>
        /// <exception cref="AssetException">If loading failed</exception>
        IEnumerable<IAsset> LoadAssets(IAssetKey key);

        /// <summary>
        /// Load assets that match the criteria of the parameters in <paramref name="key"/>.
        /// 
        /// If a required parameter is missing from key, the it is matched against all 
        /// detected filenames regardless of any implementation specific options.
        /// </summary>
        /// <param name="key">key as criteria, or null for no criteria</param>
        /// <returns>assets or null</returns>
        /// <exception cref="AssetException">If loading failed</exception>
        IEnumerable<IAsset> LoadAllAssets(IAssetKey key);
    }
    #endregion interface
}
