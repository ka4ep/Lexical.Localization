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
    public interface ILineSource : IAssetSource
    {
    }

    /// <summary>
    /// Source that provides string based key-value lines
    /// </summary>
    public interface IStringLineSource : ILineSource, IEnumerable<KeyValuePair<string, IFormulationString>>
    {
        /// <summary>
        /// Name policy that is used for converting string to <see cref="ILine"/>.
        /// </summary>
        ILineFormat KeyPolicy { get; }
    }

    /// <summary>
    /// Source that provides <see cref="ILine"/> based key-value lines.
    /// </summary>
    public interface IKeyLineSource : ILineSource, IEnumerable<ILine>
    {
    }

    /// <summary>
    /// Source that provides <see cref="ILineTree"/> based key-value lines.
    /// </summary>
    public interface ILineTreeSource : ILineSource, IEnumerable<ILineTree>
    {
    }
    #endregion interface_2

    #region attribute
    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// 
    /// This is a workaround to the problem, where service provider implementation doesn't 
    /// concatenate <see cref="IAssetSource"/> and <see cref="IEnumerable{IAssetSource}"/>s together.
    /// And supplying an <see cref="IEnumerable{IAssetSource}"/> messes up the service descriptions
    /// of <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSources : IEnumerable<IAssetSource> { }

    /// <summary>
    /// This interface signals that the implementing class provides
    /// asset sources for the class library.
    /// </summary>
    public interface ILibraryAssetSources : IAssetSources { }
    #endregion attribute
}
