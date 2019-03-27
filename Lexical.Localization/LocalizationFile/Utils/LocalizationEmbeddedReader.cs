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

namespace Lexical.Localization.Utils
{
    public abstract class LocalizationEmbeddedReader : LocalizationReader
    {
        public string ResourceName { get; protected set; }
        public Assembly Asm { get; protected set; }
        public bool ThrowIfNotFound { get; protected set; }

        public LocalizationEmbeddedReader(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, namePolicy)
        {
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        public override string ToString()
            => ResourceName;
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderStringLines : LocalizationEmbeddedReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationEmbeddedReaderStringLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, asm, resourceName, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<string, string>> empty = new KeyValuePair<string, string>[0];

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            try
            {
                using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
                }
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderKeyLines : LocalizationEmbeddedReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationEmbeddedReaderKeyLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, asm, resourceName, namePolicy, throwIfNotFound) { }

        static IEnumerable<KeyValuePair<IAssetKey, string>> empty = new KeyValuePair<IAssetKey, string>[0];

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            try
            {
                using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
                }
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<KeyValuePair<IAssetKey, string>>)this).GetEnumerator();
    }

    /// <summary>
    /// Reader that opens an embedded resource and reads as <see cref="IEnumerable{IKeyTree}"/>.
    /// </summary>
    public class LocalizationEmbeddedReaderKeyTree : LocalizationEmbeddedReader, IEnumerable<IKeyTree>
    {
        public LocalizationEmbeddedReaderKeyTree(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, asm, resourceName, namePolicy, throwIfNotFound) { }

        static IEnumerable<IKeyTree> empty = new IKeyTree[0];

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            try
            {
                using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                {
                    if (s == null) return !ThrowIfNotFound ? empty.GetEnumerator() : throw new FileNotFoundException(ResourceName);
                    IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                    if (tree == null) return empty.GetEnumerator();
                    return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
                }
            }
            catch (FileNotFoundException) when (!ThrowIfNotFound)
            {
                return empty.GetEnumerator();
            }
        }

        public override IEnumerator GetEnumerator()
            => ((IEnumerable<IKeyTree>)this).GetEnumerator();
    }

}
