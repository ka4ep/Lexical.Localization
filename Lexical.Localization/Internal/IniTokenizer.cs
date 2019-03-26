//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           26.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexical.Localization.Internal
{
    /// <summary>
    /// Token type
    /// </summary>
    public enum IniTokenType
    {
        /// <summary>
        /// Non-recognized text and white-space segment of text.
        /// </summary>
        Text,

        /// <summary>
        /// Token that represents section header "[section]"
        /// </summary>
        Section,

        /// <summary>
        /// Token that represents comment and line feed, e.g. "; comment\r\n" or "// comment\r\n", or "# comment\r\n"
        /// </summary>
        Comment,

        /// <summary>
        /// Token that represents key value pair, e.g. "key = value\r\n"
        /// </summary>
        KeyValue
    }

    /// <summary>
    /// Single token. 
    /// 
    /// Token can be a linked list, if it has <see cref="Next"/> and <see cref="Previous"/> assigned. 
    /// </summary>
    public class IniToken : ICloneable, IEnumerable<IniToken>
    {
        /// <summary>
        /// Create a Commment-token. "; comment"
        /// </summary>
        /// <param name="comment">Raw unescaped comment. Use any characters.</param>
        /// <returns></returns>
        public static IniToken Comment(string comment)
        {
            string content = "; " + IniEscape.Comment.EscapeLiteral(comment) + "\r\n";
            return new IniToken { Type = IniTokenType.Comment, Source = content, Index = 0, Length = content.Length, ValueIndex = 2, ValueLength = comment.Length };
        }

        /// <summary>
        /// Create a list of tokens from an array of <paramref name="comments"/>.
        /// </summary>
        /// <param name="comments"></param>
        /// <returns>tokens</returns>
        public static IEnumerable<IniToken> Comments(params string[] comments) => comments.Select(c => Comment(c));

        /// <summary>
        /// Create a list of tokens from an enumeration of <paramref name="comments"/>.
        /// </summary>
        /// <param name="comments"></param>
        /// <returns>tokens</returns>
        public static IEnumerable<IniToken> Comments(IEnumerable<string> comments) => comments.Select(c => Comment(c));

        /// <summary>
        /// Create a Section-token. "[Section]"
        /// </summary>
        /// <param name="section">Section name without escaping characters. Use any characters.</param>
        /// <returns>token</returns>
        public static IniToken Section(string section)
        {
            string str = "[" + IniEscape.Section.EscapeLiteral(section) + "]\r\n";
            return new IniToken { Type = IniTokenType.Section, Source = str, Index = 0, Length = str.Length, ValueIndex = 1, ValueLength = section.Length };
        }

        /// <summary>
        /// Create a Section-token. "[Section]"
        /// </summary>
        /// <param name="section">Section name with escaping characters</param>
        /// <returns>token</returns>
        public static IniToken SectionRaw(string section)
        {
            string str = "[" + section + "]\r\n";
            return new IniToken { Type = IniTokenType.Section, Source = str, Index = 0, Length = str.Length, ValueIndex = 1, ValueLength = section.Length };
        }

        /// <summary>
        /// Create a KeyValue-token. "key = value"
        /// </summary>
        /// <param name="key">Unescaped value. Use any characters.</param>
        /// <param name="value">Unescaped value. Use any characters.</param>
        /// <returns>a new token with a modified key</returns>
        public static IniToken KeyValue(string key, string value)
        {
            string str = IniEscape.Key.EscapeLiteral(key) + " = " + IniEscape.Value.EscapeLiteral(value) + "\r\n";
            return new IniToken { Type = IniTokenType.KeyValue, Source = str, Index = 0, Length = str.Length, ValueIndex = key.Length + 3, ValueLength = value.Length, KeyIndex = 0, KeyLength = key.Length };
        }


        /// <summary>
        /// Create a KeyValue-token. "key = value"
        /// </summary>
        /// <param name="key">Raw escaped value. Use any characters.</param>
        /// <param name="value">Raw escaped value. Use any characters.</param>
        /// <returns>a new token with a modified key</returns>
        public static IniToken KeyValueRaw(string key, string value)
        {
            string str = key + " = " + value + "\r\n";
            return new IniToken { Type = IniTokenType.KeyValue, Source = str, Index = 0, Length = str.Length, ValueIndex = key.Length + 3, ValueLength = value.Length, KeyIndex = 0, KeyLength = key.Length };
        }

        /// <summary>
        /// Create a new token where the value part has been modified to <paramref name="newValue"/>.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>a new token with modified value</returns>
        public IniToken CreateWithNewValue(string newValue)
        {
            if (Type == IniTokenType.Text) return Text(newValue);

            // Escape
            if (Type == IniTokenType.KeyValue) newValue = IniEscape.Value.EscapeLiteral(newValue);
            else if (Type == IniTokenType.Section) newValue = IniEscape.Section.EscapeLiteral(newValue);
            else if (Type == IniTokenType.Comment) newValue = IniEscape.Comment.EscapeLiteral(newValue);

            // Lengths before and after "value" part.
            int len1 = ValueIndex - Index, len2 = newValue == null ? 0 : newValue.Length, len3 = Index + Length - ValueIndex - ValueLength;
            StringBuilder sb = new StringBuilder(len1 + len2 + len3 + 2);
            if (len1 > 0) sb.Append(Source.Substring(Index, len1));
            if (len2 > 0) sb.Append(newValue);
            if (len3 > 0) sb.Append(Source.Substring(ValueIndex + ValueLength, len3));
            //sb.Append("\r\n");
            string str = sb.ToString();
            return new IniToken
            {
                Type = Type,
                Source = str,
                Index = 0,
                Length = str.Length,
                ValueIndex = len1,
                ValueLength = len2,
                KeyIndex = KeyIndex - Index,
                KeyLength = KeyLength
            };
        }

        /// <summary>
        /// Create a Text-token.
        /// </summary>
        /// <param name="text">text</param>
        /// <returns></returns>
        public static IniToken Text(string text)
            => new IniToken { Type = IniTokenType.Text, Source = text, Index = 0, Length = text.Length, ValueIndex = 0, ValueLength = text.Length };

        /// <summary>
        /// Token Type
        /// </summary>
        public IniTokenType Type = IniTokenType.Text;

        /// <summary>
        /// Previous token node in linked list.
        /// </summary>
        public IniToken Previous;

        /// <summary>
        /// Next token node in linked list.
        /// </summary>
        public IniToken Next;

        /// <summary>
        /// Content start index and length in iniString.
        /// </summary>
        public int Index, Length;

        /// <summary>
        /// Key start index and length in iniString.
        /// </summary>
        public int KeyIndex, KeyLength;

        /// <summary>
        /// Value start index and length in iniString.
        /// </summary>
        public int ValueIndex, ValueLength;

        /// <summary>
        /// Reference to the original ini source string.
        /// </summary>
        public string Source;

        /// <summary>
        /// The string that makes the content for this token.
        /// </summary>
        public string Content
            => Index >= 0 && Length >= 0 && Source != null ? Source.Substring(Index, Length) : null;

        /// <summary>
        /// Value in raw format with escape characters.
        /// </summary>
        public string ValueText
            => ValueIndex >= 0 && ValueLength >= 0 && Source != null ? Source.Substring(ValueIndex, ValueLength) : null;

        /// <summary>
        /// Value in unescaped format.
        /// </summary>
        public string Value
        {
            get => IniEscape.Value.UnescapeLiteral(ValueText);
            set => ReadFrom(CreateWithNewValue(value));
        }

        /// <summary>
        /// Key in escaped format.
        /// </summary>
        public string KeyText
            => KeyIndex >= 0 && KeyLength >= 0 ? Source.Substring(KeyIndex, KeyLength) : null;

        /// <summary>
        /// Key in unescaped format.
        /// </summary>
        public string Key
            => IniEscape.Key.UnescapeLiteral(KeyText);

        /// <summary>
        /// Print content
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Content;

        /// <summary>
        /// Clone token
        /// </summary>
        /// <returns></returns>
        public object Clone()
            => new IniToken { Source = Source, Type = Type, Index = Index, Length = Length, ValueIndex = ValueIndex, ValueLength = ValueLength, KeyIndex = KeyIndex, KeyLength = KeyLength };

        /// <summary>
        /// Copy from <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        public void ReadFrom(IniToken other)
        {
            this.Source = other.Source;
            this.Type = other.Type;
            this.Index = other.Index;
            this.Length = other.Length;
            this.ValueIndex = other.ValueIndex;
            this.ValueLength = other.ValueLength;
            this.KeyIndex = other.KeyIndex;
            this.KeyLength = other.KeyLength;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            IniToken other = obj as IniToken;
            if (other == null) return false;
            return other.Type == Type && other.Content == Content;
        }

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 900169;
            if (Source != null)
            {
                for (int i = Index + Length - 1; i >= Index; i--)
                    hash = hash * 999961 + Source[i];
            }
            hash = hash * 999961 + (KeyIndex - Index);
            hash = hash * 999961 + KeyLength;
            return hash;
        }

        /// <summary>
        /// If tokens are built as linked list, returns an <see cref="IEnumerable{IniToken}"/> that lists
        /// this and other succeeding nodes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IniToken> LinkedList()
        {
            for (IniToken t = this; t != null; t = t.Next)
                yield return t;
        }

        /// <summary>
        /// Insert <paramref name="token"/> before self.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>token</returns>
        public IniToken InsertBefore(IniToken token)
        {
            IniToken x = this.Previous;
            this.Previous = token;
            if (x != null) x.Next = token;
            token.Next = this;
            token.Previous = x;
            return token;
        }

        /// <summary>
        /// Insert <paramref name="token"/> after self.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>self</returns>
        public IniToken InsertAfter(IniToken token)
        {
            IniToken x = this.Next;
            this.Next = token;
            if (x != null) x.Previous = token;
            token.Previous = this;
            token.Next = x;
            return this;
        }

        /// <summary>
        /// Remove self from linked list.
        /// </summary>
        public void Remove()
        {
            if (Previous != null) Previous.Next = Next;
            if (Next != null) Next.Previous = Previous;
        }

        /// <summary>
        /// If tokens are built as linked list, returns an <see cref="IEnumerable{IniToken}"/> that lists
        /// this and other succeeding nodes.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IniToken> GetEnumerator()
            => LinkedList().GetEnumerator();

        /// <summary>
        /// If tokens are built as linked list, returns an <see cref="IEnumerable{IniToken}"/> that lists
        /// this and other succeeding nodes.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => LinkedList().GetEnumerator();

    }

    /// <summary>
    /// Class that reads text source and enumerates <see cref="IniToken"/>s.
    /// </summary>
    public class IniTokenizer : IEnumerable<IniToken>, IDisposable
    {
        /// <summary>
        /// Read tokenizer from a text file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IniTokenizer ReadTextFile(string filepath, Encoding encoding = default)
        {
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs, encoding ?? Encoding.UTF8, true, 32 * 1024))
                return new IniTokenizer(sr.ReadToEnd());
        }

        static Regex parser = new Regex(
            @"(\[(?<section>(\\[^\r\n]|[^\]\n\r\\])*)\])|((;|#|//)(?<comment>[^\r\n]*))|((?<key>(\\[^\r\n]|[^;#=\\ \t\x0B\f\r\n])+)[ \t\x0B\f]*=[ \t\x0B\f]*(?<value>(\\[^\r\n]|[^\\\n\r])*))|(?<text>.+?)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        /// <summary>
        /// Matches from Regex
        /// </summary>
        protected MatchCollection matches;

        /// <summary>
        /// Source text
        /// </summary>
        protected string text;

        /// <summary>
        /// Create tokenizer that parses <paramref name="text"/> into tokens.
        /// </summary>
        /// <param name="text"></param>
        public IniTokenizer(string text)
        {
            matches = parser.Matches(text);
            this.text = text;
        }

        /// <summary>
        /// Dispose enumerable
        /// </summary>
        public void Dispose()
        {
            matches = null;
        }

        /// <summary>
        /// Reads all tokens into an separate instance. 
        /// </summary>
        /// <returns>Head token or null if contained no tokens</returns>
        public IniToken ToLinkedList()
        {
            IniToken head = null, prev = null;
            foreach (IniToken t in this)
            {
                // Clone single instance
                IniToken token = t.Clone() as IniToken;
                // Set head
                if (head == null) head = token;
                // Set link
                if (prev != null) { prev.Next = token; token.Previous = prev; }
                // Set prev
                prev = token;
            }
            return head;
        }

        /// <summary>
        /// Returns an enumerator that iterates tokens but uses only one <see cref="IniToken"/> instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IniToken> GetEnumerator()
        {
            if (matches == null) throw new ObjectDisposedException(nameof(IniTokenizer));
            return new IniTokenEnumerator(text, matches);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (matches == null) throw new ObjectDisposedException(nameof(IniTokenizer));
            return new IniTokenEnumerator(text, matches);
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return text;
        }
    }

    class IniTokenEnumerator : IEnumerator, IEnumerator<IniToken>
    {
        /// <summary>
        /// Source text
        /// </summary>
        private string text;

        /// <summary>
        /// Matches
        /// </summary>
        private MatchCollection matches;

        /// <summary>
        /// Number of tokens
        /// </summary>
        public int Count;

        /// <summary>
        /// Pointer to the cell at current pointer.
        /// </summary>
        private int ix = -1;

        /// <summary>
        /// Current token
        /// </summary>
        private IniToken current;

        /// <summary>
        /// Current token has been read.
        /// </summary>
        private bool currentLineRead;

        public IniTokenEnumerator(string iniText, MatchCollection a_matches)
        {
            text = iniText;
            matches = a_matches;
            Count = matches.Count;
        }

        public bool MoveNext()
        {
            currentLineRead = false;

            if (ix >= Count - 1) return false;

            // Move to next token.
            Match m = matches[++ix];

            return true;
        }

        public void Reset()
        {
            current = null;
            ix = -1;
        }

        object IEnumerator.Current
            => currentLineRead ? current : ReadCurrentToken();

        IniToken IEnumerator<IniToken>.Current
            => currentLineRead ? current : ReadCurrentToken();

        public IniToken ReadCurrentToken()
        {
            currentLineRead = true;
            if (ix < 0 || ix >= Count) return current = null;
            if (current == null) current = new IniToken();
            current.Source = text;
            Match m = matches[ix], next = ix + 1 < Count ? matches[ix + 1] : null;
            current.Index = m.Index;
            current.Length = m.Length;

            if (next != null && m.Index + m.Length < next.Index) current.Length = next.Index - m.Index;

            Group g = null, g2 = null;

            // Section
            if ((g = m.Groups[1]).Success)
            {
                current.Type = IniTokenType.Section;
                current.KeyIndex = -1;
                current.KeyLength = -1;
                current.ValueIndex = g.Index;
                current.ValueLength = g.Length;
            }

            // Comment
            else if ((g = m.Groups[2]).Success)
            {
                current.Type = IniTokenType.Comment;
                current.KeyIndex = -1;
                current.KeyLength = -1;
                current.ValueIndex = g.Index;
                current.ValueLength = g.Length;
            }

            // KeyValue
            else if ((g = m.Groups[3]).Success && (g2 = m.Groups[4]).Success)
            {
                current.Type = IniTokenType.KeyValue;
                current.KeyIndex = g.Index;
                current.KeyLength = g.Length;
                current.ValueIndex = g2.Index;
                current.ValueLength = g2.Length;
            }
            else

            // Text
            if ((g = m.Groups[5]).Success)
            {
                g = m.Groups[5];
                current.Type = IniTokenType.Text;
                current.ValueIndex = g.Index;
                current.ValueLength = g.Length;
                current.KeyIndex = -1;
                current.KeyLength = -1;
            }
            else
            {
                current.Type = IniTokenType.Text;
                current.ValueIndex = -1;
                current.ValueLength = -1;
                current.KeyIndex = -1;
                current.KeyLength = -1;
            }

            return current;
        }

        public virtual void Dispose()
        {
            current = null;
            matches = null;
            ix = Count = 0;
        }
    }
    
    /// <summary>
    /// Handles escaping of ini files.
    /// </summary>
    public class IniEscape
    {
        static RegexOptions opts = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;
        static IniEscape comment = new IniEscape("\n\t\r\0\a\b\f");
        static IniEscape section = new IniEscape("\n\t\r\0\a\b\f[]");
        static IniEscape key = new IniEscape("\n\t\r\0\a\b\f=");
        static IniEscape value = new IniEscape("\n\t\r\0\a\b\f");

        /// <summary>
        /// Escape rules for comment text.
        /// </summary>
        public static IniEscape Comment => comment;

        /// <summary>
        /// Escapre rules for section text.
        /// </summary>
        public static IniEscape Section => section;

        /// <summary>
        /// Escape rules for key token.
        /// </summary>
        public static IniEscape Key => key;

        /// <summary>
        /// Escape rules for value token.
        /// </summary>
        public static IniEscape Value => value;

        /// <summary>
        /// Pattern for escaping text
        /// </summary>
        Regex LiteralEscape;

        /// <summary>
        /// Pattern for unescaping text
        /// </summary>
        Regex LiteralUnescape;

        /// <summary>
        /// Match evaluator delegates
        /// </summary>
        MatchEvaluator escapeChar, unescapeChar;

        /// <summary>
        /// Create new escaper
        /// </summary>
        /// <param name="escapeCharacters">list of characters that are to be escaped</param>
        public IniEscape(string escapeCharacters)
        {
            // Regex.Escape doen't work for brackets []
            //string escapeCharactersEscaped = Regex.Escape(escapeCharacters);
            string escapeCharactersEscaped = escapeCharacters.Select(c => c == ']' ? "\\]" : Regex.Escape("" + c)).Aggregate((a, b) => a + b);
            LiteralEscape = new Regex("[" + escapeCharactersEscaped + "]|(\\\\\\\\)", opts);
            LiteralUnescape = new Regex(@"\\[" + escapeCharactersEscaped + "]", opts);
            escapeChar = EscapeChar;
            unescapeChar = UnescapeChar;
            
        }

        /// <summary>
        /// Escape <paramref name="input"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Escaped text</returns>
        public String EscapeLiteral(String input) => input == null ? null : LiteralEscape.Replace(input, escapeChar);

        /// <summary>
        /// Unescape <paramref name="input"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Unescaped text</returns>
        public String UnescapeLiteral(String input) => input == null ? null : LiteralUnescape.Replace(input, unescapeChar);

        /// <summary>
        /// Function that escapes <paramref name="m"/>.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static String EscapeChar(Match m)
        {
            if (m.Value == "\\\\") return m.Value;
            char _ch = m.Value[0];
            switch (_ch)
            {
                case '\0': return "\\0";
                case '\a': return "\\a";
                case '\b': return "\\b";
                case '\t': return "\\t";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                default:
                    return "\\" + _ch;
            }
        }

        /// <summary>
        /// Function that unescapes <paramref name="m"/>.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static String UnescapeChar(Match m)
        {
            string capture = m.Value;
            char _ch = capture[1];
            switch (_ch)
            {
                case '0': return "\0";
                case 'a': return "\a";
                case 'b': return "\b";
                case 't': return "\t";
                case 'f': return "\f";
                case 'n': return "\n";
                case 'r': return "\r";
                case 'u':
                    char ch = (char)Hex.ToUInt(capture, 2);
                    return new string(ch, 1);
                case 'x':
                    char c = (char)Hex.ToUInt(capture, 2);
                    return new string(c, 1);
                default:
                    return new string(_ch, 1);
            }
        }
    }


}
