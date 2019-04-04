//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
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
        /// File path in the <see cref="FileProvider"/>.
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// File provider
        /// </summary>
        public IFileProvider FileProvider { get; protected set; }

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
}
