// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class IniLinesReader : ILineFileFormat, ILineTreeTextReader
    {
        private readonly static IniLinesReader non_resolving = new IniLinesReader("ini", LineAppender.NonResolving);
        private readonly static IniLinesReader resolving = new IniLinesReader("ini", LineAppender.Default);

        /// <summary>
        /// .ini file lines reader that does not resolve parameters into instantances.
        /// 
        /// Used when handling localization files as texts, not for localization
        /// </summary>
        public static IniLinesReader NonResolving => non_resolving;

        /// <summary>
        /// .ini file lines reader that resolves parameters into instantances.
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
        public static IniLinesReader Default => resolving;

        /// <summary>
        /// Escaper for "[section]" parts of .ini files. Escapes '\', ':', '[' and ']' characters and white-spaces.
        /// </summary>
        protected LineFormat escaper_section;

        /// <summary>
        /// Escaper for key parts of .ini files. Escapes '\', ':', '=' characters and white-spaces.
        /// </summary>
        protected LineFormat escaper_key;

        /// <summary>
        /// Escaper for value parts of .ini files. Escapes '\', '{', '}' characters and white-spaces.
        /// </summary>
        protected IniEscape escaper_value = new IniEscape("\\");

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
        /// Create new ini file reader.
        /// </summary>
        public IniLinesReader() : this("ini", LineAppender.NonResolving) { }

        /// <summary>
        /// Create new ini file reader.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="lineFactory"></param>
        public IniLinesReader(string ext, ILineFactory lineFactory)
        {
            this.Extension = ext ?? throw new ArgumentNullException(nameof(ext));
            this.LineFactory = lineFactory ?? throw new ArgumentNullException(nameof(LineFactory));
            this.escaper_section = new LineFormat("\\:[]", true, "\\:[]", true, lineFactory, null);
            this.escaper_key = new LineFormat("\\:= ", true, "\\:= ", true, lineFactory, null);
            this.LineFactory = lineFactory;
            lineFactory.TryGetResolver(out resolver);
        }

        /// <summary>
        /// Json text into a tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat"></param>
        /// <returns></returns>
        public ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default)
        {
            LineTree root = new LineTree();
            using (var ini = new IniTokenizer(text.ReadToEnd()))
                ReadIniIntoTree(ini, root, lineFormat, null);
            return root;
        }

        /// <summary>
        /// Read ini token stream into <paramref name="root"/>
        /// </summary>
        /// <param name="ini">ini token stream. </param>
        /// <param name="root">parent node to under which add nodes</param>
        /// <param name="lineFormat">unused</param>
        /// <param name="correspondence">(optional) if set tokens are associated to key tree. If <paramref name="correspondence"/> is provided, then <paramref name="ini"/> must be a linked list. See <see cref="IniTokenizer.ToLinkedList"/></param>
        /// <returns><paramref name="root"/></returns>
        public ILineTree ReadIniIntoTree(IEnumerable<IniToken> ini, ILineTree root, ILineFormat lineFormat, IniCorrespondence correspondence)
        {
            ILineFactory tmp;
            ILineFormat _escaper_section = lineFormat.TryGetLineFactory(out tmp) && tmp != LineFactory ? new LineFormat("\\:[]", true, "\\:[]", true, tmp, null) : this.escaper_section;
            ILineFormat _escaper_key = lineFormat.TryGetLineFactory(out tmp) && tmp != LineFactory ? new LineFormat("\\:= ", true, "\\:= ", true, tmp, null) : this.escaper_key;

            ILineTree section = null;
            foreach (IniToken token in ini)
            {
                switch (token.Type)
                {
                    case IniTokenType.Section:
                        ILine key = null;
                        if (_escaper_section.TryParse(token.ValueText, out key))
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
                        if (_escaper_key.TryParse(token.KeyText, out key_))
                        {
                            ILineTree current = key_ == null ? null : (section ?? root).GetOrCreate(key_);
                            string value = escaper_value.UnescapeLiteral(token.ValueText);

                            if (value != null)
                            {
                                IStringFormat stringFormat;
                                ILine lineValue;
                                if (current.TryGetStringFormat(resolver, out stringFormat))
                                {
                                    // Append FormatString
                                    IFormatString valueString = stringFormat.Parse(value);
                                    lineValue = LineFactory.Create<ILineValue, IFormatString>(null, valueString);

                                }
                                else
                                {
                                    // Append Hint
                                    lineValue = LineFactory.Create<ILineHint, string, string>(null, "Value", value);
                                }

                                int ix = current.Values.Count;
                                current.Values.Add(lineValue);
                                if (correspondence != null) correspondence.Values[new LineTreeValue(current, lineValue, ix)] = token;
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
