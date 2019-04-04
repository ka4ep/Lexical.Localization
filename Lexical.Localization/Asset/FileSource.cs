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
        /// File path.
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// If true, throws <see cref="FileNotFoundException"/> if file is not found.
        /// If false, returns empty enumerable.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="throwIfNotFound"></param>
        public FileSource(string filename, bool throwIfNotFound)
        {
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
