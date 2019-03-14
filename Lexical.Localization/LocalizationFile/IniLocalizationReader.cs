// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Lexical.Localization.Utils;

namespace Lexical.Localization
{
    public class IniLocalizationReader : ILocalizationFileFormat, ILocalizationKeyTreeTextReader
    {
        private readonly static IniLocalizationReader instance = new IniLocalizationReader();
        public static IniLocalizationReader Instance => instance;
        protected ParameterNamePolicy parser_comment = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");
        protected ParameterNamePolicy parser_section = new ParameterNamePolicy("\\\n\t\r\0\a\b\f[]");
        protected ParameterNamePolicy parser_key = new ParameterNamePolicy("\\\n\t\r\0\a\b\f=");
        protected ParameterNamePolicy parser_value = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");

        public string Extension { get; protected set; }

        public IniLocalizationReader() : this("ini") { }
        public IniLocalizationReader(string ext)
        {
            this.Extension = ext;
        }

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            KeyTree root = new KeyTree(Key.Root);
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
        public IKeyTree ReadIniIntoTree(IEnumerable<IniToken> ini, IKeyTree root, IAssetKeyNamePolicy namePolicy, IniCorrespondence correspondence)
        {
            IKeyTree section = null;
            foreach(IniToken token in ini)
            {
                switch (token.Type)
                {
                    case IniTokenType.Section:
                        IAssetKey key = null;
                        if (parser_section.TryParse(token.ValueText, out key))
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
                        IAssetKey key_ = null;
                        if (parser_key.TryParse(token.KeyText, out key_))
                        {
                            IKeyTree current = key_ == null ? null : (section??root).GetOrCreate(key_);
                            string value = token.Value;
                            if (value != null)
                            {
                                int ix = current.Values.Count;
                                current.Values.Add(value);
                                if (correspondence != null) correspondence.Values[new KeyTreeValue(current, value, ix)] = token;
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
