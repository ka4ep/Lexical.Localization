//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// File source to a embedded resource.
    /// </summary>
    public abstract class EmbeddedSource : IAssetSource
    {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; protected set; }

        /// <summary>
        /// Embedded resource name.
        /// </summary>
        public string ResourceName { get; protected set; }

        /// <summary>
        /// Throw <see cref="FileNotFoundException"/> if file is not found.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// Create source to embedded resource.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="throwIfNotFound"></param>
        public EmbeddedSource(Assembly assembly, string resourceName, bool throwIfNotFound)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
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
            => ResourceName;
    }

    /// <summary>
    /// A pattern of files, which are resolved to physical file when parameters in requesting key is matched to pattern.
    /// 
    /// For example if pattern is "{Assembly}.{Culture.}Localization.resx" then file name is resolved based on "Culture" and "Assembly" parametrs.
    /// </summary>
    public abstract class EmbeddedPatternSource : IAssetSource
    {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; protected set; }

        /// <summary>
        /// File pattern.
        /// </summary>
        public ILinePattern ResourcePattern { get; protected set; }

        /// <summary>
        /// If true, throws <see cref="FileNotFoundException"/> if file is not found.
        /// If false, returns empty enumerable.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourcePattern"></param>
        public EmbeddedPatternSource(Assembly assembly, ILinePattern resourcePattern)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.ResourcePattern = resourcePattern ?? throw new ArgumentNullException(nameof(resourcePattern));
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
            => Assembly.FullName + "." + ResourcePattern.Pattern;
    }

}
