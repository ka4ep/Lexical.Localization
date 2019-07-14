//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// Asset file source.
    /// </summary>
    public abstract class FileSource : IAssetSource//, IAssetSourceObservable
    {
        /// <summary>
        /// (optional) Overriding explicit path. 
        /// 
        /// If null then <see cref="IAssetLoader"/> determines the folder to use, which is typically application root.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// File path that is relative to <see cref="Path"/>, or if <see cref="Path"/> is null, then relative to application path.
        /// If <see cref="FileName"/> is rooted and <see cref="Path"/> is null, then uses <see cref="FileName"/> as is.
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// If true, throws <see cref="FileNotFoundException"/> if file is not found.
        /// If false, returns empty enumerable.
        /// </summary>
        public bool ThrowIfNotFound { get; internal set; }

        /// <summary>
        /// Return default filepath.
        /// 
        /// If <see cref="FileName"/> is rooted, then returns it as is.
        /// If <see cref="Path"/> is provided, then returns combination of <see cref="Path"/> and <see cref="FileName"/>.
        /// If <see cref="Path"/> is null, then returns <see cref="AppDomain.CurrentDomain.BaseDirectory"/> and see <see cref="FileName"/>.
        /// </summary>
        public string FilePath
            => System.IO.Path.IsPathRooted(FileName) ? FileName :
               System.IO.Path.Combine(Path ?? AppDomain.CurrentDomain.BaseDirectory, FileName);

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="throwIfNotFound"></param>
        public FileSource(string path, string filename, bool throwIfNotFound)
        {
            this.Path = path;
            this.FileName = filename ?? throw new ArgumentNullException(nameof(FileName));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void Build(IList<IAsset> list);

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public abstract IAsset PostBuild(IAsset asset);

        /// <summary>
        /// Create a clone with new path value.
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns>clone</returns>
        public abstract FileSource SetPath(string newPath);

        /// <summary>
        /// Subscribe source watcher.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        //public IDisposable Subscribe(IObserver<IFileSourceEvent> observer)
            //=> new FileObserver(this, observer, FilePath);

        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => FileName;
    }

    /// <summary>
    /// A pattern of files, which are resolved to physical file when parameters in requesting key is matched to pattern.
    /// 
    /// For example if pattern is "{Culture/}{Assembly}.dll" then file name is resolved based on "Culture" and "Assembly" parametrs.
    /// </summary>
    public abstract class FilePatternSource : IAssetSource
    {
        /// <summary>
        /// Root path
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// File name pattern.
        /// </summary>
        public ILinePattern FilePattern { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filePattern"></param>
        public FilePatternSource(string path, ILinePattern filePattern)
        {
            this.Path = path ?? throw new ArgumentNullException(nameof(path));
            this.FilePattern = filePattern ?? throw new ArgumentNullException(nameof(FilePattern));
        }

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void Build(IList<IAsset> list);

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public abstract IAsset PostBuild(IAsset asset);

        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => System.IO.Path.Combine(Path, FilePattern.Pattern);
    }
    /*
    /// <summary>
    /// File observer
    /// </summary>
    public class FileObserver : IDisposable
    {
        /// <summary>
        /// Associated source
        /// </summary>
        public readonly IAssetSource AssetSource;

        /// <summary>
        /// Associated observer
        /// </summary>
        IObserver<IFileSourceEvent> Observer;

        /// <summary>
        /// File to observe
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// Watcher class
        /// </summary>
        protected FileSystemWatcher watcher;

        /// <summary>
        /// Create observer for one file.
        /// </summary>
        /// <param name="assetSource"></param>
        /// <param name="observer"></param>
        /// <param name="filePath"></param>
        public FileObserver(IAssetSource assetSource, IObserver<IFileSourceEvent> observer, string filePath)
        {
            this.AssetSource = assetSource ?? throw new ArgumentNullException(nameof(assetSource));
            Observer = observer ?? throw new ArgumentNullException(nameof(observer));
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            FileInfo fi = new FileInfo(filePath);
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
            var _observer = Observer;
            if (_observer == null) return;
            IFileSourceEvent ae = new FileSourceChangedEvent(AssetSource, e.ChangeType);
            Observer.OnNext(ae);
        }

        /// <summary>
        /// Dispose observer
        /// </summary>
        public void Dispose()
        {
            var _watcher = watcher;
            var _observer = Observer;

            StructList4<Exception> errors = new StructList4<Exception>();
            if (_observer != null)
            {
                Observer = null;
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
        }
    }
    */
}
