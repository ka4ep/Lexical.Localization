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
    public class IniFileFormat : ILocalizationFileFormat, ILocalizationKeyTreeTextReader
    {
        private readonly static IniFileFormat instance = new IniFileFormat();
        public static IniFileFormat Instance => instance;
        public string Extension => "ini";
        static ParameterNamePolicy parser_comment = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");
        static ParameterNamePolicy parser_section = new ParameterNamePolicy("\\\n\t\r\0\a\b\f[]");
        static ParameterNamePolicy parser_key = new ParameterNamePolicy("\\\n\t\r\0\a\b\f=");
        static ParameterNamePolicy parser_value = new ParameterNamePolicy("\\\n\t\r\0\a\b\f");

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
        {
            KeyTree root = new KeyTree(Key.Root);
            using (var ini = new IniTokenReader(text.ReadToEnd()))
                ReadIniIntoTree(ini, root, namePolicy);
            return root;
        }

        /// <summary>
        /// Read ini token stream into <paramref name="node"/>
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="node">parent node to under which add nodes</param>
        /// <param name="namePolicy"></param>
        /// <returns><paramref name="node"/></returns>
        public IKeyTree ReadIniIntoTree(IniTokenReader ini, KeyTree node, IAssetKeyNamePolicy namePolicy = default)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            KeyTree section = null;
            foreach(IniToken token in ini)
            {
                switch (token.Type)
                {
                    case IniTokenType.Section:
                        Key key = null;
                        parameters.Clear();
                        if (parser_section.TryParseParameters(token.ValueText, parameters))
                        {
                            foreach (var parameter in parameters)
                                key = Key.Create(key, parameter.Key, parameter.Value);
                            section = key == null ? null : node.GetOrCreateChild(key);
                        }
                        else
                        {
                            section = null;
                        }
                        break;
                    case IniTokenType.KeyValue:
                        Key key_ = null;
                        parameters.Clear();
                        if (parser_key.TryParseParameters(token.KeyText, parameters))
                        {
                            foreach (var parameter in parameters)
                                key_ = Key.Create(key_, parameter.Key, parameter.Value);
                            KeyTree current = key_ == null ? null : (section??node).GetOrCreateChild(key_);
                            string value = token.Value;
                            if (!current.Values.Contains(value)) current.Values.Add(value);
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
