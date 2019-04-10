using Lexical.Localization;
using Lexical.Localization.Utils;
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace docs
{
    public class LocalizationReader_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                ILocalizationFileFormat format = LocalizationReaderMap.Instance["ini"];
                #endregion Snippet_0a
            }
            {
                #region Snippet_0b
                ILocalizationFileFormat format = LocalizationIniReader.Instance;
                #endregion Snippet_0b
                ILocalizationFileFormat format2a = LocalizationReaderMap.Instance["json"];
                ILocalizationFileFormat format2b = LocalizationJsonReader.Instance;
                ILocalizationFileFormat format3a = LocalizationReaderMap.Instance["xml"];
                ILocalizationFileFormat format3b = LocalizationXmlReader.Instance;
                ILocalizationFileFormat format4a = LocalizationReaderMap.Instance["resx"];
                ILocalizationFileFormat format4b = LocalizationResxReader.Instance;
                ILocalizationFileFormat format5a = LocalizationReaderMap.Instance["resources"];
                ILocalizationFileFormat format5b = LocalizationResourcesReader.Instance;
            }

            {
                #region Snippet_1a
                IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines = LocalizationReaderMap.Instance.ReadKeyLines(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LocalizationReaderMap.Instance.ReadStringLines(
                    filename: "localization.ini", 
                    namePolicy: ParameterNamePolicy.Instance,
                    throwIfNotFound: true);
                #endregion Snippet_1b
            }
            {
                #region Snippet_1c
                IKeyTree tree = LocalizationReaderMap.Instance.ReadKeyTree(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1c
            }

            {
                #region Snippet_2a
                IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines_reader = 
                    LocalizationReaderMap.Instance.FileReaderAsKeyLines(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2a
            }
            {
                #region Snippet_2b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines_reader = 
                    LocalizationReaderMap.Instance.FileReaderAsStringLines(
                        filename: "localization.ini",
                        namePolicy: ParameterNamePolicy.Instance,
                        throwIfNotFound: true);
                #endregion Snippet_2b
                var lines = string_lines_reader.ToArray();
            }
            {
                #region Snippet_2c
                IEnumerable<IKeyTree> tree_reader = 
                    LocalizationReaderMap.Instance.FileReaderAsKeyTree(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_3a
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines_reader = 
                    LocalizationReaderMap.Instance.EmbeddedReaderAsKeyLines(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_3a
                var lines = key_lines_reader.ToArray();
            }
            {
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                #region Snippet_3b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines_reader = 
                    LocalizationReaderMap.Instance.EmbeddedReaderAsStringLines(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        namePolicy: ParameterNamePolicy.Instance,
                        throwIfNotFound: true);
                #endregion Snippet_3b
                var lines = string_lines_reader.ToArray();
            }
            {
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                #region Snippet_3c
                IEnumerable<IKeyTree> tree_reader = 
                    LocalizationReaderMap.Instance.EmbeddedReaderAsKeyTree(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_3c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_4a
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines_reader = 
                    LocalizationReaderMap.Instance.FileProviderReaderAsKeyLines(
                        fileProvider: fileProvider, 
                        filepath: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_4a
                var lines = key_lines_reader.ToArray();
            }
            {
                #region Snippet_4b
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines_reader = 
                    LocalizationReaderMap.Instance.FileProviderReaderAsStringLines(
                        fileProvider: fileProvider, 
                        filepath: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_4b
                var lines = string_lines_reader.ToArray();
            }
            {
                #region Snippet_4c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<IKeyTree> tree_reader = 
                    LocalizationReaderMap.Instance.FileProviderReaderAsKeyTree(
                        fileProvider: fileProvider, 
                        filepath: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_4c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_5a
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines = LocalizationIniReader.Instance.ReadKeyLines(s);
                }
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LocalizationIniReader.Instance.ReadStringLines(
                        stream: s,
                        namePolicy: ParameterNamePolicy.Instance);
                }
                #endregion Snippet_5b
            }
            {
                #region Snippet_5c
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    IKeyTree tree = LocalizationIniReader.Instance.ReadKeyTree(s);
                }
                #endregion Snippet_5c
            }


            {
                #region Snippet_6a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines = LocalizationIniReader.Instance.ReadKeyLines(tr);
                }
                #endregion Snippet_6a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6b
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LocalizationIniReader.Instance.ReadStringLines(
                        srcText: tr,
                        namePolicy: ParameterNamePolicy.Instance);
                }
                #endregion Snippet_6b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6c
                using (TextReader tr = new StringReader(text))
                {
                    IKeyTree tree = LocalizationIniReader.Instance.ReadKeyTree(tr);
                }
                #endregion Snippet_6c
            }

            {
                #region Snippet_7a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> key_lines = 
                    LocalizationIniReader.Instance.ReadStringAsKeyLines(
                        srcText: text);
                #endregion Snippet_7a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = 
                    LocalizationIniReader.Instance.ReadStringAsStringLines(
                        srcText: text,
                        namePolicy: ParameterNamePolicy.Instance);
                #endregion Snippet_7b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7c
                IKeyTree tree = 
                    LocalizationIniReader.Instance.ReadStringAsKeyTree(
                        srcText: text);
                #endregion Snippet_7c
            }

            {
                #region Snippet_10a
                IAsset asset = LocalizationIniReader.Instance.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10a
            }
            {
                #region Snippet_10b
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IAsset asset = LocalizationIniReader.Instance.EmbeddedAsset(
                    assembly: asm,
                    resourceName: "docs.localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10b
            }
            {
                #region Snippet_10c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAsset asset = LocalizationIniReader.Instance.FileProviderAsset(
                    fileProvider: fileProvider,
                    filepath: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10c
            }
            {
                #region Snippet_10d
                IAsset asset = LocalizationReaderMap.Instance.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10d
            }

            {
                #region Snippet_11a
                IAssetSource assetSource = 
                    LocalizationIniReader.Instance.FileAssetSource(
                        filename: "localization.ini",
                        throwIfNotFound: true);
                IAssetBuilder assetBuilder = new AssetBuilder().AddSource(assetSource);
                IAsset asset = assetBuilder.Build();
                #endregion Snippet_11a
            }
            {
                #region Snippet_11b
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IAssetSource assetSource = 
                    LocalizationIniReader.Instance.EmbeddedAssetSource(
                        assembly: asm,
                        resourceName: "docs.localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11b
            }
            {
                #region Snippet_11c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAssetSource assetSource = 
                    LocalizationIniReader.Instance.FileProviderAssetSource(
                        fileProvider: fileProvider,
                        filepath: "localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11c
            }
            {
                #region Snippet_11d
                IAssetSource assetSource = LocalizationReaderMap.Instance.FileAssetSource(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_11d
            }

            {
                #region Snippet_7
                #endregion Snippet_7
            }
            {
                #region Snippet_8
                #endregion Snippet_8
            }
            {
                #region Snippet_9
                #endregion Snippet_9
            }
            {
                #region Snippet_12
                #endregion Snippet_12
            }
            {
                #region Snippet_13
                #endregion Snippet_13
            }
            {
                #region Snippet_14
                #endregion Snippet_14
            }
            {
                #region Snippet_15
                #endregion Snippet_15
            }
            {
                #region Snippet_16
                #endregion Snippet_16
            }
            {
                #region Snippet_17
                #endregion Snippet_17
            }
            {
                #region Snippet_18
                #endregion Snippet_18
            }
            {
                #region Snippet_30a
                // Create writer
                ILocalizationReader format = new ExtFileFormatReader();

                // Clone formats
                LocalizationFileFormatMap formats = LocalizationReaderMap.Instance.Clone();
                // Add to clone
                formats.Add(format);

                // Or if in deploying application project, format can be added to the global singleton
                (LocalizationReaderMap.Instance as IDictionary<string, ILocalizationFileFormat>).Add(format);
                #endregion Snippet_30a
            }

        }
    }

    #region Snippet_30
    class ExtFileFormatReader : ILocalizationKeyLinesTextReader
    {
        public string Extension => "ext";

        public IEnumerable<KeyValuePair<IAssetKey, IFormulationString>> ReadKeyLines(
            TextReader text, 
            IAssetKeyNamePolicy namePolicy = null)
        {
            IAssetKey key = Key.Create("Section", "MyClass").Append("Key", "HelloWorld").Append("Culture", "en");
            yield return new KeyValuePair<IAssetKey, IFormulationString>(key, LexicalStringFormat.Instance.Parse("Hello World!"));
        }
    }
    #endregion Snippet_30

}
