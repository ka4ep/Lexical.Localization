//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace Lexical.Localization.Utils
{
    public abstract class LocalizationReader : IEnumerable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        public ILocalizationFileFormat FileFormat { get; protected set; }

        public LocalizationReader(ILocalizationFileFormat fileFormat, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.NamePolicy = namePolicy;
        }

        public abstract IEnumerator GetEnumerator();
    }

    public abstract class LocalizationFileReader : LocalizationReader
    {
        public string FileName { get; protected set; }
        public bool ThrowIfNotFound { get; protected set; }

        public LocalizationFileReader(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, namePolicy)
        {
            this.FileName = filename ?? throw new ArgumentNullException(nameof(FileName));
            this.ThrowIfNotFound = throwIfNotFound;
        }

        public override string ToString()
            => FileName;
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{string, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderStringLines : LocalizationFileReader, IEnumerable<KeyValuePair<string, string>>
    {
        public LocalizationFileReaderStringLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationReaderExtensions_.ReadFileAsStringLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            return lines.GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationReaderExtensions_.ReadFileAsStringLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            return lines.GetEnumerator();
        }
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IAssetKey, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderKeyLines : LocalizationFileReader, IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public LocalizationFileReaderKeyLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }

        IEnumerator<KeyValuePair<IAssetKey, string>> IEnumerable<KeyValuePair<IAssetKey, string>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationReaderExtensions_.ReadFileAsKeyLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            return lines.GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IEnumerable lines = LocalizationReaderExtensions_.ReadFileAsKeyLines(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            return lines.GetEnumerator();
        }

        public override string ToString()
            => FileName;
    }

    /// <summary>
    /// Reader that opens a file and reads as <see cref="IEnumerable{KeyValuePair{IKeyTree, string}}"/>.
    /// </summary>
    public class LocalizationFileReaderKeyTree : LocalizationFileReader, IEnumerable<IKeyTree>
    {
        public LocalizationFileReaderKeyTree(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, bool throwIfNotFound) : base(fileFormat, filename, namePolicy, throwIfNotFound) { }
        static IKeyTree[] no_trees = new IKeyTree[0];

        IEnumerator<IKeyTree> IEnumerable<IKeyTree>.GetEnumerator()
        {
            IKeyTree tree = LocalizationReaderExtensions_.ReadFileAsKeyTree(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            IKeyTree[] trees = tree == null ? no_trees : new IKeyTree[] { tree };
            return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            IKeyTree tree = LocalizationReaderExtensions_.ReadFileAsKeyTree(FileFormat, FileName, NamePolicy, ThrowIfNotFound);
            IKeyTree[] trees = tree == null ? no_trees : new IKeyTree[] { tree };
            return ((IEnumerable<IKeyTree>)trees).GetEnumerator();
        }

        public override string ToString()
            => FileName;
    }



}
