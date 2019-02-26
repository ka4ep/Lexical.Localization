// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization.LocalizationFile2
{
    public class JsonFileFormat : ILocalizationFileFormat, ILocalizationTreeTextReader
    {
        private readonly static JsonFileFormat instance = new JsonFileFormat();
        public static JsonFileFormat Instance => instance;

        public string Extension => "json";
        protected ParameterNamePolicy parser = new ParameterNamePolicy("\\\n\t\r\0\a\b\f:\"");

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IKeyTree ReadTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            KeyTree root = new KeyTree(Key.Root);
            using (var json = new JsonTextReader(text))
                ReadJsonIntoTree(json, root, namePolicy);
            return root;
        }

        /// <summary>
        /// Read json token stream into <paramref name="node"/>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="node">parent node to under which add nodes</param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IKeyTree ReadJsonIntoTree(JsonTextReader json, KeyTree node, IAssetKeyNamePolicy namePolicy = default)
        {
            KeyTree current = node;
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            Stack<KeyTree> stack = new Stack<KeyTree>();
            while (json.Read())
            {
                switch (json.TokenType)
                {
                    case JsonToken.StartObject:
                        stack.Push(current);
                        break;
                    case JsonToken.EndObject:
                        current = stack.Pop();
                        break;
                    case JsonToken.PropertyName:
                        Key key = null;
                        parameters.Clear();
                        if (parser.TryParseParameters(json.Value?.ToString(), parameters))
                        {
                            foreach (var parameter in parameters)
                                key = Key.Create(key, parameter.Key, parameter.Value);
                            current = key == null ? null : stack.Peek()?.GetOrCreateChild(key);
                        }
                        else
                        {
                            current = null;
                        }
                        break;
                    case JsonToken.Date:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Float:
                    case JsonToken.Integer:
                        if (current != null)
                        {
                            string value = json.Value?.ToString();
                            if (value != null && !current.Values.Contains(value))
                                current.Values.Add(value);
                        }
                        break;
                }
            }
            return node;
        }

    }

    public class JsonFileAsset : LocalizationAsset
    {
        public JsonFileAsset(string filename) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IKeyTree keyTree = JsonFileFormat.Instance.ReadTree(stream, null);
                var lines = keyTree.ToLines(true).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public JsonFileAsset(Stream stream) : base()
        {
            IKeyTree keyTree = JsonFileFormat.Instance.ReadTree(stream, null);
            var lines = keyTree.ToLines(true).ToArray();
            AddKeySource(lines);
            Load();
        }
    }

}
