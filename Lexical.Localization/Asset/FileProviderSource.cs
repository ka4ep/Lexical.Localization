//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    using Microsoft.Extensions.FileProviders;

    /// <summary>
    /// Localization source and reader that reads lines from file provider.
    /// </summary>
    public abstract class FileProviderSource : IAssetSource
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
        public IAssetNamePattern FilePattern { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="fileProvider"></param>
        /// <param name="path"></param>
        /// <param name="filePattern"></param>
        public FileProviderPatternSource(IFileProvider fileProvider, string path, IAssetNamePattern filePattern)
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
}
