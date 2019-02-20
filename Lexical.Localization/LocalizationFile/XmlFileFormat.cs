//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Lexical.Localization.LocalizationFile
{
    public class XmlFileFormat : ILocalizationFileTextReader, ILocalizationFileStreamReader, ILocalizationFileStreamWriter, ILocalizationFileTextWriter
    {
        static readonly XmlFileFormat singleton = new XmlFileFormat();
        public static XmlFileFormat Singleton => singleton;

        public string Extension => "xml";

        public ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new XmlWritable(stream, namePolicy);

        public ILocalizationFileWritable CreateText(TextWriter text, IAssetKeyNamePolicy namePolicy = default)
            => new XmlWritable(text, namePolicy);

        public ILocalizationFileTokenizer OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new XmlReadable(stream, namePolicy);

        public ILocalizationFileTokenizer OpenText(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => new XmlReadable(text, namePolicy);
    }

    public class XmlReadable : ILocalizationFileTokenizer, IDisposable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        TextReader textReader;

        public XmlReadable(Stream stream, IAssetKeyNamePolicy namePolicy) : this(new StreamReader(stream, true), namePolicy) { }
        public XmlReadable(TextReader textReader, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = textReader;
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<Token> Read()
        {
            using (var xml = System.Xml.XmlReader.Create(textReader))
            {
                xml.MoveToContent();
                string section = null;
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element: yield return Token.Begin(xml.Name); break;
                        case XmlNodeType.EndElement: yield return Token.End(); break;
                        case XmlNodeType.Text: yield return Token.KeyValue(section, xml.Value); break;
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

    public class XmlWritable : ILocalizationFileWritable, IDisposable
    {
        protected TextWriter writer;

        public IAssetKeyNamePolicy NamePolicy { get; internal set; }

        public XmlWritable(TextWriter writer, IAssetKeyNamePolicy namePolicy)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public XmlWritable(Stream stream, IAssetKeyNamePolicy namePolicy)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            this.writer = new StreamWriter(stream, Encoding.UTF8);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public void Write(TreeNode root)
        {
            writer.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            writer.WriteLine();
            _writeRecusive(root, 0);
            writer.WriteLine();
        }

        void _writeRecusive(TreeNode node, int indent)
        {
            bool isRoot = node.Parent == null;
            string name = isRoot ? "configuration" : node.ParameterValue;
            string _name = HttpUtility.HtmlEncode(name);

            // Values
            if (node.HasValues)
            {
                // Write lines
                foreach (string value in node.Values.OrderBy(l => l, AlphaNumericComparer.Default).Distinct().ToArray())
                {
                    Indent(indent);
                    writer.Write('<');
                    writer.Write(_name);
                    writer.Write('>');
                    writer.Write(HttpUtility.HtmlEncode(value));
                    writer.Write("</");
                    writer.Write(_name);
                    writer.Write('>');
                    writer.WriteLine();
                }
            }

            // Children
            if (node.HasChildren)
            {
                Indent(indent);
                writer.Write('<');
                writer.Write(_name);
                writer.Write('>');
                writer.WriteLine();
                foreach (TreeNode childNode in node.Children.Values.OrderBy(n => n.Parameter, Key.Comparer.Default))
                    _writeRecusive(childNode, indent + 2);
                Indent(indent);
                writer.Write("</");
                writer.Write(_name);
                writer.Write('>');
                writer.WriteLine();
            }
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
