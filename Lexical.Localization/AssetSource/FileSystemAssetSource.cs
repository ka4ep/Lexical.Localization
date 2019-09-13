// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           6.9.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization.Asset
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FileAssetSource : IAssetSource, IAssetSourceFile, IAssetSourceFileSystem, IAssetSourceObservable, IAssetSourceStrings, IAssetSourceBinary, IAssetSourceLines, IAssetSourceUnformedLines, IAssetSourceLineTree
    {
        /// <summary>
        /// Reference to an asset file. Used within <see cref="IFileSystem"/>. Directory separator is '/'. Root doesn't use separator.
        /// 
        /// Example: "resources/localization.xml".
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// Specific <see cref="IFileSystem"/> to load the asset source from.
        /// 
        /// If null, then file-system is unspecified, and the caller have a reference to known file-system.
        /// 
        /// The reference is immutable and must not be modified after construction.
        /// </summary>
        public IFileSystem FileSystem { get; protected set; }

        /// <summary>
        /// File format
        /// </summary>
        public readonly ILineFileFormat FileFormat;

        /// <summary>
        /// (Optional) Line format, if file format needs one. 
        /// </summary>
        public readonly ILineFormat LineFormat;

        /// <summary>
        /// Tests whether asset source can be observed for changes.
        /// </summary>
        public bool IsObservable => (FileSystem.Capabilities & FileSystemCapabilities.Observe) == FileSystemCapabilities.Observe;

        public ILineFileFormat FileFormat => throw new NotImplementedException();

        public bool ContainsBinaryLines => throw new NotImplementedException();

        public bool EnumeratesLines => throw new NotImplementedException();

        public bool EnumeratesUnformedLines => throw new NotImplementedException();

        public bool EnumeratesLineTree => throw new NotImplementedException();

        /// <summary>
        /// Read source into lines
        /// </summary>
        /// <returns>lines</returns>
        public IEnumerator<ILine> GetEnumerator()
        {
            // Open file
            using (Stream s = FileSystem.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read lines
                IEnumerable<ILine> lines = FileFormat.ReadLines(s, LineFormat);
                // Return
                return lines.GetEnumerator();
            }
        }

        /// <summary>
        /// Read source into lines
        /// </summary>
        /// <returns>lines</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            /*
            // Open file
            using (Stream s = FileSystem.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read lines
                IEnumerable<ILine> lines = FileFormat.ReadLines(s, LineFormat);
                // Return
                return lines.GetEnumerator();
            }
            */
            // TODO use the most innate enumeration format

            return null;
        }

        /// <summary>
        /// Create asset source that refers to a single file in a file system.
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="filePath"></param>
        protected FileAssetSource(IFileSystem fileSystem, string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            FileSystem = fileSystem;
        }

        /// <summary>
        /// Subscribe to monitor changes of asset source.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<IAssetSourceEvent> observer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compare references.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is FileAssetSource otherSource ? FilePath == otherSource.FilePath && FileSystemComparerComposition.Instance.Equals(FileSystem, otherSource.FileSystem) : false;

        /// <summary>
        /// Calculate hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => FilePath.GetHashCode() ^ FileSystemComparerComposition.Instance.GetHashCode(FileSystem);

        /// <summary>
        /// Print info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => GetType().Name + "(" + FileSystem + ", " + FilePath + ")";

        public IEnumerator<ILine> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, IStringAsset>> IEnumerable<KeyValuePair<string, IStringAsset>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asset source that reads formed <see cref="ILine"/>s.
        /// </summary>
        public class Line : FileAssetSource, IAssetSourceLines
        {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="fileSystem"></param>
            /// <param name="filePath"></param>
            /// <param name="fileFormat"></param>
            /// <param name="lineFormat">(optional) line format. For example, to read .resx file into formed lines, <see cref="ILineFormat"/> is needed to parse <see cref="ILine"/> parts.</param>
            public Line(IFileSystem fileSystem, string filePath, ILineFileFormat fileFormat, ILineFormat lineFormat) : base(fileSystem, filePath)
            {
                this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
                this.LineFormat = lineFormat;
            }

        }
    }

}
