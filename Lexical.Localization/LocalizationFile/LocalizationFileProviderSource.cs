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
    public abstract class LocalizationFileProviderSource : FileProviderSource, IAssetSource, IEnumerable
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IParameterPolicy KeyPolicy { get; internal set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; internal set; }

        /// <summary>
        /// Create (abstract) source to localization file in a file provider.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="keyPolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy keyPolicy, bool throwIfNotFound) : base(fileProvider, filepath, throwIfNotFound)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.KeyPolicy = keyPolicy;
        }

        /// <summary>
        /// Create reader.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;
    /// </summary>
    public class LocalizationFileProviderStringLinesSource : LocalizationFileProviderSource, ILocalizationStringLinesSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderStringLinesSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<string, IFormulationString>> empty = new KeyValuePair<string, IFormulationString>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, IFormulationString>> IEnumerable<KeyValuePair<string, IFormulationString>>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadStringLines(s, KeyPolicy).GetEnumerator();
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, IFormulationString>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset(this, KeyPolicy));

        /// <summary>
        /// Post build action.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;.
    /// </summary>
    public class LocalizationFileProviderKeyLinesSource : LocalizationFileProviderSource, ILocalizationKeyLinesSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderKeyLinesSource(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<ILine, IFormulationString>> empty = new KeyValuePair<ILine, IFormulationString>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<ILine, IFormulationString>> IEnumerable<KeyValuePair<ILine, IFormulationString>>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                    return FileFormat.ReadKeyLines(s, KeyPolicy).GetEnumerator();
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<ILine, IFormulationString>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().Add(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;

    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationFileProviderReaderKeyTree : LocalizationFileProviderSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Create source to localization file in a <paramref name="fileProvider"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="fileProvider"></param>
        /// <param name="filepath"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationFileProviderReaderKeyTree(ILocalizationFileFormat fileFormat, IFileProvider fileProvider, string filepath, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, fileProvider, filepath, namePolicy, throwIfNotFound) { }

        static IEnumerable<ILineTree> empty = new ILineTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<ILineTree> IEnumerable<ILineTree>.GetEnumerator()
        {
            try
            {
                IFileInfo fi = FileProvider.GetFileInfo(FilePath);
                if (!ThrowIfNotFound && !fi.Exists) return empty.GetEnumerator();
                using (Stream s = fi.CreateReadStream())
                {
                    ILineTree tree = FileFormat.ReadKeyTree(s, KeyPolicy);
                    if (tree == null) return empty.GetEnumerator();
                    ILineTree[] trees = new ILineTree[] { tree };
                    return ((IEnumerable<ILineTree>)trees).GetEnumerator();
                }
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }

        }

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        public override IEnumerator GetEnumerator()
            => ((IEnumerable<ILineTree>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().Add(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }

}
