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

namespace Lexical.Localization.LocalizationFile
{
    public class IniFileFormat : ILocalizationFileTextReader, ILocalizationFileStreamReader, ILocalizationFileStreamWriter, ILocalizationFileTextWriter
    {
        static readonly IniFileFormat singleton = new IniFileFormat();
        public static IniFileFormat Singleton => singleton;

        public string Extension => "ini";

        public ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new IniWritable(stream, namePolicy);

        public ILocalizationFileWritable CreateText(TextWriter text, IAssetKeyNamePolicy namePolicy = default)
            => new IniWritable(text, namePolicy);

        public ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new IniReadable(stream, namePolicy);

        public ILocalizationFileReadable OpenText(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => new IniReadable(text, namePolicy);
    }

    public class IniReadable : ILocalizationFileReadable, IDisposable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        TextReader textReader;

        public IniReadable(TextReader textReader, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = textReader;
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IniReadable(Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            this.textReader = new StreamReader(stream, true);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;
        }

        public IEnumerable<TextElement> Read()
        {
            string currentSection = null;
            while (textReader.Peek() >= 0)
            {
                // Read, trim
                string line = textReader.ReadLine().Trim();
                if (String.IsNullOrWhiteSpace(line)) continue;
                char ch = line[0];

                // Comment
                if (ch == ';' || ch == '#' || ch == '/') continue;

                // Section
                if (ch == '[' && line[line.Length - 1] == ']')
                {
                    if (currentSection != null) { yield return TextElement.End(); currentSection = null; }
                    string sectionName = line.Substring(1, line.Length - 2).Trim();
                    if (!string.IsNullOrEmpty(sectionName)) { yield return TextElement.Begin(sectionName); currentSection = sectionName; }
                    continue;
                }

                // Key-Value
                int ix = line.IndexOf('=');
                if (ix >= 0)
                {
                    string key = line.Substring(0, ix).Trim(), value = line.Substring(ix + 1).Trim();
                    yield return TextElement.KeyValue(key, value);
                }
            }
            if (currentSection != null) yield return TextElement.End();
        }

        public void Dispose()
        {
            textReader?.Dispose();
            textReader = null;
        }
    }

    public class IniWritable : ILocalizationFileWritable, IDisposable
    {
        protected TextWriter writer;

        public IAssetKeyNamePolicy NamePolicy { get; internal set; }
        IAssetKeyNamePolicy keyNamePolicy;

        public IniWritable(Stream stream, IAssetKeyNamePolicy namePolicy) : this(new StreamWriter(stream, Encoding.UTF8), namePolicy) { }
        public IniWritable(TextWriter writer, IAssetKeyNamePolicy namePolicy)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Default;

            // Create keyNamePolicy where "culture" is not printed out second time.
            if (NamePolicy is AssetKeyNameProvider provider)
            {
                var copy = provider.Clone() as AssetKeyNameProvider;
                copy.SetParameter("culture", false);
                this.keyNamePolicy = copy;
            }
            else if (NamePolicy is IAssetNamePattern pattern)
            {
                // namepolicy is pattern
                if (pattern.ParameterMap.ContainsKey("culture"))
                {
                    // Create new pattern, remove one "culture" part
                    StringBuilder sb = new StringBuilder();
                    bool cultureRemoved = false;
                    foreach (var part in pattern.AllParts)
                    {
                        if (!cultureRemoved && part.ParameterName == "culture") { cultureRemoved = true; continue; }
                        sb.Append(part.PatternText);
                    }
                    this.keyNamePolicy = new AssetNamePattern(sb.ToString());
                }
                else
                {
                    // There is no "culture" part
                    this.keyNamePolicy = pattern;
                }
            }
            else
            {
                this.keyNamePolicy = NamePolicy;
            }
        }

        public void Write(LocalizationKeyTree root)
        {
            // Write root lines
            int c = WriteLines(root);
            if (c > 0) writer.WriteLine();

            // Write all non-culture sections
            foreach (var node in root.Children.Values.Where(node => node.Proxy.ParameterName != "culture").OrderBy(node => node.Proxy, AssetKeyProxy.Comparer.Default))
            {
                _writeRecusive(node);
                writer.WriteLine();
            }

            // Write all culture sections.
            foreach (var node in root.Children.Values.Where(node => node.Proxy.ParameterName == "culture").OrderBy(node => node.Proxy, AssetKeyProxy.Comparer.Default))
            {
                writer.Write("[");
                writer.Write(node.ParameterValue);
                writer.Write("]");
                writer.WriteLine();

                _writeRecusive(node);
                writer.WriteLine();
            }
        }

        void _writeRecusive(LocalizationKeyTree node)
        {
            WriteLines(node);

            // Children
            foreach (LocalizationKeyTree childNode in node.Children.Values.OrderBy(n => n.Proxy, AssetKeyProxy.Comparer.Default))
            {
                _writeRecusive(childNode);
            }
        }

        int WriteLines(LocalizationKeyTree node)
        {
            if (!node.HasValues) return 0;

            // Key string
            string str = keyNamePolicy.BuildName(node, LocalizationKeyTree.Parametrizer.Instance);

            // Write lines
            node.Values.Sort(AlphaNumericComparer.Default);
            foreach (string value in node.Values)
            {
                writer.Write(str);
                writer.Write(" = ");
                writer.Write(value);
                writer.WriteLine();
            }
            return node.Values.Count;
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
