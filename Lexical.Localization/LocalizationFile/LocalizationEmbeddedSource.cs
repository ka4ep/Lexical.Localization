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
        public IAssetKeyNamePolicy KeyPolicy { get; protected set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Create (abstract) embedded reader.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="keyPolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationEmbeddedSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy keyPolicy, bool throwIfNotFound) : base(assembly, resourceName, throwIfNotFound)
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
        public LocalizationEmbeddedStringLinesSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

        /// <summary>
        /// No lines
        /// </summary>
        static IEnumerable<KeyValuePair<string, string>> empty = new KeyValuePair<string, string>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
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
            => ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to list.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationStringAsset(KeyPolicy).AddSource(this).Load());

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
        public LocalizationEmbeddedKeyLinesSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<IAssetKey, string>> empty = new KeyValuePair<IAssetKey, string>[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
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
            => ((IEnumerable<KeyValuePair<IAssetKey, string>>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().AddSource(this).Load());

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
    public class LocalizationEmbeddedKeyTreeSource : LocalizationEmbeddedSource, ILocalizationKeyTreeSource
    {
        /// <summary>
        /// Create embedded localization reader that reads as key tree.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <param name="throwIfNotFound"></param>
        public LocalizationEmbeddedKeyTreeSource(ILocalizationFileFormat fileFormat, Assembly assembly, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, assembly, resourceName, namePolicy, throwIfNotFound) { }

        static IEnumerable<IKeyTree> empty = new IKeyTree[0];

        /// <summary>
        /// Open file and get new reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">if ThrowIfNotFound and not found</exception>
        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            try
            {
                using (Stream s = Assembly.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    IKeyTree tree = FileFormat.ReadKeyTree(s, KeyPolicy);
                    if (tree == null) return empty.GetEnumerator();
                    return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
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
            => ((IEnumerable<IKeyTree>)this).GetEnumerator();

        /// <summary>
        /// Add reader to <paramref name="list"/>.
        /// </summary>
        /// <param name="list"></param>
        public override void Build(IList<IAsset> list)
            => list.Add(new LocalizationAsset().AddSource(this).Load());

        /// <summary>
        /// Post build action
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public override IAsset PostBuild(IAsset asset)
            => asset;
    }


}
