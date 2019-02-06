//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Lexical.Localization.LocalizationFile
{
    public class JsonFileFormat : ILocalizationFileTextReader, ILocalizationFileStreamReader, ILocalizationFileStreamWriter, ILocalizationFileTextWriter
    {
        static readonly JsonFileFormat singleton = new JsonFileFormat();
        public static JsonFileFormat Singleton => singleton;

        public string Extension => "json";

        public ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new JsonWritable(stream, namePolicy);

        public ILocalizationFileWritable CreateText(TextWriter text, IAssetKeyNamePolicy namePolicy = default)
            => new JsonWritable(text, namePolicy);

        public ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new JsonReader(stream, namePolicy);

        public ILocalizationFileReadable OpenText(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => new JsonReader(text, namePolicy);

    }

    public class JsonReader : ILocalizationFileReadable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        TextReader textReader;

        public JsonReader(TextReader textReader, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = textReader;
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public JsonReader(Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = new StreamReader(stream, true);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<TextElement> Read()
        {
            using (var json = new JsonTextReader(textReader))
            {
                string section = null;
                while (json.Read())
                {
                    switch (json.TokenType)
                    {
                        case JsonToken.StartObject:
                            if (section != null) yield return TextElement.Begin(section);
                            section = null;
                            break;
                        case JsonToken.EndObject:
                            yield return TextElement.End();
                            break;
                        case JsonToken.PropertyName: section = json.Value?.ToString(); break;
                        case JsonToken.Date:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Float:
                        case JsonToken.Integer:
                            if (section != null) yield return TextElement.KeyValue(section, json.Value?.ToString());
                            section = null;
                            break;
                    }
                }
            }
        }
        public void Dispose()
        {
            textReader?.Dispose();
            textReader = null;
        }
    }

    public class JsonWritable : ILocalizationFileWritable, IDisposable
    {
        protected TextWriter writer;

        public IAssetKeyNamePolicy NamePolicy { get; internal set; }

        public JsonWritable(TextWriter writer, IAssetKeyNamePolicy namePolicy)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public JsonWritable(Stream stream, IAssetKeyNamePolicy namePolicy)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            this.writer = new StreamWriter(stream, Encoding.UTF8);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public void Write(LocalizationKeyTree root)
        {
            _writeRecusive(root, 0, false);
        }

        void _writeRecusive(LocalizationKeyTree node, int indent, bool continues)
        {
            // Test if is root
            bool isRoot = node.Parent == null;

            // Write name: "key": 
            Indent(indent);
            if (!isRoot)
            {
                writer.Write('"');
                writer.Write(HttpUtility.JavaScriptStringEncode(node.ParameterValue));
                writer.Write("\": ");
            }

            if (isRoot || node.HasChildren)
            {
                // Begin section: {
                writer.Write("{");
                writer.WriteLine();

                // Children
                int count = node.Children.Count;
                foreach (LocalizationKeyTree childNode in node.Children.Values.OrderBy(n => n.Proxy, AssetKeyProxy.Comparer.Default))
                    _writeRecusive(childNode, indent + 2, --count > 0);

                // End section: }\n
                Indent(indent);
                writer.Write('}');
            }
            else
            {
                // Write one value: "value",
                if (node.HasValues && node.Values.Count == 0)
                {
                    // ""
                    writer.Write("\"\"");
                }
                else if (node.Values.Count == 1)
                {
                    // "value"
                    writer.Write('\"');
                    writer.Write(HttpUtility.JavaScriptStringEncode(node.Values[0]));
                    writer.Write('\"');
                }
                else
                {
                    // Write Json array: [ "", "" ]
                    writer.Write("[ ");
                    for (int i = 0; i < node.Values.Count; i++)
                    {
                        if (i > 0) writer.Write(", ");
                        writer.Write('\"');
                        writer.Write(HttpUtility.JavaScriptStringEncode(node.Values[i]));
                        writer.Write('\"');
                    }
                    writer.Write(" ]");
                }
            }
            if (continues) writer.Write(",");
            writer.WriteLine();
        }

        void Indent(int indentCount)
        {
            for (int i = 0; i < indentCount; i++) writer.Write(' ');
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Dispose();
            }
            writer = null;
        }

        public void Flush()
            => writer?.Flush();
    }


}
