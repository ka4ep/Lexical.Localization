// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Lexical.Localization.Utils;
using Newtonsoft.Json.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads ".json" localization files.
    /// </summary>
    public class LineJsonReader : ILineFileFormat, ILineTreeTextReader
    {
        private readonly static LineJsonReader instance = new LineJsonReader();

        /// <summary>
        /// Default intance of .json reader
        /// </summary>
        public static LineJsonReader Instance => instance;

        /// <summary>
        /// Polity to read keys.
        /// </summary>
        protected LineFormat lineFormat = new LineFormat(" :\\", false, " :\\", false, ParameterInfos.Default);

        /// <summary>
        /// File extension, default "json"
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public IStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// (optional) Parameter infos for determining if parameter is key.
        /// </summary>
        protected IParameterInfos parameterInfos;

        /// <summary>
        /// Create new .json reader.
        /// </summary>
        public LineJsonReader() : this("json", CSharpFormat.Instance)
        {
        }

        /// <summary>
        /// Create new .json reader.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="valueParser"></param>
        public LineJsonReader(string ext, IStringFormat valueParser)
        {
            this.Extension = ext;
            this.ValueParser = valueParser as IStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
        }

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">unused</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default)
        {
            LineTree root = new LineTree();
            using (var json = new JsonTextReader(text))
            {
                ReadJsonIntoTree(json, root, lineFormat, null);
            }
            return root;
        }

        /// <summary>
        /// Read json token stream into <paramref name="node"/>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="node">parent node to under which add nodes</param>
        /// <param name="lineFormat">unused</param>
        /// <param name="correspondenceContext">(optional) place to update correspondence. If set <paramref name="json"/> must implement <see cref="JTokenReader"/>.</param>
        /// <returns></returns>
        public ILineTree ReadJsonIntoTree(JsonReader json, ILineTree node, ILineFormat lineFormat, JsonCorrespondence correspondenceContext)
        {
            LineFormat _lineFormat = this.lineFormat.GetParameterInfos() == lineFormat.GetParameterInfos() ? this.lineFormat : new LineFormat(" :\\", false, " :\\", false, lineFormat.GetParameterInfos());
            ILineTree current = node;
            Stack<ILineTree> stack = new Stack<ILineTree>();
            JTokenReader tokenReader = json as JTokenReader;
            bool updateCorrespondence = correspondenceContext != null && tokenReader != null;
            while (json.Read())
            {
                switch (json.TokenType)
                {
                    case JsonToken.StartObject:
                        stack.Push(current);
                        if (updateCorrespondence) correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                        break;
                    case JsonToken.EndObject:
                        current = stack.Pop();
                        break;
                    case JsonToken.PropertyName:
                        ILine key = null;
                        if (_lineFormat.TryParse(json.Value?.ToString(), out key))
                        { 
                            current = key == null ? stack.Peek() : stack.Peek()?.Create(key);
                            if (current != null && updateCorrespondence && !correspondenceContext.Nodes.ContainsLeft(current))
                            {
                                correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                            }
                        }
                        else
                        {
                            current = null;
                        }
                        break;
                    case JsonToken.Raw:
                    case JsonToken.Date:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Float:
                    case JsonToken.Integer:
                        if (current != null)
                        {
                            string value = json.Value?.ToString();
                            if (value != null)
                            {
                                int ix = current.Values.Count;
                                IFormulationString formulationString = ValueParser.Parse(value);
                                current.Values.Add(formulationString);
                                if (updateCorrespondence) correspondenceContext.Values[new LineTreeValue(current, formulationString, ix)] = (JValue) tokenReader.CurrentToken;
                            }
                        }
                        break;
                    case JsonToken.StartArray:
                        if (updateCorrespondence) correspondenceContext.Nodes.Put(current, tokenReader.CurrentToken);
                        break;
                    case JsonToken.EndArray:
                        break;
                }
            }
            return node;
        }

    }

}
