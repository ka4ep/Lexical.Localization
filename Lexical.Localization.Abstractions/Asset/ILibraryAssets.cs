// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           8.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Asset
{
    // <doc>

    /// <summary>
    /// This interface signals that the implementing class provides
    /// asset sources for the class library.
    /// </summary>
    public interface ILibraryAssetSources_ : IAssetSources
    {
    }

    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// </summary>
    public interface ILibraryAssetSource
    {
        /// <summary>
        /// Asset sources that is constructed lazily.
        /// </summary>
        IAssetSource Source { get; }
    }

    /// <summary>
    /// Used with dependency injection as a service type of multiple <see cref="IAssetSource"/>s.
    /// </summary>
    public interface ILibraryAsset
    {
        /// <summary>
        /// Asset for library
        /// </summary>
        IAsset Asset { get; }
    }

    /// <summary>
    /// </summary>
    public interface ILibraryFileSystem
    {
        /// <summary>
        /// </summary>
        IFileSystem FileSystem { get; }
    }

    /// <summary>
    /// </summary>
    public interface ILibraryFileSystems
    {
        /// <summary>
        /// </summary>
        IList<IFileSystem> FileSystems { get; }
    }


    /// <summary>
    /// </summary>
    public interface ILibraryAssetFile
    {
        /// <summary>
        /// </summary>
        IAssetFile AssetFile { get; }
    }

    /// <summary>
    /// </summary>
    public interface ILibraryAssetFiles
    {
        /// <summary>
        /// </summary>
        IList<IAssetFile> AssetFiles { get; }
    }

    /// <summary>
    /// </summary>
    public interface ILibraryAssetFilePattern
    {
        /// <summary>
        /// </summary>
        IAssetFilePattern AssetFilePattern { get; }
    }

    /// <summary>
    /// </summary>
    public interface ILibraryAssetFilePatterns
    {
        /// <summary>
        /// </summary>
        IList<IAssetFilePattern> AssetFilePatterns { get; }
    }

    // </doc>
}
