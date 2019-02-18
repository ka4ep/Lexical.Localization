//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.LocalizationFile;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    public static class AssetFileConstructors
    {
        private static readonly AssetFileConstructor ini = (s, p)=> new IniReadable(new StreamReader(s)).DecorateSections(p).ToAssetAndClose();
        private static readonly AssetFileConstructor json = (s, p)=> new JsonReader(new StreamReader(s)).DecorateSections(p).ToAssetAndClose();
        private static readonly AssetFileConstructor xml = (s, p)=> new XmlReadable(new StreamReader(s)).DecorateSections(p).ToAssetAndClose();
        private static readonly AssetFileConstructor resx = (s, p)=> new ResXReadable(new StreamReader(s)).DecorateSections(p).ToAssetAndClose();
        public static AssetFileConstructor Ini => ini;
        public static AssetFileConstructor Json => json;
        public static AssetFileConstructor Xml => xml;
        public static AssetFileConstructor ResX => resx;

        /// <summary>
        /// Extact file extension from <paramref name="filename"/>.
        /// Then search matching <see cref="AssetFileConstructor"/> from <see cref="LocalizationTextReaderBuilder"/>'s singleton instance.
        /// </summary>
        /// <param name="filename">filename whose fileformat extension is extracted.</param>
        /// <param name="policy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If file format is not supported</exception>
        public static AssetFileConstructor FileFormat(string filename, IAssetKeyNamePolicy policy = default)
        {
            string ext = LocalizationFileFormatMap.GetExtension(filename);
            ILocalizationFileFormat fileFormat;
            if (!LocalizationFileFormatMap.Singleton.TryGetValue(ext, out fileFormat)) throw new ArgumentException($"File format not supported \"{ext}\".");

            if (ext == "resx" || ext == "resources")
            {
                if (fileFormat is ILocalizationFileStreamReader _reader) return (stream, initialSections) => _reader.OpenStream(stream, policy, initialSections).ToAsset();
                if (fileFormat is ILocalizationFileTextReader __reader) return (stream, initialSections) => __reader.OpenText(new StreamReader(stream), policy, initialSections).ToAssetAndClose();
            } else
            {
                /* Fix this later */
                if (fileFormat is ILocalizationFileStreamReader _reader) return (stream, initialSections) => _reader.OpenStream(stream, policy, null).ToAsset();
                if (fileFormat is ILocalizationFileTextReader __reader) return (stream, initialSections) => __reader.OpenText(new StreamReader(stream), policy, null).ToAssetAndClose();
            }
            throw new ArgumentException($"File format provider {fileFormat.GetType().FullName} doesn't support reading localization files.");
        }

        public static ILocalizationFileReadable IniReader(string text) => new IniReadable(new StringReader(text));
        public static ILocalizationFileReadable IniReader(string text, IAssetKeyNamePolicy namePolicy) => new IniReadable(new StringReader(text), namePolicy);
        public static IAsset IniAsset(Stream stream, IReadOnlyDictionary<string, string> initialSections = null) => new IniReadable(new StreamReader(stream), null).DecorateSections(initialSections).ToAssetAndClose();
        public static IAsset IniAsset(Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections = null) => new IniReadable(new StreamReader(stream), namePolicy).DecorateSections(initialSections).ToAssetAndClose();

        public static ILocalizationFileReadable JsonReader(string text) => new JsonReader(new StringReader(text));
        public static ILocalizationFileReadable JsonReader(string text, IAssetKeyNamePolicy namePolicy) => new JsonReader(new StringReader(text), namePolicy);
        public static IAsset JsonAsset(Stream stream, IReadOnlyDictionary<string, string> initialSections = null) => new JsonReader(new StreamReader(stream)).DecorateSections(initialSections).ToAssetAndClose();
        public static IAsset JsonAsset(Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections = null) => new JsonReader(new StreamReader(stream), namePolicy).DecorateSections(initialSections).ToAssetAndClose();

        public static ILocalizationFileReadable XmlReader(string text) => new XmlReadable(new StringReader(text));
        public static ILocalizationFileReadable XmlReader(string text, IAssetKeyNamePolicy namePolicy) => new XmlReadable(new StringReader(text), namePolicy);
        public static IAsset XmlAsset(Stream stream, IReadOnlyDictionary<string, string> initialSections = null) => new XmlReadable(new StreamReader(stream)).DecorateSections(initialSections).ToAssetAndClose();
        public static IAsset XmlAsset(Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections = null) => new XmlReadable(new StreamReader(stream), namePolicy).DecorateSections(initialSections).ToAssetAndClose();

        public static IAsset ResXAsset(Stream stream, IReadOnlyDictionary<string, string> initialSections = null) => new ResXReadable(new StreamReader(stream), null).DecorateSections(initialSections).ToAssetAndClose();
        public static IAsset ResXAsset(Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections = null) => new ResXReadable(new StreamReader(stream), namePolicy).DecorateSections(initialSections).ToAssetAndClose();

        public static IAsset ResourcesAsset(Stream stream, IReadOnlyDictionary<string, string> initialSections = null) => new ResourcesReadable(stream, null).DecorateSections(initialSections).ToAssetAndClose();
        public static IAsset ResourcesAsset(Stream stream, IAssetKeyNamePolicy namePolicy, IReadOnlyDictionary<string, string> initialSections = null) => new ResourcesReadable(stream, namePolicy).DecorateSections(initialSections).ToAssetAndClose();
    }
}
