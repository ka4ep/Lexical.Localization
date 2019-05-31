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
using Lexical.Localization.StringFormat;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads ".json" localization files.
    /// </summary>
    public class JsonLinesReader : ILineFileFormat, ILineTreeTextReader
    {
        private readonly static JsonLinesReader non_resolving = new JsonLinesReader("json", LineAppender.NonResolving);
        private readonly static JsonLinesReader resolving = new JsonLinesReader("json", LineAppender.Default);

        /// <summary>
        /// .json file lines reader that does not resolve parameters into instantances.
        /// 
        /// Used when handling localization files as texts, not for localization
        /// </summary>
        public static JsonLinesReader NonResolving => non_resolving;

        /// <summary>
        /// .json file lines reader that resolves parameters into instantances.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "Value" is created as to <see cref="ILineValue"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// 
        /// Used when reading localization files for localization purposes.
        /// </summary>
        public static JsonLinesReader Default => resolving;

        /// <summary>
        /// Parameter parser.
        /// </summary>
        protected LineFormat lineFormat;

        /// <summary>
        /// The file extension without dot "ini".
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Line factory that instantiates lines.
        /// </summary>
        public ILineFactory LineFactory { get; protected set; }

        /// <summary>
        /// Resolver that was extracted from LineFactory.
        /// </summary>
        protected IResolver resolver;

        /// <summary>
        /// Create new .json reader.
        /// </summary>
        public JsonLinesReader() : this("json", LineAppender.Default)
        {
        }

        /// <summary>
        /// Create new .json reader.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="lineFactory"></param>
        public JsonLinesReader(string ext, ILineFactory lineFactory)
        {
            this.Extension = ext ?? throw new ArgumentNullException(nameof(ext));
            this.lineFormat = new LineFormat(" :\\", false, " :\\", false, lineFactory, null) ?? throw new ArgumentNullException(nameof(lineFactory));
            this.LineFactory = lineFactory;
            lineFactory.TryGetResolver(out resolver);
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
            ILineFactory tmp;
            ILineFormat _lineFormat = lineFormat.TryGetLineFactory(out tmp) && tmp != LineFactory ? new LineFormat(" :\\", false, " :\\", false, tmp, null) : this.lineFormat;

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
                                IStringFormat stringFormat;
                                if (current.TryGetStringFormat(resolver, out stringFormat))
                                {
                                    // Append FormatString
                                    IFormatString valueString = stringFormat.Parse(value);
                                    ILineValue lineValue = LineFactory.Create<ILineValue, IFormatString>(null, valueString);
                                    current.Values.Add(lineValue);
                                    if (updateCorrespondence) correspondenceContext.Values[new LineTreeValue(current, lineValue, ix)] = (JValue)tokenReader.CurrentToken;
                                }
                                else
                                {
                                    // Append Hint
                                    ILineHint lineValue = LineFactory.Create<ILineHint, string, string>(null, "Value", value);
                                    current.Values.Add(lineValue);
                                    if (updateCorrespondence) correspondenceContext.Values[new LineTreeValue(current, lineValue, ix)] = (JValue)tokenReader.CurrentToken;
                                }
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
