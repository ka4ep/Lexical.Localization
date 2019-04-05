//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexical.Localization
{
    /// <summary>
    /// Asset file source.
    /// </summary>
    public abstract class FileSource : IAssetSource
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
        /// File pattern.
        /// </summary>
        public IAssetNamePattern FilePattern { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filePattern"></param>
        public FilePatternSource(string path, IAssetNamePattern filePattern)
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
}
