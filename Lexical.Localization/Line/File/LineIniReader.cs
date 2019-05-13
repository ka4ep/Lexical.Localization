﻿// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical.Localization
{
    /// <summary>
    /// File format reader that handles .ini files. 
    /// 
    /// Uses two level structures. The first level is the section '[parameterName:parameterValue:...]'.
    /// Chacters \:[] and white-spaces are escaped.
    /// 
    /// And second level is key-value pairs 'parameterName:parameterValue:.. = value'.
    /// Characters \:= and white-spaces are escaped.
    /// </summary>
    public class LineIniReader : ILineFileFormat, ILineTreeTextReader
    {
        private readonly static LineIniReader instance = new LineIniReader();

        /// <summary>
        /// Default instance of .ini localization reader.
        /// </summary>
        public static LineIniReader Instance => instance;

        /// <summary>
        /// Escaper for "[section]" parts of .ini files. Escapes '\', ':', '[' and ']' characters and white-spaces.
        /// </summary>
        protected LineFormat escaper_section = new LineFormat("\\:[]", true, "\\:[]", true);

        /// <summary>
        /// Escaper for key parts of .ini files. Escapes '\', ':', '=' characters and white-spaces.
        /// </summary>
        protected LineFormat escaper_key = new LineFormat("\\:= ", true, "\\:= ", true);

        /// <summary>
        /// Escaper for value parts of .ini files. Escapes '\', '{', '}' characters and white-spaces.
        /// </summary>
        protected IniEscape escaper_value = new IniEscape("\\");

        /// <summary>
        /// The file extension without dot "ini".
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public IStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Create new ini file reader.
        /// </summary>
        public LineIniReader() : this("ini", CSharpFormat.Instance) { }

        /// <summary>
        /// Create new ini file reader.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="valueParser"></param>
        public LineIniReader(string ext, IStringFormat valueParser)
        {
            this.Extension = ext;
            this.ValueParser = valueParser as IStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
        }

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public ILineTree ReadLineTree(TextReader text, ILineFormat namePolicy = default)
        {
            LineTree root = new LineTree();
            using (var ini = new IniTokenizer(text.ReadToEnd()))
                ReadIniIntoTree(ini, root, namePolicy, null);
            return root;
        }

        /// <summary>
        /// Read ini token stream into <paramref name="root"/>
        /// </summary>
        /// <param name="ini">ini token stream. </param>
        /// <param name="root">parent node to under which add nodes</param>
        /// <param name="namePolicy"></param>
        /// <param name="correspondence">(optional) if set tokens are associated to key tree. If <paramref name="correspondence"/> is provided, then <paramref name="ini"/> must be a linked list. See <see cref="IniTokenizer.ToLinkedList"/></param>
        /// <returns><paramref name="root"/></returns>
        public ILineTree ReadIniIntoTree(IEnumerable<IniToken> ini, ILineTree root, ILineFormat namePolicy, IniCorrespondence correspondence)
        {
            ILineTree section = null;
            foreach (IniToken token in ini)
            {
                switch (token.Type)
                {
                    case IniTokenType.Section:
                        ILine key = null;
                        if (escaper_section.TryParse(token.ValueText, out key))
                        {
                            section = key == null ? null : root.Create(key);
                            if (section != null && correspondence != null) correspondence.Nodes.Put(section, token);
                        }
                        else
                        {
                            section = null;
                        }
                        break;
                    case IniTokenType.KeyValue:
                        ILine key_ = null;
                        if (escaper_key.TryParse(token.KeyText, out key_))
                        {
                            ILineTree current = key_ == null ? null : (section ?? root).GetOrCreate(key_);
                            string value = escaper_value.UnescapeLiteral(token.ValueText);
                            if (value != null)
                            {
                                int ix = current.Values.Count;
                                IFormulationString formulationString = ValueParser.Parse(value);
                                current.Values.Add(formulationString);
                                if (correspondence != null) correspondence.Values[new LineTreeValue(current, formulationString, ix)] = token;
                            }
                        }
                        break;
                    case IniTokenType.Comment: break;
                    case IniTokenType.Text: break;
                }
            }
            return root;
        }
    }


}