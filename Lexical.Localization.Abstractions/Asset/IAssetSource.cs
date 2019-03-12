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

    #region interface_2
    /// <summary>
    /// Signal for asset source that returns language strings
    /// </summary>
    public interface ILocalizationSource : IAssetSource
    {
        string SourceHint { get; }
    }

    /// <summary>
    /// Source that provides string based key-value lines
    /// </summary>
    public interface ILocalizationStringLinesSource : ILocalizationSource, IEnumerable<KeyValuePair<string, string>>
    {
        IAssetKeyNamePolicy NamePolicy { get; }
    }

    /// <summary>
    /// Source that provides <see cref="IAssetKey"/> based key-value lines.
    /// </summary>
    public interface ILocalizationKeyLinesSource : ILocalizationSource, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
    }
    #endregion interface_2

    #region attribute
    /// <summary>
    /// This interface signals that the implementing class provides
    /// asset sources for the class library.
    /// </summary>
    public interface ILibraryAssetSources : IEnumerable<IAssetSource> { }
    #endregion attribute
}
