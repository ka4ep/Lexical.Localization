using Lexical.Localization;
using Lexical.Localization.Utils;
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lexical.Localization.StringFormat;

namespace docs
{
    public class LocalizationReader_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_0a
                ILineFileFormat format = LineReaderMap.Default["ini"];
                #endregion Snippet_0a
            }
            {
                #region Snippet_0b
                ILineFileFormat format = IniLinesReader.Default;
                #endregion Snippet_0b
                ILineFileFormat format2a = LineReaderMap.Default["json"];
                ILineFileFormat format2b = JsonLinesReader.Default;
                ILineFileFormat format3a = LineReaderMap.Default["xml"];
                ILineFileFormat format3b = XmlLinesReader.Default;
                ILineFileFormat format4a = LineReaderMap.Default["resx"];
                ILineFileFormat format4b = ResxLinesReader.Default;
                ILineFileFormat format5a = LineReaderMap.Default["resources"];
                ILineFileFormat format5b = ResourcesLineReader.Default;
            }

            {
                #region Snippet_1a
                IEnumerable<ILine> key_lines = LineReaderMap.Default.ReadLines(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                IEnumerable<KeyValuePair<string, IString>> string_lines = LineReaderMap.Default.ReadStringLines(
                    filename: "localization.ini", 
                    lineFormat: LineFormat.Parameters,
                    throwIfNotFound: true);
                #endregion Snippet_1b
            }
            {
                #region Snippet_1c
                ILineTree tree = LineReaderMap.Default.ReadLineTree(
                    filename: "localization.ini", 
                    throwIfNotFound: true);
                #endregion Snippet_1c
            }

            {
                #region Snippet_2a
                IEnumerable<ILine> key_lines_reader = 
                    LineReaderMap.Default.FileReader(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2a
            }
            {
                #region Snippet_2b
                IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
                    LineReaderMap.Default.FileReaderAsStringLines(
                        filename: "localization.ini",
                        lineFormat: LineFormat.Parameters,
                        throwIfNotFound: true);
                #endregion Snippet_2b
                var lines = string_lines_reader.ToArray();
            }
            {
                #region Snippet_2c
                IEnumerable<ILineTree> tree_reader = 
                    LineReaderMap.Default.FileReaderAsLineTree(
                        filename: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_2c
                var lines = tree_reader.ToArray();
            }

            {
                #region Snippet_3a
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IEnumerable<ILine> key_lines_reader = 
                    LineReaderMap.Default.EmbeddedReader(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_3a
                var lines = key_lines_reader.ToArray();
            }
            {
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                #region Snippet_3b
                IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
                    LineReaderMap.Default.EmbeddedReaderAsStringLines(
                        assembly: asm, 
                        resourceName: "docs.localization.ini", 
                        namePolicy: LineFormat.Parameters,
                        throwIfNotFound: true);
                #endregion Snippet_3b
                var lines = string_lines_reader.ToArray();
            }
            {
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                #region Snippet_3c
                IEnumerable<ILineTree> tree_reader = 
                    LineReaderMap.Default.EmbeddedReaderAsLineTree(
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
                    LineReaderMap.Default.FileProviderReader(
                        fileProvider: fileProvider, 
                        filepath: "localization.ini", 
                        throwIfNotFound: true);
                #endregion Snippet_4a
                var lines = key_lines_reader.ToArray();
            }
            {
                #region Snippet_4b
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IEnumerable<KeyValuePair<string, IString>> string_lines_reader = 
                    LineReaderMap.Default.FileProviderReaderAsStringLines(
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
                    LineReaderMap.Default.FileProviderReaderAsLineTree(
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
                    IEnumerable<ILine> key_lines = IniLinesReader.Default.ReadLines(s);
                }
                #endregion Snippet_5a
            }
            {
                #region Snippet_5b
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    IEnumerable<KeyValuePair<string, IString>> string_lines = IniLinesReader.Default.ReadStringLines(
                        stream: s,
                        namePolicy: LineFormat.Parameters);
                }
                #endregion Snippet_5b
            }
            {
                #region Snippet_5c
                using (Stream s = new FileStream("localization.ini", FileMode.Open, FileAccess.Read))
                {
                    ILineTree tree = IniLinesReader.Default.ReadLineTree(s);
                }
                #endregion Snippet_5c
            }


            {
                #region Snippet_6a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<ILine> key_lines = IniLinesReader.Default.ReadLines(tr);
                }
                #endregion Snippet_6a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6b
                using (TextReader tr = new StringReader(text))
                {
                    IEnumerable<KeyValuePair<string, IString>> string_lines = IniLinesReader.Default.ReadStringLines(
                        srcText: tr,
                        namePolicy: LineFormat.Parameters);
                }
                #endregion Snippet_6b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_6c
                using (TextReader tr = new StringReader(text))
                {
                    ILineTree tree = IniLinesReader.Default.ReadLineTree(tr);
                }
                #endregion Snippet_6c
            }

            {
                #region Snippet_7a
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                IEnumerable<ILine> key_lines = 
                    IniLinesReader.Default.ReadString(
                        srcText: text);
                #endregion Snippet_7a
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7b
                IEnumerable<KeyValuePair<string, IString>> string_lines = 
                    IniLinesReader.Default.ReadStringAsStringLines(
                        srcText: text,
                        namePolicy: LineFormat.Parameters);
                #endregion Snippet_7b
            }
            {
                string text = "Culture:en:Type:MyController:Key:Hello = Hello World!\n";
                #region Snippet_7c
                ILineTree tree = 
                    IniLinesReader.Default.ReadStringAsLineTree(
                        srcText: text);
                #endregion Snippet_7c
            }

            {
                #region Snippet_10a
                IAsset asset = IniLinesReader.Default.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10a
            }
            {
                #region Snippet_10b
                Assembly asm = typeof(LocalizationReader_Examples).Assembly;
                IAsset asset = IniLinesReader.Default.EmbeddedAsset(
                    assembly: asm,
                    resourceName: "docs.localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10b
            }
            {
                #region Snippet_10c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAsset asset = IniLinesReader.Default.FileProviderAsset(
                    fileProvider: fileProvider,
                    filepath: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10c
            }
            {
                #region Snippet_10d
                IAsset asset = LineReaderMap.Default.FileAsset(
                    filename: "localization.ini",
                    throwIfNotFound: true);
                #endregion Snippet_10d
            }

            {
                #region Snippet_11a
                IAssetSource assetSource = 
                    IniLinesReader.Default.FileAssetSource(
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
                    IniLinesReader.Default.EmbeddedAssetSource(
                        assembly: asm,
                        resourceName: "docs.localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11b
            }
            {
                #region Snippet_11c
                IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                IAssetSource assetSource = 
                    IniLinesReader.Default.FileProviderAssetSource(
                        fileProvider: fileProvider,
                        filepath: "localization.ini",
                        throwIfNotFound: true);
                #endregion Snippet_11c
            }
            {
                #region Snippet_11d
                IAssetSource assetSource = LineReaderMap.Default.FileAssetSource(
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
                LineFileFormatMap formats = LineReaderMap.Default.Clone();
                // Add to clone
                formats.Add(format);

                // Or if in deploying application project, format can be added to the global singleton
                (LineReaderMap.Default as IDictionary<string, ILineFileFormat>).Add(format);
                #endregion Snippet_30a
            }

        }
    }

    #region Snippet_30
    class ExtFileFormatReader : ILineTextReader
    {
        public string Extension => "ext";

        public IEnumerable<ILine> ReadLines(
            TextReader text, 
            ILineFormat namePolicy = null)
        {
            yield return LineAppender.Default.Section("MyClass").Key("HelloWorld").Culture("en").String("Hello World!");
        }
    }
    #endregion Snippet_30

}
