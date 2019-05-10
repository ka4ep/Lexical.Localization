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

namespace Lexical.Localization
{
    /// <summary>
    /// Localization source and reader that reads lines from embedded resource.
    /// </summary>
    public abstract class LocalizationEmbeddedSource : EmbeddedSource, IAssetSource, IEnumerable
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
        /// Create (abstract) embedded reader.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="keyPolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationEmbeddedSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IParameterPolicy keyPolicy, bool throwIfNotFound) : base(assembly, resourceName, throwIfNotFound)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.KeyPolicy = keyPolicy;
            this.Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        /// <summary>
        /// Create reader.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Print info of source
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => ResourceName;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;string, string&gt;gt;
    /// </summary>
    public class LocalizationEmbeddedStringLinesSource : LocalizationEmbeddedSource, ILocalizationStringLinesSource
    {
        /// <summary>
        /// Create embedded localization reader that reads as string lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound">if true, throws <see cref="FileNotFoundException"/></param>
        public LocalizationEmbeddedStringLinesSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// No lines
        /// </summary>
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
                using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    return FileFormat.ReadStringLines(s, KeyPolicy).GetEnumerator();
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
    /// Reader that opens an embedded resource and reads as IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;
    /// </summary>
    public class LocalizationEmbeddedKeyLinesSource : LocalizationEmbeddedSource, ILocalizationKeyLinesSource
    {
        /// <summary>
        /// Create embedded localization reader that reads as key lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationEmbeddedKeyLinesSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

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
                using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    return FileFormat.ReadKeyLines(s, KeyPolicy).GetEnumerator();
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
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{ILineTree}"/>.
    /// </summary>
    public class LocalizationEmbeddedLineTreeSource : LocalizationEmbeddedSource, ILocalizationLineTreeSource
    {
        /// <summary>
        /// Create embedded localization reader that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationEmbeddedLineTreeSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IParameterPolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

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
                using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    ILineTree tree = FileFormat.ReadLineTree(s, KeyPolicy);
                    if (tree == null) return empty.GetEnumerator();
                    return ((IEnumerable<ILineTree>)new ILineTree[] { tree }).GetEnumerator();
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
