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
        static ParameterNamePolicy parser_comment = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");
        static ParameterNamePolicy parser_section = new ParameterNamePolicy("\\\n\t\r\0\a\b\f[]");
        static ParameterNamePolicy parser_key = new ParameterNamePolicy("\\\n\t\r\0\a\b\f=");
        static ParameterNamePolicy parser_value = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");

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
        /// Read ini token stream into <paramref name="node"/>
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="node">parent node to under which add nodes</param>
        /// <param name="namePolicy"></param>
        /// <param name="correspondenceContext"></param>
        /// <returns><paramref name="node"/></returns>
        public IKeyTree ReadIniIntoTree(IniTokenizer ini, IKeyTree node, IAssetKeyNamePolicy namePolicy, KeyTreeIniCorrespondence correspondenceContext)
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
                            section = key == null ? null : node.GetOrCreate(key);
                            if (section != null && correspondenceContext != null) correspondenceContext.Nodes.Put(section, token);
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
                            IKeyTree current = key_ == null ? null : (section??node).GetOrCreate(key_);
                            string value = token.Value;
                            if (value != null)
                            {
                                int ix = current.Values.Count;
                                current.Values.Add(value);
                                if (section != null && correspondenceContext != null) correspondenceContext.Values[new KeyTreeValue(section, value, ix)] = token;
                            }
                        }
                        break;
                    case IniTokenType.Comment: break;
                    case IniTokenType.Text: break;
                }
            }
            return node;
        }
    }


}
