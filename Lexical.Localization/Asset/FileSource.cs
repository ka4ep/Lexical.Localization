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
}
