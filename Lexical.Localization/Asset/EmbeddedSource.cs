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
    public abstract class EmbeddedSource : IAssetSource, IAssetSourceFileSystem, IBuildableAssetSource, IFileAssetSource
    {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; protected set; }

        /// <summary>
        /// Resource name. For example "AssemblyName.Resources.localization.ini"
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// Throw <see cref="FileNotFoundException"/> if file is not found.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// associated file system.
        /// </summary>
        protected IFileSystem fileSystem;

        /// <summary>
        /// Associated file system.
        /// </summary>
        public IFileSystem FileSystem
        {
            get
            {
                lock (this) return fileSystem ?? (fileSystem = new EmbeddedFileSystem(Assembly));
            }
        }

        /// <summary>
        /// Create source to embedded resource.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="throwIfNotFound"></param>
        public EmbeddedSource(Assembly assembly, string resourceName, bool throwIfNotFound)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.FilePath = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void Build(IList<IAsset> list);

        /// <summary>
        /// Asset source of embedded resource that contains strings.
        /// </summary>
        public class String : EmbeddedSource, IStringAssetSource
        {
            /// <summary>
            /// String asset file format.
            /// </summary>
            public ILineFileFormat FileFormat { get; protected set; }

            /// <summary>
            /// (Optional) Line format for file types that have unformed keys, such as .resx.
            /// </summary>
            public ILineFormat LineFormat { get; protected set; }

            /// <summary>
            /// Create source to embedded resource that loads string assets.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="resourceName"></param>
            /// <param name="throwIfNotFound"></param>
            /// <param name="fileFormat"></param>
            /// <param name="lineFormat">(optional) line format for file types that have unformed keys, such as .resx</param>
            public String(Assembly assembly, string resourceName, bool throwIfNotFound, ILineFileFormat fileFormat, ILineFormat lineFormat) : base(assembly, resourceName, throwIfNotFound)
            {
                this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(FileFormat));
                this.LineFormat = lineFormat;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="list"></param>
            public override void Build(IList<IAsset> list)
            {
                IEnumerable reader = FileFormat.EmbeddedReader(Assembly, FilePath, LineFormat, ThrowIfNotFound);
                IAsset asset = new StringAsset().Add(reader).Load();
                list.Add(asset);
            }
        }

        /// <summary>
        /// Asset source of embedded resource that contains binary resource.
        /// </summary>
        public class Resource : EmbeddedSource, IResourceAssetSource
        {
            /// <summary>
            /// Key for this particular file.
            /// </summary>
            public ILine Key { get; set; }

            /// <summary>
            /// Create source to embedded resource that loads string assets.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="resourceName"></param>
            /// <param name="throwIfNotFound"></param>
            public Resource(Assembly assembly, string resourceName, bool throwIfNotFound, ILine key) : base(assembly, resourceName, throwIfNotFound)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="list"></param>
            public override void Build(IList<IAsset> list)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Assembly.FullName + "." + FilePath;
    }

    /// <summary>
    /// A pattern of files, which are resolved to physical file when parameters in requesting key is matched to pattern.
    /// 
    /// For example if pattern is "{Assembly}.{Culture.}Localization.resx" then file name is resolved based on "Culture" and "Assembly" parametrs.
    /// </summary>
    public abstract class EmbeddedPatternSource : IAssetSource, IAssetSourceFileSystem, IBuildableAssetSource, IFilePatternAssetSource
    {
        /// <summary>
        /// Assembly
        /// </summary>
        public Assembly Assembly { get; protected set; }

        /// <summary>
        /// File pattern.
        /// </summary>
        public ILinePattern FilePattern { get; protected set; }

        /// <summary>
        /// If true, throws <see cref="FileNotFoundException"/> if file is not found.
        /// If false, returns empty enumerable.
        /// </summary>
        public bool ThrowIfNotFound { get; protected set; }

        /// <summary>
        /// associated file system.
        /// </summary>
        protected IFileSystem fileSystem;

        /// <summary>
        /// Associated file system.
        /// </summary>
        public IFileSystem FileSystem
        {
            get
            {
                lock (this) return fileSystem ?? (fileSystem = new EmbeddedFileSystem(Assembly));
            }
        }

        /// <summary>
        /// Create abstract file source.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourcePattern">Resource pattern, for example "assemblyname.{Culture.}{Type.}[Key]"</param>
        public EmbeddedPatternSource(Assembly assembly, ILinePattern resourcePattern)
        {
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.FilePattern = resourcePattern ?? throw new ArgumentNullException(nameof(resourcePattern));
        }

        /// <summary>
        /// (IAssetSource) Build asset into list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void Build(IList<IAsset> list);

        /// <summary>
        /// Asset source of embedded string resources.
        /// </summary>
        public class String : EmbeddedSource, IStringAssetSource
        {
            /// <summary>
            /// String asset file format.
            /// </summary>
            public ILineFileFormat FileFormat { get; protected set; }

            /// <summary>
            /// (Optional) Line format for file types that have unformed keys, such as .resx.
            /// </summary>
            public ILineFormat LineFormat { get; protected set; }

            /// <summary>
            /// Create source to embedded resource that loads string assets.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="resourceName"></param>
            /// <param name="throwIfNotFound"></param>
            /// <param name="fileFormat"></param>
            /// <param name="lineFormat">(optional) line format for file types that have unformed keys, such as .resx</param>
            public String(Assembly assembly, string resourceName, bool throwIfNotFound, ILineFileFormat fileFormat, ILineFormat lineFormat) : base(assembly, resourceName, throwIfNotFound)
            {
                this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(FileFormat));
                this.LineFormat = lineFormat;
            }

            /// <summary>
            /// Build into <see cref="IAsset"/>.
            /// </summary>
            /// <param name="list"></param>
            public override void Build(IList<IAsset> list)
            {
                //IAsset asset = new BinaryAsset().Add(this).Load();
                //list.Add(asset);
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Asset source of embedded binary resources.
        /// </summary>
        public class Resource : EmbeddedSource, IResourceAssetSource
        {
            /// <summary>
            /// Key for this particular file.
            /// </summary>
            public ILine Key { get; set; }

            /// <summary>
            /// Create source to embedded resource that loads string assets.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="resourceName"></param>
            /// <param name="throwIfNotFound"></param>
            public Resource(Assembly assembly, string resourceName, bool throwIfNotFound, ILine key) : base(assembly, resourceName, throwIfNotFound)
            {
            }

            /// <summary>
            /// Build into <see cref="IAsset"/>.
            /// </summary>
            /// <param name="list"></param>
            public override void Build(IList<IAsset> list)
            {
                //IAsset asset = new BinaryAsset().Add(this).Load();
                //list.Add(asset);
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Assembly.FullName + "." + FilePattern.Pattern;
    }

}
