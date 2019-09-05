// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Asset builder constructs an <see cref="IAsset"/>s from <see cref="IAssetConfiguration"/>.
    /// 
    /// The main purpose is to address dependency injection use-cases.
    /// 
    /// <see cref="Build"/> method assembles different parts into one <see cref="IAsset"/>.
    /// The implementation determines how parts are assembled together.
    /// </summary>
    public interface IAssetBuilder
    {
        /// <summary>
        /// Build asset. 
        /// </summary>
        /// <returns>asset</returns>
        /// <exception cref="AssetException">If the asset builder implementation could not build from the configuration.</exception>
        IAsset Build(IAssetConfiguration configuration);
    }
    // </doc>
}
