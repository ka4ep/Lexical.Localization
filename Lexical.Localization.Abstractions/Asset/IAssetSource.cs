// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Source of assets. Adds resources to builder's list.
    /// 
    /// The implementation of this class must use one of the more specific sub-interfaces:
    /// <list type="table">
    /// <item><see cref="IStringAssetSource"/></item>
    /// <item><see cref="IResourceAssetSource"/></item>
    /// <item><see cref="IBuildableAssetSource"/></item>
    /// <item><see cref="IFileAssetSource"/></item>
    /// <item><see cref="IFilePatternAssetSource"/></item>
    /// <item><see cref="IPostBuildAssetSource"/></item>
    /// </list>
    /// </summary>
    public interface IAssetSource
    {
    }

    /// <summary>
    /// Provides a specific <see cref="IFileSystem"/> that is to be used with the <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSourceFileSystem : IAssetSource
    {
        /// <summary>
        /// Specific <see cref="IFileSystem"/> to load the asset source from.
        /// 
        /// If null, then file-system is unspecified, and the caller have a reference to known file-system.
        /// </summary>
        IFileSystem FileSystem { get; }
    }

    /// <summary>
    /// Asset source that originates from one specific file.
    /// 
    /// The implementing class can implement <see cref="IAssetSourceFileSystem"/> which 
    /// gives a reference to <see cref="IFileSystem"/> from which the file is to be loaded.
    /// 
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IResourceAssetSource"/> to signal
    /// the content type of the asset file.
    /// </summary>
    public interface IFileAssetSource : IAssetSource
    {
        /// <summary>
        /// Reference to an asset file. Used within <see cref="IFileSystem"/>. Directory separator is '/'. Root doesn't use separator.
        /// 
        /// Example: "resources/localization.xml".
        /// </summary>
        string FilePath { get; }
    }

    /// <summary>
    /// Asset source that referers to a pattern of filenames.
    /// 
    /// The implementing class can implement <see cref="IAssetSourceFileSystem"/> which 
    /// gives a reference to <see cref="IFileSystem"/> from which the file is to be loaded.
    /// 
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IResourceAssetSource"/> to signal
    /// the content type of the asset file.
    /// </summary>
    public interface IFilePatternAssetSource : IAssetSource
    {
        /// <summary>
        /// Reference to a pattern of asset files. Used within <see cref="IFileSystem"/>.
        /// 
        /// Separator character is '/'. Root doesn't use separator.
        /// Example: "resources/{Culture/}localization.xml".
        /// </summary>
        ILinePattern FilePattern { get; }
    }

    /// <summary>
    /// Source that can build <see cref="IAsset"/>(s). 
    /// </summary>
    public interface IBuildableAssetSource : IAssetSource
    {
        /// <summary>
        /// Source adds its <see cref="IAsset"/>s to list.
        /// </summary>
        /// <param name="list">list to add provider(s) to</param>
        /// <returns>self</returns>
        void Build(IList<IAsset> list);
    }

    /// <summary>
    /// Post build action for <see cref="IAssetBuilder"/>.
    /// </summary>
    public interface IPostBuildAssetSource
    {
        /// <summary>
        /// Allows source to do post build action and to decorate already built asset.
        /// 
        /// This allows a source to provide decoration such as cache.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>asset or component</returns>
        IAsset PostBuild(IAsset asset);
    }
    // </doc>

    // <doc2>
    /// <summary>
    /// Signals that <see cref="IAssetSource"/> constructs string lines asset.
    /// </summary>
    public interface IStringAssetSource : IAssetSource
    {
        /// <summary>
        /// File format to use to read the file. 
        /// </summary>
        ILineFileFormat FileFormat { get; }
    }
    // </doc2>

    // <doc3>
    /// <summary>
    /// Signals that <see cref="IAssetSource"/> constructs <see cref="IResourceAsset"/>.
    /// </summary>
    public interface IResourceAssetSource : IAssetSource
    {
    }
    // </doc3>


    // <attribute>
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
    // </attribute>
}
