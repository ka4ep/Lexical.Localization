//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           7.3.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;

namespace Lexical.Localization
{
    /// <summary>
    /// Abstract localization reader.
    /// </summary>
    public abstract class LocalizationReader : IEnumerable
    {
        /// <summary>
        /// Name policy to apply to file, if applicable. Depends on file format.
        /// </summary>
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }

        /// <summary>
        /// File format 
        /// </summary>
        public ILocalizationFileFormat FileFormat { get; protected set; }

        /// <summary>
        /// Create abstract localization reader.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="namePolicy"></param>
        public LocalizationReader(ILocalizationFileFormat fileFormat, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.NamePolicy = namePolicy;
        }

        /// <summary>
        /// Create reader.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();
    }
}
