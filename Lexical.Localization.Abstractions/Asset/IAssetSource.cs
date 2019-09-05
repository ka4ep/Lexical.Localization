// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           5.9.2019
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
    ///     <item><see cref="IFileAssetSource"/></item>
    ///     <item><see cref="IFilePatternAssetSource"/></item>
    ///     <item><see cref="IStringAssetSource"/></item>
    ///     <item><see cref="IBinaryAssetSource"/></item>
    ///     <item><see cref="IAssetSourceFileSystem"/></item>
    ///     <item><see cref="IAssetSourceObservePolicy"/></item>
    /// </list>
    /// </summary>
    public interface IAssetSource
    {
    }

    /// <summary>
    /// Provides a specific <see cref="IFileSystem"/> to be used with the <see cref="IAssetSource"/>.
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
    /// Provides a specific <see cref="IAssetObservePolicy"/> for the <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSourceObservePolicy : IAssetSource
    {
        /// <summary>
        /// Asset file observe policy for asset source.
        /// </summary>
        IAssetObservePolicy ObservePolicy { get; set; }
    }

    /// <summary>
    /// Asset source that originates from one specific file.
    /// 
    /// The implementing class can implement <see cref="IAssetSourceFileSystem"/> which 
    /// gives a reference to <see cref="IFileSystem"/> from which the file is to be loaded.
    /// 
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IBinaryAssetSource"/> to signal
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
    /// The implementing class can implement <see cref="IStringAssetSource"/> or <see cref="IBinaryAssetSource"/> to signal
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
    /// Signals that the implementing <see cref="IAssetSource"/> constructs a <see cref="IStringAsset"/>.
    /// </summary>
    public interface IStringAssetSource : IAssetSource
    {
        /// <summary>
        /// File format to use to read the file. 
        /// </summary>
        ILineFileFormat FileFormat { get; }
    }

    /// <summary>
    /// Signals that the implementing <see cref="IAssetSource"/> constructs a <see cref="IBinaryAsset"/>.
    /// </summary>
    public interface IBinaryAssetSource : IAssetSource
    {
    }

    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// 
    /// This is a workaround to the problem, where service provider implementation doesn't 
    /// concatenate <see cref="IAssetSource"/> and <see cref="IEnumerable{IAssetSource}"/>s together.
    /// And supplying an <see cref="IEnumerable{IAssetSource}"/> messes up the service descriptions
    /// of <see cref="IAssetSource"/>.
    /// </summary>
    public interface IAssetSources : IEnumerable<IAssetSource> { }
    // </doc>
}
