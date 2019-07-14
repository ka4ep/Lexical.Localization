// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           14.7.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// File system based <see cref="IFileSystem"/> that loads localization files.
    /// 
    /// If file watchers have been created, and file system is disposed, then watchers will be disposed also. 
    /// <see cref="IObserver{T}.OnCompleted"/> event is forwarded to watchers.
    /// </summary>
    public class FileSystem : AbstractFileSystem, IFileSystem, IFileSystemBrowser, IFileSystemObserver, IFileSystemReader, IFileSystemWriter
    {
        static FileSystem osRoot = new FileSystem("", false);
        static Lazy<FileSystem> applicationRoot = new Lazy<FileSystem>( () => new FileSystem(AppDomain.CurrentDomain.BaseDirectory, false) );

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
        /// Allow relative paths over root's parent. 
        /// 
        /// For example, relative "../dir/file.json" is over root's parent. If value is true, then this relative path is permitted.
        /// </summary>
        public readonly bool AllowRootParent;

        /// <summary>
        /// Full absolute root path.
        /// <see cref="RootPath"/> ran with <see cref="System.IO.Path.GetFullPath(string)"/>.
        /// </summary>
        public readonly string AbsoluteRootPath;
        
        /// <summary>
        /// Create asset file system
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="allowRootParent">if true, allows relative paths that go over root's parent, e.g. "../somefile"</param>
        public FileSystem(string rootPath, bool allowRootParent = false) : base()
        {
            RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            AbsoluteRootPath = System.IO.Path.GetFullPath(rootPath);
            AllowRootParent = allowRootParent;
        }

        /// <summary>
        /// Try to open a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public bool TryRead(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            string concatenatedPath = System.IO.Path.Combine(AbsoluteRootPath, path);
            string absolutePath = System.IO.Path.GetFullPath(concatenatedPath);
            if (!AllowRootParent && !absolutePath.StartsWith(AbsoluteRootPath)) { stream = default; return false; }
            FileInfo fi = new FileInfo(absolutePath);
            if (!fi.Exists) { stream = default; return false; }
            stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, fileShare);
            return true;
        }

        /// <summary>
        /// Try to open a file for writing (and reading).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public bool TryWrite(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            string concatenatedPath = System.IO.Path.Combine(AbsoluteRootPath, path);
            string absolutePath = System.IO.Path.GetFullPath(concatenatedPath);
            if (!AllowRootParent && !absolutePath.StartsWith(AbsoluteRootPath)) { stream = default; return false; }
            stream = new FileStream(absolutePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, fileShare);
            return true;
        }

        /// <summary>
        /// Try to observe a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="observer"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool TryObserve(string filePath, IObserver<IFileSystemEntryEvent> observer, out IDisposable handle)
        {
            string concatenatedPath = System.IO.Path.Combine(AbsoluteRootPath, filePath);
            string absolutePath = System.IO.Path.GetFullPath(concatenatedPath);
            if (!AllowRootParent && !absolutePath.StartsWith(AbsoluteRootPath)) { handle = default; return false; }
            handle = new Watcher(this, observer, absolutePath, filePath);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool TryBrowse(string path, out FileSystemEntry[] files)
        {
            string concatenatedPath = RootPath == null ? path : (RootPath.EndsWith("/") || RootPath.EndsWith("\\")) ? RootPath + path : RootPath + "/" + path;

            DirectoryInfo dir = new DirectoryInfo(concatenatedPath);
            if (!dir.Exists) { files = default; return false; }

            StructList24<FileSystemEntry> list = new StructList24<FileSystemEntry>();
            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                list.Add(new FileSystemEntry { FileSystem = this, LastModified = di.LastWriteTimeUtc, Name = di.Name, Path = concatenatedPath + "/" + di.Name, Length = -1L, Type = FileSystemEntryType.Directory });
            }
            foreach (FileInfo fi in dir.GetFiles())
            {
                list.Add(new FileSystemEntry { FileSystem = this, LastModified = fi.LastWriteTimeUtc, Name = fi.Name, Path = concatenatedPath + "/" + fi.Name, Length = fi.Length, Type = FileSystemEntryType.File });
            }
            files = list.ToArray();
            return true;
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
            /// Watcher
            /// </summary>
            protected FileSystemWatcher watcher;

            /// <summary>
            /// Callback object.
            /// </summary>
            protected IObserver<IFileSystemEntryEvent> observer;

            /// <summary>
            /// Create observer for one file.
            /// </summary>
            /// <param name="fileSystem">associated file system</param>
            /// <param name="observer">observer for callbacks</param>
            /// <param name="absolutePath">Absolute path</param>
            /// <param name="relativePath">Relative path (separator is '/')</param>
            public Watcher(IFileSystem fileSystem, IObserver<IFileSystemEntryEvent> observer, string absolutePath, string relativePath)
            {
                this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
                this.AbsolutePath = absolutePath ?? throw new ArgumentNullException(nameof(absolutePath));
                this.RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
                relativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
                FileInfo fi = new FileInfo(relativePath);
                watcher = new FileSystemWatcher(fi.Directory.FullName, fi.Name);
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                watcher.IncludeSubdirectories = false;
                watcher.Changed += OnEvent;
                watcher.Created += OnEvent;
                watcher.Deleted += OnEvent;
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
                IFileSystemEntryEvent ae = new FileSystemEvent(_fileSystem, e.ChangeType, RelativePath);
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
                if (_fileSystem is AbstractFileSystem __fileSystem) __fileSystem.RemoveDisposable(this);

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
    }

    /// <summary>
    /// Default implementation of <see cref="IFileSystemEntryEvent"/>. 
    /// Used by <see cref="IFileSystem"/> implementations.
    /// </summary>
    public class FileSystemEvent : IFileSystemEntryEvent
    {
        /// <summary>
        /// System that sent the event.
        /// </summary>
        public IFileSystem FileSystem { get; protected set; }

        /// <summary>
        /// Event Change type
        /// </summary>
        public WatcherChangeTypes ChangeEvents { get; protected set; }

        /// <summary>
        /// Affected file, relative path.
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// Change type
        /// </summary>
        /// <param name="system"></param>
        /// <param name="changeEvents"></param>
        /// <param name="filePath">affected file path</param>
        public FileSystemEvent(IFileSystem system, WatcherChangeTypes changeEvents, string filePath)
        {
            FileSystem = system;
            ChangeEvents = changeEvents;
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }

    /// <summary>
    /// Base implementation for <see cref="IFileSystem"/>. 
    /// 
    /// Disposables can be attached to be disposed along with <see cref="IFileSystem"/>.
    /// Watchers can be attached as disposables, so that they forward <see cref="IObserver{T}.OnCompleted"/> event upon IFileSystem dispose.
    /// </summary>
    public abstract class AbstractFileSystem : IFileSystem, IDisposable
    {
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
        public IFileSystem AddDisposable(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            lock (m_lock)
            {
                if (this.disposeList == null) this.disposeList = new List<IDisposable>();
                this.disposeList.Add(disposable);
            }
            return this;
        }

        /// <summary>
        /// Remove disposable from dispose list.
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        public IFileSystem RemoveDisposable(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            lock (m_lock)
            {
                if (this.disposeList != null) this.disposeList.Remove(disposable);
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
    public class FileSystemProvider : AbstractFileSystem, IFileSystemBrowser, IFileSystemObserver, IFileSystemReader
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
        /// Create file provider based file system.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="subpath">(optional) subpath within the file provider</param>
        public FileSystemProvider(IFileProvider fileProvider, string subpath = null) : base()
        {
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(subpath));
            this.SubPath = subpath;
        }

        /// <summary>
        /// Try to open file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="fileShare">(not used)</param>
        /// <returns></returns>
        public bool TryRead(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) { stream = null; return false; }
            // Does file exist?
            IFileInfo fi = fp.GetFileInfo(concatenatedPath);
            if (!fi.Exists) { stream = null; return false; }
            // Read
            Stream s = fi.CreateReadStream();
            if (s == null) { stream = null; return false; }
            // Failed
            stream = s;
            return true;
        }

        /// <summary>
        /// Try to observe file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="observer"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool TryObserve(string path, IObserver<IFileSystemEntryEvent> observer, out IDisposable handle)
        {
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) { handle = null; return false; }
            // Observe
            handle = new Watcher(this, fp, observer, concatenatedPath, path);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool TryBrowse(string path, out FileSystemEntry[] files)
        {
            // Make path
            string concatenatedPath = SubPath == null ? path : (SubPath.EndsWith("/") || SubPath.EndsWith("\\")) ? SubPath + path : SubPath + "/" + path;
            // Is disposed?
            IFileProvider fp = fileProvider;
            if (fp == null) { files = null; return false; }
            // Browse
            IDirectoryContents contents = fp.GetDirectoryContents(concatenatedPath);
            if (!contents.Exists) { files = null; return false; }
            // Convert result
            StructList24<FileSystemEntry> list = new StructList24<FileSystemEntry>();
            foreach (IFileInfo fi in contents)
            {
                list.Add(new FileSystemEntry { FileSystem = this, LastModified = fi.LastModified, Name = fi.Name, Path = concatenatedPath + "/" + fi.Name, Length = fi.IsDirectory?-1L:fi.Length, Type = fi.IsDirectory?FileSystemEntryType.Directory:FileSystemEntryType.File });
            }
            files = list.ToArray();
            return true;
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
            protected IObserver<IFileSystemEntryEvent> observer;

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
            /// Create observer for one file.
            /// </summary>
            /// <param name="system"></param>
            /// <param name="fileProvider"></param>
            /// <param name="observer"></param>
            /// <param name="fileProviderPath">Absolute path</param>
            /// <param name="relativePath">Relative path (separator is '/')</param>
            public Watcher(IFileSystem system, IFileProvider fileProvider, IObserver<IFileSystemEntryEvent> observer, string fileProviderPath, string relativePath)
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

                IFileSystemEntryEvent ae = new FileSystemEvent(_fileSystem, eventType, RelativePath);
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
                if (_fileSystem is AbstractFileSystem __fileSystem) __fileSystem.RemoveDisposable(this);

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

    }

    /// <summary>
    /// Composition of multiple <see cref="IFileSystem"/>s.
    /// </summary>
    public class FileSystemComposition : AbstractFileSystem, IEnumerable<IFileSystem>, IFileSystemBrowser, IFileSystemObserver, IFileSystemReader, IFileSystemWriter
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
        /// Create composition of file systems
        /// </summary>
        /// <param name="fileSystems"></param>
        public FileSystemComposition(params IFileSystem[] fileSystems)
        {
            this.fileSystems = fileSystems;
        }

        /// <summary>
        /// Create colletion of file systems
        /// </summary>
        /// <param name="fileSystems"></param>
        public FileSystemComposition(IEnumerable<IFileSystem> fileSystems)
        {
            this.fileSystems = fileSystems.ToArray();
        }

        /// <summary>
        /// Try to list files and directories.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool TryBrowse(string path, out FileSystemEntry[] files)
        {
            StructList24<FileSystemEntry> entries = new StructList24<FileSystemEntry>();
            bool exists = false;
            foreach (var filesystem in fileSystems)
            {
                FileSystemEntry[] list;
                if (!filesystem.TryBrowse(path, out list)) continue;
                foreach (FileSystemEntry e in list)
                    entries.Add( new FileSystemEntry { FileSystem = this, Name = e.Name, Path = e.Path, Length = e.Length, Type = FileSystemEntryType.File, LastModified = e.LastModified });
                exists = true;
            }
            files = exists ? entries.ToArray() : null;
            return exists;
        }

        /// <summary>
        /// Try to observe a directory or a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="observer"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool TryObserve(string path, IObserver<IFileSystemEntryEvent> observer, out IDisposable handle)
        {
            StructList12<IDisposable> disposables = new StructList12<IDisposable>();
            foreach (var filesystem in fileSystems)
            {
                IDisposable disposable;
                if (filesystem.TryObserve(path, observer, out disposable)) disposables.Add(disposable);
            }
            if (disposables.Count == 1) { handle = disposables[0]; return true; }
            if (disposables.Count > 1) { handle = new JointObserverHandle(disposables.ToArray()); return true; }
            handle = null;
            return false;
        }

        /// <summary>
        /// Try to open a file for reading.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public bool TryRead(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            foreach (var filesystem in fileSystems)
                if (filesystem.TryRead(path, out stream, fileShare)) return true;
            stream = null;
            return false;
        }

        /// <summary>
        /// Try to open a file for writing.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <param name="fileShare"></param>
        /// <returns></returns>
        public bool TryWrite(string path, out Stream stream, FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        {
            foreach (var filesystem in fileSystems)
                if (filesystem.TryWrite(path, out stream, fileShare)) return true;
            stream = null;
            return false;
        }

        /// <summary>
        /// Get file systems
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFileSystem> GetEnumerator()
            => ((IEnumerable<IFileSystem>)fileSystems).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => fileSystems.GetEnumerator();

        class JointObserverHandle : IDisposable
        {
            IDisposable[] disposables;

            public JointObserverHandle(IDisposable[] disposables)
            {
                this.disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
            }

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
    }

}