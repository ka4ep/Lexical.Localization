//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           24.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lexical.Localization.Internal;

namespace Lexical.Localization
{
    /// <summary>
    /// Contains extensions that help instantiating <see cref="IAsset"/> from intermediate key-value formats, and <see cref="ILocalizationFileFormat"/>.
    /// </summary>
    public static class LocalizationFileExtensions
    {
        /// <summary>
        /// Read file into assetkey lines.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> ReadFileAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadKeyLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Read file into a tree format.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="srcText"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IKeyTree ReadFileAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadKeyTree(fileFormat, fs, namePolicy);
        }

        /// <summary>
        /// Read file into strings file.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> ReadFileAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LocalizationFileFormatReaderExtensions_.ReadStringLines(fileFormat, fs, namePolicy).ToArray();
        }

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateFileReaderAsKeyLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateFileReaderAsKeyTree(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderKeyTree(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens <paramref name="filename"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateFileReaderAsStringLines(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default)
            => new FileReaderStringLines(fileFormat, filename, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<IAssetKey, string>> CreateEmbeddedReaderAsKeyLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderKeyLines(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>tree</returns>
        public static IEnumerable<IKeyTree> CreateEmbeddedReaderAsKeyTree(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderKeyTree(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create reader that opens embedded <paramref name="resourceName"/> from <paramref name="asm"/> when IEnumerator is requested.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy"></param>
        /// <returns>lines</returns>
        public static IEnumerable<KeyValuePair<string, string>> CreateEmbeddedReaderAsStringLines(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default)
            => new EmbeddedReaderStringLines(fileFormat, asm, resourceName, namePolicy);

        /// <summary>
        /// Create localization asset that reads file <paramref name="filename"/>.
        /// 
        /// File is reloaded if <see cref="AssetExtensions.Reload(IAsset)"/> is called.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>reloadable localization asset</returns>
        public static IAsset CreateFileAsset(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyTreeSource(fileFormat.CreateFileReaderAsKeyTree(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LoadableLocalizationAsset().AddKeyLinesSource(fileFormat.CreateFileReaderAsKeyLines(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix)).Load();
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LoadableLocalizationStringAsset(namePolicy).AddLineStringSource(fileFormat.CreateFileReaderAsStringLines(filename, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy)).Load();
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads file <paramref name="filename"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="filename"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource CreateFileAssetSource(this ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.CreateFileReaderAsKeyTree(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.CreateFileReaderAsKeyLines(filename, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(filename);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.CreateFileReaderAsStringLines(filename, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, filename);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Create localization asset source that reads embedded resource <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="asm"></param>
        /// <param name="resourceName"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>asset source</returns>
        public static IAssetSource CreateEmbeddedAssetSource(this ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsKeyLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsKeyLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix).AddKeySuffix(suffix).ToAssetSource(resourceName);
            }
            else if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return fileFormat.CreateEmbeddedReaderAsStringLines(asm, resourceName, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy).ToAssetSource(namePolicy, resourceName);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="stream"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAsset CreateAsset(this ILocalizationFileFormat fileFormat, Stream stream, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            if (fileFormat is ILocalizationKeyTreeTextReader || fileFormat is ILocalizationKeyTreeStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyTree(stream, namePolicy).ToKeyLines().AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationKeyLinesTextReader || fileFormat is ILocalizationKeyLinesStreamReader)
            {
                return new LocalizationAsset(fileFormat.ReadKeyLines(stream, namePolicy).AddKeyPrefix(prefix).AddKeyPrefix(suffix).ToDictionary());
            }
            else
            if (fileFormat is ILocalizationStringLinesTextReader || fileFormat is ILocalizationStringLinesStreamReader)
            {
                return new LocalizationStringAsset(fileFormat.ReadStringLines(stream, namePolicy).AddKeyPrefix(prefix, namePolicy).AddKeySuffix(suffix, namePolicy), namePolicy);
            }
            throw new ArgumentException($"Cannot create asset for {fileFormat}.");
        }

        /// <summary>
        /// Read localization strings from <see cref="Stream"/> into most suitable asset implementation.
        /// 
        /// File cannot be reloaded. 
        /// </summary>
        /// <param name="fileFormat"></param>
        /// <param name="streamSource"></param>
        /// <param name="namePolicy">(optional) </param>
        /// <param name="prefix">(optional) parameters to add in front of key of each line</param>
        /// <param name="suffix">(optional) parameters to add at the end of key of each line</param>
        /// <returns>localization asset</returns>
        public static IAssetSource CreateAssetSource(this ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IAssetKeyNamePolicy namePolicy = default, IAssetKey prefix = null, IAssetKey suffix = null)
            => new AssetDelegateSource(fileFormat, streamSource, namePolicy, prefix, suffix);

    }

    internal class FileReaderStringLines : IEnumerable<KeyValuePair<string, string>>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderStringLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, string>> lines = LocalizationFileExtensions.ReadFileAsStringLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    internal class FileReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderKeyLines(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<IAssetKey, string>> lines = LocalizationFileExtensions.ReadFileAsKeyLines(FileFormat, Filename, NamePolicy);
            return lines.GetEnumerator();
        }
    }

    internal class FileReaderKeyTree : IEnumerable<IKeyTree>
    {
        public readonly string Filename;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;

        public FileReaderKeyTree(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Filename = filename ?? throw new ArgumentNullException(nameof(Filename));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<IKeyTree> GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IKeyTree tree = LocalizationFileExtensions.ReadFileAsKeyTree(FileFormat, Filename, NamePolicy);
            return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
        }
    }

    internal class EmbeddedReaderStringLines : IEnumerable<KeyValuePair<string, string>>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderStringLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadStringLines(s, NamePolicy).GetEnumerator();
        }
    }

    internal class EmbeddedReaderKeyLines : IEnumerable<KeyValuePair<IAssetKey, string>>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderKeyLines(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<KeyValuePair<IAssetKey, string>> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
                return FileFormat.ReadKeyLines(s, NamePolicy).GetEnumerator();
        }
    }

    internal class EmbeddedReaderKeyTree : IEnumerable<IKeyTree>
    {
        public readonly string ResourceName;
        public readonly IAssetKeyNamePolicy NamePolicy;
        public readonly ILocalizationFileFormat FileFormat;
        public readonly Assembly Asm;

        public EmbeddedReaderKeyTree(ILocalizationFileFormat fileFormat, Assembly asm, string resourceName, IAssetKeyNamePolicy namePolicy)
        {
            this.FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.Asm = asm ?? throw new ArgumentNullException(nameof(asm));
            this.ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            this.NamePolicy = namePolicy;
        }

        public IEnumerator<IKeyTree> GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (Stream s = Asm.GetManifestResourceStream(ResourceName))
            {
                IKeyTree tree = FileFormat.ReadKeyTree(s, NamePolicy);
                return ((IEnumerable<IKeyTree>)new IKeyTree[] { tree }).GetEnumerator();
            }
        }
    }

    internal class AssetFileSource : IAssetSource
    {
        ILocalizationFileFormat fileFormat;
        string fileName;
        IAssetKeyNamePolicy namePolicy;
        IAssetKey prefix;
        IAssetKey suffix;

        public AssetFileSource(ILocalizationFileFormat fileFormat, string filename, IAssetKeyNamePolicy namePolicy, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            this.fileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.fileName = filename ?? throw new ArgumentNullException(nameof(filename));
            this.namePolicy = namePolicy;
            this.prefix = prefix;
            this.suffix = suffix;
        }
        public void Build(IList<IAsset> list) 
            => list.Add(fileFormat.CreateFileAsset(fileName, namePolicy, prefix, suffix));
        public IAsset PostBuild(IAsset asset) 
            => asset;
    }

    internal class AssetDelegateSource : IAssetSource
    {
        ILocalizationFileFormat fileFormat;
        Func<Stream> streamSource;
        IAssetKeyNamePolicy namePolicy;
        IAssetKey prefix;
        IAssetKey suffix;

        public AssetDelegateSource(ILocalizationFileFormat fileFormat, Func<Stream> streamSource, IAssetKeyNamePolicy namePolicy, IAssetKey prefix = null, IAssetKey suffix = null)
        {
            this.fileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
            this.streamSource = streamSource ?? throw new ArgumentNullException(nameof(streamSource));
            this.namePolicy = namePolicy;
            this.prefix = prefix;
            this.suffix = suffix;
        }
        public void Build(IList<IAsset> list) 
            => list.Add(fileFormat.CreateAsset(streamSource(), namePolicy, prefix, suffix));

        public IAsset PostBuild(IAsset asset) 
            => asset;
    }

}
