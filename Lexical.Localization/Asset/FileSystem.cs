// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.7.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.FileSystem;
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// File system based <see cref="IFileSystem"/> that loads local file system files.
    /// 
    /// If file watchers have been created, and file system is disposed, then watchers will be disposed also. 
    /// <see cref="IObserver{T}.OnCompleted"/> event is forwarded to watchers.
    /// </summary>
    public class FileSystem : FileSystemBase, IFileSystem, IFileSystemBrowse, IFileSystemOpen, IFileSystemDelete, IFileSystemMove, IFileSystemCreateDirectory, IFileSystemObserve
    {
        static FileSystem osRoot = new FileSystem("");
        static Lazy<FileSystem> applicationRoot = new Lazy<FileSystem>(() => new FileSystem(AppDomain.CurrentDomain.BaseDirectory));

        /// <summary>
        /// File system system that reads from application base directory (application resources).
        /// </summary>
        public static FileSystem ApplicationRoot => applicationRoot.Value;

        /// <summary>
        /// File system system that reads from operating system root.
        /// </summary>
        public static FileSystem OSRoot => osRoot;

        /// <summary>
        /// The root path as provided with constructor.
        /// </summary>
        public readonly string RootPath;

        /// <summary>
        /// Full absolute root path.
        /// <see cref="RootPath"/> ran with <see cref="System.IO.Path.GetFullPath(string)"/>.
        /// </summary>
        public readonly string AbsoluteRootPath;

        /// <summary>
        /// Get capabilities.
        /// </summary>
        public override FileSystemCapabilities Capabilities =>
            FileSystemCapabilities.Browse | FileSystemCapabilities.CreateDirectory | FileSystemCapabilities.Delete | FileSystemCapabilities.Move |
            FileSystemCapabilities.Observe | 
            FileSystemCapabilities.Open | FileSystemCapabilities.Write | FileSystemCapabilities.Read | FileSystemCapabilities.CreateFile;

        /// <summary>
        /// Create asset file system
        /// </summary>
        /// <param name="rootPath"></param>
        public FileSystem(string rootPath) : base()
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            AbsoluteRootPath = System.IO.Path.GetFullPath(rootPath);
        }

        /// <summary>
        /// Open a file for reading and/or writing. File can be created when <paramref name="fileMode"/> is <see cref="FileMode.Create"/> or <see cref="FileMode.CreateNew"/>.
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="fileMode">determines whether to open or to create the file</param>
        /// <param name="fileAccess">how to access the file, read, write or read and write</param>
        /// <param name="fileShare">how the file will be shared by processes</param>
        /// <returns>open file stream</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support opening files</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and and the file specified by path does not exist. The file must already exist in these modes.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileMode"/>, <paramref name="fileAccess"/> or <paramref name="fileShare"/> contains an invalid value.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            string concatenatedPath = Path.Combine(AbsoluteRootPath, path);
            string absolutePath = Path.GetFullPath(concatenatedPath);
            if (!absolutePath.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");
            return new FileStream(absolutePath, fileMode, fileAccess, fileShare);
        }

        /// <summary>
        /// Create a directory, or multiple cascading directories.
        /// 
        /// If directory at <paramref name="path"/> already exists, then returns without exception.
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <returns>true if directory exists after the method, false if directory doesn't exist</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support create directory</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        public void CreateDirectory(string path)
        {
            string concatenatedPath = Path.Combine(AbsoluteRootPath, path);
            string absolutePath = Path.GetFullPath(concatenatedPath);
            if (!absolutePath.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");
            Directory.CreateDirectory(absolutePath);
        }

        /// <summary>
        /// Browse a directory for file and subdirectory entries.
        /// </summary>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <returns>a snapshot of file and directory entries</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support browse</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        public FileSystemEntry[] Browse(string path)
        {
            string concatenatedPath = RootPath == null ? path : (RootPath.EndsWith("/") || RootPath.EndsWith("\\")) ? RootPath + path : RootPath + "/" + path;
            string absolutePath = Path.GetFullPath(concatenatedPath);
            if (!absolutePath.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");

            DirectoryInfo dir = new DirectoryInfo(concatenatedPath);
            if (dir.Exists)
            {
                StructList24<FileSystemEntry> list = new StructList24<FileSystemEntry>();
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    list.Add(new FileSystemEntry { FileSystem = this, LastModified = di.LastWriteTimeUtc, Name = di.Name, Path = path.Length > 0 ? path + "/" + di.Name : di.Name, Length = -1L, Type = FileSystemEntryType.Directory });
                }
                foreach (FileInfo _fi in dir.GetFiles())
                {
                    list.Add(new FileSystemEntry { FileSystem = this, LastModified = _fi.LastWriteTimeUtc, Name = _fi.Name, Path = path.Length > 0 ? path + "/" + _fi.Name : _fi.Name, Length = _fi.Length, Type = FileSystemEntryType.File });
                }
                return list.ToArray();
            }

            FileInfo fi = new FileInfo(concatenatedPath);
            if (fi.Exists)
            {
                FileSystemEntry e = new FileSystemEntry {
                    FileSystem = this,
                    LastModified = fi.LastWriteTimeUtc,
                    Name = fi.Name,
                    Path = path,
                    Length = fi.Length,
                    Type = FileSystemEntryType.File
                };
                return new FileSystemEntry[] { e };
            }

            throw new DirectoryNotFoundException(path);
        }

        /// <summary>
        /// Delete a file or directory.
        /// 
        /// If <paramref name="recursive"/> is false and <paramref name="path"/> is a directory that is not empty, then <see cref="InvalidOperationException"/> is thrown.
        /// If <paramref name="recursive"/> is true, then any file or directory within <paramref name="path"/> is deleted as well.
        /// </summary>
        /// <param name="path">path to a file or directory</param>
        /// <param name="recursive">if path refers to directory, recurse into sub directories</param>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support deleting files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refered to a directory that wasn't empty and <paramref name="recursive"/> is false, or <paramref name="path"/> refers to non-file device</exception>
        public void Delete(string path, bool recursive = false)
        {
            string concatenatedPath = RootPath == null ? path : (RootPath.EndsWith("/") || RootPath.EndsWith("\\")) ? RootPath + path : RootPath + "/" + path;
            string absolutePath = Path.GetFullPath(concatenatedPath);
            if (!absolutePath.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");

            FileInfo fi = new FileInfo(concatenatedPath);
            if (fi.Exists) { fi.Delete(); return; }

            DirectoryInfo di = new DirectoryInfo(concatenatedPath);
            if (di.Exists) { di.Delete(recursive); return; }

            throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Try to move/rename a file or directory.
        /// </summary>
        /// <param name="oldPath">old path of a file or directory</param>
        /// <param name="newPath">new path of a file or directory</param>
        /// <exception cref="FileNotFoundException">The specified <paramref name="oldPath"/> is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="ArgumentNullException">path is null</exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support renaming/moving files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">path refers to non-file device, or an entry already exists at <paramref name="newPath"/></exception>
        public void Move(string oldPath, string newPath)
        {
            string oldConcatenatedPath = RootPath == null ? oldPath : (RootPath.EndsWith("/") || RootPath.EndsWith("\\")) ? RootPath + oldPath : RootPath + "/" + oldPath;
            string newConcatenatedPath = RootPath == null ? newPath : (RootPath.EndsWith("/") || RootPath.EndsWith("\\")) ? RootPath + newPath : RootPath + "/" + newPath;

            string oldPathAbsolute = Path.GetFullPath(oldConcatenatedPath), newPathAbsolute = Path.GetFullPath(newConcatenatedPath);
            if (!oldPathAbsolute.StartsWith(AbsoluteRootPath)) throw new FileNotFoundException("Path cannot refer outside IFileSystem root");
            if (!newPathAbsolute.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");

            FileInfo fi = new FileInfo(oldConcatenatedPath);
            if (fi.Exists) { fi.MoveTo(newConcatenatedPath); return; }

            DirectoryInfo di = new DirectoryInfo(oldConcatenatedPath);
            if (di.Exists) { di.MoveTo(newConcatenatedPath); return; }

            throw new FileNotFoundException(oldPath);
        }

        /// <summary>
        /// Attach an <paramref name="observer"/> on to a single file or directory. 
        /// Observing a directory will observe the whole subtree.
        /// </summary>
        /// <param name="path">path to file or directory. The directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <param name="observer"></param>
        /// <returns>dispose handle</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support observe</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        public IDisposable Observe(string path, IObserver<FileSystemEntryEvent> observer)
        {
            string concatenatedPath = Path.Combine(AbsoluteRootPath, path);
            string absolutePath = Path.GetFullPath(concatenatedPath);
            if (!absolutePath.StartsWith(AbsoluteRootPath)) throw new InvalidOperationException("Path cannot refer outside IFileSystem root");

            return new Watcher(this, observer, absolutePath, path);
        }

        /// <summary>
        /// File or folder watcher.
        /// </summary>
        public class Watcher : IDisposable
        {
            /// <summary>
            /// Associated system
            /// </summary>
            protected IFileSystem fileSystem;

            /// <summary>
            /// Absolute path as OS path. Separator is '\\' or '/'.
            /// </summary>
            public readonly string AbsolutePath;

            /// <summary>
            /// Relative path. Path is relative to the <see cref="fileSystem"/>'s root.
            /// The directory separator is '/'.
            /// </summary>
            public readonly string RelativePath;

            /// <summary>
            /// Relative path that is passed for FileSystemWatcher.
            /// </summary>
            public readonly string WatcherDirectoryRelativePath;

            /// <summary>
            /// Watcher
            /// </summary>
            protected FileSystemWatcher watcher;

            /// <summary>
            /// Callback object.
            /// </summary>
            protected IObserver<FileSystemEntryEvent> observer;

            /// <summary>
            /// Create observer for one file.
            /// </summary>
            /// <param name="fileSystem">associated file system</param>
            /// <param name="observer">observer for callbacks</param>
            /// <param name="absolutePath">Absolute path</param>
            /// <param name="relativePath">Relative path (separator is '/')</param>
            public Watcher(IFileSystem fileSystem, IObserver<FileSystemEntryEvent> observer, string absolutePath, string relativePath)
            {
                this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
                this.AbsolutePath = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));
                this.RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
                relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
                FileInfo fi = new FileInfo(absolutePath);
                DirectoryInfo di = new DirectoryInfo(absolutePath);
                // Watch directory
                if (di.Exists)
                {
                    watcher = new FileSystemWatcher(absolutePath);
                    watcher.IncludeSubdirectories = true;
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                    WatcherDirectoryRelativePath = RelativePath;
                }
                // Watch file
                else //if (fi.Exists)
                {
                    watcher = new FileSystemWatcher(fi.Directory.FullName, fi.Name);
                    watcher.IncludeSubdirectories = false;
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                    int ix = RelativePath.LastIndexOf('/');
                    WatcherDirectoryRelativePath = ix < 0 ? "" : RelativePath.Substring(0, ix);
                }

                watcher.Error += OnError;
                watcher.Changed += OnEvent;
                watcher.Created += OnEvent;
                watcher.Deleted += OnEvent;
                watcher.Renamed += OnEvent;
                watcher.EnableRaisingEvents = true;
            }

            /// <summary>
            /// Handle (Forward) error event.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OnError(object sender, ErrorEventArgs e)
            {
                var _observer = observer;
                if (_observer == null) return;

                // Disposed
                IFileSystem _fileSystem = fileSystem;
                if (_fileSystem == null) return;

                // Forward event.
                observer.OnError(e.GetException());
            }

            /// <summary>
            /// Forward event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OnEvent(object sender, FileSystemEventArgs e)
            {
                var _observer = observer;
                if (_observer == null) return;

                // Disposed
                IFileSystem _fileSystem = fileSystem;
                if (_fileSystem == null) return; 

                // Forward event.
                FileSystemEntryEvent ae = new FileSystemEntryEvent { FileSystem = _fileSystem, ChangeEvents = e.ChangeType, Path = WatcherDirectoryRelativePath == "" ? e.Name : WatcherDirectoryRelativePath+"/"+e.Name };
                if (Path.DirectorySeparatorChar != '/') ae.Path = ae.Path.Replace(Path.DirectorySeparatorChar, '/');
                observer.OnNext(ae);
            }

            /// <summary>
            /// Dispose observer
            /// </summary>
            /// <exception cref="AggregateException"></exception>
            public void Dispose()
            {
                var _watcher = watcher;
                var _observer = observer;

                // Clear file system reference, and remove watcher from dispose list.
                IFileSystem _fileSystem = Interlocked.Exchange(ref fileSystem, null);
                if (_fileSystem is FileSystemBase __fileSystem) __fileSystem.RemoveDisposableBase(this);

                StructList2<Exception> errors = new StructList2<Exception>();
                if (_observer != null)
                {
                    observer = null;
                    try
                    {
                        _observer.OnCompleted();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                if (_watcher != null)
                {
                    watcher = null;
                    try
                    {
                        _watcher.Dispose();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                // Throw exceptions
                if (errors.Count > 0) throw new AggregateException(errors);
            }
        }

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        public FileSystem AddDisposable(object disposable) => AddDisposableBase(disposable) as FileSystem;

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public FileSystem RemoveDisposable(object disposable) => RemoveDisposableBase(disposable) as FileSystem;

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => RootPath;
    }

    /// <summary>
    /// Base implementation for <see cref="IFileSystem"/>. 
    /// 
    /// Disposables can be attached to be disposed along with <see cref="IFileSystem"/>.
    /// Watchers can be attached as disposables, so that they forward <see cref="IObserver{T}.OnCompleted"/> event upon IFileSystem dispose.
    /// </summary>
    public abstract class FileSystemBase : IFileSystem, IDisposable
    {
        /// <summary>
        /// Get capabilities.
        /// </summary>
        public virtual FileSystemCapabilities Capabilities { get; }

        /// <summary>
        /// Lock for modifying dispose list.
        /// </summary>
        protected object m_lock = new object();

        /// <summary>
        /// Attached disposables.
        /// </summary>
        protected List<IDisposable> disposeList;

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        internal protected IFileSystem AddDisposableBase(object disposable)
        {
            if (disposable is IDisposable disp)
            {
                lock (m_lock)
                {
                    if (this.disposeList == null) this.disposeList = new List<IDisposable>();
                    this.disposeList.Add(disp);
                }
            }
            return this;
        }

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        internal protected IFileSystem RemoveDisposableBase(object disposable)
        {
            if (disposable is IDisposable disp)
            {
                lock (m_lock)
                {
                    if (this.disposeList != null) this.disposeList.Remove(disp);
                }
            }
            return this;
        }

        /// <summary>
        /// Dispose attached disposables.
        /// </summary>
        /// <exception cref="AggregateException">If dispose exception occurs</exception>
        public void Dispose()
        {
            // Get and clear
            List<IDisposable> list = Interlocked.Exchange(ref this.disposeList, null);
            // Nothing to dispose
            if (list == null) return;

            StructList4<Exception> errors = new StructList4<Exception>();
            foreach (IDisposable d in list)
            {
                try
                {
                    d.Dispose();
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }

            if (errors.Count > 0) throw new AggregateException(errors);
        }

    }

    /// <summary>
    /// File system that reads, observes and browses files from <see cref="IFileProvider"/> source.
    /// </summary>
    public class FileProviderSystem : FileSystemBase, IFileSystemBrowse, IFileSystemObserve, IFileSystemOpen
    {
        /// <summary>
        /// Optional subpath within the source <see cref="fileProvider"/>.
        /// </summary>
        protected String SubPath;

        /// <summary>
        /// Source file provider. This value is nulled upon dispose.
        /// </summary>
        protected IFileProvider fileProvider;

        /// <summary>
        /// IFileProvider capabilities
        /// </summary>
        protected FileSystemCapabilities capabilities;

        /// <summary>
        /// IFileProvider capabilities
        /// </summary>
        public override FileSystemCapabilities Capabilities => capabilities;

        /// <summary>
        /// Create file provider based file system.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="subpath">(optional) subpath within the file provider</param>
        /// <param name="capabilities">file provider capabilities</param>
        public FileProviderSystem(IFileProvider fileProvider, string subpath = null, FileSystemCapabilities capabilities = FileSystemCapabilities.Open | FileSystemCapabilities.Read | FileSystemCapabilities.Observe | FileSystemCapabilities.Browse) : base()
        {
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(subpath));
            this.SubPath = subpath;
            this.capabilities = capabilities;
        }

        /// <summary>
        /// Open a file for reading. 
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="fileMode">determines whether to open or to create the file</param>
        /// <param name="fileAccess">how to access the file, read, write or read and write</param>
        /// <param name="fileShare">how the file will be shared by processes</param>
        /// <returns>open file stream</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support opening files</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and and the file specified by path does not exist. The file must already exist in these modes.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileMode"/>, <paramref name="fileAccess"/> or <paramref name="fileShare"/> contains an invalid value.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            // Check mode is for opening existing file
            if (fileMode != FileMode.Open) throw new NotSupportedException("FileMode = "+fileMode+" is not supported");
            // Check access is for reading
            if (fileAccess != FileAccess.Read) throw new NotSupportedException("FileAccess = " + fileAccess + " is not supported");
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) throw new ObjectDisposedException(nameof(FileProviderSystem));
            // Does file exist?
            IFileInfo fi = fp.GetFileInfo(concatenatedPath);
            if (!fi.Exists) throw new FileNotFoundException(path);
            // Read
            Stream s = fi.CreateReadStream();
            if (s == null) throw new FileNotFoundException(path);
            // Ok
            return s;
        }

        /// <summary>
        /// Browse a directory for file and subdirectory entries.
        /// </summary>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <returns>a snapshot of file and directory entries</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support browse</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public FileSystemEntry[] Browse(string path)
        {
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) throw new ObjectDisposedException(nameof(FileProviderSystem));
            // Browse
            IDirectoryContents contents = fp.GetDirectoryContents(concatenatedPath);
            if (contents.Exists)
            {
                // Convert result
                StructList24<FileSystemEntry> list = new StructList24<FileSystemEntry>();
                foreach (IFileInfo _fi in contents)
                {
                    list.Add(new FileSystemEntry { FileSystem = this, LastModified = _fi.LastModified, Name = _fi.Name, Path = concatenatedPath.Length > 0 ? concatenatedPath + "/" + _fi.Name : _fi.Name, Length = _fi.IsDirectory ? -1L : _fi.Length, Type = _fi.IsDirectory ? FileSystemEntryType.Directory : FileSystemEntryType.File });
                }
                return list.ToArray();
            }

            IFileInfo fi = fp.GetFileInfo(concatenatedPath);
            if (fi.Exists)
            {
                FileSystemEntry e = new FileSystemEntry { FileSystem = this, LastModified = fi.LastModified, Name = fi.Name, Path = concatenatedPath, Length = fi.IsDirectory ? -1L : fi.Length, Type = fi.IsDirectory ? FileSystemEntryType.Directory : FileSystemEntryType.File };
                return new FileSystemEntry[] { e };
            }

            throw new DirectoryNotFoundException(path);
        }

        /// <summary>
        /// Attach an <paramref name="observer"/> on to a single file or directory. 
        /// Observing a directory will observe the whole subtree.
        /// </summary>
        /// <param name="path">path to file or directory. The directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <param name="observer"></param>
        /// <returns>dispose handle</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support observe</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public IDisposable Observe(string path, IObserver<FileSystemEntryEvent> observer)
        {
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) throw new ObjectDisposedException(nameof(FileProviderSystem));
            // Observe
            return new Watcher(this, fp, observer, concatenatedPath, path);
        }

        /// <summary>
        /// File watcher.
        /// </summary>
        public class Watcher : IDisposable
        {
            /// <summary>
            /// Path to supply to <see cref="fileProvider"/>.
            /// </summary>
            public readonly string FileProviderPath;

            /// <summary>
            /// Relative path. Path is relative to the <see cref="fileSystem"/>'s root.
            /// </summary>
            public readonly string RelativePath;

            /// <summary>
            /// Associated observer
            /// </summary>
            protected IObserver<FileSystemEntryEvent> observer;

            /// <summary>
            /// The parent file system.
            /// </summary>
            protected IFileSystem fileSystem;

            /// <summary>
            /// File provider
            /// </summary>
            protected IFileProvider fileProvider;

            /// <summary>
            /// Watcher class
            /// </summary>
            protected IDisposable watcher;

            /// <summary>
            /// Previous state of file existing.
            /// </summary>
            protected int existed;

            /// <summary>
            /// Change token
            /// </summary>
            protected IChangeToken changeToken;

            /// <summary>
            /// Print info
            /// </summary>
            /// <returns></returns>
            public override string ToString()
                => fileProvider?.ToString() ?? "disposed";

            /// <summary>
            /// Create observer for one file.
            /// </summary>
            /// <param name="system"></param>
            /// <param name="fileProvider"></param>
            /// <param name="observer"></param>
            /// <param name="fileProviderPath">Absolute path</param>
            /// <param name="relativePath">Relative path (separator is '/')</param>
            public Watcher(IFileSystem system, IFileProvider fileProvider, IObserver<FileSystemEntryEvent> observer, string fileProviderPath, string relativePath)
            {
                this.fileSystem = system ?? throw new ArgumentNullException(nameof(system));
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
                this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
                this.FileProviderPath = fileProviderPath ?? throw new ArgumentNullException(nameof(fileProviderPath));
                this.RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
                this.changeToken = fileProvider.Watch(FileProviderPath);
                this.watcher = changeToken.RegisterChangeCallback(OnEvent, this);
                this.existed = fileProvider.GetFileInfo(FileProviderPath).Exists ? 1 : 0;
            }

            /// <summary>
            /// Forward event
            /// </summary>
            /// <param name="sender"></param>
            void OnEvent(object sender)
            {
                var _observer = observer;
                if (_observer == null) return;

                // Disposed
                IFileProvider _fileProvider = fileProvider;
                IFileSystem _fileSystem = fileSystem;
                if (_fileProvider == null || _fileSystem == null) return;

                // Figure out change type
                bool exists = _fileProvider.GetFileInfo(FileProviderPath).Exists;
                bool _existed = Interlocked.CompareExchange(ref existed, exists ? 1 : 0, existed) == 1;

                WatcherChangeTypes eventType = default;
                if (_existed)
                {
                    eventType = exists ? WatcherChangeTypes.Changed : WatcherChangeTypes.Deleted;
                }
                else
                {
                    eventType = exists ? WatcherChangeTypes.Created : WatcherChangeTypes.Deleted;
                }

                FileSystemEntryEvent ae = new FileSystemEntryEvent { FileSystem = _fileSystem, ChangeEvents = eventType, Path = RelativePath };
                observer.OnNext(ae);
            }

            /// <summary>
            /// Dispose observer
            /// </summary>
            public void Dispose()
            {
                var _watcher = watcher;
                var _observer = observer;

                // Clear file system reference, and remove watcher from dispose list.
                IFileSystem _fileSystem = Interlocked.Exchange(ref fileSystem, null);
                if (_fileSystem is FileSystemBase __fileSystem) __fileSystem.RemoveDisposableBase(this);

                StructList2<Exception> errors = new StructList2<Exception>();
                if (_observer != null)
                {
                    observer = null;
                    try
                    {
                        _observer.OnCompleted();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                if (_watcher != null)
                {
                    watcher = null;
                    try
                    {
                        _watcher.Dispose();
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                if (errors.Count > 0) throw new AggregateException(errors);
                fileProvider = null;
            }
        }

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        public FileProviderSystem AddDisposable(object disposable) => AddDisposableBase(disposable) as FileProviderSystem;

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public FileProviderSystem RemoveDisposable(object disposable) => RemoveDisposableBase(disposable) as FileProviderSystem;
    }

    /// <summary>
    /// File System that represents embedded resources of an <see cref="Assembly"/>.
    /// </summary>
    public class EmbeddedFileSystem : FileSystemBase, IFileSystemBrowse, IFileSystemOpen
    {
        /// <summary>
        /// Associated Assembly
        /// </summary>
        public readonly Assembly Assembly;

        /// <summary>
        /// Get capabilities.
        /// </summary>
        public override FileSystemCapabilities Capabilities => FileSystemCapabilities.Browse | FileSystemCapabilities.Open | FileSystemCapabilities.Read;

        /// <summary>
        /// Snapshot of entries.
        /// </summary>
        protected FileSystemEntry[] entries;

        /// <summary>
        /// Create embedded 
        /// </summary>
        /// <param name="assembly"></param>
        public EmbeddedFileSystem(Assembly assembly)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        /// <summary>
        /// Create a snapshot of entries.
        /// </summary>
        /// <returns></returns>
        protected FileSystemEntry[] CreateEntries()
        {
            string[] names = Assembly.GetManifestResourceNames();

            // Get file time, or use Unix time 0.
            DateTimeOffset time;
            if (Assembly.Location != null && File.Exists(Assembly.Location))
                time = new FileInfo(Assembly.Location).LastWriteTimeUtc;
            else
                time = DateTimeOffset.FromUnixTimeSeconds(0L);

            FileSystemEntry[] result = new FileSystemEntry[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = new FileSystemEntry
                {
                    FileSystem = this,
                    LastModified = time,
                    Length = -1L,
                    Name = names[i],
                    Path = names[i],
                    Type = FileSystemEntryType.File
                };
            }
            return result;
        }

        /// <summary>
        /// Browse a list of embedded resources.
        /// 
        /// For example:
        ///     "assembly.res1"
        ///     "assembly.res2"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileSystemEntry[] Browse(string path)
            => entries ?? (entries = CreateEntries());

        /// <summary>
        /// Open embedded resource for reading.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileMode"></param>
        /// <param name="fileAccess"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (fileMode != FileMode.Open) throw new IOException($"Cannot open embedded resouce in FileMode={fileMode}");
            if (fileAccess != FileAccess.Read) throw new IOException($"Cannot open embedded resouce in FileAccess={fileAccess}");
            Stream s = Assembly.GetManifestResourceStream(path);
            if (s == null) throw new FileNotFoundException(path);
            return s;
        }

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        public EmbeddedFileSystem AddDisposable(object disposable) => AddDisposableBase(disposable) as EmbeddedFileSystem;

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public EmbeddedFileSystem RemoveDisposable(object disposable) => RemoveDisposableBase(disposable) as EmbeddedFileSystem;

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Assembly.FullName;
    }

    /// <summary>
    /// Composition of multiple <see cref="IFileSystem"/>s.
    /// </summary>
    public class FileSystemComposition : FileSystemBase, IEnumerable<IFileSystem>, IFileSystemBrowse, IFileSystemObserve, IFileSystemOpen, IFileSystemDelete, IFileSystemMove, IFileSystemCreateDirectory
    {
        /// <summary>
        /// File system components.
        /// </summary>
        protected IFileSystem[] fileSystems;

        /// <summary>
        /// Count 
        /// </summary>
        public int Count => fileSystems.Length;

        /// <summary>
        /// Union of capabilities
        /// </summary>
        protected FileSystemCapabilities capabilities;

        /// <summary>
        /// Union of capabilities.
        /// </summary>
        public override FileSystemCapabilities Capabilities => capabilities;

        /// <summary>
        /// Create composition of file systems
        /// </summary>
        /// <param name="fileSystems"></param>
        public FileSystemComposition(params IFileSystem[] fileSystems)
        {
            this.fileSystems = fileSystems;
            foreach (IFileSystem fs in fileSystems) capabilities |= fs.Capabilities;
        }

        /// <summary>
        /// Create colletion of file systems
        /// </summary>
        /// <param name="fileSystems"></param>
        public FileSystemComposition(IEnumerable<IFileSystem> fileSystems)
        {
            this.fileSystems = fileSystems.ToArray();
            foreach (IFileSystem fs in this.fileSystems) capabilities |= fs.Capabilities;
        }

        /// <summary>
        /// Browse a directory for file and subdirectory entries.
        /// </summary>
        /// <param name="path">path to directory, "" is root, separator is "/"</param>
        /// <returns>a snapshot of file and directory entries</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support browse</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public FileSystemEntry[] Browse(string path)
        {
            StructList24<FileSystemEntry> entries = new StructList24<FileSystemEntry>();
            bool exists = false, supported = false;
            foreach (var filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.Browse) == 0UL) continue;
                try
                {
                    FileSystemEntry[] list = filesystem.Browse(path);
                    exists = true; supported = true;
                    foreach (FileSystemEntry e in list)
                        entries.Add(new FileSystemEntry { FileSystem = this, Name = e.Name, Path = e.Path, Length = e.Length, Type = FileSystemEntryType.File, LastModified = e.LastModified });
                }
                catch (DirectoryNotFoundException) { supported = true; }
                catch (NotSupportedException) { }
            }
            if (!supported) throw new NotSupportedException(nameof(Browse));
            if (!exists) throw new DirectoryNotFoundException(path);
            return entries.ToArray();
        }

        /// <summary>
        /// Open a file for reading and/or writing. File can be created when <paramref name="fileMode"/> is <see cref="FileMode.Create"/> or <see cref="FileMode.CreateNew"/>.
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". Root is without preceding "/", e.g. "dir/file.xml"</param>
        /// <param name="fileMode">determines whether to open or to create the file</param>
        /// <param name="fileAccess">how to access the file, read, write or read and write</param>
        /// <param name="fileShare">how the file will be shared by processes</param>
        /// <returns>open file stream</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support opening files</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and and the file specified by path does not exist. The file must already exist in these modes.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileMode"/>, <paramref name="fileAccess"/> or <paramref name="fileShare"/> contains an invalid value.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            bool supported = false;
            foreach (var filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.Open) == 0UL) continue;
                try
                {
                    return filesystem.Open(path, fileMode, fileAccess, fileShare);
                }
                catch (FileNotFoundException) { supported = true; }
                catch (NotSupportedException) { }
            }
            if (!supported) throw new NotSupportedException(nameof(Browse));
            throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Delete a file or directory.
        /// 
        /// If <paramref name="recursive"/> is false and <paramref name="path"/> is a directory that is not empty, then <see cref="IOException"/> is thrown.
        /// If <paramref name="recursive"/> is true, then any file or directory within <paramref name="path"/> is deleted as well.
        /// </summary>
        /// <param name="path">path to a file or directory</param>
        /// <param name="recursive">if path refers to directory, recurse into sub directories</param>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error, or if <paramref name="path"/> refered to a directory that wasn't empty and <paramref name="recursive"/> is false</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support deleting files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="path"/> refers to non-file device</exception>
        /// <exception cref="ObjectDisposedException"/>
        public void Delete(string path, bool recursive = false)
        {
            bool supported = false;
            bool ok = false;
            foreach (var filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.Delete) == 0UL) continue;
                try
                {
                    filesystem.Delete(path, recursive);
                    ok = true; supported = true;
                }
                catch (FileNotFoundException) { supported = true; }
                catch (NotSupportedException) { }
            }
            if (!supported) throw new NotSupportedException(nameof(Browse));
            if (!ok) throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Try to move/rename a file or directory.
        /// </summary>
        /// <param name="oldPath">old path of a file or directory</param>
        /// <param name="newPath">new path of a file or directory</param>
        /// <exception cref="FileNotFoundException">The specified <paramref name="oldPath"/> is invalid.</exception>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="FileNotFoundException">The specified path is invalid.</exception>
        /// <exception cref="ArgumentNullException">path is null</exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support renaming/moving files</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">path refers to non-file device, or an entry already exists at <paramref name="newPath"/></exception>
        /// <exception cref="ObjectDisposedException"/>
        public void Move(string oldPath, string newPath)
        {
            bool supported = false;
            bool ok = false;
            foreach (IFileSystem filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.Move) == 0UL) continue;
                try
                { 
                    filesystem.Move(oldPath, newPath);
                    ok = true; supported = true;
                }
                catch (FileNotFoundException) { supported = true; }
                catch (NotSupportedException) { }
            }
            if (!supported) throw new NotSupportedException(nameof(Browse));
            if (!ok) throw new FileNotFoundException(oldPath);
        }

        /// <summary>
        /// Create a directory, or multiple cascading directories.
        /// 
        /// If directory at <paramref name="path"/> already exists, then returns without exception.
        /// </summary>
        /// <param name="path">Relative path to file. Directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <returns>true if directory exists after the method, false if directory doesn't exist</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support create directory</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public void CreateDirectory(string path)
        {
            bool supported = false;
            bool ok = false;
            foreach (IFileSystem filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.CreateDirectory) == 0UL) continue;
                try
                {
                    filesystem.CreateDirectory(path);
                    ok = true; supported = true;
                }
                catch (FileNotFoundException) { supported = true; }
                catch (NotSupportedException) { }
            }
            if (!supported) throw new NotSupportedException(nameof(Browse));
            if (!ok) throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Attach an <paramref name="observer"/> on to a single file or directory. 
        /// Observing a directory will observe the whole subtree.
        /// </summary>
        /// <param name="path">path to file or directory. The directory separator is "/". The root is without preceding slash "", e.g. "dir/dir2"</param>
        /// <param name="observer"></param>
        /// <returns>dispose handle</returns>
        /// <exception cref="IOException">On unexpected IO error</exception>
        /// <exception cref="SecurityException">If caller did not have permission</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is an empty string (""), contains only white space, or contains one or more invalid characters</exception>
        /// <exception cref="NotSupportedException">The <see cref="IFileSystem"/> doesn't support observe</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="path"/> refers to a non-file device, such as "con:", "com1:", "lpt1:", etc.</exception>
        /// <exception cref="ObjectDisposedException"/>
        public IDisposable Observe(string path, IObserver<FileSystemEntryEvent> observer)
        {
            StructList12<IDisposable> disposables = new StructList12<IDisposable>();
            ObserverAdapter adapter = new ObserverAdapter(this, observer);
            foreach (var filesystem in fileSystems)
            {
                if ((filesystem.Capabilities & FileSystemCapabilities.Observe) == 0UL) continue;
                try
                {
                    IDisposable disposable = filesystem.Observe(path, adapter);
                    disposables.Add(disposable);
                }
                catch (NotSupportedException) { }
            }
            if (disposables.Count == 0) throw new NotSupportedException(nameof(Observe));
            adapter.disposables = disposables.ToArray();
            return adapter;
        }

        class ObserverAdapter : IDisposable, IObserver<FileSystemEntryEvent>
        {
            IFileSystem filesystem;
            IObserver<FileSystemEntryEvent> observer;
            public IDisposable[] disposables;

            public ObserverAdapter(IFileSystem filesystem, IObserver<FileSystemEntryEvent> observer)
            {
                this.filesystem = filesystem;
                this.observer = observer;
            }

            public void OnCompleted()
                => observer.OnCompleted();

            public void OnError(Exception error)
                => observer.OnError(error);

            public void OnNext(FileSystemEntryEvent value)
                => observer.OnNext(new FileSystemEntryEvent { FileSystem = filesystem, ChangeEvents = value.ChangeEvents, Path = value.Path });

            public void Dispose()
            {
                StructList4<Exception> errors = new StructList4<Exception>();
                foreach (IDisposable d in disposables)
                {
                    try
                    {
                        d.Dispose();
                    }
                    catch (AggregateException ae)
                    {
                        foreach(Exception e in ae.InnerExceptions) errors.Add(e);
                    }
                    catch (Exception e)
                    {
                        errors.Add(e);
                    }
                }

                if (errors.Count > 0) throw new AggregateException(errors);
            }
        }

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        public FileSystemComposition AddDisposable(object disposable) => AddDisposableBase(disposable) as FileSystemComposition;

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public FileSystemComposition RemoveDisposable(object disposable) => RemoveDisposableBase(disposable) as FileSystemComposition;

        /// <summary>
        /// Get file systems
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFileSystem> GetEnumerator()
            => ((IEnumerable<IFileSystem>)fileSystems).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => fileSystems.GetEnumerator();

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => String.Join<IFileSystem>(", ", fileSystems);

    }

    /// <summary>
    /// Adapts <see cref="Lexical.FileSystem.IFileSystem"/> to <see cref="Lexical.Localization.Asset.IFileSystem"/>.
    /// </summary>
    public class FileSystemAdapter : FileSystemBase, IFileSystem, IFileSystemBrowse, IFileSystemOpen, IFileSystemObserve, IFileSystemMove, IFileSystemDelete, IFileSystemCreateDirectory
    {
        /// <summary>
        /// <see cref="Lexical.FileSystem.IFileSystem"/>
        /// </summary>
        public readonly Lexical.FileSystem.IFileSystem FileSystem;

        /// <summary>
        /// Create adapter that adapts <see cref="Lexical.FileSystem.IFileSystem"/> to <see cref="Lexical.Localization.Asset.IFileSystem"/>.
        /// </summary>
        /// <param name="fileSystem"></param>
        public FileSystemAdapter(Lexical.FileSystem.IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <inheritdoc/>
        public FileSystemEntry[] Browse(string path)
        {
            Lexical.FileSystem.FileSystemEntry[] files = FileSystem.Browse(path);
            FileSystemEntry[] result = new FileSystemEntry[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                Lexical.FileSystem.FileSystemEntry e = files[i];
                result[i] = new FileSystemEntry { FileSystem = this, LastModified = e.LastModified, Length = e.Length, Name = e.Name, Path = e.Path, Type = (FileSystemEntryType)(UInt32)e.Type };
            }
            return result;
        }

        /// <inheritdoc/>
        public void CreateDirectory(string path)
            => FileSystem.CreateDirectory(path);

        /// <inheritdoc/>
        public void Delete(string path, bool recursive = false)
            => FileSystem.Delete(path, recursive);

        /// <inheritdoc/>
        public void Move(string oldPath, string newPath)
            => FileSystem.Move(oldPath, newPath);

        /// <inheritdoc/>
        public Stream Open(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
            => FileSystem.Open(path, fileMode, fileAccess, fileShare);

        /// <inheritdoc/>
        public IDisposable Observe(string path, IObserver<FileSystemEntryEvent> observer)
            => FileSystem.Observe(path, new Observer(this, observer));

        /// <summary>
        /// Observer adapter
        /// </summary>
        class Observer : IObserver<Lexical.FileSystem.FileSystemEntryEvent>
        {
            IObserver<FileSystemEntryEvent> observer;
            IFileSystem parent;

            public Observer(IFileSystem parent, IObserver<FileSystemEntryEvent> observer)
            {
                this.parent = parent;
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
            }

            public void OnCompleted()
                => observer.OnCompleted();

            public void OnError(Exception error)
                => observer.OnError(error);

            public void OnNext(Lexical.FileSystem.FileSystemEntryEvent value)
                => observer.OnNext(new FileSystemEntryEvent
                {
                    ChangeEvents = value.ChangeEvents,
                    FileSystem = parent,
                    Path = value.Path
                });
        }

        /// <summary>
        /// Add <paramref name="disposable"/> to list of objects to be disposed along with the system.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns>filesystem</returns>
        public FileSystemAdapter AddDisposable(object disposable) => AddDisposableBase(disposable) as FileSystemAdapter;

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public FileSystemAdapter RemoveDisposable(object disposable) => RemoveDisposableBase(disposable) as FileSystemAdapter;

        /// <summary>
        /// Print info
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => FileSystem.ToString();
    }

}