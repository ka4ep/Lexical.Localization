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
        public bool Exists(string path)
            => FileSystem.Exists(path);

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