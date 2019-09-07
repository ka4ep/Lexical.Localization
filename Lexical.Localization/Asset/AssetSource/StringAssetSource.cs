// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.StringFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Asset source that refers and reads an individual string asset file from abstract filesystem source.
    /// 
    /// See the sub-classes:
    /// <list type="table">
    ///     <item><see cref="EmbeddedStringAssetFile"/></item>
    /// </list>
    /// 
    /// </summary>
    public abstract class StringAssetSource : IAssetSource, IFileAssetSource, IStringAssetSource, ILineAssetSource, IUnformedLineAssetSource, ILineTreeAssetSource
    {
        /// <summary>
        /// Reference to an asset file. Used within <see cref="IFileSystem"/>. Directory separator is '/'. Root doesn't use separator.
        /// 
        /// Example: "resources/localization.xml".
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// File format.
        /// </summary>
        public ILineFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// (Optional) Line format, if file format needs one. 
        /// </summary>
        public ILineFormat LineFormat { get; protected set; }

        /// <summary>
        /// Can read file as <see cref="ILine"/> enumerable. (for example .xml, ini, .json)
        /// </summary>
        public virtual bool EnumeratesLines => FileFormat is ILineStreamReader || FileFormat is ILineTextReader;

        /// <summary>
        /// Can read file as KeyValuePair&lt;<see cref="String"/>,<see cref="IString"/>&gt; enumerable (for example .resx).
        /// </summary>
        public bool EnumeratesUnformedLines => FileFormat is IUnformedLineStreamReader || FileFormat is IUnformedLineTextReader;

        /// <summary>
        /// Can read file as <see cref="ILineTree"/>.
        /// </summary>
        public bool EnumeratesLineTree => FileFormat is ILineTreeStreamReader || FileFormat is ILineTreeTextReader;

        /// <summary>
        /// Asset source can read string lines.
        /// </summary>
        public bool IsStringSource => true;

        /// <summary>
        /// Create asset source to string asset file.
        /// </summary>
        protected StringAssetSource()
        {
        }

        /// <summary>
        /// Create asset source to embedded string asset file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileFormat">File format</param>
        /// <param name="lineFormat">(optional) Line format. Required if <paramref name="fileFormat"/> is unformed (e.g. .resx, .resources)</param>
        protected StringAssetSource(string filePath, ILineFileFormat fileFormat, ILineFormat lineFormat)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            LineFormat = lineFormat;
        }

        /// <summary>
        /// Open stream to the file source.
        /// </summary>
        /// <returns></returns>
        public abstract Stream OpenStream();

        /// <summary>
        /// Read source into lines
        /// </summary>
        /// <returns>lines</returns>
        IEnumerator<ILine> IEnumerable<ILine>.GetEnumerator()
        {
            // Open file
            using (Stream s = OpenStream())
            {
                // Read lines
                IEnumerable<ILine> lines = FileFormat.ReadLines(s, LineFormat);
                // Return
                return lines.GetEnumerator();
            }
        }

        /// <summary>
        /// Read source into unformed lines.
        /// </summary>
        /// <returns>lines</returns>
        IEnumerator<KeyValuePair<string, IString>> IEnumerable<KeyValuePair<string, IString>>.GetEnumerator()
        {
            // Open file
            using (Stream s = OpenStream())
            {
                // Read lines
                IEnumerable<KeyValuePair<string, IString>> lines = FileFormat.ReadUnformedLines(s, LineFormat);
                // Return
                return lines.GetEnumerator();
            }
        }

        /// <summary>
        /// Read source into tree.
        /// </summary>
        /// <returns>lines</returns>
        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            // Open file
            using (Stream s = OpenStream())
            {
                // Read lines
                ILineTree lines = FileFormat.ReadLineTree(s, LineFormat);
                // Return
                return ((IEnumerable<ILineTree>)new ILineTree[] { lines }).GetEnumerator();
            }
        }

        /// <summary>
        /// Read source into lines
        /// </summary>
        /// <returns>lines</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Open file
            using (Stream s = OpenStream())
            {
                // Read lines
                IEnumerable<ILine> lines = FileFormat.ReadLines(s, LineFormat);
                // Return
                return lines.GetEnumerator();
            }
        }

        /// <summary>
        /// Print info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().Name + "(" + FilePath + ")";
    }

    /// <summary>
    /// Asset source that refers to and reads an individual string asset file from an <see cref="Assembly"/>.
    /// </summary>
    public class EmbeddedStringAssetFile : StringAssetSource
    {
        /// <summary>
        /// Refered Assembly.
        /// </summary>
        public Assembly Assembly { get; protected set; }

        /// <summary>
        /// Create asset source to embedded string asset resource (for examples .resx).
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="fileFormat">File format</param>
        /// <param name="lineFormat">(optional) Line format. Required if <paramref name="fileFormat"/> is unformed (e.g. .resx, .resources)</param>
        public EmbeddedStringAssetFile(Assembly assembly, string resourceName, ILineFileFormat fileFormat, ILineFormat lineFormat) : base(resourceName, fileFormat, lineFormat)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        /// <summary>
        /// Open resource
        /// </summary>
        /// <returns></returns>
        public override Stream OpenStream()
            => Assembly.GetManifestResourceStream(FilePath) ?? throw new FileNotFoundException(FilePath);

        /// <summary>
        /// Compare references.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is EmbeddedStringAssetFile otherSource ? otherSource.Assembly.Equals(Assembly) && otherSource.FilePath.Equals(FilePath) : false;

        /// <summary>
        /// Calculate hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => FilePath.GetHashCode() ^ Assembly.FullName.GetHashCode();

        /// <summary>
        /// Print info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().Name + "(" + Assembly.FullName + ", " + FilePath + ")";
    }

    /// <summary>
    /// Asset source that refers to and reads an individual string asset file from abstract filesystem source.
    /// </summary>
    public class FileStringAssetFile : StringAssetSource, IObservableAssetSource
    {
        /// <summary>
        /// Is source observable.
        /// </summary>
        public virtual bool IsObservable => true;

        /// <summary>
        /// Create asset source to a string asset file (for examples .xml, .ini).
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileFormat">File format</param>
        /// <param name="lineFormat">(optional) Line format. Required if <paramref name="fileFormat"/> is unformed (e.g. .resx, .resources)</param>
        public FileStringAssetFile(string filePath, ILineFileFormat fileFormat, ILineFormat lineFormat) : base(filePath, fileFormat, lineFormat)
        {
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <returns></returns>
        public override Stream OpenStream()
            => new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        /// <summary>
        /// Subscribe to monitoring the asset file.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<IAssetSourceEvent> observer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compare references.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is FileStringAssetFile otherSource ? otherSource.FilePath.Equals(FilePath) : false;

        /// <summary>
        /// Calculate hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => FilePath.GetHashCode();

        /// <summary>
        /// Print info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().Name + "(" + FilePath + ")";

        /// <summary>
        /// File watcher.
        /// </summary>
        public class Watcher : IDisposable
        {
            /// <summary>
            /// Absolute path as OS path. Separator is '\\' or '/'.
            /// </summary>
            public string AbsolutePath { get; protected set; }

            /// <summary>
            /// Relative path. 
            /// </summary>
            public string FilePath { get; protected set; }

            /// <summary>
            /// Associated asset source.
            /// </summary>
            public IAssetSource AssetSource { get; protected set; }

            /// <summary>
            /// Watcher
            /// </summary>
            protected FileSystemWatcher watcher;

            /// <summary>
            /// Callback object.
            /// </summary>
            protected IObserver<IAssetSourceEvent> observer;

            /// <summary>
            /// Create observer for one file.
            /// </summary>
            /// <param name="assetSource">associated file system</param>
            /// <param name="filePath">Relative or absolute path</param>
            /// <param name="observer">observer for callbacks</param>
            public Watcher(IAssetSource assetSource, string filePath, IObserver<IAssetSourceEvent> observer)
            {
                this.AssetSource = assetSource ?? throw new ArgumentNullException(nameof(assetSource));
                this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
                this.AbsolutePath = Path.GetFullPath(FilePath);
                this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
                FileInfo fi = new FileInfo(AbsolutePath);
                // Watch file
                if (fi.Exists)
                {
                    watcher = new FileSystemWatcher(fi.Directory.FullName, fi.Name);
                    watcher.IncludeSubdirectories = false;
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
                    int ix = FilePath.LastIndexOf('/');
                    WatcherDirectoryRelativePath = ix < 0 ? "" : FilePath.Substring(0, ix);
                } else
                // Find parent that is found.
                {
                    DirectoryInfo di = fi.Directory;
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
                FileSystemEntryEvent ae = new FileSystemEntryEvent { FileSystem = _fileSystem, ChangeEvents = e.ChangeType, Path = WatcherDirectoryRelativePath == "" ? e.Name : WatcherDirectoryRelativePath + "/" + e.Name };
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

    }

}