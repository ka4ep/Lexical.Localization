// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.7.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.IO;

namespace Lexical.Localization.Asset
{
    // <doc>
    /// <summary>
    /// Root interface for file system interfaces. See sub-interfaces:
    /// <list type="bullet">
    ///     <item><see cref="IFileSystemReader"/></item>
    ///     <item><see cref="IFileSystemWriter"/></item>
    ///     <item><see cref="IFileSystemObserver"/></item>
    ///     <item><see cref="IFileSystemBrowser"/></item>
    /// </list>
    /// </summary>
    public interface IFileSystem
    {
    }

    /// <summary>
    /// File system that can read files.
    /// </summary>
    public interface IFileSystemReader : IFileSystem
    {
        /// <summary>
        /// Try to open a file for reading.
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns>false if source or file is not found, if true opened file successfully</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        bool TryRead(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete);
    }

    /// <summary>
    /// File system that can write files.
    /// </summary>
    public interface IFileSystemWriter : IFileSystem
    {
        /// <summary>
        /// Try to open a file for writing (and reading).
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns>false if source or file is not found, if true opened file successfully</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        bool TryWrite(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete);
    }

    /// <summary>
    /// File system that can observe files and directories.
    /// </summary>
    public interface IFileSystemObserver : IFileSystem
    {
        /// <summary>
        /// Try to attach <paramref name="observer"/> a single file or folder.
        /// </summary>
        /// <param name="path">path to file or folder</param>
        /// <param name="observer"></param>
        /// <param name="handle">dispose handle</param>
        /// <returns>false if file is not observable</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        bool TryObserve(string path, IObserver<IFileSystemEntryEvent> observer, out IDisposable handle);
    }

    /// <summary>
    /// File system that can browse directories for subfiles and subdirectories.
    /// </summary>
    public interface IFileSystemBrowser : IFileSystem
    {
        /// <summary>
        /// Try to browse file and directory entries.
        /// </summary>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <param name="files">a snapshot of file and directory entries</param>
        /// <returns>true if directory was found, false if directory was not at <paramref name="path"/></returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        bool TryBrowse(string path, out FileSystemEntry[] files);
    }

    /// <summary>
    /// File entry used by <see cref="IFileSystem"/>.
    /// The entry represents the snapshot state at the time of creation.
    /// </summary>
    public struct FileSystemEntry
    {
        /// <summary>
        /// (optional) Associated file system.
        /// </summary>
        public IFileSystem FileSystem;

        /// <summary>
        /// File entry type.
        /// </summary>
        public FileSystemEntryType Type;

        /// <summary>
        /// Path that is relative to the <see cref="IFileSystem"/>.
        /// Separator is "/".
        /// </summary>
        public string Path;

        /// <summary>
        /// File length. -1 if is folder.
        /// </summary>
        public long Length;

        /// <summary>
        /// Entry name without path.
        /// </summary>
        public string Name;

        /// <summary>
        /// Date time of last modification.
        /// </summary>
        public DateTimeOffset LastModified;
    }

    /// <summary>
    /// <see cref="FileSystemEntry"/> type.
    /// </summary>
    public enum FileSystemEntryType : Int32
    {
        /// <summary>Entry is file</summary>
        File = 1,
        /// <summary>Entry is directory</summary>
        Directory = 2
    }

    /// <summary>
    /// File entry event.
    /// </summary>
    public interface IFileSystemEntryEvent
    {
        /// <summary>
        /// Sending file system.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// The affected file or folder.
        /// 
        /// Path is relative to the <see cref="FileSystem"/>'s root.
        /// 
        /// Directory separator is "/". Root path doesn't use separator.
        /// 
        /// Example: "dir/file.ext"
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Change events
        /// </summary>
        WatcherChangeTypes ChangeEvents { get; }
    }
    // </doc>
}

namespace Lexical.Localization
{
    using Lexical.Localization.Asset;

    /// <summary>
    /// Extension methods for <see cref="IFileSystem"/>.
    /// </summary>
    public static partial class IFileSystemExtensions
    {
        /// <summary>
        /// Open a file for reading.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="fileShare"></param>
        /// <exception cref="IOException">On unexpected IO error, or if file was not found</exception>
        public static Stream Read(this IFileSystem fileSystem, string path, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            Stream stream;
            if (fileSystem is IFileSystemReader reader && reader.TryRead(path, out stream, fileShare)) return stream;
            throw new FileNotFoundException("file not found " + path, path);
        }

        /// <summary>
        /// Try to open a file for reading.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns>false if source or file is not found, if true opened file successfully</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static bool TryRead(this IFileSystem fileSystem, string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            if (fileSystem is IFileSystemReader reader) return reader.TryRead(path, out stream, fileShare);
            stream = null;
            return false;
        }

        /// <summary>
        /// Open a file for writing.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="fileShare"></param>
        /// <exception cref="IOException">On unexpected IO error, or if file was not found</exception>
        public static Stream Write(this IFileSystem fileSystem, string path, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            Stream stream;
            if (fileSystem is IFileSystemWriter writer && writer.TryWrite(path, out stream, fileShare)) return stream;
            throw new IOException("Could not open file for writing: " + path);
        }

        /// <summary>
        /// Try to open a file for write.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns>false if source or file is not found, if true opened file successfully</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static bool TryWrite(this IFileSystem fileSystem, string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            if (fileSystem is IFileSystemWriter writer) return writer.TryWrite(path, out stream, fileShare);
            stream = null;
            return false;
        }

        /// <summary>
        /// Attach observer to a single file or folder.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">path to file or folder</param>
        /// <param name="observer"></param>
        /// <returns>dispose handle</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static IDisposable Observe(this IFileSystem fileSystem, string path, IObserver<IFileSystemEntryEvent> observer)
        {
            IDisposable disposable;
            if (fileSystem is IFileSystemObserver observer_ && observer_.TryObserve(path, observer, out disposable)) return disposable;
            throw new IOException("Could not observe: "+path);
        }

        /// <summary>
        /// Try to attach <paramref name="observer"/> a single file or folder.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">path to file or folder</param>
        /// <param name="observer"></param>
        /// <param name="handle">dispose handle</param>
        /// <returns>false if file is not observable</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static bool TryObserve(this IFileSystem fileSystem, string path, IObserver<IFileSystemEntryEvent> observer, out IDisposable handle)
        {
            if (fileSystem is IFileSystemObserver observer_) return observer_.TryObserve(path, observer, out handle);
            handle = null;
            return false;
        }

        /// <summary>
        /// List file and directory entries.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <returns>a snapshot of file and directory entries</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static FileSystemEntry[] Browse(this IFileSystem fileSystem, string path)
        {
            FileSystemEntry[] files;
            if (fileSystem is IFileSystemBrowser browser && browser.TryBrowse(path, out files)) return files;
            throw new IOException("Could not browse directory: "+path);
        }

        /// <summary>
        /// Try to list file and directory entries.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <param name="files">a snapshot of file and directory entries</param>
        /// <returns>true if directory was found, false if directory was not at <paramref name="path"/></returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        public static bool TryBrowse(this IFileSystem fileSystem, string path, out FileSystemEntry[] files)
        {
            if (fileSystem is IFileSystemBrowser browser) return browser.TryBrowse(path, out files);
            files = null;
            return false;
        }

    }

}
