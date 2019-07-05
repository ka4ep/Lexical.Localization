//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    using Lexical.Localization.Internal;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Primitives;
    using System.Threading;

    /// <summary>
    /// Localization source and reader that reads lines from file provider.
    /// </summary>
    public abstract class FileProviderSource : IAssetSource, IAssetSourceObservable
    {
        /// <summary>
        /// File provider
        /// </summary>
        public IFileProvider FileProvider { get; protected set; }

        /// <summary>
        /// File path in the <see cref="FileProvider"/>.
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// Throw <see cref="FileNotFoundException"/> if file is not found.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// Create source to file in a file provider.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="throwIfNotFound"></param>
        public FileProviderSource(IFileProvider fileProvider, string filepath, bool throwIfNotFound)
        {
            this.FilePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
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
        /// Subscribe source watcher.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<IFileSourceEvent> observer)
            => new FileProviderObserver(this, this.FileProvider, observer, this.FilePath);

        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => FilePath;
    }

    /// <summary>
    /// A pattern of files, which are resolved to physical file when parameters in requesting key is matched to pattern.
    /// 
    /// For example if pattern is "{Culture/}{Assembly}.dll" then file name is resolved based on "Culture" and "Assembly" parametrs.
    /// </summary>
    public abstract class FileProviderPatternSource : IAssetSource
    {
        /// <summary>
        /// File provider
        /// </summary>
        public IFileProvider FileProvider { get; protected set; }

        /// <summary>
        /// Root path
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// File pattern.
        /// </summary>
        public ILinePattern FilePattern { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="path"></param>
        /// <param name="filePattern"></param>
        public FileProviderPatternSource(IFileProvider fileProvider, string path, ILinePattern filePattern)
        {
            this.FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
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

    /// <summary>
    /// File provider observer
    /// </summary>
    public class FileProviderObserver : IDisposable
    {
        /// <summary>
        /// Associated source
        /// </summary>
        protected IAssetSource assetSource;

        /// <summary>
        /// Associated observer
        /// </summary>
        protected IObserver<IFileSourceEvent> observer;

        /// <summary>
        /// File provider
        /// </summary>
        protected IFileProvider fileProvider;

        /// <summary>
        /// File to observe
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// Watcher class
        /// </summary>
        protected IDisposable watcher;

        /// <summary>
        /// Previous state of file existing.
        /// </summary>
        protected int existed;

        /// <summary>
        /// Create observer for one file.
        /// </summary>
        /// <param name="assetSource"></param>
        /// <param name="fileProvider"></param>
        /// <param name="observer"></param>
        /// <param name="filePath"></param>
        public FileProviderObserver(IAssetSource assetSource, IFileProvider fileProvider, IObserver<IFileSourceEvent> observer, string filePath)
        {
            this.assetSource = assetSource ?? throw new ArgumentNullException(nameof(assetSource));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
            this.FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            existed = fileProvider.GetFileInfo(filePath).Exists ? 1 : 0;
            IChangeToken changeToken = fileProvider.Watch(filePath);
            watcher = changeToken.RegisterChangeCallback(OnEvent, this);
        }

        /// <summary>
        /// Forward event
        /// </summary>
        /// <param name="sender"></param>
        void OnEvent(object sender)
        {
            var _observer = observer;
            if (_observer == null) return;

            // Figure out change type
            bool exists = fileProvider.GetFileInfo(FilePath).Exists;
            bool _existed = Interlocked.CompareExchange(ref existed, exists ? 1 : 0, existed) == 1;

            WatcherChangeTypes changeType = default;
            if (_existed)
            {
                changeType = exists ? WatcherChangeTypes.Changed : WatcherChangeTypes.Deleted;
            }
            else
            {
                changeType = exists ? WatcherChangeTypes.Created : WatcherChangeTypes.Deleted;
            }

            IFileSourceEvent ae = new FileSourceChangedEvent(assetSource, changeType);
            observer.OnNext(ae);
        }

        /// <summary>
        /// Dispose observer
        /// </summary>
        public void Dispose()
        {
            var _watcher = watcher;
            var _observer = observer;

            StructList4<Exception> errors = new StructList4<Exception>();
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
