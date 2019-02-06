//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           25.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Asset;
using Lexical.Asset.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Lexical.Localization.LocalizationFile
{
    public class ResXFileFormat : ILocalizationFileTextReader, ILocalizationFileStreamReader, ILocalizationFileStreamWriter, ILocalizationFileTextWriter
    {
        static readonly ResXFileFormat singleton = new ResXFileFormat();
        public static ResXFileFormat Singleton => singleton;

        public string Extension => "resx";

        public ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new ResXWritable(stream, namePolicy);

        public ILocalizationFileWritable CreateText(TextWriter text, IAssetKeyNamePolicy namePolicy = default)
            => new ResXWritable(text, namePolicy);

        public ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new ResXReadable(stream, namePolicy);

        public ILocalizationFileReadable OpenText(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => new ResXReadable(text, namePolicy);
    }

    public class ResXReadable : ILocalizationFileReadable, IDisposable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        TextReader textReader;

        public ResXReadable(Stream stream, IAssetKeyNamePolicy namePolicy = default) : this(new StreamReader(stream, true), namePolicy) { }
        public ResXReadable(TextReader textReader, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = textReader;
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Colon_Dot_Dot;
        }

        public IEnumerable<TextElement> Read()
        {
            using (var xml = System.Xml.XmlReader.Create(textReader))
            {
                string key_name = null, key_type = null;
                bool in_value = false, in_data = false;
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xml.Name == "data") { in_data = true; key_name = xml.GetAttribute("name"); key_type = xml.GetAttribute("type"); }
                            else if (in_data && xml.Name == "value") in_value = true;
                            break;
                        case XmlNodeType.EndElement:
                            if (xml.Name == "data") { in_data = false; key_name = null; key_type = null; }
                            else if (in_data && xml.Name == "value") in_value = false;
                            break;
                        case XmlNodeType.Text:
                            if (in_value && key_name != null && xml.Value != null)
                            {
                                //if (key_type == null)
                                yield return TextElement.KeyValue(key_name, xml.Value);
                            }
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

    /// <summary>
    /// Writes .resx files.
    /// 
    /// Uses template because ResXWriter is not available in .NET Core.
    /// </summary>
    public class ResXWritable : ILocalizationFileWritable, IDisposable
    {
        public IAssetKeyNamePolicy NamePolicy { get; internal set; }
        TextWriter writer;
        string template_header, template_footer, template_keyvalue_formulation, template_binary_keyvalue_formulation;

        public ResXWritable(Stream stream, IAssetKeyNamePolicy namePolicy) : this(new StreamWriter(stream, Encoding.UTF8), namePolicy) { }
        public ResXWritable(TextWriter writer, IAssetKeyNamePolicy namePolicy)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.NamePolicy = namePolicy;

            string embedded_name = typeof(ResXWritable).Assembly.GetName().Name + ".LocalizationFile.ResXFileFormat.txt";
            using (Stream s = typeof(ResXWritable).Assembly.GetManifestResourceStream(embedded_name))
            {
                if (s == null) throw new InvalidOperationException("Could not find embedded resources " + embedded_name);

                TextReader reader = new StreamReader(s, true);
                StringBuilder sb = new StringBuilder((int)s.Length);
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (line.Contains("TEMPLATE_KEY"))
                    {
                        if (sb.Length > 0)
                        {
                            template_header = sb.ToString();
                            sb.Clear();
                        }
                        template_keyvalue_formulation = line.Replace("TEMPLATE_KEY", "{0}").Replace("TEMPLATE_VALUE", "{1}");
                    }
                    else if (line.Contains("TEMPLATE_BINARY_KEY"))
                    {
                        template_binary_keyvalue_formulation = line.Replace("TEMPLATE_BINARY_KEY", "{0}").Replace("TEMPLATE_BINARY_VALUE_REFERENCE", "{1}");
                    }
                    else
                    {
                        sb.Append(line);
                        sb.Append("\r\n");
                    }
                }
                template_footer = sb.ToString();

                if (template_keyvalue_formulation == null) throw new InvalidOperationException("Could not find TEMPLATE_KEY in .resx template file");
            }
        }

        public void Write(LocalizationKeyTree root)
        {
            writer.WriteLine(template_header);
            _writeRecusive(root);
            writer.WriteLine(template_footer);
        }

        void _writeRecusive(LocalizationKeyTree node)
        {
            if (node.HasValues)
            {
                // Write lines
                foreach (string value in node.Values.OrderBy(n => n, AlphaNumericComparer.Default))
                {
                    string key = NamePolicy.BuildName(node, LocalizationKeyTree.Parametrizer.Instance);
                    string str = string.Format(template_keyvalue_formulation, HttpUtility.HtmlEncode(key), HttpUtility.HtmlEncode(value));
                    writer.WriteLine(str);
                }
            }

            // Children
            if (node.HasChildren)
            {
                foreach (LocalizationKeyTree childNode in node.Children.Values.OrderBy(n => n.Proxy, AssetKeyProxy.Comparer.Default))
                {
                    _writeRecusive(childNode);
                }
            }
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
            }
            writer = null;
        }

        public void Flush()
            => writer?.Flush();
    }


}
