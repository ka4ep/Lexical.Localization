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
                ILineFileFormat format = LineReaderMap.Instance["ini"];
                #endregion Snippet_0a
            }
            {
                #region Snippet_0b
                ILineFileFormat format = LineIniReader.Instance;
                #endregion Snippet_0b
                ILineFileFormat format2a = LineReaderMap.Instance["json"];
                ILineFileFormat format2b = LineJsonReader.Instance;
                ILineFileFormat format3a = LineReaderMap.Instance["xml"];
                ILineFileFormat format3b = LineXmlReader.Instance;
                ILineFileFormat format4a = LineReaderMap.Instance["resx"];
                ILineFileFormat format4b = LineResxReader.Instance;
                ILineFileFormat format5a = LineReaderMap.Instance["resources"];
                ILineFileFormat format5b = LineResourcesReader.Instance;
            }

            {
                #region Snippet_1a
                IEnumerable<ILine> key_lines = LineReaderMap.Instance.ReadKeyLines(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LineReaderMap.Instance.ReadStringLines(
                    filename: "localization.ini", 
                    namePolicy: LineFormat.Instance,
                    throwIfNotFound: true);
                #endregion Snippet_1b
            }
            {
                #region Snippet_1c
                ILineTree tree = LineReaderMap.Instance.ReadLineTree(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1c
            }

            {
                #region Snippet_2a
                IEnumerable<ILine> key_lines_reader = 
                    LineReaderMap.Instance.FileReaderAsKeyLines(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2a
            }
            {
                #region Snippet_2b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines_reader = 
                    LineReaderMap.Instance.FileReaderAsStringLines(
                        filename: "localization.ini",
                        namePolicy: LineFormat.Instance,
                        throwIfNotFound: true);
                #endregion Snippet_2b
                var lines = string_lines_reader.ToArray();
            }
            {
                #region Snippet_2c
                IEnumerable<ILineTree> tree_reader = 
                    LineReaderMap.Instance.FileReaderAsLineTree(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_3a
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IEnumerable<ILine> key_lines_reader = 
                    LineReaderMap.Instance.EmbeddedReaderAsKeyLines(
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
                    LineReaderMap.Instance.EmbeddedReaderAsStringLines(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        namePolicy: LineFormat.Instance,
                        throwIfNotFound: true);
                #endregion Snippet_3b
                var lines = string_lines_reader.ToArray();
            }
            {
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                #region Snippet_3c
                IEnumerable<ILineTree> tree_reader = 
                    LineReaderMap.Instance.EmbeddedReaderAsLineTree(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_3c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_4a
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<ILine> key_lines_reader = 
                    LineReaderMap.Instance.FileProviderReaderAsKeyLines(
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
                    LineReaderMap.Instance.FileProviderReaderAsStringLines(
                        fileProvider: fileProvider, 
                        filepath: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_4b
                var lines = string_lines_reader.ToArray();
            }
            {
                #region Snippet_4c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<ILineTree> tree_reader = 
                    LineReaderMap.Instance.FileProviderReaderAsLineTree(
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
                    IEnumerable<ILine> key_lines = LineIniReader.Instance.ReadKeyLines(s);
                }
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LineIniReader.Instance.ReadStringLines(
                        stream: s,
                        namePolicy: LineFormat.Instance);
                }
                #endregion Snippet_5b
            }
            {
                #region Snippet_5c
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    ILineTree tree = LineIniReader.Instance.ReadLineTree(s);
                }
                #endregion Snippet_5c
            }


            {
                #region Snippet_6a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<ILine> key_lines = LineIniReader.Instance.ReadKeyLines(tr);
                }
                #endregion Snippet_6a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6b
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = LineIniReader.Instance.ReadStringLines(
                        srcText: tr,
                        namePolicy: LineFormat.Instance);
                }
                #endregion Snippet_6b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6c
                using (TextReader tr = new StringReader(text))
                {
                    ILineTree tree = LineIniReader.Instance.ReadLineTree(tr);
                }
                #endregion Snippet_6c
            }

            {
                #region Snippet_7a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                IEnumerable<ILine> key_lines = 
                    LineIniReader.Instance.ReadStringAsKeyLines(
                        srcText: text);
                #endregion Snippet_7a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7b
                IEnumerable<KeyValuePair<string, IFormulationString>> string_lines = 
                    LineIniReader.Instance.ReadStringAsStringLines(
                        srcText: text,
                        namePolicy: LineFormat.Instance);
                #endregion Snippet_7b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7c
                ILineTree tree = 
                    LineIniReader.Instance.ReadStringAsLineTree(
                        srcText: text);
                #endregion Snippet_7c
            }

            {
                #region Snippet_10a
                IAsset asset = LineIniReader.Instance.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10a
            }
            {
                #region Snippet_10b
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IAsset asset = LineIniReader.Instance.EmbeddedAsset(
                    assembly: asm,
                    resourceName: "docs.localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10b
            }
            {
                #region Snippet_10c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAsset asset = LineIniReader.Instance.FileProviderAsset(
                    fileProvider: fileProvider,
                    filepath: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10c
            }
            {
                #region Snippet_10d
                IAsset asset = LineReaderMap.Instance.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10d
            }

            {
                #region Snippet_11a
                IAssetSource assetSource = 
                    LineIniReader.Instance.FileAssetSource(
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
                    LineIniReader.Instance.EmbeddedAssetSource(
                        assembly: asm,
                        resourceName: "docs.localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11b
            }
            {
                #region Snippet_11c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAssetSource assetSource = 
                    LineIniReader.Instance.FileProviderAssetSource(
                        fileProvider: fileProvider,
                        filepath: "localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11c
            }
            {
                #region Snippet_11d
                IAssetSource assetSource = LineReaderMap.Instance.FileAssetSource(
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
                ILineReader format = new ExtFileFormatReader();

                // Clone formats
                LineFileFormatMap formats = LineReaderMap.Instance.Clone();
                // Add to clone
                formats.Add(format);

                // Or if in deploying application project, format can be added to the global singleton
                (LineReaderMap.Instance as IDictionary<string, ILineFileFormat>).Add(format);
                #endregion Snippet_30a
            }

        }
    }

    #region Snippet_30
    class ExtFileFormatReader : ILineTextReader
    {
        public string Extension => "ext";

        public IEnumerable<ILine> ReadKeyLines(
            TextReader text, 
            ILineFormat namePolicy = null)
        {
            ILine key = Key.Create("Section", "MyClass").Append("Key", "HelloWorld").Append("Culture", "en");
            yield return new ILine(key, CSharpFormat.Instance.Parse("Hello World!"));
        }
    }
    #endregion Snippet_30

}
