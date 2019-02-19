using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace Lexical.Localization.LocalizationFile
{
    public class ResourcesFileFormat : ILocalizationFileStreamReader, ILocalizationFileStreamWriter
    {
        static readonly ResourcesFileFormat singleton = new ResourcesFileFormat();
        public static ResourcesFileFormat Singleton => singleton;

        public string Extension => "resources";

        public ILocalizationFileWritable CreateStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new ResourcesWritable(stream, namePolicy);

        public ILocalizationFileTokenizer OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => new ResourcesReadable(stream, namePolicy);
    }

    public class ResourcesReadable : ILocalizationFileTokenizer, IDisposable
    {
        public IAssetKeyNamePolicy NamePolicy { get; protected set; }
        System.Resources.ResourceReader reader;

        public ResourcesReadable(Stream stream, IAssetKeyNamePolicy namePolicy = null)
        {
            this.reader = new System.Resources.ResourceReader(stream);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Colon_Dot_Dot;
        }

        public IEnumerable<Token> Read()
        {
            IDictionaryEnumerator dict = reader.GetEnumerator();
            while (dict.MoveNext())
            {
                string key = null, value = null;
                try
                {
                    if (dict.Key is string _key && dict.Value is string _value)
                    { key = _key; value = _value; }
                }
                catch (Exception e)
                {
                    throw new LocalizationException("Failed to read .resources file", e);
                }
                if (key!=null && value != null)
                    yield return Token.KeyValue(key, value);
            }
        }

        public void Dispose()
        {
            reader?.Dispose();
            reader = null;
        }
    }

    public class ResourcesWritable : ILocalizationFileWritable, IDisposable
    {
        protected ResourceWriter writer;

        public IAssetKeyNamePolicy NamePolicy { get; internal set; }

        public ResourcesWritable(Stream stream, IAssetKeyNamePolicy namePolicy)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            this.writer = new ResourceWriter(stream);
            this.NamePolicy = namePolicy ?? AssetKeyNameProvider.Colon_Dot_Dot;
        }

        public void Write(TreeNode node)
        {
            if (node.HasValues)
            {
                // Write lines
                foreach (string value in node.Values.OrderBy(n => n, AlphaNumericComparer.Default))
                {
                    string str = NamePolicy.BuildName(node, TreeNode.Parametrizer.Instance);
                    writer.AddResource(str, value);
                }
            }

            // Children
            if (node.HasChildren)
            {
                foreach (TreeNode childNode in node.Children.Values.OrderBy(n => n.Proxy, ParameterKey.Comparer.Default))
                {
                    Write(childNode);
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
            => writer?.Generate();
    }


}
