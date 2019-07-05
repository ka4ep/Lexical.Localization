// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           17.6.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.IO;

namespace Lexical.Localization.Asset
{
    #region interface
    /// <summary>
    /// See sub-interfaces:
    /// <list type="bullet">
    ///     <item><see cref="IBrowsableFileSource"/></item>
    ///     <item><see cref="IReadableFileSource"/></item>
    ///     <item><see cref="IObservableFileSource"/></item>
    /// </list>
    /// </summary>
    public interface IFileSource : IAssetBuilderPart
    {
    }

    /// <summary>
    /// File source needs disposing.
    /// </summary>
    public interface IDisposableFileSource : IFileSource, IDisposable
    {
    }

    /// <summary>
    /// Files can be browsed.
    /// </summary>
    public interface IBrowsableFileSource : IFileSource
    {
        /// <summary>
        /// Try to browse files.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileEntries"></param>
        /// <returns>false if source is not browsable</returns>
        /// <exception cref="IOException">On IO exception</exception>
        bool TryBrowseFiles(string path, out IEnumerable<IFileEntry> fileEntries);
    }

    /// <summary>
    /// File source that can read files
    /// </summary>
    public interface IReadableFileSource : IFileSource
    {
        /// <summary>
        /// Try open file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns>false if source or file is not readable</returns>
        /// <exception cref="IOException">On IO exception</exception>
        bool TryOpenFile(string path, out Stream stream);
    }

    /// <summary>
    /// File source that can be observed for modifications.
    /// </summary>
    public interface IObservableFileSource : IFileSource
    {
        /// <summary>
        /// Observe a file (path), or a group of files with wildcard pattern.
        /// </summary>
        /// <param name="filePattern"></param>
        /// <param name="observer"></param>
        /// <param name="handle"></param>
        /// <returns>false if source or pattern is not observable</returns>
        /// <exception cref="IOException">On IO exception</exception>
        bool TryObserve(string filePattern, IObserver<IFileSourceEvent> observer, out IDisposable handle);
    }

    /// <summary>
    /// Entry about file. 
    /// </summary>
    public interface IFileEntry
    {
        /// <summary>
        /// Path to file or folder. Folder separator is "/". "" is root.
        /// </summary>
        String Path { get; }

        /// <summary>
        /// Name within path
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Entry Type
        /// </summary>
        FileEntryType Type { get; }

        /// <summary>
        /// Length, if file. If Directory value is -1.
        /// </summary>
        long Length { get; }
    }

    /// <summary>
    /// <see cref="IFileEntry"/> type.
    /// </summary>
    public enum FileEntryType
    {
        /// <summary>
        /// Entry is a file.
        /// </summary>
        File,

        /// <summary>
        /// Entry is a directory. 
        /// </summary>
        Directory
    }

    /// <summary>
    /// Source that can send events.
    /// </summary>
    public interface IAssetObservable : IAsset, IObservable<IFileEvent>
    {
    }

    /// <summary>
    /// Source that can send events.
    /// </summary>
    public interface IAssetSourceObservable : IAssetSource, IObservable<IFileSourceEvent>
    {
    }

    /// <summary>
    /// File source event.
    /// </summary>
    public interface IFileEvent
    {
        /// <summary>
        /// Affected asset
        /// </summary>
        IAsset Asset { get; }
    }

    /// <summary>
    /// File source event.
    /// </summary>
    public interface IFileSourceEvent
    {
        /// <summary>
        /// Affected source
        /// </summary>
        IAssetSource Source { get; }
    }

    /// <summary>
    /// Change event, for <see cref="IFileEvent"/> and <see cref="IFileSourceEvent"/>.
    /// </summary>
    public interface IFileChangeEvent
    {
        /// <summary>
        /// Change type
        /// </summary>
        WatcherChangeTypes ChangeType { get; }
    }
    #endregion interface
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static partial class IFileSourceExtensions
    {
    }
}

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Source has changed.
    /// </summary>
    public class FileChangedEvent : IFileEvent, IFileChangeEvent
    {
        /// <summary>
        /// Affected asset
        /// </summary>
        public IAsset Asset { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        public WatcherChangeTypes ChangeType { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="changeType"></param>
        public FileChangedEvent(IAsset asset, WatcherChangeTypes changeType)
        {
            Asset = asset;
            ChangeType = changeType;
        }
    }

    /// <summary>
    /// Source has changed.
    /// </summary>
    public class FileSourceChangedEvent : IFileSourceEvent, IFileChangeEvent
    {
        /// <summary>
        /// Affected source
        /// </summary>
        public IAssetSource Source { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        public WatcherChangeTypes ChangeType { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="changeType"></param>
        public FileSourceChangedEvent(IAssetSource source, WatcherChangeTypes changeType)
        {
            Source = source;
            ChangeType = changeType;
        }
    }

}